# Roslynator Configuration

Use EditorConfig file to configure analyzers, refactoring and compiler diagnostic fixes.

```editorconfig
# Set severity for all analyzers that are enabled by default (https://docs.microsoft.com/en-us/visualstudio/code-quality/use-roslyn-analyzers?view=vs-2022#set-rule-severity-of-multiple-analyzer-rules-at-once-in-an-editorconfig-file)
dotnet_analyzer_diagnostic.category-roslynator.severity = default|none|silent|suggestion|warning|error

# Enable/disable all analyzers by default
# NOTE: This option can be used only in .roslynatorconfig file
roslynator_analyzers.enabled_by_default = true|false

# Set severity for a specific analyzer
dotnet_diagnostic.<ANALYZER_ID>.severity = default|none|silent|suggestion|warning|error

# Enable/disable all refactorings
roslynator_refactorings.enabled = true|false

# Enable/disable specific refactoring
roslynator_refactoring.<REFACTORING_NAME>.enabled = true|false

# Enable/disable all compiler diagnostic fixes
roslynator_compiler_diagnostic_fixes.enabled = true|false

# Enable/disable specific compiler diagnostic fix
roslynator_compiler_diagnostic_fix.<COMPILER_DIAGNOSTIC_ID>.enabled = true|false
```

## Required Options

Some analyzers require option to be set. For this purpose there is special analyzer ROS0003 that reports a diagnostic
if an analyzer is enabled but required option is not set. ROS0003 is disabled by default.

## Default Configuration

If you want to configure Roslynator on a user-wide basis you have to use Roslynator config file.
Default configuration file can be used with extension for Visual Studio or VS code.

### Format of Default Configuration File

