# Roslynator Command-line Tool

.NET Core global tool that allows to run [Roslyn](https://github.com/dotnet/roslyn) code analysis from command line.

## Requirements

.NET Core SDK 5.0 or 6.0.

## Installation

Run following command to install Roslynator command-line tool:
```shell
dotnet tool install -g roslynator.dotnet.cli
```

## Usage

Analyze project/solution:
```shell
roslynator analyze
```

Fix project/solution:
```shell
roslynator fix
```

See [documentation](https://github.com/josefpihrt/roslynator/blob/main/docs/cli/README.md) for a full list of commands.

## Feedback

* File an issue on [GitHub](https://github.com/josefpihrt/roslynator/issues/new)
* Follow on [Twitter](https://twitter.com/roslynator)

## Related Products

* [Roslynator for Visual Studio 2022](https://marketplace.visualstudio.com/items?itemName=josefpihrt.Roslynator2022)
* [Roslynator for VS Code](https://marketplace.visualstudio.com/items?itemName=josefpihrt-vscode.roslynator)
* [Roslynator Testing Framework](https://www.nuget.org/packages/Roslynator.Testing.CSharp.Xunit)
