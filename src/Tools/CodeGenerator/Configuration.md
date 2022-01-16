# Roslynator Configuration

Use EditorConfig file to configure analyzers, refactoring and compiler diagnostic fixes.

```editorconfig
# Set severity for all analyzers
dotnet_analyzer_diagnostic.category-roslynator.severity = default|none|silent|suggestion|warning|error

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

## Default Configuration

If you want to configure Roslynator on a user-wide basis you have to use Roslynator config file.

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

