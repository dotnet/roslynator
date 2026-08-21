# Roslynator.CodeFixes

A collection of [code fixes](https://josefpihrt.github.io/docs/roslynator/fixes) for C# compiler diagnostics, powered by [Roslyn](https://github.com/dotnet/roslyn).

Use this package when Roslynator IDE extensions are unavailable, such as VS Code with C# Dev Kit.
Otherwise, use the IDE extension instead.

## Requirements

* Visual Studio 2022 or 2026
* VS Code with [C#](https://marketplace.visualstudio.com/items?itemName=ms-dotnettools.csharp) extension 1.21.13 or higher
* Roslyn 3.8.0 or higher (when used directly, not as part of an IDE)

## Usage

* Add the package to your project:
   ```shell
   dotnet add package roslynator.codefixes
   ```

* Use EditorConfig to [configure](https://josefpihrt.github.io/docs/roslynator/configuration) code fixes.

## Feedback

* File an issue on [GitHub](https://github.com/dotnet/roslynator/issues/new)
* Follow on [Twitter](https://twitter.com/roslynator)

## Related Products

* [Roslynator 2026 (VS 2022 17.14+ / VS 2026)](https://marketplace.visualstudio.com/items?itemName=josefpihrt.Roslynator2026)
* [Roslynator for Visual Studio 2022](https://marketplace.visualstudio.com/items?itemName=josefpihrt.Roslynator2022)
* [Roslynator for VS Code](https://marketplace.visualstudio.com/items?itemName=josefpihrt-vscode.roslynator)
* [Roslynator Command-line Tool](https://www.nuget.org/packages/Roslynator.DotNet.Cli)
* [Roslynator Testing Framework](https://www.nuget.org/packages/Roslynator.Testing.CSharp.Xunit)
