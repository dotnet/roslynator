# Roslynator.Refactorings

A collection of 200+ [refactorings](https://josefpihrt.github.io/docs/roslynator/refactorings) for C#, powered by [Roslyn](https://github.com/dotnet/roslyn).

Use this package when Roslynator IDE extensions are unavailable, such as VS Code with C# Dev Kit.
Otherwise, use the IDE extension instead.

## Requirements

* Visual Studio 2022
* VS Code with [C#](https://marketplace.visualstudio.com/items?itemName=ms-dotnettools.csharp) extension 1.21.13 or higher
* Roslyn 3.8.0 or higher (when used directly, not as part of an IDE)

## Usage

* Add the package to your project:
   ```shell
   dotnet add package roslynator.refactorings
   ```

* Use EditorConfig to [configure](https://josefpihrt.github.io/docs/roslynator/configuration) refactorings.

## Feedback

* File an issue on [GitHub](https://github.com/dotnet/roslynator/issues/new)
* Follow on [Twitter](https://twitter.com/roslynator)

## Related Products

* [Roslynator for Visual Studio 2022](https://marketplace.visualstudio.com/items?itemName=josefpihrt.Roslynator2022)
* [Roslynator for VS Code](https://marketplace.visualstudio.com/items?itemName=josefpihrt-vscode.roslynator)
* [Roslynator Command-line Tool](https://www.nuget.org/packages/Roslynator.DotNet.Cli)
* [Roslynator Testing Framework](https://www.nuget.org/packages/Roslynator.Testing.CSharp.Xunit)
