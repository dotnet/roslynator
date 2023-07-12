dotnet build "../src/VisualStudio.sln" -c Debug -v normal `
 /p:RunCodeAnalysis=false,DeployExtension=false `
 /nr:false `
 /m

if(!$?) { Read-Host; Exit }

$vsixInstallerExe="$Env:ProgramFiles/Microsoft Visual Studio/2022/Community/Common7/IDE/VSIXInstaller"

Write-Host Uninstalling Roslynator...
& $vsixInstallerExe /q /u:d42db039-5432-4399-bb62-67a9b4c3b838
Write-Host Uninstalled Roslynator

if(!$?) { Read-Host; Exit }

Write-Host Installing Roslynator...
& $vsixInstallerExe /q "../src/VisualStudio/bin/Debug/Roslynator.VisualStudio.vsix"
Write-Host Installed Roslynator
