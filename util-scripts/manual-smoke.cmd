@echo off
setlocal
where pwsh >nul 2>nul
if errorlevel 1 (
    set "PS=powershell"
) else (
    set "PS=pwsh"
)

rem -File passes arguments as literal strings, so switch parameters such as -Reset
rem cannot bind ("Cannot convert System.String to SwitchParameter"). Invoke the
rem script through -Command so PowerShell parses the arguments itself.
"%PS%" -NoProfile -ExecutionPolicy Bypass -Command "& '%~dp0manual-smoke.ps1' %*"
set "code=%ERRORLEVEL%"
if not "%code%"=="0" (
    echo.
    echo Preparation failed. Press any key to close.
    pause >nul
)
exit /b %code%
