## Roslyn Tools
* Roslyn-based library that offers 80+ analyzers and 130+ refactorings for C#.
* [Release Notes](http://github.com/JosefPihrt/RoslynTools/blob/master/ChangeLog.md)

### Documentation

* [Refactorings](http://github.com/JosefPihrt/RoslynTools/blob/master/Refactorings.md)

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
* RCS1006 - Merge else clause with nested if statement
* RCS1007 - Avoid embedded statement
* RCS1008 - Use explicit type instead of 'var' (when the type is not obvious)
* RCS1009 - Use explicit type instead of 'var' (foreach variable)
* RCS1010 - Use 'var' instead of explicit type (when the type is obvious)
* RCS1012 - Use explicit type instead of 'var' (even if the type is obvious)
* RCS1013 - Use predefined type
* RCS1014 - Avoid implicitly-typed array
* RCS1015 - Use 'nameof' operator
* RCS1016 - Use expression-bodied member
* RCS1017 - Avoid multiline expression body
* RCS1018 - Add default access modifier
* RCS1019 - Reorder modifiers
* RCS1020 - Simplify Nullable<T> to T?
* RCS1021 - Simplify lambda expression
* RCS1022 - Simplify lambda expression parameter list
* RCS1023 - Format empty block
* RCS1024 - Format accessor list
* RCS1025 - Format each enum member on a separate line
* RCS1026 - Format each statement on a separate line
* RCS1027 - Format embedded statement on a separate line
* RCS1028 - Format switch section's statement on a separate line
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
* RCS1054 - Merge local declaration with return statement
* RCS1055 - Avoid semicolon at the end of declaration
* RCS1056 - Avoid usage of using alias directive
* RCS1057 - Add empty line between declarations
* RCS1058 - Simplify assignment expression
* RCS1059 - Avoid locking on publicly accessible instance
* RCS1060 - Declare each type in separate file
* RCS1061 - Merge if statement with nested if statement
* RCS1062 - Avoid interpolated string with no interpolation
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

* [Add boolean comparison](source/Refactorings/Refactorings.md#add-boolean-comparison)
* [Add braces](source/Refactorings/Refactorings.md#add-braces)
* [Add braces to if-else](source/Refactorings/Refactorings.md#add-braces-to-if-else)
* [Add braces to switch section](source/Refactorings/Refactorings.md#add-braces-to-switch-section)
* [Add braces to switch sections](source/Refactorings/Refactorings.md#add-braces-to-switch-sections)
* [Add cast expression](source/Refactorings/Refactorings.md#add-cast-expression)
* [Add 'ConfigureAwait(false)'](source/Refactorings/Refactorings.md#add-configureawaitfalse)
* [Add default value to parameter](source/Refactorings/Refactorings.md#add-default-value-to-parameter)
* [Add default value to return statement](source/Refactorings/Refactorings.md#add-default-value-to-return-statement)
* [Add identifier to variable declaration](source/Refactorings/Refactorings.md#add-identifier-to-variable-declaration)
* [Add parameter name to argument](source/Refactorings/Refactorings.md#add-parameter-name-to-argument)
* [Add parameter name to parameter](source/Refactorings/Refactorings.md#add-parameter-name-to-parameter)
* [Add using directive](source/Refactorings/Refactorings.md#add-using-directive)
* [Add using static directive](source/Refactorings/Refactorings.md#add-using-static-directive)
* [Change explicit type to 'var'](source/Refactorings/Refactorings.md#change-explicit-type-to-var)
* [Change method return type to 'void'](source/Refactorings/Refactorings.md#change-method-return-type-to-void)
* [Change method/property/indexer type according to return expression](source/Refactorings/Refactorings.md#change-methodpropertyindexer-type-according-to-return-expression)
* [Change method/property/indexer type according to yield return expression](source/Refactorings/Refactorings.md#change-methodpropertyindexer-type-according-to-yield-return-expression)
* [Change type according to expression](source/Refactorings/Refactorings.md#change-type-according-to-expression)
* [Change 'var' to explicit type](source/Refactorings/Refactorings.md#change-var-to-explicit-type)
* [Check parameter for null](source/Refactorings/Refactorings.md#check-parameter-for-null)
* [Collapse to initalizer](source/Refactorings/Refactorings.md#collapse-to-initalizer)
* [Comment out member](source/Refactorings/Refactorings.md#comment-out-member)
* [Comment out statement](source/Refactorings/Refactorings.md#comment-out-statement)
* [Create condition from boolean expression](source/Refactorings/Refactorings.md#create-condition-from-boolean-expression)
* [Duplicate argument](source/Refactorings/Refactorings.md#duplicate-argument)
* [Duplicate member](source/Refactorings/Refactorings.md#duplicate-member)
* [Duplicate parameter](source/Refactorings/Refactorings.md#duplicate-parameter)
* [Duplicate statement](source/Refactorings/Refactorings.md#duplicate-statement)
* [Expand assignment expression](source/Refactorings/Refactorings.md#expand-assignment-expression)
* [Expand coalesce expression](source/Refactorings/Refactorings.md#expand-coalesce-expression)
* [Expand event](source/Refactorings/Refactorings.md#expand-event)
* [Expand expression-bodied member](source/Refactorings/Refactorings.md#expand-expression-bodied-member)
* [Expand initializer](source/Refactorings/Refactorings.md#expand-initializer)
* [Expand lambda expression body](source/Refactorings/Refactorings.md#expand-lambda-expression-body)
* [Expand property](source/Refactorings/Refactorings.md#expand-property)
* [Expand property and add backing field](source/Refactorings/Refactorings.md#expand-property-and-add-backing-field)
* [Extract declaration from using statement](source/Refactorings/Refactorings.md#extract-declaration-from-using-statement)
* [Extract expression from condition](source/Refactorings/Refactorings.md#extract-expression-from-condition)
* [Extract expression from parentheses](source/Refactorings/Refactorings.md#extract-expression-from-parentheses)
* [Extract generic type](source/Refactorings/Refactorings.md#extract-generic-type)
* [Extract statement(s)](source/Refactorings/Refactorings.md#extract-statements)
* [Format accessor braces](source/Refactorings/Refactorings.md#format-accessor-braces)
* [Format argument list](source/Refactorings/Refactorings.md#format-argument-list)
* [Format binary expression](source/Refactorings/Refactorings.md#format-binary-expression)
* [Format conditional expression](source/Refactorings/Refactorings.md#format-conditional-expression)
* [Format expression chain](source/Refactorings/Refactorings.md#format-expression-chain)
* [Format initializer](source/Refactorings/Refactorings.md#format-initializer)
* [Format parameter list](source/Refactorings/Refactorings.md#format-parameter-list)
* [Generate switch sections](source/Refactorings/Refactorings.md#generate-switch-sections)
* [Initialize local with default value](source/Refactorings/Refactorings.md#initialize-local-with-default-value)
* [Inline method](source/Refactorings/Refactorings.md#inline-method)
* [Insert string interpolation](source/Refactorings/Refactorings.md#insert-string-interpolation)
* [Introduce and initialize field](source/Refactorings/Refactorings.md#introduce-and-initialize-field)
* [Introduce and initialize property](source/Refactorings/Refactorings.md#introduce-and-initialize-property)
* [Introduce constructor](source/Refactorings/Refactorings.md#introduce-constructor)
* [Make member abstract](source/Refactorings/Refactorings.md#make-member-abstract)
* [Mark all members as static](source/Refactorings/Refactorings.md#mark-all-members-as-static)
* [Mark member as static](source/Refactorings/Refactorings.md#mark-member-as-static)
* [Merge assignment expression with return statement](source/Refactorings/Refactorings.md#merge-assignment-expression-with-return-statement)
* [Merge attributes](source/Refactorings/Refactorings.md#merge-attributes)
* [Merge if statements](source/Refactorings/Refactorings.md#merge-if-statements)
* [Merge string literals](source/Refactorings/Refactorings.md#merge-string-literals)
* [Merge string literals into multiline string literal](source/Refactorings/Refactorings.md#merge-string-literals-into-multiline-string-literal)
* [Negate binary expression](source/Refactorings/Refactorings.md#negate-binary-expression)
* [Negate boolean literal](source/Refactorings/Refactorings.md#negate-boolean-literal)
* [Negate operator](source/Refactorings/Refactorings.md#negate-operator)
* [Notify property changed](source/Refactorings/Refactorings.md#notify-property-changed)
* [Remove all comments](source/Refactorings/Refactorings.md#remove-all-comments)
* [Remove all comments (except documentation comments)](source/Refactorings/Refactorings.md#remove-all-comments-except-documentation-comments)
* [Remove all documentation comments](source/Refactorings/Refactorings.md#remove-all-documentation-comments)
* [Remove all member declarations](source/Refactorings/Refactorings.md#remove-all-member-declarations)
* [Remove all region directives](source/Refactorings/Refactorings.md#remove-all-region-directives)
* [Remove all statements](source/Refactorings/Refactorings.md#remove-all-statements)
* [Remove all switch sections](source/Refactorings/Refactorings.md#remove-all-switch-sections)
* [Remove braces](source/Refactorings/Refactorings.md#remove-braces)
* [Remove braces from if-else](source/Refactorings/Refactorings.md#remove-braces-from-if-else)
* [Remove braces from switch section](source/Refactorings/Refactorings.md#remove-braces-from-switch-section)
* [Remove braces from switch sections](source/Refactorings/Refactorings.md#remove-braces-from-switch-sections)
* [Remove comment](source/Refactorings/Refactorings.md#remove-comment)
* [Remove condition from last else clause](source/Refactorings/Refactorings.md#remove-condition-from-last-else-clause)
* [Remove directive and related directives](source/Refactorings/Refactorings.md#remove-directive-and-related-directives)
* [Remove empty lines](source/Refactorings/Refactorings.md#remove-empty-lines)
* [Remove member](source/Refactorings/Refactorings.md#remove-member)
* [Remove member declarations above/below](source/Refactorings/Refactorings.md#remove-member-declarations-abovebelow)
* [Remove parameter name from argument](source/Refactorings/Refactorings.md#remove-parameter-name-from-argument)
* [Remove property initializer](source/Refactorings/Refactorings.md#remove-property-initializer)
* [Remove statement](source/Refactorings/Refactorings.md#remove-statement)
* [Remove statements from switch sections](source/Refactorings/Refactorings.md#remove-statements-from-switch-sections)
* [Rename backing field according to property name](source/Refactorings/Refactorings.md#rename-backing-field-according-to-property-name)
* [Rename identifier according to type name](source/Refactorings/Refactorings.md#rename-identifier-according-to-type-name)
* [Rename method according to type name](source/Refactorings/Refactorings.md#rename-method-according-to-type-name)
* [Rename parameter according to its type name](source/Refactorings/Refactorings.md#rename-parameter-according-to-its-type-name)
* [Rename property according to type name](source/Refactorings/Refactorings.md#rename-property-according-to-type-name)
* [Replace "" with 'string.Empty'](source/Refactorings/Refactorings.md#replace--with-stringempty)
* [Replace anonymous method with lambda expression](source/Refactorings/Refactorings.md#replace-anonymous-method-with-lambda-expression)
* [Replace 'Any/All' with 'All/Any'](source/Refactorings/Refactorings.md#replace-anyall-with-allany)
* [Replace conditional expression with expression](source/Refactorings/Refactorings.md#replace-conditional-expression-with-expression)
* [Replace conditional expression with if-else](source/Refactorings/Refactorings.md#replace-conditional-expression-with-if-else)
* [Replace constant with field](source/Refactorings/Refactorings.md#replace-constant-with-field)
* [Replace 'Count/Length' with 'Length/Count'](source/Refactorings/Refactorings.md#replace-countlength-with-lengthcount)
* [Replace do statement with while statement](source/Refactorings/Refactorings.md#replace-do-statement-with-while-statement)
* [Replace field with constant](source/Refactorings/Refactorings.md#replace-field-with-constant)
* [Replace for statement with foreach statement](source/Refactorings/Refactorings.md#replace-for-statement-with-foreach-statement)
* [Replace foreach statement with for statement](source/Refactorings/Refactorings.md#replace-foreach-statement-with-for-statement)
* [Replace 'HasFlag' with bitwise operation](source/Refactorings/Refactorings.md#replace-hasflag-with-bitwise-operation)
* [Replace if-else with conditional expression](source/Refactorings/Refactorings.md#replace-if-else-with-conditional-expression)
* [Replace increment operator with decrement operator](source/Refactorings/Refactorings.md#replace-increment-operator-with-decrement-operator)
* [Replace interpolated string with string literal](source/Refactorings/Refactorings.md#replace-interpolated-string-with-string-literal)
* [Replace method invocation with '[]'](source/Refactorings/Refactorings.md#replace-method-invocation-with-)
* [Replace method with property](source/Refactorings/Refactorings.md#replace-method-with-property)
* [Replace prefix operator to postfix operator](source/Refactorings/Refactorings.md#replace-prefix-operator-to-postfix-operator)
* [Replace property with method](source/Refactorings/Refactorings.md#replace-property-with-method)
* [Replace regular string literal with verbatim string literal](source/Refactorings/Refactorings.md#replace-regular-string-literal-with-verbatim-string-literal)
* [Replace string literal with character literal](source/Refactorings/Refactorings.md#replace-string-literal-with-character-literal)
* [Replace 'string.Empty' with ""](source/Refactorings/Refactorings.md#replace-stringempty-with-)
* [Replace 'string.Format' with interpolated string](source/Refactorings/Refactorings.md#replace-stringformat-with-interpolated-string)
* [Replace switch statement with if-else](source/Refactorings/Refactorings.md#replace-switch-statement-with-if-else)
* [Replace verbatim string literal with regular string literal](source/Refactorings/Refactorings.md#replace-verbatim-string-literal-with-regular-string-literal)
* [Replace verbatim string literal with regular string literals](source/Refactorings/Refactorings.md#replace-verbatim-string-literal-with-regular-string-literals)
* [Replace while statement with do statement](source/Refactorings/Refactorings.md#replace-while-statement-with-do-statement)
* [Reverse for loop](source/Refactorings/Refactorings.md#reverse-for-loop)
* [Simplify lambda expression](source/Refactorings/Refactorings.md#simplify-lambda-expression)
* [Split attributes](source/Refactorings/Refactorings.md#split-attributes)
* [Split variable declaration](source/Refactorings/Refactorings.md#split-variable-declaration)
* [Swap expressions in binary expression](source/Refactorings/Refactorings.md#swap-expressions-in-binary-expression)
* [Swap expressions in conditional expression](source/Refactorings/Refactorings.md#swap-expressions-in-conditional-expression)
* [Swap member declarations](source/Refactorings/Refactorings.md#swap-member-declarations)
* [Swap statements in if-else](source/Refactorings/Refactorings.md#swap-statements-in-if-else)
* [Uncomment](source/Refactorings/Refactorings.md#uncomment)
* [Use expression-bodied member](source/Refactorings/Refactorings.md#use-expression-bodied-member)
* [Wrap expression in parentheses](source/Refactorings/Refactorings.md#wrap-expression-in-parentheses)
* [Wrap in #if directive](source/Refactorings/Refactorings.md#wrap-in-if-directive)
* [Wrap in if statement](source/Refactorings/Refactorings.md#wrap-in-if-statement)
* [Wrap in region](source/Refactorings/Refactorings.md#wrap-in-region)
* [Wrap in try-catch](source/Refactorings/Refactorings.md#wrap-in-try-catch)
* [Wrap in using statement](source/Refactorings/Refactorings.md#wrap-in-using-statement)
