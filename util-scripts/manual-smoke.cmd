@echo off
setlocal
where pwsh >nul 2>nul
if errorlevel 1 (
    set "PS=powershell"
) else (
    set "PS=pwsh"
)

"%PS%" -NoProfile -ExecutionPolicy Bypass -File "%~dp0manual-smoke.ps1" %*
set "code=%ERRORLEVEL%"
if not "%code%"=="0" (
    echo.
    echo Preparation failed. Press any key to close.
    pause >nul
)
exit /b %code%
