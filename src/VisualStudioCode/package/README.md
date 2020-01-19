# Roslynator for Visual Studio Code

A collection of 500+ [analyzers](https://github.com/JosefPihrt/Roslynator/blob/master/src/Analyzers/README.md), [refactorings](https://github.com/JosefPihrt/Roslynator/blob/master/src/Refactorings/README.md) and [fixes](https://github.com/JosefPihrt/Roslynator/blob/master/src/CodeFixes/README.md) for C#, powered by [Roslyn](https://github.com/dotnet/roslyn).

For further information please with Roslynator [repo](https://github.com/JosefPihrt/Roslynator).

## Location of Configuration Files

Configuration files are located at `%LOCALAPPDATA%\JosefPihrt\Roslynator\VisualStudioCode`.
Location of `%LOCALAPPDATA%` depends on the operating system:

| OS | Path |
| -------- | ------- |
| Windows | `C:\Users\JohnDoe\AppData\Local` |
| Linux | `/home/JohnDoe/.local/share` |
| OSX | `/Users/JohnDoe/.local/share` |

## Configuration of Analyzers

Standard rule sets are used to configure analyzers on a project-wide basis.

If you want to configure analyzers on a user-wide basis you have to create file at **%LOCALAPPDATA%\JosefPihrt\Roslynator\VisualStudioCode\roslynator.ruleset** with following content:

```xml
<?xml version="1.0" encoding="utf-8"?>
<RuleSet Name="roslynator.ruleset" ToolsVersion="16.0">
  <!-- Specify default action that should be applied to all analyzers except those explicitly specified. -->
  <!-- <IncludeAll Action="None,Hidden,Info,Warning,Error" /> -->
  <Rules AnalyzerId="Roslynator.CSharp.Analyzers" RuleNamespace="Roslynator.CSharp.Analyzers">
    <!-- Specify default action that should be applied to a specified analyzer. -->
    <!-- <Rule Id="RCSxxxx" Action="None,Hidden,Info,Warning,Error" /> -->
  </Rules>
</RuleSet>
```

This rule set can be used to:

1. Enable/disable analyzer(s) by DEFAULT.
2. Change DEFAULT severity (action) of the analyzer(s).

Although it is possible to edit ruleset manually, Visual Studio has built-in support for editing ruleset.
Just add ruleset file to a solution and open it.
 
## Configuration of Refactorings and Fixes

Create file at **%LOCALAPPDATA%\JosefPihrt\Roslynator\VisualStudioCode\roslynator.config** with following content:

```xml
<?xml version="1.0" encoding="utf-8"?>
<Roslynator>
  <Settings>
    <General>
      <!-- <PrefixFieldIdentifierWithUnderscore>true</PrefixFieldIdentifierWithUnderscore> -->
    </General>
    <Refactorings>
      <!-- <Refactoring Id="RRxxxx" IsEnabled="false" /> -->
    </Refactorings>
    <CodeFixes>
      <!-- <CodeFix Id="CSxxxx.RCFxxxx" IsEnabled="false" /> -->
      <!-- <CodeFix Id="CSxxxx" IsEnabled="false" /> -->
      <!-- <CodeFix Id="RCFxxxx" IsEnabled="false" /> -->
    </CodeFixes>
  </Settings>
</Roslynator>
```

Full list of refactorings identifiers can be found [here](https://github.com/JosefPihrt/Roslynator/blob/master/src/Refactorings/README.md). Full list of fixes identifiers can be found [here](https://github.com/JosefPihrt/Roslynator/blob/master/src/CodeFixes/README.md).

Configuration for analyzers/refactorings/fixes is applied once when libraries are loaded.
Therefore, it may be neccessary to restart IDE for changes to take effect.

## Requirements

This extension requires [C# for Visual Studio Code](https://marketplace.visualstudio.com/items?itemName=ms-vscode.csharp) 1.19.0 or higher.

## Donation

Although Roslynator products are free of charge, any [donation](https://www.paypal.com/cgi-bin/webscr?cmd=_s-xclick&hosted_button_id=BX85UA346VTN6) is welcome and supports further development.

## Thanks

Thanks to [savpek](https://github.com/savpek) who pioneered the way for Roslyn analyzers on Visual Studio Code.
