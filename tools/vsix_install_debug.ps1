$msbuildExe = ./vswhere.exe -latest -prerelease -products * -requires Microsoft.Component.MSBuild -find MSBuild\**\Bin\MSBuild.exe
Write-Host "MSBuild location: $msbuildExe"

& $msbuildExe "../src/VisualStudio.sln" `
 /p:Configuration=Debug,RunCodeAnalysis=false,DeployExtension=false `
 /v:minimal `
 /nr:false `
 /m

if(!$?) { Read-Host; Exit }

Write-Host Installing Roslynator...
& "$Env:ProgramFiles/Microsoft Visual Studio/2022/Community/Common7/IDE/VSIXInstaller" /q "../src/VisualStudio/bin/Debug/net472/Roslynator.VisualStudio.vsix"
Write-Host Installed Roslynator
