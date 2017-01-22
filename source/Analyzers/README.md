## Analyzers

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
RCS1012|Use explicit type instead of 'var' \(even if the type is obvious\)|Readability|
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
RCS1023|Format empty block|Formatting|x
RCS1024|Format accessor list|Formatting|x
RCS1025|Format each enum member on a separate line|Formatting|
RCS1026|Format each statement on a separate line|Formatting|
RCS1027|Format embedded statement on a separate line|Formatting|
RCS1028|Format switch section's statement on a separate line|Formatting|
RCS1029|Format binary operator on next line|Formatting|x
RCS1030|Add empty line after embedded statement|Formatting|
RCS1031|Remove redundant braces|Redundancy|x
RCS1032|Remove redundant parentheses|Redundancy|x
RCS1033|Remove redundant boolean literal|Redundancy|x
RCS1034|Remove redundant sealed modifier|Redundancy|x
RCS1035|Remove redundant comma in initializer|Redundancy|x
RCS1036|Remove redundant empty line|Redundancy|x
RCS1037|Remove trailing white\-space|Redundancy|x
RCS1038|Remove empty statement|Redundancy|x
RCS1039|Remove empty attribute argument list|Redundancy|x
RCS1040|Remove empty else clause|Redundancy|
RCS1041|Remove empty initializer|Redundancy|x
RCS1042|Remove enum default underlying type|Redundancy|x
RCS1043|Remove partial modifier from type with a single part|Redundancy|x
RCS1044|Remove original exception from throw statement|Maintainability|x
RCS1045|Rename private field according to camel case with underscore|Naming|
RCS1046|Asynchronous method name should end with 'Async'|Naming|x
RCS1047|Non\-asynchronous method name should not end with 'Async'|Naming|x
RCS1048|Replace anonymous method with lambda expression|Usage|x
RCS1049|Simplify boolean comparison|Simplification|x
RCS1050|Add constructor argument list|Style|
RCS1051|Parenthesize condition in conditional expression|Style|
RCS1052|Declare each attribute separately|Readability|
RCS1054|Merge local declaration with return statement|Simplification|x
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
RCS1078|Replace string\.Empty with ""|General|
RCS1079|Throwing of new NotImplementedException|General|x
RCS1080|Replace 'Any' method with 'Count' or 'Length' property|Performance|x
RCS1081|Split variable declaration|Readability|
RCS1082|Replace 'Count' method with 'Count' or 'Length' property|Performance|x
RCS1083|Replace 'Count' method with 'Any' method|Performance|x
RCS1084|Replace conditional expression with coalesce expression|Simplification|x
RCS1085|Replace property with auto\-implemented property|Simplification|x
RCS1086|Use linefeed as newline|General|
RCS1087|Use carriage return \+ linefeed as newline|General|
RCS1088|Avoid usage of tab|General|
RCS1089|Use postfix unary operator instead of assignment|Simplification|x
RCS1090|Call 'ConfigureAwait\(false\)'|Design|x
RCS1091|Remove empty region|Redundancy|x
RCS1092|Add empty line after last statement in do statement|Formatting|
RCS1093|Remove file with no code|Redundancy|x
RCS1094|Declare using directive on top level|Readability|
RCS1095|Use C\# 6\.0 dictionary initializer|Usage|x
RCS1096|Use bitwise operation instead of 'HasFlag' method|Performance|x
RCS1097|Remove redundant 'ToString' call|Redundancy|x
RCS1098|Avoid 'null' on the left side of a binary expression|Readability|x
RCS1099|Default label should be last label in switch section|Readability|x
RCS1100|Format documentation summary on a single line|Formatting|
RCS1101|Format documentation summary on multiple lines|Formatting|
RCS1102|Mark class as static|Design|x
RCS1103|Replace if statement with assignment|Simplification|x
RCS1104|Simplify conditional expression|Simplification|x
RCS1105|Merge interpolation into interpolated string|Simplification|x
RCS1106|Remove empty destructor|Redundancy|x
RCS1107|Remove redundant 'ToCharArray' call|Redundancy|x
RCS1108|Add static modifier to all partial class declarations|Readability|x
RCS1109|Use 'Cast' method instead of 'Select' method|Simplification|x
RCS1110|Declare type inside namespace|Design|x
RCS1111|Add braces to switch section with multiple statements|Style|
RCS1112|Combine 'Enumerable\.Where' method chain|Simplification|x
RCS1113|Use 'string\.IsNullOrEmpty' method|Usage|x
RCS1114|Remove redundant delegate creation|Redundancy|x
RCS1115|Replace yield/return statement with expression statement|ErrorFix|x
RCS1116|Add break statement to switch section|ErrorFix|x
RCS1117|Add return statement that returns default value|ErrorFix|x
RCS1118|Mark local variable as const|General|x
RCS1119|Call 'Find' method instead of 'FirstOrDefault' method|Performance|x
RCS1120|Use \[\] instead of calling 'ElementAt'|Performance|x
RCS1121|Use \[\] instead of calling 'First'|Performance|x
RCS1122|Add missing semicolon|ErrorFix|x
RCS1123|Add parentheses according to operator precedence|Readability|x
RCS1124|Inline local variable|Simplification|x
RCS1125|Mark member as static|ErrorFix|x
RCS1126|Avoid embedded statement in if\-else|Style|
RCS1127|Merge local declaration with initialization|Simplification|x
RCS1128|Use coalesce expression|Simplification|x
RCS1129|Remove redundant field initalization|Redundancy|x
