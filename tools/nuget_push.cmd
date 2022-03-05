@echo off

set /p _apiKey=Enter API key:
set _nugetSource=https://api.nuget.org/v3/index.json

dotnet nuget push "..\out\release\Roslynator.Core.*.nupkg" --source %_nugetSource% --api-key %_apiKey%
dotnet nuget push "..\out\release\Roslynator.CSharp.*.nupkg" --source %_nugetSource% --api-key %_apiKey%
dotnet nuget push "..\out\release\Roslynator.Workspaces.Core.*.nupkg" --source %_nugetSource% --api-key %_apiKey%
dotnet nuget push "..\out\release\Roslynator.CSharp.Workspaces.*.nupkg" --source %_nugetSource% --api-key %_apiKey%

dotnet nuget push "..\out\release\Roslynator.Testing.Common.*.nupkg" --source %_nugetSource% --api-key %_apiKey%
dotnet nuget push "..\out\release\Roslynator.Testing.CSharp.*.nupkg" --source %_nugetSource% --api-key %_apiKey%
dotnet nuget push "..\out\release\Roslynator.Testing.CSharp.Xunit.*.nupkg" --source %_nugetSource% --api-key %_apiKey%

dotnet nuget push "..\out\release\Roslynator.Analyzers.*.nupkg" --source %_nugetSource% --api-key %_apiKey%
dotnet nuget push "..\out\release\Roslynator.Formatting.Analyzers.*.nupkg" --source %_nugetSource% --api-key %_apiKey%
dotnet nuget push "..\out\release\Roslynator.CodeAnalysis.Analyzers.*.nupkg" --source %_nugetSource% --api-key %_apiKey%

pause

