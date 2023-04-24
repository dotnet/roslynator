## Roslynator Analyzers

### Overview

| Package | Prefix | Comment |
| ------- | ------ | ------- |
| [Roslynator.Analyzers](https://www.nuget.org/packages/Roslynator.Analyzers) | `RCS1` | common analyzers |
| [Roslynator.Formatting.Analyzers](https://www.nuget.org/packages/Roslynator.Formatting.Analyzers) | `RCS0` | \- |
| [Roslynator.CodeAnalysis.Analyzers](https://www.nuget.org/packages/Roslynator.CodeAnalysis.Analyzers) | `RCS9` | suitable for projects that reference Roslyn packages (`Microsoft.CodeAnalysis*`) |

### List of Analyzers

| Id  | Title | Severity |
| --- | ----- | -------- |
| RCS0001 | [Add blank line after embedded statement](RCS0001.md) | None |
| RCS0002 | [Add blank line after #region](RCS0002.md) | None |
| RCS0003 | [Add blank line after using directive list](RCS0003.md) | None |
| RCS0005 | [Add blank line before #endregion](RCS0005.md) | None |
| RCS0006 | [Add blank line before using directive list](RCS0006.md) | None |
| RCS0007 | [Add blank line between accessors](RCS0007.md) | None |
| RCS0008 | [Add blank line between closing brace and next statement](RCS0008.md) | None |
| RCS0009 | [Add blank line between declaration and documentation comment](RCS0009.md) | None |
| RCS0010 | [Add blank line between declarations](RCS0010.md) | None |
| RCS0011 | [Add/remove blank line between single-line accessors](RCS0011.md) | None |
| RCS0012 | [Add blank line between single-line declarations](RCS0012.md) | None |
| RCS0013 | [Add blank line between single-line declarations of different kind](RCS0013.md) | None |
| RCS0014 | [Add blank line between switch sections](RCS0014.md) | None |
| RCS0015 | [Add/remove blank line between using directives](RCS0015.md) | None |
| RCS0016 | [Put attribute list on its own line](RCS0016.md) | None |
| RCS0020 | [Format accessor's braces on a single line or multiple lines](RCS0020.md) | None |
| RCS0021 | [Format block's braces on a single line or multiple lines](RCS0021.md) | None |
| RCS0022 | [Add new line after opening brace of empty block](RCS0022.md) | None |
| RCS0023 | [Format type declaration's braces](RCS0023.md) | None |
| RCS0024 | [Add new line after switch label](RCS0024.md) | None |
| RCS0025 | [Put full accessor on its own line](RCS0025.md) | None |
| RCS0027 | [Place new line after/before binary operator](RCS0027.md) | None |
| RCS0028 | [Place new line after/before '?:' operator](RCS0028.md) | None |
| RCS0029 | [Put constructor initializer on its own line](RCS0029.md) | None |
| RCS0030 | [Add new line before embedded statement](RCS0030.md) | None |
| RCS0031 | [Put enum member on its own line](RCS0031.md) | None |
| RCS0032 | [Place new line after/before arrow token](RCS0032.md) | None |
| RCS0033 | [Add new line before statement](RCS0033.md) | None |
| RCS0034 | [Put type parameter constraint on its own line](RCS0034.md) | None |
| RCS0036 | [Remove blank line between single-line declarations of same kind](RCS0036.md) | None |
| RCS0038 | [Remove blank line between using directives with same root namespace](RCS0038.md) | None |
| RCS0039 | [Remove new line before base list](RCS0039.md) | None |
| RCS0041 | [Remove new line between 'if' keyword and 'else' keyword](RCS0041.md) | None |
| RCS0042 | [Put auto-accessors on a single line](RCS0042.md) | None |
| RCS0043 | [Format accessor's braces on a single line when expression is on single line](RCS0043.md) | None |
| RCS0044 | [Use carriage return + linefeed as new line](RCS0044.md) | None |
| RCS0045 | [Use linefeed as new line](RCS0045.md) | None |
| RCS0046 | [Use spaces instead of tab](RCS0046.md) | None |
| RCS0047 | [\[deprecated\] Wrap and indent each node in list](RCS0047.md) | None |
| RCS0048 | [Put initializer on a single line](RCS0048.md) | None |
| RCS0049 | [Add blank line after top comment](RCS0049.md) | None |
| RCS0050 | [Add blank line before top declaration](RCS0050.md) | None |
| RCS0051 | [Add/remove new line before 'while' in 'do' statement](RCS0051.md) | None |
| RCS0052 | [Place new line after/before equals token](RCS0052.md) | None |
| RCS0053 | [Fix formatting of a list](RCS0053.md) | None |
| RCS0054 | [Fix formatting of a call chain](RCS0054.md) | None |
| RCS0055 | [Fix formatting of a binary expression chain](RCS0055.md) | None |
| RCS0056 | [A line is too long](RCS0056.md) | None |
| RCS0057 | [Normalize whitespace at the beginning of a file](RCS0057.md) | None |
| RCS0058 | [Normalize whitespace at the end of a file](RCS0058.md) | None |
| RCS0059 | [Place new line after/before null-conditional operator](RCS0059.md) | None |
| RCS0060 | [Add/remove line after file scoped namespace declaration](RCS0060.md) | None |
| RCS1001 | [Add braces (when expression spans over multiple lines)](RCS1001.md) | Info |
| RCS1002 | [Remove braces](RCS1002.md) | None |
| RCS1003 | [Add braces to if-else (when expression spans over multiple lines)](RCS1003.md) | Info |
| RCS1004 | [Remove braces from if-else](RCS1004.md) | None |
| RCS1005 | [Simplify nested using statement](RCS1005.md) | Hidden |
| RCS1006 | [Merge 'else' with nested 'if'](RCS1006.md) | Hidden |
| RCS1007 | [Add braces](RCS1007.md) | None |
| RCS1008 | [Use explicit type instead of 'var' (when the type is not obvious)](RCS1008.md) | None |
| RCS1009 | [Use explicit type instead of 'var' (foreach variable)](RCS1009.md) | None |
| RCS1010 | [Use 'var' instead of explicit type (when the type is obvious)](RCS1010.md) | None |
| RCS1012 | [Use explicit type instead of 'var' (when the type is obvious)](RCS1012.md) | None |
| RCS1013 | [Use predefined type](RCS1013.md) | None |
| RCS1014 | [Use explicitly/implicitly typed array](RCS1014.md) | None |
| RCS1015 | [Use nameof operator](RCS1015.md) | Info |
| RCS1016 | [Use block body or expression body](RCS1016.md) | None |
| RCS1018 | [Add/remove accessibility modifiers](RCS1018.md) | None |
| RCS1019 | [Order modifiers](RCS1019.md) | None |
| RCS1020 | [Simplify Nullable\<T> to T?](RCS1020.md) | Info |
| RCS1021 | [Convert lambda expression body to expression body](RCS1021.md) | Info |
| RCS1031 | [Remove unnecessary braces in switch section](RCS1031.md) | Hidden |
| RCS1032 | [Remove redundant parentheses](RCS1032.md) | Info |
| RCS1033 | [Remove redundant boolean literal](RCS1033.md) | Info |
| RCS1034 | [Remove redundant 'sealed' modifier](RCS1034.md) | Hidden |
| RCS1035 | [Remove redundant comma in initializer](RCS1035.md) | None |
| RCS1036 | [Remove unnecessary blank line](RCS1036.md) | Info |
| RCS1037 | [Remove trailing white-space](RCS1037.md) | Info |
| RCS1038 | [Remove empty statement](RCS1038.md) | Info |
| RCS1039 | [Remove argument list from attribute](RCS1039.md) | Hidden |
| RCS1040 | [Remove empty 'else' clause](RCS1040.md) | Hidden |
| RCS1041 | [Remove empty initializer](RCS1041.md) | Info |
| RCS1042 | [Remove enum default underlying type](RCS1042.md) | Hidden |
| RCS1043 | [Remove 'partial' modifier from type with a single part](RCS1043.md) | Hidden |
| RCS1044 | [Remove original exception from throw statement](RCS1044.md) | Warning |
| RCS1046 | [Asynchronous method name should end with 'Async'](RCS1046.md) | None |
| RCS1047 | [Non-asynchronous method name should not end with 'Async'](RCS1047.md) | Info |
| RCS1048 | [Use lambda expression instead of anonymous method](RCS1048.md) | Info |
| RCS1049 | [Simplify boolean comparison](RCS1049.md) | Info |
| RCS1050 | [Include/omit parentheses when creating new object](RCS1050.md) | None |
| RCS1051 | [Add/remove parentheses from condition in conditional operator](RCS1051.md) | None |
| RCS1052 | [Declare each attribute separately](RCS1052.md) | None |
| RCS1055 | [Avoid semicolon at the end of declaration](RCS1055.md) | Hidden |
| RCS1056 | [Avoid usage of using alias directive](RCS1056.md) | None |
| RCS1058 | [Use compound assignment](RCS1058.md) | Info |
| RCS1059 | [Avoid locking on publicly accessible instance](RCS1059.md) | Warning |
| RCS1060 | [Declare each type in separate file](RCS1060.md) | None |
| RCS1061 | [Merge 'if' with nested 'if'](RCS1061.md) | Hidden |
| RCS1063 | [(\[deprecated\] use RCS1252 instead) Avoid usage of do statement to create an infinite loop](RCS1063.md) | Info |
| RCS1064 | [(\[deprecated\] use RCS1252 instead) Avoid usage of for statement to create an infinite loop](RCS1064.md) | None |
| RCS1065 | [(\[deprecated\] use RCS1252 instead) Avoid usage of while statement to create an infinite loop](RCS1065.md) | None |
| RCS1066 | [Remove empty 'finally' clause](RCS1066.md) | Hidden |
| RCS1068 | [Simplify logical negation](RCS1068.md) | Info |
| RCS1069 | [Remove unnecessary case label](RCS1069.md) | Hidden |
| RCS1070 | [Remove redundant default switch section](RCS1070.md) | Hidden |
| RCS1071 | [Remove redundant base constructor call](RCS1071.md) | Hidden |
| RCS1072 | [Remove empty namespace declaration](RCS1072.md) | Info |
| RCS1073 | [Convert 'if' to 'return' statement](RCS1073.md) | Info |
| RCS1074 | [Remove redundant constructor](RCS1074.md) | Hidden |
| RCS1075 | [Avoid empty catch clause that catches System.Exception](RCS1075.md) | Warning |
| RCS1077 | [Optimize LINQ method call](RCS1077.md) | Info |
| RCS1078 | [Use "" or 'string.Empty'](RCS1078.md) | None |
| RCS1079 | [Throwing of new NotImplementedException](RCS1079.md) | None |
| RCS1080 | [Use 'Count/Length' property instead of 'Any' method](RCS1080.md) | None |
| RCS1081 | [Split variable declaration](RCS1081.md) | None |
| RCS1084 | [Use coalesce expression instead of conditional expression](RCS1084.md) | Info |
| RCS1085 | [Use auto-implemented property](RCS1085.md) | Info |
| RCS1089 | [Use --/++ operator instead of assignment](RCS1089.md) | Info |
| RCS1090 | [Add/remove 'ConfigureAwait(false)' call](RCS1090.md) | None |
| RCS1091 | [Remove empty region](RCS1091.md) | Hidden |
| RCS1093 | [Remove file with no code](RCS1093.md) | Info |
| RCS1094 | [Declare using directive on top level](RCS1094.md) | None |
| RCS1096 | [Use 'HasFlag' method or bitwise operator](RCS1096.md) | None |
| RCS1097 | [Remove redundant 'ToString' call](RCS1097.md) | Info |
| RCS1098 | [Constant values should be placed on right side of comparisons](RCS1098.md) | Info |
| RCS1099 | [Default label should be the last label in a switch section](RCS1099.md) | Info |
| RCS1100 | [(\[deprecated\] use RCS1253 instead) Format documentation summary on a single line](RCS1100.md) | None |
| RCS1101 | [(\[deprecated\] use RCS1253 instead) Format documentation summary on multiple lines](RCS1101.md) | None |
| RCS1102 | [Make class static](RCS1102.md) | Warning |
| RCS1103 | [Convert 'if' to assignment](RCS1103.md) | Info |
| RCS1104 | [Simplify conditional expression](RCS1104.md) | Info |
| RCS1105 | [Unnecessary interpolation](RCS1105.md) | Info |
| RCS1106 | [Remove empty destructor](RCS1106.md) | Info |
| RCS1107 | [Remove redundant 'ToCharArray' call](RCS1107.md) | Info |
| RCS1108 | [Add 'static' modifier to all partial class declarations](RCS1108.md) | Info |
| RCS1110 | [Declare type inside namespace](RCS1110.md) | Info |
| RCS1111 | [Add braces to switch section with multiple statements](RCS1111.md) | None |
| RCS1112 | [Combine 'Enumerable.Where' method chain](RCS1112.md) | Info |
| RCS1113 | [Use 'string.IsNullOrEmpty' method](RCS1113.md) | Info |
| RCS1114 | [Remove redundant delegate creation](RCS1114.md) | Info |
| RCS1118 | [Mark local variable as const](RCS1118.md) | Info |
| RCS1123 | [Add parentheses when necessary](RCS1123.md) | Info |
| RCS1124 | [Inline local variable](RCS1124.md) | Hidden |
| RCS1126 | [Add braces to if-else](RCS1126.md) | None |
| RCS1128 | [Use coalesce expression](RCS1128.md) | Info |
| RCS1129 | [Remove redundant field initialization](RCS1129.md) | Hidden |
| RCS1130 | [Bitwise operation on enum without Flags attribute](RCS1130.md) | Info |
| RCS1132 | [Remove redundant overriding member](RCS1132.md) | Info |
| RCS1133 | [Remove redundant Dispose/Close call](RCS1133.md) | Hidden |
| RCS1134 | [Remove redundant statement](RCS1134.md) | Hidden |
| RCS1135 | [Declare enum member with zero value (when enum has FlagsAttribute)](RCS1135.md) | Info |
| RCS1136 | [Merge switch sections with equivalent content](RCS1136.md) | Hidden |
| RCS1138 | [Add summary to documentation comment](RCS1138.md) | Warning |
| RCS1139 | [Add summary element to documentation comment](RCS1139.md) | Warning |
| RCS1140 | [Add exception to documentation comment](RCS1140.md) | Hidden |
| RCS1141 | [Add 'param' element to documentation comment](RCS1141.md) | Hidden |
| RCS1142 | [Add 'typeparam' element to documentation comment](RCS1142.md) | Hidden |
| RCS1143 | [Simplify coalesce expression](RCS1143.md) | Hidden |
| RCS1145 | [Remove redundant 'as' operator](RCS1145.md) | Hidden |
| RCS1146 | [Use conditional access](RCS1146.md) | Info |
| RCS1151 | [Remove redundant cast](RCS1151.md) | Hidden |
| RCS1154 | [Sort enum members](RCS1154.md) | Info |
| RCS1155 | [Use StringComparison when comparing strings](RCS1155.md) | Warning |
| RCS1156 | [Use string.Length instead of comparison with empty string](RCS1156.md) | Info |
| RCS1157 | [Composite enum value contains undefined flag](RCS1157.md) | Info |
| RCS1158 | [Static member in generic type should use a type parameter](RCS1158.md) | Info |
| RCS1159 | [Use EventHandler\<T>](RCS1159.md) | Info |
| RCS1160 | [Abstract type should not have public constructors](RCS1160.md) | Info |
| RCS1161 | [Enum should declare explicit values](RCS1161.md) | Hidden |
| RCS1162 | [Avoid chain of assignments](RCS1162.md) | None |
| RCS1163 | [Unused parameter](RCS1163.md) | Info |
| RCS1164 | [Unused type parameter](RCS1164.md) | Info |
| RCS1165 | [Unconstrained type parameter checked for null](RCS1165.md) | Hidden |
| RCS1166 | [Value type object is never equal to null](RCS1166.md) | Info |
| RCS1168 | [Parameter name differs from base name](RCS1168.md) | Hidden |
| RCS1169 | [Make field read-only](RCS1169.md) | Info |
| RCS1170 | [Use read-only auto-implemented property](RCS1170.md) | Info |
| RCS1171 | [Simplify lazy initialization](RCS1171.md) | Info |
| RCS1172 | [Use 'is' operator instead of 'as' operator](RCS1172.md) | Warning |
| RCS1173 | [Use coalesce expression instead of 'if'](RCS1173.md) | Info |
| RCS1174 | [Remove redundant async/await](RCS1174.md) | None |
| RCS1175 | [Unused 'this' parameter](RCS1175.md) | Info |
| RCS1176 | [Use 'var' instead of explicit type (when the type is not obvious)](RCS1176.md) | None |
| RCS1177 | [Use 'var' instead of explicit type (in foreach)](RCS1177.md) | None |
| RCS1179 | [Unnecessary assignment](RCS1179.md) | Info |
| RCS1180 | [Inline lazy initialization](RCS1180.md) | Info |
| RCS1181 | [Convert comment to documentation comment](RCS1181.md) | Hidden |
| RCS1182 | [Remove redundant base interface](RCS1182.md) | Hidden |
| RCS1186 | [Use Regex instance instead of static method](RCS1186.md) | Hidden |
| RCS1187 | [Use constant instead of field](RCS1187.md) | Info |
| RCS1188 | [Remove redundant auto-property initialization](RCS1188.md) | Hidden |
| RCS1189 | [Add or remove region name](RCS1189.md) | Hidden |
| RCS1190 | [Join string expressions](RCS1190.md) | Info |
| RCS1191 | [Declare enum value as combination of names](RCS1191.md) | Info |
| RCS1192 | [Unnecessary usage of verbatim string literal](RCS1192.md) | Info |
| RCS1193 | [Overriding member should not change 'params' modifier](RCS1193.md) | Warning |
| RCS1194 | [Implement exception constructors](RCS1194.md) | Warning |
| RCS1195 | [Use ^ operator](RCS1195.md) | Info |
| RCS1196 | [Call extension method as instance method](RCS1196.md) | Info |
| RCS1197 | [Optimize StringBuilder.Append/AppendLine call](RCS1197.md) | Info |
| RCS1198 | [Avoid unnecessary boxing of value type](RCS1198.md) | None |
| RCS1199 | [Unnecessary null check](RCS1199.md) | Info |
| RCS1200 | [Call 'Enumerable.ThenBy' instead of 'Enumerable.OrderBy'](RCS1200.md) | Info |
| RCS1201 | [Use method chaining](RCS1201.md) | Hidden |
| RCS1202 | [Avoid NullReferenceException](RCS1202.md) | Info |
| RCS1203 | [Use AttributeUsageAttribute](RCS1203.md) | Warning |
| RCS1204 | [Use EventArgs.Empty](RCS1204.md) | Info |
| RCS1205 | [Order named arguments according to the order of parameters](RCS1205.md) | Info |
| RCS1206 | [Use conditional access instead of conditional expression](RCS1206.md) | Info |
| RCS1207 | [Use anonymous function or method group](RCS1207.md) | None |
| RCS1208 | [Reduce 'if' nesting](RCS1208.md) | None |
| RCS1209 | [Order type parameter constraints](RCS1209.md) | Info |
| RCS1210 | [Return completed task instead of returning null](RCS1210.md) | Warning |
| RCS1211 | [Remove unnecessary 'else'](RCS1211.md) | Hidden |
| RCS1212 | [Remove redundant assignment](RCS1212.md) | Info |
| RCS1213 | [Remove unused member declaration](RCS1213.md) | Info |
| RCS1214 | [Unnecessary interpolated string](RCS1214.md) | Info |
| RCS1215 | [Expression is always equal to true/false](RCS1215.md) | Warning |
| RCS1216 | [Unnecessary unsafe context](RCS1216.md) | Info |
| RCS1217 | [Convert interpolated string to concatenation](RCS1217.md) | Hidden |
| RCS1218 | [Simplify code branching](RCS1218.md) | Info |
| RCS1220 | [Use pattern matching instead of combination of 'is' operator and cast operator](RCS1220.md) | Info |
| RCS1221 | [Use pattern matching instead of combination of 'as' operator and null check](RCS1221.md) | Info |
| RCS1222 | [Merge preprocessor directives](RCS1222.md) | Info |
| RCS1223 | [Mark publicly visible type with DebuggerDisplay attribute](RCS1223.md) | None |
| RCS1224 | [Make method an extension method](RCS1224.md) | Info |
| RCS1225 | [Make class sealed](RCS1225.md) | Info |
| RCS1226 | [Add paragraph to documentation comment](RCS1226.md) | Info |
| RCS1227 | [Validate arguments correctly](RCS1227.md) | Info |
| RCS1228 | [Unused element in documentation comment](RCS1228.md) | Hidden |
| RCS1229 | [Use async/await when necessary](RCS1229.md) | Info |
| RCS1230 | [Unnecessary explicit use of enumerator](RCS1230.md) | Info |
| RCS1231 | [Make parameter ref read-only](RCS1231.md) | None |
| RCS1232 | [Order elements in documentation comment](RCS1232.md) | Info |
| RCS1233 | [Use short-circuiting operator](RCS1233.md) | Info |
| RCS1234 | [Duplicate enum value](RCS1234.md) | Info |
| RCS1235 | [Optimize method call](RCS1235.md) | Info |
| RCS1236 | [Use exception filter](RCS1236.md) | Info |
| RCS1237 | [(\[deprecated\] use RCS1254 instead) Use bit shift operator](RCS1237.md) | Hidden |
| RCS1238 | [Avoid nested ?: operators](RCS1238.md) | Hidden |
| RCS1239 | [Use 'for' statement instead of 'while' statement](RCS1239.md) | Info |
| RCS1240 | [Operator is unnecessary](RCS1240.md) | Info |
| RCS1241 | [Implement non-generic counterpart](RCS1241.md) | Hidden |
| RCS1242 | [Do not pass non-read-only struct by read-only reference](RCS1242.md) | Warning |
| RCS1243 | [Duplicate word in a comment](RCS1243.md) | Info |
| RCS1244 | [Simplify 'default' expression](RCS1244.md) | Hidden |
| RCS1246 | [Use element access](RCS1246.md) | Info |
| RCS1247 | [Fix documentation comment tag](RCS1247.md) | Info |
| RCS1248 | [Normalize null check](RCS1248.md) | None |
| RCS1249 | [Unnecessary null-forgiving operator](RCS1249.md) | Info |
| RCS1250 | [Use implicit/explicit object creation](RCS1250.md) | None |
| RCS1251 | [Remove unnecessary braces from record declaration](RCS1251.md) | Info |
| RCS1252 | [Normalize usage of infinite loop](RCS1252.md) | None |
| RCS1253 | [Format documentation comment summary](RCS1253.md) | None |
| RCS1254 | [Normalize format of enum flag value](RCS1254.md) | Info |
| RCS1255 | [Simplify argument null check](RCS1255.md) | None |
| RCS1256 | [Invalid argument null check](RCS1256.md) | Info |
| RCS9001 | [Use pattern matching](RCS9001.md) | Hidden |
| RCS9002 | [Use property SyntaxNode.SpanStart](RCS9002.md) | Info |
| RCS9003 | [Unnecessary conditional access](RCS9003.md) | Info |
| RCS9004 | [Call 'Any' instead of accessing 'Count'](RCS9004.md) | Info |
| RCS9005 | [Unnecessary null check](RCS9005.md) | Info |
| RCS9006 | [Use element access](RCS9006.md) | Info |
| RCS9007 | [Use return value](RCS9007.md) | Warning |
| RCS9008 | [Call 'Last' instead of using \[\]](RCS9008.md) | Info |
| RCS9009 | [Unknown language name](RCS9009.md) | Warning |
| RCS9010 | [Specify ExportCodeRefactoringProviderAttribute.Name](RCS9010.md) | Hidden |
| RCS9011 | [Specify ExportCodeFixProviderAttribute.Name](RCS9011.md) | Hidden |


*\(Generated with [DotMarkdown](http://github.com/JosefPihrt/DotMarkdown)\)*