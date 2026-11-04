<#
.SYNOPSIS
Builds a self-contained Windows x64 release package and its SHA-256 checksum.

.DESCRIPTION
Runs the xUnit suite, publishes XmlEditorReloaded.App with the .NET runtime bundled, copies the
release documents into the package, creates a portable ZIP with a single top-level folder, and writes
a SHA-256 checksum next to the archive.

The version is read from xml-editor-reloaded/Directory.Build.props. Passing -Version is optional and
is rejected when it does not match that value, so a release tag cannot silently package another build.

.PARAMETER Version
Expected release version, for example 1.0.0. Defaults to the version in Directory.Build.props.

.PARAMETER Runtime
Runtime identifier to publish. The editor currently ships for win-x64 only.

.PARAMETER OutputDirectory
Directory for the ZIP and checksum. Defaults to out/release.

.PARAMETER SkipTests
Skip the xUnit suite. Intended for a caller that has already run the tests in the same build.

.EXAMPLE
./util-scripts/release.ps1

.EXAMPLE
./util-scripts/release.ps1 -Version 1.0.0 -OutputDirectory out/release

.NOTES
The script never writes to References/. Build output stays under out/. Exit code is zero only when
every step succeeds.
#>
[CmdletBinding()]
param(
    [string]$Version,
    [ValidateSet('win-x64')]
    [string]$Runtime = 'win-x64',
    [string]$OutputDirectory,
    [switch]$SkipTests
)

$ErrorActionPreference = 'Stop'

$repo = Split-Path -Parent $PSScriptRoot
$root = Join-Path $repo 'xml-editor-reloaded'
$project = Join-Path $root 'src\XmlEditorReloaded.App\XmlEditorReloaded.App.csproj'
$propsPath = Join-Path $root 'Directory.Build.props'
$testScript = Join-Path $PSScriptRoot 'test.ps1'

if (-not (Test-Path -LiteralPath $project)) {
    throw "Application project not found at '$project'."
}

if (-not (Test-Path -LiteralPath $propsPath)) {
    throw "Directory.Build.props not found at '$propsPath'."
}

[xml]$props = Get-Content -LiteralPath $propsPath -Raw
$propsVersion = [string]($props.SelectNodes('//Version') | Select-Object -First 1).'#text'
if ($propsVersion -notmatch '^\d+\.\d+\.\d+$') {
    throw "Directory.Build.props must contain a three-part Version; found '$propsVersion'."
}

if ([string]::IsNullOrWhiteSpace($Version)) {
    $Version = $propsVersion
}
elseif ($Version -ne $propsVersion) {
    throw "Requested version '$Version' does not match Directory.Build.props version '$propsVersion'."
}

if ($Version -notmatch '^\d+\.\d+\.\d+$') {
    throw "Version '$Version' must contain three numeric parts, for example 1.0.0."
}

if ([string]::IsNullOrWhiteSpace($OutputDirectory)) {
    $OutputDirectory = Join-Path $repo 'out\release'
}
elseif (-not [System.IO.Path]::IsPathRooted($OutputDirectory)) {
    $OutputDirectory = Join-Path $repo $OutputDirectory
}

$dotnet = (Get-Command dotnet -ErrorAction Stop).Source
$pwsh = (Get-Command pwsh -ErrorAction Stop).Source
$publishDir = Join-Path $repo "out\publish\$Runtime"
$stageRoot = Join-Path $repo 'out\release-stage'
$packageName = "XmlEditorReloaded-$Version-$Runtime"
$stageDir = Join-Path $stageRoot $packageName
$zipPath = Join-Path $OutputDirectory "$packageName.zip"
$hashPath = "$zipPath.sha256"

function Invoke-Native {
    param(
        [Parameter(Mandatory)]
        [string]$FilePath,
        [Parameter(Mandatory)]
        [string[]]$Arguments,
        [Parameter(Mandatory)]
        [string]$Description
    )

    & $FilePath @Arguments
    if ($LASTEXITCODE -ne 0) {
        throw "$Description failed with exit code $LASTEXITCODE."
    }
}

function Remove-DirectoryIfPresent {
    param([Parameter(Mandatory)][string]$Path)

    if (Test-Path -LiteralPath $Path) {
        Remove-Item -LiteralPath $Path -Recurse -Force
    }
}

Write-Host "XML Editor Reloaded $Version ($Runtime)" -ForegroundColor Cyan

if (-not $SkipTests) {
    Write-Host 'Running the xUnit suite...' -ForegroundColor Cyan
    & $pwsh -NoProfile -ExecutionPolicy Bypass -File $testScript -Configuration Release
    if ($LASTEXITCODE -ne 0) {
        throw "Tests failed with exit code $LASTEXITCODE."
    }
}
else {
    Write-Host 'Skipping the xUnit suite (-SkipTests).' -ForegroundColor Yellow
}

