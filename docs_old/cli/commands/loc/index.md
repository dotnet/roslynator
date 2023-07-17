---
sidebar_label: loc
---

# `roslynator loc`

Counts physical lines of code in the specified project or solution\.

## Synopsis

```
roslynator loc <PROJECT|SOLUTION>
    --file-log <FILE_PATH>
    --file-log-verbosity <LEVEL>
-h, --help
    --ignore-block-boundary
    --ignored-projects <PROJECT_NAME>
    --include-comments
-g, --include-generated-code
    --include-preprocessor-directives
    --include-whitespace
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

##### `--file-log <FILE_PATH>`

Path to a file that should store output\.

##### `--file-log-verbosity <LEVEL>`

Verbosity of the file log\. Allowed values are q\[uiet\], m\[inimal\], n\[ormal\], d\[etailed\] and diag\[nostic\]\.

##### `-h, --help`

Show command line help\.

##### `--ignore-block-boundary`

Indicates whether a line that contains only block boundary should not be counted\.

##### `--ignored-projects <PROJECT_NAME>`

Defines projects that should not be analyzed\.

##### `--include-comments`

Indicates whether a line that contains only comment should be counted\.

##### `-g, --include-generated-code`

Indicates whether generated code should be included\.

##### `--include-preprocessor-directives`

Indicates whether preprocessor directive line should be counted\.

##### `--include-whitespace`

Indicates whether white\-space line should be counted\.

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