## Roslynator Analyzers by Category

 Category | Title | Id | Enabled by Default 
 --- | --- | --- |:---:
Design|Abstract type should not have public constructors|RCS1160|x
Design|Avoid empty catch clause that catches System\.Exception|RCS1075|x
Design|Avoid locking on publicly accessible instance|RCS1059|x
Design|Call 'ConfigureAwait\(false\)'|RCS1090|x
Design|Composite enum value contains undefined flag|RCS1157|x
Design|Declare enum member with zero value \(when enum has FlagsAttribute\)|RCS1135|x
Design|Declare type inside namespace|RCS1110|x
Design|Implement exception constructors|RCS1194|x
Design|Make class static|RCS1102|x
Design|Mark field as read\-only|RCS1169|x
Design|Overriding member cannot change 'params' modifier|RCS1193|x
Design|Static member in generic type should use a type parameter|RCS1158|x
Design|Use AttributeUsageAttribute|RCS1203|x
Design|Use constant instead of field|RCS1187|x
Design|Use read\-only auto\-implemented property|RCS1170|x
Formatting|Add empty line after closing brace|RCS1153|
Formatting|Add empty line after embedded statement|RCS1030|
Formatting|Add empty line after last statement in do statement|RCS1092|
Formatting|Add empty line between declarations|RCS1057|x
Formatting|Avoid single\-line block|RCS1185|
Formatting|Format accessor list|RCS1024|
Formatting|Format binary operator on next line|RCS1029|x
Formatting|Format conditional expression \(format ? and : on next line\)|RCS1184|
Formatting|Format declaration braces|RCS1076|x
Formatting|Format documentation summary on a single line|RCS1100|
Formatting|Format documentation summary on multiple lines|RCS1101|
Formatting|Format each enum member on a separate line|RCS1025|
Formatting|Format each statement on a separate line|RCS1026|
Formatting|Format embedded statement on a separate line|RCS1027|
Formatting|Format empty block|RCS1023|
Formatting|Format initializer with single expression on single line|RCS1183|x
Formatting|Format switch section's statement on a separate line|RCS1028|
General|Bitwise operation on enum without Flags attribute|RCS1130|x
General|Mark local variable as const|RCS1118|x
General|Replace comment with documentation comment|RCS1181|x
General|Throwing of new NotImplementedException|RCS1079|x
General|Use "" instead of string\.Empty|RCS1078|
General|Use carriage return \+ linefeed as newline|RCS1087|
General|Use linefeed as newline|RCS1086|
General|Use space\(s\) instead of tab|RCS1088|
Maintainability|Add exception to documentation comment|RCS1140|x
Maintainability|Add parameter to documentation comment|RCS1141|x
Maintainability|Add summary element to documentation comment|RCS1139|x
Maintainability|Add summary to documentation comment|RCS1138|x
Maintainability|Add type parameter to documentation comment|RCS1142|x
Maintainability|Declare each type in separate file|RCS1060|
Maintainability|Parameter name differs from base name|RCS1168|x
Maintainability|Remove original exception from throw statement|RCS1044|x
Maintainability|Use nameof operator|RCS1015|x
Naming|Asynchronous method name should end with 'Async'|RCS1046|
Naming|Non\-asynchronous method name should not end with 'Async'|RCS1047|x
Naming|Rename private field according to camel case with underscore|RCS1045|
Performance|Avoid unnecessary boxing of value type|RCS1198|
Performance|Call 'Find' instead of 'FirstOrDefault'|RCS1119|x
Performance|Optimize StringBuilder\.Append/AppendLine call|RCS1197|x
Performance|Use \[\] instead of calling 'ElementAt'|RCS1120|x
Performance|Use \[\] instead of calling 'First'|RCS1121|x
Performance|Use 'Any' method instead of 'Count' method|RCS1083|x
Performance|Use bitwise operation instead of calling 'HasFlag'|RCS1096|x
Performance|Use 'Count/Length' property instead of 'Any' method|RCS1080|x
Performance|Use 'Count/Length' property instead of 'Count' method|RCS1082|x
Readability|Add default access modifier|RCS1018|x
Readability|Add or remove region name|RCS1189|x
Readability|Add parentheses according to operator precedence|RCS1123|x
Readability|Add 'static' modifier to all partial class declarations|RCS1108|x
Readability|Avoid chain of assignments|RCS1162|
Readability|Avoid implicitly\-typed array|RCS1014|
Readability|Avoid 'null' on the left side of a binary expression|RCS1098|x
Readability|Avoid usage of using alias directive|RCS1056|
Readability|Declare each attribute separately|RCS1052|
Readability|Declare enum value as combination of names|RCS1191|x
Readability|Declare using directive on top level|RCS1094|
Readability|Default label should be last label in switch section|RCS1099|x
Readability|Enum member should declare explicit value|RCS1161|x
Readability|Reorder modifiers|RCS1019|
Readability|Reorder named arguments according to the order of parameters|RCS1205|x
Readability|Sort enum members|RCS1154|x
Readability|Split variable declaration|RCS1081|
Readability|Use explicit type instead of 'var' \(foreach variable\)|RCS1009|x
Readability|Use explicit type instead of 'var' \(when the type is not obvious\)|RCS1008|x
Readability|Use explicit type instead of 'var' \(when the type is obvious\)|RCS1012|
Readability|Use regular string literal instead of verbatim string literal|RCS1192|x
Redundancy|Avoid interpolated string with no interpolation|RCS1062|x
Redundancy|Avoid semicolon at the end of declaration|RCS1055|x
Redundancy|Remove empty attribute argument list|RCS1039|x
Redundancy|Remove empty destructor|RCS1106|x
Redundancy|Remove empty else clause|RCS1040|
Redundancy|Remove empty finally clause|RCS1066|x
Redundancy|Remove empty initializer|RCS1041|x
Redundancy|Remove empty namespace declaration|RCS1072|x
Redundancy|Remove empty region|RCS1091|x
Redundancy|Remove empty statement|RCS1038|x
Redundancy|Remove enum default underlying type|RCS1042|x
Redundancy|Remove file with no code|RCS1093|x
Redundancy|Remove 'partial' modifier from type with a single part|RCS1043|x
Redundancy|Remove redundant 'as' operator|RCS1145|x
Redundancy|Remove redundant async/await|RCS1174|x
Redundancy|Remove redundant auto\-property initialization|RCS1188|x
Redundancy|Remove redundant base constructor call|RCS1071|x
Redundancy|Remove redundant base interface|RCS1182|x
Redundancy|Remove redundant boolean literal|RCS1033|x
Redundancy|Remove redundant cast|RCS1151|x
Redundancy|Remove redundant comma in initializer|RCS1035|x
Redundancy|Remove redundant constructor|RCS1074|x
Redundancy|Remove redundant default switch section|RCS1070|x
Redundancy|Remove redundant delegate creation|RCS1114|x
Redundancy|Remove redundant Dispose/Close call|RCS1133|x
Redundancy|Remove redundant empty line|RCS1036|x
Redundancy|Remove redundant field initalization|RCS1129|x
Redundancy|Remove redundant overriding member|RCS1132|x
Redundancy|Remove redundant parentheses|RCS1032|x
Redundancy|Remove redundant 'sealed' modifier|RCS1034|x
Redundancy|Remove redundant statement|RCS1134|x
Redundancy|Remove redundant 'ToCharArray' call|RCS1107|x
Redundancy|Remove redundant 'ToString' call|RCS1097|x
Redundancy|Remove trailing white\-space|RCS1037|x
Redundancy|Remove unnecessary case label|RCS1069|x
Redundancy|Unused parameter|RCS1163|x
Redundancy|Unused this parameter|RCS1175|x
Redundancy|Unused type parameter|RCS1164|x
Simplification|Call 'Enumerable\.Cast' instead of 'Enumerable\.Select'|RCS1109|x
Simplification|Call string\.Concat instead of string\.Join|RCS1150|x
Simplification|Combine 'Enumerable\.Where' method chain|RCS1112|x
Simplification|Inline lazy initialization|RCS1180|x
Simplification|Inline local variable|RCS1124|x
Simplification|Join string expressions|RCS1190|x
Simplification|Merge else clause with nested if statement|RCS1006|x
Simplification|Merge if statement with nested if statement|RCS1061|x
Simplification|Merge interpolation into interpolated string|RCS1105|x
Simplification|Merge local declaration with assignment|RCS1127|x
Simplification|Merge switch sections with equivalent content|RCS1136|x
Simplification|Replace if statement with assignment|RCS1103|x
Simplification|Replace if statement with return statement|RCS1073|
Simplification|Simplify boolean comparison|RCS1049|x
Simplification|Simplify boolean expression|RCS1199|x
Simplification|Simplify coalesce expression|RCS1143|x
Simplification|Simplify conditional expression|RCS1104|x
Simplification|Simplify lambda expression parameter list|RCS1022|
Simplification|Simplify lambda expression|RCS1021|x
Simplification|Simplify lazily initialized property|RCS1171|x
Simplification|Simplify LINQ method chain|RCS1077|x
Simplification|Simplify logical not expression|RCS1068|x
Simplification|Simplify nested using statement|RCS1005|x
Simplification|Simplify Nullable\<T\> to T?|RCS1020|x
Simplification|Use \-\-/\+\+ operator instead of assignment|RCS1089|x
Simplification|Use ^ operator|RCS1195|x
Simplification|Use auto\-implemented property|RCS1085|x
Simplification|Use coalesce expression instead of conditional expression|RCS1084|x
Simplification|Use coalesce expression instead of if|RCS1173|x
Simplification|Use coalesce expression|RCS1128|x
Simplification|Use compound assignment|RCS1058|x
Simplification|Use is operator instead of as operator|RCS1172|x
Simplification|Use method chaining|RCS1201|x
Simplification|Use method group instead of anonymous function|RCS1207|x
Simplification|Use return instead of assignment|RCS1179|x
Simplification|Use 'var' instead of explicit type \(in foreach\)|RCS1177|
Simplification|Use 'var' instead of explicit type \(when the type is not obvious\)|RCS1176|
Simplification|Use 'var' instead of explicit type \(when the type is obvious\)|RCS1010|x
Style|Add braces to if\-else|RCS1003|x
Style|Add braces to switch section with multiple statements|RCS1111|
Style|Add braces|RCS1001|x
Style|Add constructor argument list|RCS1050|
Style|Avoid embedded statement in if\-else|RCS1126|
Style|Avoid embedded statement|RCS1007|
Style|Avoid multiline expression body|RCS1017|
Style|Avoid usage of do statement to create an infinite loop|RCS1063|x
Style|Avoid usage of for statement to create an infinite loop|RCS1064|
Style|Avoid usage of while statement to create an inifinite loop|RCS1065|
Style|Call extension method as instance method|RCS1196|x
Style|Parenthesize condition in conditional expression|RCS1051|
Style|Remove braces from if\-else|RCS1004|
Style|Remove braces|RCS1002|
Style|Remove empty argument list|RCS1067|
Usage|Avoid NullReferenceException|RCS1202|x
Usage|Call Debug\.Fail instead of Debug\.Assert|RCS1178|x
Usage|Call 'Enumerable\.ThenBy' instead of 'Enumerable\.OrderBy'|RCS1200|x
Usage|Unconstrained type parameter checked for null|RCS1165|x
Usage|Use C\# 6\.0 dictionary initializer|RCS1095|x
Usage|Use conditional access instead of conditional expression|RCS1206|x
Usage|Use conditional access|RCS1146|x
Usage|Use EventArgs\.Empty|RCS1204|x
Usage|Use EventHandler\<T\>|RCS1159|x
Usage|Use expression\-bodied member|RCS1016|
Usage|Use lambda expression instead of anonymous method|RCS1048|x
Usage|Use predefined type|RCS1013|
Usage|Use Regex instance instead of static method|RCS1186|x
Usage|Use 'string\.IsNullOrEmpty' method|RCS1113|x
Usage|Use string\.Length instead of comparison with empty string|RCS1156|x
Usage|Use StringComparison when comparing strings|RCS1155|x
Usage|Value type object is never equal to null|RCS1166|x
