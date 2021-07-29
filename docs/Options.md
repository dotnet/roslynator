# List of EditorConfig Options

```editorconfig
# Max line length.
roslynator.max_line_length = <MAX_LINE_LENGTH>

# Remove empty line between single-line accessors.
roslynator.RCS0011.invert = true

# Remove empty line between using directives with different root namespace.
roslynator.RCS0015.invert = true

# Add newline after binary operator instead of before it.
roslynator.RCS0027.invert = true

# Add newline after conditional operator instead of before it.
roslynator.RCS0028.invert = true

# Add newline after expression-body arrow instead of before it.
roslynator.RCS0032.invert = true

# Remove newline between closing brace and 'while' keyword.
roslynator.RCS0051.invert = true

# Add newline after equals sign instead of before it.
roslynator.RCS0052.invert = true

# Use implicitly typed array.
roslynator.RCS1014.invert = true

# Use implicitly typed array (when type is obvious).
roslynator.RCS1014.use_implicit_type_when_obvious = true

# Convert expression-body to block body.
roslynator.RCS1016.invert = true

# Convert expression-body to block body when declaration is multi-line.
roslynator.RCS1016.use_block_body_when_declaration_is_multiline = true

# Convert expression-body to block body when expression is multi-line.
roslynator.RCS1016.use_block_body_when_expression_is_multiline = true

# Remove accessibility modifiers.
roslynator.RCS1018.invert = true

# Remove empty line between closing brace and switch section.
roslynator.RCS1036.remove_empty_line_between_closing_brace_and_switch_section = true

# Do not rename private static field to camel case with underscore.
roslynator.RCS1045.suppress_when_field_is_static = true

# Remove argument list from object creation expression.
roslynator.RCS1050.invert = true

# Remove parentheses from condition of conditional expression (when condition is a single token).
roslynator.RCS1051.do_not_parenthesize_single_token = true

# Use string.Empty instead of "".
roslynator.RCS1078.invert = true

# Remove call to 'ConfigureAwait'.
roslynator.RCS1090.invert = true

# Convert bitwise operation to 'HasFlag' call.
roslynator.RCS1096.invert = true

# Do not simplify conditional expression when condition is inverted.
roslynator.RCS1104.suppress_when_condition_is_inverted = true

# Convert method group to anonymous function.
roslynator.RCS1207.invert = true

# Suppress Unity script methods.
roslynator.RCS1213.suppress_unity_script_methods = true

# Do not use element access when expression is invocation.
roslynator.RCS1246.suppress_when_expression_is_invocation = true

# Use comparison instead of pattern matching to check for null.
roslynator.RCS1248.invert = true
```


*\(Generated with [DotMarkdown](http://github.com/JosefPihrt/DotMarkdown)\)*