Remove-DirectoryIfPresent -Path $publishDir
Remove-DirectoryIfPresent -Path $stageRoot
New-Item -ItemType Directory -Path $publishDir -Force | Out-Null
New-Item -ItemType Directory -Path $stageDir -Force | Out-Null
New-Item -ItemType Directory -Path $OutputDirectory -Force | Out-Null

$publishArguments = @(
    'publish', $project,
    '--configuration', 'Release',
    '--runtime', $Runtime,
    '--self-contained', 'true',
    '--output', $publishDir,
    '-p:PublishSingleFile=false',
    '-p:PublishTrimmed=false',
    '-p:PublishReadyToRun=false',
    '-p:DebugType=None',
    '-p:DebugSymbols=false',
    '-p:IncludeSourceRevisionInInformationalVersion=false',
    '-m:1'
)

Write-Host 'Publishing the self-contained application...' -ForegroundColor Cyan
Invoke-Native -FilePath $dotnet -Arguments $publishArguments -Description 'dotnet publish'

$executable = Join-Path $publishDir 'XmlEditorReloaded.App.exe'
if (-not (Test-Path -LiteralPath $executable)) {
    throw "Published executable not found at '$executable'."
}

if (-not (Test-Path -LiteralPath (Join-Path $publishDir 'e_sqlite3.dll'))) {
    throw 'The published output does not contain e_sqlite3.dll.'
}

$fileVersion = [System.Diagnostics.FileVersionInfo]::GetVersionInfo($executable)
if ($fileVersion.FileVersion -notlike "$Version.*") {
    throw "Published file version '$($fileVersion.FileVersion)' does not match '$Version'."
}

Get-ChildItem -LiteralPath $publishDir -Force |
    Copy-Item -Destination $stageDir -Recurse -Force

$documents = @(
    @{ Source = (Join-Path $root 'README.md'); Target = 'README.md' },
    @{ Source = (Join-Path $root 'USER-GUIDE.md'); Target = 'USER-GUIDE.md' },
    @{ Source = (Join-Path $root 'CHANGELOG.md'); Target = 'CHANGELOG.md' },
    @{ Source = (Join-Path $root 'LICENSE'); Target = 'LICENSE' },
    @{ Source = (Join-Path $root 'THIRD-PARTY-NOTICES.md'); Target = 'THIRD-PARTY-NOTICES.md' },
    @{ Source = (Join-Path $root 'packaging\start-portable.cmd'); Target = 'start-portable.cmd' }
)

foreach ($document in $documents) {
    if (-not (Test-Path -LiteralPath $document.Source)) {
        throw "Release document not found at '$($document.Source)'."
    }

    Copy-Item -LiteralPath $document.Source -Destination (Join-Path $stageDir $document.Target) -Force
}

$licensesSource = Join-Path $root 'licenses'
if (-not (Test-Path -LiteralPath $licensesSource)) {
    throw "License directory not found at '$licensesSource'."
}

Copy-Item -LiteralPath $licensesSource -Destination (Join-Path $stageDir 'licenses') -Recurse -Force

$pdbFiles = Get-ChildItem -LiteralPath $stageDir -Filter '*.pdb' -Recurse -File
if ($pdbFiles.Count -gt 0) {
    throw "The release package must not contain PDB files; found '$($pdbFiles[0].FullName)'."
}

if (Test-Path -LiteralPath $zipPath) {
    Remove-Item -LiteralPath $zipPath -Force
}

Add-Type -AssemblyName System.IO.Compression.FileSystem
[System.IO.Compression.ZipFile]::CreateFromDirectory(
    $stageRoot,
    $zipPath,
    [System.IO.Compression.CompressionLevel]::Optimal,
    $false)

$archive = [System.IO.Compression.ZipFile]::OpenRead($zipPath)
try {
    $entryNames = @($archive.Entries | ForEach-Object FullName)
    $expectedEntry = "$packageName/XmlEditorReloaded.App.exe"
    if ($entryNames -notcontains $expectedEntry) {
        throw "The archive does not contain '$expectedEntry'."
    }
}
finally {
    $archive.Dispose()
}

$hash = (Get-FileHash -LiteralPath $zipPath -Algorithm SHA256).Hash.ToLowerInvariant()
$zipName = [System.IO.Path]::GetFileName($zipPath)
Set-Content -LiteralPath $hashPath -Value "$hash  $zipName" -Encoding ascii

$sizeMb = [math]::Round((Get-Item -LiteralPath $zipPath).Length / 1MB, 1)
$fileCount = (Get-ChildItem -LiteralPath $stageDir -Recurse -File).Count

Write-Host ''
Write-Host "Package:  $zipPath" -ForegroundColor Green
Write-Host "Checksum: $hashPath" -ForegroundColor Green
Write-Host "Files:    $fileCount"
Write-Host "Size:     $sizeMb MB"
Write-Host "SHA-256:  $hash"
