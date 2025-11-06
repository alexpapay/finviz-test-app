@ECHO off
SETLOCAL ENABLEDELAYEDEXPANSION

set /p migration="Enter the name of the migration to rollback to (if empty, will update database only): "

dotnet ef database update !migration! --project Finviz.TestApp.ImageNet.Persistence --startup-project Finviz.TestApp.ImageNet.Api --verbose

ENDLOCAL