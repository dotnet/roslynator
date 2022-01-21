# Roslynator for Visual Studio Code

A collection of 500+ [analyzers](https://github.com/JosefPihrt/Roslynator/blob/master/src/Analyzers/README.md), [refactorings](https://github.com/JosefPihrt/Roslynator/blob/master/src/Refactorings/README.md) and [fixes](https://github.com/JosefPihrt/Roslynator/blob/master/src/CodeFixes/README.md) for C#, powered by [Roslyn](https://github.com/dotnet/roslyn).

For further information please with Roslynator [repo](https://github.com/JosefPihrt/Roslynator).

## Configuration

Use EditorConfig file to configure analyzers, refactoring and compiler diagnostic fixes.

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

Full list of available options is [here](https://github.com/josefpihrt/roslynator/docs/configuration.md)

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
| Windows | `C:/Users/<USERNAME>/AppData/Local/.roslynatorconfig` |
| Linux | `/home/<USERNAME>/.local/share/.roslynatorconfig` |
| OSX | `/Users/<USERNAME>/.local/share/.roslynatorconfig` |

Default configuration is loaded once when IDE starts. Therefore, it may be necessary to restart IDE for changes to take effect.

## Requirements

This extension requires [C# for Visual Studio Code](https://marketplace.visualstudio.com/items?itemName=ms-dotnettools.csharp) 1.21.13 or higher.

## Donation

Although Roslynator products are free of charge, any [donation](https://www.paypal.com/cgi-bin/webscr?cmd=_s-xclick&hosted_button_id=BX85UA346VTN6) is welcome and supports further development.

## Thanks

* Thanks to [Pekka Savolainen](https://github.com/savpek) who pioneered the way for Roslyn analyzers on Visual Studio Code.
* Thanks to [Adrian Wilczynski](https://github.com/AdrianWilczynski) who added several great [PRs](https://github.com/JosefPihrt/Roslynator/pulls?q=author%3AAdrianWilczynski).
