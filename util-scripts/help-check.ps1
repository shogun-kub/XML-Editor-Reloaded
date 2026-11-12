# Validates help documents against the XML table inventory.
# Reports coverage, checks front matter, field blocks and structure order, validates links,
# and (with -Write) rebuilds per-document Contents, INDEX.md and COVERAGE.md.
# Read-only with respect to game data; only help/** is written.

[CmdletBinding()]
param(
    [string]$HelpRoot,
    [string]$TableDataRoot,
    [string]$FallbackTableDataRoot,
    [ValidateSet('en', 'ru')]
    [string]$Lang = 'en',
    [switch]$Write,
    [switch]$Release
)

Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'

$repoRoot = (Resolve-Path -LiteralPath (Join-Path $PSScriptRoot '..')).Path
if (-not $HelpRoot) {
    $HelpRoot = if ($Lang -eq 'ru') {
        Join-Path $repoRoot 'xml-editor-reloaded/help/ru'
    }
    else {
        Join-Path $repoRoot 'xml-editor-reloaded/help'
    }
}
if (-not $TableDataRoot) {
    $TableDataRoot = Join-Path $repoRoot 'References/JA 2 1.13 v5/Data-1.13/TableData'
}
if (-not $FallbackTableDataRoot) {
    $FallbackTableDataRoot = Join-Path $repoRoot 'References/JA 2 1.13 v4/Data-1.13/TableData'
}

. (Join-Path $PSScriptRoot 'help-common.ps1')

if (-not (Test-Path -LiteralPath $HelpRoot)) {
    throw "Help root not found: $HelpRoot"
}

$inventory = Get-HelpTableInventory -TableDataRoot $TableDataRoot -FallbackTableDataRoot $FallbackTableDataRoot
$byTable = [System.Collections.Generic.Dictionary[string, object]]::new([System.StringComparer]::Ordinal)
foreach ($table in $inventory) {
    $byTable[$table.Table] = $table
}

$errors = [System.Collections.Generic.List[string]]::new()
$warnings = [System.Collections.Generic.List[string]]::new()
$docInfos = [System.Collections.Generic.List[object]]::new()

$docFiles = @(Get-ChildItem -LiteralPath $HelpRoot -Recurse -File -Filter *.md |
    Where-Object {
        $_.FullName -match '\\tables\\' -and
        ($Lang -ne 'en' -or $_.FullName -notlike ((Join-Path $HelpRoot 'ru') + '\*'))
    })
$docFiles = @($docFiles | Sort-Object FullName)

