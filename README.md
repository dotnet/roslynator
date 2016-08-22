## Roslyn Tools
* Roslyn-based library that offers 80+ analyzers and 120+ refactorings for C#.
* [Release Notes](http://github.com/JosefPihrt/Pihrtsoft.CodeAnalysis/blob/master/ChangeLog.md)

### Documentation

* [Refactorings](http://github.com/JosefPihrt/Pihrtsoft.CodeAnalysis/blob/master/Refactorings.md)

### Options

* Refactorings can be enabled/disabled in Visual Studio options

![Refactorings Options](/images/RefactoringsOptions.png)

### Distribution

##### C# Analyzers and Refactorings
* [Visual Studio extension](http://visualstudiogallery.msdn.microsoft.com/e83c5e41-92c5-42a3-80cc-e0720c621b5e) that contains both analyzers and refactorings.

##### C# Refactorings
* [Visual Studio extension](http://visualstudiogallery.msdn.microsoft.com/a9a2b4bc-70da-437d-9ab7-b6b8e7d76cd9) that contains only refactorings.

##### C# Analyzers
* [NuGet package](http://www.nuget.org/packages/CSharpAnalyzers/) that contains only analyzers.

### List of Analyzers

* RCS1001 - Add braces
* RCS1002 - Remove braces
* RCS1003 - Add braces to if-else
* RCS1004 - Remove braces from if-else
* RCS1005 - Simplify nested using statement
* RCS1006 - Simplify else clause containing only if statement
* RCS1007 - Avoid embedded statement
* RCS1008 - Use explicit type instead of 'var' (when the type is not obvious)
* RCS1009 - Use explicit type instead of 'var' (in 'foreach')
* RCS1010 - Use 'var' instead of explicit type (when the type is obvious)
* RCS1012 - Use explicit type instead of 'var' (even if the type is obvious)
* RCS1013 - Use predefined type
* RCS1014 - Avoid implicitly-typed array
* RCS1015 - Use nameof operator
* RCS1016 - Use expression-bodied member
* RCS1017 - Avoid multiline expression body
* RCS1018 - Add access modifier
* RCS1019 - Reorder modifiers
* RCS1020 - Simplify Nullable<T> to T?
* RCS1021 - Simplify lambda expression
* RCS1022 - Simplify lambda expression parameter list
* RCS1023 - Format block
* RCS1024 - Format accessor list
* RCS1025 - Format each enum member on separate line
* RCS1026 - Format each statement on separate line
* RCS1027 - Format embedded statement on separate line
* RCS1028 - Format switch section statement on separate line
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
* RCS1039 - Remove empty attribute argument list
* RCS1040 - Remove empty else clause
* RCS1041 - Remove empty initializer
* RCS1042 - Remove enum default underlying type
* RCS1043 - Remove 'partial' modifier from type with a single part
* RCS1044 - Remove original exception from throw statement
* RCS1045 - Rename private field according to camel case with underscore
* RCS1046 - Asynchronous method name should end with 'Async'
* RCS1047 - Non-asynchronous method name should not end with 'Async'
* RCS1048 - Replace anonymous method with lambda expression
* RCS1049 - Simplify boolean comparison
* RCS1050 - Add constructor argument list
* RCS1051 - Wrap conditional expression condition in parentheses
* RCS1052 - Declare each attribute separately
* RCS1053 - Replace 'foreach' with 'for'
* RCS1054 - Merge local declaration with return statement
* RCS1055 - Avoid semicolon at the end of declaration
* RCS1056 - Avoid alias directive
* RCS1057 - Add empty line between declarations
* RCS1058 - Simplify assignment expression
* RCS1059 - Avoid locking on publicly accessible instance
* RCS1060 - Declare each type in separate file
* RCS1061 - Merge if statement with nested if statement
* RCS1062 - Replace interpolated string with string literal
* RCS1063 - Avoid usage of do statement to create an infinite loop
* RCS1064 - Avoid usage of for statement to create an infinite loop
* RCS1065 - Avoid usage of while statement to create an inifinite loop
* RCS1066 - Remove empty finally clause
* RCS1067 - Remove empty argument list
* RCS1068 - Simplify logical not expression
* RCS1069 - Remove unnecessary case label
* RCS1070 - Remove redundant default switch section
* RCS1071 - Remove redundant base constructor call
* RCS1072 - Remove empty namespace declaration
* RCS1073 - Replace if statement with return statement
* RCS1074 - Remove redundant constructor
* RCS1075 - Avoid empty catch clause that catches System.Exception
* RCS1076 - Format declaration braces
* RCS1077 - Simplify LINQ method chain
* RCS1078 - Replace string.Empty with ""
* RCS1079 - Throwing of new NotImplementedException
* RCS1080 - Replace 'Any' method with 'Count' or 'Length' property
* RCS1081 - Split variable declaration
* RCS1082 - Replace 'Count' method with 'Count' or 'Length' property
* RCS1083 - Replace 'Count' method with 'Any' method
* RCS1084 - Replace conditional expression with coalesce expression
* RCS1085 - Replace property with auto-implemented property

### List of Refactorings

* [Add boolean comparison](Refactorings.md#add-boolean-comparison)
* [Add braces](Refactorings.md#add-braces)
* [Add braces to 'if-else'](Refactorings.md#add-braces-to-if-else)
* [Add braces to switch section](Refactorings.md#add-braces-to-switch-section)
* [Add braces to switch sections](Refactorings.md#add-braces-to-switch-sections)
* [Add cast expression](Refactorings.md#add-cast-expression)
* [Add 'ConfigureAwait(false)'](Refactorings.md#add-configureawaitfalse)
* [Add default value to parameter](Refactorings.md#add-default-value-to-parameter)
* [Add default value to return statement](Refactorings.md#add-default-value-to-return-statement)
* [Add identifier to variable declaration](Refactorings.md#add-identifier-to-variable-declaration)
* [Add interpolation](Refactorings.md#add-interpolation)
* [Add parameter name to argument](Refactorings.md#add-parameter-name-to-argument)
* [Add parameter name to parameter](Refactorings.md#add-parameter-name-to-parameter)
* [Add 'using' directive](Refactorings.md#add-using-directive)
* [Change explicit type to 'var'](Refactorings.md#change-explicit-type-to-var)
* [Change method return type to 'void'](Refactorings.md#change-method-return-type-to-void)
* [Change method/property/indexer type according to return expression](Refactorings.md#change-methodpropertyindexer-type-according-to-return-expression)
* [Change method/property/indexer type according to yield return expression](Refactorings.md#change-methodpropertyindexer-type-according-to-yield-return-expression)
* [Change type according to expression](Refactorings.md#change-type-according-to-expression)
* [Change 'var' to explicit type](Refactorings.md#change-var-to-explicit-type)
* [Check parameter for null](Refactorings.md#check-parameter-for-null)
* [Collapse to initalizer](Refactorings.md#collapse-to-initalizer)
* [Comment out member](Refactorings.md#comment-out-member)
* [Comment out statement](Refactorings.md#comment-out-statement)
* [Duplicate argument](Refactorings.md#duplicate-argument)
* [Duplicate member](Refactorings.md#duplicate-member)
* [Duplicate parameter](Refactorings.md#duplicate-parameter)
* [Duplicate statement](Refactorings.md#duplicate-statement)
* [Expand assignment expression](Refactorings.md#expand-assignment-expression)
* [Expand coalesce expression](Refactorings.md#expand-coalesce-expression)
* [Expand event](Refactorings.md#expand-event)
* [Expand expression-bodied member](Refactorings.md#expand-expression-bodied-member)
* [Expand initializer](Refactorings.md#expand-initializer)
* [Expand lambda expression body](Refactorings.md#expand-lambda-expression-body)
* [Expand property](Refactorings.md#expand-property)
* [Expand property and add backing field](Refactorings.md#expand-property-and-add-backing-field)
* [Extract declaration from 'using' statement](Refactorings.md#extract-declaration-from-using-statement)
* [Extract expression from condition](Refactorings.md#extract-expression-from-condition)
* [Extract expression from parentheses](Refactorings.md#extract-expression-from-parentheses)
* [Extract generic type](Refactorings.md#extract-generic-type)
* [Extract statement(s)](Refactorings.md#extract-statements)
* [Format accessor braces](Refactorings.md#format-accessor-braces)
* [Format argument list](Refactorings.md#format-argument-list)
* [Format binary expression](Refactorings.md#format-binary-expression)
* [Format conditional expression](Refactorings.md#format-conditional-expression)
* [Format expression chain](Refactorings.md#format-expression-chain)
* [Format initializer](Refactorings.md#format-initializer)
* [Format parameter list](Refactorings.md#format-parameter-list)
* [Generate switch sections](Refactorings.md#generate-switch-sections)
* [Initialize local with default value](Refactorings.md#initialize-local-with-default-value)
* [Inline method](Refactorings.md#inline-method)
* [Introduce and initialize field](Refactorings.md#introduce-and-initialize-field)
* [Introduce and initialize property](Refactorings.md#introduce-and-initialize-property)
* [Introduce constructor](Refactorings.md#introduce-constructor)
* [Introduce 'using static' directive](Refactorings.md#introduce-using-static-directive)
* [Make member abstract](Refactorings.md#make-member-abstract)
* [Mark all members as 'static'](Refactorings.md#mark-all-members-as-static)
* [Mark member as 'static'](Refactorings.md#mark-member-as-static)
* [Merge assignment expression with return statement](Refactorings.md#merge-assignment-expression-with-return-statement)
* [Merge attributes](Refactorings.md#merge-attributes)
* [Merge 'if' statements](Refactorings.md#merge-if-statements)
* [Merge string literals](Refactorings.md#merge-string-literals)
* [Merge string literals into multiline string literal](Refactorings.md#merge-string-literals-into-multiline-string-literal)
* [Negate binary expression](Refactorings.md#negate-binary-expression)
* [Negate boolean literal](Refactorings.md#negate-boolean-literal)
* [Negate operator](Refactorings.md#negate-operator)
* [Notify property changed](Refactorings.md#notify-property-changed)
* [Remove all '#region' directives](Refactorings.md#remove-all-region-directives)
* [Remove all comments](Refactorings.md#remove-all-comments)
* [Remove all comments (except xml comments)](Refactorings.md#remove-all-comments-except-xml-comments)
* [Remove all member declarations](Refactorings.md#remove-all-member-declarations)
* [Remove all statements](Refactorings.md#remove-all-statements)
* [Remove all switch sections](Refactorings.md#remove-all-switch-sections)
* [Remove all xml comments](Refactorings.md#remove-all-xml-comments)
* [Remove braces](Refactorings.md#remove-braces)
* [Remove braces from 'if-else'](Refactorings.md#remove-braces-from-if-else)
* [Remove braces from switch section](Refactorings.md#remove-braces-from-switch-section)
* [Remove braces from switch sections](Refactorings.md#remove-braces-from-switch-sections)
* [Remove comment](Refactorings.md#remove-comment)
* [Remove condition from last 'else'](Refactorings.md#remove-condition-from-last-else)
* [Remove directive and related directives](Refactorings.md#remove-directive-and-related-directives)
* [Remove empty lines](Refactorings.md#remove-empty-lines)
* [Remove member](Refactorings.md#remove-member)
* [Remove member declarations above/below](Refactorings.md#remove-member-declarations-abovebelow)
* [Remove parameter name from argument](Refactorings.md#remove-parameter-name-from-argument)
* [Remove property initializer](Refactorings.md#remove-property-initializer)
* [Remove statement](Refactorings.md#remove-statement)
* [Remove statements from switch sections](Refactorings.md#remove-statements-from-switch-sections)
* [Rename backing field according to property name](Refactorings.md#rename-backing-field-according-to-property-name)
* [Rename identifier according to type name](Refactorings.md#rename-identifier-according-to-type-name)
* [Rename method according to type name](Refactorings.md#rename-method-according-to-type-name)
* [Rename parameter according to its type name](Refactorings.md#rename-parameter-according-to-its-type-name)
* [Rename property according to type name](Refactorings.md#rename-property-according-to-type-name)
* [Replace "" with 'string.Empty'](Refactorings.md#replace--with-stringempty)
* [Replace anonymous method with lambda expression](Refactorings.md#replace-anonymous-method-with-lambda-expression)
* [Replace 'Any/All' with 'All/Any'](Refactorings.md#replace-anyall-with-allany)
* [Replace boolean expression with 'if' statement](Refactorings.md#replace-boolean-expression-with-if-statement)
* [Replace conditional expression with 'if-else'](Refactorings.md#replace-conditional-expression-with-if-else)
* [Replace constant with field](Refactorings.md#replace-constant-with-field)
* [Replace 'Count/Length' with 'Length/Count'](Refactorings.md#replace-countlength-with-lengthcount)
* [Replace 'do' with 'while'](Refactorings.md#replace-do-with-while)
* [Replace field with constant](Refactorings.md#replace-field-with-constant)
* [Replace 'for' with 'foreach'](Refactorings.md#replace-for-with-foreach)
* [Replace 'foreach' with 'for'](Refactorings.md#replace-foreach-with-for)
* [Replace 'HasFlag' with bitwise operation](Refactorings.md#replace-hasflag-with-bitwise-operation)
* [Replace increment operator with decrement operator](Refactorings.md#replace-increment-operator-with-decrement-operator)
* [Replace interpolated string with string literal](Refactorings.md#replace-interpolated-string-with-string-literal)
* [Replace method invocation with '[]'](Refactorings.md#replace-method-invocation-with-)
* [Replace method with property](Refactorings.md#replace-method-with-property)
* [Replace prefix operator to postfix operator](Refactorings.md#replace-prefix-operator-to-postfix-operator)
* [Replace property with method](Refactorings.md#replace-property-with-method)
* [Replace regular string literal with verbatim string literal](Refactorings.md#replace-regular-string-literal-with-verbatim-string-literal)
* [Replace string literal with character literal](Refactorings.md#replace-string-literal-with-character-literal)
* [Replace string literal with interpolated string](Refactorings.md#replace-string-literal-with-interpolated-string)
* [Replace 'string.Empty' with ""](Refactorings.md#replace-stringempty-with-)
* [Replace 'string.Format' with interpolated string](Refactorings.md#replace-stringformat-with-interpolated-string)
* [Replace 'switch' with 'if-else'](Refactorings.md#replace-switch-with-if-else)
* [Replace verbatim string literal with regular string literal](Refactorings.md#replace-verbatim-string-literal-with-regular-string-literal)
* [Replace verbatim string literal with regular string literals](Refactorings.md#replace-verbatim-string-literal-with-regular-string-literals)
* [Replace 'while' with 'do'](Refactorings.md#replace-while-with-do)
* [Reverse 'for' loop](Refactorings.md#reverse-for-loop)
* [Simplify lambda expression](Refactorings.md#simplify-lambda-expression)
* [Split attributes](Refactorings.md#split-attributes)
* [Split variable declaration](Refactorings.md#split-variable-declaration)
* [Swap expressions in binary expression](Refactorings.md#swap-expressions-in-binary-expression)
* [Swap expressions in conditional expression](Refactorings.md#swap-expressions-in-conditional-expression)
* [Swap member declarations](Refactorings.md#swap-member-declarations)
* [Swap statements in 'if-else'](Refactorings.md#swap-statements-in-if-else)
* [Uncomment](Refactorings.md#uncomment)
* [Use expression-bodied member](Refactorings.md#use-expression-bodied-member)
* [Wrap expression in parentheses](Refactorings.md#wrap-expression-in-parentheses)
* [Wrap in '#if' directive](Refactorings.md#wrap-in-if-directive)
* [Wrap in '#region'](Refactorings.md#wrap-in-region)
* [Wrap in 'if' statement](Refactorings.md#wrap-in-if-statement)
* [Wrap in 'try-catch'](Refactorings.md#wrap-in-try-catch)
* [Wrap in 'using' statement](Refactorings.md#wrap-in-using-statement)
