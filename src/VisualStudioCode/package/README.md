# Roslynator for Visual Studio Code

A collection of [refactorings](https://josefpihrt.github.io/docs/roslynator/refactorings), and [fixes](https://josefpihrt.github.io/docs/roslynator/fixes) for C#, powered by [Roslyn](https://github.com/dotnet/roslyn).

Analyzers are not included in this extension. Add [Roslynator.Analyzers](https://www.nuget.org/packages/roslynator.analyzers) to your projects for diagnostics in the editor and `dotnet build`.

## Prerequisites

This extension requires **legacy OmniSharp** (not C# Dev Kit):

- Set VS Code setting `dotnet.server.useOmnisharp` to `true`
- Disable extension **C# Dev Kit** (if installed)
- Use a recent [C# extension](https://marketplace.visualstudio.com/items?itemName=ms-dotnettools.csharp) with OmniSharp **1.39.15+** (Roslyn 5.x)

NOTE: After each installation, Roslynator updates `omnisharp.json` with references to its DLLs.

[C# Dev Kit](https://marketplace.visualstudio.com/items?itemName=ms-dotnettools.csdevkit) does not currently support loading Roslyn features from an extension (see related [issue](https://github.com/dotnet/vscode-csharp/issues/6790)), so this extension does not work with C# Dev Kit.
As an alternative, use NuGet packages that provide [refactorings](https://www.nuget.org/packages/roslynator.refactorings) and [code fixes for compiler diagnostics](https://www.nuget.org/packages/roslynator.codefixes).

## Configuration

Use an EditorConfig file to configure analyzers, refactorings, and compiler diagnostic fixes.

```editorconfig
# Set severity for all analyzers that are enabled by default (https://docs.microsoft.com/en-us/visualstudio/code-quality/use-roslyn-analyzers?view=vs-2022#set-rule-severity-of-multiple-analyzer-rules-at-once-in-an-editorconfig-file)
dotnet_analyzer_diagnostic.category-roslynator.severity = default|none|silent|suggestion|warning|error

# Enable/disable all analyzers by default.
# NOTE: This option can be used only in .roslynatorconfig file
roslynator_analyzers.enabled_by_default = true|false

# Set severity for a specific analyzer
dotnet_diagnostic.<ANALYZER_ID>.severity = default|none|silent|suggestion|warning|error

# Enable/disable all refactorings
roslynator_refactorings.enabled = true|false

# Enable/disable specific refactoring
roslynator_refactoring.<REFACTORING_NAME>.enabled = true|false

# Enable/disable all compiler diagnostic fixes
roslynator_compiler_diagnostic_fixes.enabled = true|false

# Enable/disable specific compiler diagnostic fix
roslynator_compiler_diagnostic_fix.<COMPILER_DIAGNOSTIC_ID>.enabled = true|false
```

See the [full list of configuration options](https://josefpihrt.github.io/docs/roslynator/configuration).

## Default Configuration

To configure Roslynator for all projects on your machine, use a `.roslynatorconfig` file.

To open the config file:

1) Press Ctrl + Shift + P
2) Type "roslynator"
3) Select "Roslynator: Open Default Configuration File (.roslynatorconfig)"

## Location of Configuration File

The configuration file is located at `%LOCALAPPDATA%/JosefPihrt/Roslynator/.roslynatorconfig`.
The value of `%LOCALAPPDATA%` depends on the operating system:

| OS | Path |
| -------- | ------- |
| Windows | `C:/Users/<USERNAME>/AppData/Local/JosefPihrt/Roslynator/.roslynatorconfig` |
| Linux | `/home/<USERNAME>/.local/share/JosefPihrt/Roslynator/.roslynatorconfig` |
| OSX | `/Users/<USERNAME>/.local/share/JosefPihrt/Roslynator/.roslynatorconfig` |

Default configuration is loaded once when the IDE starts. Therefore, it may be necessary to restart the IDE for changes to take effect.

## Requirements

This extension requires [C# for Visual Studio Code](https://marketplace.visualstudio.com/items?itemName=ms-dotnettools.csharp) with OmniSharp **1.39.15+** (Roslyn 5.x). See [Prerequisites](#prerequisites).

## Donation

Although Roslynator is free of charge, [donations](https://www.paypal.com/cgi-bin/webscr?cmd=_s-xclick&hosted_button_id=BX85UA346VTN6) are welcome and support further development.

## Thanks

* Thanks to [Pekka Savolainen](https://github.com/savpek), who pioneered Roslyn analyzers on Visual Studio Code.
* Thanks to [Adrian Wilczynski](https://github.com/AdrianWilczynski) for several great [PRs](https://github.com/dotnet/roslynator/pulls?q=author%3AAdrianWilczynski).
