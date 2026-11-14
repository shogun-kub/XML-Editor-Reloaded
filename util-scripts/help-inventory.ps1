# Builds the XML field inventory used by the help documents.
# Read-only: inspects TableData, never writes to References/.

[CmdletBinding()]
param(
    [string]$TableDataRoot,
    [string]$FallbackTableDataRoot,
    [string]$OutputDirectory
)

Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'

$repoRoot = (Resolve-Path -LiteralPath (Join-Path $PSScriptRoot '..')).Path
if (-not $TableDataRoot) {
    $TableDataRoot = Join-Path $repoRoot 'References/JA 2 1.13 v5/Data-1.13/TableData'
}
if (-not $FallbackTableDataRoot) {
    $FallbackTableDataRoot = Join-Path $repoRoot 'References/JA 2 1.13 v4/Data-1.13/TableData'
}
if (-not $OutputDirectory) {
    $OutputDirectory = Join-Path $repoRoot '.tmp/help'
}

. (Join-Path $PSScriptRoot 'help-common.ps1')

if (-not (Test-Path -LiteralPath $TableDataRoot)) {
    throw "TableData root not found: $TableDataRoot"
}

$inventory = Get-HelpTableInventory -TableDataRoot $TableDataRoot -FallbackTableDataRoot $FallbackTableDataRoot
$totalPaths = 0
foreach ($table in $inventory) {
    $totalPaths += $table.Paths.Count
}

New-Item -ItemType Directory -Force -Path $OutputDirectory | Out-Null

$jsonPath = Join-Path $OutputDirectory 'inventory.json'
$csvPath = Join-Path $OutputDirectory 'inventory.csv'

$inventory |
    ConvertTo-Json -Depth 6 |
    Set-Content -LiteralPath $jsonPath -Encoding utf8NoBOM

$inventory |
    Select-Object Table, File, Root, SourceVersion, ParseError,
        @{ Name = 'PathCount'; Expression = { $_.Paths.Count } },
        @{ Name = 'Paths'; Expression = { $_.Paths -join '|' } } |
    Export-Csv -LiteralPath $csvPath -NoTypeInformation -Encoding utf8NoBOM

$parsed = @($inventory | Where-Object { -not $_.ParseError })
$failed = @($inventory | Where-Object { $_.ParseError })
$fallback = @($inventory | Where-Object { $_.SourceVersion -eq 'v4-fallback' })

Write-Host "Tables: $($inventory.Count) (parsed: $($parsed.Count), failed: $($failed.Count), v4 fallback: $($fallback.Count))"
Write-Host "Leaf paths: $totalPaths"
Write-Host "Inventory: $jsonPath"
Write-Host "CSV:       $csvPath"

if ($failed.Count -gt 0) {
    Write-Host ''
    Write-Host 'Unparsed tables:'
    foreach ($table in $failed) {
        Write-Host "  $($table.Table): $($table.ParseError)"
    }
}
