## C# Analyzers and Refactorings
* Roslyn-based library that offers 60+ analyzers and 70+ refactorings for C#.
* [Release Notes](http://github.com/JosefPihrt/Pihrtsoft.CodeAnalysis/blob/master/ChangeLog.md)
* http://pihrt.net/roslyn

### Distribution

##### C# Analyzers and Refactorings
* [Visual Studio extension](http://visualstudiogallery.msdn.microsoft.com/e83c5e41-92c5-42a3-80cc-e0720c621b5e) that contains both analyzers and refactorings.

##### C# Refactorings
* [Visual Studio extension](http://visualstudiogallery.msdn.microsoft.com/a9a2b4bc-70da-437d-9ab7-b6b8e7d76cd9) that contains only refactorings.

##### C# Analyzers
* [NuGet package](http://www.nuget.org/packages/CSharpAnalyzers/) that contains only analyzers.

### List of Analyzers

* RCS1001 - Add braces to a statement
* RCS1002 - Remove braces from a statement
* RCS1003 - Add braces to if-else chain
* RCS1004 - Remove braces from if-else chain
* RCS1005 - Simplify nested using statement
* RCS1006 - Simplify else clause containing only if statement
* RCS1007 - Avoid embedded statement
* RCS1008 - Declare explicit type (when the type is not obvious)
* RCS1009 - Declare explicit type in foreach (when the type is not obvious)
* RCS1010 - Declare implicit type (when the type is obvious)
* RCS1012 - Declare explicit type (even if the type is obvious)
* RCS1013 - Use predefined type
* RCS1014 - Avoid implicit array creation
* RCS1015 - Use nameof operator
* RCS1016 - Use expression-bodied member
* RCS1017 - Avoid multiline expression body
* RCS1018 - Add access modifier
* RCS1019 - Reorder modifiers
* RCS1020 - Simplify Nullable<T> to T?
* RCS1021 - Simplify lambda expression
* RCS1022 - Simplify lambda expression's parameter list
* RCS1023 - Format block
* RCS1024 - Format accessor list
* RCS1025 - Format each enum member on separate line
* RCS1026 - Format each statement on separate line
* RCS1027 - Format embedded statement on separate line
* RCS1028 - Format switch section's statement on separate line
* RCS1029 - Format binary operator on next line
* RCS1030 - Add empty line after embedded statement
* RCS1031 - Remove redundant braces
* RCS1032 - Remove redundant parentheses
* RCS1033 - Remove redundant boolean literal
* RCS1034 - Remove redundant 'sealed' modifier
* RCS1035 - Remove redundant comma in initializer
* RCS1036 - Remove redundant empty line
* RCS1037 - Remove trailing white-space
* RCS1038 - Remove empty statement
* RCS1039 - Remove empty attribute's argument list
* RCS1040 - Remove empty else clause
* RCS1041 - Remove empty object initializer
* RCS1042 - Remove enum's default underlying type
* RCS1043 - Remove 'partial' modifier from type with a single part
* RCS1044 - Remove original exception from throw statement
* RCS1045 - Rename private field according to camel case with underscore
* RCS1046 - Add 'Async' suffix to asynchronous method name
* RCS1047 - Remove 'Async' suffix from non-asynchronous method name
* RCS1048 - Use lambda expression instead of anonymous method
* RCS1049 - Simplify boolean comparison
* RCS1050 - Add constructor's argument list
* RCS1051 - Add parentheses to conditional expression's condition
* RCS1052 - Declare each attribute separately
* RCS1053 - Convert foreach to for
* RCS1054 - Merge local declaration with return statement
* RCS1055 - Avoid semicolon at the end of declaration
* RCS1056 - Avoid using alias directive
* RCS1057 - Add empty line between declarations
* RCS1058 - Simplify assignment expression
* RCS1059 - Avoid locking on publicly accessible instance
* RCS1060 - Declare each type in separate file
* RCS1061 - Merge if statement with contained if statement
* RCS1062 - Use string literal instead of interpolated string
* RCS1063 - Avoid usage of do statement to create an infinite loop
* RCS1064 - Use while statement to create an infinite loop
* RCS1065 - Use for statement to create an infinite loop

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
