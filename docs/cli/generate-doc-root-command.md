
# `generate-doc-root` Command

Generates root documentation file from specified assemblies.

## Synopsis

```
roslynator generate-doc-root
-a|--assemblies
-h|--heading
-o|--output
-r|--references
[--depth]
[--ignored-names]
[--ignored-parts]
[--no-class-hierarchy]
[--no-mark-obsolete]
[--no-precedence-for-system]
[--omit-containing-namespace]
[--root-directory-url]
[--scroll-to-content]
[--visibility]
```

## Options

### Required Options

**`-a|--assemblies`** `<ASSEMBLIES>`

Defines one or more assemblies that should be used as a source for the documentation.

**`-h|--heading`** `<ROOT_FILE_HEADING>`

Defines a heading of the root documentation file.

**`-o|--output`** `<OUTPUT_DIRECTORY>`

Defines a path for the output directory.

**`-r|--references`** `<ASSEMBLY_REFERENCE | ASSEMBLY_REFERENCES_FILE>`

Defines one or more paths to assembly or a file that contains a list of all assemblies. Each assembly must be on separate line.

### Optional Options

**`[--depth]`** `{member|type|namespace}`

Defines a depth of a documentation. Default value is `member`.

**`[--ignored-names]`** `<FULLY_QUALIFIED_METADATA_NAME>`

Defines a list of metadata names that should be excluded from a documentation. Namespace of type names can be specified.

**`[--ignored-parts]`** `{content namespaces classes static-classes structs interfaces enums delegates other}`

Defines parts of a root documentation that should be excluded.

**`[--no-class-hierarchy]`**

Indicates whether classes should be displayed as a list instead of hierarchy tree.

**`[--no-mark-obsolete]`**

Indicates whether obsolete types and members should not be marked as `[deprecated]`.

**`[--no-precedence-for-system]`**

Indicates whether symbols contained in `System` namespace should be ordered as any other symbols and not before other symbols.

**`[--omit-containing-namespace]`**

Indicates whether a containing namespace should be omitted when displaying type name.

**`[--root-directory-url]`** <ROOT_DIRECTORY_URL>

Defines a relative url to the documentation root directory.

**`[--scroll-to-content]`**

Indicates whether a link should lead to the top of the documentation content.

**`[--visibility]`** `{public|internal|private}`

Defines a visibility of a type or a member. Default value is `public`.

## See Also

* [Roslynator Command-Line Interface](README.md)
