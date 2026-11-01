<#
.SYNOPSIS
Removes scratch data from the repository `.tmp` root.

.DESCRIPTION
`.tmp` holds everything that is not worth keeping in git: probe projects, UI
smoke workspaces, restore logs, failure dumps. It is ignored by git but it grows
fast (a single UI smoke run can be hundreds of megabytes), so it is cleaned at
the end of a task.

The script only ever touches `<repo>/.tmp`. It never deletes the root itself and
refuses to run when `.tmp` is a junction or resolves outside the repository.

What is kept:

  * every top-level item modified within `-KeepHours` (default 1 hour), which
    protects files a still-running job may be writing;
  * the newest `-KeepUiSmoke` run directories inside `.tmp/ui-smoke`
    (default 2), because a failed smoke run is the one you inspect afterwards;
  * every top-level item matching a `-Keep` wildcard.

Everything else is removed. Locked files are reported and turn the exit code
non-zero instead of aborting the cleanup.

.PARAMETER KeepHours
Keep top-level items modified more recently than this many hours. Defaults to 1.
Pass 0 to ignore modification time entirely.

.PARAMETER KeepUiSmoke
Number of newest `.tmp/ui-smoke/<run>` directories to keep. Defaults to 2. Pass 0
to remove the whole `ui-smoke` tree.

.PARAMETER Keep
Wildcards matched against top-level names that must never be removed, for
example `-Keep 'probe*','*.log'`.

.PARAMETER Quiet
Do not list every removed item; print the summary only.

.EXAMPLE
./util-scripts/clean-tmp.ps1 -WhatIf

Shows what would be removed without deleting anything.

.EXAMPLE
./util-scripts/clean-tmp.ps1

Removes everything older than an hour, keeping the two newest UI smoke runs.

.EXAMPLE
./util-scripts/clean-tmp.ps1 -KeepHours 0 -KeepUiSmoke 0

Empties `.tmp` completely.

.NOTES
Exit code is zero when everything planned was removed (or nothing needed
removal) and one when at least one item could not be removed.
#>
[CmdletBinding(SupportsShouldProcess = $true)]
param(
    [double]$KeepHours = 1,

    [int]$KeepUiSmoke = 2,

    [string[]]$Keep = @(),

    [switch]$Quiet
)

Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'

function Get-ItemSize {
    param([System.IO.FileSystemInfo]$Item)

    if (-not $Item.PSIsContainer) {
        return [int64]$Item.Length
    }

    if ($Item.Attributes -band [System.IO.FileAttributes]::ReparsePoint) {
        return [int64]0
    }

    $sum = Get-ChildItem -LiteralPath $Item.FullName -Recurse -File -Force -ErrorAction SilentlyContinue |
        Measure-Object -Property Length -Sum
    return [int64]($sum.Sum)
}

function Format-Size {
    param([int64]$Bytes)

    if ($Bytes -ge 1GB) { return '{0,7:N2} GB' -f ($Bytes / 1GB) }
    if ($Bytes -ge 1MB) { return '{0,7:N1} MB' -f ($Bytes / 1MB) }
    if ($Bytes -ge 1KB) { return '{0,7:N1} KB' -f ($Bytes / 1KB) }
    return '{0,7} B ' -f $Bytes
}

function Remove-ItemSafe {
    param([System.IO.FileSystemInfo]$Item)

    if ($Item.Attributes -band [System.IO.FileAttributes]::ReparsePoint) {
        [System.IO.File]::Delete($Item.FullName)
        return
    }

    if ($Item.PSIsContainer) {
        Remove-Item -LiteralPath $Item.FullName -Recurse -Force
        return
    }

    [System.IO.File]::SetAttributes($Item.FullName, [System.IO.FileAttributes]::Normal)
    Remove-Item -LiteralPath $Item.FullName -Force
}

$repo = Split-Path -Parent $PSScriptRoot
$root = Join-Path $repo '.tmp'

if (-not (Test-Path -LiteralPath $root)) {
    Write-Host "cleanup: $root does not exist, nothing to do"
    exit 0
}

$rootItem = Get-Item -LiteralPath $root -Force
if ($rootItem.Attributes -band [System.IO.FileAttributes]::ReparsePoint) {
    Write-Error "cleanup: $root is a reparse point; refusing to clean through it"
    exit 2
}

