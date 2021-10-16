
# Roslynator Command Line Tool

## Commands

* [analyze](analyze-command.md)
* [fix](fix-command.md)
* [format](format-command.md)
* [generate-doc](generate-doc-command.md)
* [generate-doc-root](generate-doc-root-command.md)
* [list-symbols](list-symbols-command.md)
* [loc](loc-command.md)
* [lloc](lloc-command.md)
* [rename-symbol](rename-symbol-command.md)
* [spellcheck](spellcheck-command.md)

## Packages

### Roslynator.DotNet.Cli &ensp;[![NuGet](https://img.shields.io/nuget/v/Roslynator.DotNet.Cli.svg)](https://nuget.org/packages/Roslynator.DotNet.Cli)

* [.NET Core global tool](https://docs.microsoft.com/dotnet/core/tools/global-tools)
  * cross-platform
  * can be run directly from command line
* It is recommended to use this tool.
* Version 0.2.0 or higher requires .NET Core SDK 5.0

Run following command to install it:

```
dotnet tool install -g roslynator.dotnet.cli
```

### Roslynator.CommandLine &ensp;[![NuGet](https://img.shields.io/nuget/v/Roslynator.CommandLine.svg)](https://nuget.org/packages/Roslynator.CommandLine)

* Stand-alone application that requires .NET Framework (Windows only).
* It can be used for .NET Framework projects (old style `csproj`).
* [Download package](https://www.nuget.org/api/v2/package/Roslynator.CommandLine/0.2.0) and run `Roslynator.exe`.

## Exit Code

Value | Comment
--- | ---
0 | Success\*
1 | Not a success\*\*
2 | Error occurred or execution canceled

\* A diagnostic was found (`analyze` command ) or a diagnostic was fixed (`fix` command) etc.

\*\* No diagnostic was found (`analyze` command) or no diagnostic was fixed (`fix` command) etc.

## Which MSBuild Instance to Use

If you are using version 0.1.5 or lower it may be necessary to specify MSBuild instance  - a directory where MSBuild binaries are located.

### Roslynator.CommandLine

You should specify MSBuild instance that is part of Visual Studio installation. It should be similar to `C:/Program Files/Microsoft Visual Studio/2019/Community/MSBuild/Current/Bin`

### Roslynator.DotNet.Cli

You should specify MSBuild instance that is part of .NET Core SDK installation. It should be similar to `C:/Program Files/dotnet/sdk/3.1.200`

## See Also

* [Change Log](../../src/CommandLine/ChangeLog.md)
* [How to Fix All Diagnostics in a Solution](../HowToFixAllDiagnostics.md)
* [How to Generate API Documentation](../HowToGenerateDocumentation.md)
