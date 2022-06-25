---
sidebar_label: migrate
---

# `roslynator migrate`

Migrates analyzers to a new version\.

## Synopsis

```
roslynator migrate <PATH>
-d, --dry-run
-h, --help
    --identifier <IDENTIFIER>
    --target-version <VERSION>
-v, --verbosity <LEVEL>
```

## Arguments

**`<PATH>`**

A path to a directory, project file or a ruleset file\.

## Options

##### `-d, --dry-run`

Migrate analyzers to a new version but do not save changes to a disk\.

##### `-h, --help`

Show command line help\.

##### `--identifier <IDENTIFIER>`

Identifier of a package to be migrated\.

##### `--target-version <VERSION>`

A package version to migrate to\.

##### `-v, --verbosity <LEVEL>`

Verbosity of the log\. Allowed values are q\[uiet\], m\[inimal\], n\[ormal\], d\[etailed\] and diag\[nostic\]\.

*\(Generated with [DotMarkdown](https://github.com/JosefPihrt/DotMarkdown)\)*