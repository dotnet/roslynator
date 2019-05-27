# Roslynator <img align="left" width="48px" height="48px" src="http://pihrt.net/images/Roslynator.ico">

A collection of 500+ [analyzers](src/Analyzers/README.md), [refactorings](src/Refactorings/README.md) and [fixes](src/CodeFixes/README.md) for C#, powered by [Roslyn](http://github.com/dotnet/roslyn).

### Features

* [Extensions for Visual Studio](#extensions-for-visual-studio)
* [NuGet Analyzers](#nuget-analyzers)
* [Roslynator API](#roslynator-api)
* [Roslynator Command-Line Interface](#roslynator-command-line-interface)
* [Roslynator for VS Code](#roslynator-for-vs-code)
* [Release notes](ChangeLog.md)
* Follow on [Twitter](https://twitter.com/roslynator)

### New Features

* [Fix all diagnostics in a solution](docs/HowToFixAllDiagnostics.md)
* [Generate API documentation](docs/HowToGenerateDocumentation.md)

### Donation

> "It's so good, that I made my first ever donation." Mateusz Piasecki, Roslynator user.

Although Roslynator products are free of charge, any [donation](https://www.paypal.com/cgi-bin/webscr?cmd=_s-xclick&hosted_button_id=BX85UA346VTN6) is welcome and supports further development.

## Extensions for Visual Studio

| Extension | Comment |
| --- | --- |
| [Roslynator 2019](https://marketplace.visualstudio.com/items?itemName=josefpihrt.Roslynator2019) | contains analyzers, refactorings and fixes for CS diagnostics. |
| [Roslynator 2017](https://marketplace.visualstudio.com/items?itemName=josefpihrt.Roslynator2017) | contains analyzers, refactorings and fixes for CS diagnostics. |

## NuGet Analyzers

| Package | Version | Comment |
| --- | --- | --- |
| [Roslynator.Analyzers](https://www.nuget.org/packages/Roslynator.Analyzers) | [![NuGet](https://img.shields.io/nuget/v/Roslynator.Analyzers.svg)](https://www.nuget.org/packages/Roslynator.Analyzers) | contains only analyzers |

## Roslynator API

* Roslynator API is meant be used for development of your own analyzers/refactorings.
* It does not contain any analyzers/refactorings itself.
* See [API Reference](docs/api/README.md).

| Package | Version | Built on top of |
| --- | --- | --- |
| [Roslynator.Core](https://www.nuget.org/packages/Roslynator.Core) | [![NuGet](https://img.shields.io/nuget/v/Roslynator.Core.svg)](https://www.nuget.org/packages/Roslynator.Core) | [Microsoft.CodeAnalysis.Common](https://www.nuget.org/packages/Microsoft.CodeAnalysis.Common) |
| [Roslynator.Workspaces.Core](https://www.nuget.org/packages/Roslynator.Workspaces.Core) | [![NuGet](https://img.shields.io/nuget/v/Roslynator.Workspaces.Core.svg)](https://www.nuget.org/packages/Roslynator.Workspaces.Core) | [Microsoft.CodeAnalysis.Workspaces.Common](https://www.nuget.org/packages/Microsoft.CodeAnalysis.Workspaces.Common) |
| [Roslynator.CSharp](https://www.nuget.org/packages/Roslynator.CSharp) | [![NuGet](https://img.shields.io/nuget/v/Roslynator.CSharp.svg)](https://www.nuget.org/packages/Roslynator.CSharp) | [Microsoft.CodeAnalysis.CSharp](https://www.nuget.org/packages/Microsoft.CodeAnalysis.CSharp) |
| [Roslynator.CSharp.Workspaces](https://www.nuget.org/packages/Roslynator.CSharp.Workspaces) | [![NuGet](https://img.shields.io/nuget/v/Roslynator.CSharp.Workspaces.svg)](https://www.nuget.org/packages/Roslynator.CSharp.Workspaces) | [Microsoft.CodeAnalysis.CSharp.Workspaces](https://www.nuget.org/packages/Microsoft.CodeAnalysis.CSharp.Workspaces) |

## Roslynator Command-Line Interface

* Roslynator CLI is distributed via NuGet package [Roslynator.CommandLine](https://www.nuget.org/packages/Roslynator.CommandLine). &ensp;[![NuGet](https://img.shields.io/nuget/v/Roslynator.CommandLine.svg)](https://www.nuget.org/packages/Roslynator.CommandLine)
* See [documentation](docs/cli/README.md).

## Roslynator for VS Code

Currently VS Code does not support distribution of Roslyn-based tools in an extension.
Also it does not support analyzers at all.
Please read the [tutorial](docs/RoslynatorForVisualStudioCode.md) how to install refactorings and code fixes for CS diagnostics.

## Documentation

* [Analyzers vs. Refactorings](docs/AnalyzersVsRefactorings.md)
* [How to Configure Analyzers](docs/HowToConfigureAnalyzers.md)
* [How to Configure Refactorings](docs/HowToConfigureRefactorings.md)
* [How to Customize Rules for a Project](docs/HowToCustomizeRulesForProject.md)
* [How to Fix All Diagnostics in a Solution](docs/HowToFixAllDiagnostics.md)
* [How to Generate API Documentation](docs/HowToGenerateDocumentation.md)

Would you like to improve Roslynator documentation? Please see [how to update documentation](docs/HowToUpdateDocumentation.md).

## Other Projects

* [Snippetica](https://github.com/JosefPihrt/Snippetica) - A collection of snippets for C++, C#, HTML, JSON, Markdown, VB, XAML and XML
* [DotMarkdown](https://github.com/JosefPihrt/DotMarkdown) - Markdown framework for .NET
* [LINQ to Regex](https://github.com/JosefPihrt/LinqToRegex) - A library that provides language integrated access to .NET regular expressions
* [Snippet Manager](https://github.com/JosefPihrt/SnippetManager) - A library that enables to work with Visual Studio snippets
* [Regexator](http://pihrt.net/Regexator) - A comprehensive development environment for .NET regular expressions
