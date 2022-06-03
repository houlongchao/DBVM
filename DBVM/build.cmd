@echo off

dotnet publish -r win-x64 -f net6.0 -c Release -p:PublishSingleFile=true -p:PublishTrimmed=true --self-contained true
pause