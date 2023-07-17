# How to Use Both Visual Studio Extension (VSIX) and NuGet Package

If you use both VSIX and NuGet package **Roslynator.Analyzers** it is necessary to ensure that analyzers won't run twice. The solution differs by Visual Studio version:

## Roslynator 2019

For Visual Studio 2019, add following content to file `%LOCALAPPDATA%\JosefPihrt\Roslynator\VisualStudio\2019\roslynator.ruleset`:

```xml
<?xml version="1.0" encoding="utf-8"?>
<RuleSet Name="roslynator.ruleset" ToolsVersion="16.0">
  <IncludeAll Action="None" />
</RuleSet>
```

This will effectively disable all analyzers in VSIX.

Usually VSIX contains more analyzers than NuGet. It's because new version of NuGet has not been published yet. These analyzers can be enabled one by one.

```xml
<?xml version="1.0" encoding="utf-8"?>
<RuleSet Name="roslynator.ruleset" ToolsVersion="16.0">
  <IncludeAll Action="None" />
  <Rule Id="RCS1234" Action="Info" />   <!-- This analyzer is contained only in VSIX -->
</RuleSet>
```

## Roslynator 2017

For Visual Studio 2017, use VSIX [Roslynator Refactorings 2017](https://marketplace.visualstudio.com/items?itemName=josefpihrt.RoslynatorRefactorings2017) which does not contain analyzers.
