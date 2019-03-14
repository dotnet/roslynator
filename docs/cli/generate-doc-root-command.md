
# `generate-doc-root` Command

Generates root documentation file from specified assemblies.

## Synopsis

```
roslynator generate-doc-root
-h|--heading
-o|--output
[--depth]
[--file-log]
[--file-log-verbosity]
[--ignored-names]
[--ignored-parts]
[--ignored-projects]
[--language]
[--msbuild-path]
[--no-class-hierarchy]
[--no-mark-obsolete]
[--no-precedence-for-system]
[--omit-containing-namespace]
[-p|--properties]
[--projects]
[--root-directory-url]
[--scroll-to-content]
[-v|--verbosity]
[--visibility]
```

## Arguments

**`PROJECT|SOLUTION`**

The project or solution to analyze.

## Options

### Required Options

**`-h|--heading`** `<ROOT_FILE_HEADING>`

Defines a heading of the root documentation file.

**`-o|--output`** `<OUTPUT_DIRECTORY>`

Defines a path for the output directory.

### Optional Options

**`[--depth]`** `{member|type|namespace}`

Defines a depth of a documentation. Default value is `member`.

**`[--ignored-names]`** `<FULLY_QUALIFIED_METADATA_NAME>`

Defines a list of metadata names that should be excluded from a documentation. Namespace of type names can be specified.

**`[--ignored-parts]`** `{content namespaces classes static-classes structs interfaces enums delegates other}`

Defines parts of a root documentation that should be excluded.

**`--ignored-projects`** <PROJECT_NAME>

Defines projects that should be skipped.

**`--language`** `{cs[harp]|v[isual-]b[asic])}`

Defines project language.

**`--msbuild-path`** <MSBUILD_PATH>

Defines a path to MSBuild. This option must be specified if there are multiple locations of MSBuild (usually multiple installations of Visual Studio).

**`[--no-class-hierarchy]`**

Indicates whether classes should be displayed as a list instead of hierarchy tree.

**`[--no-mark-obsolete]`**

Indicates whether obsolete types and members should not be marked as `[deprecated]`.

**`[--no-precedence-for-system]`**

Indicates whether symbols contained in `System` namespace should be ordered as any other symbols and not before other symbols.

**`[--omit-containing-namespace]`**

Indicates whether a containing namespace should be omitted when displaying type name.

**`--projects`** <PROJECT_NAME>

Defines projects that should be analyzed.

**`-p|--properties`** `<NAME=VALUE>`

Defines one or more MSBuild properties.

**`[--root-directory-url]`** <ROOT_DIRECTORY_URL>

Defines a relative url to the documentation root directory.

**`[--scroll-to-content]`**

Indicates whether a link should lead to the top of the documentation content.

**`-v|--verbosity`** `{q[uiet]|m[inimal]|n[ormal]|d[etailed]|diag[nostic]}`

Defines the amount of information to display in the log.

**`[--visibility]`** `{public|internal|private}`

Defines a visibility of a type or a member. Default value is `public`.

## See Also

* [Roslynator Command-Line Interface](README.md)
