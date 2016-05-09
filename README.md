## Pihrtsoft.CodeAnalysis
* Roslyn-based library that offers more than 60 analyzers and more than 70 refactorings for C#.
* [Release Notes](https://github.com/JosefPihrt/Pihrtsoft.CodeAnalysis/blob/master/ChangeLog.md)
* http://pihrt.net/roslyn

### Distribution

##### C# Analyzers and Refactorings
* [Visual Studio extension](https://visualstudiogallery.msdn.microsoft.com/e83c5e41-92c5-42a3-80cc-e0720c621b5e) that contains both analyzers and refactorings.

##### C# Refactorings
* [Visual Studio extension](https://visualstudiogallery.msdn.microsoft.com/a9a2b4bc-70da-437d-9ab7-b6b8e7d76cd9) that contains only refactorings.

##### C# Analyzers
* [NuGet package](https://www.nuget.org/packages/CSharpAnalyzers/) that contains only analyzers.

### List of Analyzers

* Add braces to a statement
* Add access modifier
* Add 'Async' suffix to asynchronous method name
* Add braces to if-else chain
* Add constructor's argument list
* Add empty line after embedded statement
* Add empty line between declarations
* Add parentheses to conditional expression's condition
* Avoid embedded statement
* Avoid implicit array creation
* Avoid locking on publicly accessible instance
* Avoid multiline expression body
* Avoid semicolon at the end of declaration
* Avoid using alias directive
* Convert foreach to for
* Declare each attribute separately
* Declare each type in separate file
* Declare explicit type (even if the type is obvious)
* Declare explicit type (when the type is not obvious)
* Declare explicit type in foreach (when the type is not obvious)
* Declare implicit type (when the type is obvious)
* Format accessor list
* Format binary operator on next line
* Format block
* Format each enum member on separate line
* Format each statement on separate line
* Format embedded statement on separate line
* Format switch section's statement on separate line
* Merge if statement with contained if statement
* Merge local declaration with return statement
* Remove 'Async' suffix from non-asynchronous method name
* Remove braces from a statement
* Remove braces from if-else chain
* Remove empty attribute's argument list
* Remove empty else clause
* Remove empty object initializer
* Remove empty statement
* Remove enum's default underlying type
* Remove original exception from throw statement
* Remove 'partial' modifier from type with a single part
* Remove redundant boolean literal
* Remove redundant braces
* Remove redundant comma in initializer
* Remove redundant empty line
* Remove redundant parentheses
* Remove redundant 'sealed' modifier
* Remove trailing white-space
* Rename private field according to camel case with underscore
* Reorder modifiers
* Simplify assignment expression
* Simplify boolean comparison
* Simplify else clause containing only if statement
* Simplify lambda expression
* Simplify lambda expression's parameter list
* Simplify nested using statement
* Simplify Nullable<T> to T?
* Use expression-bodied member
* Use lambda expression instead of anonymous method
* Use nameof operator
* Use predefined type

### List of Refactorings

* Add boolean comparison
* Add braces to embedded statement
* Add braces to if-else chain
* Add braces to switch section
* Add braces to switch sections
* Add cast according to parameter type
* Add parameter name
* Add parameter name to each argument
* Add parentheses
* Add using statement
* Convert "" to string.Empty
* Convert conditional expression to if-else
* Convert constant to read-only field
* Convert for to foreach
* Convert foreach to for
* Convert interpolated string to string literal
* Convert method to read-only property
* Convert read-only field to constant
* Convert read-only property to method
* Convert string literal to interpolated string
* Convert string.Empty to ""
* Convert switch to if-else chain
* Convert to increment/decrement operator
* Convert to interpolated string
* Convert to prefix/postfix operator
* Duplicate member
* Expand assignment expression
* Expand expression-bodied member
* Expand lambda expression's body
* Expand object initializer
* Expand property
* Expand property and add backing field
* Extract declaration from using statement
* Extract expression from parentheses
* Extract generic type
* Extract statement(s)
* Format all arguments on a single line
* Format all parameters on a single line
* Format binary expressions on multiple lines
* Format conditional expression on multiple lines
* Format conditional expression to a single line
* Format each argument on separate line
* Format each parameter on separate line
* Format expression chain on a single line
* Format expression chain on multiple lines
* Format initializer on a single line
* Format initializer on multiple lines
* Change 'Any/All' to 'All/Any'
* Change foreach variable's declared type according to expression
* Change foreach variable's declared type to implicit
* Change type
* Change type according to expression
* Check parameter for null
* Make member abstract
* Negate binary expression
* Negate boolean literal
* Negate operator
* Remove all comments
* Remove all comments (except xml comments)
* Remove all regions
* Remove braces from if-else chain
* Remove braces from switch section
* Remove braces from switch sections
* Remove comment
* Remove member
* Remove parameter name
* Remove parameter name from each argument
* Remove property initializer
* Rename backing field according to property name
* Rename foreach variable according to its type name
* Rename method according to type name
* Rename parameter according to its type name
* Rename property according to type name
* Rename variable/field/const according to type name
* Reverse for loop
* Swap conditional expression's statements
* Swap if-else statements
* Uncomment
