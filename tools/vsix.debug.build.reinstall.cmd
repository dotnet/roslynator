@echo off

"C:\Program Files\Microsoft Visual Studio\2017\Community\MSBuild\15.0\Bin\msbuild" "..\src\VisualStudio\VisualStudio.csproj" ^
 /t:Clean,Build ^
 /p:Configuration=Debug,RunCodeAnalysis=false,DeployExtension=false ^
 /nr:false ^
 /v:normal ^
 /m

if errorlevel 1 (
 pause
 exit
)

set _vsixInstallerExe="C:\Program Files\Microsoft Visual Studio\2017\Community\Common7\IDE\VSIXInstaller"

echo Uninstalling Roslynator...
%_vsixInstallerExe% /q /u:9289a8ab-1bb6-496b-9992-9f7ea27f66a8
echo Uninstalled Roslynator

if errorlevel 1 (
 pause
 exit
)

echo Installing Roslynator...
%_vsixInstallerExe% /q "..\src\VisualStudio\bin\Debug\Roslynator.VisualStudio.vsix"
echo Installed Roslynator

pause