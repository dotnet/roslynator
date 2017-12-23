# How to: Configure Analyzers

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

## Step by Step Tutorial

### 1) Create Your Own Rule Set File

* Skip this step if you already have one.
* Go to Solution Explorer - Solution - Project - References - Analyzers - Open Active Rule Set.

![Open Active Rule Set](/images/OpenActiveRuleSet.png)

* Modify rule set.
* Save rule set (this will create a new file **ProjectName.ruleset** in your project folder.
* Move rule set file to a solution root folder (or any other location).

![Rule Set Editor](/images/RuleSetEditor.png)

### 2) Modify Rule Set File Manually

* Open rule set file in text editor.
* Change its name (rule set is represented in the IDE by its name).

![Edit Rule Set File](/images/EditRuleSetFile.png)

### 3) Attach Rule Set to Project(s)

* Go to Main Menu - Analysis - Configure Code Analysis - For Solution

![Configure Code Analysis for Solution](/images/ConfigureCodeAnalysisForSolution.png)

* Change rule set for each project.
* Change configuration and repeat previous step (optional).

![Code Analysis Settings](/images/CodeAnalysisSettings.png)

## How to Enable Rule Set in .NET Core project

To enable ruleset file in .NET Core project it is necessary to manually edit csproj file by adding relative or absolute path to rule set file:

```xml
<PropertyGroup>
	...
  <CodeAnalysisRuleSet>my.ruleset</CodeAnalysisRuleSet>
	...
</PropertyGroup>
```

## MSDN Links

* [How to: Create a Custom Rule Set](https://msdn.microsoft.com/en-us/library/dd264974.aspx)
* [Working in the Code Analysis Rule Set Editor](https://msdn.microsoft.com/en-us/library/dd380626.aspx)
* [How to: Specify Managed Code Rule Sets for Multiple Projects in a Solution](https://msdn.microsoft.com/en-us/library/dd465181.aspx)