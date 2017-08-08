## Roslynator Analyzers

 Id | Title | Category | Enabled by Default 
 --- | --- | --- |:---:
RCS1001|Add braces|Style|x
RCS1002|Remove braces|Style|
RCS1003|Add braces to if\-else|Style|x
RCS1004|Remove braces from if\-else|Style|
RCS1005|Simplify nested using statement|Simplification|x
RCS1006|Merge else clause with nested if statement|Simplification|x
RCS1007|Avoid embedded statement|Style|
RCS1008|Use explicit type instead of 'var' \(when the type is not obvious\)|Readability|x
RCS1009|Use explicit type instead of 'var' \(foreach variable\)|Readability|x
RCS1010|Use 'var' instead of explicit type \(when the type is obvious\)|Simplification|x
RCS1012|Use explicit type instead of 'var' \(when the type is obvious\)|Readability|
RCS1013|Use predefined type|Usage|
RCS1014|Avoid implicitly\-typed array|Readability|
RCS1015|Use nameof operator|Maintainability|x
RCS1016|Use expression\-bodied member|Usage|
RCS1017|Avoid multiline expression body|Style|
RCS1018|Add default access modifier|Readability|x
RCS1019|Reorder modifiers|Readability|
RCS1020|Simplify Nullable\<T\> to T?|Simplification|x
RCS1021|Simplify lambda expression|Simplification|x
RCS1022|Simplify lambda expression parameter list|Simplification|
RCS1023|Format empty block|Formatting|
RCS1024|Format accessor list|Formatting|
RCS1025|Format each enum member on a separate line|Formatting|
RCS1026|Format each statement on a separate line|Formatting|
RCS1027|Format embedded statement on a separate line|Formatting|
RCS1028|Format switch section's statement on a separate line|Formatting|
RCS1029|Format binary operator on next line|Formatting|x
RCS1030|Add empty line after embedded statement|Formatting|
RCS1032|Remove redundant parentheses|Redundancy|x
RCS1033|Remove redundant boolean literal|Redundancy|x
RCS1034|Remove redundant 'sealed' modifier|Redundancy|x
RCS1035|Remove redundant comma in initializer|Redundancy|x
RCS1036|Remove redundant empty line|Redundancy|x
RCS1037|Remove trailing white\-space|Redundancy|x
RCS1038|Remove empty statement|Redundancy|x
RCS1039|Remove empty attribute argument list|Redundancy|x
RCS1040|Remove empty else clause|Redundancy|
RCS1041|Remove empty initializer|Redundancy|x
RCS1042|Remove enum default underlying type|Redundancy|x
RCS1043|Remove 'partial' modifier from type with a single part|Redundancy|x
RCS1044|Remove original exception from throw statement|Maintainability|x
RCS1045|Rename private field according to camel case with underscore|Naming|
RCS1046|Asynchronous method name should end with 'Async'|Naming|
RCS1047|Non\-asynchronous method name should not end with 'Async'|Naming|x
RCS1048|Use lambda expression instead of anonymous method|Usage|x
RCS1049|Simplify boolean comparison|Simplification|x
RCS1050|Add constructor argument list|Style|
RCS1051|Parenthesize condition in conditional expression|Style|
RCS1052|Declare each attribute separately|Readability|
RCS1055|Avoid semicolon at the end of declaration|Redundancy|x
RCS1056|Avoid usage of using alias directive|Readability|
RCS1057|Add empty line between declarations|Formatting|x
RCS1058|Use compound assignment|Simplification|x
RCS1059|Avoid locking on publicly accessible instance|Design|x
RCS1060|Declare each type in separate file|Maintainability|
RCS1061|Merge if statement with nested if statement|Simplification|x
RCS1062|Avoid interpolated string with no interpolation|Redundancy|x
RCS1063|Avoid usage of do statement to create an infinite loop|Style|x
RCS1064|Avoid usage of for statement to create an infinite loop|Style|
RCS1065|Avoid usage of while statement to create an inifinite loop|Style|
RCS1066|Remove empty finally clause|Redundancy|x
RCS1067|Remove empty argument list|Style|
RCS1068|Simplify logical not expression|Simplification|x
RCS1069|Remove unnecessary case label|Redundancy|x
RCS1070|Remove redundant default switch section|Redundancy|x
RCS1071|Remove redundant base constructor call|Redundancy|x
RCS1072|Remove empty namespace declaration|Redundancy|x
RCS1073|Replace if statement with return statement|Simplification|
RCS1074|Remove redundant constructor|Redundancy|x
RCS1075|Avoid empty catch clause that catches System\.Exception|Design|x
RCS1076|Format declaration braces|Formatting|x
RCS1077|Simplify LINQ method chain|Simplification|x
RCS1078|Use "" instead of string\.Empty|General|
RCS1079|Throwing of new NotImplementedException|General|x
RCS1080|Use 'Count/Length' property instead of 'Any' method|Performance|x
RCS1081|Split variable declaration|Readability|
RCS1082|Use 'Count/Length' property instead of 'Count' method|Performance|x
RCS1083|Use 'Any' method instead of 'Count' method|Performance|x
RCS1084|Use coalesce expression instead of conditional expression|Simplification|x
RCS1085|Use auto\-implemented property|Simplification|x
RCS1086|Use linefeed as newline|General|
RCS1087|Use carriage return \+ linefeed as newline|General|
RCS1088|Use space\(s\) instead of tab|General|
RCS1089|Use \-\-/\+\+ operator instead of assignment|Simplification|x
RCS1090|Call 'ConfigureAwait\(false\)'|Design|x
RCS1091|Remove empty region|Redundancy|x
RCS1092|Add empty line after last statement in do statement|Formatting|
RCS1093|Remove file with no code|Redundancy|x
RCS1094|Declare using directive on top level|Readability|
RCS1095|Use C\# 6\.0 dictionary initializer|Usage|x
RCS1096|Use bitwise operation instead of calling 'HasFlag'|Performance|x
RCS1097|Remove redundant 'ToString' call|Redundancy|x
RCS1098|Avoid 'null' on the left side of a binary expression|Readability|x
RCS1099|Default label should be last label in switch section|Readability|x
RCS1100|Format documentation summary on a single line|Formatting|
RCS1101|Format documentation summary on multiple lines|Formatting|
RCS1102|Make class static|Design|x
RCS1103|Replace if statement with assignment|Simplification|x
RCS1104|Simplify conditional expression|Simplification|x
RCS1105|Merge interpolation into interpolated string|Simplification|x
RCS1106|Remove empty destructor|Redundancy|x
RCS1107|Remove redundant 'ToCharArray' call|Redundancy|x
RCS1108|Add 'static' modifier to all partial class declarations|Readability|x
RCS1109|Call 'Enumerable\.Cast' instead of 'Enumerable\.Select'|Simplification|x
RCS1110|Declare type inside namespace|Design|x
RCS1111|Add braces to switch section with multiple statements|Style|
RCS1112|Combine 'Enumerable\.Where' method chain|Simplification|x
RCS1113|Use 'string\.IsNullOrEmpty' method|Usage|x
RCS1114|Remove redundant delegate creation|Redundancy|x
RCS1118|Mark local variable as const|General|x
RCS1119|Call 'Find' instead of 'FirstOrDefault'|Performance|x
RCS1120|Use \[\] instead of calling 'ElementAt'|Performance|x
RCS1121|Use \[\] instead of calling 'First'|Performance|x
RCS1123|Add parentheses according to operator precedence|Readability|x
RCS1124|Inline local variable|Simplification|x
RCS1126|Avoid embedded statement in if\-else|Style|
RCS1127|Merge local declaration with assignment|Simplification|x
RCS1128|Use coalesce expression|Simplification|x
RCS1129|Remove redundant field initalization|Redundancy|x
RCS1130|Bitwise operation on enum without Flags attribute|General|x
RCS1132|Remove redundant overriding member|Redundancy|x
RCS1133|Remove redundant Dispose/Close call|Redundancy|x
RCS1134|Remove redundant statement|Redundancy|x
RCS1135|Declare enum member with zero value \(when enum has FlagsAttribute\)|Design|x
RCS1136|Merge switch sections with equivalent content|Simplification|x
RCS1138|Add summary to documentation comment|Maintainability|x
RCS1139|Add summary element to documentation comment|Maintainability|x
RCS1140|Add exception to documentation comment|Maintainability|x
RCS1141|Add parameter to documentation comment|Maintainability|x
RCS1142|Add type parameter to documentation comment|Maintainability|x
RCS1143|Simplify coalesce expression|Simplification|x
RCS1145|Remove redundant 'as' operator|Redundancy|x
RCS1146|Use conditional access|Usage|x
RCS1150|Call string\.Concat instead of string\.Join|Simplification|x
RCS1151|Remove redundant cast|Redundancy|x
RCS1153|Add empty line after closing brace|Formatting|
RCS1154|Sort enum members|Readability|x
RCS1155|Use StringComparison when comparing strings|Usage|x
RCS1156|Use string\.Length instead of comparison with empty string|Usage|x
RCS1157|Composite enum value contains undefined flag|Design|x
RCS1158|Static member in generic type should use a type parameter|Design|x
RCS1159|Use EventHandler\<T\>|Usage|x
RCS1160|Abstract type should not have public constructors|Design|x
RCS1161|Enum member should declare explicit value|Readability|x
RCS1162|Avoid chain of assignments|Readability|
RCS1163|Unused parameter|Redundancy|x
RCS1164|Unused type parameter|Redundancy|x
RCS1165|Unconstrained type parameter checked for null|Usage|x
RCS1166|Value type object is never equal to null|Usage|x
RCS1168|Parameter name differs from base name|Maintainability|x
RCS1169|Mark field as read\-only|Design|x
RCS1170|Use read\-only auto\-implemented property|Design|x
RCS1171|Simplify lazily initialized property|Simplification|x
RCS1172|Use is operator instead of as operator|Simplification|x
RCS1173|Use coalesce expression instead of if|Simplification|x
RCS1174|Remove redundant async/await|Redundancy|x
RCS1175|Unused this parameter|Redundancy|x
RCS1176|Use 'var' instead of explicit type \(when the type is not obvious\)|Simplification|
RCS1177|Use 'var' instead of explicit type \(in foreach\)|Simplification|
RCS1178|Call Debug\.Fail instead of Debug\.Assert|Usage|x
RCS1179|Use return instead of assignment|Simplification|x
RCS1180|Inline lazy initialization|Simplification|x
RCS1181|Replace comment with documentation comment|General|x
RCS1182|Remove redundant base interface|Redundancy|x
RCS1183|Format initializer with single expression on single line|Formatting|x
RCS1184|Format conditional expression \(format ? and : on next line\)|Formatting|
RCS1185|Avoid single\-line block|Formatting|
RCS1186|Use Regex instance instead of static method|Usage|x
RCS1187|Use constant instead of field|Design|x
RCS1188|Remove redundant auto\-property initialization|Redundancy|x
RCS1189|Add or remove region name|Readability|x
RCS1190|Join string expressions|Simplification|x
RCS1191|Declare enum value as combination of names|Readability|x
RCS1192|Use regular string literal instead of verbatim string literal|Readability|x
RCS1193|Overriding member cannot change 'params' modifier|Design|x
RCS1194|Implement exception constructors|Design|x
RCS1195|Use ^ operator|Simplification|x
RCS1196|Call extension method as instance method|Style|x
RCS1197|Optimize StringBuilder\.Append/AppendLine call|Performance|x
RCS1198|Avoid unnecessary boxing of value type|Performance|
RCS1199|Simplify boolean expression|Simplification|x
RCS1200|Call 'Enumerable\.ThenBy' instead of 'Enumerable\.OrderBy'|Usage|x
RCS1201|Use method chaining|Simplification|x
RCS1202|Avoid NullReferenceException|Usage|x
RCS1203|Use AttributeUsageAttribute|Design|x
RCS1204|Use EventArgs\.Empty|Usage|x
RCS1205|Reorder named arguments according to the order of parameters|Readability|x
RCS1206|Use conditional access instead of conditional expression|Usage|x
RCS1207|Use method group instead of anonymous function|Simplification|x
