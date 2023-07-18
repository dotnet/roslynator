# How to Configure Ruleset File

## New csproj format (.NET Core, .NET Standard)

### 1) Add New Rule Set File to Your Solution

* Go to Solution Explorer - Solution - Context Menu - Add - New Item - Code Analysis Rule Set

### 2) Modify Rule Set File Manually

* Open ruleset file in text editor.
* Change value of attribute 'Name' (ruleset is represented in the IDE by its name).

![Edit Rule Set File](/img/roslynator/edit-new-ruleset-file.png)

### 3) Attach Rule Set to Project(s)

* Open csproj file in text editor or go to Solution Explorer - Project - Context Menu - Edit ProjectName.csproj
* Add following `PropertyGroup` (or add element to the existing `PropertyGroup`):

```xml
<PropertyGroup>
  <CodeAnalysisRuleSet>RELATIVE_OR_ABSOLUTE_PATH_TO_RULESET_FILE</CodeAnalysisRuleSet>
</PropertyGroup>
```

## Old csproj format (.NET Framework)

### 1) Create Your Own Rule Set File

* Skip this step if you already have one.
* Go to Solution Explorer - Solution - Project - References - Analyzers - Open Active Rule Set.

![Open Active Rule Set](/img/roslynator/open-active-ruleset.png)

* Modify ruleset.
* Save ruleset (this will create a new file **ProjectName.ruleset** in your project folder.
* Move ruleset file to a solution root folder (or any other location).

![Rule Set Editor](/img/roslynator/ruleset-editor.png)

### 2) Modify Rule Set File Manually

* Open ruleset file in text editor.
* Change value of attribute 'Name' (ruleset is represented in the IDE by its name).

![Edit Rule Set File](/img/roslynator/edit-ruleset-file.png)

### 3) Attach Rule Set to Project(s)

* Go to Main Menu - Analysis - Configure Code Analysis - For Solution

![Configure Code Analysis for Solution](/img/roslynator/configure-code-analysis-for-solution.png)

* Change ruleset for each project.
* Change configuration and repeat previous step (optional).

![Code Analysis Settings](/img/roslynator/code-analysis-settings.png)

## Directory.Build.props File

It may be convenient to to specify ruleset in **Directory.Build.props** file.
This file is typically located in a root folder of a solution.

```xml
<Project>
  <PropertyGroup>
    <CodeAnalysisRuleSet>$(MSBuildThisFileDirectory)my.ruleset</CodeAnalysisRuleSet>
  </PropertyGroup>
</Project>
```

## See Also

* [How to: Create a Custom Rule Set](https://msdn.microsoft.com/en-us/library/dd264974.aspx)
* [Working in the Code Analysis Rule Set Editor](https://msdn.microsoft.com/en-us/library/dd380626.aspx)
* [How to: Specify Managed Code Rule Sets for Multiple Projects in a Solution](https://msdn.microsoft.com/en-us/library/dd465181.aspx)
* [Rule Set XML Schema](https://github.com/dotnet/roslyn/blob/main/src/Compilers/Core/Portable/RuleSet/RuleSetSchema.xsd)
* [Customize your build](https://docs.microsoft.com/en-us/visualstudio/msbuild/customize-your-build)
* [Common MSBuild properties and items with Directory.Build.props](https://thomaslevesque.com/2017/09/18/common-msbuild-properties-and-items-with-directory-build-props/)
