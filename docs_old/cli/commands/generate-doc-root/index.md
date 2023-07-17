---
sidebar_label: generate-doc-root
---

# `roslynator generate-doc-root`

Generates root documentation file from specified project/solution\.

## Synopsis

```
roslynator generate-doc-root <PROJECT|SOLUTION>
    --depth <DEPTH>
    --file-log <FILE_PATH>
    --file-log-verbosity <LEVEL>
    --heading <HEADING>
-h, --help
    --host <HOST>
    --ignored-names <FULLY_QUALIFIED_METADATA_NAME>
    --ignored-parts <IGNORED_PARTS>
    --ignored-projects <PROJECT_NAME>
    --include-containing-namespace <INCLUDE_CONTAINING_NAMESPACE>
    --include-system-namespace
    --language <LANGUAGE>
-m, --msbuild-path <DIRECTORY_PATH>
    --no-mark-obsolete
    --no-precedence-for-system
-o, --output <DIRECTORY_PATH>
    --projects <PROJECT_NAME>
-p, --properties <NAME=VALUE>
    --root-directory-url <URL>
    --scroll-to-content
-v, --verbosity <LEVEL>
    --visibility <VISIBILITY>
```

## Arguments

**`<PROJECT|SOLUTION>`**

The project or solution file\.

## Options

##### `--depth <DEPTH>`

Defines a depth of a documentation\. Allowed values are member \(default\), type or namespace\.

##### `--file-log <FILE_PATH>`

Path to a file that should store output\.

##### `--file-log-verbosity <LEVEL>`

Verbosity of the file log\. Allowed values are q\[uiet\], m\[inimal\], n\[ormal\], d\[etailed\] and diag\[nostic\]\.

##### `--heading <HEADING>`

Defines a heading of the root documentation file\.

##### `-h, --help`

Show command line help\.

##### `--host <HOST>`

Defines a host where the content will be published\. Allowed values are docusaurus, github or sphinx\.

##### `--ignored-names <FULLY_QUALIFIED_METADATA_NAME>`

Defines a list of metadata names that should be excluded from a documentation\. Namespace of type names can be specified\.

##### `--ignored-parts <IGNORED_PARTS>`

Defines parts of a root documentation that should be excluded\. Allowed values are content, namespaces, class\-hierarchy, types and other

##### `--ignored-projects <PROJECT_NAME>`

Defines projects that should not be analyzed\.

##### `--include-containing-namespace <INCLUDE_CONTAINING_NAMESPACE>`

Defines parts of a documentation that should include containing namespace\. Allowed value is class\-hierarchy\.

##### `--include-system-namespace`

Indicates whether namespace should be included when a type is directly contained in namespace 'System'\.

##### `--language <LANGUAGE>`

Defines project language\. Allowed values are cs\[harp\] or v\[isual\-\]b\[asic\]

##### `-m, --msbuild-path <DIRECTORY_PATH>`

Defines a path to MSBuild directory\.

##### `--no-mark-obsolete`

Indicates whether obsolete types and members should not be marked as '\[deprecated\]'\.

##### `--no-precedence-for-system`

Indicates whether symbols contained in 'System' namespace should be ordered as any other symbols and not before other symbols\.

##### `-o, --output <DIRECTORY_PATH>`

Defines a path for the output directory\.

##### `--projects <PROJECT_NAME>`

Defines projects that should be analyzed\.

##### `-p, --properties <NAME=VALUE>`

Defines one or more MSBuild properties\.

##### `--root-directory-url <URL>`

Defines a relative url to the documentation root directory\.

##### `--scroll-to-content`

Indicates whether a link should lead to the top of the documentation content\. This option is applicable when host is set to 'github'\.

##### `-v, --verbosity <LEVEL>`

Verbosity of the log\. Allowed values are q\[uiet\], m\[inimal\], n\[ormal\], d\[etailed\] and diag\[nostic\]\.

##### `--visibility <VISIBILITY>`

Defines a visibility of a type or a member\. Allowed values are public \(default\), internal or private\.

*\(Generated with [DotMarkdown](https://github.com/JosefPihrt/DotMarkdown)\)*