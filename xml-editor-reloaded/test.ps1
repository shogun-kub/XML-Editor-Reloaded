<#
.SYNOPSIS
Builds the solution and runs every test project.

.DESCRIPTION
Runs the xUnit projects one after another. The solution is built with a single
MSBuild node because confined environments (the DSH sandbox) deny the named
pipes MSBuild worker nodes use; a normal `dotnet test` on the solution therefore
hangs there. Outside such an environment the stock commands remain valid.

Every run uses its own temporary root under `.tmp` because the App tests create
workspaces in `%TEMP%`; a shared tree accumulates copies of the game data. Old
`run-*` roots are removed before the run starts. A successful run removes its
own root, so `.tmp` stays empty unless a run fails; the failed run is kept for
diagnostics (at most one) and is removed by the next run.

.PARAMETER Configuration
Build configuration passed to both build and test. Defaults to Debug.

.PARAMETER Filter
Optional xUnit/VSTest filter, for example "Category=Performance".

.PARAMETER NoBuild
Skip the build and test the already compiled assemblies.

.PARAMETER KeepTemp
Keep the temporary root after a successful run, for example to inspect copied
corpora. Failed runs are always kept.

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
    [switch]$NoBuild,
    [switch]$KeepTemp
)

$ErrorActionPreference = 'Stop'
$root = $PSScriptRoot
$solution = Join-Path $root 'XmlEditorReloaded.slnx'
$testsDirectory = Join-Path $root 'tests'
$tmpBase = Join-Path $root '.tmp'

function Remove-TestTemp {
    param([string]$Path)

    for ($attempt = 1; $attempt -le 3; $attempt++) {
        if (-not (Test-Path -LiteralPath $Path)) {
            return
        }

        try {
            Remove-Item -LiteralPath $Path -Recurse -Force -ErrorAction Stop
            return
        }
        catch {
            if ($attempt -eq 3) {
                Write-Warning "Could not remove the temporary test directory '$Path': $($_.Exception.Message)"
                return
            }

            Start-Sleep -Milliseconds 300
        }
    }
}

# Remove the roots of previous runs before creating a new one, so a failing run can never leave more
# than one diagnostic copy of the corpus behind. Only run-* directories belong to this script.
if (Test-Path -LiteralPath $tmpBase) {
    Get-ChildItem -LiteralPath $tmpBase -Directory -Filter 'run-*' -ErrorAction SilentlyContinue |
        ForEach-Object { Remove-TestTemp -Path $_.FullName }
}

$tempRoot = Join-Path $tmpBase ('run-' + [guid]::NewGuid().ToString('N'))
New-Item -ItemType Directory -Path $tempRoot -Force | Out-Null
$env:TEMP = $tempRoot
$env:TMP = $tempRoot

if (-not $NoBuild) {
    Write-Host "Restoring $solution (single MSBuild node)..." -ForegroundColor Cyan
    dotnet msbuild $solution -t:Restore -m:1
    if ($LASTEXITCODE -ne 0) {
        Remove-TestTemp -Path $tempRoot
        exit $LASTEXITCODE
    }

    Write-Host "Building $solution ($Configuration, single MSBuild node)..." -ForegroundColor Cyan
    dotnet build $solution --configuration $Configuration -m:1 --no-restore
    if ($LASTEXITCODE -ne 0) {
        Remove-TestTemp -Path $tempRoot
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
    Write-Host "Test artifacts kept for diagnostics: $tempRoot" -ForegroundColor Yellow
    exit 1
}

if ($KeepTemp) {
    Write-Host "Test artifacts kept: $tempRoot" -ForegroundColor Cyan
}
else {
    Remove-TestTemp -Path $tempRoot
}

Write-Host "All $($projects.Count) test projects passed." -ForegroundColor Green
