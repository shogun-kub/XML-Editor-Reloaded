# Regenerates the inventory section of CLASS-MAP.md from the C# sources.
# The introductory architecture text is maintained by hand between the markers.

[CmdletBinding()]
param()

$ErrorActionPreference = 'Stop'
$root = Split-Path $PSScriptRoot -Parent
$mapPath = Join-Path $root 'CLASS-MAP.md'
$startMarker = '<!-- BEGIN GENERATED INVENTORY -->'
$endMarker = '<!-- END GENERATED INVENTORY -->'
$testStartMarker = '<!-- BEGIN GENERATED TEST INVENTORY -->'
$testEndMarker = '<!-- END GENERATED TEST INVENTORY -->'

$typePattern = '^\s*(?<access>public|internal)\s+(?<mods>(?:(?:sealed|static|abstract|partial|readonly|ref)\s+)*)(?<kind>record\s+struct|record\s+class|class|record|interface|enum|struct)\s+(?<name>[A-Za-z_][A-Za-z0-9_]*)'
$emptyDescriptions = @{
    CharacterCounterConverter = 'Formats the remaining-character counter shown next to text fields.'
    Program = 'CLI entry point of the help generator and release checker.'
}

function Get-TypeRows([string]$projectPath) {
    $projectDir = (Resolve-Path (Join-Path $root $projectPath)).Path
    $projectName = Split-Path $projectDir -Leaf
    $rows = [System.Collections.Generic.List[object]]::new()
    $files = Get-ChildItem $projectDir -Recurse -Filter '*.cs' |
        Where-Object { $_.FullName -notmatch '\\(obj|bin)\\' } |
        Sort-Object FullName
    foreach ($file in $files) {
        $lines = Get-Content -LiteralPath $file.FullName
        for ($i = 0; $i -lt $lines.Count; $i++) {
            if ($lines[$i] -notmatch $typePattern) { continue }
            $kind = $Matches.kind -replace 'record class', 'record'
            $name = $Matches.name
            if ($name -match '^(AssemblyInfo)$') { continue }

            $commentLines = [System.Collections.Generic.List[string]]::new()
            for ($j = $i - 1; $j -ge [Math]::Max(0, $i - 60); $j--) {
                if ($lines[$j] -match '^\s*///') { $commentLines.Insert(0, $lines[$j].Trim()) }
                elseif ($commentLines.Count -gt 0) { break }
            }

            $comment = $commentLines -join ' '
            $summary = ''
            $match = [regex]::Match($comment, '<summary>(.*?)</summary>', [System.Text.RegularExpressions.RegexOptions]::Singleline)
            if ($match.Success) {
                $summary = $match.Groups[1].Value
                $summary = [regex]::Replace($summary, '<see\s+cref="[^"]*?([A-Za-z0-9_]+)"\s*/>', '$1')
                $summary = [regex]::Replace($summary, '<see\s+langword="([^"]+)"\s*/>', '$1')
                $summary = [regex]::Replace($summary, '<paramref\s+name="[^"]*"\s*/>', 'parameter')
                $summary = $summary -replace '///', ' '
                $summary = $summary -replace '<[^>]+>', ' '
                $summary = [System.Net.WebUtility]::HtmlDecode($summary)
                $summary = ($summary -replace '\s+', ' ').Trim()
            }

            if ([string]::IsNullOrWhiteSpace($summary)) {
                $summary = $emptyDescriptions[$name]
            }

            if ([string]::IsNullOrWhiteSpace($summary)) {
                $summary = 'Описание в XML-комментарии отсутствует.'
            }

            if ($summary.Length -gt 300) {
                $summary = $summary.Substring(0, 297) + '...'
            }

            $summary = $summary.Replace('|', '\|')
            $relative = $file.FullName.Substring($projectDir.Length + 1).Replace('\', '/')
            $folder = [System.IO.Path]::GetDirectoryName($relative)?.Replace('\', '/')
            if ([string]::IsNullOrWhiteSpace($folder)) { $folder = '.' }
            $rows.Add([pscustomobject]@{
                Project = $projectName
                Folder = $folder
                Kind = $kind
                Name = $name
                Summary = $summary
                File = $relative
                Line = $i + 1
            })
        }
    }

    return $rows
}

function Format-Inventory([string[]]$projectPaths, [string]$headingLevel) {
    $output = [System.Text.StringBuilder]::new()
    foreach ($projectPath in $projectPaths) {
        $rows = @(Get-TypeRows $projectPath | Sort-Object Folder, Name, Line)
        $projectName = Split-Path $projectPath -Leaf
        if ($projectPath -eq 'xml-editor-reloaded/tools/XmlEditorReloaded.HelpTool') {
            [void]$output.AppendLine("$headingLevel $projectName (1 тип)")
        }
        else {
            [void]$output.AppendLine("$headingLevel $projectName ($($rows.Count) типов)")
        }

        [void]$output.AppendLine()

        if ($projectPath -eq 'xml-editor-reloaded/tools/XmlEditorReloaded.HelpTool') {
            [void]$output.AppendLine('| Тип и класс | Назначение | Исходник |')
            [void]$output.AppendLine('|---|---|---|')
            [void]$output.AppendLine('| `Program` (class) | CLI entry point of the help generator and release checker. | [Program.cs](xml-editor-reloaded/tools/XmlEditorReloaded.HelpTool/Program.cs) |')
            [void]$output.AppendLine()
            continue
        }

        $folder = ''
        foreach ($row in $rows) {
            if ($row.Folder -ne $folder) {
                if ($folder.Length -gt 0) {
                    [void]$output.AppendLine()
                }

                $folder = $row.Folder
                [void]$output.AppendLine("#### ``$folder``")
                [void]$output.AppendLine()
                [void]$output.AppendLine('| Тип и класс | Назначение | Исходник |')
                [void]$output.AppendLine('|---|---|---|')
            }

            $source = "$projectPath/$($row.File)"
            $label = [System.IO.Path]::GetFileName($row.File)
            [void]$output.AppendLine("| ``$($row.Name)`` ($($row.Kind)) | $($row.Summary) | [$label`:$($row.Line)]($source#L$($row.Line)) |")
        }

        [void]$output.AppendLine()
    }

    return $output.ToString().TrimEnd()
}

if (-not (Test-Path -LiteralPath $mapPath)) {
    throw "CLASS-MAP.md not found at $mapPath."
}

$map = [System.IO.File]::ReadAllText($mapPath)
if (-not $map.Contains($startMarker) -or -not $map.Contains($endMarker) -or
    -not $map.Contains($testStartMarker) -or -not $map.Contains($testEndMarker)) {
    throw 'CLASS-MAP.md does not contain all generated-inventory markers.'
}

$production = @(
    'xml-editor-reloaded/src/XmlEditorReloaded.Core',
    'xml-editor-reloaded/src/XmlEditorReloaded.Xml',
    'xml-editor-reloaded/src/XmlEditorReloaded.Data',
    'xml-editor-reloaded/src/XmlEditorReloaded.Imaging',
    'xml-editor-reloaded/src/XmlEditorReloaded.App',
    'xml-editor-reloaded/tools/XmlEditorReloaded.HelpTool'
)
$tests = @(
    'xml-editor-reloaded/tests/XmlEditorReloaded.Core.Tests',
    'xml-editor-reloaded/tests/XmlEditorReloaded.Xml.Tests',
    'xml-editor-reloaded/tests/XmlEditorReloaded.Data.Tests',
    'xml-editor-reloaded/tests/XmlEditorReloaded.Imaging.Tests',
    'xml-editor-reloaded/tests/XmlEditorReloaded.App.Tests',
    'xml-editor-reloaded/tests/XmlEditorReloaded.UiSmoke'
)

$inventory = Format-Inventory $production '###'
$testInventory = Format-Inventory $tests '###'

$map = [regex]::Replace(
    $map,
    [regex]::Escape($startMarker) + '.*?' + [regex]::Escape($endMarker),
    { param($match) "$startMarker`r`n`r`n$inventory`r`n`r`n$endMarker" },
    [System.Text.RegularExpressions.RegexOptions]::Singleline)
$map = [regex]::Replace(
    $map,
    [regex]::Escape($testStartMarker) + '.*?' + [regex]::Escape($testEndMarker),
    { param($match) "$testStartMarker`r`n`r`n$testInventory`r`n`r`n$testEndMarker" },
    [System.Text.RegularExpressions.RegexOptions]::Singleline)

[System.IO.File]::WriteAllText($mapPath, $map, [System.Text.UTF8Encoding]::new($false))
Write-Host "Updated $mapPath"
