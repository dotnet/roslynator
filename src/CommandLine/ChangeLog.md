### 0.3.1 (2022-01-16)

* Add support for .NET 6

### 0.3.0 (2021-11-14)

* Bump Roslyn version to 4.0.1
* Add option `--max-word-length` to `spellcheck` command
* Add spellchecking of string literals
* Fix exit code so the tool can be used in CI/CD pipeline
  * Return 0 if no diagnostic is found

### 0.2.0 (2021-09-05)

* Add command [`rename-symbol`](https://github.com/JosefPihrt/Roslynator/blob/master/docs/cli/rename-symbol-command.md)
  * This command enables to rename multiple symbols in one batch.
* It is no longer required to specify path to MSBuild directory using `--msbuild-path` option.
  * Latest version will be selected by default.
* .NET Core CLI targets .NET 5.0

### 0.1.5 (2021-07-11)

* Add support for redirected/piped input
  * Redirected/piped input is treated as a list of project/solution paths separated with newlines.
  * Following commands are supported: `analyze`, `fix`, `format`, `list-symbols`, `loc`, `lloc` and `spellcheck`
* Bump Roslyn API version to 3.10.0
* Various improvements to `spellcheck` command

### 0.1.0-beta2 (2018-11-27)

* Add commands `analyze`, `format`, `loc` and `lloc`.

### 0.1.0-beta (2018-10-14)

* Initial release
