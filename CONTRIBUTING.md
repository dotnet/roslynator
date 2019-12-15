# Contributing to Roslynator

Guidelines for contributing to the Roslynator repo.

## Submitting Pull Requests

* **DO** submit issues for bug fixes or features.
* **DO** add unit tests for bug fixes or features.
* **DO** ensure submissions pass build and are merge conflict free.
* **DO NOT** submit new analyzer/refactoring/fix without discussing it first.
* **DO NOT** submit large formatting/documentation changes without discussing it first.

## Creating Issues

* **DO** create a new issue rather than commenting a closed issue.
* **DO** include analyzer/refactoring/error ID in a title (i.e. RCSxxxx, RRxxxx or CSxxxx).
* **DO** use a descriptive title that identifies the issue or requested feature.
* **DO** specify a detailed description of the issue or requested feature.
* **DO** provide the following for bug reports:
  * Describe the expected behavior and the actual behavior.
  * Provide example code that reproduces the issue.
  * Provide any relevant exception messages and stack traces.

## Coding Style

* **DO** follow [CoreFX Coding Style](https://github.com/dotnet/corefx/blob/master/Documentation/coding-guidelines/coding-style.md) (except using `s_` and `t_` prefix for field names).
* **DO** install [Roslynator for Visual Studio](https://marketplace.visualstudio.com/items?itemName=josefpihrt.Roslynator2019) and follow suggestions.