Format of the file is same as format of [global AnalyzerConfig](https://docs.microsoft.com/en-us/dotnet/fundamentals/code-analysis/configuration-files#global-analyzerconfig).
It essentially means that file must contain top-level entry `is_global = true` and cannot contain section headers (such as `[*.cs]`).

### Location of Default Configuration File

Configuration file is located at `%LOCALAPPDATA%/JosefPihrt/Roslynator/.roslynatorconfig`.
Location of `%LOCALAPPDATA%` depends on the operating system:

| OS | Path |
| -------- | ------- |
| Windows | `C:/Users/<USERNAME>/AppData/Local/.roslynatorconfig` |
| Linux | `/home/<USERNAME>/.local/share/.roslynatorconfig` |
| OSX | `/Users/<USERNAME>/.local/share/.roslynatorconfig` |

Default configuration is loaded once when IDE starts. Therefore, it may be necessary to restart IDE for changes to take effect.

# Full List of Options

```editorconfig
# Options

roslynator_accessibility_modifiers = explicit|implicit
# Applicable to: rcs1018

roslynator_accessor_braces_style = multi_line|single_line_when_expression_is_on_single_line
# Default: multi_line
# Applicable to: rcs0020

roslynator_array_creation_type_style = explicit|implicit|implicit_when_type_is_obvious
# Applicable to: rcs1014

roslynator_arrow_token_new_line = after|before
# Applicable to: rcs0032

roslynator_binary_operator_new_line = after|before
# Applicable to: rcs0027

roslynator_blank_line_between_closing_brace_and_switch_section = true|false
# Applicable to: rcs0014, rcs1036

roslynator_blank_line_between_single_line_accessors = true|false
# Applicable to: rcs0011

roslynator_blank_line_between_using_directives = never|separate_groups
# Applicable to: rcs0015

roslynator_block_braces_style = multi_line|single_line_when_empty
# Default: multi_line
# Applicable to: rcs0021

roslynator_body_style = block|expression
# Applicable to: rcs1016

roslynator_conditional_operator_condition_parentheses_style = include|omit|omit_when_condition_is_single_token
# Applicable to: rcs1051

roslynator_conditional_operator_new_line = after|before
# Applicable to: rcs0028

roslynator_configure_await = true|false
# Applicable to: rcs1090

roslynator_empty_string_style = field|literal
# Applicable to: rcs1078

roslynator_enum_has_flag_style = method|operator
# Applicable to: rcs1096

roslynator_equals_token_new_line = after|before
# Applicable to: rcs0052

roslynator_max_line_length = <NUM>
# Default: 140
# Applicable to: rcs0056

roslynator_new_line_at_end_of_file = true|false
# Applicable to: rcs0058

roslynator_new_line_before_while_in_do_statement = true|false
# Applicable to: rcs0051

roslynator_null_conditional_operator_new_line = after|before
# Applicable to: rcs0059

roslynator_null_check_style = equality_operator|pattern_matching
# Applicable to: rcs1248

roslynator_object_creation_parentheses_style = include|omit
# Applicable to: rcs1050

roslynator_object_creation_type_style = explicit|implicit|implicit_when_type_is_obvious
# Applicable to: rcs1250

roslynator_prefix_field_identifier_with_underscore = true|false

roslynator_suppress_unity_script_methods = true|false
# Applicable to: rcs1213

roslynator_use_anonymous_function_or_method_group = anonymous_function|method_group
# Applicable to: rcs1207

roslynator_use_block_body_when_declaration_spans_over_multiple_lines = true|false
# Applicable to: rcs1016

roslynator_use_block_body_when_expression_spans_over_multiple_lines = true|false
# Applicable to: rcs1016

roslynator_use_var_instead_of_implicit_object_creation = true|false
# Applicable to: rcs1250


# Analyzers

# Add blank line after embedded statement
dotnet_diagnostic.rcs0001.severity = none

# Add blank line after #region
dotnet_diagnostic.rcs0002.severity = none

# Add blank line after using directive list
dotnet_diagnostic.rcs0003.severity = none

# Add blank line before #endregion
dotnet_diagnostic.rcs0005.severity = none

# Add blank line before using directive list
dotnet_diagnostic.rcs0006.severity = none

# Add blank line between accessors
dotnet_diagnostic.rcs0007.severity = none

# Add blank line between closing brace and next statement
dotnet_diagnostic.rcs0008.severity = none

# Add blank line between declaration and documentation comment
dotnet_diagnostic.rcs0009.severity = none

# Add blank line between declarations
dotnet_diagnostic.rcs0010.severity = none

# Add/remove blank line between single-line accessors
dotnet_diagnostic.rcs0011.severity = none
# Options: roslynator_blank_line_between_single_line_accessors

# Add blank line between single-line declarations
dotnet_diagnostic.rcs0012.severity = none

# Add blank line between single-line declarations of different kind
dotnet_diagnostic.rcs0013.severity = none

# Add blank line between switch sections
dotnet_diagnostic.rcs0014.severity = none
# Options: roslynator_blank_line_between_closing_brace_and_switch_section

# Add/remove blank line between using directives
dotnet_diagnostic.rcs0015.severity = none
# Options: roslynator_blank_line_between_using_directives

# Put attribute list on its own line
dotnet_diagnostic.rcs0016.severity = none

# Format accessor's braces on a single line or multiple lines
dotnet_diagnostic.rcs0020.severity = none
# Options: roslynator_accessor_braces_style

# Format block's braces on a single line or multiple lines
dotnet_diagnostic.rcs0021.severity = none
# Options: roslynator_block_braces_style

# Add new line after opening brace of empty block
dotnet_diagnostic.rcs0022.severity = none

# Format type declaration's braces
dotnet_diagnostic.rcs0023.severity = none

# Add new line after switch label
dotnet_diagnostic.rcs0024.severity = none

# Put full accessor on its own line
dotnet_diagnostic.rcs0025.severity = none

# Place new line after/before binary operator
dotnet_diagnostic.rcs0027.severity = none
# Options: roslynator_binary_operator_new_line

# Place new line after/before '?:' operator
dotnet_diagnostic.rcs0028.severity = none
# Options: roslynator_conditional_operator_new_line

# Put constructor initializer on its own line
dotnet_diagnostic.rcs0029.severity = none

# Add new line before embedded statement
dotnet_diagnostic.rcs0030.severity = none

# Add new line before enum member
dotnet_diagnostic.rcs0031.severity = none

# Place new line after/before arrow token
dotnet_diagnostic.rcs0032.severity = none
# Options: roslynator_arrow_token_new_line

# Add new line before statement
dotnet_diagnostic.rcs0033.severity = none

# Add new line before type parameter constraint
dotnet_diagnostic.rcs0034.severity = none

# Remove blank line between single-line declarations of same kind
dotnet_diagnostic.rcs0036.severity = none

# Remove blank line between using directives with same root namespace
dotnet_diagnostic.rcs0038.severity = none

# Remove new line before base list
dotnet_diagnostic.rcs0039.severity = none

# Remove new line between 'if' keyword and 'else' keyword
dotnet_diagnostic.rcs0041.severity = none

# Put auto-accessors on a single line
dotnet_diagnostic.rcs0042.severity = none

# Format accessor's braces on a single line when expression is on single line
dotnet_diagnostic.rcs0043.severity = none

# Use carriage return + linefeed as new line
dotnet_diagnostic.rcs0044.severity = none

# Use linefeed as new line
dotnet_diagnostic.rcs0045.severity = none

# Use spaces instead of tab
dotnet_diagnostic.rcs0046.severity = none

# [deprecated] Wrap and indent each node in list
dotnet_diagnostic.rcs0047.severity = none

# Put initializer on a single line
dotnet_diagnostic.rcs0048.severity = none

# Add blank line after top comment
dotnet_diagnostic.rcs0049.severity = none

# Add blank line before top declaration
dotnet_diagnostic.rcs0050.severity = none

# Add/remove new line before 'while' in 'do' statement
dotnet_diagnostic.rcs0051.severity = none
# Options: roslynator_new_line_before_while_in_do_statement

# Place new line after/before equals token
dotnet_diagnostic.rcs0052.severity = none
# Options: roslynator_equals_token_new_line

# Fix formatting of a list
dotnet_diagnostic.rcs0053.severity = none

# Fix formatting of a call chain
dotnet_diagnostic.rcs0054.severity = none

# Fix formatting of a binary expression chain
dotnet_diagnostic.rcs0055.severity = none

# A line is too long
dotnet_diagnostic.rcs0056.severity = none
# Options: roslynator_max_line_length

# Normalize whitespace at the beginning of a file
dotnet_diagnostic.rcs0057.severity = none

# Normalize whitespace at the end of a file
dotnet_diagnostic.rcs0058.severity = none
# Options: roslynator_new_line_at_end_of_file

# Place new line after/before null-conditional operator
dotnet_diagnostic.rcs0059.severity = none
# Options: roslynator_null_conditional_operator_new_line

# Add braces (when expression spans over multiple lines)
dotnet_diagnostic.rcs1001.severity = suggestion

# Remove braces
dotnet_diagnostic.rcs1002.severity = none

# Add braces to if-else (when expression spans over multiple lines)
dotnet_diagnostic.rcs1003.severity = suggestion

# Remove braces from if-else
dotnet_diagnostic.rcs1004.severity = none

# Simplify nested using statement
dotnet_diagnostic.rcs1005.severity = silent

# Merge 'else' with nested 'if'
dotnet_diagnostic.rcs1006.severity = silent

# Add braces
dotnet_diagnostic.rcs1007.severity = none

# Use explicit type instead of 'var' (when the type is not obvious)
dotnet_diagnostic.rcs1008.severity = none

# Use explicit type instead of 'var' (foreach variable)
dotnet_diagnostic.rcs1009.severity = none

# Use 'var' instead of explicit type (when the type is obvious)
dotnet_diagnostic.rcs1010.severity = none

# Use explicit type instead of 'var' (when the type is obvious)
dotnet_diagnostic.rcs1012.severity = none

# Use predefined type
dotnet_diagnostic.rcs1013.severity = none

# Use explicitly/implicitly typed array
dotnet_diagnostic.rcs1014.severity = none
# Options: roslynator_array_creation_type_style

# Use nameof operator
dotnet_diagnostic.rcs1015.severity = suggestion

# Use block body or expression body
dotnet_diagnostic.rcs1016.severity = none
# Options: roslynator_body_style, roslynator_use_block_body_when_declaration_spans_over_multiple_lines, roslynator_use_block_body_when_expression_spans_over_multiple_lines

# Add/remove accessibility modifiers
dotnet_diagnostic.rcs1018.severity = none
# Options: roslynator_accessibility_modifiers

# Order modifiers
dotnet_diagnostic.rcs1019.severity = none

# Simplify Nullable<T> to T?
dotnet_diagnostic.rcs1020.severity = suggestion

# Convert lambda expression body to expression body
dotnet_diagnostic.rcs1021.severity = suggestion

# Remove unnecessary braces
dotnet_diagnostic.rcs1031.severity = silent

# Remove redundant parentheses
dotnet_diagnostic.rcs1032.severity = suggestion

# Remove redundant boolean literal
dotnet_diagnostic.rcs1033.severity = suggestion

# Remove redundant 'sealed' modifier
dotnet_diagnostic.rcs1034.severity = silent

# Remove redundant comma in initializer
dotnet_diagnostic.rcs1035.severity = none

# Remove unnecessary blank line
dotnet_diagnostic.rcs1036.severity = suggestion
# Options: roslynator_blank_line_between_closing_brace_and_switch_section

# Remove trailing white-space
dotnet_diagnostic.rcs1037.severity = suggestion

# Remove empty statement
dotnet_diagnostic.rcs1038.severity = suggestion

# Remove argument list from attribute
dotnet_diagnostic.rcs1039.severity = silent

# Remove empty 'else' clause
dotnet_diagnostic.rcs1040.severity = silent

# Remove empty initializer
dotnet_diagnostic.rcs1041.severity = suggestion

# Remove enum default underlying type
dotnet_diagnostic.rcs1042.severity = silent

# Remove 'partial' modifier from type with a single part
dotnet_diagnostic.rcs1043.severity = silent

# Remove original exception from throw statement
dotnet_diagnostic.rcs1044.severity = warning

# Asynchronous method name should end with 'Async'
dotnet_diagnostic.rcs1046.severity = none

# Non-asynchronous method name should not end with 'Async'
dotnet_diagnostic.rcs1047.severity = suggestion

# Use lambda expression instead of anonymous method
dotnet_diagnostic.rcs1048.severity = suggestion

# Simplify boolean comparison
dotnet_diagnostic.rcs1049.severity = suggestion

# Include/omit parentheses when creating new object
dotnet_diagnostic.rcs1050.severity = none
# Options: roslynator_object_creation_parentheses_style

# Add/remove parentheses from condition in conditional operator
dotnet_diagnostic.rcs1051.severity = none
# Options: roslynator_conditional_operator_condition_parentheses_style

# Declare each attribute separately
dotnet_diagnostic.rcs1052.severity = none

# Avoid semicolon at the end of declaration
dotnet_diagnostic.rcs1055.severity = silent

# Avoid usage of using alias directive
dotnet_diagnostic.rcs1056.severity = none

# Use compound assignment
dotnet_diagnostic.rcs1058.severity = suggestion

# Avoid locking on publicly accessible instance
dotnet_diagnostic.rcs1059.severity = warning

# Declare each type in separate file
dotnet_diagnostic.rcs1060.severity = none

# Merge 'if' with nested 'if'
dotnet_diagnostic.rcs1061.severity = silent

# Avoid usage of do statement to create an infinite loop
dotnet_diagnostic.rcs1063.severity = suggestion

# Avoid usage of for statement to create an infinite loop
dotnet_diagnostic.rcs1064.severity = none

# Avoid usage of while statement to create an infinite loop
dotnet_diagnostic.rcs1065.severity = none

# Remove empty 'finally' clause
dotnet_diagnostic.rcs1066.severity = silent

# Simplify logical negation
dotnet_diagnostic.rcs1068.severity = suggestion

# Remove unnecessary case label
dotnet_diagnostic.rcs1069.severity = silent

# Remove redundant default switch section
dotnet_diagnostic.rcs1070.severity = silent

# Remove redundant base constructor call
dotnet_diagnostic.rcs1071.severity = silent

# Remove empty namespace declaration
dotnet_diagnostic.rcs1072.severity = suggestion

# Convert 'if' to 'return' statement
dotnet_diagnostic.rcs1073.severity = suggestion

# Remove redundant constructor
dotnet_diagnostic.rcs1074.severity = silent

# Avoid empty catch clause that catches System.Exception
dotnet_diagnostic.rcs1075.severity = warning

# Optimize LINQ method call
dotnet_diagnostic.rcs1077.severity = suggestion

# Use "" or 'string.Empty'
dotnet_diagnostic.rcs1078.severity = none
# Options: roslynator_empty_string_style

# Throwing of new NotImplementedException
dotnet_diagnostic.rcs1079.severity = none

# Use 'Count/Length' property instead of 'Any' method
dotnet_diagnostic.rcs1080.severity = suggestion

# Split variable declaration
dotnet_diagnostic.rcs1081.severity = none

# Use coalesce expression instead of conditional expression
dotnet_diagnostic.rcs1084.severity = suggestion

# Use auto-implemented property
dotnet_diagnostic.rcs1085.severity = suggestion

# Use --/++ operator instead of assignment
dotnet_diagnostic.rcs1089.severity = suggestion

# Add/remove 'ConfigureAwait(false)' call
dotnet_diagnostic.rcs1090.severity = none
# Options: roslynator_configure_await

# Remove empty region
dotnet_diagnostic.rcs1091.severity = silent

# Remove file with no code
dotnet_diagnostic.rcs1093.severity = suggestion

# Declare using directive on top level
dotnet_diagnostic.rcs1094.severity = none

# Use 'HasFlag' method or bitwise operator
dotnet_diagnostic.rcs1096.severity = none
# Options: roslynator_enum_has_flag_style

# Remove redundant 'ToString' call
dotnet_diagnostic.rcs1097.severity = suggestion

# Constant values should be placed on right side of comparisons
dotnet_diagnostic.rcs1098.severity = suggestion

# Default label should be the last label in a switch section
dotnet_diagnostic.rcs1099.severity = suggestion

# Format documentation summary on a single line
dotnet_diagnostic.rcs1100.severity = none

# Format documentation summary on multiple lines
dotnet_diagnostic.rcs1101.severity = none

# Make class static
dotnet_diagnostic.rcs1102.severity = warning

# Convert 'if' to assignment
dotnet_diagnostic.rcs1103.severity = suggestion

# Simplify conditional expression
dotnet_diagnostic.rcs1104.severity = suggestion

# Unnecessary interpolation
dotnet_diagnostic.rcs1105.severity = suggestion

# Remove empty destructor
dotnet_diagnostic.rcs1106.severity = suggestion

# Remove redundant 'ToCharArray' call
dotnet_diagnostic.rcs1107.severity = suggestion

# Add 'static' modifier to all partial class declarations
dotnet_diagnostic.rcs1108.severity = suggestion

# Declare type inside namespace
dotnet_diagnostic.rcs1110.severity = suggestion

# Add braces to switch section with multiple statements
dotnet_diagnostic.rcs1111.severity = none

# Combine 'Enumerable.Where' method chain
dotnet_diagnostic.rcs1112.severity = suggestion

# Use 'string.IsNullOrEmpty' method
dotnet_diagnostic.rcs1113.severity = suggestion

# Remove redundant delegate creation
dotnet_diagnostic.rcs1114.severity = suggestion

# Mark local variable as const
dotnet_diagnostic.rcs1118.severity = suggestion

# Add parentheses when necessary
dotnet_diagnostic.rcs1123.severity = suggestion

# Inline local variable
dotnet_diagnostic.rcs1124.severity = silent

# Add braces to if-else
dotnet_diagnostic.rcs1126.severity = none

# Use coalesce expression
dotnet_diagnostic.rcs1128.severity = suggestion

# Remove redundant field initialization
dotnet_diagnostic.rcs1129.severity = silent

# Bitwise operation on enum without Flags attribute
dotnet_diagnostic.rcs1130.severity = suggestion

# Remove redundant overriding member
dotnet_diagnostic.rcs1132.severity = suggestion

# Remove redundant Dispose/Close call
dotnet_diagnostic.rcs1133.severity = silent

# Remove redundant statement
dotnet_diagnostic.rcs1134.severity = silent

# Declare enum member with zero value (when enum has FlagsAttribute)
dotnet_diagnostic.rcs1135.severity = suggestion

# Merge switch sections with equivalent content
dotnet_diagnostic.rcs1136.severity = silent

# Add summary to documentation comment
dotnet_diagnostic.rcs1138.severity = warning

# Add summary element to documentation comment
dotnet_diagnostic.rcs1139.severity = warning

# Add exception to documentation comment
dotnet_diagnostic.rcs1140.severity = silent

# Add 'param' element to documentation comment
dotnet_diagnostic.rcs1141.severity = silent

# Add 'typeparam' element to documentation comment
dotnet_diagnostic.rcs1142.severity = silent

# Simplify coalesce expression
dotnet_diagnostic.rcs1143.severity = silent

# Remove redundant 'as' operator
dotnet_diagnostic.rcs1145.severity = silent

# Use conditional access
dotnet_diagnostic.rcs1146.severity = suggestion

# Remove redundant cast
dotnet_diagnostic.rcs1151.severity = silent

# Sort enum members
dotnet_diagnostic.rcs1154.severity = suggestion

# Use StringComparison when comparing strings
dotnet_diagnostic.rcs1155.severity = warning

# Use string.Length instead of comparison with empty string
dotnet_diagnostic.rcs1156.severity = suggestion

# Composite enum value contains undefined flag
dotnet_diagnostic.rcs1157.severity = suggestion

# Static member in generic type should use a type parameter
dotnet_diagnostic.rcs1158.severity = suggestion

# Use EventHandler<T>
dotnet_diagnostic.rcs1159.severity = suggestion

# Abstract type should not have public constructors
dotnet_diagnostic.rcs1160.severity = suggestion

# Enum should declare explicit values
dotnet_diagnostic.rcs1161.severity = silent

# Avoid chain of assignments
dotnet_diagnostic.rcs1162.severity = none

# Unused parameter
dotnet_diagnostic.rcs1163.severity = suggestion

# Unused type parameter
dotnet_diagnostic.rcs1164.severity = suggestion

# Unconstrained type parameter checked for null
dotnet_diagnostic.rcs1165.severity = silent

# Value type object is never equal to null
dotnet_diagnostic.rcs1166.severity = suggestion

# Parameter name differs from base name
dotnet_diagnostic.rcs1168.severity = silent

# Make field read-only
dotnet_diagnostic.rcs1169.severity = suggestion

# Use read-only auto-implemented property
dotnet_diagnostic.rcs1170.severity = suggestion

# Simplify lazy initialization
dotnet_diagnostic.rcs1171.severity = suggestion

# Use 'is' operator instead of 'as' operator
dotnet_diagnostic.rcs1172.severity = warning

# Use coalesce expression instead of 'if'
dotnet_diagnostic.rcs1173.severity = suggestion

# Remove redundant async/await
dotnet_diagnostic.rcs1174.severity = none

# Unused this parameter
dotnet_diagnostic.rcs1175.severity = suggestion

# Use 'var' instead of explicit type (when the type is not obvious)
dotnet_diagnostic.rcs1176.severity = none

# Use 'var' instead of explicit type (in foreach)
dotnet_diagnostic.rcs1177.severity = none

# Unnecessary assignment
dotnet_diagnostic.rcs1179.severity = suggestion

# Inline lazy initialization
dotnet_diagnostic.rcs1180.severity = suggestion

# Convert comment to documentation comment
dotnet_diagnostic.rcs1181.severity = silent

# Remove redundant base interface
dotnet_diagnostic.rcs1182.severity = silent

# Use Regex instance instead of static method
dotnet_diagnostic.rcs1186.severity = silent

# Use constant instead of field
dotnet_diagnostic.rcs1187.severity = suggestion

# Remove redundant auto-property initialization
dotnet_diagnostic.rcs1188.severity = silent

# Add or remove region name
dotnet_diagnostic.rcs1189.severity = silent

# Join string expressions
dotnet_diagnostic.rcs1190.severity = suggestion

# Declare enum value as combination of names
dotnet_diagnostic.rcs1191.severity = suggestion

# Unnecessary usage of verbatim string literal
dotnet_diagnostic.rcs1192.severity = suggestion

# Overriding member should not change 'params' modifier
dotnet_diagnostic.rcs1193.severity = warning

# Implement exception constructors
dotnet_diagnostic.rcs1194.severity = warning

# Use ^ operator
dotnet_diagnostic.rcs1195.severity = suggestion

# Call extension method as instance method
dotnet_diagnostic.rcs1196.severity = suggestion

# Optimize StringBuilder.Append/AppendLine call
dotnet_diagnostic.rcs1197.severity = suggestion

# Avoid unnecessary boxing of value type
dotnet_diagnostic.rcs1198.severity = none

# Unnecessary null check
dotnet_diagnostic.rcs1199.severity = suggestion

# Call 'Enumerable.ThenBy' instead of 'Enumerable.OrderBy'
dotnet_diagnostic.rcs1200.severity = suggestion

# Use method chaining
dotnet_diagnostic.rcs1201.severity = silent

# Avoid NullReferenceException
dotnet_diagnostic.rcs1202.severity = suggestion

# Use AttributeUsageAttribute
dotnet_diagnostic.rcs1203.severity = warning

# Use EventArgs.Empty
dotnet_diagnostic.rcs1204.severity = suggestion

# Order named arguments according to the order of parameters
dotnet_diagnostic.rcs1205.severity = suggestion

# Use conditional access instead of conditional expression
dotnet_diagnostic.rcs1206.severity = suggestion

# Use anonymous function or method group
dotnet_diagnostic.rcs1207.severity = none
# Options: roslynator_use_anonymous_function_or_method_group

# Reduce 'if' nesting
dotnet_diagnostic.rcs1208.severity = none

# Order type parameter constraints
dotnet_diagnostic.rcs1209.severity = suggestion

# Return completed task instead of returning null
dotnet_diagnostic.rcs1210.severity = warning

# Remove unnecessary 'else'
dotnet_diagnostic.rcs1211.severity = silent

# Remove redundant assignment
dotnet_diagnostic.rcs1212.severity = suggestion

# Remove unused member declaration
dotnet_diagnostic.rcs1213.severity = suggestion
# Options: roslynator_suppress_unity_script_methods

# Unnecessary interpolated string
dotnet_diagnostic.rcs1214.severity = suggestion

# Expression is always equal to true/false
dotnet_diagnostic.rcs1215.severity = warning

# Unnecessary unsafe context
dotnet_diagnostic.rcs1216.severity = suggestion

# Convert interpolated string to concatenation
dotnet_diagnostic.rcs1217.severity = silent

# Simplify code branching
dotnet_diagnostic.rcs1218.severity = suggestion

# Use pattern matching instead of combination of 'is' operator and cast operator
dotnet_diagnostic.rcs1220.severity = suggestion

# Use pattern matching instead of combination of 'as' operator and null check
dotnet_diagnostic.rcs1221.severity = suggestion

# Merge preprocessor directives
dotnet_diagnostic.rcs1222.severity = suggestion

# Mark publicly visible type with DebuggerDisplay attribute
dotnet_diagnostic.rcs1223.severity = none

# Make method an extension method
dotnet_diagnostic.rcs1224.severity = suggestion

# Make class sealed
dotnet_diagnostic.rcs1225.severity = suggestion

# Add paragraph to documentation comment
dotnet_diagnostic.rcs1226.severity = suggestion

# Validate arguments correctly
dotnet_diagnostic.rcs1227.severity = suggestion

# Unused element in documentation comment
dotnet_diagnostic.rcs1228.severity = silent

# Use async/await when necessary
dotnet_diagnostic.rcs1229.severity = suggestion

# Unnecessary explicit use of enumerator
dotnet_diagnostic.rcs1230.severity = suggestion

# Make parameter ref read-only
dotnet_diagnostic.rcs1231.severity = none

# Order elements in documentation comment
dotnet_diagnostic.rcs1232.severity = suggestion

# Use short-circuiting operator
dotnet_diagnostic.rcs1233.severity = suggestion

# Duplicate enum value
dotnet_diagnostic.rcs1234.severity = suggestion

# Optimize method call
dotnet_diagnostic.rcs1235.severity = suggestion

# Use exception filter
dotnet_diagnostic.rcs1236.severity = suggestion

# Use bit shift operator
dotnet_diagnostic.rcs1237.severity = silent

# Avoid nested ?: operators
dotnet_diagnostic.rcs1238.severity = silent

# Use 'for' statement instead of 'while' statement
dotnet_diagnostic.rcs1239.severity = suggestion

# Operator is unnecessary
dotnet_diagnostic.rcs1240.severity = suggestion

# Implement non-generic counterpart
dotnet_diagnostic.rcs1241.severity = silent

# Do not pass non-read-only struct by read-only reference
dotnet_diagnostic.rcs1242.severity = warning

# Duplicate word in a comment
dotnet_diagnostic.rcs1243.severity = suggestion

# Simplify 'default' expression
dotnet_diagnostic.rcs1244.severity = silent

# Use element access
dotnet_diagnostic.rcs1246.severity = suggestion

# Fix documentation comment tag
dotnet_diagnostic.rcs1247.severity = suggestion

# Normalize null check
dotnet_diagnostic.rcs1248.severity = none
# Options: roslynator_null_check_style

# Unnecessary null-forgiving operator
dotnet_diagnostic.rcs1249.severity = suggestion

# Use implicit/explicit object creation
dotnet_diagnostic.rcs1250.severity = none
# Options: roslynator_object_creation_type_style, roslynator_use_var_instead_of_implicit_object_creation

# Use pattern matching
dotnet_diagnostic.rcs9001.severity = silent

# Use property SyntaxNode.SpanStart
dotnet_diagnostic.rcs9002.severity = suggestion

# Unnecessary conditional access
dotnet_diagnostic.rcs9003.severity = suggestion

# Call 'Any' instead of accessing 'Count'
dotnet_diagnostic.rcs9004.severity = suggestion

# Unnecessary null check
dotnet_diagnostic.rcs9005.severity = suggestion

# Use element access
dotnet_diagnostic.rcs9006.severity = suggestion

# Use return value
dotnet_diagnostic.rcs9007.severity = warning

# Call 'Last' instead of using []
dotnet_diagnostic.rcs9008.severity = suggestion

# Unknown language name
dotnet_diagnostic.rcs9009.severity = warning

# Specify ExportCodeRefactoringProviderAttribute.Name
dotnet_diagnostic.rcs9010.severity = silent

# Specify ExportCodeFixProviderAttribute.Name
dotnet_diagnostic.rcs9011.severity = silent


# Refactorings

roslynator_refactoring.add_all_properties_to_initializer.enabled = true
roslynator_refactoring.add_argument_name.enabled = true
roslynator_refactoring.add_braces.enabled = true
roslynator_refactoring.add_braces_to_if_else.enabled = true
roslynator_refactoring.add_braces_to_switch_section.enabled = true
roslynator_refactoring.add_braces_to_switch_sections.enabled = true
roslynator_refactoring.add_default_value_to_parameter.enabled = true
roslynator_refactoring.add_empty_line_between_declarations.enabled = true
roslynator_refactoring.add_exception_element_to_documentation_comment.enabled = true
roslynator_refactoring.add_generic_parameter_to_declaration.enabled = true
roslynator_refactoring.add_member_to_interface.enabled = true
roslynator_refactoring.add_missing_cases_to_switch.enabled = true
roslynator_refactoring.add_parameter_to_interface_member.enabled = true
roslynator_refactoring.add_tag_to_documentation_comment.enabled = true
roslynator_refactoring.add_using_directive.enabled = true
roslynator_refactoring.add_using_static_directive.enabled = true
roslynator_refactoring.call_extension_method_as_instance_method.enabled = true
roslynator_refactoring.call_indexof_instead_of_contains.enabled = true
roslynator_refactoring.comment_out_member_declaration.enabled = true
roslynator_refactoring.comment_out_statement.enabled = true
roslynator_refactoring.convert_auto_property_to_full_property.enabled = true
roslynator_refactoring.convert_auto_property_to_full_property_without_backing_field.enabled = true
roslynator_refactoring.convert_block_body_to_expression_body.enabled = true
roslynator_refactoring.convert_comment_to_documentation_comment.enabled = true
roslynator_refactoring.convert_conditional_expression_to_if_else.enabled = true
roslynator_refactoring.convert_do_to_while.enabled = true
roslynator_refactoring.convert_expression_body_to_block_body.enabled = true
roslynator_refactoring.convert_for_to_foreach.enabled = true
roslynator_refactoring.convert_for_to_while.enabled = true
roslynator_refactoring.convert_foreach_to_for.enabled = true
roslynator_refactoring.convert_foreach_to_for_and_reverse_loop.enabled = false
roslynator_refactoring.convert_hasflag_call_to_bitwise_operation.enabled = true
roslynator_refactoring.convert_hexadecimal_literal_to_decimal_literal.enabled = true
roslynator_refactoring.convert_if_to_conditional_expression.enabled = true
roslynator_refactoring.convert_if_to_switch.enabled = true
roslynator_refactoring.convert_interpolated_string_to_concatenation.enabled = true
roslynator_refactoring.convert_interpolated_string_to_string_format.enabled = true
roslynator_refactoring.convert_interpolated_string_to_string_literal.enabled = true
roslynator_refactoring.convert_lambda_block_body_to_expression_body.enabled = true
roslynator_refactoring.convert_lambda_expression_body_to_block_body.enabled = true
roslynator_refactoring.convert_method_group_to_lambda.enabled = true
roslynator_refactoring.convert_regular_string_literal_to_verbatim_string_literal.enabled = true
roslynator_refactoring.convert_return_statement_to_if.enabled = true
roslynator_refactoring.convert_statements_to_if_else.enabled = true
roslynator_refactoring.convert_string_format_to_interpolated_string.enabled = true
roslynator_refactoring.convert_switch_expression_to_switch_statement.enabled = true
roslynator_refactoring.convert_switch_to_if.enabled = true
roslynator_refactoring.convert_verbatim_string_literal_to_regular_string_literal.enabled = true
roslynator_refactoring.convert_verbatim_string_literal_to_regular_string_literals.enabled = true
roslynator_refactoring.convert_while_to_do.enabled = true
roslynator_refactoring.convert_while_to_for.enabled = true
roslynator_refactoring.copy_argument.enabled = true
roslynator_refactoring.copy_documentation_comment_from_base_member.enabled = true
roslynator_refactoring.copy_member_declaration.enabled = true
roslynator_refactoring.copy_parameter.enabled = true
roslynator_refactoring.copy_statement.enabled = true
roslynator_refactoring.copy_switch_section.enabled = true
roslynator_refactoring.expand_coalesce_expression.enabled = true
roslynator_refactoring.expand_compound_assignment.enabled = true
roslynator_refactoring.expand_event_declaration.enabled = true
roslynator_refactoring.expand_initializer.enabled = false
roslynator_refactoring.expand_positional_constructor.enabled = true
roslynator_refactoring.extract_event_handler_method.enabled = true
roslynator_refactoring.extract_expression_from_condition.enabled = true
roslynator_refactoring.extract_type_declaration_to_new_file.enabled = true
roslynator_refactoring.generate_base_constructors.enabled = true
roslynator_refactoring.generate_combined_enum_member.enabled = true
roslynator_refactoring.generate_enum_member.enabled = true
roslynator_refactoring.generate_enum_values.enabled = true
roslynator_refactoring.generate_event_invoking_method.enabled = true
roslynator_refactoring.generate_property_for_debuggerdisplay_attribute.enabled = true
roslynator_refactoring.change_accessibility.enabled = true
roslynator_refactoring.change_method_return_type_to_void.enabled = true
roslynator_refactoring.change_type_according_to_expression.enabled = true
roslynator_refactoring.check_expression_for_null.enabled = true
roslynator_refactoring.check_parameter_for_null.enabled = true
roslynator_refactoring.implement_custom_enumerator.enabled = true
roslynator_refactoring.implement_iequatable.enabled = true
roslynator_refactoring.initialize_field_from_constructor.enabled = true
roslynator_refactoring.initialize_local_variable_with_default_value.enabled = true
roslynator_refactoring.inline_alias_expression.enabled = true
roslynator_refactoring.inline_constant.enabled = true
roslynator_refactoring.inline_constant_value.enabled = true
roslynator_refactoring.inline_method.enabled = true
roslynator_refactoring.inline_property.enabled = true
roslynator_refactoring.inline_using_static.enabled = true
roslynator_refactoring.insert_string_interpolation.enabled = true
roslynator_refactoring.introduce_and_initialize_field.enabled = true
roslynator_refactoring.introduce_and_initialize_property.enabled = true
roslynator_refactoring.introduce_constructor.enabled = false
roslynator_refactoring.introduce_field_to_lock_on.enabled = true
roslynator_refactoring.introduce_local_variable.enabled = true
roslynator_refactoring.invert_binary_expression.enabled = true
roslynator_refactoring.invert_boolean_literal.enabled = true
roslynator_refactoring.invert_conditional_expression.enabled = true
roslynator_refactoring.invert_if.enabled = true
roslynator_refactoring.invert_if_else.enabled = true
roslynator_refactoring.invert_is_expression.enabled = true
roslynator_refactoring.invert_linq_method_call.enabled = true
roslynator_refactoring.invert_operator.enabled = true
roslynator_refactoring.invert_prefix_or_postfix_unary_expression.enabled = true
roslynator_refactoring.join_string_expressions.enabled = true
roslynator_refactoring.make_member_abstract.enabled = true
roslynator_refactoring.make_member_virtual.enabled = true
roslynator_refactoring.merge_attributes.enabled = true
roslynator_refactoring.merge_if_statements.enabled = true
roslynator_refactoring.merge_if_with_parent_if.enabled = true
roslynator_refactoring.merge_local_declarations.enabled = true
roslynator_refactoring.merge_switch_sections.enabled = true
roslynator_refactoring.move_unsafe_context_to_containing_declaration.enabled = true
roslynator_refactoring.notify_when_property_changes.enabled = true
roslynator_refactoring.parenthesize_expression.enabled = true
roslynator_refactoring.promote_local_variable_to_parameter.enabled = true
roslynator_refactoring.remove_all_comments.enabled = true
roslynator_refactoring.remove_all_comments_except_documentation_comments.enabled = true
roslynator_refactoring.remove_all_documentation_comments.enabled = false
roslynator_refactoring.remove_all_member_declarations.enabled = true
roslynator_refactoring.remove_all_preprocessor_directives.enabled = true
roslynator_refactoring.remove_all_region_directives.enabled = true
roslynator_refactoring.remove_all_statements.enabled = true
roslynator_refactoring.remove_all_switch_sections.enabled = true
roslynator_refactoring.remove_argument_name.enabled = true
roslynator_refactoring.remove_async_await.enabled = true
roslynator_refactoring.remove_braces.enabled = true
roslynator_refactoring.remove_braces_from_if_else.enabled = true
roslynator_refactoring.remove_braces_from_switch_section.enabled = true
roslynator_refactoring.remove_braces_from_switch_sections.enabled = true
roslynator_refactoring.remove_comment.enabled = true
roslynator_refactoring.remove_condition_from_last_else.enabled = true
roslynator_refactoring.remove_containing_statement.enabled = true
roslynator_refactoring.remove_empty_lines.enabled = true
roslynator_refactoring.remove_enum_member_value.enabled = true
roslynator_refactoring.remove_instantiation_of_local_variable.enabled = true
roslynator_refactoring.remove_interpolation.enabled = true
roslynator_refactoring.remove_member_declaration.enabled = true
roslynator_refactoring.remove_member_declarations_above_or_below.enabled = true
roslynator_refactoring.remove_parentheses.enabled = true
roslynator_refactoring.remove_preprocessor_directive.enabled = true
roslynator_refactoring.remove_property_initializer.enabled = true
roslynator_refactoring.remove_region.enabled = true
roslynator_refactoring.remove_statement.enabled = true
roslynator_refactoring.remove_unnecessary_assignment.enabled = true
roslynator_refactoring.rename_identifier_according_to_type_name.enabled = true
roslynator_refactoring.rename_method_according_to_type_name.enabled = true
roslynator_refactoring.rename_parameter_according_to_type_name.enabled = true
roslynator_refactoring.rename_property_according_to_type_name.enabled = true
roslynator_refactoring.replace_as_expression_with_explicit_cast.enabled = true
roslynator_refactoring.replace_conditional_expression_with_true_or_false_branch.enabled = true
roslynator_refactoring.replace_equality_operator_with_string_equals.enabled = true
roslynator_refactoring.replace_equality_operator_with_string_isnullorempty.enabled = true
roslynator_refactoring.replace_equality_operator_with_string_isnullorwhitespace.enabled = true
roslynator_refactoring.replace_explicit_cast_with_as_expression.enabled = true
roslynator_refactoring.replace_interpolated_string_with_interpolation_expression.enabled = true
roslynator_refactoring.replace_method_with_property.enabled = false
roslynator_refactoring.replace_null_literal_with_default_expression.enabled = true
roslynator_refactoring.replace_prefix_operator_with_postfix_operator.enabled = true
roslynator_refactoring.replace_property_with_method.enabled = true
roslynator_refactoring.reverse_for_statement.enabled = true
roslynator_refactoring.simplify_if.enabled = true
roslynator_refactoring.sort_case_labels.enabled = true
roslynator_refactoring.sort_member_declarations.enabled = true
roslynator_refactoring.split_attributes.enabled = true
roslynator_refactoring.split_if.enabled = true
roslynator_refactoring.split_if_else.enabled = true
roslynator_refactoring.split_switch_labels.enabled = true
roslynator_refactoring.split_variable_declaration.enabled = true
roslynator_refactoring.swap_binary_operands.enabled = true
roslynator_refactoring.swap_member_declarations.enabled = true
roslynator_refactoring.sync_property_name_and_backing_field_name.enabled = true
roslynator_refactoring.uncomment_multiline_comment.enabled = true
roslynator_refactoring.uncomment_singleline_comment.enabled = true
roslynator_refactoring.use_coalesce_expression_instead_of_if.enabled = true
roslynator_refactoring.use_constant_instead_of_readonly_field.enabled = true
roslynator_refactoring.use_element_access_instead_of_linq_method.enabled = true
roslynator_refactoring.use_enumerator_explicitly.enabled = true
roslynator_refactoring.use_explicit_type.enabled = true
roslynator_refactoring.use_implicit_type.enabled = true
roslynator_refactoring.use_index_initializer.enabled = true
roslynator_refactoring.use_lambda_instead_of_anonymous_method.enabled = true
roslynator_refactoring.use_list_instead_of_yield.enabled = true
roslynator_refactoring.use_object_initializer.enabled = true
roslynator_refactoring.use_readonly_field_instead_of_constant.enabled = true
roslynator_refactoring.use_string_empty_instead_of_empty_string_literal.enabled = false
roslynator_refactoring.use_stringbuilder_instead_of_concatenation.enabled = true
roslynator_refactoring.wrap_arguments.enabled = true
roslynator_refactoring.wrap_binary_expression.enabled = true
roslynator_refactoring.wrap_call_chain.enabled = true
roslynator_refactoring.wrap_conditional_expression.enabled = true
roslynator_refactoring.wrap_constraint_clauses.enabled = true
roslynator_refactoring.wrap_initializer_expressions.enabled = true
roslynator_refactoring.wrap_lines_in_preprocessor_directive.enabled = true
roslynator_refactoring.wrap_lines_in_region.enabled = true
roslynator_refactoring.wrap_lines_in_try_catch.enabled = true
roslynator_refactoring.wrap_parameters.enabled = true
roslynator_refactoring.wrap_statements_in_condition.enabled = true
roslynator_refactoring.wrap_statements_in_using_statement.enabled = true

# Compiler diagnostic fixes

roslynator_compiler_diagnostic_fix.cs0019.enabled = true
roslynator_compiler_diagnostic_fix.cs0021.enabled = true
roslynator_compiler_diagnostic_fix.cs0023.enabled = true
roslynator_compiler_diagnostic_fix.cs0029.enabled = true
roslynator_compiler_diagnostic_fix.cs0030.enabled = true
roslynator_compiler_diagnostic_fix.cs0037.enabled = true
roslynator_compiler_diagnostic_fix.cs0069.enabled = true
roslynator_compiler_diagnostic_fix.cs0077.enabled = true
roslynator_compiler_diagnostic_fix.cs0080.enabled = true
roslynator_compiler_diagnostic_fix.cs0101.enabled = true
roslynator_compiler_diagnostic_fix.cs0102.enabled = true
roslynator_compiler_diagnostic_fix.cs0103.enabled = true
roslynator_compiler_diagnostic_fix.cs0106.enabled = true
roslynator_compiler_diagnostic_fix.cs0107.enabled = true
roslynator_compiler_diagnostic_fix.cs0108.enabled = true
roslynator_compiler_diagnostic_fix.cs0109.enabled = true
roslynator_compiler_diagnostic_fix.cs0112.enabled = true
roslynator_compiler_diagnostic_fix.cs0114.enabled = true
roslynator_compiler_diagnostic_fix.cs0115.enabled = true
roslynator_compiler_diagnostic_fix.cs0119.enabled = true
roslynator_compiler_diagnostic_fix.cs0120.enabled = true
roslynator_compiler_diagnostic_fix.cs0123.enabled = true
roslynator_compiler_diagnostic_fix.cs0126.enabled = true
roslynator_compiler_diagnostic_fix.cs0127.enabled = true
roslynator_compiler_diagnostic_fix.cs0128.enabled = true
roslynator_compiler_diagnostic_fix.cs0131.enabled = true
roslynator_compiler_diagnostic_fix.cs0132.enabled = true
roslynator_compiler_diagnostic_fix.cs0133.enabled = true
roslynator_compiler_diagnostic_fix.cs0136.enabled = true
roslynator_compiler_diagnostic_fix.cs0139.enabled = true
roslynator_compiler_diagnostic_fix.cs0152.enabled = true
roslynator_compiler_diagnostic_fix.cs0161.enabled = true
roslynator_compiler_diagnostic_fix.cs0162.enabled = true
roslynator_compiler_diagnostic_fix.cs0163.enabled = true
roslynator_compiler_diagnostic_fix.cs0164.enabled = true
roslynator_compiler_diagnostic_fix.cs0165.enabled = true
roslynator_compiler_diagnostic_fix.cs0168.enabled = true
roslynator_compiler_diagnostic_fix.cs0173.enabled = true
roslynator_compiler_diagnostic_fix.cs0177.enabled = true
roslynator_compiler_diagnostic_fix.cs0191.enabled = true
roslynator_compiler_diagnostic_fix.cs0192.enabled = true
roslynator_compiler_diagnostic_fix.cs0201.enabled = true
roslynator_compiler_diagnostic_fix.cs0214.enabled = true
roslynator_compiler_diagnostic_fix.cs0216.enabled = true
roslynator_compiler_diagnostic_fix.cs0219.enabled = true
roslynator_compiler_diagnostic_fix.cs0221.enabled = true
roslynator_compiler_diagnostic_fix.cs0225.enabled = true
roslynator_compiler_diagnostic_fix.cs0238.enabled = true
roslynator_compiler_diagnostic_fix.cs0246.enabled = true
roslynator_compiler_diagnostic_fix.cs0260.enabled = true
roslynator_compiler_diagnostic_fix.cs0262.enabled = true
roslynator_compiler_diagnostic_fix.cs0266.enabled = true
roslynator_compiler_diagnostic_fix.cs0267.enabled = true
roslynator_compiler_diagnostic_fix.cs0272.enabled = true
roslynator_compiler_diagnostic_fix.cs0275.enabled = true
roslynator_compiler_diagnostic_fix.cs0305.enabled = true
roslynator_compiler_diagnostic_fix.cs0401.enabled = true
roslynator_compiler_diagnostic_fix.cs0403.enabled = true
roslynator_compiler_diagnostic_fix.cs0405.enabled = true
roslynator_compiler_diagnostic_fix.cs0407.enabled = true
roslynator_compiler_diagnostic_fix.cs0409.enabled = true
roslynator_compiler_diagnostic_fix.cs0428.enabled = true
roslynator_compiler_diagnostic_fix.cs0441.enabled = true
roslynator_compiler_diagnostic_fix.cs0442.enabled = true
roslynator_compiler_diagnostic_fix.cs0449.enabled = true
roslynator_compiler_diagnostic_fix.cs0450.enabled = true
roslynator_compiler_diagnostic_fix.cs0451.enabled = true
roslynator_compiler_diagnostic_fix.cs0472.enabled = true
roslynator_compiler_diagnostic_fix.cs0500.enabled = true
roslynator_compiler_diagnostic_fix.cs0501.enabled = true
roslynator_compiler_diagnostic_fix.cs0507.enabled = true
roslynator_compiler_diagnostic_fix.cs0508.enabled = true
roslynator_compiler_diagnostic_fix.cs0513.enabled = true
roslynator_compiler_diagnostic_fix.cs0515.enabled = true
roslynator_compiler_diagnostic_fix.cs0524.enabled = true
roslynator_compiler_diagnostic_fix.cs0525.enabled = true
roslynator_compiler_diagnostic_fix.cs0527.enabled = true
roslynator_compiler_diagnostic_fix.cs0531.enabled = true
roslynator_compiler_diagnostic_fix.cs0539.enabled = true
roslynator_compiler_diagnostic_fix.cs0541.enabled = true
roslynator_compiler_diagnostic_fix.cs0549.enabled = true
roslynator_compiler_diagnostic_fix.cs0558.enabled = true
roslynator_compiler_diagnostic_fix.cs0567.enabled = true
roslynator_compiler_diagnostic_fix.cs0568.enabled = true
roslynator_compiler_diagnostic_fix.cs0573.enabled = true
roslynator_compiler_diagnostic_fix.cs0574.enabled = true
roslynator_compiler_diagnostic_fix.cs0575.enabled = true
roslynator_compiler_diagnostic_fix.cs0579.enabled = true
roslynator_compiler_diagnostic_fix.cs0592.enabled = true
roslynator_compiler_diagnostic_fix.cs0621.enabled = true
roslynator_compiler_diagnostic_fix.cs0628.enabled = true
roslynator_compiler_diagnostic_fix.cs0659.enabled = true
roslynator_compiler_diagnostic_fix.cs0660.enabled = true
roslynator_compiler_diagnostic_fix.cs0661.enabled = true
roslynator_compiler_diagnostic_fix.cs0678.enabled = true
roslynator_compiler_diagnostic_fix.cs0693.enabled = true
roslynator_compiler_diagnostic_fix.cs0708.enabled = true
roslynator_compiler_diagnostic_fix.cs0710.enabled = true
roslynator_compiler_diagnostic_fix.cs0713.enabled = true
roslynator_compiler_diagnostic_fix.cs0714.enabled = true
roslynator_compiler_diagnostic_fix.cs0718.enabled = true
roslynator_compiler_diagnostic_fix.cs0750.enabled = true
roslynator_compiler_diagnostic_fix.cs0751.enabled = true
roslynator_compiler_diagnostic_fix.cs0753.enabled = true
roslynator_compiler_diagnostic_fix.cs0756.enabled = true
roslynator_compiler_diagnostic_fix.cs0759.enabled = true
roslynator_compiler_diagnostic_fix.cs0766.enabled = true
roslynator_compiler_diagnostic_fix.cs0815.enabled = true
roslynator_compiler_diagnostic_fix.cs0819.enabled = true
roslynator_compiler_diagnostic_fix.cs0822.enabled = true
roslynator_compiler_diagnostic_fix.cs1002.enabled = true
roslynator_compiler_diagnostic_fix.cs1003.enabled = true
roslynator_compiler_diagnostic_fix.cs1004.enabled = true
roslynator_compiler_diagnostic_fix.cs1012.enabled = true
roslynator_compiler_diagnostic_fix.cs1023.enabled = true
roslynator_compiler_diagnostic_fix.cs1031.enabled = true
roslynator_compiler_diagnostic_fix.cs1057.enabled = true
roslynator_compiler_diagnostic_fix.cs1061.enabled = true
roslynator_compiler_diagnostic_fix.cs1100.enabled = true
roslynator_compiler_diagnostic_fix.cs1105.enabled = true
roslynator_compiler_diagnostic_fix.cs1106.enabled = true
roslynator_compiler_diagnostic_fix.cs1503.enabled = true
roslynator_compiler_diagnostic_fix.cs1522.enabled = true
roslynator_compiler_diagnostic_fix.cs1526.enabled = true
roslynator_compiler_diagnostic_fix.cs1527.enabled = true
roslynator_compiler_diagnostic_fix.cs1591.enabled = true
roslynator_compiler_diagnostic_fix.cs1597.enabled = true
roslynator_compiler_diagnostic_fix.cs1609.enabled = true
roslynator_compiler_diagnostic_fix.cs1615.enabled = true
roslynator_compiler_diagnostic_fix.cs1620.enabled = true
roslynator_compiler_diagnostic_fix.cs1621.enabled = true
roslynator_compiler_diagnostic_fix.cs1622.enabled = true
roslynator_compiler_diagnostic_fix.cs1623.enabled = true
roslynator_compiler_diagnostic_fix.cs1624.enabled = true
roslynator_compiler_diagnostic_fix.cs1643.enabled = true
roslynator_compiler_diagnostic_fix.cs1674.enabled = true
roslynator_compiler_diagnostic_fix.cs1689.enabled = true
roslynator_compiler_diagnostic_fix.cs1715.enabled = true
roslynator_compiler_diagnostic_fix.cs1717.enabled = true
roslynator_compiler_diagnostic_fix.cs1722.enabled = true
roslynator_compiler_diagnostic_fix.cs1737.enabled = true
roslynator_compiler_diagnostic_fix.cs1741.enabled = true
roslynator_compiler_diagnostic_fix.cs1743.enabled = true
roslynator_compiler_diagnostic_fix.cs1750.enabled = true
roslynator_compiler_diagnostic_fix.cs1751.enabled = true
roslynator_compiler_diagnostic_fix.cs1955.enabled = true
roslynator_compiler_diagnostic_fix.cs1983.enabled = true
roslynator_compiler_diagnostic_fix.cs1988.enabled = true
roslynator_compiler_diagnostic_fix.cs1994.enabled = true
roslynator_compiler_diagnostic_fix.cs1997.enabled = true
roslynator_compiler_diagnostic_fix.cs3000.enabled = true
roslynator_compiler_diagnostic_fix.cs3001.enabled = true
roslynator_compiler_diagnostic_fix.cs3002.enabled = true
roslynator_compiler_diagnostic_fix.cs3003.enabled = true
roslynator_compiler_diagnostic_fix.cs3005.enabled = true
roslynator_compiler_diagnostic_fix.cs3006.enabled = true
roslynator_compiler_diagnostic_fix.cs3007.enabled = true
roslynator_compiler_diagnostic_fix.cs3008.enabled = true
roslynator_compiler_diagnostic_fix.cs3009.enabled = true
roslynator_compiler_diagnostic_fix.cs3016.enabled = true
roslynator_compiler_diagnostic_fix.cs3024.enabled = true
roslynator_compiler_diagnostic_fix.cs3027.enabled = true
roslynator_compiler_diagnostic_fix.cs7036.enabled = true
roslynator_compiler_diagnostic_fix.cs8050.enabled = true
roslynator_compiler_diagnostic_fix.cs8070.enabled = true
roslynator_compiler_diagnostic_fix.cs8112.enabled = true
roslynator_compiler_diagnostic_fix.cs8139.enabled = true
roslynator_compiler_diagnostic_fix.cs8340.enabled = true
roslynator_compiler_diagnostic_fix.cs8403.enabled = true
roslynator_compiler_diagnostic_fix.cs8618.enabled = true
roslynator_compiler_diagnostic_fix.cs8625.enabled = true
roslynator_compiler_diagnostic_fix.cs8632.enabled = true
```
