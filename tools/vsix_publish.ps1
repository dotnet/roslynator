$version = Read-Host -Prompt "Enter version"
$personalAccessToken = Read-Host -Prompt "Enter Personal Access Token"

Clear-Host

& $"$env:ProgramFiles/Microsoft Visual Studio/2022/Community/VSSDK/VisualStudioIntegration/Tools/Bin/VsixPublisher.exe" publish `
 -payload "../src/VisualStudio/bin/Release/Roslynator.VisualStudio.$version.vsix" `
 -publishManifest "../src/VisualStudio/manifest.json" `
 -personalAccessToken $personalAccessToken

Write-Host DONE
