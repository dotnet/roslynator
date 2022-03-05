# Roslynator Configuration

Use EditorConfig file to configure analyzers, refactoring and compiler diagnostic fixes.

```editorconfig
# Set severity for all analyzers that are enabled by default (https://docs.microsoft.com/en-us/visualstudio/code-quality/use-roslyn-analyzers?view=vs-2022#set-rule-severity-of-multiple-analyzer-rules-at-once-in-an-editorconfig-file)
dotnet_analyzer_diagnostic.category-roslynator.severity = default|none|silent|suggestion|warning|error

# Enable/disable all analyzers by default
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

## Required Options

Some analyzers require option to be set. For this purpose there is special analyzer ROS0003 that reports a diagnostic
if an analyzer is enabled but required option is not set. ROS0003 is disabled by default.

## Default Configuration

If you want to configure Roslynator on a user-wide basis you have to use Roslynator config file.
Default configuration file can be used with extension for Visual Studio or VS code.

### Format of Default Configuration File

Format of the file is same as format of [global AnalyzerConfig](https://docs.microsoft.com/en-us/dotnet/fundamentals/code-analysis/configuration-files#global-analyzerconfig).
It essentially means that file must contain top-level entry `is_global = true` and cannot contain section headers (such as `[*.cs]`).

### Location of Default Configuration File

Configuration file is located at `%LOCALAPPDATA%/JosefPihrt/Roslynator/.roslynatorconfig`.
Location of `%LOCALAPPDATA%` depends on the operating system:

| OS | Path |
| -------- | ------- |
| Windows | `C:/Users/<USERNAME>/AppData/Local/.roslynatorconfig` |
| Linux | `/home/<USERNAME>/.local/share/.roslynatorconfig` |
| OSX | `/Users/<USERNAME>/.local/share/.roslynatorconfig` |

Default configuration is loaded once when IDE starts. Therefore, it may be necessary to restart IDE for changes to take effect.

