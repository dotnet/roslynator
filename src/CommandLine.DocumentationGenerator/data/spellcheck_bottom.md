## Redirected/Piped Input

Redirected/piped input will be used as a list of project/solution paths separated with newlines.

## How to Suppress Spellchecking

Possible misspelling or typo is reported as a diagnostic `RCS2001`.
Thus it is possible to [suppress](../HowToSuppressDiagnostic.md) it as any other diagnostic. 

## List of Allowed Words

* It is required to specify one or more wordlists (parameter `--words`).
* Wordlist is defined as a text file that contains list of values separated with newlines.
* Each value is either a valid word (for example `misspell`) or a fix in a format `<ERROR>: <FIX>` (for example `mispell: misspell`).
* Word matching is case-insensitive by default (use option `--case-sensitive` to specify case-sensitive matching).
* It is recommended to use [Wordb](https://github.com/JosefPihrt/Wordb/tree/main/data) wordlists that are specifically tailored to be used for spellchecking.