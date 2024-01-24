# Roslynator for Visual Studio Code

A collection of 500+ [analyzers](https://josefpihrt.github.io/docs/roslynator/analyzers), [refactorings](https://josefpihrt.github.io/docs/roslynator/refactorings) and [fixes](https://josefpihrt.github.io/docs/roslynator/fixes) for C#, powered by [Roslyn](https://github.com/dotnet/roslyn).

IMPORTANT: Analyzers will be removed from Roslynator for VS Code in the next major release.
It's recommended to use Roslynator NuGet packages (e.g. [Roslynator.Analyzers](https://www.nuget.org/packages/roslynator.analyzers)) instead.

## Prerequsities

Prerequisite for this extension is to use OmniSharp:

- Set VS Code setting `dotnet.server.useOmnisharp` to `true`
- Disable extension **C# Dev Kit** (if installed)

NOTE: After each installation, Roslynator updates `omnisharp.json` to include references to Roslynator DLLs.

[C# Dev Kit](https://marketplace.visualstudio.com/items?itemName=ms-dotnettools.csdevkit) currently does not support loading Roslyn features from an extension (see related [issue](https://github.com/dotnet/vscode-csharp/issues/6790)), which means that this extension won't work with C# Dev Kit.
As an alternative, it's possible to use NuGet packages that provide [refactorings](https://www.nuget.org/packages/roslynator.refactorings)
 and [code fixes for compiler diagnostics](https://www.nuget.org/packages/roslynator.codefixes).

## Configuration

Use EditorConfig file to configure analyzers, refactorings and compiler diagnostic fixes.

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

Full list of available options is [here](https://josefpihrt.github.io/docs/roslynator/configuration)

## Default Configuration

If you want to configure Roslynator on a user-wide basis you have to use Roslynator config file.

How to open config file:

1) Press Ctrl + Shift + P
2) Type "roslynator"
3) Select "Roslynator: Open Default Configuration File (.roslynatorconfig)"

## Location of Configuration File

Configuration file is located at `%LOCALAPPDATA%/JosefPihrt/Roslynator/.roslynatorconfig`.
Location of `%LOCALAPPDATA%` depends on the operating system:

| OS | Path |
| -------- | ------- |
| Windows | `C:/Users/<USERNAME>/AppData/Local/JosefPihrt/Roslynator/.roslynatorconfig` |
| Linux | `/home/<USERNAME>/.local/share/JosefPihrt/Roslynator/.roslynatorconfig` |
| OSX | `/Users/<USERNAME>/.local/share/JosefPihrt/Roslynator/.roslynatorconfig` |

Default configuration is loaded once when IDE starts. Therefore, it may be necessary to restart IDE for changes to take effect.

## Requirements

This extension requires [C# for Visual Studio Code](https://marketplace.visualstudio.com/items?itemName=ms-dotnettools.csharp) 1.21.13 or higher.

## Donation

Although Roslynator products are free of charge, any [donation](https://www.paypal.com/cgi-bin/webscr?cmd=_s-xclick&hosted_button_id=BX85UA346VTN6) is welcome and supports further development.

## Thanks

* Thanks to [Pekka Savolainen](https://github.com/savpek) who pioneered the way for Roslyn analyzers on Visual Studio Code.
* Thanks to [Adrian Wilczynski](https://github.com/AdrianWilczynski) who added several great [PRs](https://github.com/dotnet/roslynator/pulls?q=author%3AAdrianWilczynski).
