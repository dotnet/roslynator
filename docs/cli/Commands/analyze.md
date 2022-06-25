---
sidebar_label: analyze
---

# `roslynator analyze`

Analyzes specified project or solution and reports diagnostics\.

## Synopsis

```
roslynator analyze <PROJECT|SOLUTION>
-a, --analyzer-assemblies <PATH>
    --culture <CULTURE_ID>
    --execution-time
    --file-log <FILE_PATH>
    --file-log-verbosity <LEVEL>
-h, --help
    --ignore-analyzer-references
    --ignore-compiler-diagnostics
    --ignored-diagnostics <DIAGNOSTIC_ID>
    --ignored-projects <PROJECT_NAME>
    --language <LANGUAGE>
-m, --msbuild-path <DIRECTORY_PATH>
-o, --output <FILE_PATH>
    --projects <PROJECT_NAME>
-p, --properties <NAME=VALUE>
    --report-not-configurable
    --report-suppressed-diagnostics
    --severity-level <LEVEL>
    --supported-diagnostics <DIAGNOSTIC_ID>
-v, --verbosity <LEVEL>
```

## Arguments

**`<PROJECT|SOLUTION>`**

Path to one or more project/solution files\.

## Options

##### `-a, --analyzer-assemblies <PATH>`

Define one or more paths to an analyzer assembly or a directory that should be searched recursively for analyzer assemblies\.

##### `--culture <CULTURE_ID>`

Defines culture that should be used to display diagnostic message\.

##### `--execution-time`

Indicates whether to measure execution time of each analyzer\.

##### `--file-log <FILE_PATH>`

Path to a file that should store output\.

##### `--file-log-verbosity <LEVEL>`

Verbosity of the file log\. Allowed values are q\[uiet\], m\[inimal\], n\[ormal\], d\[etailed\] and diag\[nostic\]\.

##### `-h, --help`

Show command line help\.

##### `--ignore-analyzer-references`

Indicates whether analyzers that are referenced in a project should be ignored\.

##### `--ignore-compiler-diagnostics`

Indicates whether to display compiler diagnostics\.

##### `--ignored-diagnostics <DIAGNOSTIC_ID>`

Defines diagnostics that should not be reported\.

##### `--ignored-projects <PROJECT_NAME>`

Defines projects that should not be analyzed\.

##### `--language <LANGUAGE>`

Defines project language\. Allowed values are cs\[harp\] or v\[isual\-\]b\[asic\]

##### `-m, --msbuild-path <DIRECTORY_PATH>`

Defines a path to MSBuild directory\.

##### `-o, --output <FILE_PATH>`

Defines path to file that will store reported diagnostics in XML format\.

##### `--projects <PROJECT_NAME>`

Defines projects that should be analyzed\.

##### `-p, --properties <NAME=VALUE>`

Defines one or more MSBuild properties\.

##### `--report-not-configurable`

Indicates whether diagnostics with 'NotConfigurable' tag should be reported\.

##### `--report-suppressed-diagnostics`

Indicates whether suppressed diagnostics should be reported\.

##### `--severity-level <LEVEL>`

Defines minimally required severity for a diagnostic\. Allowed values are hidden, info \(default\), warning or error\.

##### `--supported-diagnostics <DIAGNOSTIC_ID>`

Defines diagnostics that should be reported\.

##### `-v, --verbosity <LEVEL>`

Verbosity of the log\. Allowed values are q\[uiet\], m\[inimal\], n\[ormal\], d\[etailed\] and diag\[nostic\]\.

## Redirected/Piped Input

Redirected/piped input will be used as a list of project/solution paths separated with newlines.

*\(Generated with [DotMarkdown](https://github.com/JosefPihrt/DotMarkdown)\)*