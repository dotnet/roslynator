---
sidebar_label: fix
---

# `roslynator fix`

Fixes diagnostics in the specified project or solution\.

## Synopsis

```
roslynator fix <PROJECT|SOLUTION>
-a, --analyzer-assemblies <PATH>
    --batch-size <BATCH_SIZE>
    --culture <CULTURE_ID>
    --diagnostic-fix-map <DIAGNOSTIC_ID=EQUIVALENCE_KEY>
    --diagnostic-fixer-map <DIAGNOSTIC_ID=FIXER_FULL_NAME>
    --diagnostics-fixable-one-by-one <DIAGNOSTIC_ID>
    --file-banner <FILE_BANNER>
    --file-log <FILE_PATH>
    --file-log-verbosity <LEVEL>
    --fix-scope <FIX_SCOPE>
    --format
-h, --help
    --ignore-analyzer-references
    --ignore-compiler-errors
    --ignored-compiler-diagnostics
    --ignored-diagnostics <DIAGNOSTIC_ID>
    --ignored-projects <PROJECT_NAME>
    --language <LANGUAGE>
    --max-iterations <MAX_ITERATIONS>
-m, --msbuild-path <DIRECTORY_PATH>
    --projects <PROJECT_NAME>
-p, --properties <NAME=VALUE>
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

##### `--batch-size <BATCH_SIZE>`

Defines maximum number of diagnostics that can be fixed in one batch\.

##### `--culture <CULTURE_ID>`

Defines culture that should be used to display diagnostic message\.

##### `--diagnostic-fix-map <DIAGNOSTIC_ID=EQUIVALENCE_KEY>`

Defines mapping between diagnostic and its fix \(CodeAction\)\.

##### `--diagnostic-fixer-map <DIAGNOSTIC_ID=FIXER_FULL_NAME>`

Defines mapping between diagnostic and its fixer \(CodeFixProvider\)\. If there are two \(or more\) fixers for a diagnostic and both provide a fix it is necessary to determine which one should be used to fix the diagnostic\. Set verbosity to 'diagnostic' to see which diagnostics cannot be fixed due to multiple fixers\.

##### `--diagnostics-fixable-one-by-one <DIAGNOSTIC_ID>`

Defines diagnostics that can be fixed even if there is no FixAllProvider for them\.

##### `--file-banner <FILE_BANNER>`

Defines text that should be at the of each source file\.

##### `--file-log <FILE_PATH>`

Path to a file that should store output\.

##### `--file-log-verbosity <LEVEL>`

Verbosity of the file log\. Allowed values are q\[uiet\], m\[inimal\], n\[ormal\], d\[etailed\] and diag\[nostic\]\.

##### `--fix-scope <FIX_SCOPE>`

Defines fix scope\. Allowed values are project \(default\) or document\.

##### `--format`

Indicates whether each document should be formatted\.

##### `-h, --help`

Show command line help\.

##### `--ignore-analyzer-references`

Indicates whether analyzers that are referenced in a project should be ignored\.

##### `--ignore-compiler-errors`

Indicates whether fixing should continue even if compilation has errors\.

##### `--ignored-compiler-diagnostics`

Defines compiler diagnostics that should be ignored even if \-\-ignore\-compiler\-errors is not set\.

##### `--ignored-diagnostics <DIAGNOSTIC_ID>`

Defines diagnostics that should not be reported\.

##### `--ignored-projects <PROJECT_NAME>`

Defines projects that should not be analyzed\.

##### `--language <LANGUAGE>`

Defines project language\. Allowed values are cs\[harp\] or v\[isual\-\]b\[asic\]

##### `--max-iterations <MAX_ITERATIONS>`

Defines maximum numbers of fixing iterations\.

##### `-m, --msbuild-path <DIRECTORY_PATH>`

Defines a path to MSBuild directory\.

##### `--projects <PROJECT_NAME>`

Defines projects that should be analyzed\.

##### `-p, --properties <NAME=VALUE>`

Defines one or more MSBuild properties\.

##### `--severity-level <LEVEL>`

Defines minimally required severity for a diagnostic\. Allowed values are hidden, info \(default\), warning or error\.

##### `--supported-diagnostics <DIAGNOSTIC_ID>`

Defines diagnostics that should be reported\.

##### `-v, --verbosity <LEVEL>`

Verbosity of the log\. Allowed values are q\[uiet\], m\[inimal\], n\[ormal\], d\[etailed\] and diag\[nostic\]\.

## Redirected/Piped Input

Redirected/piped input will be used as a list of project/solution paths separated with newlines.

*\(Generated with [DotMarkdown](https://github.com/JosefPihrt/DotMarkdown)\)*