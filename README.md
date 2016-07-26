## C# Analyzers and Refactorings
* Roslyn-based library that offers 80+ analyzers and 100+ refactorings for C#.
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

* RCS1001 - Replace embedded statement with block
* RCS1002 - Replace block with embedded statement
* RCS1003 - Replace embedded statement with block (in if-else)
* RCS1004 - Replace block with embedded statement (in if-else)
* RCS1005 - Simplify nested using statement
* RCS1006 - Simplify else clause containing only if statement
* RCS1007 - Avoid embedded statement
* RCS1008 - Replace 'var' with explicit type (when the type is not obvious)
* RCS1009 - Replace 'var' with explicit type (in foreach)
* RCS1010 - Replace explicit type with 'var' (when the type is obvious)
* RCS1012 - Replace 'var' with explicit type (even if the type is obvious)
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
* RCS1053 - Replace foreach with for
* RCS1054 - Merge local declaration with return statement
* RCS1055 - Avoid semicolon at the end of declaration
* RCS1056 - Avoid using alias directive
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
* [Add cast expression](Refactorings.md#add-cast-expression)
* [Add default value to parameter](Refactorings.md#add-default-value-to-parameter)
* [Add identifier to variable declaration](Refactorings.md#add-identifier-to-variable-declaration)
* [Add interpolation](Refactorings.md#add-interpolation)
* [Add parameter name to argument](Refactorings.md#add-parameter-name-to-argument)
* [Add parameter name to parameter](Refactorings.md#add-parameter-name-to-parameter)
* [Change method return type to void](Refactorings.md#change-method-return-type-to-void)
* [Change method/property/indexer type according to return expression](Refactorings.md#change-methodpropertyindexer-type-according-to-return-expression)
* [Change method/property/indexer type according to yield return expression](Refactorings.md#change-methodpropertyindexer-type-according-to-yield-return-expression)
* [Change type according to expression](Refactorings.md#change-type-according-to-expression)
* [Check parameter for null](Refactorings.md#check-parameter-for-null)
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
* [Extract declaration from using statement](Refactorings.md#extract-declaration-from-using-statement)
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
* [Introduce constructor from selected member(s)](Refactorings.md#introduce-constructor-from-selected-members)
* [Introduce using static directive](Refactorings.md#introduce-using-static-directive)
* [Make member abstract](Refactorings.md#make-member-abstract)
* [Mark all members as static](Refactorings.md#mark-all-members-as-static)
* [Mark member as static](Refactorings.md#mark-member-as-static)
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
* [Remove all xml comments](Refactorings.md#remove-all-xml-comments)
* [Remove comment](Refactorings.md#remove-comment)
* [Remove condition from last else-if](Refactorings.md#remove-condition-from-last-else-if)
* [Remove empty lines](Refactorings.md#remove-empty-lines)
* [Remove member](Refactorings.md#remove-member)
* [Remove member declarations above/below](Refactorings.md#remove-member-declarations-abovebelow)
* [Remove parameter name from argument](Refactorings.md#remove-parameter-name-from-argument)
* [Remove property initializer](Refactorings.md#remove-property-initializer)
* [Remove statement](Refactorings.md#remove-statement)
* [Rename backing field according to property name](Refactorings.md#rename-backing-field-according-to-property-name)
* [Rename identifier according to type name](Refactorings.md#rename-identifier-according-to-type-name)
* [Rename method according to type name](Refactorings.md#rename-method-according-to-type-name)
* [Rename parameter according to its type name](Refactorings.md#rename-parameter-according-to-its-type-name)
* [Rename property according to type name](Refactorings.md#rename-property-according-to-type-name)
* [Replace "" with string.Empty](Refactorings.md#replace--with-stringempty)
* [Replace anonymous method with lambda expression](Refactorings.md#replace-anonymous-method-with-lambda-expression)
* [Replace 'Any/All' with 'All/Any'](Refactorings.md#replace-anyall-with-allany)
* [Replace block with embedded statement](Refactorings.md#replace-block-with-embedded-statement)
* [Replace block with embedded statement (in if-else)](Refactorings.md#replace-block-with-embedded-statement-in-if-else)
* [Replace block with statements (in each section)](Refactorings.md#replace-block-with-statements-in-each-section)
* [Replace conditional expression with if-else](Refactorings.md#replace-conditional-expression-with-if-else)
* [Replace constant with field](Refactorings.md#replace-constant-with-field)
* [Replace 'Count/Length' with 'Length/Count'](Refactorings.md#replace-countlength-with-lengthcount)
* [Replace do statement with while statement](Refactorings.md#replace-do-statement-with-while-statement)
* [Replace embedded statement with block](Refactorings.md#replace-embedded-statement-with-block)
* [Replace embedded statement with block (in if-else)](Refactorings.md#replace-embedded-statement-with-block-in-if-else)
* [Replace Enum.HasFlag method with bitwise operation](Refactorings.md#replace-enumhasflag-method-with-bitwise-operation)
* [Replace explicit type with 'var'](Refactorings.md#replace-explicit-type-with-var)
* [Replace field with constant](Refactorings.md#replace-field-with-constant)
* [Replace for with foreach](Refactorings.md#replace-for-with-foreach)
* [Replace foreach with for](Refactorings.md#replace-foreach-with-for)
* [Replace increment operator with decrement operator](Refactorings.md#replace-increment-operator-with-decrement-operator)
* [Replace interpolated string with string literal](Refactorings.md#replace-interpolated-string-with-string-literal)
* [Replace method invocation with '[]'](Refactorings.md#replace-method-invocation-with-)
* [Replace method with property](Refactorings.md#replace-method-with-property)
* [Replace prefix operator to postfix operator](Refactorings.md#replace-prefix-operator-to-postfix-operator)
* [Replace property with method](Refactorings.md#replace-property-with-method)
* [Replace regular string literal with verbatim string literal](Refactorings.md#replace-regular-string-literal-with-verbatim-string-literal)
* [Replace return statement with if statement](Refactorings.md#replace-return-statement-with-if-statement)
* [Replace statements with block (in each section)](Refactorings.md#replace-statements-with-block-in-each-section)
* [Replace string literal with character literal](Refactorings.md#replace-string-literal-with-character-literal)
* [Replace string literal with interpolated string](Refactorings.md#replace-string-literal-with-interpolated-string)
* [Replace string.Empty with ""](Refactorings.md#replace-stringempty-with-)
* [Replace string.Format with interpolated string](Refactorings.md#replace-stringformat-with-interpolated-string)
* [Replace switch section block with statements](Refactorings.md#replace-switch-section-block-with-statements)
* [Replace switch section statements with block](Refactorings.md#replace-switch-section-statements-with-block)
* [Replace switch with if-else](Refactorings.md#replace-switch-with-if-else)
* [Replace 'var' with explicit type](Refactorings.md#replace-var-with-explicit-type)
* [Replace verbatim string literal with regular string literal](Refactorings.md#replace-verbatim-string-literal-with-regular-string-literal)
* [Replace verbatim string literal with regular string literals](Refactorings.md#replace-verbatim-string-literal-with-regular-string-literals)
* [Replace while statement with do statement](Refactorings.md#replace-while-statement-with-do-statement)
* [Reverse for loop](Refactorings.md#reverse-for-loop)
* [Simplify lambda expression](Refactorings.md#simplify-lambda-expression)
* [Split attributes](Refactorings.md#split-attributes)
* [Split variable declaration ](Refactorings.md#split-variable-declaration-)
* [Swap expressions in binary expression](Refactorings.md#swap-expressions-in-binary-expression)
* [Swap expressions in conditional expression](Refactorings.md#swap-expressions-in-conditional-expression)
* [Swap member declarations](Refactorings.md#swap-member-declarations)
* [Swap statements in if-else](Refactorings.md#swap-statements-in-if-else)
* [Uncomment](Refactorings.md#uncomment)
* [Use expression-bodied member](Refactorings.md#use-expression-bodied-member)
* [Wrap declaration in using statement](Refactorings.md#wrap-declaration-in-using-statement)
* [Wrap expression in parentheses](Refactorings.md#wrap-expression-in-parentheses)
* [Wrap statements in if statement](Refactorings.md#wrap-statements-in-if-statement)
* [Wrap statements in try-catch](Refactorings.md#wrap-statements-in-try-catch)
