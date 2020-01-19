@echo off

cd ..\src\VisualStudioCode\package

del /S /Q roslyn\*.dll

del /S /Q roslynator-*.vsix

if errorlevel 1 (
 pause
 exit
)
