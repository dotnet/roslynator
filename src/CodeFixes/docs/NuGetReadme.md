# Roslynator.CodeFixes

A collection [code fixes](https://josefpihrt.github.io/docs/roslynator/fixes) for C# compiler diagnostics, powered by [Roslyn](https://github.com/dotnet/roslyn).

This package is recommended to be used in an enviroment where Roslynator IDE extensions cannot be used, e.g. VS Code + C# Dev Kit.
Otherwise, do not use this package and use IDE extension which has the same functionality.

## Requirements

* Visual Studio 2022
* VS Code with [C#](https://marketplace.visualstudio.com/items?itemName=ms-dotnettools.csharp) extension 1.21.13 or higher
* Roslyn 3.8.0 or higher (when used directly, not as a part of IDE)

## Usage

* Add package to your project:
   ```shell
   dotnet add package roslynator.codefixes
   ```

* Use EditorConfig to [configure](https://josefpihrt.github.io/docs/roslynator/configuration) analyzers.

## Feedback

* File an issue on [GitHub](https://github.com/dotnet/roslynator/issues/new)
* Follow on [Twitter](https://twitter.com/roslynator)

## Related Products

* [Roslynator for Visual Studio 2022](https://marketplace.visualstudio.com/items?itemName=josefpihrt.Roslynator2022)
* [Roslynator for VS Code](https://marketplace.visualstudio.com/items?itemName=josefpihrt-vscode.roslynator)
* [Roslynator Command-line Tool](https://www.nuget.org/packages/Roslynator.DotNet.Cli)
* [Roslynator Testing Framework](https://www.nuget.org/packages/Roslynator.Testing.CSharp.Xunit)
