<#
.SYNOPSIS
Downloads open SonarQube issues for the local project into .tmp/sonar.

.DESCRIPTION
Uses the SonarQube Web API with a local token and writes a JSON file plus a
human-readable Markdown summary. The token is read from `sonartoken.txt` in the
repository root (ignored by git) or from the SONAR_TOKEN environment variable.
The token is never written to the output.

The Markdown file is intended for review by the developer and by the coding
assistant; it contains one row per issue with rule, severity, file, line, and
message.

.PARAMETER HostUrl
SonarQube server URL. Defaults to the local LAN server.

.PARAMETER ProjectKey
SonarQube project key.

.PARAMETER TokenFile
Local token file. Defaults to `sonartoken.txt` in the repository root.

.PARAMETER OutputDirectory
Directory for issues.json/issues.md. Defaults to `.tmp/sonar`.

.PARAMETER IncludeResolved
Include resolved issues as well as open ones.

.PARAMETER PageSize
API page size; SonarQube accepts at most 500.

.EXAMPLE
./util-scripts/sonar-issues.ps1

.EXAMPLE
./util-scripts/sonar-issues.ps1 -IncludeResolved -OutputDirectory .tmp/sonar/all
#>
[CmdletBinding()]
param(
    [string]$HostUrl = 'http://alpine-box.lan:9000',
    [string]$ProjectKey = 'XML-Editor-Reloaded',
    [string]$TokenFile = '',
    [string]$OutputDirectory = '',
    [switch]$IncludeResolved,
    [ValidateRange(1, 500)]
    [int]$PageSize = 500,
    [ValidateRange(0, 300)]
    [int]$WaitSeconds = 30
)

$ErrorActionPreference = 'Stop'

. (Join-Path $PSScriptRoot 'sonar-common.ps1')

$repo = Get-SonarRepositoryRoot
if ([string]::IsNullOrWhiteSpace($TokenFile)) {
    $TokenFile = Join-Path $repo 'sonartoken.txt'
}
elseif (-not [System.IO.Path]::IsPathRooted($TokenFile)) {
    $TokenFile = Join-Path $repo $TokenFile
}

if ([string]::IsNullOrWhiteSpace($OutputDirectory)) {
    $OutputDirectory = Join-Path $repo '.tmp/sonar'
}
elseif (-not [System.IO.Path]::IsPathRooted($OutputDirectory)) {
    $OutputDirectory = Join-Path $repo $OutputDirectory
}

if ([string]::IsNullOrWhiteSpace($HostUrl) -or [string]::IsNullOrWhiteSpace($ProjectKey)) {
    throw 'HostUrl and ProjectKey must not be empty.'
}

$token = Read-SonarToken -TokenFile $TokenFile
$baseUrl = $HostUrl.TrimEnd('/')
$headers = @{ Authorization = "Bearer $token" }
$issues = [System.Collections.Generic.List[object]]::new()
$projectPrefix = "$($ProjectKey):"
$deadline = [DateTime]::UtcNow.AddSeconds($WaitSeconds)

try {
    do {
        $issues.Clear()
        $page = 1
        $total = $null

        do {
            $query = "api/issues/search?componentKeys=$([uri]::EscapeDataString($ProjectKey))&ps=$PageSize&p=$page"
            if (-not $IncludeResolved) {
                $query += '&resolved=false'
            }

            $response = Invoke-RestMethod -Method Get -Uri "$baseUrl/$query" -Headers $headers -NoProxy
            if ($null -eq $response) {
                throw 'SonarQube returned an empty response.'
            }

            if ($null -eq $total) {
                $total = if ($null -ne $response.total) { [int]$response.total }
                    elseif ($null -ne $response.paging) { [int]$response.paging.total }
                    else { 0 }
            }

            $batch = @($response.issues)
            foreach ($issue in $batch) {
                $component = [string]$issue.component
                $file = if ($component.StartsWith($projectPrefix, [System.StringComparison]::OrdinalIgnoreCase)) {
                    $component.Substring($projectPrefix.Length)
                }
                elseif ($component -match '^[^:]+:(.+)$') {
                    $Matches[1]
                }
                else {
                    $component
                }

                $line = if ($null -ne $issue.textRange) { $issue.textRange.startLine }
                    elseif ($null -ne $issue.line) { $issue.line }
                    else { $null }
                $column = if ($null -ne $issue.textRange) { $issue.textRange.startOffset }
                    else { $null }

                $issues.Add([pscustomobject]@{
                    key = [string]$issue.key
                    rule = [string]$issue.rule
                    severity = [string]$issue.severity
                    type = [string]$issue.type
                    status = [string]$issue.status
                    file = $file
                    line = $line
                    column = $column
                    message = [string]$issue.message
                    effort = [string]$issue.effort
                    debt = [string]$issue.debt
                })
            }

            if ($batch.Count -eq 0) {
                break
            }

            $page++
        }
        while ($issues.Count -lt $total)

        if ($issues.Count -gt 0 -or $total -gt 0 -or [DateTime]::UtcNow -ge $deadline) {
            break
        }

        Write-Host 'SonarQube has not processed the report yet; waiting...' -ForegroundColor DarkYellow
        Start-Sleep -Seconds 2
    }
    while ($true)
}
catch {
    throw "Could not fetch SonarQube issues from '$baseUrl': $($_.Exception.Message)"
}

