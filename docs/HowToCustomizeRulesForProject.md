# How to: Customize Rules for a Project

It is common that you have one rule set in you solution that is shared across all projects (**global.ruleset**).

It is also common that you want to enable/disable certain analyzer in one or several projects.

## Example: Disable an Analyzer in a Project

Let's say you would like to disable analyzer **RCS1090** in a project **AspNetCoreProject** which uses rule set **global.ruleset**.

1) Create a new rule set file with following content:

```xml
<?xml version="1.0" encoding="utf-8"?>
<RuleSet Name="Rules for ASP.NET Core" ToolsVersion="15.0">
  <Include Path="relative_or_absolute_path_to_global_ruleset" Action="Default" />
  <Rules AnalyzerId="Roslynator.CSharp.Analyzers" RuleNamespace="Roslynator.CSharp.Analyzers">
    <Rule Id="RCS1090" Action="None" />
  </Rules>
</RuleSet>
```

2) Update **AspNetCoreProject.csproj** so it uses newly created rule set instead of **global.ruleset**.

Analyzer **RCS1090** is now disabled in **AspNetCoreProject** and all other rules are inherited from **global.ruleset**.
