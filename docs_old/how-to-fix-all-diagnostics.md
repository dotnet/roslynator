# How to Fix All Diagnostics in a Solution

1) Install [Roslynator.DotNet.Cli](https://www.nuget.org/packages/Roslynator.DotNet.Cli/)&ensp;[![NuGet](https://img.shields.io/nuget/v/Roslynator.DotNet.Cli.svg)](https://nuget.org/packages/Roslynator.DotNet.Cli): 
  - Run `dotnet tool install -g Roslynator.DotNet.Cli`

1) Execute following command:

```shell
roslynator fix SOLUTION
```

## How to Reference Analyzer Assemblies

Roslynator will use analyzers and code fixes that are referenced as NuGet packages.
It will not use analyzers and code fixes that are part of Visual Studio extensions.
If you want to use these assemblies you have to use `--analyzer-assemblies` option. For example:

```shell
roslynator fix SOLUTION --analyzer-assemblies ANALYZER_ASSEMBLY PATH_TO_DIRECTORY_WITH_ANALYZER_ASSEMBLIES
```

Analyzer assembly locations are specific to the version of Visual Studio being used. An example location for Visual Studio 2019 Community:

```shell
C:/Program Files/Microsoft Visual Studio/2019/Community/Common7/IDE/CommonExtensions/Microsoft/ManagedLanguages/VBCSharp
```

## How to Use Custom Rule Set

```shell
roslynator fix SOLUTION -p CodeAnalysisRuleSet=FULL_PATH_TO_RULE_SET
```

It is necessary to specify full path to rule set as each project has different working directory.

## See Also

* [`fix` command](cli/commands/fix/index.md)
