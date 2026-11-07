# Validates help documents against the XML table inventory.
# Reports coverage, checks front matter, field blocks and structure order, validates links,
# and (with -Write) rebuilds per-document Contents, INDEX.md and COVERAGE.md.
# Read-only with respect to game data; only help/** is written.

[CmdletBinding()]
param(
    [string]$HelpRoot,
    [string]$TableDataRoot,
    [string]$FallbackTableDataRoot,
    [switch]$Write,
    [switch]$Release
)

Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'

$repoRoot = (Resolve-Path -LiteralPath (Join-Path $PSScriptRoot '..')).Path
if (-not $HelpRoot) {
    $HelpRoot = Join-Path $repoRoot 'xml-editor-reloaded/help'
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
    Where-Object { $_.FullName -match '\\tables\\' })
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
        if ([string]$data['lang'] -ne 'en') {
            $errors.Add("${relative}: release documents must be English")
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
    if (($fieldPaths | Select-Object -Unique).Count -ne $fieldPaths.Count) {
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
        $fieldType = [string]$field.Values['Type']
        $fieldType = $fieldType.Trim('`').Trim()
        if ($fieldType -in @('Integer', 'Decimal', 'Float', 'UINT8', 'Flags')) {
            if (-not $field.Values.ContainsKey('Description') -or
                [string]$field.Values['Description'] -notmatch 'Numeric semantics:') {
                $errors.Add("${relative}: numeric field '$($field.Path)' is missing 'Numeric semantics:' in Description")
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
    $indexLines = [System.Collections.Generic.List[string]]::new()
    $indexLines.Add('# Table help index')
    $indexLines.Add('')
    $indexLines.Add("Generated by ``util-scripts/help-check.ps1 -Write``. Do not edit manually.")
    $indexLines.Add('')
    $indexLines.Add("Documented: $($coveredTables.Count) of $($inventory.Count) tables; $coveredPaths of $totalPaths field paths.")
    $indexLines.Add('')
    $indexLines.Add('| Table | File | Document | Status |')
    $indexLines.Add('|---|---|---|---|')
    foreach ($table in $inventory) {
        $doc = if ($tableOwners.ContainsKey($table.Table)) { $tableOwners[$table.Table] } else { $null }
        $docLink = if ($doc) { "[$doc]($doc)" } else { '—' }
        $status = if ($doc) { 'documented' } else { 'pending' }
        $cleanName = Get-HelpCleanTableName -Table $table.Table
        $indexLines.Add("| ``$cleanName`` | ``$($table.File)`` | $docLink | $status |")
    }
    Write-HelpLines -Path (Join-Path $HelpRoot 'INDEX.md') -Lines $indexLines

    $coverageLines = [System.Collections.Generic.List[string]]::new()
    $coverageLines.Add('# Help coverage')
    $coverageLines.Add('')
    $coverageLines.Add("Generated by ``util-scripts/help-check.ps1 -Write``. Do not edit manually.")
    $coverageLines.Add('')
    $coverageLines.Add('## Summary')
    $coverageLines.Add('')
    $coverageLines.Add('| Metric | Value |')
    $coverageLines.Add('|---|---:|')
    $coverageLines.Add("| Tables in inventory | $($inventory.Count) |")
    $coverageLines.Add("| Tables documented | $($coveredTables.Count) |")
    $coverageLines.Add("| Field paths in inventory | $totalPaths |")
    $coverageLines.Add("| Field paths documented | $coveredPaths |")
    $coverageLines.Add("| Documents | $($docInfos.Count) |")
    $coverageLines.Add('')
    $coverageLines.Add('## Pending tables by folder')
    $coverageLines.Add('')
    $pendingByFolder = @{}
    foreach ($table in $pendingTables) {
        $folder = if ($table.File.Contains('/')) { $table.File.Split('/')[0] } else { '(root)' }
        if (-not $pendingByFolder.ContainsKey($folder)) {
            $pendingByFolder[$folder] = 0
        }
        $pendingByFolder[$folder]++
    }
    $coverageLines.Add('| Folder | Pending |')
    $coverageLines.Add('|---|---:|')
    foreach ($folder in ($pendingByFolder.Keys | Sort-Object)) {
        $coverageLines.Add("| ``$folder`` | $($pendingByFolder[$folder]) |")
    }
    $coverageLines.Add('')
    $coverageLines.Add('## Documented tables')
    $coverageLines.Add('')
    $coverageLines.Add('| Table | File | Document | Paths | Source |')
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
