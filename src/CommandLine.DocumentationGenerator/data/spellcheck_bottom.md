## Redirected/Piped Input

Redirected or piped input is treated as a list of project/solution paths, one per line.

## How to Suppress Spellchecking

Misspellings and typos are reported as diagnostic `RCS2001`.
You can suppress it like any other diagnostic.

## List of Allowed Words

* Specify one or more wordlists with the `--words` parameter.
* A wordlist is a text file containing values separated by newlines.
* Each value is either a valid word (for example, `misspell`) or a fix in the format `<ERROR>: <FIX>` (for example, `mispell: misspell`).
* Word matching is case-insensitive by default (use `--case-sensitive` for case-sensitive matching).
* [Wordb](https://github.com/josefpihrt/Wordb/tree/main/data) wordlists are tailored for spellchecking.

## Output

* Command output contains up to four lists in the following order:
  * Lines containing unknown words — for example, a method name made up of multiple words where one or more is unknown, such as `GetMaxWidht`.
  * Unknown words — words not found in any wordlist.
  * Auto fixes — automatically applied fixes.
  * User-applied fixes — fixes applied by the user (when `--interactive` is set).

Use these lists to update wordlists so they match the codebase more closely.

NOTE: Set verbosity to `normal` (default) or higher for the output to include these lists.