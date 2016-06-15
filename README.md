## C# Analyzers and Refactorings
* Roslyn-based library that offers 80+ analyzers and 100+ refactorings for C#.
* [Release Notes](http://github.com/JosefPihrt/Pihrtsoft.CodeAnalysis/blob/master/ChangeLog.md)

### Documentation

* [Refactorings](http://github.com/JosefPihrt/Pihrtsoft.CodeAnalysis/blob/master/Refactorings.md)

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
* RCS1066 - Remove empty finally clause
* RCS1067 - Remove empty argument list
* RCS1068 - Simplify logical not expression
* RCS1069 - Remove unnecessary case label
* RCS1070 - Remove redundant default switch section
* RCS1071 - Remove redundant base constructor call
* RCS1072 - Remove empty namespace declaration
* RCS1073 - Simplify if statement to return statement
* RCS1074 - Remove redundant constructor
* RCS1075 - Avoid empty catch clause that catches System.Exception
* RCS1076 - Format declaration braces
* RCS1077 - Simplify LINQ method chain
* RCS1078 - Avoid usage of string.Empty
* RCS1079 - Throwing of new NotImplementedException
* RCS1080 - Use 'Count' or 'Length' property instead of 'Any' method
* RCS1081 - Split declaration into multiple declarations
* RCS1082 - Use 'Count' or 'Length' property instead of 'Count' method
* RCS1083 - Use 'Any' method instead of 'Count' method
* RCS1084 - Use coalesce expression instead of conditional expression
* RCS1085 - Use auto-implemented property

### List of Refactorings

* [Access element using '[]' instead of 'First/Last/ElementAt' method](Refactorings.md#access-element-using--instead-of-firstlastelementat-method)
* [Add boolean comparison](Refactorings.md#add-boolean-comparison)
* [Add braces to embedded statement](Refactorings.md#add-braces-to-embedded-statement)
* [Add braces to if-else chain](Refactorings.md#add-braces-to-if-else-chain)
* [Add braces to switch section](Refactorings.md#add-braces-to-switch-section)
* [Add braces to switch sections](Refactorings.md#add-braces-to-switch-sections)
* [Add cast according to parameter type](Refactorings.md#add-cast-according-to-parameter-type)
* [Add cast to assignment expression](Refactorings.md#add-cast-to-assignment-expression)
* [Add cast to return statement's expression](Refactorings.md#add-cast-to-return-statements-expression)
* [Add cast to variable declaration](Refactorings.md#add-cast-to-variable-declaration)
* [Add parameter name according to its type name](Refactorings.md#add-parameter-name-according-to-its-type-name)
* [Add parameter name to argument](Refactorings.md#add-parameter-name-to-argument)
* [Add parentheses](Refactorings.md#add-parentheses)
* [Add using statement](Refactorings.md#add-using-statement)
* [Change 'Any/All' to 'All/Any'](Refactorings.md#change-anyall-to-allany)
* [Change foreach variable's declared type according to expression](Refactorings.md#change-foreach-variables-declared-type-according-to-expression)
* [Change foreach variable's declared type to 'var'](Refactorings.md#change-foreach-variables-declared-type-to-var)
* [Change method/property/indexer type according to return statement](Refactorings.md#change-methodpropertyindexer-type-according-to-return-statement)
* [Change method/property/indexer type according to yield return statement](Refactorings.md#change-methodpropertyindexer-type-according-to-yield-return-statement)
* [Change type according to expression](Refactorings.md#change-type-according-to-expression)
* [Change variable declaration type](Refactorings.md#change-variable-declaration-type)
* [Check parameter for null](Refactorings.md#check-parameter-for-null)
* [Convert "" to string.Empty](Refactorings.md#convert--to-stringempty)
* [Convert conditional expression to if-else](Refactorings.md#convert-conditional-expression-to-if-else)
* [Convert constant to read-only field](Refactorings.md#convert-constant-to-read-only-field)
* [Convert for to foreach](Refactorings.md#convert-for-to-foreach)
* [Convert foreach to for](Refactorings.md#convert-foreach-to-for)
* [Convert interpolated string to string literal](Refactorings.md#convert-interpolated-string-to-string-literal)
* [Convert method to read-only property](Refactorings.md#convert-method-to-read-only-property)
* [Convert read-only field to constant](Refactorings.md#convert-read-only-field-to-constant)
* [Convert read-only property to method](Refactorings.md#convert-read-only-property-to-method)
* [Convert regular string literal to verbatim string literal](Refactorings.md#convert-regular-string-literal-to-verbatim-string-literal)
* [Convert string literal to interpolated string](Refactorings.md#convert-string-literal-to-interpolated-string)
* [Convert string.Empty to ""](Refactorings.md#convert-stringempty-to-)
* [Convert switch to if-else chain](Refactorings.md#convert-switch-to-if-else-chain)
* [Convert to increment/decrement operator](Refactorings.md#convert-to-incrementdecrement-operator)
* [Convert to interpolated string](Refactorings.md#convert-to-interpolated-string)
* [Convert to prefix/postfix operator](Refactorings.md#convert-to-prefixpostfix-operator)
* [Convert verbatim string literal to regular string literal](Refactorings.md#convert-verbatim-string-literal-to-regular-string-literal)
* [Convert verbatim string literal to regular string literals](Refactorings.md#convert-verbatim-string-literal-to-regular-string-literals)
* [Duplicate argument](Refactorings.md#duplicate-argument)
* [Duplicate member](Refactorings.md#duplicate-member)
* [Duplicate parameter](Refactorings.md#duplicate-parameter)
* [Expand assignment expression](Refactorings.md#expand-assignment-expression)
* [Expand coalesce expression](Refactorings.md#expand-coalesce-expression)
* [Expand event](Refactorings.md#expand-event)
* [Expand expression-bodied member](Refactorings.md#expand-expression-bodied-member)
* [Expand lambda expression's body](Refactorings.md#expand-lambda-expressions-body)
* [Expand object initializer](Refactorings.md#expand-object-initializer)
* [Expand property](Refactorings.md#expand-property)
* [Expand property and add backing field](Refactorings.md#expand-property-and-add-backing-field)
* [Extract declaration from using statement](Refactorings.md#extract-declaration-from-using-statement)
* [Extract expression from parentheses](Refactorings.md#extract-expression-from-parentheses)
* [Extract generic type](Refactorings.md#extract-generic-type)
* [Extract statement(s)](Refactorings.md#extract-statements)
* [Format accessor braces on multiple lines](Refactorings.md#format-accessor-braces-on-multiple-lines)
* [Format all arguments on a single line](Refactorings.md#format-all-arguments-on-a-single-line)
* [Format all parameters on a single line](Refactorings.md#format-all-parameters-on-a-single-line)
* [Format binary expressions on multiple lines](Refactorings.md#format-binary-expressions-on-multiple-lines)
* [Format conditional expression on multiple lines](Refactorings.md#format-conditional-expression-on-multiple-lines)
* [Format conditional expression to a single line](Refactorings.md#format-conditional-expression-to-a-single-line)
* [Format each argument on separate line](Refactorings.md#format-each-argument-on-separate-line)
* [Format each parameter on separate line](Refactorings.md#format-each-parameter-on-separate-line)
* [Format expression chain on a single line](Refactorings.md#format-expression-chain-on-a-single-line)
* [Format expression chain on multiple lines](Refactorings.md#format-expression-chain-on-multiple-lines)
* [Format initializer on a single line](Refactorings.md#format-initializer-on-a-single-line)
* [Format initializer on multiple lines](Refactorings.md#format-initializer-on-multiple-lines)
* [Introduce constructor from selected member(s)](Refactorings.md#introduce-constructor-from-selected-members)
* [Make member abstract](Refactorings.md#make-member-abstract)
* [Merge attributes](Refactorings.md#merge-attributes)
* [Merge string literals](Refactorings.md#merge-string-literals)
* [Merge string literals into multiline string literal](Refactorings.md#merge-string-literals-into-multiline-string-literal)
* [Negate binary expression](Refactorings.md#negate-binary-expression)
* [Negate boolean literal](Refactorings.md#negate-boolean-literal)
* [Negate operator](Refactorings.md#negate-operator)
* [Notify property changed](Refactorings.md#notify-property-changed)
* [Remove all comments](Refactorings.md#remove-all-comments)
* [Remove all comments (except xml comments)](Refactorings.md#remove-all-comments-except-xml-comments)
* [Remove all regions](Refactorings.md#remove-all-regions)
* [Remove braces from if-else chain](Refactorings.md#remove-braces-from-if-else-chain)
* [Remove braces from switch section](Refactorings.md#remove-braces-from-switch-section)
* [Remove braces from switch sections](Refactorings.md#remove-braces-from-switch-sections)
* [Remove comment](Refactorings.md#remove-comment)
* [Remove member](Refactorings.md#remove-member)
* [Remove parameter name from argument](Refactorings.md#remove-parameter-name-from-argument)
* [Remove property initializer](Refactorings.md#remove-property-initializer)
* [Rename backing field according to property name](Refactorings.md#rename-backing-field-according-to-property-name)
* [Rename foreach variable according to its type name](Refactorings.md#rename-foreach-variable-according-to-its-type-name)
* [Rename local/field/const according to type name](Refactorings.md#rename-localfieldconst-according-to-type-name)
* [Rename method according to type name](Refactorings.md#rename-method-according-to-type-name)
* [Rename parameter according to its type name](Refactorings.md#rename-parameter-according-to-its-type-name)
* [Rename property according to type name](Refactorings.md#rename-property-according-to-type-name)
* [Reverse for loop](Refactorings.md#reverse-for-loop)
* [Split attributes](Refactorings.md#split-attributes)
* [Swap arguments](Refactorings.md#swap-arguments)
* [Swap expressions in binary expression](Refactorings.md#swap-expressions-in-binary-expression)
* [Swap expressions in conditional expression](Refactorings.md#swap-expressions-in-conditional-expression)
* [Swap members](Refactorings.md#swap-members)
* [Swap parameters](Refactorings.md#swap-parameters)
* [Swap statements in if-else](Refactorings.md#swap-statements-in-if-else)
* [Uncomment](Refactorings.md#uncomment)
* [Use expression-bodied member](Refactorings.md#use-expression-bodied-member)
