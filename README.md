# Roslynator <img align="left" width="48px" height="48px" src="images/roslynator-logo-small.png" />

Roslynator is a set of code analysis tools for C#, powered by [Roslyn](https://github.com/dotnet/roslyn).

Analyzers are not included in Roslynator IDE extensions. Use Roslynator NuGet packages (e.g. [Roslynator.Analyzers](https://www.nuget.org/packages/roslynator.analyzers)) for diagnostics.

## Tools

- IDE extensions for:
  - [Visual Studio 2026](https://marketplace.visualstudio.com/items?itemName=josefpihrt.Roslynator2026) (refactorings and code fixes; analyzers via NuGet)
  - [VS Code](https://marketplace.visualstudio.com/items?itemName=josefpihrt-vscode.roslynator) (refactorings and compiler diagnostic fixes; analyzers via NuGet)
    - Requires legacy OmniSharp (`dotnet.server.useOmnisharp`: `true`). With C# Dev Kit, use NuGet packages instead.
  - [Open VSX](https://open-vsx.org/extension/josefpihrt-vscode/roslynator)
- [NuGet packages](#nuget-packages) that contain a collection of analyzers
  - [Roslynator.Analyzers](https://www.nuget.org/packages/Roslynator.Analyzers)
  - [Roslynator.CodeAnalysis.Analyzers](https://www.nuget.org/packages/Roslynator.CodeAnalysis.Analyzers)
  - [Roslynator.Formatting.Analyzers](https://www.nuget.org/packages/Roslynator.Formatting.Analyzers)
- [Testing framework](#testing-framework) that allows unit testing of analyzers, refactorings, and code fixes
- [.NET client libraries](#client-libraries) that extend the Roslyn API
- [Command line tool](#command-line-tool)

## Documentation

- [Configuration](https://josefpihrt.github.io/docs/roslynator/configuration)
- [Guides](https://josefpihrt.github.io/docs/roslynator/category/guides)
- [Roslynator CLI](https://josefpihrt.github.io/docs/roslynator/cli)
- [Roslynator Testing Framework](https://josefpihrt.github.io/docs/roslynator/testing)
- [Roslynator Client Libraries](https://josefpihrt.github.io/docs/roslynator/ref)

## Contributions

Contributions are welcome! See the [developer documentation](https://josefpihrt.github.io/docs/roslynator/developers), [contributing with agent skills](https://josefpihrt.github.io/docs/roslynator/contributing-with-agent-skills), and [open issues](https://github.com/dotnet/roslynator/issues?q=is%3Aissue+is%3Aopen+sort%3Aupdated-desc+label%3Aup-for-grabs).

**Agent skills** in [.claude/skills/](.claude/skills/) provide step-by-step workflows for Cursor and Claude Code (adding analyzers, refactorings, compiler fixes, and bug fixes). You can also read `SKILL.md` files directly as contributor guides. See [CONTRIBUTING.md](CONTRIBUTING.md).

TIP: Bugfixes or small improvements can be implemented right away. Larger tasks, such as adding a new analyzer or refactoring, should be discussed first.

## Donations

Special thanks to:
  - [.NET on AWS Open Source Software Fund](https://github.com/aws/dotnet-foss) for donating $6000.00 USD ($500.00 for 12 months starting November 2024).
  - Microsoft for donating $1000.00 USD.
  - @IanKemp for donating $13.00 USD / month starting July 2021.
  - @Genbox for donating $5.00 USD / month starting October 2024.
  - Timo Nürnberg for donating $5.00 USD / month starting March 2025.

## .NET Foundation

This project is supported by the [.NET Foundation](https://www.dotnetfoundation.org/projects).

This project has adopted the code of conduct defined by the Contributor Covenant to clarify expected behavior in our community.
For more information see the [.NET Foundation Code of Conduct](https://dotnetfoundation.org/code-of-conduct). 

## Command Line Tool

To install the CLI:
```sh
dotnet tool install -g roslynator.dotnet.cli
```

See the [CLI documentation](https://josefpihrt.github.io/docs/roslynator/cli) for more information.

The CLI is also integrated in [MegaLinter](https://megalinter.io/), an open-source linter aggregator for CI (see its [Roslynator page](https://megalinter.io/latest/descriptors/csharp_roslynator/)).

## Testing Framework

- Use the testing framework to unit-test analyzers, refactorings, and code fixes.
- See the [testing documentation](https://josefpihrt.github.io/docs/roslynator/testing) for more information.

## Client Libraries

- The client libraries extend Roslyn and are intended for building custom analyzers and refactorings.
- These packages do not include analyzers or refactorings.
- See the [API reference](https://josefpihrt.github.io/docs/roslynator/ref).

| Package | Version | Extends |
| --- | --- | --- |
| [Roslynator.Core](https://www.nuget.org/packages/Roslynator.Core) | [![NuGet](https://img.shields.io/nuget/v/Roslynator.Core.svg)](https://www.nuget.org/packages/Roslynator.Core) | [Microsoft.CodeAnalysis.Common](https://www.nuget.org/packages/Microsoft.CodeAnalysis.Common) |
| [Roslynator.Workspaces.Core](https://www.nuget.org/packages/Roslynator.Workspaces.Core) | [![NuGet](https://img.shields.io/nuget/v/Roslynator.Workspaces.Core.svg)](https://www.nuget.org/packages/Roslynator.Workspaces.Core) | [Microsoft.CodeAnalysis.Workspaces.Common](https://www.nuget.org/packages/Microsoft.CodeAnalysis.Workspaces.Common) |
| [Roslynator.CSharp](https://www.nuget.org/packages/Roslynator.CSharp) | [![NuGet](https://img.shields.io/nuget/v/Roslynator.CSharp.svg)](https://www.nuget.org/packages/Roslynator.CSharp) | [Microsoft.CodeAnalysis.CSharp](https://www.nuget.org/packages/Microsoft.CodeAnalysis.CSharp) |
| [Roslynator.CSharp.Workspaces](https://www.nuget.org/packages/Roslynator.CSharp.Workspaces) | [![NuGet](https://img.shields.io/nuget/v/Roslynator.CSharp.Workspaces.svg)](https://www.nuget.org/packages/Roslynator.CSharp.Workspaces) | [Microsoft.CodeAnalysis.CSharp.Workspaces](https://www.nuget.org/packages/Microsoft.CodeAnalysis.CSharp.Workspaces) |
