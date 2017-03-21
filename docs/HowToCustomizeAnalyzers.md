# How to Customize Analyzers

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

### 1) Create your own ruleset file

* Skip this step if you already have one.
* Go to Solution Explorer - Solution - Project - References - Analyzers - Open Active Rule Set.
* Modify rule set.
* Save rule set (this will create a new file **ProjectName.ruleset** in your project folder.
* Move ruleset file to a solution root folder (or any other location).

![Open Active Rule Set](/images/OpenActiveRuleSet.png)
![Rule Set Editor](/images/RuleSetEditor.png)

### 2) Modify ruleset file manually

* Open ruleset file in text editor.
* Change its name (rule set is represented in the IDE by its name).

![Edit RuleSet File](/images/EditRuleSetFile.png)

### 3) Attach rule set to project(s)

* Go to Main Menu - Analysis - Configure Code Analysis - For Solution
* Change rule set for each project.
* Change configuration and repeat previous step (optional).

![Configure Code Analysis for Solution](/images/ConfigureCodeAnalysisForSolution.png)

![Code Analysis Settings](/images/CodeAnalysisSettings.png)

## MSDN Links

* [How to: Create a Custom Rule Set](https://msdn.microsoft.com/en-us/library/dd264974.aspx)
* [Working in the Code Analysis Rule Set Editor](https://msdn.microsoft.com/en-us/library/dd380626.aspx)
* [How to: Specify Managed Code Rule Sets for Multiple Projects in a Solution](https://msdn.microsoft.com/en-us/library/dd465181.aspx)