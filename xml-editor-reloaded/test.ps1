<#
.SYNOPSIS
Builds the solution and runs every test project.

.DESCRIPTION
Runs the xUnit projects one after another. The solution is built with a single
MSBuild node because confined environments (the DSH sandbox) deny the named
pipes MSBuild worker nodes use; a normal `dotnet test` on the solution therefore
hangs there. Outside such an environment the stock commands remain valid.

.PARAMETER Configuration
Build configuration passed to both build and test. Defaults to Debug.

.PARAMETER Filter
Optional xUnit/VSTest filter, for example "Category=Performance".

.PARAMETER NoBuild
Skip the build and test the already compiled assemblies.

.EXAMPLE
./test.ps1

.EXAMPLE
./test.ps1 -Filter "FullyQualifiedName~TableValidator"

.NOTES
Exit code is zero only when the build and all test projects succeed.
#>
[CmdletBinding()]
param(
    [string]$Configuration = 'Debug',
    [string]$Filter,
    [switch]$NoBuild
)

$ErrorActionPreference = 'Stop'
$root = $PSScriptRoot
$solution = Join-Path $root 'XmlEditorReloaded.slnx'
$testsDirectory = Join-Path $root 'tests'

# The App tests create their workspaces under %TEMP%\xer-app-tests. A shared
# temp tree accumulates thousands of leftovers and eventually makes directory
# creation fail, so give every run its own clean root.
$tempRoot = Join-Path $root ('.tmp\run-' + [guid]::NewGuid().ToString('N'))
New-Item -ItemType Directory -Path $tempRoot -Force | Out-Null
$env:TEMP = $tempRoot
$env:TMP = $tempRoot

if (-not $NoBuild) {
    Write-Host "Restoring $solution (single MSBuild node)..." -ForegroundColor Cyan
    dotnet msbuild $solution -t:Restore -m:1
    if ($LASTEXITCODE -ne 0) {
        exit $LASTEXITCODE
    }

    Write-Host "Building $solution ($Configuration, single MSBuild node)..." -ForegroundColor Cyan
    dotnet build $solution --configuration $Configuration -m:1 --no-restore
    if ($LASTEXITCODE -ne 0) {
        exit $LASTEXITCODE
    }
}

$projects = Get-ChildItem $testsDirectory -Filter '*.csproj' -Recurse | Sort-Object Name
$failed = @()

foreach ($project in $projects) {
    Write-Host "Testing $($project.BaseName)..." -ForegroundColor Cyan
    $testArguments = @(
        'test', $project.FullName,
        '--configuration', $Configuration,
        '--no-build', '--no-restore'
    )
    if ($Filter) {
        $testArguments += @('--filter', $Filter)
    }
    dotnet @testArguments
    if ($LASTEXITCODE -ne 0) {
        $failed += $project.BaseName
    }
}

if ($failed.Count -gt 0) {
    Write-Host "Failed projects: $($failed -join ', ')" -ForegroundColor Red
    exit 1
}

Write-Host "All $($projects.Count) test projects passed." -ForegroundColor Green
