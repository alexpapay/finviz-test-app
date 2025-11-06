@ECHO off
SETLOCAL ENABLEDELAYEDEXPANSION

REM Confirmation prompt
echo Are you sure you want to remove the latest migration? (Y/N)
choice /C YN /M "Confirm action:"
if errorlevel 2 (
    echo Migration removal canceled.
    exit /b
)

REM Remove the migration
dotnet ef migrations remove --project Finviz.TestApp.ImageNet.Persistence --startup-project Finviz.TestApp.ImageNet.Api --verbose

ENDLOCAL