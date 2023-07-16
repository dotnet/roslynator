$apiKey = Read-Host -Prompt "Enter API key"
$nugetSource = https://api.nuget.org/v3/index.json

dotnet nuget push "../out/release/Roslynator.Core.*.nupkg" --source $nugetSource --api-key $apiKey
dotnet nuget push "../out/release/Roslynator.CSharp.*.nupkg" --source $nugetSource --api-key $apiKey
dotnet nuget push "../out/release/Roslynator.Workspaces.Core.*.nupkg" --source $nugetSource --api-key $apiKey
dotnet nuget push "../out/release/Roslynator.CSharp.Workspaces.*.nupkg" --source $nugetSource --api-key $apiKey

dotnet nuget push "../out/release/Roslynator.Testing.Common.*.nupkg" --source $nugetSource --api-key $apiKey
dotnet nuget push "../out/release/Roslynator.Testing.CSharp.*.nupkg" --source $nugetSource --api-key $apiKey
dotnet nuget push "../out/release/Roslynator.Testing.CSharp.Xunit.*.nupkg" --source $nugetSource --api-key $apiKey
dotnet nuget push "../out/release/Roslynator.Testing.CSharp.MSTest.*.nupkg" --source $nugetSource --api-key $apiKey

dotnet nuget push "../out/release/Roslynator.Analyzers.*.nupkg" --source $nugetSource --api-key $apiKey
dotnet nuget push "../out/release/Roslynator.Formatting.Analyzers.*.nupkg" --source $nugetSource --api-key $apiKey
dotnet nuget push "../out/release/Roslynator.CodeAnalysis.Analyzers.*.nupkg" --source $nugetSource --api-key $apiKey

dotnet nuget push "../out/release/Roslynator.CommandLine.*.nupkg" --source $nugetSource --api-key $apiKey
dotnet nuget push "../out/release/Roslynator.DotNet.Cli.*.nupkg" --source $nugetSource --api-key $apiKey

Write-Host DONE
