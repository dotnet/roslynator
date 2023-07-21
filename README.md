# Roslynator <img align="left" width="48px" height="48px" src="http://pihrt.net/images/Roslynator.ico" />

A collection of 500+ [analyzers](https://josefpihrt.github.io/docs/roslynator/analyzers), [refactorings](https://josefpihrt.github.io/docs/roslynator/refactorings) and [fixes](https://josefpihrt.github.io/docs/roslynator/fixes) for C#, powered by [Roslyn](https://github.com/dotnet/roslyn).

## Features

* [IDE Extensions](#extensions)
* [NuGet Packages](#nuget-packages)
* [Roslynator Client Libraries](#roslynator-client-libraries)
* [Roslynator Command Line Tool](#roslynator-command-line-tool)
* [Release notes](https://github.com/JosefPihrt/Roslynator/blob/main/ChangeLog.md)
* Follow on [Twitter](https://twitter.com/roslynator)

## Documentation

* [Configuration](https://josefpihrt.github.io/docs/roslynator/configuration)
* [Guides](https://josefpihrt.github.io/docs/roslynator/guides)
* [Developers](https://josefpihrt.github.io/docs/roslynator/developers)
* [Roslynator CLI](https://josefpihrt.github.io/docs/roslynator/cli)
* [Roslynator Client Libraries](https://josefpihrt.github.io/docs/roslynator/ref)

## Contributions

Contributions are welcome!
Please read [CONTRIBUTING.md](CONTRIBUTING.MD) and [documentation for developers](https://josefpihrt.github.io/docs/roslynator/developers).
In a nutshell, bugfixes or small improvements can be implemented right away. Larger task like adding new analyzer/refactoring should be discussed first.

## Donation

* Although Roslynator products are free of charge, any [donation](https://www.paypal.com/cgi-bin/webscr?cmd=_s-xclick&hosted_button_id=BX85UA346VTN6) is welcome and supports further development.
* [List of donations](https://github.com/JosefPihrt/Roslynator/blob/main/Donations.md)

## Extensions

- [Visual Studio 2022](https://marketplace.visualstudio.com/items?itemName=josefpihrt.Roslynator2022)
- [VS Code](https://marketplace.visualstudio.com/items?itemName=josefpihrt-vscode.roslynator)
- [Open VSX](https://open-vsx.org/extension/josefpihrt-vscode/roslynator)

*Note: Extensions contain analyzers, refactorings and fixes for C# compiler diagnostics.*

## NuGet Packages

| Package | Version | Comment |
| --- | --- | --- |
| [Roslynator.Analyzers](https://www.nuget.org/packages/Roslynator.Analyzers) | [![NuGet](https://img.shields.io/nuget/v/Roslynator.Analyzers.svg)](https://www.nuget.org/packages/Roslynator.Analyzers) | common analyzers (RCS1xxx) ([list](http://pihrt.net/Roslynator/Analyzers?Query=RCS1)) |
| [Roslynator.CodeAnalysis.Analyzers](https://www.nuget.org/packages/Roslynator.CodeAnalysis.Analyzers) | [![NuGet](https://img.shields.io/nuget/v/Roslynator.CodeAnalysis.Analyzers.svg)](https://www.nuget.org/packages/Roslynator.CodeAnalysis.Analyzers) | analyzers for Roslyn API (RCS9xxx) ([list](http://pihrt.net/Roslynator/Analyzers?Query=RCS9)) |
| [Roslynator.Formatting.Analyzers](https://www.nuget.org/packages/Roslynator.Formatting.Analyzers) | [![NuGet](https://img.shields.io/nuget/v/Roslynator.Formatting.Analyzers.svg)](https://www.nuget.org/packages/Roslynator.Formatting.Analyzers) | formatting analyzers (RCS0xxx) ([list](http://pihrt.net/Roslynator/Analyzers?Query=RCS0)) |

*Note: All analyzers in package Roslynator.Formatting.Analyzers are disabled by default.*

## Roslynator Command Line Tool

Run following command to install Roslynator command line tool:
```
dotnet tool install -g roslynator.dotnet.cli
```

* [Documentation](https://josefpihrt.github.io/docs/cli)

## Roslynator Testing Framework

* Roslynator Testing Framework can be used for unit testing of analyzers, refactorings and code fixes.
* Framework is distributed as NuGet [package](https://www.nuget.org/packages/Roslynator.Testing.CSharp.Xunit). &ensp;[![NuGet](https://img.shields.io/nuget/v/Roslynator.Testing.CSharp.Xunit.svg)](https://www.nuget.org/packages/Roslynator.Testing.CSharp.Xunit)
* Learn how to use the framework from actual usages in Roslynator repo:
  * Tests of analyzers are [here](src/Tests/Analyzers.Tests), [here](src/Tests/CodeAnalysis.Analyzers.Tests) and [here](https://github.com/JosefPihrt/Roslynator/tree/main/src/Tests/Formatting.Analyzers.Tests)
  * Tests of refactorings are [here](https://github.com/JosefPihrt/Roslynator/tree/main/src/Tests/Refactorings.Tests)
  * Tests of fixes of compiler diagnostics are [here](https://github.com/JosefPihrt/Roslynator/tree/main/src/Tests/CodeFixes.Tests)

## Roslynator Client Libraries

* Roslynator client libraries are meant be used for development of your own analyzers/refactorings.
* It does not contain any analyzers/refactorings itself.
* See [reference](https://josefpihrt.github.io/docs/roslynator/ref).

| Package | Version | Built on top of |
| --- | --- | --- |
| [Roslynator.Core](https://www.nuget.org/packages/Roslynator.Core) | [![NuGet](https://img.shields.io/nuget/v/Roslynator.Core.svg)](https://www.nuget.org/packages/Roslynator.Core) | [Microsoft.CodeAnalysis.Common](https://www.nuget.org/packages/Microsoft.CodeAnalysis.Common) |
| [Roslynator.Workspaces.Core](https://www.nuget.org/packages/Roslynator.Workspaces.Core) | [![NuGet](https://img.shields.io/nuget/v/Roslynator.Workspaces.Core.svg)](https://www.nuget.org/packages/Roslynator.Workspaces.Core) | [Microsoft.CodeAnalysis.Workspaces.Common](https://www.nuget.org/packages/Microsoft.CodeAnalysis.Workspaces.Common) |
| [Roslynator.CSharp](https://www.nuget.org/packages/Roslynator.CSharp) | [![NuGet](https://img.shields.io/nuget/v/Roslynator.CSharp.svg)](https://www.nuget.org/packages/Roslynator.CSharp) | [Microsoft.CodeAnalysis.CSharp](https://www.nuget.org/packages/Microsoft.CodeAnalysis.CSharp) |
| [Roslynator.CSharp.Workspaces](https://www.nuget.org/packages/Roslynator.CSharp.Workspaces) | [![NuGet](https://img.shields.io/nuget/v/Roslynator.CSharp.Workspaces.svg)](https://www.nuget.org/packages/Roslynator.CSharp.Workspaces) | [Microsoft.CodeAnalysis.CSharp.Workspaces](https://www.nuget.org/packages/Microsoft.CodeAnalysis.CSharp.Workspaces) |

## Other Projects

* [Snippetica](https://github.com/JosefPihrt/Snippetica) - Collection of snippets for C++, C#, HTML, JSON, Markdown, VB, XAML and XML
* [DotMarkdown](https://github.com/JosefPihrt/DotMarkdown) - Markdown framework for .NET
* [Regexator](http://pihrt.net/Regexator) - Desktop application for development of .NET regular expressions