foreach ($file in $docFiles) {
    $relative = [System.IO.Path]::GetRelativePath($HelpRoot, $file.FullName).Replace('\', '/')
    $frontMatter = Read-HelpFrontMatter -Path $file.FullName
    if ($null -eq $frontMatter) {
        $errors.Add("${relative}: missing or malformed YAML front matter")
        continue
    }

    $data = $frontMatter.Data
    $required = @('schema', 'lang', 'family', 'title', 'tables', 'table_ids', 'files', 'root', 'record', 'status')
    foreach ($key in $required) {
        if (-not $data.ContainsKey($key)) {
            $errors.Add("${relative}: front matter is missing '$key'")
        }
    }
    if (-not $data.ContainsKey('schema') -or [string]$data['schema'] -ne '1') {
        $errors.Add("${relative}: 'schema' must be 1")
    }
    if ($data.ContainsKey('lang') -and [string]$data['lang'] -notin @('en', 'ru')) {
        $errors.Add("${relative}: 'lang' must be 'en' or 'ru'")
    }
    if ($data.ContainsKey('status') -and [string]$data['status'] -notin @('draft', 'reviewed', 'verified')) {
        $errors.Add("${relative}: 'status' must be draft, reviewed or verified")
    }
    if ($Release) {
        if ([string]$data['lang'] -ne $Lang) {
            $errors.Add("${relative}: release documents must use lang '$Lang'")
        }
        if ([string]$data['status'] -eq 'draft') {
            $errors.Add("${relative}: release documents must not be draft")
        }
    }

    $tables = @($data['tables'])
    $tableIds = @($data['table_ids'])
    $files = @($data['files'])
    if ($tables.Count -eq 0) {
        $errors.Add("${relative}: 'tables' is empty")
    }
    if ($tableIds.Count -eq 0) {
        $errors.Add("${relative}: 'table_ids' is empty")
    }
    if ($tableIds.Count -ne $tables.Count) {
        $errors.Add("${relative}: 'tables' and 'table_ids' must have the same length")
    }
    if ($files.Count -eq 0) {
        $errors.Add("${relative}: 'files' is empty")
    }
    foreach ($fileName in $files) {
        $v5Path = Join-Path $TableDataRoot $fileName
        $v4Path = Join-Path $FallbackTableDataRoot $fileName
        if (-not (Test-Path -LiteralPath $v5Path) -and -not (Test-Path -LiteralPath $v4Path)) {
            $warnings.Add("${relative}: file '$fileName' not found in v5 or v4")
        }
    }

    $pathMode = if ($data.ContainsKey('path_mode')) { [string]$data['path_mode'] } else { 'full' }
    if ($pathMode -notin @('full', 'relative')) {
        $errors.Add("${relative}: path_mode must be 'full' or 'relative'")
        $pathMode = 'full'
    }

    $fields = Get-HelpDocFields -Lines $frontMatter.Lines -Start $frontMatter.BodyStart
    if ($fields.Count -eq 0) {
        $errors.Add("${relative}: no field blocks (### `path`) found")
    }
    $fieldPaths = @($fields | ForEach-Object { $_.Path })
    if ((@($fieldPaths | Select-Object -Unique)).Count -ne $fieldPaths.Count) {
        $errors.Add("${relative}: duplicate field path in one document")
    }

    $fieldLeaves = @($fieldPaths | ForEach-Object { ($_ -split '/')[-1] })
    $keyText = if ($data.ContainsKey('key')) { [string]$data['key'] } else { '' }
    $keyNames = @()
    if ($keyText -and $keyText -notin @('none', 'unknown')) {
        $keyNames = @($keyText -split '\s*\+\s*' | ForEach-Object { $_.Trim() } | Where-Object { $_ })
    }
    if ($keyNames.Count -gt 0) {
        for ($i = 0; $i -lt $keyNames.Count; $i++) {
            if ($i -ge $fieldLeaves.Count -or $fieldLeaves[$i] -ne $keyNames[$i]) {
                $errors.Add("${relative}: key field '$($keyNames[$i])' must be first in ## Fields (found '$($fieldLeaves[$i])')")
                break
            }
        }
    }

    $structureLeaves = Get-HelpStructureLeaves -Lines $frontMatter.Lines -Start $frontMatter.BodyStart
    $fieldLeafSet = [System.Collections.Generic.HashSet[string]]::new([System.StringComparer]::Ordinal)
    foreach ($leaf in $fieldLeaves) {
        [void]$fieldLeafSet.Add($leaf)
    }
    $structureFields = @($structureLeaves | Where-Object { $fieldLeafSet.Contains($_) })
    if ($structureFields.Count -ne $fieldLeaves.Count) {
        $errors.Add("${relative}: ## Structure must list every documented field exactly once (found $($structureFields.Count), expected $($fieldLeaves.Count))")
    }
    else {
        for ($i = 0; $i -lt $fieldLeaves.Count; $i++) {
            if ($structureFields[$i] -ne $fieldLeaves[$i]) {
                $errors.Add("${relative}: ## Structure field '$($structureFields[$i])' does not match ## Fields order at '$($fieldLeaves[$i])'")
                break
            }
        }
    }

    foreach ($field in $fields) {
        foreach ($key in @('Type', 'Description', 'Source', 'Confidence')) {
            if (-not $field.Values.ContainsKey($key) -or [string]::IsNullOrWhiteSpace([string]$field.Values[$key])) {
                $errors.Add("${relative}: field '$($field.Path)' is missing '$key'")
            }
        }
        if ($field.Values.ContainsKey('Confidence')) {
            $confidence = [string]$field.Values['Confidence']
            $confidence = $confidence.Trim('`').Trim()
            if ($confidence -notin @('source', 'comment', 'metadata', 'inferred', 'unknown')) {
                $errors.Add("${relative}: field '$($field.Path)' has unknown confidence '$confidence'")
            }
        }
    }

    $tableInfo = [System.Collections.Generic.List[object]]::new()
    $allExpected = [System.Collections.Generic.List[string]]::new()
    foreach ($tableName in $tableIds) {
        if (-not $byTable.ContainsKey($tableName)) {
            $errors.Add("${relative}: table '$tableName' is not present in the inventory")
            continue
        }

        $inventoryTable = $byTable[$tableName]
        if ($inventoryTable.ParseError -and $inventoryTable.SourceVersion -ne 'v4-fallback') {
            $warnings.Add("${relative}: table '$tableName' is unverifiable (XML parse error); skipped")
            continue
        }

        $expected = @($inventoryTable.Paths)
        foreach ($expectedPath in $expected) {
            $allExpected.Add($expectedPath)
        }
        $missing = [System.Collections.Generic.List[string]]::new()
        $covered = 0
        foreach ($expectedPath in $expected) {
            $matched = $false
            foreach ($fieldPath in $fieldPaths) {
                if ($pathMode -eq 'relative') {
                    if ($fieldPath -eq $expectedPath -or
                        $expectedPath.EndsWith('/' + $fieldPath, [System.StringComparison]::Ordinal)) {
                        $matched = $true
                        break
                    }
                }
                elseif ($fieldPath -eq $expectedPath) {
                    $matched = $true
                    break
                }
            }
            if ($matched) {
                $covered++
            }
            else {
                $missing.Add($expectedPath)
            }
        }

        if ($missing.Count -gt 0) {
            $shown = @($missing | Select-Object -First 20)
            $suffix = if ($missing.Count -gt $shown.Count) { " (+$($missing.Count - $shown.Count) more)" } else { '' }
            $errors.Add("${relative}: table '$tableName' misses fields: $($shown -join ', ')$suffix")
        }

        $tableInfo.Add([pscustomobject]@{
            Table = $tableName
            Expected = $expected.Count
            Covered = $covered
            SourceVersion = $inventoryTable.SourceVersion
            ParseError = $inventoryTable.ParseError
        })
    }

    if ($allExpected.Count -gt 0) {
        foreach ($fieldPath in $fieldPaths) {
            $found = $false
            foreach ($expectedPath in $allExpected) {
                if ($pathMode -eq 'relative') {
                    if ($fieldPath -eq $expectedPath -or
                        $expectedPath.EndsWith('/' + $fieldPath, [System.StringComparison]::Ordinal)) {
                        $found = $true
                        break
                    }
                }
                elseif ($fieldPath -eq $expectedPath) {
                    $found = $true
                    break
                }
            }
            if (-not $found) {
                $errors.Add("${relative}: field '$fieldPath' is not present in any listed table")
            }
        }
    }

    $docInfos.Add([pscustomobject]@{
        Path = $file.FullName
        Relative = $relative
        FrontMatter = $data
        Fields = $fields
        Tables = $tableIds
        TableInfo = $tableInfo
        PathMode = $pathMode
    })
}

# Russian documents must mirror the English structure and field paths.
if ($Lang -eq 'ru') {
    $enTablesRoot = Join-Path $repoRoot 'xml-editor-reloaded/help/tables'
    $ruTablesRoot = Join-Path $HelpRoot 'tables'
    if (-not (Test-Path -LiteralPath $enTablesRoot)) {
        $errors.Add("English help root not found: $enTablesRoot")
    }
    elseif (-not (Test-Path -LiteralPath $ruTablesRoot)) {
        $errors.Add("Russian tables directory not found: $ruTablesRoot")
    }
    else {
        $enFiles = @(Get-ChildItem -LiteralPath $enTablesRoot -Recurse -File -Filter *.md |
            Sort-Object FullName)
        $enRelative = [System.Collections.Generic.HashSet[string]]::new([System.StringComparer]::Ordinal)
        foreach ($enFile in $enFiles) {
            $relative = [System.IO.Path]::GetRelativePath($enTablesRoot, $enFile.FullName).Replace('\', '/')
            [void]$enRelative.Add($relative)
            $ruFile = Join-Path $ruTablesRoot $relative
            if (-not (Test-Path -LiteralPath $ruFile)) {
                $errors.Add("${relative}: Russian document is missing")
                continue
            }

            $enFrontMatter = Read-HelpFrontMatter -Path $enFile.FullName
            $ruFrontMatter = Read-HelpFrontMatter -Path $ruFile
            if ($null -eq $enFrontMatter -or $null -eq $ruFrontMatter) {
                $errors.Add("${relative}: front matter cannot be compared")
                continue
            }

            foreach ($key in @('schema', 'family', 'tables', 'table_ids', 'files', 'root', 'record', 'key', 'path_mode')) {
                $enValue = if ($enFrontMatter.Data.ContainsKey($key)) {
                    (@($enFrontMatter.Data[$key]) -join '|')
                }
                else {
                    '<missing>'
                }
                $ruValue = if ($ruFrontMatter.Data.ContainsKey($key)) {
                    (@($ruFrontMatter.Data[$key]) -join '|')
                }
                else {
                    '<missing>'
                }
                if ($enValue -ne $ruValue) {
                    $errors.Add("${relative}: front matter '$key' differs from English")
                }
            }

            if ([string]$ruFrontMatter.Data['lang'] -ne 'ru') {
                $errors.Add("${relative}: Russian document must declare lang: ru")
            }

            $enFields = @(Get-HelpDocFields -Lines $enFrontMatter.Lines -Start $enFrontMatter.BodyStart)
            $ruFields = @(Get-HelpDocFields -Lines $ruFrontMatter.Lines -Start $ruFrontMatter.BodyStart)
            if ($enFields.Count -ne $ruFields.Count) {
                $errors.Add("${relative}: Russian field count $($ruFields.Count) differs from English $($enFields.Count)")
            }
            else {
                for ($i = 0; $i -lt $enFields.Count; $i++) {
                    if ($enFields[$i].Path -ne $ruFields[$i].Path) {
                        $errors.Add("${relative}: field $($i + 1) is '$($ruFields[$i].Path)', expected '$($enFields[$i].Path)'")
                        break
                    }

                    foreach ($key in @('Type', 'Source', 'Confidence')) {
                        $enValue = if ($enFields[$i].Values.ContainsKey($key)) { [string]$enFields[$i].Values[$key] } else { '' }
                        $ruValue = if ($ruFields[$i].Values.ContainsKey($key)) { [string]$ruFields[$i].Values[$key] } else { '' }
                        if ($enValue -ne $ruValue) {
                            $errors.Add("${relative}: field '$($enFields[$i].Path)' has a different '$key'")
                        }
                    }
                }
            }

            $enHeadings = @(Get-HelpHeadings -Lines $enFrontMatter.Lines -Start $enFrontMatter.BodyStart |
                Where-Object { $_.Level -ge 2 })
            $ruHeadings = @(Get-HelpHeadings -Lines $ruFrontMatter.Lines -Start $ruFrontMatter.BodyStart |
                Where-Object { $_.Level -ge 2 })
            if ($enHeadings.Count -ne $ruHeadings.Count) {
                $errors.Add("${relative}: Russian heading count differs from English")
            }
            else {
                for ($i = 0; $i -lt $enHeadings.Count; $i++) {
                    if ($enHeadings[$i].Text -ne $ruHeadings[$i].Text -or
                        $enHeadings[$i].Anchor -ne $ruHeadings[$i].Anchor) {
                        $errors.Add("${relative}: heading '$($ruHeadings[$i].Text)' differs from '$($enHeadings[$i].Text)'")
                        break
                    }
                }
            }

            $enLinks = @(Get-HelpMarkdownLinks -Content (Get-Content -LiteralPath $enFile.FullName -Raw) |
                Sort-Object)
            $ruLinks = @(Get-HelpMarkdownLinks -Content (Get-Content -LiteralPath $ruFile -Raw) |
                Sort-Object)
            if (($enLinks -join "`n") -ne ($ruLinks -join "`n")) {
                $errors.Add("${relative}: links differ from English")
            }
        }

        $ruFiles = @(Get-ChildItem -LiteralPath $ruTablesRoot -Recurse -File -Filter *.md |
            Sort-Object FullName)
        foreach ($ruFile in $ruFiles) {
            $relative = [System.IO.Path]::GetRelativePath($ruTablesRoot, $ruFile.FullName).Replace('\', '/')
            if (-not $enRelative.Contains($relative)) {
                $errors.Add("${relative}: Russian document has no English source")
            }
        }

        $pageFiles = @(
            'GLOSSARY.md',
            'FORMULAS.md',
            'SOURCES.md',
            'OPTIONS.md',
            'Localization.md',
            'EMAIL-TEXT.md'
        )
        $enHelpRoot = Join-Path $repoRoot 'xml-editor-reloaded/help'
        foreach ($pageFile in $pageFiles) {
            if ((Test-Path -LiteralPath (Join-Path $enHelpRoot $pageFile)) -and
                -not (Test-Path -LiteralPath (Join-Path $HelpRoot $pageFile))) {
                $errors.Add("${pageFile}: Russian page is missing")
            }
        }

        Write-Host "Russian parity checked: $($enFiles.Count) documents"
    }
}

# Detect the same table documented twice.
$tableOwners = @{}
foreach ($doc in $docInfos) {
    foreach ($tableName in $doc.Tables) {
        if ($tableOwners.ContainsKey($tableName)) {
            $errors.Add("table '$tableName' is documented by both '$($tableOwners[$tableName])' and '$($doc.Relative)'")
        }
        else {
            $tableOwners[$tableName] = $doc.Relative
        }
    }
}

# Rebuild Contents sections before link validation so generated anchors are included.
if ($Write) {
    foreach ($doc in $docInfos) {
        $frontMatter = Read-HelpFrontMatter -Path $doc.Path
        $headings = Get-HelpHeadings -Lines $frontMatter.Lines -Start $frontMatter.BodyStart
        $contents = [System.Collections.Generic.List[string]]::new()
        $contents.Add('## Contents')
        $contents.Add('')
        foreach ($heading in $headings) {
            if ($heading.Text -eq 'Contents') {
                continue
            }
            $indent = if ($heading.Level -ge 3) { '  ' } else { '' }
            $contents.Add("$indent- [$($heading.Text)](#$($heading.Anchor))")
        }
        $contents.Add('')

        $lines = [System.Collections.Generic.List[string]]::new()
        $lines.AddRange([string[]]$frontMatter.Lines)
        $contentsIndex = -1
        for ($i = $frontMatter.BodyStart; $i -lt $lines.Count; $i++) {
            if ($lines[$i] -match '^##\s+Contents\s*$') {
                $contentsIndex = $i
                break
            }
        }

        if ($contentsIndex -ge 0) {
            $end = $lines.Count
            for ($i = $contentsIndex + 1; $i -lt $lines.Count; $i++) {
                if ($lines[$i] -match '^##\s+') {
                    $end = $i
                    break
                }
            }
            $before = @($lines[0..($contentsIndex - 1)])
            $after = if ($end -lt $lines.Count) { @($lines[$end..($lines.Count - 1)]) } else { @() }
            $result = @($before) + @($contents) + @($after)
            Write-HelpLines -Path $doc.Path -Lines $result
        }
        else {
            $insertAt = -1
            for ($i = 0; $i -lt $lines.Count; $i++) {
                if ($lines[$i] -match '^#\s+') {
                    $insertAt = $i + 1
                    break
                }
            }
            if ($insertAt -ge 0) {
                if ($insertAt -lt $lines.Count -and $lines[$insertAt].Trim().Length -ne 0) {
                    $contents.Insert(0, '')
                }
                $result = @($lines[0..($insertAt - 1)]) + @($contents) + @($lines[$insertAt..($lines.Count - 1)])
                Write-HelpLines -Path $doc.Path -Lines $result
            }
        }
    }
}

# Build anchors for every Markdown file in the help tree.
$anchorMap = @{}
$allMarkdown = @(Get-ChildItem -LiteralPath $HelpRoot -Recurse -File -Filter *.md)
$codeFilePattern = '(?i)(?:Tactical|Strategic|Ja2|Utils|Laptop|Editor)/[^\s`)]+\.(?:cpp|h|hpp)\b'
$codeLinePattern = '(?i)\.(?:cpp|h|hpp):\d+'
foreach ($file in $allMarkdown) {
    $text = [System.IO.File]::ReadAllText($file.FullName)
    $relative = [System.IO.Path]::GetRelativePath($HelpRoot, $file.FullName).Replace('\', '/')
    if ($text -match $codeFilePattern -or $text -match $codeLinePattern) {
        $errors.Add("${relative}: contains a source file/line reference; use a general Source description")
    }
}
foreach ($file in $allMarkdown) {
    $frontMatter = Read-HelpFrontMatter -Path $file.FullName
    $lines = [System.IO.File]::ReadAllLines($file.FullName)
    $start = if ($frontMatter) { $frontMatter.BodyStart } else { 0 }
    $anchors = [System.Collections.Generic.HashSet[string]]::new([System.StringComparer]::Ordinal)
    foreach ($heading in Get-HelpHeadings -Lines $lines -Start $start) {
        [void]$anchors.Add($heading.Anchor)
    }
    $anchorMap[$file.FullName] = $anchors
}

foreach ($file in $allMarkdown) {
    $content = [System.IO.File]::ReadAllText($file.FullName)
    $relative = [System.IO.Path]::GetRelativePath($HelpRoot, $file.FullName).Replace('\', '/')
    foreach ($link in Get-HelpMarkdownLinks -Content $content) {
        if ($link -match '^(https?:|mailto:|ftp:)') {
            continue
        }
        $pathPart = $link
        $anchor = $null
        $hash = $link.IndexOf('#')
        if ($hash -ge 0) {
            $pathPart = $link.Substring(0, $hash)
            $anchor = $link.Substring($hash + 1)
        }
        $targetFile = if ([string]::IsNullOrEmpty($pathPart)) {
            $file.FullName
        }
        else {
            $candidate = Join-Path (Split-Path -Parent $file.FullName) $pathPart
            [System.IO.Path]::GetFullPath($candidate)
        }
        if (-not (Test-Path -LiteralPath $targetFile)) {
            $errors.Add("${relative}: link target not found: $link")
            continue
        }
        if ($anchor -and $targetFile.EndsWith('.md', [System.StringComparison]::OrdinalIgnoreCase)) {
            if (-not $anchorMap.ContainsKey($targetFile) -or -not $anchorMap[$targetFile].Contains($anchor)) {
                $errors.Add("${relative}: anchor not found: $link")
            }
        }
    }
}

# Coverage summary.
$coveredTables = [System.Collections.Generic.HashSet[string]]::new([System.StringComparer]::Ordinal)
$coveredPaths = 0
$totalPaths = 0
foreach ($table in $inventory) {
    $totalPaths += $table.Paths.Count
}
foreach ($doc in $docInfos) {
    foreach ($info in $doc.TableInfo) {
        [void]$coveredTables.Add($info.Table)
        $coveredPaths += $info.Covered
    }
}

$pendingTables = @($inventory | Where-Object { -not $coveredTables.Contains($_.Table) })

if ($Release -and $pendingTables.Count -gt 0) {
    $errors.Add("release requires all tables: $($pendingTables.Count) still pending")
}

if ($Write) {
    $isRussian = $Lang -eq 'ru'
    $indexTitle = if ($isRussian) { '# Индекс справки по таблицам' } else { '# Table help index' }
    $generatedBy = if ($isRussian) {
        'Сгенерировано `util-scripts/help-check.ps1 -Write`. Не редактировать вручную.'
    }
    else {
        'Generated by `util-scripts/help-check.ps1 -Write`. Do not edit manually.'
    }
    $documentedLine = if ($isRussian) {
        "Документировано: $($coveredTables.Count) из $($inventory.Count) таблиц; $coveredPaths из $totalPaths путей полей."
    }
    else {
        "Documented: $($coveredTables.Count) of $($inventory.Count) tables; $coveredPaths of $totalPaths field paths."
    }
    $indexHeader = if ($isRussian) { '| Таблица | Файл | Документ | Статус |' } else { '| Table | File | Document | Status |' }
    $documentedStatus = if ($isRussian) { 'документирована' } else { 'documented' }
    $pendingStatus = if ($isRussian) { 'ожидает' } else { 'pending' }

    $indexLines = [System.Collections.Generic.List[string]]::new()
    $indexLines.Add($indexTitle)
    $indexLines.Add('')
    $indexLines.Add($generatedBy)
    $indexLines.Add('')
    $indexLines.Add($documentedLine)
    $indexLines.Add('')
    $indexLines.Add($indexHeader)
    $indexLines.Add('|---|---|---|---|')
    foreach ($table in $inventory) {
        $doc = if ($tableOwners.ContainsKey($table.Table)) { $tableOwners[$table.Table] } else { $null }
        $docLink = if ($doc) { "[$doc]($doc)" } else { '—' }
        $status = if ($doc) { $documentedStatus } else { $pendingStatus }
        $cleanName = Get-HelpCleanTableName -Table $table.Table
        $indexLines.Add("| ``$cleanName`` | ``$($table.File)`` | $docLink | $status |")
    }
    Write-HelpLines -Path (Join-Path $HelpRoot 'INDEX.md') -Lines $indexLines

    $coverageTitle = if ($isRussian) { '# Покрытие справки' } else { '# Help coverage' }
    $summaryTitle = if ($isRussian) { '## Сводка' } else { '## Summary' }
    $metricHeader = if ($isRussian) { '| Показатель | Значение |' } else { '| Metric | Value |' }
    $tablesInventory = if ($isRussian) { 'Таблиц в инвентаре' } else { 'Tables in inventory' }
    $tablesDocumented = if ($isRussian) { 'Таблиц документировано' } else { 'Tables documented' }
    $pathsInventory = if ($isRussian) { 'Путей полей в инвентаре' } else { 'Field paths in inventory' }
    $pathsDocumented = if ($isRussian) { 'Путей полей документировано' } else { 'Field paths documented' }
    $documentsLabel = if ($isRussian) { 'Документов' } else { 'Documents' }
    $pendingFolderTitle = if ($isRussian) { '## Ожидающие таблицы по папкам' } else { '## Pending tables by folder' }
    $folderHeader = if ($isRussian) { '| Папка | Ожидает |' } else { '| Folder | Pending |' }
    $documentedTablesTitle = if ($isRussian) { '## Документированные таблицы' } else { '## Documented tables' }
    $tableHeader = if ($isRussian) { '| Таблица | Файл | Документ | Пути | Источник |' } else { '| Table | File | Document | Paths | Source |' }

    $coverageLines = [System.Collections.Generic.List[string]]::new()
    $coverageLines.Add($coverageTitle)
    $coverageLines.Add('')
    $coverageLines.Add($generatedBy)
    $coverageLines.Add('')
    $coverageLines.Add($summaryTitle)
    $coverageLines.Add('')
    $coverageLines.Add($metricHeader)
    $coverageLines.Add('|---|---:|')
    $coverageLines.Add("| $tablesInventory | $($inventory.Count) |")
    $coverageLines.Add("| $tablesDocumented | $($coveredTables.Count) |")
    $coverageLines.Add("| $pathsInventory | $totalPaths |")
    $coverageLines.Add("| $pathsDocumented | $coveredPaths |")
    $coverageLines.Add("| $documentsLabel | $($docInfos.Count) |")
    $coverageLines.Add('')
    $coverageLines.Add($pendingFolderTitle)
    $coverageLines.Add('')
    $pendingByFolder = @{}
    foreach ($table in $pendingTables) {
        $folder = if ($table.File.Contains('/')) { $table.File.Split('/')[0] } else { '(root)' }
        if (-not $pendingByFolder.ContainsKey($folder)) {
            $pendingByFolder[$folder] = 0
        }
        $pendingByFolder[$folder]++
    }
    $coverageLines.Add($folderHeader)
    $coverageLines.Add('|---|---:|')
    foreach ($folder in ($pendingByFolder.Keys | Sort-Object)) {
        $coverageLines.Add("| ``$folder`` | $($pendingByFolder[$folder]) |")
    }
    $coverageLines.Add('')
    $coverageLines.Add($documentedTablesTitle)
    $coverageLines.Add('')
    $coverageLines.Add($tableHeader)
    $coverageLines.Add('|---|---|---|---:|---|')
    foreach ($doc in $docInfos) {
        foreach ($info in $doc.TableInfo) {
            $source = if ($info.ParseError) { 'v4 fallback' } else { $info.SourceVersion }
            $cleanName = Get-HelpCleanTableName -Table $info.Table
            $file = $byTable[$info.Table].File
            $coverageLines.Add("| ``$cleanName`` | ``$file`` | ``$($doc.Relative)`` | $($info.Covered)/$($info.Expected) | $source |")
        }
    }
    Write-HelpLines -Path (Join-Path $HelpRoot 'COVERAGE.md') -Lines $coverageLines
}

Write-Host "Documents: $($docInfos.Count)"
Write-Host "Tables documented: $($coveredTables.Count) / $($inventory.Count)"
Write-Host "Field paths documented: $coveredPaths / $totalPaths"

if ($warnings.Count -gt 0) {
    Write-Host ''
    Write-Host "Warnings ($($warnings.Count)):"
    foreach ($warning in $warnings) {
        Write-Host "  $warning"
    }
}

if ($errors.Count -gt 0) {
    Write-Host ''
    Write-Host "Errors ($($errors.Count)):"
    foreach ($errorMessage in $errors) {
        Write-Host "  $errorMessage"
    }
    exit 1
}

Write-Host 'Help check passed.'
exit 0
