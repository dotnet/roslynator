# How to: Configure Analyzers

## Content

* [Introduction](#introduction) 
* [What Is Rule Set](#what-is-rule-set)
* [How to Configure a Rule Set](#how-to-configure-a-rule-set)
* [How to Suppress a Diagnostic](#how-to-suppress-a-diagnostic)

## Introduction

* It is a common requirement to enable/disable specific analyzer or/and to change its action (from **Warning** to **Info** etc.)
* This can be easily accomplished by using **rule set**.

## What is Rule Set

* Rule set is a group of rules where each rule define "Action" for a specific analyzer.
* Action **None** deactivates analyzer.
* Other actions specifies that analyzer is active but it differs in how it is displayed in the IDE.
  
Action | Description
--- | ---
**None** | disabled
**Hidden** | not visible
**Info** | visible as **Message**
**Warning** | visible as **Warning**
**Error** | visible as **Error**

Rule set is typically stored in a file with extension **ruleset** and it has following structure:

```xml
<?xml version="1.0" encoding="utf-8"?>
<RuleSet Name="My Rules" ToolsVersion="15.0">
  <Rules AnalyzerId="Roslynator.CSharp.Analyzers" RuleNamespace="Roslynator.CSharp.Analyzers">
    <Rule Id="RCS1001" Action="Warning" />
  </Rules>
</RuleSet>
```

## How to Configure a Rule Set

### Old csproj format (.NET Framework)

#### 1) Create Your Own Rule Set File

* Skip this step if you already have one.
* Go to Solution Explorer - Solution - Project - References - Analyzers - Open Active Rule Set.

![Open Active Rule Set](/images/OpenActiveRuleSet.png)

* Modify rule set.
* Save rule set (this will create a new file **ProjectName.ruleset** in your project folder.
* Move rule set file to a solution root folder (or any other location).

![Rule Set Editor](/images/RuleSetEditor.png)

#### 2) Modify Rule Set File Manually

* Open rule set file in text editor.
* Change value of attribute 'Name' (rule set is represented in the IDE by its name).

![Edit Rule Set File](/images/EditRuleSetFile.png)

#### 3) Attach Rule Set to Project(s)

* Go to Main Menu - Analysis - Configure Code Analysis - For Solution

![Configure Code Analysis for Solution](/images/ConfigureCodeAnalysisForSolution.png)

* Change rule set for each project.
* Change configuration and repeat previous step (optional).

![Code Analysis Settings](/images/CodeAnalysisSettings.png)


### New csproj format (.NET Core, .NET Standard)

#### 1) Add New Rule Set File to Your Solution

* Go to Solution Explorer - Solution - Context Menu - Add - New Item - Code Analysis Rule Set

#### 2) Modify Rule Set File Manually

* Open rule set file in text editor.
* Change value of attribute 'Name' (rule set is represented in the IDE by its name).

![Edit Rule Set File](/images/EditNewRuleSetFile.png)

#### 3) Attach Rule Set to Project(s)

* Open csproj file in text editor or go to Solution Explorer - Project - Context Menu - Edit ProjectName.csproj
* Add following `PropertyGroup` (or add element to the existing `PropertyGroup`):

```xml
<PropertyGroup>
  <CodeAnalysisRuleSet>relative_or_absolute_path_to_ruleset_file</CodeAnalysisRuleSet>
</PropertyGroup>
```

## How to Suppress a Diagnostic

If you want to disable an analyzer completely you have use a rule set.
But if you want to suppress a diagnostic you have to use either `SuppressMessageAttribute` or `#pragma warning` preprocessor directive.

### Add `SuppressMessageAttribute` to Containing Declaration

```csharp
[SuppressMessage("Readability", "RCS1008", Justification = "<Pending>")]
void M()
{
    var x = Foo(); // no RCS1008 here
}
```

### Add `SuppressMessageAttribute` to Assembly

```csharp

[assembly: SuppressMessage("Readability", "RCS1008", Justification = "<Pending>", Scope = "member", Target = "~M:C.M")]

class C
{
    void M()
    {
        var x = Foo(); // no RCS1008 here
    }
}
```

### Add `#pragma warning` Preprocessor Directive

```csharp
#pragma warning disable RCS1008
var x = Foo(); // no RCS1008 here
#pragma warning restore RCS1008
```

## See Also

* [How to: Create a Custom Rule Set](https://msdn.microsoft.com/en-us/library/dd264974.aspx)
* [Working in the Code Analysis Rule Set Editor](https://msdn.microsoft.com/en-us/library/dd380626.aspx)
* [How to: Specify Managed Code Rule Sets for Multiple Projects in a Solution](https://msdn.microsoft.com/en-us/library/dd465181.aspx)
* [Rule Set XML Schema](https://github.com/dotnet/roslyn/blob/master/src/Compilers/Core/Portable/RuleSet/RuleSetSchema.xsd)
