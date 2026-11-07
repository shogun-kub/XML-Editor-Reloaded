# Shared helpers for the TableData help tooling.
# English-only identifiers; comments are English as well.

Set-StrictMode -Version Latest

$script:OverlayPattern = '^(Russian|English|German|French|Italian|Spanish|Polish|Dutch|Chinese|Japanese)\.'

function Get-HelpLeafPaths {
    param(
        [Parameter(Mandatory)][System.Xml.XmlNode]$Node,
        [string]$Prefix,
        [Parameter(Mandatory)][AllowEmptyCollection()][System.Collections.Generic.List[object]]$Result
    )

    $rawPath = if ([string]::IsNullOrEmpty($Prefix)) { $Node.LocalName } else { "$Prefix/$($Node.LocalName)" }
    $path = [string]$rawPath

    $children = [System.Collections.Generic.List[System.Xml.XmlNode]]::new()
    $textBuilder = [System.Text.StringBuilder]::new()
    foreach ($child in $Node.ChildNodes) {
        if ($child.NodeType -eq [System.Xml.XmlNodeType]::Element) {
            $children.Add($child)
        }
        elseif ($child.NodeType -eq [System.Xml.XmlNodeType]::Text -or
                $child.NodeType -eq [System.Xml.XmlNodeType]::CDATA) {
            [void]$textBuilder.Append($child.Value)
        }
    }

    if ($children.Count -eq 0) {
        $text = $textBuilder.ToString().Trim()

        $Result.Add([pscustomobject]@{
            Path = $path
            HasText = $text.Length -gt 0
            Sample = if ($text.Length -gt 0) { $text.Substring(0, [Math]::Min(80, $text.Length)) } else { '' }
        })
        return
    }

    foreach ($child in $children) {
        Get-HelpLeafPaths -Node $child -Prefix ([string]$path) -Result $Result
    }
}

function Read-HelpXmlTable {
    param(
        [Parameter(Mandatory)][string]$Path,
        [Parameter(Mandatory)][string]$Table,
        [Parameter(Mandatory)][string]$RelativePath,
        [string]$SourceVersion = 'v5'
    )

    $document = [System.Xml.XmlDocument]::new()
    $document.PreserveWhitespace = $true
    $document.Load($Path)

    $root = $document.DocumentElement
    $paths = [System.Collections.Generic.List[object]]::new()
    Get-HelpLeafPaths -Node $root -Prefix '' -Result $paths

    $topLevelNames = [System.Collections.Generic.HashSet[string]]::new([System.StringComparer]::Ordinal)
    foreach ($child in $root.ChildNodes) {
        if ($child.NodeType -eq [System.Xml.XmlNodeType]::Element) {
            [void]$topLevelNames.Add($child.LocalName)
        }
    }
    $topLevel = @($topLevelNames | Sort-Object)

    $uniquePaths = @($paths |
        Group-Object Path |
        ForEach-Object { $_.Group[0] } |
        Sort-Object Path)

    return [pscustomobject]@{
        Table = $Table
        File = $RelativePath
        Root = $root.LocalName
        TopLevelChildren = $topLevel
        Paths = @($uniquePaths | ForEach-Object { $_.Path })
        Leaves = @($uniquePaths)
        ParseError = $null
        SourceVersion = $SourceVersion
    }
}

