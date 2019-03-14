
# `format` Command

Formats documents in the specified project or solution.

## Synopsis

```
roslynator format <PROJECT|SOLUTION>
[--culture]
[--empty-line-after-closing-brace]
[--empty-line-after-embedded-statement]
[--empty-line-before-while-in-do-statement]
[--empty-line-between-declarations]
[--end-of-line]
[--format-accessor-list]
[--include-generated-code]
[--ignored-projects]
[--language]
[--file-log]
[--file-log-verbosity]
[--msbuild-path]
[--new-line-after-switch-label]
[--new-line-before-binary-operator]
[--new-line-before-closing-brace-in-block]
[--new-line-before-closing-brace-in-empty-block]
[--new-line-before-closing-brace-in-empty-declaration]
[--new-line-before-conditional-expression-operator]
[--new-line-before-embedded-statement]
[--new-line-before-enum-member]
[--new-line-before-statement]
[--projects]
[--remove-redundant-empty-line]
[-v|--verbosity]
```

## Arguments

**`PROJECT|SOLUTION`**

The project or solution to fix.

### Optional Options

**`--culture`** <CULTURE_ID>

Defines culture that should be used to display diagnostic message.

**`--empty-line-after-closing-brace`**

Indicates whether a closing brace should be followed with empty line.

*Note: This option is available only for C#. For details see [RCS1153](https://github.com/JosefPihrt/Roslynator/blob/master/docs/analyzers/RCS1153.md).*

**`--empty-line-after-embedded-statement`**

Indicates whether an embedded statement should be followed with empty line.

*Note: This option is available only for C#. For details see [RCS1030](https://github.com/JosefPihrt/Roslynator/blob/master/docs/analyzers/RCS1030.md).*

**`--empty-line-before-while-in-do-statement`**

Indicates whether while keyword in `do` statement should be preceded with empty line.

*Note: This option is available only for C#. For details see [RCS1092](https://github.com/JosefPihrt/Roslynator/blob/master/docs/analyzers/RCS1092.md).*

**`--empty-line-between-declarations`**

Indicates whether member declarations should be separated with empty line.

*Note: This option is available only for C#. For details see [RCS1057](https://github.com/JosefPihrt/Roslynator/blob/master/docs/analyzers/RCS1057.md).*

**`--end-of-line`** {lf|crlf}

Defines end of line character(s).

*Note: For details see [RCS1086](https://github.com/JosefPihrt/Roslynator/blob/master/docs/analyzers/RCS1086.md) and [RCS1087](https://github.com/JosefPihrt/Roslynator/blob/master/docs/analyzers/RCS1087.md).*

**`--format-accessor-list`**

Indicates whether access list should be formatted.

*Note: This option is available only for C#. For details see [RCS1024](https://github.com/JosefPihrt/Roslynator/blob/master/docs/analyzers/RCS1024.md).*

**`--include-generated-code`**

Indicates whether generated code should be formatted.

**`--ignored-projects`** <PROJECT_NAME>

Defines project names that should not be formatted.

**`--language`** `{cs[harp]|v[isual-]b[asic])}`

Defines project language.

**`--msbuild-path`** <MSBUILD_PATH>

Defines a path to MSBuild. This option must be specified if there are multiple locations of MSBuild (usually multiple installations of Visual Studio).

**`--new-line-after-switch-label`**

Indicates whether switch label should be followed with new line.

*Note: This option is available only for C#. For details see [RCS1028](https://github.com/JosefPihrt/Roslynator/blob/master/docs/analyzers/RCS1028.md).*

**`--new-line-before-binary-operator`**

Indicates whether a binary operator should be preceded with new line.

*Note: This option is available only for C#. For details see [RCS1029](https://github.com/JosefPihrt/Roslynator/blob/master/docs/analyzers/RCS1029.md).*

**`--new-line-before-closing-brace-in-block`**

Indicates whether closing brace in a block should be preceded with new line.

*Note: This option is available only for C#. For details see [RCS1185](https://github.com/JosefPihrt/Roslynator/blob/master/docs/analyzers/RCS1185.md).*

**`--new-line-before-closing-brace-in-empty-block`**

Indicates whether closing brace in an empty block should be preceded with new line.

*Note: This option is available only for C#. For details see [RCS1023](https://github.com/JosefPihrt/Roslynator/blob/master/docs/analyzers/RCS1023.md).*

**`--new-line-before-closing-brace-in-empty-declaration`**

Indicates whether closing brace in a type declaration should be preceded with new line.

*Note: This option is available only for C#. For details see [RCS1076](https://github.com/JosefPihrt/Roslynator/blob/master/docs/analyzers/RCS1076.md).*

**`--new-line-before-conditional-expression-operator`**

Indicates whether operator in a conditional expression be preceded with new line.

*Note: This option is available only for C#. For details see [RCS1184](https://github.com/JosefPihrt/Roslynator/blob/master/docs/analyzers/RCS1184.md).*

**`--new-line-before-embedded-statement`**

Indicates whether an embedded statement should be preceded with new line.

*Note: This option is available only for C#. For details see [RCS1027](https://github.com/JosefPihrt/Roslynator/blob/master/docs/analyzers/RCS1027.md).*

**`--new-line-before-enum-member`**

Indicates whether an enum member declaration should be preceded with new line.

*Note: This option is available only for C#. For details see [RCS1025](https://github.com/JosefPihrt/Roslynator/blob/master/docs/analyzers/RCS1025.md).*

**`--new-line-before-statement`**

Indicates whether a statement should be preceded with new line.

*Note: This option is available only for C#. For details see [RCS1026](https://github.com/JosefPihrt/Roslynator/blob/master/docs/analyzers/RCS1026.md).*

**`--projects`** <PROJECT_NAME>

Defines projects that should be formatted.

**`-p|--properties`** `<NAME=VALUE>`

Defines one or more MSBuild properties.

**`--remove-redundant-empty-line`**

Indicates whether redundant empty lines should be removed.

*Note: This option is available only for C#. For details see [RCS1036](https://github.com/JosefPihrt/Roslynator/blob/master/docs/analyzers/RCS1036.md).*

**`-v|--verbosity`** `{q[uiet]|m[inimal]|n[ormal]|d[etailed]|diag[nostic]}`

Defines the amount of information to display in the log.

## See Also

* [Roslynator Command-Line Interface](README.md)
