---
sidebar_label: format
---

# `roslynator format`

Formats documents in the specified project or solution\.

## Synopsis

```
roslynator format <PROJECT|SOLUTION>
    --culture <CULTURE_ID>
    --file-log <FILE_PATH>
    --file-log-verbosity <LEVEL>
-h, --help
    --ignored-projects <PROJECT_NAME>
-g, --include-generated-code
    --language <LANGUAGE>
-m, --msbuild-path <DIRECTORY_PATH>
    --projects <PROJECT_NAME>
-p, --properties <NAME=VALUE>
-v, --verbosity <LEVEL>
```

## Arguments

**`<PROJECT|SOLUTION>`**

Path to one or more project/solution files\.

## Options

##### `--culture <CULTURE_ID>`

Defines culture that should be used to display diagnostic message\.

##### `--file-log <FILE_PATH>`

Path to a file that should store output\.

##### `--file-log-verbosity <LEVEL>`

Verbosity of the file log\. Allowed values are q\[uiet\], m\[inimal\], n\[ormal\], d\[etailed\] and diag\[nostic\]\.

##### `-h, --help`

Show command line help\.

##### `--ignored-projects <PROJECT_NAME>`

Defines projects that should not be analyzed\.

##### `-g, --include-generated-code`

Indicates whether generated code should be formatted\.

##### `--language <LANGUAGE>`

Defines project language\. Allowed values are cs\[harp\] or v\[isual\-\]b\[asic\]

##### `-m, --msbuild-path <DIRECTORY_PATH>`

Defines a path to MSBuild directory\.

##### `--projects <PROJECT_NAME>`

Defines projects that should be analyzed\.

##### `-p, --properties <NAME=VALUE>`

Defines one or more MSBuild properties\.

##### `-v, --verbosity <LEVEL>`

Verbosity of the log\. Allowed values are q\[uiet\], m\[inimal\], n\[ormal\], d\[etailed\] and diag\[nostic\]\.

## Redirected/Piped Input

Redirected/piped input will be used as a list of project/solution paths separated with newlines.

*\(Generated with [DotMarkdown](https://github.com/JosefPihrt/DotMarkdown)\)*