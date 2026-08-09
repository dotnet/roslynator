# Roslynator Command-line Tool

.NET Framework stand-alone application for running [Roslyn](https://github.com/dotnet/roslyn) code analysis from the command line.

## Requirements

.NET Framework 4.8 or higher.

## Installation

No installation is required. Unzip the NuGet package and run `roslynator.exe`.

## Usage

The CLI does not include analyzers (such as [Roslynator.Analyzers](https://www.nuget.org/packages/roslynator.analyzers)).
Reference analyzers as NuGet packages, or add analyzer assemblies with `--analyzer-assemblies`.

Analyze a project or solution:
```shell
roslynator analyze
```

Fix a project or solution:
```shell
roslynator fix
```

See the [CLI documentation](https://josefpihrt.github.io/docs/roslynator/cli) for the full list of commands.

## Feedback

* File an issue on [GitHub](https://github.com/dotnet/roslynator/issues/new)
* Follow on [Twitter](https://twitter.com/roslynator)

## Related Products

* [Roslynator for Visual Studio 2022](https://marketplace.visualstudio.com/items?itemName=josefpihrt.Roslynator2022)
* [Roslynator for VS Code](https://marketplace.visualstudio.com/items?itemName=josefpihrt-vscode.roslynator)
* [Roslynator Testing Framework](https://www.nuget.org/packages/Roslynator.Testing.CSharp.Xunit)
