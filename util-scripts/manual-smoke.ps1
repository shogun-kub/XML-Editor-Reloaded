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

$v4 = Join-Path $rootFull 'v4'
$v5 = Join-Path $rootFull 'v5'
$appData = Join-Path $rootFull 'appdata'
New-Item -ItemType Directory -Force -Path $rootFull, $appData | Out-Null

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

    Write-Host "Копирую $ReferenceName -> $Target ..."
    Copy-Item -Path (Join-Path $source '*') -Destination $Target -Recurse -Force
}

Copy-DataSet -ReferenceName 'JA 2 1.13 v4' -Target $v4
if ($V5) {
    Copy-DataSet -ReferenceName 'JA 2 1.13 v5' -Target $v5
}

$dataDirectories = @($v4)
if ($V5) {
    $dataDirectories += $v5
}

$settings = [ordered]@{
    dataDirectories       = $dataDirectories
    activeDataDirectory   = $v4
    activeLanguageCode    = $null
    uiLanguageCode        = 'ru'
    recentDataDirectories = @($v4)
}

$settingsPath = Join-Path $appData 'settings.json'
$json = $settings | ConvertTo-Json -Depth 5
[System.IO.File]::WriteAllText(
    $settingsPath,
    $json,
    [System.Text.UTF8Encoding]::new($false))

Write-Host ''
Write-Host 'Подготовлено:'
Write-Host "  Рабочий каталог: $rootFull"
Write-Host "  v4:             $v4"
if ($V5) {
    Write-Host "  v5:             $v5"
}
Write-Host "  AppData:        $appData"
Write-Host "  Settings:       $settingsPath"
Write-Host ''

if ($NoLaunch) {
    Write-Host 'Запуск пропущен (-NoLaunch).'
    exit 0
}

$env:XER_APP_DATA = $appData
Write-Host "Запускаю $appExe"
Start-Process -FilePath $appExe -WorkingDirectory (Split-Path -Parent $appExe)
Write-Host 'Приложение запущено. Это окно можно закрыть.'