$resolvedRoot = $rootItem.FullName.TrimEnd('\')
$resolvedRepo = (Get-Item -LiteralPath $repo -Force).FullName.TrimEnd('\')
if (-not $resolvedRoot.StartsWith($resolvedRepo + '\', [StringComparison]::OrdinalIgnoreCase)) {
    Write-Error "cleanup: $resolvedRoot is outside $resolvedRepo; refusing to clean"
    exit 2
}

$cutoff = (Get-Date).AddHours(-[math]::Abs($KeepHours))

$planned = [System.Collections.Generic.List[object]]::new()
$kept = [System.Collections.Generic.List[object]]::new()

function Add-Planned {
    param([System.IO.FileSystemInfo]$Item, [string]$Reason)

    $planned.Add([pscustomobject]@{
        Path   = $Item.FullName
        Name   = $Item.Name
        Size   = Get-ItemSize -Item $Item
        Reason = $Reason
    })
}

foreach ($item in Get-ChildItem -LiteralPath $root -Force) {
    if ($item.Name -eq 'ui-smoke' -and $item.PSIsContainer -and -not ($item.Attributes -band [System.IO.FileAttributes]::ReparsePoint)) {
        $runs = @(Get-ChildItem -LiteralPath $item.FullName -Directory -Force -ErrorAction SilentlyContinue |
            Sort-Object -Property LastWriteTime -Descending)

        if ($KeepUiSmoke -le 0) {
            Add-Planned -Item $item -Reason 'all UI smoke runs'
            continue
        }

        $index = 0
        foreach ($run in $runs) {
            $index++
            if ($index -le $KeepUiSmoke) {
                $kept.Add([pscustomobject]@{ Name = "ui-smoke\$($run.Name)"; Reason = "newest smoke run #$index" })
                continue
            }

            if ($run.LastWriteTime -gt $cutoff) {
                $kept.Add([pscustomobject]@{ Name = "ui-smoke\$($run.Name)"; Reason = 'modified recently' })
                continue
            }

            Add-Planned -Item $run -Reason 'old smoke run'
        }

        if ($runs.Count -eq 0) {
            $kept.Add([pscustomobject]@{ Name = 'ui-smoke'; Reason = 'empty' })
        }

        continue
    }

    $matched = @($Keep | Where-Object { $_ -and $item.Name -like $_ })
    if ($matched.Count -gt 0) {
        $kept.Add([pscustomobject]@{ Name = $item.Name; Reason = "-Keep $($matched -join ',')" })
        continue
    }

    if ($item.LastWriteTime -gt $cutoff) {
        $kept.Add([pscustomobject]@{ Name = $item.Name; Reason = 'modified recently' })
        continue
    }

    Add-Planned -Item $item -Reason 'old'
}

$plannedBytes = [int64]0
foreach ($entry in $planned) { $plannedBytes += $entry.Size }

Write-Host 'cleanup: repository scratch root'
Write-Host "  root:   $resolvedRoot"
Write-Host ("  policy: keep newer than {0} h, keep {1} UI smoke run(s){2}" -f $KeepHours, $KeepUiSmoke, $(if ($Keep.Count -gt 0) { ", keep $($Keep -join ', ')" } else { '' }))
Write-Host ("  found:  {0} item(s) to remove, {1} kept, {2} total" -f $planned.Count, $kept.Count, (Format-Size ($plannedBytes)))

if ($planned.Count -eq 0) {
    Write-Host 'cleanup: nothing to remove'
    exit 0
}

if ($WhatIfPreference) {
    foreach ($entry in $planned) {
        Write-Host ("  would remove  {0}  {1}  {2}" -f (Format-Size $entry.Size), $entry.Name, $entry.Reason)
    }

    Write-Host ("cleanup: dry run, {0} would be freed" -f (Format-Size ($plannedBytes)))
    exit 0
}

$removedCount = 0
$removedBytes = [int64]0
$failed = [System.Collections.Generic.List[object]]::new()

foreach ($entry in $planned) {
    try {
        $item = Get-Item -LiteralPath $entry.Path -Force
        Remove-ItemSafe -Item $item
        $removedCount++
        $removedBytes += $entry.Size
        if (-not $Quiet) {
            Write-Host ("  removed  {0}  {1}" -f (Format-Size $entry.Size), $entry.Name)
        }
    }
    catch {
        $failed.Add([pscustomobject]@{ Name = $entry.Name; Error = $_.Exception.Message })
    }
}

if (-not $Quiet -and $kept.Count -gt 0) {
    foreach ($entry in $kept) {
        Write-Host ("  kept     {0}  ({1})" -f $entry.Name, $entry.Reason)
    }
}

Write-Host ("cleanup: removed {0} item(s), freed {1}; kept {2}; failed {3}" -f $removedCount, (Format-Size $removedBytes), $kept.Count, $failed.Count)

if ($failed.Count -gt 0) {
    foreach ($entry in $failed) {
        Write-Warning ("could not remove {0}: {1}" -f $entry.Name, $entry.Error)
    }

    exit 1
}

exit 0
