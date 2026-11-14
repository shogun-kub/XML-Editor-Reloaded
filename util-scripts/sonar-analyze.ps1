<#
.SYNOPSIS
Runs a local SonarScanner for .NET analysis around the normal build and test cycle.

.DESCRIPTION
The SonarScanner for .NET must see the compiler invocation, so this script runs:
restore, `sonarscanner begin`, build, tests, and `sonarscanner end` in that order.
It is intentionally a local-only wrapper: the LAN SonarQube server is not reachable
from GitHub-hosted runners, and no token or server URL is committed.

The token is read from `sonartoken.txt` in the repository root (ignored by git) or
from the SONAR_TOKEN environment variable. The token is never printed by this script.

.PARAMETER HostUrl
SonarQube server URL. Defaults to the local LAN server.

.PARAMETER ProjectKey
SonarQube project key.

.PARAMETER Configuration
Build configuration used for restore, build, and tests.

.PARAMETER TokenFile
Local token file. Defaults to `sonartoken.txt` in the repository root.

.PARAMETER SkipTests
Skip the xUnit suite. The analyzer still sees the build output.

.EXAMPLE
./util-scripts/sonar-analyze.ps1

.EXAMPLE
./util-scripts/sonar-analyze.ps1 -Configuration Release -SkipTests

.NOTES
Requires the dotnet-sonarscanner global tool. Community Edition does not support
branch analysis, so the current checkout is analyzed as the project's build.
#>
[CmdletBinding()]
param(
    [string]$HostUrl = 'http://alpine-box.lan:9000',
    [string]$ProjectKey = 'XML-Editor-Reloaded',
    [ValidateSet('Debug', 'Release')]
    [string]$Configuration = 'Debug',
    [string]$TokenFile = '',
    [switch]$SkipTests
)

$ErrorActionPreference = 'Stop'

. (Join-Path $PSScriptRoot 'sonar-common.ps1')

$repo = Get-SonarRepositoryRoot
$root = Join-Path $repo 'xml-editor-reloaded'
$solution = Join-Path $root 'XmlEditorReloaded.slnx'
$propsPath = Join-Path $root 'Directory.Build.props'
if ([string]::IsNullOrWhiteSpace($TokenFile)) {
    $TokenFile = Join-Path $repo 'sonartoken.txt'
}
elseif (-not [System.IO.Path]::IsPathRooted($TokenFile)) {
    $TokenFile = Join-Path $repo $TokenFile
}

if (-not (Test-Path -LiteralPath $solution)) {
    throw "Solution not found at '$solution'."
}

if (-not (Get-Command dotnet -ErrorAction SilentlyContinue)) {
    throw 'The dotnet CLI is not available in PATH.'
}

if (-not (Get-Command dotnet-sonarscanner -ErrorAction SilentlyContinue)) {
    throw 'dotnet-sonarscanner is not installed. Run: dotnet tool install --global dotnet-sonarscanner'
}

if ([string]::IsNullOrWhiteSpace($HostUrl) -or [string]::IsNullOrWhiteSpace($ProjectKey)) {
    throw 'HostUrl and ProjectKey must not be empty.'
}

$token = Read-SonarToken -TokenFile $TokenFile
$version = Get-SonarProjectVersion -PropsPath $propsPath
$scanRoot = Join-Path $repo '.tmp/sonar'
$coverageRoot = Join-Path $scanRoot 'coverage'
New-Item -ItemType Directory -Path $scanRoot -Force | Out-Null

# The LAN server must not go through the machine-wide HTTP proxy that is used for
# internet access. NO_PROXY is enough for the .NET scanner and NuGet clients.
$serverUri = [uri]$HostUrl
$noProxyHosts = @($serverUri.Host, 'localhost', '127.0.0.1') |
    Where-Object { -not [string]::IsNullOrWhiteSpace($_) } |
    Select-Object -Unique
