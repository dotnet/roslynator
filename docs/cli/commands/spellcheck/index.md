---
sidebar_label: spellcheck
---

# `roslynator spellcheck`

Searches the specified project or solution for possible misspellings or typos\.

## Synopsis

```
roslynator spellcheck <PROJECT|SOLUTION>
    --case-sensitive
    --culture <CULTURE_ID>
-d, --dry-run
    --file-log <FILE_PATH>
    --file-log-verbosity <LEVEL>
-h, --help
    --ignored-projects <PROJECT_NAME>
    --ignored-scope <SCOPE>
    --include-generated-code
    --interactive
    --language <LANGUAGE>
    --max-word-length <NUM>
    --min-word-length <NUM>
-m, --msbuild-path <DIRECTORY_PATH>
    --projects <PROJECT_NAME>
-p, --properties <NAME=VALUE>
    --scope <SCOPE>
-v, --verbosity <LEVEL>
    --visibility <VISIBILITY>
    --words <PATH>
```

## Arguments

**`<PROJECT|SOLUTION>`**

Path to one or more project/solution files\.

## Options

##### `--case-sensitive`

Specifies case\-sensitive matching\.

##### `--culture <CULTURE_ID>`

Defines culture that should be used to display diagnostic message\.

##### `-d, --dry-run`

Display misspellings and typos but do not save changes to a disk\.

##### `--file-log <FILE_PATH>`

Path to a file that should store output\.

##### `--file-log-verbosity <LEVEL>`

Verbosity of the file log\. Allowed values are q\[uiet\], m\[inimal\], n\[ormal\], d\[etailed\] and diag\[nostic\]\.

##### `-h, --help`

Show command line help\.

##### `--ignored-projects <PROJECT_NAME>`

Defines projects that should not be analyzed\.

##### `--ignored-scope <SCOPE>`

Defines syntax that should not be analyzed\. Allowed values are comment, type, member, local, parameter, literal, non\-symbol and symbol\.

##### `--include-generated-code`

Indicates whether generated code should be spellchecked\.

##### `--interactive`

Enable editing of a replacement\.

##### `--language <LANGUAGE>`

Defines project language\. Allowed values are cs\[harp\] or v\[isual\-\]b\[asic\]

##### `--max-word-length <NUM>`

Specifies maximal word length to be checked\.

##### `--min-word-length <NUM>`

Specifies minimal word length to be checked\. Default value is 3\.

##### `-m, --msbuild-path <DIRECTORY_PATH>`

Defines a path to MSBuild directory\.

##### `--projects <PROJECT_NAME>`

Defines projects that should be analyzed\.

##### `-p, --properties <NAME=VALUE>`

Defines one or more MSBuild properties\.

##### `--scope <SCOPE>`

Defines syntax that should be analyzed\. Allowed values are comment, type, member, local, parameter, literal, non\-symbol, symbol and all\. Literals are not analyzed by default\.

##### `-v, --verbosity <LEVEL>`

Verbosity of the log\. Allowed values are q\[uiet\], m\[inimal\], n\[ormal\], d\[etailed\] and diag\[nostic\]\.

##### `--visibility <VISIBILITY>`

Defines a  maximal visibility of a symbol to be fixable\. Allowed values are public \(default\), internal or private\.

##### `--words <PATH>`

Specified path to file and/or directory that contains list of allowed words\.

## Redirected/Piped Input

Redirected/piped input will be used as a list of project/solution paths separated with newlines.

## How to Suppress Spellchecking

Possible misspelling or typo is reported as a diagnostic `RCS2001`.
Thus it is possible to suppress it as any other diagnostic. 

## List of Allowed Words

* It is required to specify one or more wordlists (parameter `--words`).
* Wordlist is defined as a text file that contains list of values separated with newlines.
* Each value is either a valid word (for example `misspell`) or a fix in a format `<ERROR>: <FIX>` (for example `mispell: misspell`).
* Word matching is case-insensitive by default (use option `--case-sensitive` to specify case-sensitive matching).
* It is recommended to use [Wordb](https://github.com/JosefPihrt/Wordb/tree/main/data) wordlists that are specifically tailored to be used for spellchecking.

## Output

* Command output contains up to four lists in a following order:
  * Words containing unknown words - for example a method name that comprises multiple words where one or more of them is unknown such as `GetMaxWidht`.
  * Unknown words - List of words that were not found in any wordlist.
  * Auto fixes - List of automatically applied fixes.
  * User-applied fixes - List of fixes applied by the user (when `--interactive` is set).

These lists can be used to update wordlists so they match the code base more precisely.

NOTE: The verbosity must be set to `normal` (default) or higher for the output to contain these lists.

*\(Generated with [DotMarkdown](https://github.com/JosefPihrt/DotMarkdown)\)*