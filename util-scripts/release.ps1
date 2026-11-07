<#
.SYNOPSIS
Builds self-contained and framework-dependent Windows x64 release packages and their SHA-256 checksums.

.DESCRIPTION
Runs the xUnit suite, publishes XmlEditorReloaded.App twice: once with the .NET runtime bundled and
once framework-dependent (it then requires the .NET 10 Desktop Runtime), copies the release documents
into each package, creates a ZIP with a single top-level folder per flavor, and writes a SHA-256
checksum next to each archive.

The version is read from xml-editor-reloaded/Directory.Build.props. Passing -Version is optional and
is rejected when it does not match that value, so a release tag cannot silently package another build.

.PARAMETER Version
Expected release version, for example 1.0.0. Defaults to the version in Directory.Build.props.

.PARAMETER Runtime
Runtime identifier to publish. The editor currently ships for win-x64 only.

.PARAMETER OutputDirectory
Directory for the ZIPs and checksums. Defaults to out/release.

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
$stageRoot = Join-Path $repo 'out\release-stage'

function Invoke-Native {
    param(
        [Parameter(Mandatory)]
        [string]$FilePath,
        [Parameter(Mandatory)]
        [string[]]$Arguments,
        [Parameter(Mandatory)]
        [string]$Description
    )

    & $FilePath @Arguments | Out-Host
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

Remove-DirectoryIfPresent -Path $stageRoot
New-Item -ItemType Directory -Path $OutputDirectory -Force | Out-Null

Add-Type -AssemblyName System.IO.Compression.FileSystem

$documents = @(
    @{ Source = (Join-Path $root 'README.md'); Target = 'README.md' },
    @{ Source = (Join-Path $root 'USER-GUIDE.md'); Target = 'USER-GUIDE.md' },
    @{ Source = (Join-Path $root 'USER-GUIDE-RU.md'); Target = 'USER-GUIDE-RU.md' },
    @{ Source = (Join-Path $root 'CHANGELOG.md'); Target = 'CHANGELOG.md' },
    @{ Source = (Join-Path $root 'LICENSE'); Target = 'LICENSE' },
    @{ Source = (Join-Path $root 'THIRD-PARTY-NOTICES.md'); Target = 'THIRD-PARTY-NOTICES.md' },
    @{ Source = (Join-Path $root 'packaging\start-portable.cmd'); Target = 'start-portable.cmd' }
)

$licensesSource = Join-Path $root 'licenses'
if (-not (Test-Path -LiteralPath $licensesSource)) {
    throw "License directory not found at '$licensesSource'."
}

function New-ReleasePackage {
    param(
        [Parameter(Mandatory)][string]$Flavor,
        [Parameter(Mandatory)][AllowEmptyString()][string]$PackageSuffix,
        [Parameter(Mandatory)][string]$PublishDirectory,
        [Parameter(Mandatory)][bool]$SelfContained,
        [Parameter(Mandatory)][string]$Description
    )

    $packageName = "XmlEditorReloaded-$Version-$Runtime$PackageSuffix"
    $flavorStageRoot = Join-Path $stageRoot $Flavor
    $stageDir = Join-Path $flavorStageRoot $packageName
    $zipPath = Join-Path $OutputDirectory "$packageName.zip"
    $hashPath = "$zipPath.sha256"

    Remove-DirectoryIfPresent -Path $PublishDirectory
    Remove-DirectoryIfPresent -Path $flavorStageRoot
    New-Item -ItemType Directory -Path $PublishDirectory -Force | Out-Null
    New-Item -ItemType Directory -Path $stageDir -Force | Out-Null

    $publishArguments = @(
        'publish', $project,
        '--configuration', 'Release',
        '--runtime', $Runtime,
        '--self-contained', $(if ($SelfContained) { 'true' } else { 'false' }),
        '--output', $PublishDirectory,
        '-p:PublishSingleFile=false',
        '-p:PublishTrimmed=false',
        '-p:PublishReadyToRun=false',
        '-p:DebugType=None',
        '-p:DebugSymbols=false',
        '-p:IncludeSourceRevisionInInformationalVersion=false',
        '-m:1'
    )

    Write-Host "Publishing the $Description application..." -ForegroundColor Cyan
    Invoke-Native -FilePath $dotnet -Arguments $publishArguments -Description "dotnet publish ($Description)"

    $executable = Join-Path $PublishDirectory 'XmlEditorReloaded.App.exe'
    if (-not (Test-Path -LiteralPath $executable)) {
        throw "Published executable not found at '$executable'."
    }

    if (-not (Test-Path -LiteralPath (Join-Path $PublishDirectory 'e_sqlite3.dll'))) {
        throw "The $Description output does not contain e_sqlite3.dll."
    }

    if ($SelfContained) {
        if (-not (Test-Path -LiteralPath (Join-Path $PublishDirectory 'coreclr.dll'))) {
            throw 'The self-contained output does not contain coreclr.dll.'
        }
    }
    else {
        $bundledRuntime = Get-ChildItem -LiteralPath $PublishDirectory -Recurse -File |
            Where-Object { $_.Name -in @('coreclr.dll', 'hostfxr.dll', 'hostpolicy.dll') }
        if ($bundledRuntime) {
            throw "The framework-dependent output contains runtime files; first: '$($bundledRuntime[0].FullName)'."
        }
    }

    $fileVersion = [System.Diagnostics.FileVersionInfo]::GetVersionInfo($executable)
    if ($fileVersion.FileVersion -notlike "$Version.*") {
        throw "Published file version '$($fileVersion.FileVersion)' does not match '$Version'."
    }

    Get-ChildItem -LiteralPath $PublishDirectory -Force |
        Copy-Item -Destination $stageDir -Recurse -Force |
        Out-Null

    foreach ($document in $documents) {
        if (-not (Test-Path -LiteralPath $document.Source)) {
            throw "Release document not found at '$($document.Source)'."
        }

        Copy-Item -LiteralPath $document.Source -Destination (Join-Path $stageDir $document.Target) -Force |
            Out-Null
    }

    Copy-Item -LiteralPath $licensesSource -Destination (Join-Path $stageDir 'licenses') -Recurse -Force |
        Out-Null

    $pdbFiles = Get-ChildItem -LiteralPath $stageDir -Filter '*.pdb' -Recurse -File
    if ($pdbFiles.Count -gt 0) {
        throw "The release package must not contain PDB files; found '$($pdbFiles[0].FullName)'."
    }

    if (Test-Path -LiteralPath $zipPath) {
        Remove-Item -LiteralPath $zipPath -Force
    }

    [System.IO.Compression.ZipFile]::CreateFromDirectory(
        $flavorStageRoot,
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

    return [pscustomobject]@{
        Flavor = $Flavor
        PackageName = $packageName
        Zip = $zipPath
        HashFile = $hashPath
        Files = (Get-ChildItem -LiteralPath $stageDir -Recurse -File).Count
        SizeMb = [math]::Round((Get-Item -LiteralPath $zipPath).Length / 1MB, 1)
        Sha256 = $hash
    }
}

$packages = @(
    New-ReleasePackage `
        -Flavor 'self-contained' `
        -PackageSuffix '' `
        -PublishDirectory (Join-Path $repo "out\publish\$Runtime") `
        -SelfContained $true `
        -Description 'self-contained'
    New-ReleasePackage `
        -Flavor 'framework-dependent' `
        -PackageSuffix '-framework-dependent' `
        -PublishDirectory (Join-Path $repo "out\publish\$Runtime-framework-dependent") `
        -SelfContained $false `
        -Description 'framework-dependent'
)

Write-Host ''
foreach ($package in $packages) {
    Write-Host "Package ($($package.Flavor)): $($package.Zip)" -ForegroundColor Green
    Write-Host "Checksum: $($package.HashFile)" -ForegroundColor Green
    Write-Host "Files:    $($package.Files)"
    Write-Host "Size:     $($package.SizeMb) MB"
    Write-Host "SHA-256:  $($package.Sha256)"
    Write-Host ''
}
