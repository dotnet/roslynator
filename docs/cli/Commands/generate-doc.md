---
sidebar_label: generate-doc
---

# `roslynator generate-doc`

Generates reference documentation from specified project/solution\.

## Synopsis

```
roslynator generate-doc <PROJECT|SOLUTION>
    --additional-xml-documentation <FILE_PATH>
    --depth <DEPTH>
    --file-log <FILE_PATH>
    --file-log-verbosity <LEVEL>
    --files-layout <LAYOUT>
    --group-by-common-namespace
    --heading <HEADING>
-h, --help
    --host <HOST>
    --ignored-common-parts <IGNORED_COMMON_PARTS>
    --ignored-member-parts <IGNORED_MEMBER_PARTS>
    --ignored-names <FULLY_QUALIFIED_METADATA_NAME>
    --ignored-namespace-parts <IGNORED_NAMESPACE_PARTS>
    --ignored-projects <PROJECT_NAME>
    --ignored-root-parts <IGNORED_ROOT_PARTS>
    --ignored-title-parts <IGNORED_TITLE_PARTS>
    --ignored-type-parts <IGNORED_TYPE_PARTS>
    --include-all-derived-types
    --include-containing-namespace <INCLUDE_CONTAINING_NAMESPACE>
    --include-ienumerable
    --include-inherited-interface-members
    --include-system-namespace
    --inheritance-style <INHERITANCE_STYLE>
    --language <LANGUAGE>
    --max-derived-types <MAX_DERIVED_TYPES>
-m, --msbuild-path <DIRECTORY_PATH>
    --no-delete
    --no-mark-obsolete
    --no-precedence-for-system
    --no-wrap-base-types
    --no-wrap-constraints
    --omit-attribute-arguments
    --omit-inherited-atttributes
    --omit-member-parts
-o, --output <DIRECTORY_PATH>
    --preferred-culture <CULTURE_ID>
    --projects <PROJECT_NAME>
-p, --properties <NAME=VALUE>
    --scroll-to-content
-v, --verbosity <LEVEL>
    --visibility <VISIBILITY>
```

## Arguments

**`<PROJECT|SOLUTION>`**

The project or solution file\.

## Options

##### `--additional-xml-documentation <FILE_PATH>`

Defines one or more xml documentation files that should be included\. These files can contain a documentation for namespaces, for instance\.

##### `--depth <DEPTH>`

Defines a depth of a documentation\. Allowed values are member \(default\), type or namespace\.

##### `--file-log <FILE_PATH>`

Path to a file that should store output\.

##### `--file-log-verbosity <LEVEL>`

Verbosity of the file log\. Allowed values are q\[uiet\], m\[inimal\], n\[ormal\], d\[etailed\] and diag\[nostic\]\.

##### `--files-layout <LAYOUT>`

Defines layout of documentation files\. Allowed values are hierarchical \(default\) or flat\-namespaces\.

##### `--group-by-common-namespace`

Indicates whether to group namespaces by greatest common namespace\.

##### `--heading <HEADING>`

Defines a heading of the root documentation file\.

##### `-h, --help`

Show command line help\.

##### `--host <HOST>`

Defines a host where the content will be published\. Allowed values are docusaurus, github or sphinx\.

##### `--ignored-common-parts <IGNORED_COMMON_PARTS>`

Defines common parts of a documentation that should be excluded\. Allowed value is content\.

##### `--ignored-member-parts <IGNORED_MEMBER_PARTS>`

Defines parts of a member documentation that should be excluded\. Allowed values are overloads, containing\-type, containing\-assembly, obsolete\-message, summary, declaration, type\-parameters, parameters, return\-value, implements, attributes, exceptions, examples, remarks and see\-also\.

##### `--ignored-names <FULLY_QUALIFIED_METADATA_NAME>`

Defines a list of metadata names that should be excluded from a documentation\. Namespace of type names can be specified\.

##### `--ignored-namespace-parts <IGNORED_NAMESPACE_PARTS>`

Defines parts of a namespace documentation that should be excluded\. Allowed values are content, containing\-namespace, summary, examples, remarks, classes, structs, interfaces, enums, delegates and see\-also\.

##### `--ignored-projects <PROJECT_NAME>`

