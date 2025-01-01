# Roslynator Command-line Tool

.NET Core global tool that allows to run [Roslyn](https://github.com/dotnet/roslyn) code analysis from command line.

## Requirements

.NET Core SDK 7, 8 or 9.

## Installation

Run following command to install Roslynator command-line tool:
```shell
dotnet tool install -g roslynator.dotnet.cli
```

## Usage

Roslynator command-line tool does not contain any analyzers (such as [Roslynator.Analyzers](https://www.nuget.org/packages/roslynator.analyzers)).
Analyzers are either referenced as NuGet packages or it is possible to add analyzer assemblies with parameter `--analyzer-assemblies`.

Analyze project/solution:
```shell
roslynator analyze
```

Fix project/solution:
```shell
roslynator fix
```

See [documentation](https://josefpihrt.github.io/docs/roslynator/cli) for a full list of commands.

## Feedback

* File an issue on [GitHub](https://github.com/dotnet/roslynator/issues/new)
* Follow on [Twitter](https://twitter.com/roslynator)

## Related Products

* [Roslynator for Visual Studio 2022](https://marketplace.visualstudio.com/items?itemName=josefpihrt.Roslynator2022)
* [Roslynator for VS Code](https://marketplace.visualstudio.com/items?itemName=josefpihrt-vscode.roslynator)
* [Roslynator Testing Framework](https://www.nuget.org/packages/Roslynator.Testing.CSharp.Xunit)
