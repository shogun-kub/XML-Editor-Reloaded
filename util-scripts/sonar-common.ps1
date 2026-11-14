# Shared helpers for the local SonarQube scripts. Dot-source this file from a script.

$script:SonarRepositoryRoot = Split-Path -Parent $PSScriptRoot

function Get-SonarRepositoryRoot {
    return $script:SonarRepositoryRoot
}

function Read-SonarToken {
    param(
        [Parameter(Mandatory = $true)]
        [string]$TokenFile
    )

    if (Test-Path -LiteralPath $TokenFile) {
        $token = ([System.IO.File]::ReadAllText($TokenFile)).Trim()
        if (-not [string]::IsNullOrWhiteSpace($token)) {
            return $token
        }
    }

    if (-not [string]::IsNullOrWhiteSpace($env:SONAR_TOKEN)) {
        return $env:SONAR_TOKEN.Trim()
    }

    throw "SonarQube token not found. Put it into '$TokenFile' (ignored by git) or set the SONAR_TOKEN environment variable."
}

function Get-SonarProjectVersion {
    param(
        [Parameter(Mandatory = $true)]
        [string]$PropsPath
    )

    if (-not (Test-Path -LiteralPath $PropsPath)) {
        return 'local'
    }

    [xml]$props = Get-Content -LiteralPath $PropsPath
    $version = @($props.Project.PropertyGroup | ForEach-Object { $_.Version } |
        Where-Object { -not [string]::IsNullOrWhiteSpace($_) }) |
        Select-Object -First 1
    return $(if ($version) { $version } else { 'local' })
}
