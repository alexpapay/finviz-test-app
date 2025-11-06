@ECHO off
SETLOCAL ENABLEDELAYEDEXPANSION

set /p migration="Enter name of new migration: "

dotnet ef migrations add !migration! --project Finviz.TestApp.ImageNet.Persistence --startup-project Finviz.TestApp.ImageNet.Api --verbose

ENDLOCAL