# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]

### Changed

- [CLI] Bump Roslyn to 4.6.0 ([#1106](https://github.com/josefpihrt/roslynator/pull/1106)).
- Migrate all scripts to PowerShell ([#1111](https://github.com/josefpihrt/roslynator/pull/1111)).

### Fixed

- Fix [RCS1016](https://github.com/JosefPihrt/Roslynator/blob/main/docs/analyzers/RCS1016.md) ([#1090](https://github.com/josefpihrt/roslynator/pull/1090)).
- Improve inversion of logical expressions to handling additional cases ([#1086](https://github.com/josefpihrt/roslynator/pull/1086)).
- Fix [RCS1084](https://github.com/JosefPihrt/Roslynator/blob/main/docs/analyzers/RCS1084.md) ([#1085](https://github.com/josefpihrt/roslynator/pull/1085)).
- Fix [RCS1169](https://github.com/JosefPihrt/Roslynator/blob/main/docs/analyzers/RCS1169.md) ([#1092](https://github.com/JosefPihrt/Roslynator/pull/1092)).
- Recognize more shapes of IAsyncEnumerable as being Async ([RCS1047](https://github.com/JosefPihrt/Roslynator/blob/main/docs/analyzers/RCS1047.md)) ([#1084](https://github.com/josefpihrt/roslynator/pull/1084)).
- Fix [RCS1197](https://github.com/JosefPihrt/Roslynator/blob/main/docs/analyzers/RCS1197.md) ([#1093](https://github.com/JosefPihrt/Roslynator/pull/1093)).
- Fix [RCS1056](https://github.com/JosefPihrt/Roslynator/blob/main/docs/analyzers/RCS1056.md) ([#1096](https://github.com/JosefPihrt/Roslynator/pull/1096)).
- Fix [RCS1216](https://github.com/JosefPihrt/Roslynator/blob/main/docs/analyzers/RCS1216.md) ([#1094](https://github.com/JosefPihrt/Roslynator/pull/1094)).
- Fix [RCS1146](https://github.com/JosefPihrt/Roslynator/blob/main/docs/analyzers/RCS1146.md) ([#1098](https://github.com/JosefPihrt/Roslynator/pull/1098)).
- Fix [RCS1154](https://github.com/JosefPihrt/Roslynator/blob/main/docs/analyzers/RCS1154.md) ([#1105](https://github.com/JosefPihrt/Roslynator/pull/1105)).
- Fix [RCS1211](https://github.com/JosefPihrt/Roslynator/blob/main/docs/analyzers/RCS1211.md) ([#1095](https://github.com/JosefPihrt/Roslynator/pull/1095)).

## [4.3.0] - 2023-04-24

### Changed

- [CLI] Bump Roslyn to 4.5.0 ([#1043](https://github.com/josefpihrt/roslynator/pull/1043)).
- [CLI] Downgrade version of Microsoft.Build.Locator from 1.5.5 to 1.4.1 ([#1079](https://github.com/JosefPihrt/Roslynator/pull/1079))
- [CLI] Add more information about the found diagnostics to the XML output file ([#1078](https://github.com/josefpihrt/roslynator/pull/1078) by @PeterKaszab).

### Fixed

- Fix [RCS1084](https://github.com/JosefPihrt/Roslynator/blob/main/docs/analyzers/RCS1084.md) ([#1006](https://github.com/josefpihrt/roslynator/pull/1006)).
- Fix [RCS1244](https://github.com/JosefPihrt/Roslynator/blob/main/docs/analyzers/RCS1244.md) ([#1007](https://github.com/josefpihrt/roslynator/pull/1007)).
- [CLI] Add nullable reference type modifier when creating a list of symbols (`list-symbols` command) ([#1013](https://github.com/josefpihrt/roslynator/pull/1013)).
- Add/remove blank line after file scoped namespace declaration ([RCS0060](https://github.com/JosefPihrt/Roslynator/blob/main/docs/analyzers/RCS0060.md)) ([#1014](https://github.com/josefpihrt/roslynator/pull/1014)).
- Do not remove overriding member in record ([RCS1132](https://github.com/JosefPihrt/Roslynator/blob/main/docs/analyzers/RCS1132.md)) ([#1015](https://github.com/josefpihrt/roslynator/pull/1015)).
- Do not remove parameterless empty constructor in a struct with field initializers ([RCS1074](https://github.com/JosefPihrt/Roslynator/blob/main/docs/analyzers/RCS1074.md)) ([#1021](https://github.com/josefpihrt/roslynator/pull/1021)).
- Do not suggest to use generic event handler ([RCS1159](https://github.com/JosefPihrt/Roslynator/blob/main/docs/analyzers/RCS1159.md)) ([#1022](https://github.com/josefpihrt/roslynator/pull/1022)).
- Fix ([RCS1077](https://github.com/JosefPihrt/Roslynator/blob/main/docs/analyzers/RCS1077.md)) ([#1023](https://github.com/josefpihrt/roslynator/pull/1023)).
- Fix ([RCS1097](https://github.com/JosefPihrt/Roslynator/blob/main/docs/analyzers/RCS1097.md)) ([#1037](https://github.com/JosefPihrt/Roslynator/pull/1037) by @jamesHargreaves12).
- Do not report ([RCS1170](https://github.com/JosefPihrt/Roslynator/blob/main/docs/analyzers/RCS1170.md)) when `Microsoft.AspNetCore.Components.InjectAttribute` is used ([#1046](https://github.com/JosefPihrt/Roslynator/pull/1046)).
- Fix ([RCS1235](https://github.com/JosefPihrt/Roslynator/blob/main/docs/analyzers/RCS1235.md)) ([#1047](https://github.com/JosefPihrt/Roslynator/pull/1047)).
- Fix ([RCS1206](https://github.com/JosefPihrt/Roslynator/blob/main/docs/analyzers/RCS1206.md)) ([#1049](https://github.com/JosefPihrt/Roslynator/pull/1049)).
- Prevent possible recursion in ([RCS1235](https://github.com/JosefPihrt/Roslynator/blob/main/docs/analyzers/RCS1235.md)) ([#1054](https://github.com/JosefPihrt/Roslynator/pull/1054)).
- Fix ([RCS1223](https://github.com/JosefPihrt/Roslynator/blob/main/docs/analyzers/RCS1223.md)) ([#1051](https://github.com/JosefPihrt/Roslynator/pull/1051) by @jamesHargreaves12).
- Do not remove braces in the cases where there are overlapping local variables. ([RCS1031](https://github.com/JosefPihrt/Roslynator/blob/main/docs/analyzers/RCS1031.md), [RCS1211](https://github.com/JosefPihrt/Roslynator/blob/main/docs/analyzers/RCS1211.md), [RCS1208](https://github.com/JosefPihrt/Roslynator/blob/main/docs/analyzers/RCS1208.md), [RCS1061](https://github.com/JosefPihrt/Roslynator/blob/main/docs/analyzers/RCS1061.md)) ([#1039](https://github.com/JosefPihrt/Roslynator/pull/1039), [#1062](https://github.com/JosefPihrt/Roslynator/pull/1062) by @jamesHargreaves12).
- [CLI] Analyze command does not create the XML output file and returns incorrect exit code when only compiler diagnostics are reported ([#1056](https://github.com/JosefPihrt/Roslynator/pull/1056) by @PeterKaszab).
- [CLI] Fix exit code when multiple projects are processed ([#1061](https://github.com/JosefPihrt/Roslynator/pull/1061) by @PeterKaszab).
- Fix code fix for CS0164 ([#1031](https://github.com/JosefPihrt/Roslynator/pull/1031) by @jamesHargreaves12).
- Do not report `System.Windows.DependencyPropertyChangedEventArgs` as unused parameter ([RCS1163](https://github.com/JosefPihrt/Roslynator/blob/main/docs/analyzers/RCS1163.md)) ([#1068](https://github.com/JosefPihrt/Roslynator/pull/1068)).
- Fix ([RCS1032](https://github.com/JosefPihrt/Roslynator/blob/main/docs/analyzers/RCS1032.md)) ([#1064](https://github.com/JosefPihrt/Roslynator/pull/1064) by @jamesHargreaves12).
- Update processing of .globalconfig file to prioritize file-specific diagnostic severities over global diagnostic severities ([#1066](https://github.com/JosefPihrt/Roslynator/pull/1066/files) by @jamesHargreaves12).
- Fix RCS1009 to handles discard designations ([#1063](https://github.com/JosefPihrt/Roslynator/pull/1063/files) by @jamesHargreaves12).
- [CLI] Fix number of formatted documents, file banners added ([#1072](https://github.com/JosefPihrt/Roslynator/pull/1072)).
- Improve support for coalesce expressions in code fixes that require computing the logical inversion of an expression, such as [RCS1208](https://github.com/JosefPihrt/Roslynator/blob/main/docs/analyzers/RCS1208.md) ([#1069](https://github.com/JosefPihrt/Roslynator/pull/1069) by @jamesHargreaves12).

## [4.2.0] - 2022-11-27

### Added

- Add Arm64 VS 2022 extension support ([#990](https://github.com/JosefPihrt/Roslynator/pull/990) by @snickler).
- Add analyzer "Add/remove blank line after file scoped namespace declaration" ([RCS0060](https://github.com/JosefPihrt/Roslynator/blob/main/docs/analyzers/RCS0060.md)) ([#993](https://github.com/josefpihrt/roslynator/pull/993)).
  - Required option: `roslynator_blank_line_after_file_scoped_namespace_declaration = true|false`
  - Not enabled by default.
- Add analyzer "Simplify argument null check" ([RCS1255](https://github.com/JosefPihrt/Roslynator/blob/main/docs/analyzers/RCS1255.md)) ([#994](https://github.com/JosefPihrt/Roslynator/pull/994)).
  - Use `ArgumentNullException.ThrowIfNull` instead of `if` null check.
  - Not enabled by default.
- Add analyzer "Invalid argument null check" ([RCS1256](https://github.com/JosefPihrt/Roslynator/blob/main/docs/analyzers/RCS1256.md)) ([#888](https://github.com/JosefPihrt/Roslynator/pull/888)).
  - This analyzer reports null checks of arguments that are:
    - annotated as nullable reference type.
    - optional and its default value is `null`.
- Add package `Roslynator.Testing.CSharp.MSTest` ([#997](https://github.com/JosefPihrt/Roslynator/pull/997)).

### Changed

- Disable [RCS1080](https://github.com/JosefPihrt/Roslynator/blob/main/docs/analyzers/RCS1080.md) by default ([#980](https://github.com/josefpihrt/roslynator/pull/980)).
- [CLI] Bump Roslyn to 4.4.0 ([#998](https://github.com/josefpihrt/roslynator/pull/998)).
- [CLI] Add support for .NET 7 and remove support for .NET 5 ([#985](https://github.com/josefpihrt/roslynator/pull/985).

### Fixed

- Fix [RCS1080](https://github.com/JosefPihrt/Roslynator/blob/main/docs/analyzers/RCS1080.md) when collection is derived from `List<T>` ([#986](https://github.com/josefpihrt/roslynator/pull/986)).
- Fix retrieving of trusted platform assemblies - separator differs by OS ([#987](https://github.com/josefpihrt/roslynator/pull/987)).
- Fix refactoring ([RR0014](https://github.com/JosefPihrt/Roslynator/blob/main/docs/analyzers/RR0014.md)) ([#988](https://github.com/josefpihrt/roslynator/pull/988)).
- Fix refactoring ([RR0180](https://github.com/JosefPihrt/Roslynator/blob/main/docs/analyzers/RR0180.md)) ([#988](https://github.com/josefpihrt/roslynator/pull/988)).
- Recognize `ArgumentNullException.ThrowIfNull` ([RCS1227](https://github.com/JosefPihrt/Roslynator/blob/main/docs/analyzers/RCS1227.md)) ([#992](https://github.com/josefpihrt/roslynator/pull/992)).
- Detect pattern matching in [RCS1146](https://github.com/JosefPihrt/Roslynator/blob/main/docs/analyzers/RCS1146.md) ([#999](https://github.com/josefpihrt/roslynator/pull/999)).
- Handle `using` directive that starts with `global::` [RCS0015](https://github.com/JosefPihrt/Roslynator/blob/main/docs/analyzers/RCS0015.md) ([#1000](https://github.com/josefpihrt/roslynator/pull/1000)).
- [VS Extension] Reference all assemblies as 'Analyzer' and 'MefComponent' in vsix manifest ([#1002](https://github.com/josefpihrt/roslynator/pull/1002)).
  - Fixes `AD0001` with `System.IO.FileNotFoundException` on Visual Studio 17.4 and later.

## [4.1.2] - 2022-10-31

### Added

- Convert more syntax to implicit object creation (RCS1250) ([#910](https://github.com/josefpihrt/roslynator/pull/910)).
- Add code fix for CS0037 ([#929](https://github.com/josefpihrt/roslynator/pull/929)).
- [CLI] Generate reference documentation that can be published with Docusaurus ([#918](https://github.com/josefpihrt/roslynator/pull/918)).
  - `roslynator generate-doc --host docusaurus`
- [CLI] Generate reference documentation that can be published with Sphinx ([#961](https://github.com/josefpihrt/roslynator/pull/961)).
  - `roslynator generate-doc --host sphinx`
- [CLI] Basic support for `<inheritdoc />` when generating documentation (`generate-doc` command) ([#972](https://github.com/josefpihrt/roslynator/pull/972)).
- [CLI] Add option `ignored-title-parts` (`generate-doc` command) ([#975](https://github.com/josefpihrt/roslynator/pull/975)).
- Publish Roslynator to [Open VSX Registry](https://open-vsx.org/extension/josefpihrt-vscode/roslynator) ([#820](https://github.com/JosefPihrt/Roslynator/issues/820)).

### Changed

- Rename default branch to `main`.
- Format changelog according to 'Keep a Changelog' ([#915](https://github.com/josefpihrt/roslynator/pull/915)).
- [CLI] Improve release build of command-line tool ([#912](https://github.com/josefpihrt/roslynator/pull/912)).
- Do not sort properties in an initializer ([RR0216](https://github.com/JosefPihrt/Roslynator/blob/main/docs/refactorings/RR0216.md)).
- [CLI] Bump Roslyn to 4.3.1 ([#969](https://github.com/josefpihrt/roslynator/pull/969)).
- [CLI] Bump Microsoft.Build.Locator to 1.5.5 ([#969](https://github.com/josefpihrt/roslynator/pull/969)).

### Fixed

- [CLI] Fix filtering of projects (relates to `--projects` or `--ignored-projects` parameter) ([#914](https://github.com/josefpihrt/roslynator/pull/914)).
- Refactoring "Add using directive" (RR0014) now works when file-scoped namespace is used ([#932](https://github.com/josefpihrt/roslynator/pull/932)).
- Add parentheses if necessary in a code fix for [RCS1197](https://github.com/JosefPihrt/Roslynator/blob/main/docs/analyzers/RCS1197.md) ([#928](https://github.com/josefpihrt/roslynator/pull/928) by @karl-sjogren).
- Do not simplify default expression if it would change semantics ([RCS1244](https://github.com/JosefPihrt/Roslynator/blob/main/docs/analyzers/RCS1244.md)) ([#939](https://github.com/josefpihrt/roslynator/pull/939).
- Fix NullReferenceException in [RCS1198](https://github.com/JosefPihrt/Roslynator/blob/main/docs/analyzers/RCS1198.md) ([#940](https://github.com/josefpihrt/roslynator/pull/940).
- Order named arguments even if optional arguments are not specified [RCS1205](https://github.com/JosefPihrt/Roslynator/blob/main/docs/analyzers/RCS1205.md) ([#941](https://github.com/josefpihrt/roslynator/pull/941), ([#965](https://github.com/josefpihrt/roslynator/pull/965).
- Prefix identifier with `@` if necessary ([RCS1220](https://github.com/JosefPihrt/Roslynator/blob/main/docs/analyzers/RCS1220.md)) ([#943](https://github.com/josefpihrt/roslynator/pull/943).
- Do not suggest to make local variable a const when it is used in ref extension method ([RCS1118](https://github.com/JosefPihrt/Roslynator/blob/main/docs/analyzers/RCS1118.md)) ([#948](https://github.com/josefpihrt/roslynator/pull/948).
- Fix formatting of argument list ([#952](https://github.com/josefpihrt/roslynator/pull/952).
- Do not remove async/await when 'using declaration' is used ([#953](https://github.com/josefpihrt/roslynator/pull/953).
- Convert if-else to return statement when pattern matching is used ([RCS1073](https://github.com/JosefPihrt/Roslynator/blob/main/docs/analyzers/RCS1073.md)) ([#956](https://github.com/josefpihrt/roslynator/pull/956).
- [CLI] Include compiler diagnostics in the xml output file of the `roslynator analyze` command ([#964](https://github.com/JosefPihrt/Roslynator/pull/964) by @PeterKaszab).
- Do not simplify 'default' expression if the type is inferred ([RCS1244](https://github.com/JosefPihrt/Roslynator/blob/main/docs/analyzers/RCS1244.md)) ([#966](https://github.com/josefpihrt/roslynator/pull/966).
- Use explicit type from lambda expression ([RCS1008](https://github.com/JosefPihrt/Roslynator/blob/main/docs/analyzers/RCS1008.md)) ([#967](https://github.com/josefpihrt/roslynator/pull/967).
- Do not remove constructor if it is decorated with 'UsedImplicitlyAttribute' ([RCS1074](https://github.com/JosefPihrt/Roslynator/blob/main/docs/analyzers/RCS1074.md)) ([#968](https://github.com/josefpihrt/roslynator/pull/968).
- Detect argument null check in the form of `ArgumentNullException.ThrowIfNull` ([RR0025](https://github.com/JosefPihrt/Roslynator/blob/main/docs/refactorings/RR0025.md), [RCS1227](https://github.com/JosefPihrt/Roslynator/blob/main/docs/analyzers/RCS1227.md)) ([#974](https://github.com/josefpihrt/roslynator/pull/974).
- Do not make generic class static if it's inherited ([RCS1102](https://github.com/JosefPihrt/Roslynator/blob/main/docs/analyzers/RCS1102.md)) ([#978](https://github.com/josefpihrt/roslynator/pull/978).

-----
<!-- Content below does not adhere to 'Keep a Changelog' format -->

### 4.1.1 (2022-05-29)

* Bug fixes

### 4.1.0 (2022-03-29)

* Add analyzer [RCS1254](https://github.com/JosefPihrt/Roslynator/blob/main/docs/analyzers/RCS1254.md) (Normalize format of enum flag value)
* Add analyzer [RCS1253](https://github.com/JosefPihrt/Roslynator/blob/main/docs/analyzers/RCS1253.md) (Format documentation comment summary)
* Add analyzer [RCS1252](https://github.com/JosefPihrt/Roslynator/blob/main/docs/analyzers/RCS1252.md) (Normalize usage of infinite loop)
* Add analyzer [RCS1251](https://github.com/JosefPihrt/Roslynator/blob/main/docs/analyzers/RCS1251.md) (Remove unnecessary braces from record declaration)
* Add refactoring [Deconstruct foreach variable (RR0217)](https://github.com/JosefPihrt/Roslynator/blob/main/docs/refactoring/RR0217.md)
* Add code fix for CS8602, CS8604
* Add suggestion to call AddRange instead of Add (RCS1235)
* Put back refactoring "Split local declaration and assignment" (RR0194) ([issue](https://github.com/JosefPihrt/Roslynator/issues/881))
* Adopt activation events in VS Code extension ([issue](https://github.com/JosefPihrt/Roslynator/issues/883)) (thanks to [ProphetLamb](https://github.com/ProphetLamb))
* Fix: Get config value from global AnalyzerConfig if available ([issue](https://github.com/JosefPihrt/Roslynator/issues/884))
* Fix: Do not suggest using null-forgiving operator for parameter default value (CS8625)
* Fix: Check if equality operator is overloaded (RCS1171)
* Fix: Do not remove field initialization in struct with constructor(s) (RCS1129)

### 4.0.3 (2022-01-29)

* Fixed release for VS Code

### 4.0.2 (2022-01-29)

* Disable analyzer ROS003 by default ([commit](https://github.com/JosefPihrt/Roslynator/commit/9c562921b6ae4eb46e1cfe252282e6b2ad520ca6))
* Analyzers that require option to be set should disabled by default (RCS1018, RCS1096, RCS1250) ([commit](https://github.com/JosefPihrt/Roslynator/commit/de374858f9d8120a6f6d705ad685101ed1bab699))

#### Bug fixes

* Fix analyzer [RCS1014](https://github.com/JosefPihrt/Roslynator/blob/main/docs/analyzers/RCS1014.md) (Use explicitly/implicitly typed array) ([commit](https://github.com/JosefPihrt/Roslynator/commit/004a83756b9fbcf117710d7afb6bab964a59f1be))
* Fix analyzer [RCS1016](https://github.com/JosefPihrt/Roslynator/blob/main/docs/analyzers/RCS1016.md) (Use block body or expression body) ([commit](https://github.com/JosefPihrt/Roslynator/commit/8c633e966f2706d3888fd942dd186d066d440ac0))
* Fix refactoring AddUsingDirective (RR0013) ([commit](https://github.com/JosefPihrt/Roslynator/commit/199787bdf921aeeecd69d9a118bbb86419bce81a))
* Propagate some options from .roslynatorconfig ([commit](https://github.com/JosefPihrt/Roslynator/commit/a619ebf285d1de77941a9c4a5fce46bb19485d3a))
* Enable ROS analyzers to be set from .roslynatorconfig ([commit](https://github.com/JosefPihrt/Roslynator/commit/a4c0ad8fb60d694cc7d2546016d742547f1d585b))
* Files generated with source generators have relative paths ([commit](https://github.com/JosefPihrt/Roslynator/commit/cec55ab23404a11f4fe332a3568ab87a4016e55b))

### 4.0.1 (2022-01-21)

* Bug fixes

### 4.0.0 (2022-01-16)

* Bump Roslyn version to 4.0.1
* Change category of all analyzers to 'Roslynator'
* Migrate all options to EditorConfig
  * Enable/disable all analyzers
  * Enable/disable all refactorings
  * Enable/disable specific refactoring
  * Enable/disable all compiler diagnostics fixes
  * Enable/disable specific compiler diagnostic fix

* Add analyzer [RCS0057](https://github.com/JosefPihrt/Roslynator/blob/main/docs/analyzers/RCS0057.md) (Normalize whitespace at the beginning of a file)
* Add analyzer [RCS0058](https://github.com/JosefPihrt/Roslynator/blob/main/docs/analyzers/RCS0058.md) (Normalize whitespace at the end of a file)
* Add analyzer [RCS0059](https://github.com/JosefPihrt/Roslynator/blob/main/docs/analyzers/RCS0059.md) (Place new line after/before null-conditional operator)
* Add analyzer [RCS1249](https://github.com/JosefPihrt/Roslynator/blob/main/docs/analyzers/RCS1249.md) (Unnecessary null-forgiving operator)
* Add analyzer [RCS1250](https://github.com/JosefPihrt/Roslynator/blob/main/docs/analyzers/RCS1250.md) (Use implicit/explicit object creation)
* Add refactoring ExpandPositionalRecord (RR0215)
* Add refactoring AddAllPropertiesToInitializer (RR0216)
* Add code fix for CS8403, CS8618 and CS8625

### 3.3.0 (2021-11-14)

* Bunch of bug fixes and small improvements
* Disable analyzers RCS1079 and RCS1090 by default

### 3.2.2 (2021-08-15)

* Ensure that shared assemblies with be loaded properly on .NET Core ([issue](https://github.com/JosefPihrt/Roslynator/issues/833))

### 3.2.1 (2021-06-30)

* Publish Roslynator for Visual Studio 2022 Preview
* Bug fixes and various improvements

### 3.2.0 (2021-04-26)

* Publish [Roslynator Testing Framework](https://www.nuget.org/packages/Roslynator.Testing.CSharp.Xunit)
* Support editorconfig to configure analyzer options ([commit](https://github.com/JosefPihrt/Roslynator/commit/da88ce64e0b3975ad69e05a1d4cdcc761f358a09))
* Update references to Roslyn API to 3.8.0
* A bunch of bug fixes

#### Analyzers

* Add option to invert analyzer [RCS1016](https://github.com/JosefPihrt/Roslynator/blob/main/docs/analyzers/RCS1016.md) ([commit](https://github.com/JosefPihrt/Roslynator/commit/67a0fc5cfe9dd793cc6e504513ed6805678c1739))
* Add more cases to analyzer [RCS1218](https://github.com/JosefPihrt/Roslynator/blob/main/docs/analyzers/RCS1218.md) ([commit](https://github.com/JosefPihrt/Roslynator/commit/37e8edb7a2eefdd4a7749dd6a3f5b473ebbdcc0a))
* Convert `!= null` to `is not null` ([RCS1248](https://github.com/JosefPihrt/Roslynator/blob/main/docs/analyzers/RCS1248.md)) ([commit](https://github.com/JosefPihrt/Roslynator/commit/432a8fea3147447536dbb8fac47598ad1db68158))

#### Code Fixes

* Add code fix for CS7036 ([commit](https://github.com/JosefPihrt/Roslynator/commit/9eae7307b9cab96c2d91e97aef8bda098c7e92d9))
* Add code fix for CS8632 ([commit](https://github.com/JosefPihrt/Roslynator/commit/2c1d9ca64d2305e1ce278e1db6563d82582c4613))
* Improve code fix for CS0029, CS0246 ([commit](https://github.com/JosefPihrt/Roslynator/commit/5557ad29412b5f758cb97da6e298e1f4b0d49e3d))
* Add option for code fix for CS1591 ([commit](https://github.com/JosefPihrt/Roslynator/commit/089dbed656556a526f236dce75eadffb4e1d78a0))

### 3.1.0 (2021-01-04)

* Add analyzer [RCS0056](https://github.com/JosefPihrt/Roslynator/blob/main/docs/analyzers/RCS0056.md) (Line is too long)
* Add option to suppress diagnostic from Unity script methods (RCS1213)
* Consider syntax `var foo = Foo.Parse(value)` as having obvious type `Foo`
* Update references to Roslyn API to 3.7.0

### 3.0.1 (2020-10-19)

* Add analyzer [RCS0055](https://github.com/JosefPihrt/Roslynator/blob/main/docs/analyzers/RCS0055.md) (Fix formatting of a binary expression chain)
* Add analyzer [RCS0054](https://github.com/JosefPihrt/Roslynator/blob/main/docs/analyzers/RCS0054.md) (Fix formatting of a call chain)
* Add analyzer [RCS0053](https://github.com/JosefPihrt/Roslynator/blob/main/docs/analyzers/RCS0053.md) (Fix formatting of a list)
* Add analyzer [RCS0052](https://github.com/JosefPihrt/Roslynator/blob/main/docs/analyzers/RCS0052.md) (Add newline before equals sign instead of after it (or vice versa))
* Add analyzer [RCS1248](https://github.com/JosefPihrt/Roslynator/blob/main/docs/analyzers/RCS1248.md) (Use 'is null' pattern instead of comparison (or vice versa)) ([issue](https://github.com/JosefPihrt/Roslynator/issues/458))
* Add analyzer [RCS1247](https://github.com/JosefPihrt/Roslynator/blob/main/docs/analyzers/RCS1247.md) (Fix documentation comment tag)
* Add analyzer option [RCS1207i](https://github.com/JosefPihrt/Roslynator/blob/main/docs/analyzers/RCS1207i.md) (Convert method group to anonymous function)
* Add analyzer option [RCS1090i](https://github.com/JosefPihrt/Roslynator/blob/main/docs/analyzers/RCS1090i.md) (Remove call to 'ConfigureAwait')
* Add analyzer option [RCS1018i](https://github.com/JosefPihrt/Roslynator/blob/main/docs/analyzers/RCS1018i.md) (Remove accessibility modifiers) ([issue](https://github.com/JosefPihrt/Roslynator/issues/260))
* Add analyzer option [RCS1014i](https://github.com/JosefPihrt/Roslynator/blob/main/docs/analyzers/RCS1014i.md) (Use implicitly typed array)
* Add analyzer option [RCS1014a](https://github.com/JosefPihrt/Roslynator/blob/main/docs/analyzers/RCS1014a.md) (Use implicitly typed array (when type is obvious))
* Add analyzer option [RCS1078i](https://github.com/JosefPihrt/Roslynator/blob/main/docs/analyzers/RCS1078i.md) (Use string.Empty instead of "")
* Add analyzer option [RCS1016a](https://github.com/JosefPihrt/Roslynator/blob/main/docs/analyzers/RCS1016a.md) (Convert expression-body to block body when expression is multi-line)
* Add analyzer option [RCS1016b](https://github.com/JosefPihrt/Roslynator/blob/main/docs/analyzers/RCS1016b.md) (Convert expression-body to block body when declaration is multi-line)
* Disable by default analyzer [RCS1207i](https://github.com/JosefPihrt/Roslynator/blob/main/docs/analyzers/RCS1207i.md) (Convert method group to anonymous function)
* Remove analyzer [RCS1219](https://github.com/JosefPihrt/Roslynator/blob/main/docs/analyzers/RCS1219.md) (Call 'Enumerable.Skip' and 'Enumerable.Any' instead of 'Enumerable.Count')
* Rename analyzer "Avoid 'null' on left side of binary expression" to "Constant values should be placed on right side of comparisons" [RCS1098](https://github.com/JosefPihrt/Roslynator/blob/main/docs/analyzers/RCS1098.md)
* Rename analyzer "Simplify boolean expression" to "Unnecessary null check" [RCS1199](https://github.com/JosefPihrt/Roslynator/blob/main/docs/analyzers/RCS1199.md) ([issue](https://github.com/JosefPihrt/Roslynator/issues/373))

* More syntax is considered as having obvious type:
  * string literal
  * character literal
  * boolean literal
  * implicit array creation that contains only expressions whose type is obvious

### 3.0.0 (2020-06-16)

* Update references to Roslyn API to 3.5.0
* Release .NET Core Global Tool [Roslynator.DotNet.Cli](https://www.nuget.org/packages/roslynator.dotnet.cli) 
* Introduce concept of "[Analyzer Options](https://github.com/JosefPihrt/Roslynator/blob/main/docs/AnalyzerOptions.md)"
* Reassign ID for some analyzers.
  * See "[How to: Migrate Analyzers to Version 3.0](https://github.com/JosefPihrt/Roslynator/blob/main/docs/HowToMigrateAnalyzersToVersion3.md)"
* Remove references to Roslynator assemblies from omnisharp.json on uninstall (VS Code)

#### New Analyzers

* [RCS0048](https://github.com/JosefPihrt/Roslynator/blob/main/docs/analyzers/RCS0048.md) (Remove newlines from initializer with single\-line expression).
* [RCS0049](https://github.com/JosefPihrt/Roslynator/blob/main/docs/analyzers/RCS0049.md) (Add empty line after top comment).
* [RCS0050](https://github.com/JosefPihrt/Roslynator/blob/main/docs/analyzers/RCS0050.md) (Add empty line before top declaration).
* [RCS0051](https://github.com/JosefPihrt/Roslynator/blob/main/docs/analyzers/RCS0051.md) (Add newline between closing brace and 'while' keyword \(or vice versa\)).
* [RCS1246](https://github.com/JosefPihrt/Roslynator/blob/main/docs/analyzers/RCS1246.md) (Use element access).

#### New Refactorings

* [RR0214](https://github.com/JosefPihrt/Roslynator/blob/main/docs/refactorings/RR0214.md) (Convert 'switch' expression to 'switch' statement).

### 2.9.0 (2020-03-13)

* Switch to Roslyn 3.x libraries
* Add `Directory.Build.props` file
* Add open configuration commands to Command Palette (VS Code) ([PR](https://github.com/JosefPihrt/Roslynator/pull/648))

#### Bug Fixes

* Fix key duplication/handle camel case names in `omnisharp.json` ([PR](https://github.com/JosefPihrt/Roslynator/pull/645))
* Use prefix unary operator instead of postfix unary operator ([RCS1089](https://github.com/JosefPihrt/Roslynator/blob/main/docs/analyzers/RCS1089.md)) ([issue](https://github.com/JosefPihrt/Roslynator/issues/639))
* Cast of `this` to its interface cannot be null ([RCS1202](https://github.com/JosefPihrt/Roslynator/blob/main/docs/analyzers/RCS1202.md)) ([issue](https://github.com/JosefPihrt/Roslynator/issues/640))
* Do not remove braces in switch section if it contains 'using variable' ([RCS1031](https://github.com/JosefPihrt/Roslynator/blob/main/docs/analyzers/RCS1031.md)) ([issue](https://github.com/JosefPihrt/Roslynator/issues/632))

#### New Analyzers

* [RCS1242](https://github.com/JosefPihrt/Roslynator/blob/main/docs/analyzers/RCS1242.md) (DoNotPassNonReadOnlyStructByReadOnlyReference).
* [RCS1243](https://github.com/JosefPihrt/Roslynator/blob/main/docs/analyzers/RCS1243.md) (DuplicateWordInComment).
* [RCS1244](https://github.com/JosefPihrt/Roslynator/blob/main/docs/analyzers/RCS1244.md) (SimplifyDefaultExpression).
* [RCS1245](https://github.com/JosefPihrt/Roslynator/blob/main/docs/analyzers/RCS1245.md) (SimplifyConditionalExpression2) ([issue](https://github.com/JosefPihrt/Roslynator/issues/612)).

#### Analyzers

* Disable analyzer [RCS1057](https://github.com/JosefPihrt/Roslynator/blob/main/docs/analyzers/RCS1057.md) by default ([issue](https://github.com/JosefPihrt/Roslynator/issues/590)).
* Merge analyzer [RCS1156](https://github.com/JosefPihrt/Roslynator/blob/main/docs/analyzers/RCS1156.md) with [RCS1113](https://github.com/JosefPihrt/Roslynator/blob/main/docs/analyzers/RCS1113.md) ([issue](https://github.com/JosefPihrt/Roslynator/issues/650)).
  * `x == ""` should be replaced with `string.IsNullOrEmpty(x)`
* Improve analyzer [RCS1215](https://github.com/JosefPihrt/Roslynator/blob/main/docs/analyzers/RCS1215.md) ([commit](https://github.com/JosefPihrt/Roslynator/commit/0fdd97f9a62463f8b004abeb17a8b8509374c35a)).
  * `x == double.NaN` should be replaced with `double.IsNaN(x)`
* Enable [RCS1169](https://github.com/JosefPihrt/Roslynator/blob/main/docs/analyzers/RCS1169.md) and [RCS1170](https://github.com/JosefPihrt/Roslynator/blob/main/docs/analyzers/RCS1170.md) if the type is read-only struct ([commit](https://github.com/JosefPihrt/Roslynator/commit/f34e105433dbc65686369adf712b0b99d93eaef7)).
* Improve analyzer [RCS1077](https://github.com/JosefPihrt/Roslynator/blob/main/docs/analyzers/RCS1077.md) ([commit](https://github.com/JosefPihrt/Roslynator/commit/3ee275442cb16f6a9104b42d582ba7d76d6df88c)).
  * `x.OrderBy(y => y).Reverse()` can be simplified to `x.OrderByDescending(y => y)`
  * `x.SelectMany(y => y).Count()` can be simplified to `x.Sum(y => y.Count)` if `x` has `Count` or `Length` property
* Improve analyzer [RCS1161](https://github.com/JosefPihrt/Roslynator/blob/main/docs/analyzers/RCS1161.md) - Declare explicit enum value using `<<` operator ([commit](https://github.com/JosefPihrt/Roslynator/commit/6b78496efe1a2f2678f2ef2a71986e2bee006863)).
* Improve analyzer [RCS1036](https://github.com/JosefPihrt/Roslynator/blob/main/docs/analyzers/RCS1036.md) - remove empty line between documentation comment and declaration ([commit](https://github.com/JosefPihrt/Roslynator/commit/de0f1205671281679866e92edd9337a7416409e6)).
* Improve analyzer [RCS1037](https://github.com/JosefPihrt/Roslynator/blob/main/docs/analyzers/RCS1037.md) - remove trailing white-space from documentation comment ([commit](https://github.com/JosefPihrt/Roslynator/commit/c3f7d193ee37d04de7e2c698aab7f3e1e6350e80)).
* Improve analyzer [RCS1143](https://github.com/JosefPihrt/Roslynator/blob/main/docs/analyzers/RCS1143.md) ([commit](https://github.com/JosefPihrt/Roslynator/commit/4c4281ebdf8eb0aa1a77d5e5bfda71bc66cce1df))
  * `x?.M() ?? default(int?)` can be simplified to `x?.M()` if `x` is a nullable struct.
* Improve analyzer [RCS1206](https://github.com/JosefPihrt/Roslynator/blob/main/docs/analyzers/RCS1206.md) ([commit](https://github.com/JosefPihrt/Roslynator/commit/88dd4cea4df07f036a8296511410ccff70f8fefe))
  * `(x != null) ? x.M() : default(int?)` can be simplified to `x?.M()` if `x` is a nullable struct.

### 2.3.1 (2020-01-20)

* Last release of package Roslynator.Analyzers (2.3.0) that references Roslyn 2.x (VS 2017)

### 2.3.0 (2019-12-28)

* Last release of Roslynator for VS 2017
* Automatically update configuration in omnisharp.json (VS Code) ([PR](https://github.com/JosefPihrt/Roslynator/pull/623)).

### 2.2.1 (2019-10-26)

* Add set of formatting analyzers (RCS0...).

### 2.2.0 (2019-09-28)

* Enable configuration for non-Windows systems (VS Code).

#### Analyzers

* Disable analyzer [RCS1029](https://github.com/JosefPihrt/Roslynator/blob/main/docs/analyzers/RCS1029.md) (FormatBinaryOperatorOnNextLine) by default.

## 2.1.4 (2019-08-13)

* Initial release of Roslynator for VS Code.

### 2.1.3 (2019-08-06)

#### Analyzers

* Publish package [Roslynator.CodeAnalysis.Analyzers 1.0.0-beta](https://www.nuget.org/packages/Roslynator.CodeAnalysis.Analyzers/1.0.0-beta)
* Add analyzer [RCS1236](https://github.com/JosefPihrt/Roslynator/blob/main/docs/analyzers/RCS1236.md) (UseExceptionFilter).
* Add analyzer [RCS1237](https://github.com/JosefPihrt/Roslynator/blob/main/docs/analyzers/RCS1237.md) (UseBitShiftOperator).
* Add analyzer [RCS1238](https://github.com/JosefPihrt/Roslynator/blob/main/docs/analyzers/RCS1238.md) (AvoidNestedConditionalOperators).
* Add analyzer [RCS1239](https://github.com/JosefPihrt/Roslynator/blob/main/docs/analyzers/RCS1239.md) (UseForStatementInsteadOfWhileStatement).
* Add analyzer [RCS1240](https://github.com/JosefPihrt/Roslynator/blob/main/docs/analyzers/RCS1240.md) (UnnecessaryOperator).
* Add analyzer [RCS1241](https://github.com/JosefPihrt/Roslynator/blob/main/docs/analyzers/RCS1241.md) (ImplementNonGenericCounterpart).

#### Refactorings

* Add refactoring [RR0213](https://github.com/JosefPihrt/Roslynator/blob/main/docs/refactorings/RR0213.md) (AddParameterToInterfaceMember)

### 2.1.1 (2019-05-13)

#### Analyzers

* Add analyzer [RCS1235](https://github.com/JosefPihrt/Roslynator/blob/main/docs/analyzers/RCS1235.md) (OptimizeMethodCall).
  * Incorporate [RCS1150](https://github.com/JosefPihrt/Roslynator/blob/main/docs/analyzers/RCS1150.md) and [RCS1178](https://github.com/JosefPihrt/Roslynator/blob/main/docs/analyzers/RCS1178.md) into [RCS1235](https://github.com/JosefPihrt/Roslynator/blob/main/docs/analyzers/RCS1235.md).
* Enable by default analyzer [RCS1023](https://github.com/JosefPihrt/Roslynator/blob/main/docs/analyzers/RCS1023.md) (FormatEmptyBlock) and change default severity to 'Hidden'.
* Change default severity of analyzer [RCS1168](https://github.com/JosefPihrt/Roslynator/blob/main/docs/analyzers/RCS1168.md) (ParameterNameDiffersFromBaseName) to 'Hidden'.

#### Refactorings

* Add refactoring [RR0212](https://github.com/JosefPihrt/Roslynator/blob/main/docs/refactorings/RR0212.md) (DuplicateSwitchSection)

### 2.1.0 (2019-03-25)

* Export/import Visual Studio options.

#### Analyzers

* Disable analyzer [RCS1231](https://github.com/JosefPihrt/Roslynator/blob/main/docs/analyzers/RCS1231.md) (MakeParameterRefReadOnly) by default.

#### Code Fixes

* Add code fixes for CS0191, CS0192, CS1012.

### 2.0.2 (2019-01-06)

* First release of Roslynator 2019 (for Visual Studio 2019)

#### New Features

* Support global suppression of diagnostics.
  * Go to Visual Studio Tools > Options > Roslynator > Global Suppressions

#### Analyzers

* Add analyzer [RCS1232](https://github.com/JosefPihrt/Roslynator/blob/main/docs/analyzers/RCS1232.md) (OrderElementsInDocumentationComment)
* Add analyzer [RCS1233](https://github.com/JosefPihrt/Roslynator/blob/main/docs/analyzers/RCS1233.md) (UseShortCircuitingOperator)
* Add analyzer [RCS1234](https://github.com/JosefPihrt/Roslynator/blob/main/docs/analyzers/RCS1234.md) (DuplicateEnumValue)

#### Refactorings

* Refactoring [RR0120](https://github.com/JosefPihrt/Roslynator/blob/main/docs/refactorings/RR0120.md) (ReplaceConditionalExpressionWithIfElse) can be applied recursively.
* Add refactoring [RR0022](https://github.com/JosefPihrt/Roslynator/blob/main/docs/refactorings/RR0022.md) (ChangeTypeAccordingToExpression)
* Add refactoring [RR0210](https://github.com/JosefPihrt/Roslynator/blob/main/docs/refactorings/RR0210.md) (ImplementCustomEnumerator)
* Add refactoring [RR0211](https://github.com/JosefPihrt/Roslynator/blob/main/docs/refactorings/RR0211.md) (ConvertStatementsToIfElse)

#### Code Fixes

* Add code fix for CS0029, CS0131, CS0621, CS3000, CS3001, CS3002, CS3003, CS3005, CS3006, CS3007, CS3008, CS3009, CS3016, CS3024, CS3027.

### 2.0.1 (2018-11-26)

#### Analyzers

* Add analyzer [RCS1230](https://github.com/JosefPihrt/Roslynator/blob/main/docs/analyzers/RCS1230.md) (UnnecessaryUsageOfEnumerator)
* Add analyzer [RCS1231](https://github.com/JosefPihrt/Roslynator/blob/main/docs/analyzers/RCS1231.md) (MakeParameterRefReadOnly)

### 2.0.0 (2018-10-14)

#### New Features

* Add nuget package [Roslynator.CommandLine](https://nuget.org/packages/Roslynator.CommandLine)
  * [Fix all diagnostics in a solution](https://github.com/JosefPihrt/Roslynator/blob/main/docs/HowToFixAllDiagnostics.md)
  * [Generate API documentation](https://github.com/JosefPihrt/Roslynator/blob/main/docs/HowToGenerateDocumentation.md)

#### Analyzers

* Change default severity of [RCS1141](https://github.com/JosefPihrt/Roslynator/blob/main/docs/analyzers/RCS1141.md), [RCS1142](https://github.com/JosefPihrt/Roslynator/blob/main/docs/analyzers/RCS1142.md) and [RCS1165](https://github.com/JosefPihrt/Roslynator/blob/main/docs/analyzers/RCS1165.md) to 'Hidden'
* Disable [RCS1174](https://github.com/JosefPihrt/Roslynator/blob/main/docs/analyzers/RCS1174.md) by default
* Improve analyzer [RCS1128](https://github.com/JosefPihrt/Roslynator/blob/main/docs/analyzers/RCS1128.md) - `x.GetValueOrDefault(y)` can be replaced with `x ?? y`
* Change code fix for [RCS1194](https://github.com/JosefPihrt/Roslynator/blob/main/docs/analyzers/RCS1194.md) - do not generate "serialization" constructor

#### Refactorings

* Add refactoring [RR0209](https://github.com/JosefPihrt/Roslynator/blob/main/docs/refactorings/RR0209.md) (RemoveAsyncAwait)

#### Code Fixes

* Add code fix for CS0119.

### 1.9.2 (2018-08-10)

#### Analyzers

* Add analyzer [RCS1228](https://github.com/JosefPihrt/Roslynator/blob/main/docs/analyzers/RCS1228.md) (UnusedElementInDocumentationComment)
* Add analyzer [RCS1229](https://github.com/JosefPihrt/Roslynator/blob/main/docs/analyzers/RCS1229.md) (UseAsyncAwait)
* Add code fix for analyzer [RCS1163](https://github.com/JosefPihrt/Roslynator/blob/main/docs/analyzers/RCS1163.md) (UnusedParameter)

#### Refactorings

* Add refactoring [RR0208](https://github.com/JosefPihrt/Roslynator/blob/main/docs/refactorings/RR0208.md) (AddTagToDocumentationComment)

#### Code Fixes

* Add code fixes for CS8050 and CS8139.

### 1.9.1 (2018-07-06)

#### Analyzers

* Add analyzer [RCS1227](https://github.com/JosefPihrt/Roslynator/blob/main/docs/analyzers/RCS1227.md) (ValidateArgumentsCorrectly)

#### Refactorings

* Add refactoring [RR0206](https://github.com/JosefPihrt/Roslynator/blob/main/docs/refactorings/RR0206.md) (ReplaceForEachWithEnumerator)
* Add refactoring [RR0207](https://github.com/JosefPihrt/Roslynator/blob/main/docs/refactorings/RR0207.md) (SortCaseLabels)
* Enable refactorings [RR0037](https://github.com/JosefPihrt/Roslynator/blob/main/docs/refactorings/RR0037.md) (ExpandExpressionBody) and [RR0169](https://github.com/JosefPihrt/Roslynator/blob/main/docs/refactorings/RR0169.md) (UseExpressionBodiedMember) for multiple members.
* Extend refactoring [RR0189](https://github.com/JosefPihrt/Roslynator/blob/main/docs/refactorings/RR0189.md) (ReduceIfNesting) and rename it to InvertIf.

### 1.9.0 (2018-06-13)

#### Analyzers

* Incorporate [RCS1082](https://github.com/JosefPihrt/Roslynator/blob/main/docs/analyzers/RCS1082.md), [RCS1083](https://github.com/JosefPihrt/Roslynator/blob/main/docs/analyzers/RCS1083.md), [RCS1109](https://github.com/JosefPihrt/Roslynator/blob/main/docs/analyzers/RCS1109.md), [RCS1119](https://github.com/JosefPihrt/Roslynator/blob/main/docs/analyzers/RCS1119.md), [RCS1120](https://github.com/JosefPihrt/Roslynator/blob/main/docs/analyzers/RCS1120.md) and [RCS1121](https://github.com/JosefPihrt/Roslynator/blob/main/docs/analyzers/RCS1121.md) into [RCS1077](https://github.com/JosefPihrt/Roslynator/blob/main/docs/analyzers/RCS1077.md)

#### Refactorings

* Disable [RR0010](https://github.com/JosefPihrt/Roslynator/blob/main/docs/refactorings/RR0010.md) and [RR0012](https://github.com/JosefPihrt/Roslynator/blob/main/docs/refactorings/RR0012.md) by default

### 1.8.3 (2018-05-17)

#### Analyzers

* Add analyzer [RCS1223](https://github.com/JosefPihrt/Roslynator/blob/main/docs/analyzers/RCS1223.md) (MarkTypeWithDebuggerDisplayAttribute)
* Add analyzer [RCS1224](https://github.com/JosefPihrt/Roslynator/blob/main/docs/analyzers/RCS1224.md) (MakeMethodExtensionMethod)
* Add analyzer [RCS1225](https://github.com/JosefPihrt/Roslynator/blob/main/docs/analyzers/RCS1225.md) (MakeSealedClass)
* Add analyzer [RCS1226](https://github.com/JosefPihrt/Roslynator/blob/main/docs/analyzers/RCS1226.md) (AddParagraphToDocumentationComment)
* Improve analyzer [RCS1146](https://github.com/JosefPihrt/Roslynator/blob/main/docs/analyzers/RCS1146.md) (UseConditionalAccess)
  * `x == null || x.y` can be simplified to `x?.y != false`
  * `x == null || !x.y` can be simplified to `x?.y != true`

#### Refactorings

* Improve refactoring [RR0051](https://github.com/JosefPihrt/Roslynator/blob/main/docs/refactorings/RR0051.md) (FormatExpressionChain)
  * A chain that contains conditional access (`x?.y`) will be properly formatted.

### 1.8.2 (2018-05-02)

#### Analyzers

* Add analyzer [RCS1220](https://github.com/JosefPihrt/Roslynator/blob/main/docs/analyzers/RCS1220.md) (UsePatternMatchingInsteadOfIsAndCast)
* Add analyzer [RCS1221](https://github.com/JosefPihrt/Roslynator/blob/main/docs/analyzers/RCS1221.md) (UsePatternMatchingInsteadOfAsAndNullCheck)
* Add analyzer [RCS1222](https://github.com/JosefPihrt/Roslynator/blob/main/docs/analyzers/RCS1222.md) (MergePreprocessorDirectives)

#### Code Fixes

* Add code fixes for CS0136, CS0210, CS1003, CS1624, and CS1983.

### 1.8.1 (2018-04-17)

#### Analyzers

* Add analyzer [RCS1218](https://github.com/JosefPihrt/Roslynator/blob/main/docs/analyzers/RCS1218.md) (SimplifyCodeBranching)
* Add analyzer [RCS1219](https://github.com/JosefPihrt/Roslynator/blob/main/docs/analyzers/RCS1219.md) (CallSkipAndAnyInsteadOfCount) (split from [RCS1083](https://github.com/JosefPihrt/Roslynator/blob/main/docs/analyzers/RCS1083.md))

#### Refactorings

* Add refactoring [RR0202](https://github.com/JosefPihrt/Roslynator/blob/main/docs/refactorings/RR0202.md) (MoveUnsafeContextToContainingDeclaration)
* Add refactoring [RR0203](https://github.com/JosefPihrt/Roslynator/blob/main/docs/refactorings/RR0203.md) (ExtractEventHandlerMethod)
* Add refactoring [RR0204](https://github.com/JosefPihrt/Roslynator/blob/main/docs/refactorings/RR0204.md) (GeneratePropertyForDebuggerDisplayAttribute)
* Add refactoring [RR0205](https://github.com/JosefPihrt/Roslynator/blob/main/docs/refactorings/RR0205.md) (AddEmptyLineBetweenDeclarations)

#### Code Fixes

* Add code fixes for CS0152, CS0238, CS0524, CS0525, CS0549, CS0567, CS0568, CS0574, CS0575, CS0714, CS1737, CS1743, CS8340.

### 1.8.0 (2018-03-20)

#### Analyzers

##### Changes of "IsEnabledByDefault"

* [RCS1008](https://github.com/JosefPihrt/Roslynator/blob/main/docs/analyzers/RCS1008.md): disabled by default
* [RCS1009](https://github.com/JosefPihrt/Roslynator/blob/main/docs/analyzers/RCS1009.md): disabled by default
* [RCS1010](https://github.com/JosefPihrt/Roslynator/blob/main/docs/analyzers/RCS1010.md): disabled by default
* [RCS1035](https://github.com/JosefPihrt/Roslynator/blob/main/docs/analyzers/RCS1035.md): disabled by default
* [RCS1040](https://github.com/JosefPihrt/Roslynator/blob/main/docs/analyzers/RCS1040.md): enabled by default
* [RCS1073](https://github.com/JosefPihrt/Roslynator/blob/main/docs/analyzers/RCS1073.md): enabled by default

##### Changes of "DefaultSeverity"

* [RCS1017](https://github.com/JosefPihrt/Roslynator/blob/main/docs/analyzers/RCS1017.md): from Warning to Info
* [RCS1026](https://github.com/JosefPihrt/Roslynator/blob/main/docs/analyzers/RCS1026.md): from Warning to Info
* [RCS1027](https://github.com/JosefPihrt/Roslynator/blob/main/docs/analyzers/RCS1027.md): from Warning to Info
* [RCS1028](https://github.com/JosefPihrt/Roslynator/blob/main/docs/analyzers/RCS1028.md): from Warning to Info
* [RCS1030](https://github.com/JosefPihrt/Roslynator/blob/main/docs/analyzers/RCS1030.md): from Warning to Info
* [RCS1044](https://github.com/JosefPihrt/Roslynator/blob/main/docs/analyzers/RCS1044.md): from Info to Warning
* [RCS1045](https://github.com/JosefPihrt/Roslynator/blob/main/docs/analyzers/RCS1045.md): from Warning to Info
* [RCS1055](https://github.com/JosefPihrt/Roslynator/blob/main/docs/analyzers/RCS1055.md): from Info to Hidden
* [RCS1056](https://github.com/JosefPihrt/Roslynator/blob/main/docs/analyzers/RCS1056.md): from Warning to Info
* [RCS1073](https://github.com/JosefPihrt/Roslynator/blob/main/docs/analyzers/RCS1073.md): from Hidden to Info
* [RCS1076](https://github.com/JosefPihrt/Roslynator/blob/main/docs/analyzers/RCS1076.md): from Info to Hidden
* [RCS1081](https://github.com/JosefPihrt/Roslynator/blob/main/docs/analyzers/RCS1081.md): from Warning to Info
* [RCS1086](https://github.com/JosefPihrt/Roslynator/blob/main/docs/analyzers/RCS1086.md): from Warning to Info
* [RCS1087](https://github.com/JosefPihrt/Roslynator/blob/main/docs/analyzers/RCS1087.md): from Warning to Info
* [RCS1088](https://github.com/JosefPihrt/Roslynator/blob/main/docs/analyzers/RCS1088.md): from Warning to Info
* [RCS1094](https://github.com/JosefPihrt/Roslynator/blob/main/docs/analyzers/RCS1094.md): from Warning to Info
* [RCS1110](https://github.com/JosefPihrt/Roslynator/blob/main/docs/analyzers/RCS1110.md): from Warning to Info
* [RCS1182](https://github.com/JosefPihrt/Roslynator/blob/main/docs/analyzers/RCS1182.md): from Info to Hidden

### 1.7.2 (2018-03-06)

#### Analyzers

* Add analyzer [RCS1217](https://github.com/JosefPihrt/Roslynator/blob/main/docs/analyzers/RCS1217.md) (ReplaceInterpolatedStringWithStringConcatenation).

#### Refactorings

* Add refactoring [RR0201](https://github.com/JosefPihrt/Roslynator/blob/main/docs/refactorings/RR0201.md) (ReplaceInterpolatedStringWithStringFormat).

### 1.7.1 (2018-02-14)

#### Analyzers

* Add analyzer [RCS1216](https://github.com/JosefPihrt/Roslynator/blob/main/docs/analyzers/RCS1216.md) (UnnecessaryUnsafeContext).
* Improve analyzer [RCS1181](https://github.com/JosefPihrt/Roslynator/blob/main/docs/analyzers/RCS1181.md) (ReplaceCommentWithDocumentationComment) - support trailing comment.

### 1.7.0 (2018-02-02)

#### Analyzers

* Rename analyzer AddBraces to AddBracesWhenExpressionSpansOverMultipleLines ([RCS1001](https://github.com/JosefPihrt/Roslynator/blob/main/docs/analyzers/RCS1001.md)).
* Rename analyzer AddBracesToIfElse to AddBracesToIfElseWhenExpressionSpansOverMultipleLines ([RCS1003](https://github.com/JosefPihrt/Roslynator/blob/main/docs/analyzers/RCS1003.md)).
* Rename analyzer AvoidEmbeddedStatement to AddBraces ([RCS1007](https://github.com/JosefPihrt/Roslynator/blob/main/docs/analyzers/RCS1007.md)).
* Rename analyzer AvoidEmbeddedStatementInIfElse to AddBracesToIfElse ([RCS1126](https://github.com/JosefPihrt/Roslynator/blob/main/docs/analyzers/RCS1126.md)).

#### Refactorings

* Add refactoring [RR0200](https://github.com/JosefPihrt/Roslynator/blob/main/docs/refactorings/RR0200.md) (UncommentMultilineComment).

### 1.6.30 (2018-01-19)

* Add support for 'private protected' accessibility.

#### Analyzers

* Do not report unused parameter ([RCS1163](https://github.com/JosefPihrt/Roslynator/blob/main/docs/analyzers/RCS1163.md)) when parameter name consists of underscore(s).

#### Refactorings

* Add refactoring [RR0198](https://github.com/JosefPihrt/Roslynator/blob/main/docs/refactorings/RR0198.md) (InlineProperty).
* Add refactoring [RR0199](https://github.com/JosefPihrt/Roslynator/blob/main/docs/refactorings/RR0199.md) (RemoveEnumMemberValue).
* Remove, duplicate or comment out local function.
* Change accessibility for selected members.

#### Code Fixes

* Add code fixes for CS0029, CS0133, CS0201, CS0501, CS0527.

### 1.6.20 (2018-01-03)

#### Analyzers

* Add analyzer [RCS1214](https://github.com/JosefPihrt/Roslynator/blob/main/docs/analyzers/RCS1214.md) (AvoidInterpolatedStringWithNoInterpolatedText).
* Add analyzer [RCS1215](https://github.com/JosefPihrt/Roslynator/blob/main/docs/analyzers/RCS1215.md) (ExpressionIsAlwaysEqualToTrueOrFalse).

#### Refactorings

* Add refactoring [RR0197](https://github.com/JosefPihrt/Roslynator/blob/main/docs/refactorings/RR0197.md) (InitializeFieldFromConstructor).

#### Code Fixes

* Add code fixes for CS1503, CS1751.

### 1.6.10 (2017-12-21)

#### Analyzers

* Add analyzer [RCS1213](https://github.com/JosefPihrt/Roslynator/blob/main/docs/analyzers/RCS1213.md) (UnusedMemberDeclaration).
* Improve analyzer [RCS1163](https://github.com/JosefPihrt/Roslynator/blob/main/docs/analyzers/RCS1163.md) (UnusedParameter)
  * Report unused parameters of lambda expressions and anonymous methods.

#### Code Fixes

* Add code fixes for CS0030, CS1597.

### 1.6.0 (2017-12-13)

#### Refactorings

* Add refactoring [RR0195](https://github.com/JosefPihrt/Roslynator/blob/main/docs/refactorings/RR0195.md) (AddMemberToInterface).
* Add refactoring [RR0196](https://github.com/JosefPihrt/Roslynator/blob/main/docs/refactorings/RR0196.md) (MergeIfWithParentIf).

#### Code Fixes

Add code fix for CS1031 and CS8112.

### 1.5.14 (2017-11-29)

#### Refactorings

* Add refactoring [RR0193](https://github.com/JosefPihrt/Roslynator/blob/main/docs/refactorings/RR0193.md) (ReplaceInterpolatedStringWithConcatenation).
* Add refactoring [RR0194](https://github.com/JosefPihrt/Roslynator/blob/main/docs/refactorings/RR0194.md) (SplitDeclarationAndInitialization).

#### Code Fixes

* Add code fixes for CS0246.

### 1.5.13 (2017-11-09)

#### Analyzers

* Add analyzer [RCS1212](https://github.com/JosefPihrt/Roslynator/blob/main/docs/analyzers/RCS1212.md) (RemoveRedundantAssignment).

#### Refactorings

* Add refactoring [RR0192](https://github.com/JosefPihrt/Roslynator/blob/main/docs/refactorings/RR0192.md) (ReplaceCommentWithDocumentationComment).

#### Code Fixes

* Add code fixes for CS0216, CS0659, CS0660, CS0661 and CS1526.

### 1.5.12 (2017-10-19)

#### Analyzers

* Add analyzer [RCS1210](https://github.com/JosefPihrt/Roslynator/blob/main/docs/analyzers/RCS1210.md) (ReturnTaskInsteadOfNull).
* Add analyzer [RCS1211](https://github.com/JosefPihrt/Roslynator/blob/main/docs/analyzers/RCS1211.md) (RemoveUnnecessaryElseClause).
* Remove analyzer [RCS1022](https://github.com/JosefPihrt/Roslynator/blob/main/docs/analyzers/RCS1022.md) (SimplifyLambdaExpressionParameterList).

#### Refactorings

* Replace refactoring [RR0019](https://github.com/JosefPihrt/Roslynator/blob/main/docs/refactorings/RR0019.md) (ChangeMemberTypeAccordingToReturnExpression) with code fix.
* Replace refactoring [RR0020](https://github.com/JosefPihrt/Roslynator/blob/main/docs/refactorings/RR0020.md) (ChangeMemberTypeAccordingToYieldReturnExpression) with code fix.
* Replace refactoring [RR0008](https://github.com/JosefPihrt/Roslynator/blob/main/docs/refactorings/RR0008.md) (AddDefaultValueToReturnStatement) with code fix.

#### Code Fixes

  * Add code fix for CS0126, CS0139, CS0713 and CS1750.

### 1.5.10 (2017-10-04)

#### Code Fixes

  * Add code fixes for CS0103, CS0192, CS0403 and CS0541.

### 1.5.0 (2017-09-22)

 * Bug fixes.

### 1.4.58 (2017-09-16)

#### Analyzers

  * Remove analyzer [RCS1095](https://github.com/JosefPihrt/Roslynator/blob/main/docs/analyzers/RCS1095.md) (UseCSharp6DictionaryInitializer)

#### Refactorings

##### New Refactorings

  * [RR0191](https://github.com/JosefPihrt/Roslynator/blob/main/docs/refactorings/RR0191.md) (UseCSharp6DictionaryInitializer)

### 1.4.57 (2017-09-06)

#### Refactorings

##### New Refactorings

  * [RR0190](https://github.com/JosefPihrt/Roslynator/blob/main/docs/refactorings/RR0190.md) (ReplaceIfElseWithIfReturn)

#### Code Fixes

  * Add code fix for CS0021.

### 1.4.56 (2017-08-28)

#### Analyzers

##### New Analyzers

  * [RCS1209](https://github.com/JosefPihrt/Roslynator/blob/main/docs/analyzers/RCS1209.md) (ReorderTypeParameterConstraints)

### 1.4.55 (2017-08-16)

#### Code Fixes

  * Add code fixes for CS0077, CS0201, CS0472, CS1623.

#### Analyzers

##### New Analyzers

  * [RCS1208](https://github.com/JosefPihrt/Roslynator/blob/main/docs/analyzers/RCS1208.md) (ReduceIfNesting)

#### Refactorings

##### New Refactorings

  * [RR0189](https://github.com/JosefPihrt/Roslynator/blob/main/docs/refactorings/RR0189.md) (ReduceIfNesting)

### 1.4.54 (2017-08-08)

#### Code Fixes

  * Improve code fixes for CS0162, CS1061.

#### Analyzers

* Add code fix for analyzer [RCS1168](https://github.com/JosefPihrt/Roslynator/blob/main/docs/analyzers/RCS1168.md) (ParameterNameDiffersFromBase)

##### New Analyzers

* [RCS1203](https://github.com/JosefPihrt/Roslynator/blob/main/docs/analyzers/RCS1203.md) (UseAttributeUsageAttribute)
* [RCS1204](https://github.com/JosefPihrt/Roslynator/blob/main/docs/analyzers/RCS1204.md) (UseEventArgsEmpty)
* [RCS1205](https://github.com/JosefPihrt/Roslynator/blob/main/docs/analyzers/RCS1205.md) (ReorderNamedArguments)
* [RCS1206](https://github.com/JosefPihrt/Roslynator/blob/main/docs/analyzers/RCS1206.md) (UseConditionalAccessInsteadOfConditionalExpression)
* [RCS1207](https://github.com/JosefPihrt/Roslynator/blob/main/docs/analyzers/RCS1207.md) (UseMethodGroupInsteadOfAnonymousFunction)

### 1.4.53 (2017-08-02)

#### Code Fixes

  * New code fixes for CS0139, CS0266, CS0592, CS1689.

#### Analyzers

##### New Analyzers

* [RCS1199](https://github.com/JosefPihrt/Roslynator/blob/main/docs/analyzers/RCS1199.md) (SimplifyBooleanExpression)
* [RCS1200](https://github.com/JosefPihrt/Roslynator/blob/main/docs/analyzers/RCS1200.md) (CallThenByInsteadOfOrderBy)
* [RCS1201](https://github.com/JosefPihrt/Roslynator/blob/main/docs/analyzers/RCS1201.md) (UseMethodChaining)
* [RCS1202](https://github.com/JosefPihrt/Roslynator/blob/main/docs/analyzers/RCS1202.md) (UseConditionalAccessToAvoidNullReferenceException)

### 1.4.52 (2017-07-24)

#### Code Fixes

  * New code fixes for CS0115, CS1106, CS1621, CS1988.

### 1.4.51 (2017-07-19)

#### Refactorings

  * [RR0073](https://github.com/JosefPihrt/Roslynator/blob/main/docs/refactorings/RR0073.md) (MarkContainingClassAsAbstract) has been replaced with code fix.

##### New Refactorings

  * [RR0187](https://github.com/JosefPihrt/Roslynator/blob/main/docs/refactorings/RR0187.md) (FormatWhereConstraint)
  * [RR0188](https://github.com/JosefPihrt/Roslynator/blob/main/docs/refactorings/RR0188.md) (ReplaceForEachWithForAndReverseLoop)

#### Code Fixes

##### New Code Fixes

Code fixes has been added for the following compiler diagnostics:

  * NamespaceAlreadyContainsDefinition (CS0101)
  * TypeAlreadyContainsDefinition (CS0102)
  * TypeOfConditionalExpressionCannotBeDetermined (CS0173)
  * OutParameterMustBeAssignedToBeforeControlLeavesCurrentMethod (CS0177)
  * NewConstraintMustBeLastConstraintSpecified (CS0401)
  * DuplicateConstraintForTypeParameter (CS0405)
  * ConstraintClauseHasAlreadyBeenSpecified (CS0409)
  * ClassOrStructConstraintMustComeBeforeAnyOtherConstraints (CS0449)
  * CannotSpecifyBothConstraintClassAndClassOrStructConstraint (CS0450)
  * NewConstraintCannotBeUsedWithStructConstraint (CS0451)
  * TypeParameterHasSameNameAsTypeParameterFromOuterType (CS0693)
  * StaticTypesCannotBeUsedAsTypeArguments (CS0718)
  * PartialMethodCannotHaveAccessModifiersOrVirtualAbstractOverrideNewSealedOrExternModifiers (CS0750)
  * NoDefiningDeclarationFoundForImplementingDeclarationOfPartialMethod (CS0759)
  * PartialMethodsMustHaveVoidReturnType (CS0766)
  * MethodHasParameterModifierThisWhichIsNotOnFirstParameter (CS1100)
  * ExtensionMethodMustBeStatic (CS1105)
  * ElementsDefinedInNamespaceCannotBeExplicitlyDeclaredAsPrivateProtectedOrProtectedInternal (CS1527)
  * AsyncModifierCanOnlyBeUsedInMethodsThatHaveBody (CS1994)

### 1.4.50 (2017-07-04)

* Add code fixes that fix 80+ compiler diagnostics (like 'CS0001')

#### Analyzers

* Following analyzers have been replaced with code fixes:

  * [RCS1115](https://github.com/JosefPihrt/Roslynator/blob/main/docs/analyzers/RCS1115.md) (ReplaceReturnStatementWithExpressionStatement)
  * [RCS1116](https://github.com/JosefPihrt/Roslynator/blob/main/docs/analyzers/RCS1116.md) (AddBreakStatementToSwitchSection)
  * [RCS1117](https://github.com/JosefPihrt/Roslynator/blob/main/docs/analyzers/RCS1117.md) (AddReturnStatementThatReturnsDefaultValue)
  * [RCS1122](https://github.com/JosefPihrt/Roslynator/blob/main/docs/analyzers/RCS1122.md) (AddMissingSemicolon)
  * [RCS1125](https://github.com/JosefPihrt/Roslynator/blob/main/docs/analyzers/RCS1125.md) (MarkMemberAsStatic)
  * [RCS1131](https://github.com/JosefPihrt/Roslynator/blob/main/docs/analyzers/RCS1131.md) (ReplaceReturnWithYieldReturn)
  * [RCS1137](https://github.com/JosefPihrt/Roslynator/blob/main/docs/analyzers/RCS1137.md) (AddDocumentationComment)
  * [RCS1144](https://github.com/JosefPihrt/Roslynator/blob/main/docs/analyzers/RCS1144.md) (MarkContainingClassAsAbstract)
  * [RCS1147](https://github.com/JosefPihrt/Roslynator/blob/main/docs/analyzers/RCS1147.md) (RemoveInapplicableModifier)
  * [RCS1148](https://github.com/JosefPihrt/Roslynator/blob/main/docs/analyzers/RCS1148.md) (RemoveUnreachableCode)
  * [RCS1149](https://github.com/JosefPihrt/Roslynator/blob/main/docs/analyzers/RCS1149.md) (RemoveImplementationFromAbstractMember)
  * [RCS1152](https://github.com/JosefPihrt/Roslynator/blob/main/docs/analyzers/RCS1152.md) (MemberTypeMustMatchOverriddenMemberType)
  * [RCS1176](https://github.com/JosefPihrt/Roslynator/blob/main/docs/analyzers/RCS1176.md) (OverridingMemberCannotChangeAccessModifiers)

#### Refactorings

* Following refactorings have been replaced with code fixes:

  * [RR0001](https://github.com/JosefPihrt/Roslynator/blob/main/docs/refactorings/RR0001.md) (AddBooleanComparison)
  * [RR0042](https://github.com/JosefPihrt/Roslynator/blob/main/docs/refactorings/RR0042.md) (ExtractDeclarationFromUsingStatement)
  * [RR0072](https://github.com/JosefPihrt/Roslynator/blob/main/docs/refactorings/RR0072.md) (MarkMemberAsStatic)
  * [RR0122](https://github.com/JosefPihrt/Roslynator/blob/main/docs/refactorings/RR0122.md) (ReplaceCountWithLengthOrLengthWitCount)
  * [RR0146](https://github.com/JosefPihrt/Roslynator/blob/main/docs/refactorings/RR0146.md) (ReplaceStringLiteralWithCharacterLiteral)

##### New Refactorings

  * [RR0186](https://github.com/JosefPihrt/Roslynator/blob/main/docs/refactorings/RR0186.md) (ChangeAccessibility)

### 1.4.13 (2017-06-21)

#### Analyzers

##### New Analyzers

* [RCS1197](https://github.com/JosefPihrt/Roslynator/blob/main/docs/analyzers/RCS1197.md) (OptimizeStringBuilderAppendCall)
* [RCS1198](https://github.com/JosefPihrt/Roslynator/blob/main/docs/analyzers/RCS1198.md) (AvoidBoxingOfValueType)

### 1.4.12 (2017-06-11)

#### Analyzers

##### New Analyzers

* [RCS1192](https://github.com/JosefPihrt/Roslynator/blob/main/docs/analyzers/RCS1192.md) (UseRegularStringLiteralInsteadOfVerbatimStringLiteral)
* [RCS1193](https://github.com/JosefPihrt/Roslynator/blob/main/docs/analyzers/RCS1193.md) (OverridingMemberCannotChangeParamsModifier)
* [RCS1194](https://github.com/JosefPihrt/Roslynator/blob/main/docs/analyzers/RCS1194.md) (ImplementExceptionConstructors)
* [RCS1195](https://github.com/JosefPihrt/Roslynator/blob/main/docs/analyzers/RCS1195.md) (UseExclusiveOrOperator)
* [RCS1196](https://github.com/JosefPihrt/Roslynator/blob/main/docs/analyzers/RCS1196.md) (CallExtensionMethodAsInstanceMethod)

#### Refactorings

##### New Refactorings

* [RR0183](https://github.com/JosefPihrt/Roslynator/blob/main/docs/refactorings/RR0183.md) (UseListInsteadOfYield)
* [RR0184](https://github.com/JosefPihrt/Roslynator/blob/main/docs/refactorings/RR0184.md) (SplitIfStatement)
* [RR0185](https://github.com/JosefPihrt/Roslynator/blob/main/docs/refactorings/RR0185.md) (ReplaceObjectCreationWithDefaultValue)

### 1.4.1 (2017-06-05)

#### Analyzers

##### New Analyzers

* [RCS1191](https://github.com/JosefPihrt/Roslynator/blob/main/docs/analyzers/RCS1191.md) (DeclareEnumValueAsCombinationOfNames)
* [RCS1190](https://github.com/JosefPihrt/Roslynator/blob/main/docs/analyzers/RCS1190.md) (MergeStringExpressions)
* [RCS1189](https://github.com/JosefPihrt/Roslynator/blob/main/docs/analyzers/RCS1189.md) (AddOrRemoveRegionName)
* [RCS1188](https://github.com/JosefPihrt/Roslynator/blob/main/docs/analyzers/RCS1188.md) (RemoveRedundantAutoPropertyInitialization)
* [RCS1187](https://github.com/JosefPihrt/Roslynator/blob/main/docs/analyzers/RCS1187.md) (MarkFieldAsConst)
* [RCS1186](https://github.com/JosefPihrt/Roslynator/blob/main/docs/analyzers/RCS1186.md) (UseRegexInstanceInsteadOfStaticMethod)

#### Refactorings

##### New Refactorings

* [RR0182](https://github.com/JosefPihrt/Roslynator/blob/main/docs/refactorings/RR0182.md) (UseStringBuilderInsteadOfConcatenation)
* [RR0181](https://github.com/JosefPihrt/Roslynator/blob/main/docs/refactorings/RR0181.md) (InlineConstant)

### 1.4.0 (2017-05-29)

#### Analyzers

* Delete analyzer [RCS1054](https://github.com/JosefPihrt/Roslynator/blob/main/docs/analyzers/RCS1054.md) (MergeLocalDeclarationWithReturnStatement) - Its functionality is incorporated into analyzer [RCS1124](https://github.com/JosefPihrt/Roslynator/blob/main/docs/analyzers/RCS1124.md) (InlineLocalVariable).
* Disable analyzer [RCS1024](https://github.com/JosefPihrt/Roslynator/blob/main/docs/analyzers/RCS1024.md) (FormatAccessorList) by default.
* Disable analyzer [RCS1023](https://github.com/JosefPihrt/Roslynator/blob/main/docs/analyzers/RCS1023.md) (FormatEmptyBlock) by default.
* Modify analyzer [RCS1091](https://github.com/JosefPihrt/Roslynator/blob/main/docs/analyzers/RCS1091.md) (RemoveEmptyRegion) - Change default severity from Info to Hidden.
* Modify analyzer [RCS1157](https://github.com/JosefPihrt/Roslynator/blob/main/docs/analyzers/RCS1157.md) (CompositeEnumValueContainsUndefinedFlag) - Change default severity from Warning to Info.
* Modify analyzer [RCS1032](https://github.com/JosefPihrt/Roslynator/blob/main/docs/analyzers/RCS1032.md) (RemoveRedundantParentheses) - Exclude following syntaxes from analyzer:
  * AssignmentExpression.Right
  * ForEachExpression.Expression
  * EqualsValueClause.Value

#### Refactorings

* Modify refactoring [RR0024](https://github.com/JosefPihrt/Roslynator/blob/main/docs/refactorings/RR0024.md) (CheckExpressionForNull) - Do not add empty line.

### 1.3.11 (2017-05-18)

* A lot of bug fixes and improvements.

### 1.3.10 (2017-04-24)

#### Analyzers

* Improve analyzer [RCS1147](https://github.com/JosefPihrt/Roslynator/blob/main/docs/analyzers/RCS1147.md) (RemoveInapplicableModifier) - Analyze local function.
* Improve analyzer [RCS1077](https://github.com/JosefPihrt/Roslynator/blob/main/docs/analyzers/RCS1077.md) (SimplifyMethodChain) - Merge combination of Where and Any.
* Improve analyzer [RCS1158](https://github.com/JosefPihrt/Roslynator/blob/main/docs/analyzers/RCS1158.md) (StaticMemberInGenericTypeShouldUseTypeParameter) - Member must be public, internal or protected internal.

##### New Analyzers

* [RCS1178](https://github.com/JosefPihrt/Roslynator/blob/main/docs/analyzers/RCS1178.md) (CallDebugFailInsteadOfDebugAssert)
* [RCS1179](https://github.com/JosefPihrt/Roslynator/blob/main/docs/analyzers/RCS1179.md) (UseReturnInsteadOfAssignment)
* [RCS1180](https://github.com/JosefPihrt/Roslynator/blob/main/docs/analyzers/RCS1180.md) (InlineLazyInitialization)
* [RCS1181](https://github.com/JosefPihrt/Roslynator/blob/main/docs/analyzers/RCS1181.md) (ReplaceCommentWithDocumentationComment)
* [RCS1182](https://github.com/JosefPihrt/Roslynator/blob/main/docs/analyzers/RCS1182.md) (RemoveRedundantBaseInterface)
* [RCS1183](https://github.com/JosefPihrt/Roslynator/blob/main/docs/analyzers/RCS1183.md) (FormatInitializerWithSingleExpressionOnSingleLine)
* [RCS1184](https://github.com/JosefPihrt/Roslynator/blob/main/docs/analyzers/RCS1184.md) (FormatConditionalExpression)
* [RCS1185](https://github.com/JosefPihrt/Roslynator/blob/main/docs/analyzers/RCS1185.md) (AvoidSingleLineBlock)

### 1.3.0 (2017-04-02)

* Add support for configuration file.

#### Analyzers

* Disable [RCS1176](https://github.com/JosefPihrt/Roslynator/blob/main/docs/analyzers/RCS1176.md) (UseVarInsteadOfExplicitTypeWhenTypeIsNotObvious) by default.
* Disable [RCS1177](https://github.com/JosefPihrt/Roslynator/blob/main/docs/analyzers/RCS1177.md) (UseVarInsteadOfExplicitTypeInForEach) by default.

### 1.2.53 (2017-03-27)

* Filter list of refactorings in options.
* Bug fixes.

#### Analyzers

* Change default severity of [RCS1140](https://github.com/JosefPihrt/Roslynator/blob/main/docs/analyzers/RCS1140.md) (AddExceptionToDocumentationComment) from Warning to Hidden.
* Change default severity of [RCS1161](https://github.com/JosefPihrt/Roslynator/blob/main/docs/analyzers/RCS1161.md) (EnumMemberShouldDeclareExplicitValue) from Warning to Hidden.

### 1.2.52 (2017-03-22)

#### Analyzers

##### New Analyzers

* [RCS1176](https://github.com/JosefPihrt/Roslynator/blob/main/docs/analyzers/RCS1176.md) (UseVarInsteadOfExplicitTypeWhenTypeIsNotObvious)
* [RCS1177](https://github.com/JosefPihrt/Roslynator/blob/main/docs/analyzers/RCS1177.md) (UseVarInsteadOfExplicitTypeInForEach)

#### Refactorings

##### New Refactorings

* [RR0180](https://github.com/JosefPihrt/Roslynator/blob/main/docs/refactorings/RR0180.md) (InlineUsingStatic)

### 1.2.51 (2017-03-14)

* Bug fixes.

### 1.2.50 (2017-03-13)

* Improved options page with a list of refactorings.
  * Refactorings are displayed in WPF control instead of property grid.
  * Each refactoring has an identifier 'RR....' to avoid confusion with analyzers.

#### Analyzers

##### New Analyzers

* UnusedThisParameter

#### Refactorings

##### New Refactorings

* ImplementIEquatableOfT
* AddTypeParameter

### 1.2.16 (2017-03-02)

#### Analyzers

##### New Analyzers

* SimplifyLazilyInitializedProperty
* UseIsOperatorInsteadOfAsOperator
* UseCoalesceOperatorInsteadOfIf
* RemoveRedundantAsyncAwait

#### Refactorings

##### New Refactorings

* ReplaceHexadecimalLiteralWithDecimalLiteral
* WrapInElseClause

### 1.2.15 (2017-02-23)

#### Analyzers

##### Changes

* Analyzer RemoveRedundantBraces was deleted.

##### New Analyzers

* MarkFieldAsReadOnly
* UseReadOnlyAutoProperty

### 1.2.14 (2017-02-19)

#### Analyzers

##### New Analyzers

* ParameterNameDiffersFromBase
* OverridingMemberCannotChangeAccessModifiers
* ValueTypeCheckedForNull
* UnconstrainedTypeParameterCheckedForNull
* UnusedTypeParameter
* UnusedParameter

### 1.2.13 (2017-02-11)

#### Analyzers

##### New Analyzers

* SortEnumMembers
* UseStringComparison
* UseStringLengthInsteadOfComparisonWithEmptyString
* CompositeEnumValueContainsUndefinedFlag
* AvoidStaticMembersInGenericTypes
* UseGenericEventHandler
* AbstractTypeShouldNotHavePublicConstructors
* EnumMemberShouldDeclareExplicitValue
* AvoidChainOfAssignments

#### Refactorings

##### New Refactorings

* ReplaceExpressionWithConstantValue

### 1.2.12 (2017-02-02)

#### Analyzers

##### New Analyzers

* SimplifyCoalesceExpression
* MarkContainingClassAsAbstract
* RemoveRedundantAsOperator
* UseConditionalAccess
* RemoveInapplicableModifier
* RemoveUnreachableCode
* RemoveImplementationFromAbstractMember
* CallStringConcatInsteadOfStringJoin
* RemoveRedundantCast
* MemberTypeMustMatchOverriddenMemberType
* AddEmptyLineAfterClosingBrace

#### Refactorings

##### New Refactorings

* SortMemberDeclarations
* ReplaceWhileWithFor
* GenerateEnumValues
* GenerateEnumMember
* GenerateCombinedEnumMember

### 1.2.11 (2017-01-27)

#### Analyzers

##### New Analyzers

* BitwiseOperatorOnEnumWithoutFlagsAttribute
* ReplaceReturnWithYieldReturn
* RemoveRedundantOverriddenMember
* RemoveRedundantDisposeOrCloseCall
* RemoveRedundantContinueStatement
* DeclareEnumMemberWithZeroValue
* MergeSwitchSectionsWithEquivalentContent
* AddDocumentationComment
* AddSummaryToDocumentationComment
* AddSummaryElementToDocumentationComment
* AddExceptionToDocumentationComment
* AddParameterToDocumentationComment
* AddTypeParameterToDocumentationComment

### 1.2.10 (2017-01-22)

#### Analyzers

##### New Analyzers

* ReplaceReturnStatementWithExpressionStatement
* AddBreakStatementToSwitchSection
* AddReturnStatementThatReturnsDefaultValue
* MarkLocalVariableAsConst
* CallFindMethodInsteadOfFirstOrDefaultMethod
* UseElementAccessInsteadOfElementAt
* UseElementAccessInsteadOfFirst
* AddMissingSemicolon
* AddParenthesesAccordingToOperatorPrecedence
* InlineLocalVariable
* MarkMemberAsStatic
* AvoidEmbeddedStatementInIfElse
* MergeLocalDeclarationWithInitialization
* UseCoalesceExpression
* RemoveRedundantFieldInitialization

### 1.2.0 (2017-01-18)

* Release of package Roslynator.Analyzers 1.2.0
* Release of package CSharpAnalyzers 1.2.0

### 1.1.95 (2017-01-04)

* Initial release of Roslynator 2017 and Roslynator Refactorings 2017

### 1.1.90 (2016-12-16)

#### Refactorings

##### New Refactorings

* MergeStringExpressions
* ReplaceForWithWhile
* MarkContainingClassAsAbstract
* MakeMemberVirtual

### 1.1.8 (2016-12-07)

#### Refactorings

##### New Refactorings

* ReplaceStatementWithIfStatement
* NegateIsExpression
* ReplaceCastWithAs
* SplitSwitchLabels
* CheckExpressionForNull

### 1.1.7 (2016-11-29)

#### Refactorings

##### New Refactorings

* CallExtensionMethodAsInstanceMethod
* ReplaceMethodGroupWithLambda
* ReplaceIfStatementWithReturnStatement
* IntroduceLocalFromExpressionStatementThatReturnsValue

### 1.1.6 (2016-11-24)

#### Analyzers

##### New Analyzers

* CombineEnumerableWhereMethodChain
* UseStringIsNullOrEmptyMethod
* RemoveRedundantDelegateCreation

#### Refactorings

##### New Refactorings

* AddExceptionToDocumentationComment
* ReplaceNullLiteralExpressionWithDefaultExpression

### 1.1.5 (2016-11-19)

#### Analyzers

##### New Analyzers

* RemoveEmptyDestructor
* RemoveRedundantStringToCharArrayCall
* AddStaticModifierToAllPartialClassDeclarations
* UseCastMethodInsteadOfSelectMethod
* DeclareTypeInsideNamespace
* AddBracesToSwitchSectionWithMultipleStatements

#### Refactorings

##### New Refactorings

* ReplaceEqualsExpressionWithStringIsNullOrEmpty
* ReplaceEqualsExpressionWithStringIsNullOrWhiteSpace

### 1.1.4 (2016-11-15)

#### Analyzers

##### New Analyzers

* FormatDocumentationSummaryOnSingleLine
* FormatDocumentationSummaryOnMultipleLines
* MarkClassAsStatic
* SimplifyIfElseStatement
* SimplifyConditionalExpression
* MergeInterpolationIntoInterpolatedString

#### Refactorings

##### New Refactorings

* MergeInterpolationIntoInterpolatedString

### 1.1.3 (2016-11-12)

#### Analyzers

##### New Analyzers

* RemoveRedundantToStringCall
* AvoidNullLiteralExpressionOnLeftSideOfBinaryExpression
* DefaultLabelShouldBeLastLabelInSwitchSection

#### Refactorings

##### New Refactorings

* IntroduceFieldToLockOn

### 1.1.2 (2016-11-10)

#### Analyzers

##### New Analyzers

* UseCSharp6DictionaryInitializer
* UseBitwiseOperationInsteadOfHasFlagMethod

#### Refactorings

##### New Refactorings

* CopyDocumentationCommentFromBaseMember

### 1.1.1 (2016-11-06)

#### Analyzers

##### New Analyzers

* RemoveFileWithNoCode
* DeclareUsingDirectiveOnTopLevel

#### Refactorings

##### New Refactorings

* RemoveRegion

### 1.1.0 (2016-11-04)

#### Refactorings

##### New Refactorings

* ReplaceAsWithCast
* ReplaceEqualsExpressionWithStringEquals

### 1.0.9 (2016-11-02)

#### Refactorings

##### New Refactorings

* RemoveUsingAliasDirective
* ReplaceInterpolatedStringWithInterpolationExpression

### 1.0.8 (2016-10-31)

#### Analyzers

##### New Analyzers

* AddEmptyLineAfterLastStatementInDoStatement

#### Refactorings

##### New Refactorings

* ReplaceIfElseWithSwitch

### 1.0.7 (2016-10-29)

#### Refactorings

##### New Refactorings

* RemoveAllPreprocessorDirectives
* AddToMethodInvocation

### 1.0.6 (2016-10-26)

#### Analyzers

##### New Analyzers

* RemoveEmptyRegion

#### Refactorings

##### New Refactorings

* GenerateOnEventMethod

### 1.0.5 (2016-10-24)

#### Refactorings

##### Improvements

* InlineMethod - void method with multiple statements can be inlined.
* CheckParameterForNull - refactoring can be applied to multiple parameters at once.
* AddBraces - braces can be added to if statement in last else-if.

##### New Refactorings

* GenerateBaseConstructors

### 1.0.4 (2016-10-20)

#### Refactorings

##### New Refactorings

* PromoteLocalToParameter
* RemoveInterpolation

### 1.0.3 (2016-10-15)

#### Analyzers

##### New Analyzers

* UsePostfixUnaryOperatorInsteadOfAssignment
* AddConfigureAwait

### 1.0.2 (2016-10-12)

#### Analyzers

##### New Analyzers

* UseLinefeedAsNewline
* UseCarriageReturnAndLinefeedAsNewline
* AvoidUsageOfTab

### 1.0.1 (2016-10-08)

#### Refactorings

##### Changes

* ReplaceMethodWithProperty and ReplacePropertyWithMethod refactorings significantly improved.

##### New Refactorings

* ExtractTypeDeclarationToNewFile
* MergeLocalDeclarations

### 1.0.0 (2016-10-03)

* Entire project was renamed to **Roslynator**
* Visual Studio extension **C# Analyzers and Refactorings** was renamed to **Roslynator**
* Visual Studio extension **C# Refactorings** was renamed to **Roslynator Refactorings**
* Some assemblies were renamed. As a result **ruleset** files must be updated in a following way:
  * replace &lt;Rules AnalyzerId="Pihrtsoft.CodeAnalysis.CSharp" RuleNamespace="Pihrtsoft.CodeAnalysis.CSharp">
  * with &lt;Rules AnalyzerId="Roslynator.CSharp.Analyzers" RuleNamespace="Roslynator.CSharp.Analyzers">

### 0.99.5 (2016-09-12)

#### Analyzers

##### Changes

* "DeclareEachTypeInSeparateFile" has code fix.

##### Bug Fixes

* "ReplacePropertyWithAutoProperty" - property and field must be of equal type.

#### Refactorings

##### Bug Fixes

* "InsertInterpolation" - '{' and '}' are escaped by doubling when creating interpolated string from string literal.

### 0.99.0 (2016-08-28)

#### Analyzers

##### Changes

* "UseExplicitTypeInsteadOfVar" and "UseVarInsteadOfExplicitType" allow 'var' for enum member expression.
* "AddDefaultAccessModifier" works with partial classes.
* "AvoidUsageOfUsingAliasDirective" has code fix.

#### Refactorings

##### New Refactorings

* ReplaceIfElseWithConditionalExpression
* ReplaceConditionalExpressionWithExpression

### 0.98.0 (2016-08-14)

#### Analyzers

##### Changes

* "RemoveRedundantEmptyLine" analyzer - empty line is allowed when it is last line in 'do' statement's body (when 'while' token is on the same line as closing brace)
* "UseExplicitTypeInsteadOfVar" and "UseVarInsteadOfExplicitType" analyzers - 'var' is allowed for 'default(T)' expression

#### Refactorings

##### New Refactorings

* MergeAssignmentExpressionWithReturnStatement
* CollapseToInitializer
* IntroduceAndInitializeField
* IntroduceAndInitializeProperty

### 0.97.0 (2016-08-08)

#### Refactorings

##### New Refactorings

* AddRegion
* AddIfDirective
* RemoveAllStatements
* RemoveAllMembers
* AddUsingDirective

### 0.96.0 (2016-08-05)

#### Refactorings

##### New Refactorings

* MergeIfStatements
* AddDefaultValueToReturnStatement
* InlineMethod

### 0.95.0 (2016-07-30)

#### Refactorings

##### New Refactorings

* AddExpressionFromIfStatement
* RemoveAllSwitchSections
* RemoveStatementsFromSwitchSections
* AddConfigureAwait
* RemovePreprocessorDirectiveAndRelatedDirectives

### 0.94.0 (2016-07-26)

#### Refactorings

##### New Refactorings

* ReplaceReturnStatementWithIfStatement
* WrapStatementsInTryCatch
* WrapStatementsInIfStatement
* RemoveMemberDeclarations

### 0.93.0 (2016-07-21)

#### Refactorings

##### New Refactorings

* AddIdentifierToVariableDeclaration
* RemoveEmptyLines

### 0.92.0 (2016-07-18)

#### Refactorings

##### New Refactorings

* CommentOutMember
* CommentOutStatement
* InitializerLocalWithDefaultValue
* AddDefaultValueToParameter

##### Improvements

* refactoring "ChangeTypeAccordingToExpression" works for field declaration
* refactoring "AddCastExpression" works for case label expression
* refactoring "FormatExpressionChain" does not format namespace
* refactoring "ReplacePropertyWithMethod" works for property with setter
* refactoring "ReverseForLoop" works for reversed for loop

### 0.91.0 (2016-07-11)

#### Refactorings

##### New Refactorings

* RemoveConditionFromLastElseIf
* RemoveAllXmlComments
* RemoveStatement
* DuplicateStatement
* ReplaceAnonymousMethodWithLambdaExpression
* SplitVariableDeclaration
* ReplaceCountWithLengthOrLengthWithCount

##### Changes

* ChangeMethodReturnTypeToVoid
  * refactoring is available only when method body contains at least one statement
  * refactoring is not available for async method that returns Task
* IntroduceUsingStaticDirective
  * refactoring is available only when class name is selected

### 0.9.90 (2016-07-08)

#### Refactorings

##### New Refactorings

* ReplaceDoStatementWithWhileStatement
* ReplaceWhileStatementWithDoStatement
* IntroduceUsingStaticDirective
* ChangeMethodReturnTypeToVoid
* ReplaceEnumHasFlagWithBitwiseOperation

### 0.9.81 (2016-07-06)

#### Refactorings

##### Changes

* refactoring "FormatBinaryExpression" is available for bitwise and/or expressions.
* refactorings for argument and argument list are also available for attribute argument and attribute argument list.

##### Bug Fixes

* refactorings "RemoveComment" and "RemoveAllComments" are available at comment inside trivia.
* refactoring "AddCastExpressionToArgument" handles properly params parameter.
* refactoring "ExpandPropertyAndAddBackingField" handles properly read-only auto-property.

### 0.9.80 (2016-07-05)

#### Analyzers

##### Changes

* many analyzers renamed
* **developmentDependency** element added to CSharpAnalyzers.nuspec

#### Refactorings

##### New Refactorings

* AddInterpolation
* SimplifyLambdaExpression

##### Changes

* refactorings can be enabled/disabled in Visual Studio UI (Tools - Options)
* some refactorings are available only when C# 6.0 is available.
* many refactorings renamed
* refactoring "ChangeMemberTypeAccordingToReturnExpression" improved for async method
* refactoring "AddCastToReturnExpression" improved for async method
* refactoring "CheckParameterForNull" is not available for lambda and anonymous method

##### Bug Fixes

* refactoring "MarkMemberAsStatic" should not be available for a constant.

### 0.9.70 (2016-06-23)

#### Analyzers

##### Changes

* analyzer "MergeIfStatementWithContainedIfStatement" renamed to "MergeIfStatementWithNestedIfStatement"

#### Refactorings

##### New Refactorings

* MarkMemberAsStatic
* MarkAllMembersAsStatic
* FormatAccessorBracesOnSingleLine
* GenerateSwitchSections
* ConvertStringLiteralToCharacterLiteral

##### Changes

* refactoring "ReverseForLoop" is available within 'for' keyword.
* refactoring "SwapExpressionsInBinaryExpression" is available only for logical and/or expression.
* refactoring "AddCastAccordingToParameterType" can offer more than one cast.
* refactorings "SwapParameters" and "SwapArguments" removed (these are covered by "Change signature..." dialog)
* refactorings "RemoveMember" and "DuplicateMember" are available only at opening/closing brace

##### Bug Fixes

* refactoring "RemoveAllRegions" is available inside #endregion directive.
* refactoring "RenameMethodAccordingToTypeName" handles properly async method.

### 0.9.60 (2016-06-14)

#### Analyzers

##### Changes

* UseNameOfOperator analyzer:
  * only quote marks (and at sign) are faded out.
  * analyzer detects property name in property setter.
* SimplifyLambdaExpressionParameterList analyzer - parenthesized lambda with parameter list with a single parameter without type can be simplified to simple lambda

##### Bug Fixes

* UseExpressionBodiedMember analyzer

#### Refactorings

##### New Refactorings

* Duplicate argument
* Add cast to return statement's expression
* Add cast to variable declaration
* Merge string literals
* Merge string literals into multiline string literal
* Convert regular string literal to verbatim string literal
* Convert verbatim string literal to regular string literal
* Convert verbatim string literal to regular string literals
* Use expression-bodied member

##### Changes

* "Extract expression from parentheses" refactoring is available when cursor is on opening/closing parenthesis.

##### Bug Fixes

* "Check parameter for null" refactoring is available for lambda expression and anonymous method.
* "Remove comment" and "Remove all comments" refactorings is available when cursor is inside xml documentation comment.
* "Convert foreach to for" refactoring is available for string expression.

### 0.9.50 (2016-06-02)

#### Analyzers

##### New Analyzers

* SplitDeclarationIntoMultipleDeclarations
* UseCountOrLengthPropertyInsteadOfCountMethod
* UseAnyMethodInsteadOfCountMethod
* UseCoalesceExpressionInsteadOfConditionalExpression
* UseAutoImplementedProperty

##### Changes

* DeclareExplicitType and DeclareImplicitType analyzers - 'var' is allowed for ThisExpression.

##### Bug Fixes

* "RemoveRedundantEmptyLine" analyzer - empty line can be between using directive (or extern alias) inside namespace declaration and first member declaration.

#### Refactorings

##### New Refactorings

* Expand coalesce expression
* Expand event
* Swap members
* Split attributes
* Merge attributes
* Change method/property/indexer type according to yield return statement
* Notify property changed
* Add cast to assignment expression
* Format accessor braces on multiple lines

##### Changes

* "Remove/duplicate member" refactoring:
  * triggers inside header or on closing brace (if any)
  * is available for method/constructor/property/indexer/operator/event/namespace/class/struct/interface.
* "Add/remove parameter name" refactoring - argument(s) must be selected.
* "Rename variable/method/property/parameter according to type name" refactorings - predefined types are excluded.
* "Convert method to read-only property" refactoring - triggers only inside method header.
* "Convert property to method" refactoring - triggers only inside property header
* "Make method/property/indexer method" refactoring - triggers only inside method/property/indexer header

##### Bug Fixes

* "Convert constant to read-only field" refactoring - static keyword is added if the constant is declared in static class.
* "Convert switch to if-else chain" refactoring - there must be at least one non-default section.
* "Rename parameter according to type name" refactoring - now works for lambda's argument list.
* "Add parentheses" refactoring

### 0.9.40 (2016-05-24)

#### Analyzers

* **NEW** - **"RemoveEmptyFinallyClause"** analyzer and code fix added
* **NEW** - **"RemoveEmptyArgumentList"** analyzer and code fix added
* **NEW** - **"SimplifyLogicalNotExpression"** analyzer and code fix added
* **NEW** - **"RemoveUnnecessaryCaseLabel"** analyzer and code fix added
* **NEW** - **"RemoveRedundantDefaultSwitchSection"** analyzer and code fix added
* **NEW** - **"RemoveRedundantBaseConstructorCall"** analyzer and code fix added
* **NEW** - **"RemoveEmptyNamespaceDeclaration"** analyzer and code fix added
* **NEW** - **"SimplifyIfStatementToReturnStatement"** analyzer and code fix added
* **NEW** - **"RemoveRedundantConstructor"** analyzer and code fix added
* **NEW** - **"AvoidEmptyCatchClauseThatCatchesSystemException"** analyzer and code fix added
* **NEW** - **"FormatDeclarationBraces"** analyzer and code fix added
* **NEW** - **"SimplifyLinqMethodChain"** analyzer and code fix added
* **NEW** - **"AvoidUsageOfStringEmpty"** analyzer and code fix added
* **NEW** - **"ThrowingOfNewNotImplementedException"** analyzer added
* **NEW** - **"UseCountOrLengthPropertyInsteadOfAnyMethod"** analyzer and code fix added

#### Refactorings

* **NEW** - **"Swap arguments"** refactoring added
* **NEW** - **"Swap expressions"** refactoring added
* **NEW** - **"Swap parameters"** refactoring added
* **NEW** - **"Duplicate parameter"** refactoring added
* **NEW** - **"Access element using '[]' instead of 'First/Last/ElementAt' method"** refactoring added
* **NEW** - **"Introduce constructor from selected member(s)"** refactoring added
* **NEW** - **"Change method/property/indexer type according to return statement"** refactoring added
* **"Remove member"** refactoring removes xml comment that belongs to a member
* **"Add boolean comparison"** refactoring works for return statement in method/property/indexer
* **"Convert string literal to interpolated string"** refactoring adds empty interpolation
* Bug fixed in **"Rename field according to property name"** refactoring
* Bug fixed in **"Convert foreach to for"** refactoring

### 0.9.30 (2016-05-16)

#### Analyzers

* **NEW** - **"UseForStatementToCreateInfiniteLoop"** analyzer and code fix added
* **NEW** - **"UseWhileStatementToCreateInfiniteLoop"** analyzer and code fix added
* **NEW** - **"AvoidUsageOfDoStatementToCreateInfiniteLoop"** analyzer and code fix added
* **NEW** - **UseStringLiteralInsteadOfInterpolatedString** analyzer and code fix added
* **"RemoveRedundantEmptyLine"** analyzer enhanced
* **"FormatAccessorList"** analyzer now works for auto-property accessor list
* **"MergeLocalDeclarationWithReturnStatement"** code fix now works when cursor is in return statement
* **"MergeIfStatementWithContainedIfStatement"** code fix improved (unnecessary parentheses are not added)
* bug fixed in **"SimplifyAssignmentExpression"** analyzer

#### Refactorings

* **"Extract statement(s) from if statement"** refactoring now works for topmost if statement that has else clause
* **"Format binary expression on multiple lines"** refactoring now works for a single binary expression
* **"Negate binary expression"** refactoring now works properly for a chain of logical and/or expressions
* **"Remove parameter name from each argument"** refactoring now works when any argument has parameter name
* **"Expand property and add backing field"** improved (accessor is on a single line)

### 0.9.20 (2016-05-09)

#### Analyzers

* **NEW** - **MergeIfStatementWithContainedIfStatement** analyzer and code fix added
* **NEW** - **DeclareEachTypeInSeparateFile** analyzer added
* **NEW** - **AvoidLockingOnPubliclyAccessibleInstance** analyzer and code fix added (without batch fixer)
* **NEW** - **SimplifyAssignmentExpression** analyzer and code fix added
* **NEW** - **AddEmptyLinesBetweenDeclarations** analyzer and code fix added
* **NEW** - **AvoidUsingAliasDirective** analyzer added
* **NEW** - **AvoidSemicolonAtEndOfDeclaration** analyzer and code fix added
* **UseLogicalNotOperator** analyzer renamed to **SimplifyBooleanComparison** and improved
* **RemoveRedundantBooleanLiteral** analyzer now works for `&& true` and `|| false`

#### Refactorings

* **NEW** - **"Add boolean comparison"** refactoring added
* **NEW** - **"Convert interpolated string to string literal"** refactoring added
* **NEW** - **"Convert string literal to interpolated string"** refactoring added
* **NEW** - **"Change 'Any/All' to 'All/Any'"** refactoring added
* **"Format all parameters on a single line"** refactoring now works for parameter list with a single parameter
* **"Convert to constant"** refactoring now works only for predefined types (except object)
* **"Remove comment/comments"** refactorings now work for comments that are inside trivia
* **"Make member abstract"** refactoring now work only for non-abstract indexer/method/property that are in abstract class
* **"Add/remove parameter name (to/from each argument)"** refactorings now work when cursor is right behind the parameter
* Bug fixed in **"Uncomment"** refactoring

### 0.9.11 (2016-04-30)
 
* Bug fixes and minor improvements
 
### 0.9.1 (2016-04-27)
 
* Bug fixes
 
### 0.9.0 (2016-04-26)
 
* Initial release
