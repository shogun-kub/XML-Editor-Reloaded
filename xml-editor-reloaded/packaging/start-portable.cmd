@echo off
setlocal

if not exist "%~dp0XmlEditorReloaded.App.exe" (
    echo XmlEditorReloaded.App.exe was not found next to this launcher.
    pause
    exit /b 1
)

set "XER_APP_DATA=%~dp0data"
start "" "%~dp0XmlEditorReloaded.App.exe"
endlocal