$existingNoProxy = @($env:NO_PROXY, $env:no_proxy) |
    Where-Object { -not [string]::IsNullOrWhiteSpace($_) }
$env:NO_PROXY = (@($existingNoProxy) + $noProxyHosts | Select-Object -Unique) -join ','
$env:no_proxy = $env:NO_PROXY

$commonProperties = @(
    "/d:sonar.host.url=$($HostUrl.TrimEnd('/'))",
    "/d:sonar.token=$token",
    '/d:sonar.sourceEncoding=UTF-8',
    '/d:sonar.sources=xml-editor-reloaded/src,xml-editor-reloaded/tools',
    '/d:sonar.tests=xml-editor-reloaded/tests',
    '/d:sonar.exclusions=**/obj/**,**/bin/**,**/out/**,**/*.g.cs,**/*.Designer.cs,References/**',
    '/d:sonar.coverage.exclusions=**/tests/**,**/obj/**,**/bin/**',
    '/d:sonar.scanner.scanAll=false'
)

if (-not $SkipTests) {
    $coveragePattern = ($coverageRoot -replace '\\', '/') + '/**/coverage.cobertura.xml'
    $trxPattern = ($coverageRoot -replace '\\', '/') + '/**/*.trx'
    $commonProperties += @(
        "/d:sonar.cs.cobertura.reportsPaths=$coveragePattern",
        "/d:sonar.cs.vstest.reportsPaths=$trxPattern"
    )
}

$begun = $false
$failure = $null
$previousLocation = Get-Location
try {
    Set-Location $repo

    Write-Host "Restoring $solution" -ForegroundColor Cyan
    & dotnet restore $solution -m:1
    if ($LASTEXITCODE -ne 0) {
        throw "dotnet restore failed with exit code $LASTEXITCODE."
    }

    Write-Host "Starting SonarScanner for $ProjectKey" -ForegroundColor Cyan
    & dotnet sonarscanner begin "/k:$ProjectKey" "/v:$version" @commonProperties
    if ($LASTEXITCODE -ne 0) {
        throw "sonarscanner begin failed with exit code $LASTEXITCODE."
    }

    $begun = $true

    Write-Host "Building $solution ($Configuration)" -ForegroundColor Cyan
    & dotnet build $solution --configuration $Configuration -m:1 --no-restore
    if ($LASTEXITCODE -ne 0) {
        throw "dotnet build failed with exit code $LASTEXITCODE."
    }

    if (-not $SkipTests) {
        Write-Host 'Running tests with coverage' -ForegroundColor Cyan
        Remove-Item -LiteralPath $coverageRoot -Recurse -Force -ErrorAction SilentlyContinue
        New-Item -ItemType Directory -Path $coverageRoot -Force | Out-Null
        & pwsh -NoProfile -ExecutionPolicy Bypass -File (Join-Path $PSScriptRoot 'test.ps1') `
            -Configuration $Configuration -NoBuild -CollectCoverage -ResultsDirectory $coverageRoot
        if ($LASTEXITCODE -ne 0) {
            throw "Tests failed with exit code $LASTEXITCODE."
        }
    }
}
catch {
    $failure = $_
    Write-Host $_.Exception.Message -ForegroundColor Red
}
finally {
    if ($begun) {
        Write-Host 'Completing SonarQube analysis' -ForegroundColor Cyan
        & dotnet sonarscanner end "/d:sonar.token=$token"
        if ($LASTEXITCODE -ne 0 -and $null -eq $failure) {
            $failure = "sonarscanner end failed with exit code $LASTEXITCODE."
        }
    }

    Set-Location $previousLocation
}

if ($null -ne $failure) {
    exit 1
}

Write-Host 'SonarQube analysis completed.' -ForegroundColor Green
Write-Host 'Fetch issues with: pwsh -NoProfile -ExecutionPolicy Bypass -File util-scripts/sonar-issues.ps1'
