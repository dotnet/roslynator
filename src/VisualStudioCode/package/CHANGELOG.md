# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]

## [4.3.0] - 2023-04-24

### Changed

- [CLI] Bump Roslyn to 4.5.0 ([#1043](https://github.com/josefpihrt/roslynator/pull/1043)).
- [CLI] Downgrade version of Microsoft.Build.Locator from 1.5.5 to 1.4.1 ([#1079](https://github.com/JosefPihrt/Roslynator/pull/1079))

### Fixed

- Fix [RCS1084](https://josefpihrt.github.io/docs/roslynator/analyzers/RCS1084) ([#1006](https://github.com/josefpihrt/roslynator/pull/1006)).
- Fix [RCS1244](https://josefpihrt.github.io/docs/roslynator/analyzers/RCS1244) ([#1007](https://github.com/josefpihrt/roslynator/pull/1007)).
- [CLI] Add nullable reference type modifier when creating a list of symbols (`list-symbols` command) ([#1013](https://github.com/josefpihrt/roslynator/pull/1013)).
- Add/remove blank line after file scoped namespace declaration ([RCS0060](https://josefpihrt.github.io/docs/roslynator/analyzers/RCS0060)) ([#1014](https://github.com/josefpihrt/roslynator/pull/1014)).
- Do not remove overriding member in record ([RCS1132](https://josefpihrt.github.io/docs/roslynator/analyzers/RCS1132)) ([#1015](https://github.com/josefpihrt/roslynator/pull/1015)).
- Do not remove parameterless empty constructor in a struct with field initializers ([RCS1074](https://josefpihrt.github.io/docs/roslynator/analyzers/RCS1074)) ([#1021](https://github.com/josefpihrt/roslynator/pull/1021)).
- Do not suggest to use generic event handler ([RCS1159](https://josefpihrt.github.io/docs/roslynator/analyzers/RCS1159)) ([#1022](https://github.com/josefpihrt/roslynator/pull/1022)).
- Fix ([RCS1077](https://josefpihrt.github.io/docs/roslynator/analyzers/RCS1077)) ([#1023](https://github.com/josefpihrt/roslynator/pull/1023)).
- Fix ([RCS1097](https://josefpihrt.github.io/docs/roslynator/analyzers/RCS1097)) ([#1037](https://github.com/JosefPihrt/Roslynator/pull/1037) by @jamesHargreaves12).
- Do not report ([RCS1170](https://josefpihrt.github.io/docs/roslynator/analyzers/RCS1170)) when `Microsoft.AspNetCore.Components.InjectAttribute` is used ([#1046](https://github.com/JosefPihrt/Roslynator/pull/1046)).
- Fix ([RCS1235](https://josefpihrt.github.io/docs/roslynator/analyzers/RCS1235)) ([#1047](https://github.com/JosefPihrt/Roslynator/pull/1047)).
- Fix ([RCS1206](https://josefpihrt.github.io/docs/roslynator/analyzers/RCS1206)) ([#1049](https://github.com/JosefPihrt/Roslynator/pull/1049)).
- Prevent possible recursion in ([RCS1235](https://josefpihrt.github.io/docs/roslynator/analyzers/RCS1235)) ([#1054](https://github.com/JosefPihrt/Roslynator/pull/1054)).
- Fix ([RCS1223](https://josefpihrt.github.io/docs/roslynator/analyzers/RCS1223)) ([#1051](https://github.com/JosefPihrt/Roslynator/pull/1051) by @jamesHargreaves12).
- Do not remove braces in the cases where there are overlapping local variables. ([RCS1031](https://josefpihrt.github.io/docs/roslynator/analyzers/RCS1031), [RCS1211](https://josefpihrt.github.io/docs/roslynator/analyzers/RCS1211), [RCS1208](https://josefpihrt.github.io/docs/roslynator/analyzers/RCS1208), [RCS1061](https://josefpihrt.github.io/docs/roslynator/analyzers/RCS1061)) ([#1039](https://github.com/JosefPihrt/Roslynator/pull/1039), [#1062](https://github.com/JosefPihrt/Roslynator/pull/1062) by @jamesHargreaves12).
- [CLI] Analyze command does not create the XML output file and returns incorrect exit code when only compiler diagnostics are reported ([#1056](https://github.com/JosefPihrt/Roslynator/pull/1056) by @PeterKaszab).
- [CLI] Fix exit code when multiple projects are processed ([#1061](https://github.com/JosefPihrt/Roslynator/pull/1061) by @PeterKaszab).
- Fix code fix for CS0164 ([#1031](https://github.com/JosefPihrt/Roslynator/pull/1031) by @jamesHargreaves12).
- Do not report `System.Windows.DependencyPropertyChangedEventArgs` as unused parameter ([RCS1163](https://josefpihrt.github.io/docs/roslynator/analyzers/RCS1163)) ([#1068](https://github.com/JosefPihrt/Roslynator/pull/1068)).
- Fix ([RCS1032](https://josefpihrt.github.io/docs/roslynator/analyzers/RCS1032)) ([#1064](https://github.com/JosefPihrt/Roslynator/pull/1064) by @jamesHargreaves12).
- Update processing of .globalconfig file to prioritize file-specific diagnostic severities over global diagnostic severities ([#1066](https://github.com/JosefPihrt/Roslynator/pull/1066/files) by @jamesHargreaves12).
- Fix RCS1009 to handles discard designations ([#1063](https://github.com/JosefPihrt/Roslynator/pull/1063/files) by @jamesHargreaves12).
- [CLI] Fix number of formatted documents, file banners added ([#1072](https://github.com/JosefPihrt/Roslynator/pull/1072)).
- Improve support for coalesce expressions in code fixes that require computing the logical inversion of an expression, such as [RCS1208](https://josefpihrt.github.io/docs/roslynator/analyzers/RCS1208) ([#1069](https://github.com/JosefPihrt/Roslynator/pull/1069) by @jamesHargreaves12).

## [4.2.0] - 2022-11-27

### Added

- Add Arm64 VS 2022 extension support ([#990](https://github.com/JosefPihrt/Roslynator/pull/990) by @snickler).
- Add analyzer "Add/remove blank line after file scoped namespace declaration" ([RCS0060](https://josefpihrt.github.io/docs/roslynator/analyzers/RCS0060)) ([#993](https://github.com/josefpihrt/roslynator/pull/993)).
  - Required option: `roslynator_blank_line_after_file_scoped_namespace_declaration = true|false`
  - Not enabled by default.
- Add analyzer "Simplify argument null check" ([RCS1255](https://josefpihrt.github.io/docs/roslynator/analyzers/RCS1255)) ([#994](https://github.com/JosefPihrt/Roslynator/pull/994)).
  - Use `ArgumentNullException.ThrowIfNull` instead of `if` null check.
  - Not enabled by default.
- Add analyzer "Invalid argument null check" ([RCS1256](https://josefpihrt.github.io/docs/roslynator/analyzers/RCS1256)) ([#888](https://github.com/JosefPihrt/Roslynator/pull/888)).
  - This analyzer reports null checks of arguments that are:
    - annotated as nullable reference type.
    - optional and its default value is `null`.
- Add package `Roslynator.Testing.CSharp.MSTest` ([#997](https://github.com/JosefPihrt/Roslynator/pull/997)).

### Changed

- Disable [RCS1080](https://josefpihrt.github.io/docs/roslynator/analyzers/RCS1080) by default ([#980](https://github.com/josefpihrt/roslynator/pull/980)).
- [CLI] Bump Roslyn to 4.4.0 ([#998](https://github.com/josefpihrt/roslynator/pull/998)).
- [CLI] Add support for .NET 7 and remove support for .NET 5 ([#985](https://github.com/josefpihrt/roslynator/pull/985).

### Fixed

- Fix [RCS1080](https://josefpihrt.github.io/docs/roslynator/analyzers/RCS1080) when collection is derived from `List<T>` ([#986](https://github.com/josefpihrt/roslynator/pull/986)).
- Fix retrieving of trusted platform assemblies - separator differs by OS ([#987](https://github.com/josefpihrt/roslynator/pull/987)).
- Fix refactoring ([RR0014](https://josefpihrt.github.io/docs/roslynator/analyzers/RR0014)) ([#988](https://github.com/josefpihrt/roslynator/pull/988)).
- Fix refactoring ([RR0180](https://josefpihrt.github.io/docs/roslynator/analyzers/RR0180)) ([#988](https://github.com/josefpihrt/roslynator/pull/988)).
- Recognize `ArgumentNullException.ThrowIfNull` ([RCS1227](https://josefpihrt.github.io/docs/roslynator/analyzers/RCS1227)) ([#992](https://github.com/josefpihrt/roslynator/pull/992)).
- Detect pattern matching in [RCS1146](https://josefpihrt.github.io/docs/roslynator/analyzers/RCS1146) ([#999](https://github.com/josefpihrt/roslynator/pull/999)).
- Handle `using` directive that starts with `global::` [RCS0015](https://josefpihrt.github.io/docs/roslynator/analyzers/RCS0015) ([#1000](https://github.com/josefpihrt/roslynator/pull/1000)).
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

### Changed

- Rename default branch to `main`.
- Format changelog according to 'Keep a Changelog' ([#915](https://github.com/josefpihrt/roslynator/pull/915)).
- [CLI] Improve release build of command-line tool ([#912](https://github.com/josefpihrt/roslynator/pull/912)).
- Do not sort properties in an initializer ([RR0216](https://josefpihrt.github.io/docs/roslynator/refactorings/RR0216)).
- [CLI] Bump Roslyn to 4.3.1 ([#969](https://github.com/josefpihrt/roslynator/pull/969)).
- [CLI] Bump Microsoft.Build.Locator to 1.5.5 ([#969](https://github.com/josefpihrt/roslynator/pull/969)).

### Fixed

- [CLI] Fix filtering of projects (relates to `--projects` or `--ignored-projects` parameter) ([#914](https://github.com/josefpihrt/roslynator/pull/914)).
- Refactoring "Add using directive" (RR0014) now works when file-scoped namespace is used ([#932](https://github.com/josefpihrt/roslynator/pull/932)).
- Add parentheses if necessary in a code fix for [RCS1197](https://josefpihrt.github.io/docs/roslynator/analyzers/RCS1197) ([#928](https://github.com/josefpihrt/roslynator/pull/928) by @karl-sjogren).
- Do not simplify default expression if it would change semantics ([RCS1244](https://josefpihrt.github.io/docs/roslynator/analyzers/RCS1244)) ([#939](https://github.com/josefpihrt/roslynator/pull/939).
- Fix NullReferenceException in [RCS1198](https://josefpihrt.github.io/docs/roslynator/analyzers/RCS1198) ([#940](https://github.com/josefpihrt/roslynator/pull/940).
- Order named arguments even if optional arguments are not specified [RCS1205](https://josefpihrt.github.io/docs/roslynator/analyzers/RCS1205) ([#941](https://github.com/josefpihrt/roslynator/pull/941), ([#965](https://github.com/josefpihrt/roslynator/pull/965).
- Prefix identifier with `@` if necessary ([RCS1220](https://josefpihrt.github.io/docs/roslynator/analyzers/RCS1220)) ([#943](https://github.com/josefpihrt/roslynator/pull/943).
- Do not suggest to make local variable a const when it is used in ref extension method ([RCS1118](https://josefpihrt.github.io/docs/roslynator/analyzers/RCS1118)) ([#948](https://github.com/josefpihrt/roslynator/pull/948).
- Fix formatting of argument list ([#952](https://github.com/josefpihrt/roslynator/pull/952).
- Do not remove async/await when 'using declaration' is used ([#953](https://github.com/josefpihrt/roslynator/pull/953).
- Convert if-else to return statement when pattern matching is used ([RCS1073](https://josefpihrt.github.io/docs/roslynator/analyzers/RCS1073)) ([#956](https://github.com/josefpihrt/roslynator/pull/956).
- [CLI] Include compiler diagnostics in the xml output file of the `roslynator analyze` command ([#964](https://github.com/JosefPihrt/Roslynator/pull/964) by @PeterKaszab).
- Do not simplify 'default' expression if the type is inferred ([RCS1244](https://josefpihrt.github.io/docs/roslynator/analyzers/RCS1244)) ([#966](https://github.com/josefpihrt/roslynator/pull/966).
- Use explicit type from lambda expression ([RCS1008](https://josefpihrt.github.io/docs/roslynator/analyzers/RCS1008)) ([#967](https://github.com/josefpihrt/roslynator/pull/967).
- Do not remove constructor if it is decorated with 'UsedImplicitlyAttribute' ([RCS1074](https://josefpihrt.github.io/docs/roslynator/analyzers/RCS1074)) ([#968](https://github.com/josefpihrt/roslynator/pull/968).
- Detect argument null check in the form of `ArgumentNullException.ThrowIfNull` ([RR0025](https://josefpihrt.github.io/docs/roslynator/refactorings/RR0025), [RCS1227](https://josefpihrt.github.io/docs/roslynator/analyzers/RCS1227)) ([#974](https://github.com/josefpihrt/roslynator/pull/974).
- Do not make generic class static if it's inherited ([RCS1102](https://josefpihrt.github.io/docs/roslynator/analyzers/RCS1102)) ([#978](https://github.com/josefpihrt/roslynator/pull/978).

-----
<!-- Content below does not adhere to 'Keep a Changelog' format -->

## 4.1.1 (2022-05-29)

* Bug fixes

## 4.1.0 (2022-03-29)

* Add analyzer [RCS1254](https://josefpihrt.github.io/docs/roslynator/analyzers/RCS1254) (Normalize format of enum flag value)
* Add analyzer [RCS1253](https://josefpihrt.github.io/docs/roslynator/analyzers/RCS1253) (Format documentation comment summary)
* Add analyzer [RCS1252](https://josefpihrt.github.io/docs/roslynator/analyzers/RCS1252) (Normalize usage of infinite loop)
* Add analyzer [RCS1251](https://josefpihrt.github.io/docs/roslynator/analyzers/RCS1251) (Remove unnecessary braces from record declaration)
* Add refactoring [Deconstruct foreach variable (RR0217)](https://josefpihrt.github.io/docs/roslynator/refactorings/RR0217)
* Add code fix for CS8602, CS8604
* Add suggestion to call AddRange instead of Add (RCS1235)
* Put back refactoring "Split local declaration and assignment" (RR0194) ([issue](https://github.com/JosefPihrt/Roslynator/issues/881))
* Adopt activation events in VS Code extension ([issue](https://github.com/JosefPihrt/Roslynator/issues/883)) (thanks to [ProphetLamb](https://github.com/ProphetLamb))
* Fix: Get config value from global AnalyzerConfig if available ([issue](https://github.com/JosefPihrt/Roslynator/issues/884))
* Fix: Do not suggest using null-forgiving operator for parameter default value (CS8625)
* Fix: Check if equality operator is overloaded (RCS1171)
* Fix: Do not remove field initialization in struct with constructor(s) (RCS1129)

## 4.0.3 (2022-01-29)

* Fixed release for VS Code

## 4.0.2 (2022-01-29)

* Disable analyzer ROS003 by default ([commit](https://github.com/JosefPihrt/Roslynator/commit/9c562921b6ae4eb46e1cfe252282e6b2ad520ca6))
* Analyzers that require option to be set should disabled by default (RCS1018, RCS1096, RCS1250) ([commit](https://github.com/JosefPihrt/Roslynator/commit/de374858f9d8120a6f6d705ad685101ed1bab699))

### Bug fixes

* Fix analyzer [RCS1014](https://josefpihrt.github.io/docs/roslynator/analyzers/RCS1014) (Use explicitly/implicitly typed array) ([commit](https://github.com/JosefPihrt/Roslynator/commit/004a83756b9fbcf117710d7afb6bab964a59f1be))
* Fix analyzer [RCS1016](https://josefpihrt.github.io/docs/roslynator/analyzers/RCS1016) (Use block body or expression body) ([commit](https://github.com/JosefPihrt/Roslynator/commit/8c633e966f2706d3888fd942dd186d066d440ac0))
* Fix refactoring AddUsingDirective (RR0013) ([commit](https://github.com/JosefPihrt/Roslynator/commit/199787bdf921aeeecd69d9a118bbb86419bce81a))
* Propagate some options from .roslynatorconfig ([commit](https://github.com/JosefPihrt/Roslynator/commit/a619ebf285d1de77941a9c4a5fce46bb19485d3a))
* Enable ROS analyzers to be set from .roslynatorconfig ([commit](https://github.com/JosefPihrt/Roslynator/commit/a4c0ad8fb60d694cc7d2546016d742547f1d585b))
* Files generated with source generators have relative paths ([commit](https://github.com/JosefPihrt/Roslynator/commit/cec55ab23404a11f4fe332a3568ab87a4016e55b))

## 4.0.1 (2022-01-21)

* Bug fixes

## 4.0.0 (2022-01-16)

* Bump Roslyn version to 4.0.1
* Change category of all analyzers to 'Roslynator'
* Migrate all options to EditorConfig
  * Enable/disable all analyzers
  * Enable/disable all refactorings
  * Enable/disable specific refactoring
  * Enable/disable all compiler diagnostics fixes
  * Enable/disable specific compiler diagnostic fix

* Add analyzer NormalizeWhitespaceAtBeginningOfFile (RCS0057)
* Add analyzer NormalizeWhitespaceAtEndOfFile (RCS0058)
* Add analyzer PlaceNewLineAfterOrBeforeNullConditionalOperator (RCS0059)
* Add analyzer UnnecessaryNullForgivingOperator (RCS1249)
* Add analyzer UseImplicitOrExplicitObjectCreation (RCS1250)
* Add refactoring ExpandPositionalRecord (RR0215)
* Add refactoring AddAllPropertiesToInitializer (RR0216)
* Add code fix for CS8403, CS8618 and CS8625

## 3.3.0 (2021-11-14)

* Bunch of bug fixes and small improvements
* Disable analyzers RCS1079 and RCS1090 by default

## 3.2.2 (2021-08-15)

* Ensure that shared assemblies with be loaded properly on .NET Core ([issue](https://github.com/JosefPihrt/Roslynator/issues/833))

## 3.2.1 (2021-06-30)

* Publish Roslynator for Visual Studio 2022 Preview
* Bug fixes and various improvements

## 3.2.0 (2021-04-26)

* Publish [Roslynator Testing Framework](https://www.nuget.org/packages/Roslynator.Testing.CSharp.Xunit)
* Support editorconfig to configure analyzer options ([commit](https://github.com/JosefPihrt/Roslynator/commit/da88ce64e0b3975ad69e05a1d4cdcc761f358a09))
* Update references to Roslyn API to 3.8.0
* A bunch of bug fixes

### Analyzers

* Add option to invert analyzer [RCS1016](https://josefpihrt.github.io/docs/roslynator/analyzers/RCS1016) ([commit](https://github.com/JosefPihrt/Roslynator/commit/67a0fc5cfe9dd793cc6e504513ed6805678c1739))
* Add more cases to analyzer [RCS1218](https://josefpihrt.github.io/docs/roslynator/analyzers/RCS1218) ([commit](https://github.com/JosefPihrt/Roslynator/commit/37e8edb7a2eefdd4a7749dd6a3f5b473ebbdcc0a))
* Convert `!= null` to `is not null` ([RCS1248](https://josefpihrt.github.io/docs/roslynator/analyzers/RCS1248)) ([commit](https://github.com/JosefPihrt/Roslynator/commit/432a8fea3147447536dbb8fac47598ad1db68158))

### Code Fixes

* Add code fix for CS7036 ([commit](https://github.com/JosefPihrt/Roslynator/commit/9eae7307b9cab96c2d91e97aef8bda098c7e92d9))
* Add code fix for CS8632 ([commit](https://github.com/JosefPihrt/Roslynator/commit/2c1d9ca64d2305e1ce278e1db6563d82582c4613))
* Improve code fix for CS0029, CS0246 ([commit](https://github.com/JosefPihrt/Roslynator/commit/5557ad29412b5f758cb97da6e298e1f4b0d49e3d))
* Add option for code fix for CS1591 ([commit](https://github.com/JosefPihrt/Roslynator/commit/089dbed656556a526f236dce75eadffb4e1d78a0))

## 3.1.0 (2021-01-04)

* Add analyzer [RCS0056](https://josefpihrt.github.io/docs/roslynator/analyzers/RCS0056) (Line is too long)
* Add option to suppress diagnostic from Unity script methods (RCS1213)
* Consider syntax `var foo = Foo.Parse(value)` as having obvious type `Foo`
* Update references to Roslyn API to 3.7.0

## 3.0.1 (2020-10-19)

* Add analyzer [RCS0055](https://josefpihrt.github.io/docs/roslynator/analyzers/RCS0055) (Fix formatting of a binary expression chain)
* Add analyzer [RCS0054](https://josefpihrt.github.io/docs/roslynator/analyzers/RCS0054) (Fix formatting of a call chain)
* Add analyzer [RCS0053](https://josefpihrt.github.io/docs/roslynator/analyzers/RCS0053) (Fix formatting of a list)
* Add analyzer [RCS0052](https://josefpihrt.github.io/docs/roslynator/analyzers/RCS0052) (Add newline before equals sign instead of after it (or vice versa))
* Add analyzer [RCS1248](https://josefpihrt.github.io/docs/roslynator/analyzers/RCS1248) (Use 'is null' pattern instead of comparison (or vice versa)) ([issue](https://github.com/JosefPihrt/Roslynator/issues/458))
* Add analyzer [RCS1247](https://josefpihrt.github.io/docs/roslynator/analyzers/RCS1247) (Fix documentation comment tag)
* Add analyzer option [RCS1207i](https://josefpihrt.github.io/docs/roslynator/analyzers/RCS1207i) (Convert method group to anonymous function)
* Add analyzer option [RCS1090i](https://josefpihrt.github.io/docs/roslynator/analyzers/RCS1090i) (Remove call to 'ConfigureAwait')
* Add analyzer option [RCS1018i](https://josefpihrt.github.io/docs/roslynator/analyzers/RCS1018i) (Remove accessibility modifiers) ([issue](https://github.com/JosefPihrt/Roslynator/issues/260))
* Add analyzer option [RCS1014i](https://josefpihrt.github.io/docs/roslynator/analyzers/RCS1014i) (Use implicitly typed array)
* Add analyzer option [RCS1014a](https://josefpihrt.github.io/docs/roslynator/analyzers/RCS1014a) (Use implicitly typed array (when type is obvious))
* Add analyzer option [RCS1078i](https://josefpihrt.github.io/docs/roslynator/analyzers/RCS1078i) (Use string.Empty instead of "")
* Add analyzer option [RCS1016a](https://josefpihrt.github.io/docs/roslynator/analyzers/RCS1016a) (Convert expression-body to block body when expression is multi-line)
* Add analyzer option [RCS1016b](https://josefpihrt.github.io/docs/roslynator/analyzers/RCS1016b) (Convert expression-body to block body when declaration is multi-line)
* Disable by default analyzer [RCS1207i](https://josefpihrt.github.io/docs/roslynator/analyzers/RCS1207i) (Convert method group to anonymous function)
* Remove analyzer [RCS1219](https://josefpihrt.github.io/docs/roslynator/analyzers/RCS1219) (Call 'Enumerable.Skip' and 'Enumerable.Any' instead of 'Enumerable.Count')
* Rename analyzer "Avoid 'null' on left side of binary expression" to "Constant values should be placed on right side of comparisons" [RCS1098](https://josefpihrt.github.io/docs/roslynator/analyzers/RCS1098)
* Rename analyzer "Simplify boolean expression" to "Unnecessary null check" [RCS1199](https://josefpihrt.github.io/docs/roslynator/analyzers/RCS1199) ([issue](https://github.com/JosefPihrt/Roslynator/issues/373))

* More syntax is considered as having obvious type:
  * string literal
  * character literal
  * boolean literal
  * implicit array creation that contains only expressions whose type is obvious

## 3.0.0 (2020-06-16)

* Update references to Roslyn API to 3.5.0
* Introduce concept of "[Analyzer Options](https://github.com/JosefPihrt/Roslynator/blob/main/docs/AnalyzerOptions)"
* Reassign ID for some analyzers.
  * See "[How to: Migrate Analyzers to Version 3.0](https://github.com/JosefPihrt/Roslynator/blob/main/docs/HowToMigrateAnalyzersToVersion3)"
* Remove references to Roslynator assemblies from omnisharp.json on uninstall

## 2.9.0 (2020-03-13)

* Switch to Roslyn 3.x libraries
* Add `Directory.Build.props` file
* Add open configuration commands to Command Palette (VS Code) ([PR](https://github.com/JosefPihrt/Roslynator/pull/648))

### Bug Fixes

* Fix key duplication/handle camel case names in `omnisharp.json` ([PR](https://github.com/JosefPihrt/Roslynator/pull/645))
* Use prefix unary operator instead of postfix unary operator ([RCS1089](https://josefpihrt.github.io/docs/roslynator/analyzers/RCS1089)) ([issue](https://github.com/JosefPihrt/Roslynator/issues/639))
* Cast of `this` to its interface cannot be null ([RCS1202](https://josefpihrt.github.io/docs/roslynator/analyzers/RCS1202)) ([issue](https://github.com/JosefPihrt/Roslynator/issues/640))
* Do not remove braces in switch section if it contains 'using variable' ([RCS1031](https://josefpihrt.github.io/docs/roslynator/analyzers/RCS1031)) ([issue](https://github.com/JosefPihrt/Roslynator/issues/632))

### New Analyzers

* [RCS1242](https://josefpihrt.github.io/docs/roslynator/analyzers/RCS1242) (DoNotPassNonReadOnlyStructByReadOnlyReference).
* [RCS1243](https://josefpihrt.github.io/docs/roslynator/analyzers/RCS1243) (DuplicateWordInComment).
* [RCS1244](https://josefpihrt.github.io/docs/roslynator/analyzers/RCS1244) (SimplifyDefaultExpression).
* [RCS1245](https://josefpihrt.github.io/docs/roslynator/analyzers/RCS1245) (SimplifyConditionalExpression2) ([issue](https://github.com/JosefPihrt/Roslynator/issues/612)).

### Analyzers

* Disable analyzer [RCS1057](https://josefpihrt.github.io/docs/roslynator/analyzers/RCS1057) by default ([issue](https://github.com/JosefPihrt/Roslynator/issues/590)).
* Merge analyzer [RCS1156](https://josefpihrt.github.io/docs/roslynator/analyzers/RCS1156) with [RCS1113](https://josefpihrt.github.io/docs/roslynator/analyzers/RCS1113) ([issue](https://github.com/JosefPihrt/Roslynator/issues/650)).
  * `x == ""` should be replaced with `string.IsNullOrEmpty(x)`
* Improve analyzer [RCS1215](https://josefpihrt.github.io/docs/roslynator/analyzers/RCS1215) ([commit](https://github.com/JosefPihrt/Roslynator/commit/0fdd97f9a62463f8b004abeb17a8b8509374c35a)).
  * `x == double.NaN` should be replaced with `double.IsNaN(x)`
* Enable [RCS1169](https://josefpihrt.github.io/docs/roslynator/analyzers/RCS1169) and [RCS1170](https://josefpihrt.github.io/docs/roslynator/analyzers/RCS1170) if the type is read-only struct ([commit](https://github.com/JosefPihrt/Roslynator/commit/f34e105433dbc65686369adf712b0b99d93eaef7)).
* Improve analyzer [RCS1077](https://josefpihrt.github.io/docs/roslynator/analyzers/RCS1077) ([commit](https://github.com/JosefPihrt/Roslynator/commit/3ee275442cb16f6a9104b42d582ba7d76d6df88c)).
  * `x.OrderBy(y => y).Reverse()` can be simplified to `x.OrderByDescending(y => y)`
  * `x.SelectMany(y => y).Count()` can be simplified to `x.Sum(y => y.Count)` if `x` has `Count` or `Length` property
* Improve analyzer [RCS1161](https://josefpihrt.github.io/docs/roslynator/analyzers/RCS1161) - Declare explicit enum value using `<<` operator ([commit](https://github.com/JosefPihrt/Roslynator/commit/6b78496efe1a2f2678f2ef2a71986e2bee006863)).
* Improve analyzer [RCS1036](https://josefpihrt.github.io/docs/roslynator/analyzers/RCS1036) - remove empty line between documentation comment and declaration ([commit](https://github.com/JosefPihrt/Roslynator/commit/de0f1205671281679866e92edd9337a7416409e6)).
* Improve analyzer [RCS1037](https://josefpihrt.github.io/docs/roslynator/analyzers/RCS1037) - remove trailing white-space from documentation comment ([commit](https://github.com/JosefPihrt/Roslynator/commit/c3f7d193ee37d04de7e2c698aab7f3e1e6350e80)).
* Improve analyzer [RCS1143](https://josefpihrt.github.io/docs/roslynator/analyzers/RCS1143) ([commit](https://github.com/JosefPihrt/Roslynator/commit/4c4281ebdf8eb0aa1a77d5e5bfda71bc66cce1df))
  * `x?.M() ?? default(int?)` can be simplified to `x?.M()` if `x` is a nullable struct.
* Improve analyzer [RCS1206](https://josefpihrt.github.io/docs/roslynator/analyzers/RCS1206) ([commit](https://github.com/JosefPihrt/Roslynator/commit/88dd4cea4df07f036a8296511410ccff70f8fefe))
  * `(x != null) ? x.M() : default(int?)` can be simplified to `x?.M()` if `x` is a nullable struct.

## 2.3.0 (2019-12-28)

* Automatically update configuration in omnisharp.json (VS Code) ([PR](https://github.com/JosefPihrt/Roslynator/pull/623)).

## 2.2.0 (2019-09-28)

* Enable configuration for non-Windows systems (VS Code).

### Analyzers

* Disable analyzer [RCS1029](https://josefpihrt.github.io/docs/roslynator/analyzers/RCS1029) (FormatBinaryOperatorOnNextLine) by default.

## 2.1.4 (2019-08-13)

* Initial release of Roslynator for VS Code.
