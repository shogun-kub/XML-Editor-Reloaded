[CmdletBinding()]
param(
    [string]$Root = (Join-Path $env:USERPROFILE 'XER-manual-smoke'),
    [switch]$V5,
    [switch]$Reset,
    [switch]$RefreshData,
    [switch]$NoLaunch
)

$ErrorActionPreference = 'Stop'

$repo = Split-Path -Parent $PSScriptRoot
$references = Join-Path $repo 'References'
$appExe = Join-Path $repo 'out\bin\XmlEditorReloaded.App\debug\XmlEditorReloaded.App.exe'

if (-not (Test-Path -LiteralPath $references)) {
    throw "Не найден каталог References: $references"
}

if (-not (Test-Path -LiteralPath $appExe)) {
    throw "Не найдено приложение: $appExe. Сначала соберите решение."
}

# A low-integrity process cannot write to the normal user profile and would produce
# SQLite Error 14 / io.access_denied in the editor data directory.
$groups = & whoami /groups 2>$null
if ($groups | Where-Object { $_ -match 'S-1-16-4096' }) {
    throw @'
Приложение запускается из процесса с Low integrity, поэтому каталог данных в профиле
пользователя будет недоступен. Запустите manual-smoke.cmd из обычного Проводника или
нового окна PowerShell, а не из терминала агента/DSH.
'@
}

$rootFull = [System.IO.Path]::GetFullPath($Root)
$rootOfDrive = [System.IO.Path]::GetPathRoot($rootFull)
if ([string]::Equals($rootFull.TrimEnd('\'), $rootOfDrive.TrimEnd('\'), [StringComparison]::OrdinalIgnoreCase)) {
    throw "Нельзя использовать корень диска как рабочий каталог: $rootFull"
}

if ($rootFull.StartsWith(
        [System.IO.Path]::GetFullPath($references).TrimEnd('\') + '\',
        [StringComparison]::OrdinalIgnoreCase)) {
    throw "Рабочий каталог не может находиться внутри References: $rootFull"
}

$app = Get-Process -Name 'XmlEditorReloaded.App' -ErrorAction SilentlyContinue
if ($app) {
    throw "Редактор уже запущен (PID: $($app.Id -join ', ')). Закройте его и повторите запуск."
}

if ($Reset -and (Test-Path -LiteralPath $rootFull)) {
    Remove-Item -LiteralPath $rootFull -Recurse -Force
}

$v4Root = Join-Path $rootFull 'v4'
$v5Root = Join-Path $rootFull 'v5'
$appData = Join-Path $rootFull 'appdata'
New-Item -ItemType Directory -Force -Path $rootFull, $appData | Out-Null

# The manual scenarios only read tables and item sprites. Copying the whole game directory
# would take several gigabytes and tens of thousands of files, so only the folders the
# editor consumes are materialized: tables, big sprites, medium sprites, and small sprites.
$copiedFolders = @(
    'Data-1.13\TableData',
    'Data-1.13\BigItems',
    'Data-1.13\Interface',
    'Data-1.13\tilesets\0'
)

function Copy-DataSet {
    param(
        [Parameter(Mandatory)] [string]$ReferenceName,
        [Parameter(Mandatory)] [string]$Target
    )

    $source = Join-Path $references $ReferenceName
    if (-not (Test-Path -LiteralPath $source)) {
        throw "Не найден датасет References\$ReferenceName"
    }

    $tableData = Join-Path $Target 'Data-1.13\TableData'
    if (-not $RefreshData -and (Test-Path -LiteralPath $tableData)) {
        Write-Host "Датасет уже подготовлен: $Target"
        return
    }

    if (Test-Path -LiteralPath $Target) {
        Remove-Item -LiteralPath $Target -Recurse -Force
    }

    New-Item -ItemType Directory -Force -Path $Target | Out-Null
    Write-Host "Копирую $ReferenceName -> $Target ..."
    foreach ($relative in $copiedFolders) {
        $from = Join-Path $source $relative
        if (-not (Test-Path -LiteralPath $from)) {
            throw "В датасете References\$ReferenceName отсутствует $relative"
        }

        $to = Join-Path $Target $relative
        New-Item -ItemType Directory -Force -Path (Split-Path -Parent $to) | Out-Null
        Copy-Item -LiteralPath $from -Destination $to -Recurse -Force
        Write-Host "  $relative"
    }
}

Copy-DataSet -ReferenceName 'JA 2 1.13 v4' -Target $v4Root
if ($V5) {
    Copy-DataSet -ReferenceName 'JA 2 1.13 v5' -Target $v5Root
}

$dataDirectories = @($v4Root)
if ($V5) {
    $dataDirectories += $v5Root
}

$settings = [ordered]@{
    dataDirectories       = $dataDirectories
    activeDataDirectory   = $v4Root
    activeLanguageCode    = $null
    uiLanguageCode        = 'ru'
    recentDataDirectories = @($v4Root)
}

$settingsPath = Join-Path $appData 'settings.json'
$json = $settings | ConvertTo-Json -Depth 5
[System.IO.File]::WriteAllText(
    $settingsPath,
    $json,
    [System.Text.UTF8Encoding]::new($false))

$readmePath = Join-Path $rootFull 'README.txt'
$readme = @"
XER manual smoke environment
============================

Prepared by util-scripts/manual-smoke.ps1.

v4 data set:   $v4Root
AppData:       $appData
Settings:      $settingsPath

The data set is a minimal copy: TableData, BigItems, Interface, tilesets\0.
It is used only for read-only manual scenarios; the originals in References are untouched.

For S83 edit $settingsPath while the editor is closed.

To rebuild this directory, run from a normal PowerShell:
  .\util-scripts\manual-smoke.cmd -Reset
"@
[System.IO.File]::WriteAllText(
    $readmePath,
    $readme,
    [System.Text.UTF8Encoding]::new($false))

Write-Host ''
Write-Host 'Подготовлено:'
Write-Host "  Рабочий каталог: $rootFull"
Write-Host "  v4:             $v4Root"
if ($V5) {
    Write-Host "  v5:             $v5Root"
}
Write-Host "  AppData:        $appData"
Write-Host "  Settings:       $settingsPath"
Write-Host "  README:         $readmePath"
Write-Host ''

if ($NoLaunch) {
    Write-Host 'Запуск пропущен (-NoLaunch).'
    exit 0
}

$env:XER_APP_DATA = $appData
Write-Host "Запускаю $appExe"
Start-Process -FilePath $appExe -WorkingDirectory (Split-Path -Parent $appExe)
Write-Host 'Приложение запущено. Это окно можно закрыть.'
