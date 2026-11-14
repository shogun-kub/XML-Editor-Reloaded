# Generates or checks the Russian help translation status table.
# The table lives in help/ru/TRANSLATION.md; source documents are never modified.

[CmdletBinding()]
param(
    [switch]$Write,
    [switch]$Check
)

Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'

$repoRoot = (Resolve-Path -LiteralPath (Join-Path $PSScriptRoot '..')).Path
$helpRoot = Join-Path $repoRoot 'xml-editor-reloaded/help'
$enTables = Join-Path $helpRoot 'tables'
$ruRoot = Join-Path $helpRoot 'ru'
$ruTables = Join-Path $ruRoot 'tables'
$output = Join-Path $ruRoot 'TRANSLATION.md'

. (Join-Path $PSScriptRoot 'help-common.ps1')

if (-not (Test-Path -LiteralPath $enTables)) {
    throw "English help tables not found: $enTables"
}

$rows = [System.Collections.Generic.List[string]]::new()
$translated = 0
$missing = 0
$draft = 0
$documents = @(Get-ChildItem -LiteralPath $enTables -Recurse -File -Filter *.md | Sort-Object FullName)
foreach ($file in $documents) {
    $relative = [System.IO.Path]::GetRelativePath($enTables, $file.FullName).Replace('\', '/')
    $ruFile = Join-Path $ruTables $relative
    if (-not (Test-Path -LiteralPath $ruFile)) {
        $rows.Add("| ``$relative`` | missing | — | — | Russian document is missing |")
        $missing++
        continue
    }

    $frontMatter = Read-HelpFrontMatter -Path $ruFile
    $status = if ($frontMatter -and $frontMatter.Data.ContainsKey('status')) {
        [string]$frontMatter.Data['status']
    }
    else {
        'unknown'
    }
    $updated = if ($frontMatter -and $frontMatter.Data.ContainsKey('updated')) {
        [string]$frontMatter.Data['updated']
    }
    else {
        '—'
    }
    if ($status -eq 'draft') {
        $draft++
    }
    else {
        $translated++
    }

    $notes = if ($status -eq 'verified') { 'structural and content review' } else { 'structural parity checked' }
    $rows.Add("| ``$relative`` | $status | $updated | lead | $notes |")
}

$lines = [System.Collections.Generic.List[string]]::new()
$lines.Add('# Статус перевода справки')
$lines.Add('')
$lines.Add('Сгенерировано `util-scripts/help-translation-status.ps1 -Write`. Не редактировать вручную.')
$lines.Add('')
$lines.Add("Документов: $($documents.Count); переведено: $translated; черновиков: $draft; отсутствует: $missing.")
$lines.Add('')
$lines.Add('| Документ | Статус | Обновлён | Проверил | Примечание |')
$lines.Add('|---|---|---|---|---|')
$lines.AddRange($rows)
$content = ($lines -join "`n") + "`n"

if ($Write) {
    $directory = Split-Path -Parent $output
    if (-not (Test-Path -LiteralPath $directory)) {
        New-Item -ItemType Directory -Path $directory | Out-Null
    }

    [System.IO.File]::WriteAllText($output, $content, [System.Text.UTF8Encoding]::new($false))
    Write-Host "Wrote $output"
}

if ($Check) {
    if (-not (Test-Path -LiteralPath $output)) {
        Write-Error "Translation status '$output' does not exist."
        exit 2
    }

    $existing = [System.IO.File]::ReadAllText($output)
    if ($existing -ne $content) {
        Write-Error "Translation status '$output' is out of date."
        exit 2
    }

    Write-Host "Translation status is up to date."
}

if (-not $Write -and -not $Check) {
    $content | Write-Output
}

if ($missing -gt 0) {
    exit 1
}

exit 0
