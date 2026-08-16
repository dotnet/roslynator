# Roslynator.Testing.CSharp.Xunit

Testing framework for unit-testing [Roslyn](https://github.com/dotnet/roslyn) analyzers, refactorings, and code fixes.

## Choosing the Roslyn version

This package depends on `Microsoft.CodeAnalysis.*` with a low minimum version, so it does not force a particular Roslyn version on your test project. Add a reference to the Roslyn version you want your tests to run against - typically the same version your analyzer library is built for:

```xml
<PackageReference Include="Microsoft.CodeAnalysis.CSharp.Workspaces" Version="4.14.0" />
```

The chosen version also determines the maximum C# language version available in your test source code (for example, Roslyn 3.8 supports up to C# 9).

## Usage

Examples in the Roslynator repository:

* Analyzer tests: [Analyzers.Tests](https://github.com/dotnet/roslynator/tree/main/src/Tests/Analyzers.Tests), [CodeAnalysis.Analyzers.Tests](https://github.com/dotnet/roslynator/tree/main/src/Tests/CodeAnalysis.Analyzers.Tests), and [Formatting.Analyzers.Tests](https://github.com/dotnet/roslynator/tree/main/src/Tests/Formatting.Analyzers.Tests)
* Refactoring tests: [Refactorings.Tests](https://github.com/dotnet/roslynator/tree/main/src/Tests/Refactorings.Tests)
* Compiler diagnostic fix tests: [CodeFixes.Tests](https://github.com/dotnet/roslynator/tree/main/src/Tests/CodeFixes.Tests)

## Feedback

* File an issue on [GitHub](https://github.com/dotnet/roslynator/issues/new)
* Follow on [Twitter](https://twitter.com/roslynator)

## Related Products

* [Roslynator for Visual Studio 2026](https://marketplace.visualstudio.com/items?itemName=josefpihrt.Roslynator2026)
* [Roslynator for Visual Studio 2022](https://marketplace.visualstudio.com/items?itemName=josefpihrt.Roslynator2022)
* [Roslynator for VS Code](https://marketplace.visualstudio.com/items?itemName=josefpihrt-vscode.roslynator)
* [Roslynator Command-line Tool](https://www.nuget.org/packages/Roslynator.DotNet.Cli)