function Get-HelpTableInventory {
    [CmdletBinding()]
    param(
        [Parameter(Mandatory)][string]$TableDataRoot,
        [string]$FallbackTableDataRoot
    )

    $root = (Resolve-Path -LiteralPath $TableDataRoot).Path
    $fallback = if ($FallbackTableDataRoot -and (Test-Path -LiteralPath $FallbackTableDataRoot)) {
        (Resolve-Path -LiteralPath $FallbackTableDataRoot).Path
    } else {
        $null
    }

    $tables = [System.Collections.Generic.List[object]]::new()
    foreach ($file in Get-ChildItem -LiteralPath $root -Recurse -File -Filter *.xml) {
        $relativePath = [System.IO.Path]::GetRelativePath($root, $file.FullName).Replace('\', '/')
        $fileName = [System.IO.Path]::GetFileName($relativePath)
        if ($fileName -match $script:OverlayPattern) {
            continue
        }

        $table = $relativePath.Substring(0, $relativePath.Length - 4).Replace('/', '.')
        try {
            $tables.Add((Read-HelpXmlTable -Path $file.FullName -Table $table -RelativePath $relativePath -SourceVersion 'v5'))
        }
        catch {
            $message = $_.Exception.Message
            if ($fallback) {
                $fallbackFile = Join-Path $fallback $relativePath
                if (Test-Path -LiteralPath $fallbackFile) {
                    try {
                        $item = Read-HelpXmlTable -Path $fallbackFile -Table $table -RelativePath $relativePath -SourceVersion 'v4-fallback'
                        $item.ParseError = $message
                        $tables.Add($item)
                        continue
                    }
                    catch {
                        $message = "$message; fallback failed: $($_.Exception.Message)"
                    }
                }
            }

            $tables.Add([pscustomobject]@{
                Table = $table
                File = $relativePath
                Root = $null
                TopLevelChildren = @()
                Paths = @()
                Leaves = @()
                ParseError = $message
                SourceVersion = 'v5'
            })
        }
    }

    return @($tables | Sort-Object Table)
}

function Read-HelpFrontMatter {
    param([Parameter(Mandatory)][string]$Path)

    $lines = [System.IO.File]::ReadAllLines($Path)
    if ($lines.Count -lt 3 -or $lines[0].Trim() -ne '---') {
        return $null
    }

    $end = -1
    for ($i = 1; $i -lt $lines.Count; $i++) {
        if ($lines[$i].Trim() -eq '---') {
            $end = $i
            break
        }
    }
    if ($end -lt 0) {
        return $null
    }

    $data = @{}
    $currentKey = $null
    for ($i = 1; $i -lt $end; $i++) {
        $line = $lines[$i]
        if ($line -match '^\s*-\s+(.*)$') {
            if ($currentKey) {
                $data[$currentKey] = @($data[$currentKey]) + $matches[1].Trim().Trim('"').Trim("'")
            }
            continue
        }
        if ($line -match '^([A-Za-z_][A-Za-z0-9_]*):\s*(.*)$') {
            $key = $matches[1]
            $value = $matches[2].Trim()
            if ($value.Length -eq 0) {
                $data[$key] = @()
                $currentKey = $key
            }
            else {
                $data[$key] = $value.Trim('"').Trim("'")
                $currentKey = $null
            }
        }
    }

    return [pscustomobject]@{
        Data = $data
        Lines = $lines
        BodyStart = $end + 1
    }
}

function Get-HelpCleanTableName {
    param([Parameter(Mandatory)][string]$Table)

    $dot = $Table.LastIndexOf('.')
    if ($dot -ge 0 -and $dot -lt ($Table.Length - 1)) {
        return $Table.Substring($dot + 1)
    }
    return $Table
}

function ConvertTo-HelpAnchor {
    param([Parameter(Mandatory)][string]$Text)

    $value = $Text.ToLowerInvariant()
    $value = $value -replace '`', ''
    $value = $value -replace '[^\p{L}\p{Nd}\s_-]', ''
    $value = $value -replace '\s+', '-'
    $value = $value -replace '-+', '-'
    return $value.Trim('-')
}

function Get-HelpHeadings {
    param(
        [Parameter(Mandatory)][AllowEmptyCollection()][AllowEmptyString()][string[]]$Lines,
        [int]$Start = 0
    )

    $headings = [System.Collections.Generic.List[object]]::new()
    for ($i = $Start; $i -lt $Lines.Count; $i++) {
        if ($Lines[$i] -match '^(##|###)\s+(.+?)\s*$') {
            $text = $matches[2].Trim()
            $headings.Add([pscustomobject]@{
                Level = $matches[1].Length
                Text = $text
                Anchor = ConvertTo-HelpAnchor $text
                Line = $i
            })
        }
    }
    return $headings
}

function Get-HelpDocFields {
    param(
        [Parameter(Mandatory)][AllowEmptyCollection()][AllowEmptyString()][string[]]$Lines,
        [int]$Start = 0
    )

    $fields = [System.Collections.Generic.List[object]]::new()
    $current = $null

    for ($i = $Start; $i -lt $Lines.Count; $i++) {
        $line = $Lines[$i]
        if ($line -match '^###\s+`([^`]+)`\s*$') {
            $current = [pscustomobject]@{
                Path = $matches[1].Trim()
                Values = @{}
                Line = $i
            }
            $fields.Add($current)
            continue
        }
        if ($line -match '^##\s+') {
            $current = $null
            continue
        }
        if ($null -ne $current -and $line -match '^- \*\*([^*]+):\*\*\s*(.*)$') {
            $current.Values[$matches[1].Trim()] = $matches[2].Trim()
        }
    }

    return $fields
}

function Get-HelpMarkdownLinks {
    param([Parameter(Mandatory)][AllowEmptyString()][string]$Content)

    $links = [System.Collections.Generic.List[string]]::new()
    foreach ($match in [regex]::Matches($Content, '\[[^\]]*\]\(([^)]+)\)')) {
        $links.Add($match.Groups[1].Value.Trim())
    }
    return $links
}

function Write-HelpLines {
    param(
        [Parameter(Mandatory)][string]$Path,
        [Parameter(Mandatory)][AllowEmptyCollection()][AllowEmptyString()][string[]]$Lines
    )

    $text = ($Lines -join "`n") + "`n"
    [System.IO.File]::WriteAllText($Path, $text, [System.Text.UTF8Encoding]::new($false))
}

