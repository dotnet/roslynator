# Analyzers vs. Refactorings

There are two basic concepts in code analysis:

* **Analyzer** (+ **Code Fix**)
* **Refactoring**

Unfortunately, some developers do not distiguish between these two concepts which causes a confusion.

### Basic Terms

Term | Description
--- | ---
Analyzer | Represents a general rule (code style) that should be followed.
Diagnostic | Represents a specific issue reported by the analyzer.
Code Fix | Represents an operation that will fix reported issue.
Refactoring | Represents a single operation that is provided on demand for a given span of text.

## Analyzers

> Analyzer represents a general rule (code style) that should be followed.

Analyzer runs in background and analyzes the source code. When it finds a code that is not in compliance with a rule it will report a **diagnostic**. The diagnostic is then displayed in the IDE (Error List, squiggles) and can be fixed if there is a **code fix**. Code fix may provide 'Fix all occurrences in ...' option which enables to apply multiple fixes at once.

#### Analyzers in the IDE

All diagnostics are displayed in the Error List. Each diagnostic is visible by squiggles in the code editor. Diagnostics that are marked as hidden are not visible.

Hotkey Ctrl+. will display available code fixes. Code fixes have precedence over refactorings so they are displayed first. There is an identifier and a description in the fly-out menu. Also there is 'Suppress ...' item at the bottom of the context menu.

![Code Fix in Context Menu](/images/CodeFixInContextMenu.png)

#### Configuration

Analyzers can be configured by using **rule set** file (see [How to Configure Analyzers](http://github.com/JosefPihrt/Roslynator/blob/master/docs/HowToConfigureAnalyzers.md)).

## Refactorings

> Refactoring represents a single operation that is provided on demand for a given span of text.

When it is requested IDE will suggest a list of refactorings that are applicable for a given span of text. Refactorings usually have no identifier. However, Roslynator refactorings have identifier similar to **RR1234**. This identifier is not displayed in the IDE, but it is displayed in Visual Studio options.

#### Refactorings in the IDE

Hotkey Ctrl+. will display available refactorings. Refactorings have lower precedence so they are displayed below diagnostics. There is no identifier and no description in the fly-out menu.

![Refactoring in Context Menu](/images/RefactoringInContextMenu.png)

#### Configuration

Roslynator refactorings can be configured in Visual Studio options.
