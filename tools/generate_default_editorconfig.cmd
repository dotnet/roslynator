@echo off

dotnet build "..\src\Tools\EditorConfigGenerator\EditorConfigGenerator.csproj" -c Release

"..\src\Tools\EditorConfigGenerator\bin\Release\net461\Roslynator.EditorConfigGenerator.exe" "..\src"