$fetchedAt = [DateTimeOffset]::Now.ToString('o')
$result = [ordered]@{
    hostUrl = $baseUrl
    projectKey = $ProjectKey
    fetchedAt = $fetchedAt
    includeResolved = [bool]$IncludeResolved
    total = $issues.Count
    issues = $issues
}

New-Item -ItemType Directory -Path $OutputDirectory -Force | Out-Null
$jsonPath = Join-Path $OutputDirectory 'issues.json'
$markdownPath = Join-Path $OutputDirectory 'issues.md'
[System.IO.File]::WriteAllText($jsonPath, ($result | ConvertTo-Json -Depth 20), [System.Text.UTF8Encoding]::new($false))

$severityRank = @{
    BLOCKER = 0
    CRITICAL = 1
    MAJOR = 2
    MINOR = 3
    INFO = 4
}
$ordered = @($issues | Sort-Object `
        @{ Expression = { $severityRank[$_.severity] }; Ascending = $true }, `
        @{ Expression = { $_.file }; Ascending = $true }, `
        @{ Expression = { $_.line }; Ascending = $true })

$summary = [System.Text.StringBuilder]::new()
[void]$summary.AppendLine('# SonarQube issues')
[void]$summary.AppendLine()
[void]$summary.AppendLine("- **Server:** $baseUrl")
[void]$summary.AppendLine("- **Project:** ``$ProjectKey``")
[void]$summary.AppendLine("- **Fetched:** $fetchedAt")
[void]$summary.AppendLine("- **Issues:** $($issues.Count)")
[void]$summary.AppendLine()
[void]$summary.AppendLine('## Summary by severity')
[void]$summary.AppendLine()
[void]$summary.AppendLine('| Severity | Count |')
[void]$summary.AppendLine('|---|---:|')
foreach ($severity in 'BLOCKER', 'CRITICAL', 'MAJOR', 'MINOR', 'INFO') {
    $count = @($issues | Where-Object severity -eq $severity).Count
    if ($count -gt 0) {
        [void]$summary.AppendLine("| $severity | $count |")
    }
}

[void]$summary.AppendLine()
[void]$summary.AppendLine('## Issues')
[void]$summary.AppendLine()
[void]$summary.AppendLine('| Severity | Rule | Type | File | Line | Message |')
[void]$summary.AppendLine('|---|---|---|---|---:|---|')
foreach ($issue in $ordered) {
    $message = ($issue.message -replace '\|', '\|') -replace "`r?`n", ' '
    $file = ($issue.file -replace '\|', '\|') -replace "`r?`n", ' '
    $line = if ($null -eq $issue.line) { '' } else { [string]$issue.line }
    [void]$summary.AppendLine("| $($issue.severity) | ``$($issue.rule)`` | $($issue.type) | ``$file`` | $line | $message |")
}

if ($issues.Count -eq 0) {
    [void]$summary.AppendLine()
    [void]$summary.AppendLine('_No open issues._')
}

[System.IO.File]::WriteAllText($markdownPath, $summary.ToString(), [System.Text.UTF8Encoding]::new($false))

Write-Host "Fetched $($issues.Count) issue(s) from $baseUrl" -ForegroundColor Cyan
Write-Host "JSON: $jsonPath"
Write-Host "Markdown: $markdownPath"
if ($issues.Count -gt 0) {
    foreach ($severity in 'BLOCKER', 'CRITICAL', 'MAJOR', 'MINOR', 'INFO') {
        $count = @($issues | Where-Object severity -eq $severity).Count
        if ($count -gt 0) {
            Write-Host ("  {0,-9} {1}" -f $severity, $count)
        }
    }
}
