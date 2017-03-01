## Roslynator
* A collection of 160+ analyzers and 170+ refactorings for C#, powered by Roslyn.
* [Analyzers](http://github.com/JosefPihrt/Roslynator/blob/master/source/Analyzers/README.md)
* [Refactorings](http://github.com/JosefPihrt/Roslynator/blob/master/source/Refactorings/README.md)
* [Release Notes](http://github.com/JosefPihrt/Roslynator/blob/master/ChangeLog.md)

### Settings

* Analyzers can be enabled/disabled using **rule set**. Please see [How to Customize Analyzers](http://github.com/JosefPihrt/Roslynator/blob/master/docs/HowToCustomizeAnalyzers.md).
* Refactorings can be enabled/disabled in Visual Studio options

![Refactorings Options](/images/RefactoringsOptions.png)

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

### List of Analyzers

* RCS1001 - Add braces
* RCS1002 - Remove braces
* RCS1003 - Add braces to if\-else
* RCS1004 - Remove braces from if\-else
* RCS1005 - Simplify nested using statement
* RCS1006 - Merge else clause with nested if statement
* RCS1007 - Avoid embedded statement
* RCS1008 - Use explicit type instead of 'var' \(when the type is not obvious\)
* RCS1009 - Use explicit type instead of 'var' \(foreach variable\)
* RCS1010 - Use 'var' instead of explicit type \(when the type is obvious\)
* RCS1012 - Use explicit type instead of 'var' \(even if the type is obvious\)
* RCS1013 - Use predefined type
* RCS1014 - Avoid implicitly\-typed array
* RCS1015 - Use nameof operator
* RCS1016 - Use expression\-bodied member
* RCS1017 - Avoid multiline expression body
* RCS1018 - Add default access modifier
* RCS1019 - Reorder modifiers
* RCS1020 - Simplify Nullable\<T\> to T?
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
* RCS1032 - Remove redundant parentheses
* RCS1033 - Remove redundant boolean literal
* RCS1034 - Remove redundant sealed modifier
* RCS1035 - Remove redundant comma in initializer
* RCS1036 - Remove redundant empty line
* RCS1037 - Remove trailing white\-space
* RCS1038 - Remove empty statement
* RCS1039 - Remove empty attribute argument list
* RCS1040 - Remove empty else clause
* RCS1041 - Remove empty initializer
* RCS1042 - Remove enum default underlying type
* RCS1043 - Remove partial modifier from type with a single part
* RCS1044 - Remove original exception from throw statement
* RCS1045 - Rename private field according to camel case with underscore
* RCS1046 - Asynchronous method name should end with 'Async'
* RCS1047 - Non\-asynchronous method name should not end with 'Async'
* RCS1048 - Use lambda expression instead of anonymous method
* RCS1049 - Simplify boolean comparison
* RCS1050 - Add constructor argument list
* RCS1051 - Parenthesize condition in conditional expression
* RCS1052 - Declare each attribute separately
* RCS1054 - Merge local declaration with return statement
* RCS1055 - Avoid semicolon at the end of declaration
* RCS1056 - Avoid usage of using alias directive
* RCS1057 - Add empty line between declarations
* RCS1058 - Use compound assignment
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
* RCS1075 - Avoid empty catch clause that catches System\.Exception
* RCS1076 - Format declaration braces
* RCS1077 - Simplify LINQ method chain
* RCS1078 - Use "" instead of string\.Empty
* RCS1079 - Throwing of new NotImplementedException
* RCS1080 - Use 'Count/Length' property instead of 'Any' method
* RCS1081 - Split variable declaration
* RCS1082 - Use 'Count/Length' property instead of 'Count' method
* RCS1083 - Use 'Any' method instead of 'Count' method
* RCS1084 - Use coalesce expression instead of conditional expression
* RCS1085 - Use auto\-implemented property instead of expanded property
* RCS1086 - Use linefeed as newline
* RCS1087 - Use carriage return \+ linefeed as newline
* RCS1088 - Use space\(s\) instead of tab
* RCS1089 - Use postfix unary operator instead of assignment
* RCS1090 - Call 'ConfigureAwait\(false\)'
* RCS1091 - Remove empty region
* RCS1092 - Add empty line after last statement in do statement
* RCS1093 - Remove file with no code
* RCS1094 - Declare using directive on top level
* RCS1095 - Use C\# 6\.0 dictionary initializer
* RCS1096 - Use bitwise operation instead of calling 'HasFlag'
* RCS1097 - Remove redundant 'ToString' call
* RCS1098 - Avoid 'null' on the left side of a binary expression
* RCS1099 - Default label should be last label in switch section
* RCS1100 - Format documentation summary on a single line
* RCS1101 - Format documentation summary on multiple lines
* RCS1102 - Mark class as static
* RCS1103 - Replace if statement with assignment
* RCS1104 - Simplify conditional expression
* RCS1105 - Merge interpolation into interpolated string
* RCS1106 - Remove empty destructor
* RCS1107 - Remove redundant 'ToCharArray' call
* RCS1108 - Add static modifier to all partial class declarations
* RCS1109 - Use 'Cast' method instead of 'Select' method
* RCS1110 - Declare type inside namespace
* RCS1111 - Add braces to switch section with multiple statements
* RCS1112 - Combine 'Enumerable\.Where' method chain
* RCS1113 - Use 'string\.IsNullOrEmpty' method
* RCS1114 - Remove redundant delegate creation
* RCS1115 - Replace yield/return statement with expression statement
* RCS1116 - Add break statement to switch section
* RCS1117 - Add return statement that returns default value
* RCS1118 - Mark local variable as const
* RCS1119 - Call 'Find' method instead of 'FirstOrDefault' method
* RCS1120 - Use \[\] instead of calling 'ElementAt'
* RCS1121 - Use \[\] instead of calling 'First'
* RCS1122 - Add missing semicolon
* RCS1123 - Add parentheses according to operator precedence
* RCS1124 - Inline local variable
* RCS1125 - Mark member as static
* RCS1126 - Avoid embedded statement in if\-else
* RCS1127 - Merge local declaration with initialization
* RCS1128 - Use coalesce expression
* RCS1129 - Remove redundant field initalization
* RCS1130 - Bitwise operation on enum without Flags attribute
* RCS1131 - Replace return with yield return
* RCS1132 - Remove redundant overriding member
* RCS1133 - Remove redundant Dispose/Close call
* RCS1134 - Remove redundant continue statement
* RCS1135 - Declare enum member with zero value \(when enum has FlagsAttribute\)
* RCS1136 - Merge switch sections with equivalent content
* RCS1137 - Add documentation comment to publicly visible type or member
* RCS1138 - Add summary to documentation comment
* RCS1139 - Add summary element to documentation comment
* RCS1140 - Add exception to documentation comment
* RCS1141 - Add parameter to documentation comment
* RCS1142 - Add type parameter to documentation comment
* RCS1143 - Simplify coalesce expression
* RCS1144 - Mark containing class as abstract
* RCS1145 - Remove redundant 'as' operator
* RCS1146 - Use conditional access
* RCS1147 - Remove inapplicable modifier
* RCS1148 - Remove unreachable code
* RCS1149 - Remove implementation from abstract member
* RCS1150 - Call string\.Concat instead of string\.Join
* RCS1151 - Remove redundant cast
* RCS1152 - Member type must match overriden member type
* RCS1153 - Add empty line after closing brace
* RCS1154 - Sort enum members
* RCS1155 - Use StringComparison when comparing strings
* RCS1156 - Use string\.Length instead of comparison with empty string
* RCS1157 - Composite enum value contains undefined flag
* RCS1158 - Avoid static members in generic types
* RCS1159 - Use EventHandler\<T\>
* RCS1160 - Abstract type should not have public constructors
* RCS1161 - Enum member should declare explicit value
* RCS1162 - Avoid chain of assignments
* RCS1163 - Unused parameter
* RCS1164 - Unused type parameter
* RCS1165 - Unconstrained type parameter checked for null
* RCS1166 - Value type checked for null
* RCS1167 - Overriding member cannot change access modifiers
* RCS1168 - Parameter name differs from base name
* RCS1169 - Mark field as read\-only
* RCS1170 - Use read\-only auto\-implemented property

### List of Refactorings

* [Add boolean comparison](source/Refactorings/Refactorings.md#add-boolean-comparison)
* [Add braces](source/Refactorings/Refactorings.md#add-braces)
* [Add braces to if\-else](source/Refactorings/Refactorings.md#add-braces-to-if-else)
* [Add braces to switch section](source/Refactorings/Refactorings.md#add-braces-to-switch-section)
* [Add braces to switch sections](source/Refactorings/Refactorings.md#add-braces-to-switch-sections)
* [Add cast expression](source/Refactorings/Refactorings.md#add-cast-expression)
* [Add default value to parameter](source/Refactorings/Refactorings.md#add-default-value-to-parameter)
* [Add default value to return statement](source/Refactorings/Refactorings.md#add-default-value-to-return-statement)
* [Add exception to documentation comment](source/Refactorings/Refactorings.md#add-exception-to-documentation-comment)
* [Add identifier to variable declaration](source/Refactorings/Refactorings.md#add-identifier-to-variable-declaration)
* [Add parameter name to argument](source/Refactorings/Refactorings.md#add-parameter-name-to-argument)
* [Add parameter name to parameter](source/Refactorings/Refactorings.md#add-parameter-name-to-parameter)
* [Add using directive](source/Refactorings/Refactorings.md#add-using-directive)
* [Add using static directive](source/Refactorings/Refactorings.md#add-using-static-directive)
* [Call 'ConfigureAwait\(false\)'](source/Refactorings/Refactorings.md#call-configureawaitfalse)
* [Call extension method as instance method](source/Refactorings/Refactorings.md#call-extension-method-as-instance-method)
* [Call 'To\.\.\.' method \(ToString, ToArray, ToList\)](source/Refactorings/Refactorings.md#call-to-method-tostring-toarray-tolist)
* [Change explicit type to 'var'](source/Refactorings/Refactorings.md#change-explicit-type-to-var)
* [Change method return type to 'void'](source/Refactorings/Refactorings.md#change-method-return-type-to-void)
* [Change method/property/indexer type according to return expression](source/Refactorings/Refactorings.md#change-methodpropertyindexer-type-according-to-return-expression)
* [Change method/property/indexer type according to yield return expression](source/Refactorings/Refactorings.md#change-methodpropertyindexer-type-according-to-yield-return-expression)
* [Change type according to expression](source/Refactorings/Refactorings.md#change-type-according-to-expression)
* [Change 'var' to explicit type](source/Refactorings/Refactorings.md#change-var-to-explicit-type)
* [Check expression for null](source/Refactorings/Refactorings.md#check-expression-for-null)
* [Check parameter for null](source/Refactorings/Refactorings.md#check-parameter-for-null)
* [Collapse to initalizer](source/Refactorings/Refactorings.md#collapse-to-initalizer)
* [Comment out member](source/Refactorings/Refactorings.md#comment-out-member)
* [Comment out statement](source/Refactorings/Refactorings.md#comment-out-statement)
* [Copy documentation comment from base member](source/Refactorings/Refactorings.md#copy-documentation-comment-from-base-member)
* [Duplicate argument](source/Refactorings/Refactorings.md#duplicate-argument)
* [Duplicate member](source/Refactorings/Refactorings.md#duplicate-member)
* [Duplicate parameter](source/Refactorings/Refactorings.md#duplicate-parameter)
* [Duplicate statement](source/Refactorings/Refactorings.md#duplicate-statement)
* [Expand assignment expression](source/Refactorings/Refactorings.md#expand-assignment-expression)
* [Expand coalesce expression](source/Refactorings/Refactorings.md#expand-coalesce-expression)
* [Expand event](source/Refactorings/Refactorings.md#expand-event)
* [Expand expression body](source/Refactorings/Refactorings.md#expand-expression-body)
* [Expand initializer](source/Refactorings/Refactorings.md#expand-initializer)
* [Expand lambda expression body](source/Refactorings/Refactorings.md#expand-lambda-expression-body)
* [Expand property](source/Refactorings/Refactorings.md#expand-property)
* [Expand property and add backing field](source/Refactorings/Refactorings.md#expand-property-and-add-backing-field)
* [Extract declaration from using statement](source/Refactorings/Refactorings.md#extract-declaration-from-using-statement)
* [Extract expression from condition](source/Refactorings/Refactorings.md#extract-expression-from-condition)
* [Extract generic type](source/Refactorings/Refactorings.md#extract-generic-type)
* [Extract statement\(s\)](source/Refactorings/Refactorings.md#extract-statements)
* [Extract type declaration to a new file](source/Refactorings/Refactorings.md#extract-type-declaration-to-a-new-file)
* [Format accessor braces](source/Refactorings/Refactorings.md#format-accessor-braces)
* [Format argument list](source/Refactorings/Refactorings.md#format-argument-list)
* [Format binary expression](source/Refactorings/Refactorings.md#format-binary-expression)
* [Format conditional expression](source/Refactorings/Refactorings.md#format-conditional-expression)
* [Format expression chain](source/Refactorings/Refactorings.md#format-expression-chain)
* [Format initializer](source/Refactorings/Refactorings.md#format-initializer)
* [Format parameter list](source/Refactorings/Refactorings.md#format-parameter-list)
* [Generate base constructors](source/Refactorings/Refactorings.md#generate-base-constructors)
* [Generate combined enum member](source/Refactorings/Refactorings.md#generate-combined-enum-member)
* [Generate enum member](source/Refactorings/Refactorings.md#generate-enum-member)
* [Generate enum values](source/Refactorings/Refactorings.md#generate-enum-values)
* [Generate event invoking method](source/Refactorings/Refactorings.md#generate-event-invoking-method)
* [Generate switch sections](source/Refactorings/Refactorings.md#generate-switch-sections)
* [Initialize local with default value](source/Refactorings/Refactorings.md#initialize-local-with-default-value)
* [Inline alias expression](source/Refactorings/Refactorings.md#inline-alias-expression)
* [Inline method](source/Refactorings/Refactorings.md#inline-method)
* [Insert string interpolation](source/Refactorings/Refactorings.md#insert-string-interpolation)
* [Introduce and initialize field](source/Refactorings/Refactorings.md#introduce-and-initialize-field)
* [Introduce and initialize property](source/Refactorings/Refactorings.md#introduce-and-initialize-property)
* [Introduce constructor](source/Refactorings/Refactorings.md#introduce-constructor)
* [Introduce field to lock on](source/Refactorings/Refactorings.md#introduce-field-to-lock-on)
* [Introduce local from statement that returns value](source/Refactorings/Refactorings.md#introduce-local-from-statement-that-returns-value)
* [Make member abstract](source/Refactorings/Refactorings.md#make-member-abstract)
* [Make member virtual](source/Refactorings/Refactorings.md#make-member-virtual)
* [Mark containing class as abstract](source/Refactorings/Refactorings.md#mark-containing-class-as-abstract)
* [Mark member as static](source/Refactorings/Refactorings.md#mark-member-as-static)
* [Merge assignment expression with return statement](source/Refactorings/Refactorings.md#merge-assignment-expression-with-return-statement)
* [Merge attributes](source/Refactorings/Refactorings.md#merge-attributes)
* [Merge if statements](source/Refactorings/Refactorings.md#merge-if-statements)
* [Merge interpolation into interpolated string](source/Refactorings/Refactorings.md#merge-interpolation-into-interpolated-string)
* [Merge local declarations](source/Refactorings/Refactorings.md#merge-local-declarations)
* [Merge string expressions](source/Refactorings/Refactorings.md#merge-string-expressions)
* [Negate binary expression](source/Refactorings/Refactorings.md#negate-binary-expression)
* [Negate boolean literal](source/Refactorings/Refactorings.md#negate-boolean-literal)
* [Negate is expression](source/Refactorings/Refactorings.md#negate-is-expression)
* [Negate operator](source/Refactorings/Refactorings.md#negate-operator)
* [Notify property changed](source/Refactorings/Refactorings.md#notify-property-changed)
* [Parenthesize expression](source/Refactorings/Refactorings.md#parenthesize-expression)
* [Promote local to parameter](source/Refactorings/Refactorings.md#promote-local-to-parameter)
* [Remove all comments](source/Refactorings/Refactorings.md#remove-all-comments)
* [Remove all comments \(except documentation comments\)](source/Refactorings/Refactorings.md#remove-all-comments-except-documentation-comments)
* [Remove all documentation comments](source/Refactorings/Refactorings.md#remove-all-documentation-comments)
* [Remove all member declarations](source/Refactorings/Refactorings.md#remove-all-member-declarations)
* [Remove all preprocessor directives](source/Refactorings/Refactorings.md#remove-all-preprocessor-directives)
* [Remove all region directives](source/Refactorings/Refactorings.md#remove-all-region-directives)
* [Remove all statements](source/Refactorings/Refactorings.md#remove-all-statements)
* [Remove all switch sections](source/Refactorings/Refactorings.md#remove-all-switch-sections)
* [Remove braces](source/Refactorings/Refactorings.md#remove-braces)
* [Remove braces from if\-else](source/Refactorings/Refactorings.md#remove-braces-from-if-else)
* [Remove braces from switch section](source/Refactorings/Refactorings.md#remove-braces-from-switch-section)
* [Remove braces from switch sections](source/Refactorings/Refactorings.md#remove-braces-from-switch-sections)
* [Remove comment](source/Refactorings/Refactorings.md#remove-comment)
* [Remove condition from last else clause](source/Refactorings/Refactorings.md#remove-condition-from-last-else-clause)
* [Remove directive and related directives](source/Refactorings/Refactorings.md#remove-directive-and-related-directives)
* [Remove empty lines](source/Refactorings/Refactorings.md#remove-empty-lines)
* [Remove interpolation](source/Refactorings/Refactorings.md#remove-interpolation)
* [Remove member](source/Refactorings/Refactorings.md#remove-member)
* [Remove member declarations above/below](source/Refactorings/Refactorings.md#remove-member-declarations-abovebelow)
* [Remove parameter name from argument](source/Refactorings/Refactorings.md#remove-parameter-name-from-argument)
* [Remove parentheses](source/Refactorings/Refactorings.md#remove-parentheses)
* [Remove property initializer](source/Refactorings/Refactorings.md#remove-property-initializer)
* [Remove region](source/Refactorings/Refactorings.md#remove-region)
* [Remove statement](source/Refactorings/Refactorings.md#remove-statement)
* [Remove statements from switch sections](source/Refactorings/Refactorings.md#remove-statements-from-switch-sections)
* [Rename backing field according to property name](source/Refactorings/Refactorings.md#rename-backing-field-according-to-property-name)
* [Rename identifier according to type name](source/Refactorings/Refactorings.md#rename-identifier-according-to-type-name)
* [Rename method according to type name](source/Refactorings/Refactorings.md#rename-method-according-to-type-name)
* [Rename parameter according to its type name](source/Refactorings/Refactorings.md#rename-parameter-according-to-its-type-name)
* [Rename property according to type name](source/Refactorings/Refactorings.md#rename-property-according-to-type-name)
* [Replace Any with All \(or All with Any\)](source/Refactorings/Refactorings.md#replace-any-with-all-or-all-with-any)
* [Replace as expression with cast expression](source/Refactorings/Refactorings.md#replace-as-expression-with-cast-expression)
* [Replace cast expression with as expression](source/Refactorings/Refactorings.md#replace-cast-expression-with-as-expression)
* [Replace conditional expression with expression](source/Refactorings/Refactorings.md#replace-conditional-expression-with-expression)
* [Replace conditional expression with if\-else](source/Refactorings/Refactorings.md#replace-conditional-expression-with-if-else)
* [Replace constant with field](source/Refactorings/Refactorings.md#replace-constant-with-field)
* [Replace Count property with Length property \(or Length with Count\)](source/Refactorings/Refactorings.md#replace-count-property-with-length-property-or-length-with-count)
* [Replace do statement with while statement](source/Refactorings/Refactorings.md#replace-do-statement-with-while-statement)
* [Replace equals expression with String\.Equals](source/Refactorings/Refactorings.md#replace-equals-expression-with-stringequals)
* [Replace equals expression with String\.IsNullOrEmpty](source/Refactorings/Refactorings.md#replace-equals-expression-with-stringisnullorempty)
* [Replace equals expression with String\.IsNullOrWhiteSpace](source/Refactorings/Refactorings.md#replace-equals-expression-with-stringisnullorwhitespace)
* [Replace expression with constant value](source/Refactorings/Refactorings.md#replace-expression-with-constant-value)
* [Replace field with constant](source/Refactorings/Refactorings.md#replace-field-with-constant)
* [Replace for statement with foreach statement](source/Refactorings/Refactorings.md#replace-for-statement-with-foreach-statement)
* [Replace for statement with while statement](source/Refactorings/Refactorings.md#replace-for-statement-with-while-statement)
* [Replace foreach statement with for statement](source/Refactorings/Refactorings.md#replace-foreach-statement-with-for-statement)
* [Replace if\-else with switch statement](source/Refactorings/Refactorings.md#replace-if-else-with-switch-statement)
* [Replace increment operator with decrement operator](source/Refactorings/Refactorings.md#replace-increment-operator-with-decrement-operator)
* [Replace interpolated string with interpolation expression](source/Refactorings/Refactorings.md#replace-interpolated-string-with-interpolation-expression)
* [Replace interpolated string with string literal](source/Refactorings/Refactorings.md#replace-interpolated-string-with-string-literal)
* [Replace method group with lambda](source/Refactorings/Refactorings.md#replace-method-group-with-lambda)
* [Replace method with property](source/Refactorings/Refactorings.md#replace-method-with-property)
* [Replace null literal expression with default expression](source/Refactorings/Refactorings.md#replace-null-literal-expression-with-default-expression)
* [Replace prefix operator to postfix operator](source/Refactorings/Refactorings.md#replace-prefix-operator-to-postfix-operator)
* [Replace property with method](source/Refactorings/Refactorings.md#replace-property-with-method)
* [Replace regular string literal with verbatim string literal](source/Refactorings/Refactorings.md#replace-regular-string-literal-with-verbatim-string-literal)
* [Replace statement with if statement](source/Refactorings/Refactorings.md#replace-statement-with-if-statement)
* [Replace string literal with character literal](source/Refactorings/Refactorings.md#replace-string-literal-with-character-literal)
* [Replace String\.Contains with String\.IndexOf](source/Refactorings/Refactorings.md#replace-stringcontains-with-stringindexof)
* [Replace String\.Format with interpolated string](source/Refactorings/Refactorings.md#replace-stringformat-with-interpolated-string)
* [Replace switch statement with if\-else](source/Refactorings/Refactorings.md#replace-switch-statement-with-if-else)
* [Replace verbatim string literal with regular string literal](source/Refactorings/Refactorings.md#replace-verbatim-string-literal-with-regular-string-literal)
* [Replace verbatim string literal with regular string literals](source/Refactorings/Refactorings.md#replace-verbatim-string-literal-with-regular-string-literals)
* [Replace while statement with do statement](source/Refactorings/Refactorings.md#replace-while-statement-with-do-statement)
* [Replace while statement with for statement](source/Refactorings/Refactorings.md#replace-while-statement-with-for-statement)
* [Reverse for loop](source/Refactorings/Refactorings.md#reverse-for-loop)
* [Simplify if](source/Refactorings/Refactorings.md#simplify-if)
* [Simplify lambda expression](source/Refactorings/Refactorings.md#simplify-lambda-expression)
* [Sort member declarations](source/Refactorings/Refactorings.md#sort-member-declarations)
* [Split attributes](source/Refactorings/Refactorings.md#split-attributes)
* [Split switch labels](source/Refactorings/Refactorings.md#split-switch-labels)
* [Split variable declaration](source/Refactorings/Refactorings.md#split-variable-declaration)
* [Swap expressions in binary expression](source/Refactorings/Refactorings.md#swap-expressions-in-binary-expression)
* [Swap expressions in conditional expression](source/Refactorings/Refactorings.md#swap-expressions-in-conditional-expression)
* [Swap member declarations](source/Refactorings/Refactorings.md#swap-member-declarations)
* [Swap statements in if\-else](source/Refactorings/Refactorings.md#swap-statements-in-if-else)
* [Uncomment](source/Refactorings/Refactorings.md#uncomment)
* [Use "" instead of String\.Empty](source/Refactorings/Refactorings.md#use--instead-of-stringempty)
* [Use bitwise operation instead of calling 'HasFlag'](source/Refactorings/Refactorings.md#use-bitwise-operation-instead-of-calling-hasflag)
* [Use coalesce expression instead of if](source/Refactorings/Refactorings.md#use-coalesce-expression-instead-of-if)
* [Use conditional expression instead of if](source/Refactorings/Refactorings.md#use-conditional-expression-instead-of-if)
* [Use element access instead of 'First/Last'ElementAt' method](source/Refactorings/Refactorings.md#use-element-access-instead-of-firstlastelementat-method)
* [Use expression\-bodied member](source/Refactorings/Refactorings.md#use-expression-bodied-member)
* [Use lambda expression instead of anonymous method](source/Refactorings/Refactorings.md#use-lambda-expression-instead-of-anonymous-method)
* [Use String\.Empty instead of ""](source/Refactorings/Refactorings.md#use-stringempty-instead-of-)
* [Wrap in \#if directive](source/Refactorings/Refactorings.md#wrap-in-if-directive)
* [Wrap in condition](source/Refactorings/Refactorings.md#wrap-in-condition)
* [Wrap in region](source/Refactorings/Refactorings.md#wrap-in-region)
* [Wrap in try\-catch](source/Refactorings/Refactorings.md#wrap-in-try-catch)
* [Wrap in using statement](source/Refactorings/Refactorings.md#wrap-in-using-statement)
