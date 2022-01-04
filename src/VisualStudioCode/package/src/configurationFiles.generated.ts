export const configurationFileContent = {
	roslynatorconfig: `is_global = true
# Roslynator Config File

is_global = true

# Options in this file can be used
#  1) In a standard.editorconfig file
#  2) In a Roslynator default configuration file
#     Location of the file depends on the operation system:
#       Windows: C:/Users/<USERNAME>/AppData/Local/.roslynatorconfig
#       Linux: /home/<<USERNAME>>/.local/share/.roslynatorconfig
#       OSX: /Users/<<USERNAME>>/.local/share/.roslynatorconfig
#     The file must contain "is_global = true" directive
#     Default configuration is loaded once when IDE starts. Therefore, it may be necessary to restart IDE for changes to take effect.

## Set severity for all analyzers
#dotnet_analyzer_diagnostic.category-roslynator.severity = default|none|silent|suggestion|warning|error

## Set severity for a specific analyzer
#dotnet_diagnostic.<ANALYZER_ID>.severity = default|none|silent|suggestion|warning|error

## Enable/disable all refactorings
#roslynator.refactorings.enabled = true|false

## Enable/disable specific refactoring
#roslynator.refactoring.<REFACTORING_NAME>.enabled = true|false

## Enable/disable all fixes for compiler diagnostics
#roslynator.compiler_diagnostic_fixes.enabled = true|false

## Enable/disable fix for a specific compiler diagnostics
#roslynator.compiler_diagnostic_fix.<COMPILER_DIAGNOSTIC_ID>.enabled = true|false

# Options

roslynator.accessibility_modifiers = explicit|implicit
# Applicable to: RCS1018

roslynator.anonymous_function_or_method_group = anonymous_function|method_group
# Applicable to: RCS1207

roslynator.array_creation_type_style = explicit|implicit|implicit_when_type_is_obvious
# Applicable to: RCS1014

roslynator.arrow_token_new_line = after|before
# Applicable to: RCS0032

roslynator.binary_operator_new_line = after|before
# Applicable to: RCS0027

roslynator.blank_line_between_closing_brace_and_switch_section = true|false
# Applicable to: RCS1036

roslynator.blank_line_between_single_line_accessors = true|false
# Applicable to: RCS0011

roslynator.blank_line_between_using_directive_groups = true|false
# Applicable to: RCS0015

roslynator.body_style = block|expression
# Applicable to: RCS1016

roslynator.condition_in_conditional_operator_parentheses_style = include|omit||omit_when_condition_is_single_token
# Applicable to: RCS1051

roslynator.conditional_operator_new_line = after|before
# Applicable to: RCS0028

roslynator.configure_await = true|false
# Applicable to: RCS1090

roslynator.empty_string_style = field|literal
# Applicable to: RCS1078

roslynator.enum_has_flag_style = method|operator
# Applicable to: RCS1096

roslynator.equals_token_new_line = after|before
# Applicable to: RCS0052

roslynator.max_line_length = 140
# Applicable to: RCS0056

roslynator.new_line_at_end_of_file = true|false
# Applicable to: RCS0058

roslynator.new_line_before_while_in_do_statement = true|false
# Applicable to: RCS0051

roslynator.null_check_style = equality_operator|pattern_matching
# Applicable to: RCS1248

roslynator.object_creation_parentheses_style = include|omit
# Applicable to: RCS1050

roslynator.object_creation_type_style = explicit|implicit|implicit_when_type_is_obvious
# Applicable to: RCS1250

roslynator.prefer_block_body_when_declaration_spans_over_multiple_lines = true|false
# Applicable to: RCS1016

roslynator.prefer_block_body_when_expression_spans_over_multiple_lines = true|false
# Applicable to: RCS1016

roslynator.prefix_field_identifier_with_underscore = true|false

roslynator.suppress_unity_script_methods = true|false
# Applicable to: RCS1213

roslynator.use_var_instead_of_implicit_object_creation = true|false
# Applicable to: RCS1250


# Analyzers

# Add empty line after embedded statement
dotnet_diagnostic.RCS0001.severity = none

# Add empty line after #region
dotnet_diagnostic.RCS0002.severity = none

# Add empty line after using directive list
dotnet_diagnostic.RCS0003.severity = none

# Add empty line before #endregion
dotnet_diagnostic.RCS0005.severity = none

# Add empty line before using directive list
dotnet_diagnostic.RCS0006.severity = none

# Add empty line between accessors
dotnet_diagnostic.RCS0007.severity = none

# Add empty line between block and statement
dotnet_diagnostic.RCS0008.severity = none

# Add empty line between declaration and documentation comment
dotnet_diagnostic.RCS0009.severity = none

# Add empty line between declarations
dotnet_diagnostic.RCS0010.severity = none

# Add/remove blank line between single-line accessors
dotnet_diagnostic.RCS0011.severity = none
# Remove empty line between single-line accessors
roslynator.RCS0011.invert = false

# Add empty line between single-line declarations
dotnet_diagnostic.RCS0012.severity = none

# Add empty line between single-line declarations of different kind
dotnet_diagnostic.RCS0013.severity = none

# Add empty line between switch sections
dotnet_diagnostic.RCS0014.severity = none

# Add/remove empty line between using directive groups
dotnet_diagnostic.RCS0015.severity = none
# Remove empty line between using directives with different root namespace
roslynator.RCS0015.invert = false

# Add new line after attribute list
dotnet_diagnostic.RCS0016.severity = none

# Add new line after opening brace of accessor
dotnet_diagnostic.RCS0020.severity = none

# Add new line after opening brace of block
dotnet_diagnostic.RCS0021.severity = none

# Add new line after opening brace of empty block
dotnet_diagnostic.RCS0022.severity = none

# Add new line after opening brace of type declaration
dotnet_diagnostic.RCS0023.severity = none

# Add new line after switch label
dotnet_diagnostic.RCS0024.severity = none

# Add new line before accessor of full property
dotnet_diagnostic.RCS0025.severity = none

# Place new line after/before binary operator
dotnet_diagnostic.RCS0027.severity = none
# Add new line after binary operator instead of before it
roslynator.RCS0027.invert = false

# Place new line after/before '?:' operator
dotnet_diagnostic.RCS0028.severity = none
# Add new line after conditional operator instead of before it
roslynator.RCS0028.invert = false

# Add new line before constructor initializer
dotnet_diagnostic.RCS0029.severity = none

# Add new line before embedded statement
dotnet_diagnostic.RCS0030.severity = none

# Add new line before enum member
dotnet_diagnostic.RCS0031.severity = none

# Place new line after/before arrow token
dotnet_diagnostic.RCS0032.severity = none
# Add new line after expression body arrow instead of before it
roslynator.RCS0032.invert = false

# Add new line before statement
dotnet_diagnostic.RCS0033.severity = none

# Add new line before type parameter constraint
dotnet_diagnostic.RCS0034.severity = none

# Remove empty line between single-line declarations of same kind
dotnet_diagnostic.RCS0036.severity = none

# Remove empty line between using directives with same root namespace
dotnet_diagnostic.RCS0038.severity = none

# Remove new line before base list
dotnet_diagnostic.RCS0039.severity = none

# Remove new line between 'if' keyword and 'else' keyword
dotnet_diagnostic.RCS0041.severity = none

# Remove new lines from accessor list of auto-property
dotnet_diagnostic.RCS0042.severity = none

# Remove new lines from accessor with single-line expression
dotnet_diagnostic.RCS0043.severity = none

# Use carriage return + linefeed as new line
dotnet_diagnostic.RCS0044.severity = none

# Use linefeed as new line
dotnet_diagnostic.RCS0045.severity = none

# Use spaces instead of tab
dotnet_diagnostic.RCS0046.severity = none

# [deprecated] Wrap and indent each node in list
dotnet_diagnostic.RCS0047.severity = none

# Remove new lines from initializer with single-line expression
dotnet_diagnostic.RCS0048.severity = none

# Add empty line after top comment
dotnet_diagnostic.RCS0049.severity = none

# Add empty line before top declaration
dotnet_diagnostic.RCS0050.severity = none

# Add/remove new line before 'while' in 'do' statement
dotnet_diagnostic.RCS0051.severity = none
# Remove new line between closing brace and 'while' keyword
roslynator.RCS0051.invert = false

# Place new line after/before equals token
dotnet_diagnostic.RCS0052.severity = none
# Add new line after equals sign instead of before it
roslynator.RCS0052.invert = false

# Fix formatting of a list
dotnet_diagnostic.RCS0053.severity = none

# Fix formatting of a call chain
dotnet_diagnostic.RCS0054.severity = none

# Fix formatting of a binary expression chain
dotnet_diagnostic.RCS0055.severity = none

# A line is too long
dotnet_diagnostic.RCS0056.severity = none

# Normalize whitespace at the beginning of a file
dotnet_diagnostic.RCS0057.severity = none

# Normalize whitespace at the end of a file
dotnet_diagnostic.RCS0058.severity = none

# Add braces (when expression spans over multiple lines)
dotnet_diagnostic.RCS1001.severity = suggestion

# Remove braces
dotnet_diagnostic.RCS1002.severity = none

# Add braces to if-else (when expression spans over multiple lines)
dotnet_diagnostic.RCS1003.severity = suggestion

# Remove braces from if-else
dotnet_diagnostic.RCS1004.severity = none

# Simplify nested using statement
dotnet_diagnostic.RCS1005.severity = silent

# Merge 'else' with nested 'if'
dotnet_diagnostic.RCS1006.severity = silent

# Add braces
dotnet_diagnostic.RCS1007.severity = none

# Use explicit type instead of 'var' (when the type is not obvious)
dotnet_diagnostic.RCS1008.severity = none

# Use explicit type instead of 'var' (foreach variable)
dotnet_diagnostic.RCS1009.severity = none

# Use 'var' instead of explicit type (when the type is obvious)
dotnet_diagnostic.RCS1010.severity = none

# Use explicit type instead of 'var' (when the type is obvious)
dotnet_diagnostic.RCS1012.severity = none

# Use predefined type
dotnet_diagnostic.RCS1013.severity = none

# Use explicitly/implicitly typed array
dotnet_diagnostic.RCS1014.severity = none
# Use implicitly typed array
roslynator.RCS1014.invert = false
# Use implicitly typed array (when type is obvious)
roslynator.RCS1014.use_implicit_type_when_obvious = false

# Use nameof operator
dotnet_diagnostic.RCS1015.severity = suggestion

# Use block body or expression body
dotnet_diagnostic.RCS1016.severity = none
# Convert expression body to block body
roslynator.RCS1016.invert = false
# Convert expression body to block body when declaration is multi-line
roslynator.RCS1016.use_block_body_when_declaration_is_multiline = false
# Convert expression body to block body when expression is multi-line
roslynator.RCS1016.use_block_body_when_expression_is_multiline = false

# Add/remove accessibility modifiers
dotnet_diagnostic.RCS1018.severity = suggestion
# Remove accessibility modifiers
roslynator.RCS1018.invert = false

# Order modifiers
dotnet_diagnostic.RCS1019.severity = none

# Simplify Nullable<T> to T?
dotnet_diagnostic.RCS1020.severity = suggestion

# Convert lambda expression body to expression body
dotnet_diagnostic.RCS1021.severity = suggestion

# Remove unnecessary braces
dotnet_diagnostic.RCS1031.severity = silent

# Remove redundant parentheses
dotnet_diagnostic.RCS1032.severity = suggestion

# Remove redundant boolean literal
dotnet_diagnostic.RCS1033.severity = suggestion

# Remove redundant 'sealed' modifier
dotnet_diagnostic.RCS1034.severity = silent

# Remove redundant comma in initializer
dotnet_diagnostic.RCS1035.severity = none

# Remove redundant empty line
dotnet_diagnostic.RCS1036.severity = suggestion
# Remove empty line between closing brace and switch section
roslynator.RCS1036.remove_empty_line_between_closing_brace_and_switch_section = false

# Remove trailing white-space
dotnet_diagnostic.RCS1037.severity = suggestion

# Remove empty statement
dotnet_diagnostic.RCS1038.severity = suggestion

# Remove argument list from attribute
dotnet_diagnostic.RCS1039.severity = silent

# Remove empty 'else' clause
dotnet_diagnostic.RCS1040.severity = silent

# Remove empty initializer
dotnet_diagnostic.RCS1041.severity = suggestion

# Remove enum default underlying type
dotnet_diagnostic.RCS1042.severity = silent

# Remove 'partial' modifier from type with a single part
dotnet_diagnostic.RCS1043.severity = silent

# Remove original exception from throw statement
dotnet_diagnostic.RCS1044.severity = warning

# Asynchronous method name should end with 'Async'
dotnet_diagnostic.RCS1046.severity = none

# Non-asynchronous method name should not end with 'Async'
dotnet_diagnostic.RCS1047.severity = suggestion

# Use lambda expression instead of anonymous method
dotnet_diagnostic.RCS1048.severity = suggestion

# Simplify boolean comparison
dotnet_diagnostic.RCS1049.severity = suggestion

# Include/omit parentheses when creating new object
dotnet_diagnostic.RCS1050.severity = none
# Remove parentheses when creating new object
roslynator.RCS1050.invert = false

# Add/remove parentheses from condition in conditional operator
dotnet_diagnostic.RCS1051.severity = none
# Remove parentheses from condition of conditional expression (when condition is a single token)
roslynator.RCS1051.do_not_parenthesize_single_token = false

# Declare each attribute separately
dotnet_diagnostic.RCS1052.severity = none

# Avoid semicolon at the end of declaration
dotnet_diagnostic.RCS1055.severity = silent

# Avoid usage of using alias directive
dotnet_diagnostic.RCS1056.severity = none

# Use compound assignment
dotnet_diagnostic.RCS1058.severity = suggestion

# Avoid locking on publicly accessible instance
dotnet_diagnostic.RCS1059.severity = warning

# Declare each type in separate file
dotnet_diagnostic.RCS1060.severity = none

# Merge 'if' with nested 'if'
dotnet_diagnostic.RCS1061.severity = silent

# Avoid usage of do statement to create an infinite loop
dotnet_diagnostic.RCS1063.severity = suggestion

# Avoid usage of for statement to create an infinite loop
dotnet_diagnostic.RCS1064.severity = none

# Avoid usage of while statement to create an infinite loop
dotnet_diagnostic.RCS1065.severity = none

# Remove empty 'finally' clause
dotnet_diagnostic.RCS1066.severity = silent

# Simplify logical negation
dotnet_diagnostic.RCS1068.severity = suggestion

# Remove unnecessary case label
dotnet_diagnostic.RCS1069.severity = silent

# Remove redundant default switch section
dotnet_diagnostic.RCS1070.severity = silent

# Remove redundant base constructor call
dotnet_diagnostic.RCS1071.severity = silent

# Remove empty namespace declaration
dotnet_diagnostic.RCS1072.severity = suggestion

# Convert 'if' to 'return' statement
dotnet_diagnostic.RCS1073.severity = suggestion

# Remove redundant constructor
dotnet_diagnostic.RCS1074.severity = silent

# Avoid empty catch clause that catches System.Exception
dotnet_diagnostic.RCS1075.severity = warning

# Optimize LINQ method call
dotnet_diagnostic.RCS1077.severity = suggestion

# Use "" or 'string.Empty'
dotnet_diagnostic.RCS1078.severity = none
# Use string.Empty instead of ""
roslynator.RCS1078.invert = false

# Throwing of new NotImplementedException
dotnet_diagnostic.RCS1079.severity = none

# Use 'Count/Length' property instead of 'Any' method
dotnet_diagnostic.RCS1080.severity = suggestion

# Split variable declaration
dotnet_diagnostic.RCS1081.severity = none

# Use coalesce expression instead of conditional expression
dotnet_diagnostic.RCS1084.severity = suggestion

# Use auto-implemented property
dotnet_diagnostic.RCS1085.severity = suggestion

# Use --/++ operator instead of assignment
dotnet_diagnostic.RCS1089.severity = suggestion

# Add/remove 'ConfigureAwait(false)' call
dotnet_diagnostic.RCS1090.severity = none
# Remove call to 'ConfigureAwait'
roslynator.RCS1090.invert = false

# Remove empty region
dotnet_diagnostic.RCS1091.severity = silent

# Remove file with no code
dotnet_diagnostic.RCS1093.severity = suggestion

# Declare using directive on top level
dotnet_diagnostic.RCS1094.severity = none

# Use 'HasFlag' method or bitwise operator
dotnet_diagnostic.RCS1096.severity = suggestion
# Convert bitwise operation to 'HasFlag' call
roslynator.RCS1096.invert = false

# Remove redundant 'ToString' call
dotnet_diagnostic.RCS1097.severity = suggestion

# Constant values should be placed on right side of comparisons
dotnet_diagnostic.RCS1098.severity = suggestion

# Default label should be the last label in a switch section
dotnet_diagnostic.RCS1099.severity = suggestion

# Format documentation summary on a single line
dotnet_diagnostic.RCS1100.severity = none

# Format documentation summary on multiple lines
dotnet_diagnostic.RCS1101.severity = none

# Make class static
dotnet_diagnostic.RCS1102.severity = warning

# Convert 'if' to assignment
dotnet_diagnostic.RCS1103.severity = suggestion

# Simplify conditional expression
dotnet_diagnostic.RCS1104.severity = suggestion
# Do not simplify conditional expression when condition is inverted
roslynator.RCS1104.suppress_when_condition_is_inverted = false

# Unnecessary interpolation
dotnet_diagnostic.RCS1105.severity = suggestion

# Remove empty destructor
dotnet_diagnostic.RCS1106.severity = suggestion

# Remove redundant 'ToCharArray' call
dotnet_diagnostic.RCS1107.severity = suggestion

# Add 'static' modifier to all partial class declarations
dotnet_diagnostic.RCS1108.severity = suggestion

# Declare type inside namespace
dotnet_diagnostic.RCS1110.severity = suggestion

# Add braces to switch section with multiple statements
dotnet_diagnostic.RCS1111.severity = none

# Combine 'Enumerable.Where' method chain
dotnet_diagnostic.RCS1112.severity = suggestion

# Use 'string.IsNullOrEmpty' method
dotnet_diagnostic.RCS1113.severity = suggestion

# Remove redundant delegate creation
dotnet_diagnostic.RCS1114.severity = suggestion

# Mark local variable as const
dotnet_diagnostic.RCS1118.severity = suggestion

# Add parentheses when necessary
dotnet_diagnostic.RCS1123.severity = suggestion

# Inline local variable
dotnet_diagnostic.RCS1124.severity = silent

# Add braces to if-else
dotnet_diagnostic.RCS1126.severity = none

# Use coalesce expression
dotnet_diagnostic.RCS1128.severity = suggestion

# Remove redundant field initialization
dotnet_diagnostic.RCS1129.severity = silent

# Bitwise operation on enum without Flags attribute
dotnet_diagnostic.RCS1130.severity = suggestion

# Remove redundant overriding member
dotnet_diagnostic.RCS1132.severity = suggestion

# Remove redundant Dispose/Close call
dotnet_diagnostic.RCS1133.severity = silent

# Remove redundant statement
dotnet_diagnostic.RCS1134.severity = silent

# Declare enum member with zero value (when enum has FlagsAttribute)
dotnet_diagnostic.RCS1135.severity = suggestion

# Merge switch sections with equivalent content
dotnet_diagnostic.RCS1136.severity = silent

# Add summary to documentation comment
dotnet_diagnostic.RCS1138.severity = warning

# Add summary element to documentation comment
dotnet_diagnostic.RCS1139.severity = warning

# Add exception to documentation comment
dotnet_diagnostic.RCS1140.severity = silent

# Add 'param' element to documentation comment
dotnet_diagnostic.RCS1141.severity = silent

# Add 'typeparam' element to documentation comment
dotnet_diagnostic.RCS1142.severity = silent

# Simplify coalesce expression
dotnet_diagnostic.RCS1143.severity = silent

# Remove redundant 'as' operator
dotnet_diagnostic.RCS1145.severity = silent

# Use conditional access
dotnet_diagnostic.RCS1146.severity = suggestion

# Remove redundant cast
dotnet_diagnostic.RCS1151.severity = silent

# Sort enum members
dotnet_diagnostic.RCS1154.severity = suggestion

# Use StringComparison when comparing strings
dotnet_diagnostic.RCS1155.severity = warning

# Use string.Length instead of comparison with empty string
dotnet_diagnostic.RCS1156.severity = suggestion

# Composite enum value contains undefined flag
dotnet_diagnostic.RCS1157.severity = suggestion

# Static member in generic type should use a type parameter
dotnet_diagnostic.RCS1158.severity = suggestion

# Use EventHandler<T>
dotnet_diagnostic.RCS1159.severity = suggestion

# Abstract type should not have public constructors
dotnet_diagnostic.RCS1160.severity = suggestion

# Enum should declare explicit values
dotnet_diagnostic.RCS1161.severity = silent

# Avoid chain of assignments
dotnet_diagnostic.RCS1162.severity = none

# Unused parameter
dotnet_diagnostic.RCS1163.severity = suggestion

# Unused type parameter
dotnet_diagnostic.RCS1164.severity = suggestion

# Unconstrained type parameter checked for null
dotnet_diagnostic.RCS1165.severity = silent

# Value type object is never equal to null
dotnet_diagnostic.RCS1166.severity = suggestion

# Parameter name differs from base name
dotnet_diagnostic.RCS1168.severity = silent

# Make field read-only
dotnet_diagnostic.RCS1169.severity = suggestion

# Use read-only auto-implemented property
dotnet_diagnostic.RCS1170.severity = suggestion

# Simplify lazy initialization
dotnet_diagnostic.RCS1171.severity = suggestion

# Use 'is' operator instead of 'as' operator
dotnet_diagnostic.RCS1172.severity = warning

# Use coalesce expression instead of 'if'
dotnet_diagnostic.RCS1173.severity = suggestion

# Remove redundant async/await
dotnet_diagnostic.RCS1174.severity = none

# Unused this parameter
dotnet_diagnostic.RCS1175.severity = suggestion

# Use 'var' instead of explicit type (when the type is not obvious)
dotnet_diagnostic.RCS1176.severity = none

# Use 'var' instead of explicit type (in foreach)
dotnet_diagnostic.RCS1177.severity = none

# Unnecessary assignment
dotnet_diagnostic.RCS1179.severity = suggestion

# Inline lazy initialization
dotnet_diagnostic.RCS1180.severity = suggestion

# Convert comment to documentation comment
dotnet_diagnostic.RCS1181.severity = silent

# Remove redundant base interface
dotnet_diagnostic.RCS1182.severity = silent

# Use Regex instance instead of static method
dotnet_diagnostic.RCS1186.severity = silent

# Use constant instead of field
dotnet_diagnostic.RCS1187.severity = suggestion

# Remove redundant auto-property initialization
dotnet_diagnostic.RCS1188.severity = silent

# Add or remove region name
dotnet_diagnostic.RCS1189.severity = silent

# Join string expressions
dotnet_diagnostic.RCS1190.severity = suggestion

# Declare enum value as combination of names
dotnet_diagnostic.RCS1191.severity = suggestion

# Unnecessary usage of verbatim string literal
dotnet_diagnostic.RCS1192.severity = suggestion

# Overriding member should not change 'params' modifier
dotnet_diagnostic.RCS1193.severity = warning

# Implement exception constructors
dotnet_diagnostic.RCS1194.severity = warning

# Use ^ operator
dotnet_diagnostic.RCS1195.severity = suggestion

# Call extension method as instance method
dotnet_diagnostic.RCS1196.severity = suggestion

# Optimize StringBuilder.Append/AppendLine call
dotnet_diagnostic.RCS1197.severity = suggestion

# Avoid unnecessary boxing of value type
dotnet_diagnostic.RCS1198.severity = none

# Unnecessary null check
dotnet_diagnostic.RCS1199.severity = suggestion

# Call 'Enumerable.ThenBy' instead of 'Enumerable.OrderBy'
dotnet_diagnostic.RCS1200.severity = suggestion

# Use method chaining
dotnet_diagnostic.RCS1201.severity = silent

# Avoid NullReferenceException
dotnet_diagnostic.RCS1202.severity = suggestion

# Use AttributeUsageAttribute
dotnet_diagnostic.RCS1203.severity = warning

# Use EventArgs.Empty
dotnet_diagnostic.RCS1204.severity = suggestion

# Order named arguments according to the order of parameters
dotnet_diagnostic.RCS1205.severity = suggestion

# Use conditional access instead of conditional expression
dotnet_diagnostic.RCS1206.severity = suggestion

# Use anonymous function or method group
dotnet_diagnostic.RCS1207.severity = none
# Convert method group to anonymous function
roslynator.RCS1207.invert = false

# Reduce 'if' nesting
dotnet_diagnostic.RCS1208.severity = none

# Order type parameter constraints
dotnet_diagnostic.RCS1209.severity = suggestion

# Return completed task instead of returning null
dotnet_diagnostic.RCS1210.severity = warning

# Remove unnecessary 'else'
dotnet_diagnostic.RCS1211.severity = silent

# Remove redundant assignment
dotnet_diagnostic.RCS1212.severity = suggestion

# Remove unused member declaration
dotnet_diagnostic.RCS1213.severity = suggestion
# Suppress Unity script methods
roslynator.RCS1213.suppress_unity_script_methods = false

# Unnecessary interpolated string
dotnet_diagnostic.RCS1214.severity = suggestion

# Expression is always equal to true/false
dotnet_diagnostic.RCS1215.severity = warning

# Unnecessary unsafe context
dotnet_diagnostic.RCS1216.severity = suggestion

# Convert interpolated string to concatenation
dotnet_diagnostic.RCS1217.severity = silent

# Simplify code branching
dotnet_diagnostic.RCS1218.severity = suggestion

# Use pattern matching instead of combination of 'is' operator and cast operator
dotnet_diagnostic.RCS1220.severity = suggestion

# Use pattern matching instead of combination of 'as' operator and null check
dotnet_diagnostic.RCS1221.severity = suggestion

# Merge preprocessor directives
dotnet_diagnostic.RCS1222.severity = suggestion

# Mark publicly visible type with DebuggerDisplay attribute
dotnet_diagnostic.RCS1223.severity = none

# Make method an extension method
dotnet_diagnostic.RCS1224.severity = suggestion

# Make class sealed
dotnet_diagnostic.RCS1225.severity = suggestion

# Add paragraph to documentation comment
dotnet_diagnostic.RCS1226.severity = suggestion

# Validate arguments correctly
dotnet_diagnostic.RCS1227.severity = suggestion

# Unused element in documentation comment
dotnet_diagnostic.RCS1228.severity = silent

# Use async/await when necessary
dotnet_diagnostic.RCS1229.severity = suggestion

# Unnecessary explicit use of enumerator
dotnet_diagnostic.RCS1230.severity = suggestion

# Make parameter ref read-only
dotnet_diagnostic.RCS1231.severity = none

# Order elements in documentation comment
dotnet_diagnostic.RCS1232.severity = suggestion

# Use short-circuiting operator
dotnet_diagnostic.RCS1233.severity = suggestion

# Duplicate enum value
dotnet_diagnostic.RCS1234.severity = suggestion

# Optimize method call
dotnet_diagnostic.RCS1235.severity = suggestion

# Use exception filter
dotnet_diagnostic.RCS1236.severity = suggestion

# Use bit shift operator
dotnet_diagnostic.RCS1237.severity = silent

# Avoid nested ?: operators
dotnet_diagnostic.RCS1238.severity = silent

# Use 'for' statement instead of 'while' statement
dotnet_diagnostic.RCS1239.severity = suggestion

# Operator is unnecessary
dotnet_diagnostic.RCS1240.severity = suggestion

# Implement non-generic counterpart
dotnet_diagnostic.RCS1241.severity = silent

# Do not pass non-read-only struct by read-only reference
dotnet_diagnostic.RCS1242.severity = warning

# Duplicate word in a comment
dotnet_diagnostic.RCS1243.severity = suggestion

# Simplify 'default' expression
dotnet_diagnostic.RCS1244.severity = silent

# Use element access
dotnet_diagnostic.RCS1246.severity = suggestion
# Do not use element access when expression is invocation
roslynator.RCS1246.suppress_when_expression_is_invocation = false

# Fix documentation comment tag
dotnet_diagnostic.RCS1247.severity = suggestion

# Normalize null check
dotnet_diagnostic.RCS1248.severity = none
# Use comparison instead of pattern matching to check for null
roslynator.RCS1248.invert = false

# Unnecessary null-forgiving operator
dotnet_diagnostic.RCS1249.severity = suggestion

# Use implicit/explicit object creation
dotnet_diagnostic.RCS1250.severity = suggestion

# Use pattern matching
dotnet_diagnostic.RCS9001.severity = silent

# Use property SyntaxNode.SpanStart
dotnet_diagnostic.RCS9002.severity = suggestion

# Unnecessary conditional access
dotnet_diagnostic.RCS9003.severity = suggestion

# Call 'Any' instead of accessing 'Count'
dotnet_diagnostic.RCS9004.severity = suggestion

# Unnecessary null check
dotnet_diagnostic.RCS9005.severity = suggestion

# Use element access
dotnet_diagnostic.RCS9006.severity = suggestion

# Use return value
dotnet_diagnostic.RCS9007.severity = warning

# Call 'Last' instead of using []
dotnet_diagnostic.RCS9008.severity = suggestion

# Unknown language name
dotnet_diagnostic.RCS9009.severity = warning

# Specify ExportCodeRefactoringProviderAttribute.Name
dotnet_diagnostic.RCS9010.severity = silent

# Specify ExportCodeFixProviderAttribute.Name
dotnet_diagnostic.RCS9011.severity = silent


# Refactorings

roslynator.refactoring.add_all_properties_to_initializer.enabled = true
roslynator.refactoring.add_argument_name.enabled = true
roslynator.refactoring.add_braces.enabled = true
roslynator.refactoring.add_braces_to_if_else.enabled = true
roslynator.refactoring.add_braces_to_switch_section.enabled = true
roslynator.refactoring.add_braces_to_switch_sections.enabled = true
roslynator.refactoring.add_default_value_to_parameter.enabled = true
roslynator.refactoring.add_empty_line_between_declarations.enabled = true
roslynator.refactoring.add_exception_element_to_documentation_comment.enabled = true
roslynator.refactoring.add_generic_parameter_to_declaration.enabled = true
roslynator.refactoring.add_member_to_interface.enabled = true
roslynator.refactoring.add_missing_cases_to_switch.enabled = true
roslynator.refactoring.add_parameter_to_interface_member.enabled = true
roslynator.refactoring.add_tag_to_documentation_comment.enabled = true
roslynator.refactoring.add_using_directive.enabled = true
roslynator.refactoring.add_using_static_directive.enabled = true
roslynator.refactoring.call_extension_method_as_instance_method.enabled = true
roslynator.refactoring.call_indexof_instead_of_contains.enabled = true
roslynator.refactoring.comment_out_member_declaration.enabled = true
roslynator.refactoring.comment_out_statement.enabled = true
roslynator.refactoring.convert_auto_property_to_full_property.enabled = true
roslynator.refactoring.convert_auto_property_to_full_property_without_backing_field.enabled = true
roslynator.refactoring.convert_block_body_to_expression_body.enabled = true
roslynator.refactoring.convert_comment_to_documentation_comment.enabled = true
roslynator.refactoring.convert_conditional_expression_to_if_else.enabled = true
roslynator.refactoring.convert_do_to_while.enabled = true
roslynator.refactoring.convert_expression_body_to_block_body.enabled = true
roslynator.refactoring.convert_for_to_foreach.enabled = true
roslynator.refactoring.convert_for_to_while.enabled = true
roslynator.refactoring.convert_foreach_to_for.enabled = true
roslynator.refactoring.convert_foreach_to_for_and_reverse_loop.enabled = false
roslynator.refactoring.convert_hasflag_call_to_bitwise_operation.enabled = true
roslynator.refactoring.convert_hexadecimal_literal_to_decimal_literal.enabled = true
roslynator.refactoring.convert_if_to_conditional_expression.enabled = true
roslynator.refactoring.convert_if_to_switch.enabled = true
roslynator.refactoring.convert_interpolated_string_to_concatenation.enabled = true
roslynator.refactoring.convert_interpolated_string_to_string_format.enabled = true
roslynator.refactoring.convert_interpolated_string_to_string_literal.enabled = true
roslynator.refactoring.convert_lambda_block_body_to_expression_body.enabled = true
roslynator.refactoring.convert_lambda_expression_body_to_block_body.enabled = true
roslynator.refactoring.convert_method_group_to_lambda.enabled = true
roslynator.refactoring.convert_regular_string_literal_to_verbatim_string_literal.enabled = true
roslynator.refactoring.convert_return_statement_to_if.enabled = true
roslynator.refactoring.convert_statements_to_if_else.enabled = true
roslynator.refactoring.convert_string_format_to_interpolated_string.enabled = true
roslynator.refactoring.convert_switch_expression_to_switch_statement.enabled = true
roslynator.refactoring.convert_switch_to_if.enabled = true
roslynator.refactoring.convert_verbatim_string_literal_to_regular_string_literal.enabled = true
roslynator.refactoring.convert_verbatim_string_literal_to_regular_string_literals.enabled = true
roslynator.refactoring.convert_while_to_do.enabled = true
roslynator.refactoring.convert_while_to_for.enabled = true
roslynator.refactoring.copy_argument.enabled = true
roslynator.refactoring.copy_documentation_comment_from_base_member.enabled = true
roslynator.refactoring.copy_member_declaration.enabled = true
roslynator.refactoring.copy_parameter.enabled = true
roslynator.refactoring.copy_statement.enabled = true
roslynator.refactoring.copy_switch_section.enabled = true
roslynator.refactoring.expand_coalesce_expression.enabled = true
roslynator.refactoring.expand_compound_assignment.enabled = true
roslynator.refactoring.expand_event_declaration.enabled = true
roslynator.refactoring.expand_initializer.enabled = false
roslynator.refactoring.expand_positional_constructor.enabled = true
roslynator.refactoring.extract_event_handler_method.enabled = true
roslynator.refactoring.extract_expression_from_condition.enabled = true
roslynator.refactoring.extract_type_declaration_to_new_file.enabled = true
roslynator.refactoring.generate_base_constructors.enabled = true
roslynator.refactoring.generate_combined_enum_member.enabled = true
roslynator.refactoring.generate_enum_member.enabled = true
roslynator.refactoring.generate_enum_values.enabled = true
roslynator.refactoring.generate_event_invoking_method.enabled = true
roslynator.refactoring.generate_property_for_debuggerdisplay_attribute.enabled = true
roslynator.refactoring.change_accessibility.enabled = true
roslynator.refactoring.change_method_return_type_to_void.enabled = true
roslynator.refactoring.change_type_according_to_expression.enabled = true
roslynator.refactoring.check_expression_for_null.enabled = true
roslynator.refactoring.check_parameter_for_null.enabled = true
roslynator.refactoring.implement_custom_enumerator.enabled = true
roslynator.refactoring.implement_iequatable.enabled = true
roslynator.refactoring.initialize_field_from_constructor.enabled = true
roslynator.refactoring.initialize_local_variable_with_default_value.enabled = true
roslynator.refactoring.inline_alias_expression.enabled = true
roslynator.refactoring.inline_constant.enabled = true
roslynator.refactoring.inline_constant_value.enabled = true
roslynator.refactoring.inline_method.enabled = true
roslynator.refactoring.inline_property.enabled = true
roslynator.refactoring.inline_using_static.enabled = true
roslynator.refactoring.insert_string_interpolation.enabled = true
roslynator.refactoring.introduce_and_initialize_field.enabled = true
roslynator.refactoring.introduce_and_initialize_property.enabled = true
roslynator.refactoring.introduce_constructor.enabled = false
roslynator.refactoring.introduce_field_to_lock_on.enabled = true
roslynator.refactoring.introduce_local_variable.enabled = true
roslynator.refactoring.invert_binary_expression.enabled = true
roslynator.refactoring.invert_boolean_literal.enabled = true
roslynator.refactoring.invert_conditional_expression.enabled = true
roslynator.refactoring.invert_if.enabled = true
roslynator.refactoring.invert_if_else.enabled = true
roslynator.refactoring.invert_is_expression.enabled = true
roslynator.refactoring.invert_linq_method_call.enabled = true
roslynator.refactoring.invert_operator.enabled = true
roslynator.refactoring.invert_prefix_or_postfix_unary_expression.enabled = true
roslynator.refactoring.join_string_expressions.enabled = true
roslynator.refactoring.make_member_abstract.enabled = true
roslynator.refactoring.make_member_virtual.enabled = true
roslynator.refactoring.merge_attributes.enabled = true
roslynator.refactoring.merge_if_statements.enabled = true
roslynator.refactoring.merge_if_with_parent_if.enabled = true
roslynator.refactoring.merge_local_declarations.enabled = true
roslynator.refactoring.merge_switch_sections.enabled = true
roslynator.refactoring.move_unsafe_context_to_containing_declaration.enabled = true
roslynator.refactoring.notify_when_property_changes.enabled = true
roslynator.refactoring.parenthesize_expression.enabled = true
roslynator.refactoring.promote_local_variable_to_parameter.enabled = true
roslynator.refactoring.remove_all_comments.enabled = true
roslynator.refactoring.remove_all_comments_except_documentation_comments.enabled = true
roslynator.refactoring.remove_all_documentation_comments.enabled = false
roslynator.refactoring.remove_all_member_declarations.enabled = true
roslynator.refactoring.remove_all_preprocessor_directives.enabled = true
roslynator.refactoring.remove_all_region_directives.enabled = true
roslynator.refactoring.remove_all_statements.enabled = true
roslynator.refactoring.remove_all_switch_sections.enabled = true
roslynator.refactoring.remove_argument_name.enabled = true
roslynator.refactoring.remove_async_await.enabled = true
roslynator.refactoring.remove_braces.enabled = true
roslynator.refactoring.remove_braces_from_if_else.enabled = true
roslynator.refactoring.remove_braces_from_switch_section.enabled = true
roslynator.refactoring.remove_braces_from_switch_sections.enabled = true
roslynator.refactoring.remove_comment.enabled = true
roslynator.refactoring.remove_condition_from_last_else.enabled = true
roslynator.refactoring.remove_containing_statement.enabled = true
roslynator.refactoring.remove_empty_lines.enabled = true
roslynator.refactoring.remove_enum_member_value.enabled = true
roslynator.refactoring.remove_instantiation_of_local_variable.enabled = true
roslynator.refactoring.remove_interpolation.enabled = true
roslynator.refactoring.remove_member_declaration.enabled = true
roslynator.refactoring.remove_member_declarations_above_or_below.enabled = true
roslynator.refactoring.remove_parentheses.enabled = true
roslynator.refactoring.remove_preprocessor_directive.enabled = true
roslynator.refactoring.remove_property_initializer.enabled = true
roslynator.refactoring.remove_region.enabled = true
roslynator.refactoring.remove_statement.enabled = true
roslynator.refactoring.remove_unnecessary_assignment.enabled = true
roslynator.refactoring.rename_identifier_according_to_type_name.enabled = true
roslynator.refactoring.rename_method_according_to_type_name.enabled = true
roslynator.refactoring.rename_parameter_according_to_type_name.enabled = true
roslynator.refactoring.rename_property_according_to_type_name.enabled = true
roslynator.refactoring.replace_as_expression_with_explicit_cast.enabled = true
roslynator.refactoring.replace_conditional_expression_with_true_or_false_branch.enabled = true
roslynator.refactoring.replace_equality_operator_with_string_equals.enabled = true
roslynator.refactoring.replace_equality_operator_with_string_isnullorempty.enabled = true
roslynator.refactoring.replace_equality_operator_with_string_isnullorwhitespace.enabled = true
roslynator.refactoring.replace_explicit_cast_with_as_expression.enabled = true
roslynator.refactoring.replace_interpolated_string_with_interpolation_expression.enabled = true
roslynator.refactoring.replace_method_with_property.enabled = false
roslynator.refactoring.replace_null_literal_with_default_expression.enabled = true
roslynator.refactoring.replace_prefix_operator_with_postfix_operator.enabled = true
roslynator.refactoring.replace_property_with_method.enabled = true
roslynator.refactoring.reverse_for_statement.enabled = true
roslynator.refactoring.simplify_if.enabled = true
roslynator.refactoring.sort_case_labels.enabled = true
roslynator.refactoring.sort_member_declarations.enabled = true
roslynator.refactoring.split_attributes.enabled = true
roslynator.refactoring.split_if.enabled = true
roslynator.refactoring.split_if_else.enabled = true
roslynator.refactoring.split_switch_labels.enabled = true
roslynator.refactoring.split_variable_declaration.enabled = true
roslynator.refactoring.swap_binary_operands.enabled = true
roslynator.refactoring.swap_member_declarations.enabled = true
roslynator.refactoring.sync_property_name_and_backing_field_name.enabled = true
roslynator.refactoring.uncomment_multiline_comment.enabled = true
roslynator.refactoring.uncomment_singleline_comment.enabled = true
roslynator.refactoring.use_coalesce_expression_instead_of_if.enabled = true
roslynator.refactoring.use_constant_instead_of_readonly_field.enabled = true
roslynator.refactoring.use_element_access_instead_of_linq_method.enabled = true
roslynator.refactoring.use_enumerator_explicitly.enabled = true
roslynator.refactoring.use_explicit_type.enabled = true
roslynator.refactoring.use_implicit_type.enabled = true
roslynator.refactoring.use_index_initializer.enabled = true
roslynator.refactoring.use_lambda_instead_of_anonymous_method.enabled = true
roslynator.refactoring.use_list_instead_of_yield.enabled = true
roslynator.refactoring.use_object_initializer.enabled = true
roslynator.refactoring.use_readonly_field_instead_of_constant.enabled = true
roslynator.refactoring.use_string_empty_instead_of_empty_string_literal.enabled = false
roslynator.refactoring.use_stringbuilder_instead_of_concatenation.enabled = true
roslynator.refactoring.wrap_arguments.enabled = true
roslynator.refactoring.wrap_binary_expression.enabled = true
roslynator.refactoring.wrap_call_chain.enabled = true
roslynator.refactoring.wrap_conditional_expression.enabled = true
roslynator.refactoring.wrap_constraint_clauses.enabled = true
roslynator.refactoring.wrap_initializer_expressions.enabled = true
roslynator.refactoring.wrap_lines_in_preprocessor_directive.enabled = true
roslynator.refactoring.wrap_lines_in_region.enabled = true
roslynator.refactoring.wrap_lines_in_try_catch.enabled = true
roslynator.refactoring.wrap_parameters.enabled = true
roslynator.refactoring.wrap_statements_in_condition.enabled = true
roslynator.refactoring.wrap_statements_in_using_statement.enabled = true

# Compiler diagnostic fixes

roslynator.compiler_diagnostic_fix.CS0019.enabled = true
roslynator.compiler_diagnostic_fix.CS0021.enabled = true
roslynator.compiler_diagnostic_fix.CS0023.enabled = true
roslynator.compiler_diagnostic_fix.CS0029.enabled = true
roslynator.compiler_diagnostic_fix.CS0030.enabled = true
roslynator.compiler_diagnostic_fix.CS0037.enabled = true
roslynator.compiler_diagnostic_fix.CS0069.enabled = true
roslynator.compiler_diagnostic_fix.CS0077.enabled = true
roslynator.compiler_diagnostic_fix.CS0080.enabled = true
roslynator.compiler_diagnostic_fix.CS0101.enabled = true
roslynator.compiler_diagnostic_fix.CS0102.enabled = true
roslynator.compiler_diagnostic_fix.CS0103.enabled = true
roslynator.compiler_diagnostic_fix.CS0106.enabled = true
roslynator.compiler_diagnostic_fix.CS0107.enabled = true
roslynator.compiler_diagnostic_fix.CS0108.enabled = true
roslynator.compiler_diagnostic_fix.CS0109.enabled = true
roslynator.compiler_diagnostic_fix.CS0112.enabled = true
roslynator.compiler_diagnostic_fix.CS0114.enabled = true
roslynator.compiler_diagnostic_fix.CS0115.enabled = true
roslynator.compiler_diagnostic_fix.CS0119.enabled = true
roslynator.compiler_diagnostic_fix.CS0120.enabled = true
roslynator.compiler_diagnostic_fix.CS0123.enabled = true
roslynator.compiler_diagnostic_fix.CS0126.enabled = true
roslynator.compiler_diagnostic_fix.CS0127.enabled = true
roslynator.compiler_diagnostic_fix.CS0128.enabled = true
roslynator.compiler_diagnostic_fix.CS0131.enabled = true
roslynator.compiler_diagnostic_fix.CS0132.enabled = true
roslynator.compiler_diagnostic_fix.CS0133.enabled = true
roslynator.compiler_diagnostic_fix.CS0136.enabled = true
roslynator.compiler_diagnostic_fix.CS0139.enabled = true
roslynator.compiler_diagnostic_fix.CS0152.enabled = true
roslynator.compiler_diagnostic_fix.CS0161.enabled = true
roslynator.compiler_diagnostic_fix.CS0162.enabled = true
roslynator.compiler_diagnostic_fix.CS0163.enabled = true
roslynator.compiler_diagnostic_fix.CS0164.enabled = true
roslynator.compiler_diagnostic_fix.CS0165.enabled = true
roslynator.compiler_diagnostic_fix.CS0168.enabled = true
roslynator.compiler_diagnostic_fix.CS0173.enabled = true
roslynator.compiler_diagnostic_fix.CS0177.enabled = true
roslynator.compiler_diagnostic_fix.CS0191.enabled = true
roslynator.compiler_diagnostic_fix.CS0192.enabled = true
roslynator.compiler_diagnostic_fix.CS0201.enabled = true
roslynator.compiler_diagnostic_fix.CS0214.enabled = true
roslynator.compiler_diagnostic_fix.CS0216.enabled = true
roslynator.compiler_diagnostic_fix.CS0219.enabled = true
roslynator.compiler_diagnostic_fix.CS0221.enabled = true
roslynator.compiler_diagnostic_fix.CS0225.enabled = true
roslynator.compiler_diagnostic_fix.CS0238.enabled = true
roslynator.compiler_diagnostic_fix.CS0246.enabled = true
roslynator.compiler_diagnostic_fix.CS0260.enabled = true
roslynator.compiler_diagnostic_fix.CS0262.enabled = true
roslynator.compiler_diagnostic_fix.CS0266.enabled = true
roslynator.compiler_diagnostic_fix.CS0267.enabled = true
roslynator.compiler_diagnostic_fix.CS0272.enabled = true
roslynator.compiler_diagnostic_fix.CS0275.enabled = true
roslynator.compiler_diagnostic_fix.CS0305.enabled = true
roslynator.compiler_diagnostic_fix.CS0401.enabled = true
roslynator.compiler_diagnostic_fix.CS0403.enabled = true
roslynator.compiler_diagnostic_fix.CS0405.enabled = true
roslynator.compiler_diagnostic_fix.CS0407.enabled = true
roslynator.compiler_diagnostic_fix.CS0409.enabled = true
roslynator.compiler_diagnostic_fix.CS0428.enabled = true
roslynator.compiler_diagnostic_fix.CS0441.enabled = true
roslynator.compiler_diagnostic_fix.CS0442.enabled = true
roslynator.compiler_diagnostic_fix.CS0449.enabled = true
roslynator.compiler_diagnostic_fix.CS0450.enabled = true
roslynator.compiler_diagnostic_fix.CS0451.enabled = true
roslynator.compiler_diagnostic_fix.CS0472.enabled = true
roslynator.compiler_diagnostic_fix.CS0500.enabled = true
roslynator.compiler_diagnostic_fix.CS0501.enabled = true
roslynator.compiler_diagnostic_fix.CS0507.enabled = true
roslynator.compiler_diagnostic_fix.CS0508.enabled = true
roslynator.compiler_diagnostic_fix.CS0513.enabled = true
roslynator.compiler_diagnostic_fix.CS0515.enabled = true
roslynator.compiler_diagnostic_fix.CS0524.enabled = true
roslynator.compiler_diagnostic_fix.CS0525.enabled = true
roslynator.compiler_diagnostic_fix.CS0527.enabled = true
roslynator.compiler_diagnostic_fix.CS0531.enabled = true
roslynator.compiler_diagnostic_fix.CS0539.enabled = true
roslynator.compiler_diagnostic_fix.CS0541.enabled = true
roslynator.compiler_diagnostic_fix.CS0549.enabled = true
roslynator.compiler_diagnostic_fix.CS0558.enabled = true
roslynator.compiler_diagnostic_fix.CS0567.enabled = true
roslynator.compiler_diagnostic_fix.CS0568.enabled = true
roslynator.compiler_diagnostic_fix.CS0573.enabled = true
roslynator.compiler_diagnostic_fix.CS0574.enabled = true
roslynator.compiler_diagnostic_fix.CS0575.enabled = true
roslynator.compiler_diagnostic_fix.CS0579.enabled = true
roslynator.compiler_diagnostic_fix.CS0592.enabled = true
roslynator.compiler_diagnostic_fix.CS0621.enabled = true
roslynator.compiler_diagnostic_fix.CS0628.enabled = true
roslynator.compiler_diagnostic_fix.CS0659.enabled = true
roslynator.compiler_diagnostic_fix.CS0660.enabled = true
roslynator.compiler_diagnostic_fix.CS0661.enabled = true
roslynator.compiler_diagnostic_fix.CS0678.enabled = true
roslynator.compiler_diagnostic_fix.CS0693.enabled = true
roslynator.compiler_diagnostic_fix.CS0708.enabled = true
roslynator.compiler_diagnostic_fix.CS0710.enabled = true
roslynator.compiler_diagnostic_fix.CS0713.enabled = true
roslynator.compiler_diagnostic_fix.CS0714.enabled = true
roslynator.compiler_diagnostic_fix.CS0718.enabled = true
roslynator.compiler_diagnostic_fix.CS0750.enabled = true
roslynator.compiler_diagnostic_fix.CS0751.enabled = true
roslynator.compiler_diagnostic_fix.CS0753.enabled = true
roslynator.compiler_diagnostic_fix.CS0756.enabled = true
roslynator.compiler_diagnostic_fix.CS0759.enabled = true
roslynator.compiler_diagnostic_fix.CS0766.enabled = true
roslynator.compiler_diagnostic_fix.CS0815.enabled = true
roslynator.compiler_diagnostic_fix.CS0819.enabled = true
roslynator.compiler_diagnostic_fix.CS0822.enabled = true
roslynator.compiler_diagnostic_fix.CS1002.enabled = true
roslynator.compiler_diagnostic_fix.CS1003.enabled = true
roslynator.compiler_diagnostic_fix.CS1004.enabled = true
roslynator.compiler_diagnostic_fix.CS1012.enabled = true
roslynator.compiler_diagnostic_fix.CS1023.enabled = true
roslynator.compiler_diagnostic_fix.CS1031.enabled = true
roslynator.compiler_diagnostic_fix.CS1057.enabled = true
roslynator.compiler_diagnostic_fix.CS1061.enabled = true
roslynator.compiler_diagnostic_fix.CS1100.enabled = true
roslynator.compiler_diagnostic_fix.CS1105.enabled = true
roslynator.compiler_diagnostic_fix.CS1106.enabled = true
roslynator.compiler_diagnostic_fix.CS1503.enabled = true
roslynator.compiler_diagnostic_fix.CS1522.enabled = true
roslynator.compiler_diagnostic_fix.CS1526.enabled = true
roslynator.compiler_diagnostic_fix.CS1527.enabled = true
roslynator.compiler_diagnostic_fix.CS1591.enabled = true
roslynator.compiler_diagnostic_fix.CS1597.enabled = true
roslynator.compiler_diagnostic_fix.CS1609.enabled = true
roslynator.compiler_diagnostic_fix.CS1615.enabled = true
roslynator.compiler_diagnostic_fix.CS1620.enabled = true
roslynator.compiler_diagnostic_fix.CS1621.enabled = true
roslynator.compiler_diagnostic_fix.CS1622.enabled = true
roslynator.compiler_diagnostic_fix.CS1623.enabled = true
roslynator.compiler_diagnostic_fix.CS1624.enabled = true
roslynator.compiler_diagnostic_fix.CS1643.enabled = true
roslynator.compiler_diagnostic_fix.CS1674.enabled = true
roslynator.compiler_diagnostic_fix.CS1689.enabled = true
roslynator.compiler_diagnostic_fix.CS1715.enabled = true
roslynator.compiler_diagnostic_fix.CS1717.enabled = true
roslynator.compiler_diagnostic_fix.CS1722.enabled = true
roslynator.compiler_diagnostic_fix.CS1737.enabled = true
roslynator.compiler_diagnostic_fix.CS1741.enabled = true
roslynator.compiler_diagnostic_fix.CS1743.enabled = true
roslynator.compiler_diagnostic_fix.CS1750.enabled = true
roslynator.compiler_diagnostic_fix.CS1751.enabled = true
roslynator.compiler_diagnostic_fix.CS1955.enabled = true
roslynator.compiler_diagnostic_fix.CS1983.enabled = true
roslynator.compiler_diagnostic_fix.CS1988.enabled = true
roslynator.compiler_diagnostic_fix.CS1994.enabled = true
roslynator.compiler_diagnostic_fix.CS1997.enabled = true
roslynator.compiler_diagnostic_fix.CS3000.enabled = true
roslynator.compiler_diagnostic_fix.CS3001.enabled = true
roslynator.compiler_diagnostic_fix.CS3002.enabled = true
roslynator.compiler_diagnostic_fix.CS3003.enabled = true
roslynator.compiler_diagnostic_fix.CS3005.enabled = true
roslynator.compiler_diagnostic_fix.CS3006.enabled = true
roslynator.compiler_diagnostic_fix.CS3007.enabled = true
roslynator.compiler_diagnostic_fix.CS3008.enabled = true
roslynator.compiler_diagnostic_fix.CS3009.enabled = true
roslynator.compiler_diagnostic_fix.CS3016.enabled = true
roslynator.compiler_diagnostic_fix.CS3024.enabled = true
roslynator.compiler_diagnostic_fix.CS3027.enabled = true
roslynator.compiler_diagnostic_fix.CS7036.enabled = true
roslynator.compiler_diagnostic_fix.CS8050.enabled = true
roslynator.compiler_diagnostic_fix.CS8070.enabled = true
roslynator.compiler_diagnostic_fix.CS8112.enabled = true
roslynator.compiler_diagnostic_fix.CS8139.enabled = true
roslynator.compiler_diagnostic_fix.CS8340.enabled = true
roslynator.compiler_diagnostic_fix.CS8403.enabled = true
roslynator.compiler_diagnostic_fix.CS8618.enabled = true
roslynator.compiler_diagnostic_fix.CS8625.enabled = true
roslynator.compiler_diagnostic_fix.CS8632.enabled = true
`
};