Defines projects that should not be analyzed\.

##### `--ignored-root-parts <IGNORED_ROOT_PARTS>`

Defines parts of a root documentation that should be excluded\. Allowed values are content, namespaces, class\-hierarchy, types and other\.

##### `--ignored-title-parts <IGNORED_TITLE_PARTS>`

Defines title parts of a documentation that should be excluded\. Allowed value is containing\-namespace, containing\-type, parameters and explicit\-implementation\.

##### `--ignored-type-parts <IGNORED_TYPE_PARTS>`

Defines parts of a type documentation that should be excluded\. Allowed values are content, containing\-namespace, containing\-assembly, obsolete\-message, summary, declaration, type\-parameters, parameters, return\-value, inheritance, attributes, derived, implements, examples, remarks, constructors, fields, indexers, properties, methods, operators, events, explicit\-interface\-implementations, extension\-methods, classes, structs, interfaces, enums, delegates and see\-also\.

##### `--include-all-derived-types`

Indicates whether all derived types should be included in the list of derived types\. By default only types that directly inherits from a specified type are displayed\.

##### `--include-containing-namespace <INCLUDE_CONTAINING_NAMESPACE>`

Defines parts of a documentation that should include containing namespace\. Allowed values are class\-hierarchy, containing\-type, parameter, return\-type, base\-type, attribute, derived\-type, implemented\-interface, implemented\-member, exception, see\-also and all\.

##### `--include-ienumerable`

Indicates whether interface System\.Collections\.IEnumerable should be included in a documentation if a type also implements interface System\.Collections\.Generic\.IEnumerable\<T\>\.

##### `--include-inherited-interface-members`

Indicates whether inherited interface members should be displayed in a list of members\.

##### `--include-system-namespace`

Indicates whether namespace should be included when a type is directly contained in namespace 'System'\.

##### `--inheritance-style <INHERITANCE_STYLE>`

Defines a style of a type inheritance\. Allowed values are horizontal \(default\) or vertical\.

##### `--language <LANGUAGE>`

Defines project language\. Allowed values are cs\[harp\] or v\[isual\-\]b\[asic\]

##### `--max-derived-types <MAX_DERIVED_TYPES>`

Defines maximum number derived types that should be displayed\. Default value is 5\.

##### `-m, --msbuild-path <DIRECTORY_PATH>`

Defines a path to MSBuild directory\.

##### `--no-delete`

Indicates whether output directory should not be deleted at the beginning of the process\.

##### `--no-mark-obsolete`

Indicates whether obsolete types and members should not be marked as '\[deprecated\]'\.

##### `--no-precedence-for-system`

Indicates whether symbols contained in 'System' namespace should be ordered as any other symbols and not before other symbols\.

##### `--no-wrap-base-types`

Indicates whether base types should not be wrapped\.

##### `--no-wrap-constraints`

Indicates whether constraints should not be wrapped\.

##### `--omit-attribute-arguments`

Indicates whether attribute arguments should be omitted when displaying an attribute\.

##### `--omit-inherited-atttributes`

Indicates whether inherited attributes should be omitted\.

##### `--omit-member-parts`

Defines parts of member definition that should be omitted\. Allowed values are constant\-value, implements, inherited\-from and overrides\.

##### `-o, --output <DIRECTORY_PATH>`

Defines a path for the output directory\.

##### `--preferred-culture <CULTURE_ID>`

Defines culture that should be used when searching for xml documentation files\.

##### `--projects <PROJECT_NAME>`

Defines projects that should be analyzed\.

##### `-p, --properties <NAME=VALUE>`

Defines one or more MSBuild properties\.

##### `--scroll-to-content`

Indicates whether a link should lead to the top of the documentation content\. This option is applicable when host is set to 'github'\.

##### `-v, --verbosity <LEVEL>`

Verbosity of the log\. Allowed values are q\[uiet\], m\[inimal\], n\[ormal\], d\[etailed\] and diag\[nostic\]\.

##### `--visibility <VISIBILITY>`

Defines a visibility of a type or a member\. Allowed values are public \(default\), internal or private\.

*\(Generated with [DotMarkdown](https://github.com/JosefPihrt/DotMarkdown)\)*