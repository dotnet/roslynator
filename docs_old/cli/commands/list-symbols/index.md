---
sidebar_label: list-symbols
---

# `roslynator list-symbols`

Lists symbols from the specified project or solution\.

## Synopsis

```
roslynator list-symbols <PROJECT|SOLUTION>
    --depth <DEPTH>
    --documentation
    --empty-line-between-members
    --external-assemblies <ASSEMBLY_FILE>
    --file-log <FILE_PATH>
    --file-log-verbosity <LEVEL>
    --group-by-assembly
-h, --help
    --hierarchy-root <FULLY_QUALIFIED_METADATA_NAME>
    --ignored-attributes <FULLY_QUALIFIED_METADATA_NAME>
    --ignored-parts <IGNORED_PARTS>
    --ignored-projects <PROJECT_NAME>
    --ignored-symbols <FULLY_QUALIFIED_METADATA_NAME>
    --indent-chars <INDENT_CHARS>
    --language <LANGUAGE>
    --layout
-m, --msbuild-path <DIRECTORY_PATH>
-o, --output <FILE_PATH>
    --projects <PROJECT_NAME>
-p, --properties <NAME=VALUE>
-v, --verbosity <LEVEL>
    --visibility <VISIBILITY>
    --wrap-list
```

## Arguments

**`<PROJECT|SOLUTION>`**

Path to one or more project/solution files\.

## Options

##### `--depth <DEPTH>`

Defines a depth of a list of symbols\. Allowed values are member \(default\), type or namespace\.

##### `--documentation`

Indicates whether a documentation should be included\.

##### `--empty-line-between-members`

Indicates whether an empty line should be added between two member definitions\.

##### `--external-assemblies <ASSEMBLY_FILE>`

Defines file name/path to external assemblies that should be included\.

##### `--file-log <FILE_PATH>`

Path to a file that should store output\.

##### `--file-log-verbosity <LEVEL>`

Verbosity of the file log\. Allowed values are q\[uiet\], m\[inimal\], n\[ormal\], d\[etailed\] and diag\[nostic\]\.

##### `--group-by-assembly`

Indicates whether symbols should be grouped by assembly\.

##### `-h, --help`

Show command line help\.

##### `--hierarchy-root <FULLY_QUALIFIED_METADATA_NAME>`

Defines symbol that should be used as a root of a type hierarchy\.

##### `--ignored-attributes <FULLY_QUALIFIED_METADATA_NAME>`

Defines a list of attributes that should be ignored\.

##### `--ignored-parts <IGNORED_PARTS>`

Defines parts of a symbol definition that should be excluded\. Allowed values are assemblies, containing\-namespace, containing\-namespace\-in\-type\-hierarchy, attributes, assembly\-attributes, attribute\-arguments, accessibility, modifiers, parameter\-name, parameter\-default\-value, base\-type, base\-interfaces, constraints, trailing\-semicolon, trailing\-comma\.

##### `--ignored-projects <PROJECT_NAME>`

Defines projects that should not be analyzed\.

##### `--ignored-symbols <FULLY_QUALIFIED_METADATA_NAME>`

Defines a list of symbols that should be ignored\. Namespace of types can be specified\.

##### `--indent-chars <INDENT_CHARS>`

Defines characters that should be used for indentation\. Default value is two spaces\.

##### `--language <LANGUAGE>`

Defines project language\. Allowed values are cs\[harp\] or v\[isual\-\]b\[asic\]

##### `--layout`

Defines layout of a list of symbol definitions\. Allowed values are namespace\-list \(default\), namespace\-hierarchy or type\-hierarchy\.

##### `-m, --msbuild-path <DIRECTORY_PATH>`

Defines a path to MSBuild directory\.

##### `-o, --output <FILE_PATH>`

Defines path to file\(s\) that will store a list of symbol definitions\.

##### `--projects <PROJECT_NAME>`

Defines projects that should be analyzed\.

##### `-p, --properties <NAME=VALUE>`

Defines one or more MSBuild properties\.

##### `-v, --verbosity <LEVEL>`

Verbosity of the log\. Allowed values are q\[uiet\], m\[inimal\], n\[ormal\], d\[etailed\] and diag\[nostic\]\.

##### `--visibility <VISIBILITY>`

Defines one or more visibility of a type or a member\. Allowed values are public, internal or private\.

##### `--wrap-list`

Specifies syntax lists that should be wrapped\. Allowed values are attributes, parameters, base\-types and constraints\.

## Redirected/Piped Input

Redirected/piped input will be used as a list of project/solution paths separated with newlines.

*\(Generated with [DotMarkdown](https://github.com/JosefPihrt/DotMarkdown)\)*