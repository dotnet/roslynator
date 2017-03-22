## Roslynator
* A collection of 170+ analyzers and 170+ refactorings for C#, powered by Roslyn.
* [List of Analyzers](http://github.com/JosefPihrt/Roslynator/blob/master/source/Analyzers/README.md)
* [List of Refactorings](http://github.com/JosefPihrt/Roslynator/blob/master/source/Refactorings/README.md)
* [Release Notes](http://github.com/JosefPihrt/Roslynator/blob/master/ChangeLog.md)

### Products

#### Extensions for Visual Studio 2017

* [Roslynator 2017](http://marketplace.visualstudio.com/items?itemName=josefpihrt.Roslynator2017) (analyzers and refactorings).
* [Roslynator Refactorings 2017](http://marketplace.visualstudio.com/items?itemName=josefpihrt.RoslynatorRefactorings2017) (refactorings only).

#### Extensions for Visual Studio 2015

* [Roslynator](http://visualstudiogallery.msdn.microsoft.com/e83c5e41-92c5-42a3-80cc-e0720c621b5e) (analyzers and refactorings).
* [Roslynator Refactorings](http://visualstudiogallery.msdn.microsoft.com/a9a2b4bc-70da-437d-9ab7-b6b8e7d76cd9) (refactorings only).

#### NuGet Packages

* [Roslynator.Analyzers](http://www.nuget.org/packages/Roslynator.Analyzers/) (analyzers only).
  * This package is dependent on Microsoft.CodeAnalysis.CSharp.Workspaces.2.0.0 (Visual Studio 2017 or higher).
* [C# Analyzers](http://www.nuget.org/packages/CSharpAnalyzers/) (analyzers only).
  * This package is dependent on Microsoft.CodeAnalysis.CSharp.Workspaces.1.0.0 (Visual Studio 2015 or higher).

### Settings

* Analyzers can be enabled/disabled using **rule set**. Please see [How to Customize Analyzers](http://github.com/JosefPihrt/Roslynator/blob/master/docs/HowToCustomizeAnalyzers.md).
* Refactorings can be enabled/disabled in Visual Studio options.

![Refactorings Options](/images/RefactoringsOptions.png)
