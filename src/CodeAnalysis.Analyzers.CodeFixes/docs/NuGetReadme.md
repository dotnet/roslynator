# Roslynator.CodeAnalysis.Analyzers

A collection of analyzers for Roslyn API, powered by [Roslyn](https://github.com/dotnet/roslyn).

The package is applicable for projects that reference Roslyn packages (Microsoft.CodeAnalysis*).

## Requirements

* Visual Studio 2022
* VS Code with [C# for Visual Studio Code](https://marketplace.visualstudio.com/items?itemName=ms-dotnettools.csharp) 1.21.13 or higher
* Roslyn 4.0.1 (when used directly, not as a part of IDE)

## Usage

* Add package to your project:
   ```shell
   dotnet add package roslynator.formatting.analyzers
   ```

* Use EditorConfig to [configure](https://github.com/josefpihrt/roslynator/blob/main/docs/Configuration.md) analyzers.

## Feedback

* File an issue on [GitHub](https://github.com/josefpihrt/roslynator/issues/new)
* Follow on [Twitter](https://twitter.com/roslynator)

## Related Products

* [Roslynator for Visual Studio 2022](https://marketplace.visualstudio.com/items?itemName=josefpihrt.Roslynator2022)
* [Roslynator for VS Code](https://marketplace.visualstudio.com/items?itemName=josefpihrt-vscode.roslynator)
* [Roslynator Command-line Tool](https://www.nuget.org/packages/Roslynator.DotNet.Cli)
* [Roslynator Testing Framework](https://www.nuget.org/packages/Roslynator.Testing.CSharp.Xunit)
