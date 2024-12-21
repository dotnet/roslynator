# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]

## [4.12.10] - 2024-12-17

### Fixed

- Fix analyzer [RCS1213](https://josefpihrt.github.io/docs/roslynator/analyzers/RCS1213) ([PR](https://github.com/dotnet/roslynator/pull/1586))
- Improve code fixer for [RCS1228](https://josefpihrt.github.io/docs/roslynator/analyzers/RCS1228) ([PR](https://github.com/dotnet/roslynator/pull/1585))
- Fix diagnostic message for [RCS0032](https://josefpihrt.github.io/docs/roslynator/analyzers/RCS0032) ([PR](https://github.com/dotnet/roslynator/pull/1588))

### Changed

- Update whitespace formatting rules ([PR](https://github.com/dotnet/roslynator/pull/1576))
- Ensure that diagnostics are not reported with zero length ([PR](https://github.com/dotnet/roslynator/pull/1590))

## [4.12.9] - 2024-10-25

### Fixed

- Fix analyzer [RCS1090](https://josefpihrt.github.io/docs/roslynator/analyzers/RCS1090) ([PR](https://github.com/dotnet/roslynator/pull/1566))
- Fix analyzer [RCS1124](https://josefpihrt.github.io/docs/roslynator/analyzers/RCS1124) ([PR](https://github.com/dotnet/roslynator/pull/1572))
- [CLI] Fix command `generate-doc` ([PR](https://github.com/dotnet/roslynator/pull/1568), [PR](https://github.com/dotnet/roslynator/pull/1570))

### Changed

- Update analyzer [RCS1077](https://josefpihrt.github.io/docs/roslynator/analyzers/RCS1077) ([PR](https://github.com/dotnet/roslynator/pull/1653))
    - Do not suggest to change `list.FirstOrDefault(predicate)` to `list.Find(predicate)`.
      Performance gain is negligible and actually `FirstOrDefault` can be even faster on .NET 9 (see related [issue](https://github.com/dotnet/roslynator/pull/1531) for more details).

## [4.12.8] - 2024-10-11

### Fixed

- Fix analyzer [RCS0053](https://josefpihrt.github.io/docs/roslynator/analyzers/RCS0053) ([PR](https://github.com/dotnet/roslynator/pull/1547))
- Fix analyzer [RCS1223](https://josefpihrt.github.io/docs/roslynator/analyzers/RCS1223) ([PR](https://github.com/dotnet/roslynator/pull/1552))
- Fix analyzer [RCS1140](https://josefpihrt.github.io/docs/roslynator/analyzers/RCS1140) ([PR](https://github.com/dotnet/roslynator/pull/1554))
- Fix analyzer [RCS1096](https://josefpihrt.github.io/docs/roslynator/analyzers/RCS1096) ([PR](https://github.com/dotnet/roslynator/pull/1558))
- [CLI] Improve removing of unused symbols ([PR](https://github.com/dotnet/roslynator/pull/1550))
- [CLI] Fix command `generate-doc` ([PR](https://github.com/dotnet/roslynator/pull/1559))

## [4.12.7] - 2024-10-01

### Fixed

- Fix analyzer [RCS1202](https://josefpihrt.github.io/docs/roslynator/analyzers/RCS1202) ([PR](https://github.com/dotnet/roslynator/pull/1542))
- Fix analyzer [RCS1246](https://josefpihrt.github.io/docs/roslynator/analyzers/RCS1246) ([PR](https://github.com/dotnet/roslynator/pull/1543))
- Fix analyzer [RCS1140](https://josefpihrt.github.io/docs/roslynator/analyzers/RCS1140) ([PR](https://github.com/dotnet/roslynator/pull/1524) by @Qluxzz)
- Fix analyzer [RCS1077](https://josefpihrt.github.io/docs/roslynator/analyzers/RCS1077) ([PR](https://github.com/dotnet/roslynator/pull/1544))

### Changed

- Add support for duck-typed awaitables and task-like types for Task/Async-related analyzers ([PR](https://github.com/dotnet/roslynator/pull/1535) by @Govorunb)
    - Affects the following analyzers:
        - [RCS1046](https://josefpihrt.github.io/docs/roslynator/analyzers/RCS1046)
        - [RCS1047](https://josefpihrt.github.io/docs/roslynator/analyzers/RCS1047)
        - [RCS1090](https://josefpihrt.github.io/docs/roslynator/analyzers/RCS1090)
        - [RCS1174](https://josefpihrt.github.io/docs/roslynator/analyzers/RCS1174)
        - [RCS1229](https://josefpihrt.github.io/docs/roslynator/analyzers/RCS1229)
        - [RCS1261](https://josefpihrt.github.io/docs/roslynator/analyzers/RCS1261)
    - Affects refactoring [RR0209](https://josefpihrt.github.io/docs/roslynator/refactorings/RR0209)

## [4.12.6] - 2024-09-23

### Added

- Analyzer [RCS1077](https://josefpihrt.github.io/docs/roslynator/analyzers/RCS1077) now suggests to use `Order` instead of `OrderBy` ([PR](https://github.com/dotnet/roslynator/pull/1522) by @BenjaminBrienen)

### Fixed

- Fix analyzer [RCS0053](https://josefpihrt.github.io/docs/roslynator/analyzers/RCS0053) ([PR](https://github.com/dotnet/roslynator/pull/1518))
- Fix analyzer [RCS0056](https://josefpihrt.github.io/docs/roslynator/analyzers/RCS0056) ([PR](https://github.com/dotnet/roslynator/pull/1521))
- Fix analyzer [RCS1181](https://josefpihrt.github.io/docs/roslynator/analyzers/RCS1181) ([PR](https://github.com/dotnet/roslynator/pull/1526))
- Fix analyzer [RCS0005](https://josefpihrt.github.io/docs/roslynator/analyzers/RCS0005) ([PR](https://github.com/dotnet/roslynator/pull/1533))
- Fix analyzer [RCS1181](https://josefpihrt.github.io/docs/roslynator/analyzers/RCS1181) ([PR](https://github.com/dotnet/roslynator/pull/1534))

## [4.12.5] - 2024-09-13

### Fixed

- Fix analyzer [RCS1182](https://josefpihrt.github.io/docs/roslynator/analyzers/RCS1182) ([PR](https://github.com/dotnet/roslynator/pull/1502))
- Fix analyzer [RCS1198](https://josefpihrt.github.io/docs/roslynator/analyzers/RCS1198) ([PR](https://github.com/dotnet/roslynator/pull/1501))
- Fix analyzer [RCS1214](https://josefpihrt.github.io/docs/roslynator/analyzers/RCS1214) ([PR](https://github.com/dotnet/roslynator/pull/1500))
- Fix analyzer [RCS1018](https://josefpihrt.github.io/docs/roslynator/analyzers/RCS1018) ([PR](https://github.com/dotnet/roslynator/pull/1510))
- Fix analyzer [RCS1264](https://josefpihrt.github.io/docs/roslynator/analyzers/RCS1264) ([PR](https://github.com/dotnet/roslynator/pull/1511))
- Fix analyzer [RCS0053](https://josefpihrt.github.io/docs/roslynator/analyzers/RCS0053) ([PR](https://github.com/dotnet/roslynator/pull/1512))
- Fix analyzer [RCS0056](https://josefpihrt.github.io/docs/roslynator/analyzers/RCS0056) ([PR](https://github.com/dotnet/roslynator/pull/1514))

### Changed

- Bump Roslyn to 4.11.0 ([PR](https://github.com/dotnet/roslynator/pull/1483))
    - Applies to CLI and testing library.

### Removed

- [CLI] Remove support for .NET SDK 6 ([PR](https://github.com/dotnet/roslynator/pull/1483))

## [4.12.4] - 2024-06-01

### Fixed

- Fix analyzer [RCS1108](https://josefpihrt.github.io/docs/roslynator/analyzers/RCS1108) ([PR](https://github.com/dotnet/roslynator/pull/1469))
- Fix analyzer [RCS1201](https://josefpihrt.github.io/docs/roslynator/analyzers/RCS1201) ([PR](https://github.com/dotnet/roslynator/pull/1470))
- Fix analyzer [RCS0012](https://josefpihrt.github.io/docs/roslynator/analyzers/RCS0012) ([PR](https://github.com/dotnet/roslynator/pull/1472))
- [CLI] Fix duplicate analyzers ([PR](https://github.com/dotnet/roslynator/pull/1477))

## [4.12.3] - 2024-05-10

### Fixed

- Fix analyzer [RCS1246](https://josefpihrt.github.io/docs/roslynator/analyzers/RCS1246) ([PR](https://github.com/dotnet/roslynator/pull/1460))
- Fix analyzer [RCS1085](https://josefpihrt.github.io/docs/roslynator/analyzers/RCS1085) ([PR](https://github.com/dotnet/roslynator/pull/1461))
- Fix analyzer [RCS1077](https://josefpihrt.github.io/docs/roslynator/analyzers/RCS1077) ([PR](https://github.com/dotnet/roslynator/pull/1463))
- [CLI] Fix `roslynator analyze --include/--exclude` ([PR](https://github.com/dotnet/roslynator/pull/1459))
- Fix analyzer [RCS0036](https://josefpihrt.github.io/docs/roslynator/analyzers/RCS0036) ([PR](https://github.com/dotnet/roslynator/pull/1466))

## [4.12.2] - 2024-04-23

### Fixed

- [CLI] Fix loading of `slnf` files ([PR](https://github.com/dotnet/roslynator/pull/1447))
- [CLI] Fix `--severity-level` ([PR](https://github.com/dotnet/roslynator/pull/1449))
- Fix analyzer [RCS1246](https://josefpihrt.github.io/docs/roslynator/analyzers/RCS1246) ([PR](https://github.com/dotnet/roslynator/pull/1451))

## [4.12.1] - 2024-04-15

### Changed

- [CLI] Bump Roslyn to 4.9.2 ([PR](https://github.com/dotnet/roslynator/pull/1441))
- Convert `Last()` to `[]` ([RCS1246](https://josefpihrt.github.io/docs/roslynator/analyzers/RCS1246)) ([PR](https://github.com/dotnet/roslynator/pull/1436))

### Fixed

- Fix analyzer [RCS1077](https://josefpihrt.github.io/docs/roslynator/analyzers/RCS1077) ([PR](https://github.com/dotnet/roslynator/pull/1428))
- Fix export of `ILanguageService` ([PR](https://github.com/dotnet/roslynator/pull/1442))

## [4.12.0] - 2024-03-19

### Added

- Add analyzer "Simplify numeric comparison" [RCS1268](https://josefpihrt.github.io/docs/roslynator/analyzers/RCS1268) ([PR](https://github.com/dotnet/roslynator/pull/1405) by @jakubreznak)

### Fixed

- Fix analyzer [RCS1267](https://josefpihrt.github.io/docs/roslynator/analyzers/RCS1267) ([PR](https://github.com/dotnet/roslynator/pull/1412))
- Fix "Unknown value 'Default'" exception ([PR](https://github.com/dotnet/roslynator/pull/1411))
- Fix name of `UnityEngine.SerializeField` attribute ([PR](https://github.com/dotnet/roslynator/pull/1419))
- Fix analyzer [RCS1077](https://josefpihrt.github.io/docs/roslynator/analyzers/RCS1077) ([PR](https://github.com/dotnet/roslynator/pull/1421))

## [4.11.0] - 2024-02-19

### Added

- Add analyzer "Use raw string literal" [RCS1266](https://josefpihrt.github.io/docs/roslynator/analyzers/RCS1266) ([PR](https://github.com/dotnet/roslynator/pull/1375))
- Add analyzer "Convert 'string.Concat' to interpolated string" [RCS1267](https://josefpihrt.github.io/docs/roslynator/analyzers/RCS1267) ([PR](https://github.com/dotnet/roslynator/pull/1379))
- Simplify LINQ query [RCS1077](https://josefpihrt.github.io/docs/roslynator/analyzers/1077) ([PR](https://github.com/dotnet/roslynator/pull/1384))
  - `items.Select(selector).Average()` => `items.Average(selector)`
  - `items.Select(selector).Sum()` => `items.Sum(selector)`

### Fixed

- Fix analyzer [RCS0049](https://josefpihrt.github.io/docs/roslynator/analyzers/RCS0049) ([PR](https://github.com/dotnet/roslynator/pull/1386))
- Fix analyzer [RCS1159](https://josefpihrt.github.io/docs/roslynator/analyzers/RCS1159) ([PR](https://github.com/dotnet/roslynator/pull/1390))
- Fix analyzer [RCS1019](https://josefpihrt.github.io/docs/roslynator/analyzers/RCS1019) ([PR](https://github.com/dotnet/roslynator/pull/1402))
- Fix analyzer [RCS1250](https://josefpihrt.github.io/docs/roslynator/analyzers/RCS1250) ([PR](https://github.com/dotnet/roslynator/pull/1403), [PR](https://github.com/dotnet/roslynator/pull/1404))
- Fix code fix for [CS8600](https://josefpihrt.github.io/docs/roslynator/fixes/CS8600) changing the wrong type when casts or `var` are involved ([PR](https://github.com/dotnet/roslynator/pull/1393) by @jroessel)
- Fix Roslyn multi-targeting ([PR](https://github.com/dotnet/roslynator/pull/1407))

## [4.10.0] - 2024-01-24

### Added

- Publish NuGet packages that provide [refactorings](https://www.nuget.org/packages/roslynator.refactorings) and [code fixes for compiler diagnostics](https://www.nuget.org/packages/roslynator.codefixes) ([PR](https://github.com/dotnet/roslynator/pull/1358))
  - These packages are recommended to be used in an environment where Roslynator IDE extension cannot be used, e.g. VS Code + C# Dev Kit (see related [issue](https://github.com/dotnet/vscode-csharp/issues/6790))
- Add analyzer "Remove redundant catch block" [RCS1265](https://josefpihrt.github.io/docs/roslynator/analyzers/RCS1265) ([PR](https://github.com/dotnet/roslynator/pull/1364) by @jakubreznak)
- [CLI] Spellcheck file names ([PR](https://github.com/dotnet/roslynator/pull/1368))
  - `roslynator spellcheck --scope file-name` 

### Changed

- Update analyzer [RCS1197](https://josefpihrt.github.io/docs/roslynator/analyzers/RCS1197) ([PR](https://github.com/dotnet/roslynator/pull/1370))
  - Do not report interpolated string and string concatenation 

### Fixed

- Fix analyzer [RCS1055](https://josefpihrt.github.io/docs/roslynator/analyzers/RCS1055) ([PR](https://github.com/dotnet/roslynator/pull/1361))
- Fix analyzer [RCS1261](https://josefpihrt.github.io/docs/roslynator/analyzers/RCS1261) ([PR](https://github.com/dotnet/roslynator/pull/1374))
- Fix analyzer [RCS0056](https://josefpihrt.github.io/docs/roslynator/analyzers/RCS0056) ([PR](https://github.com/dotnet/roslynator/pull/1373))
- Fix analyzer [RCS1211](https://josefpihrt.github.io/docs/roslynator/analyzers/RCS1211) ([PR](https://github.com/dotnet/roslynator/pull/1377))
- Fix analyzer [RCS0061](https://josefpihrt.github.io/docs/roslynator/analyzers/RCS0061) ([PR](https://github.com/dotnet/roslynator/pull/1376))

## [4.9.0] - 2024-01-10

### Added

- Add support for Unity ([PR](https://github.com/dotnet/roslynator/pull/1349))
  - [Unity uses Roslyn 3.8](https://docs.unity3d.com/Manual/roslyn-analyzers.html) and this version is now supported by Roslynator NuGet packages with analyzers (Roslynator.Analyzers etc.)

### Fixed

- Fix analyzer [RCS0034](https://josefpihrt.github.io/docs/roslynator/analyzers/RCS0034) ([PR](https://github.com/dotnet/roslynator/pull/1351))
- Fix analyzer [RCS0023](https://josefpihrt.github.io/docs/roslynator/analyzers/RCS0023) ([PR](https://github.com/dotnet/roslynator/pull/1352))
- Fix analyzer [RCS1014](https://josefpihrt.github.io/docs/roslynator/analyzers/RCS1014) ([PR](https://github.com/dotnet/roslynator/pull/1350))

## [4.8.0] - 2024-01-02

### Added

- Add analyzer "Add/remove blank line between switch sections" ([RCS0061](https://josefpihrt.github.io/docs/roslynator/analyzers/RCS0061)) ([PR](https://github.com/dotnet/roslynator/pull/1302))
  - Option (required): `roslynator_blank_line_between_switch_sections = include|omit|omit_after_block`
  - Make analyzer [RCS0014](https://josefpihrt.github.io/docs/roslynator/analyzers/RCS0014) obsolete
- Add analyzer "Declare explicit/implicit type" ([RCS1264](https://josefpihrt.github.io/docs/roslynator/analyzers/RCS1264)) ([PR](https://github.com/dotnet/roslynator/pull/1335))
  - Required option: `roslynator_use_var = always | never | when_type_is_obvious`
  - This analyzer consolidates following analyzers (which are made obsolete):
    - [RCS1008](https://josefpihrt.github.io/docs/roslynator/analyzers/RCS1008)
    - [RCS1009](https://josefpihrt.github.io/docs/roslynator/analyzers/RCS1009)
    - [RCS1010](https://josefpihrt.github.io/docs/roslynator/analyzers/RCS1010)
    - [RCS1012](https://josefpihrt.github.io/docs/roslynator/analyzers/RCS1012)
    - [RCS1176](https://josefpihrt.github.io/docs/roslynator/analyzers/RCS1176)
    - [RCS1177](https://josefpihrt.github.io/docs/roslynator/analyzers/RCS1177)
- Add code fix "Declare as nullable" ([PR](https://github.com/dotnet/roslynator/pull/1333))
  - Applicable to: `CS8600`, `CS8610`, `CS8765` and `CS8767`
- Add option `roslynator_use_collection_expression = true|false` ([PR](https://github.com/dotnet/roslynator/pull/1325))
  - Applicable to [RCS1014](https://josefpihrt.github.io/docs/roslynator/analyzers/RCS1014) and [RCS1250](https://josefpihrt.github.io/docs/roslynator/analyzers/RCS1250)

### Changed

- Replace type declaration's empty braces with semicolon ([RCS1251](https://josefpihrt.github.io/docs/roslynator/analyzers/RCS1251) ([PR](https://github.com/dotnet/roslynator/pull/1323), [PR](https://github.com/dotnet/roslynator/pull/1327))
- [TestFramework] Bump `MSTest.TestFramework` to `3.1.1` ([PR](https://github.com/dotnet/roslynator/pull/1332))
- [TestFramework] Bump `xunit.assert` to `2.6.2` ([PR](https://github.com/dotnet/roslynator/pull/1332))
- Bump Roslyn to 4.7.0 ([PR](https://github.com/dotnet/roslynator/pull/1325))

### Fixed

- Fix analyzer [RCS1262](https://josefpihrt.github.io/docs/roslynator/analyzers/RCS1262) ([PR](https://github.com/dotnet/roslynator/pull/1339))
- Fix analyzer [RCS1213](https://josefpihrt.github.io/docs/roslynator/analyzers/RCS1213) ([PR](https://github.com/dotnet/roslynator/pull/1343))

## [4.7.0] - 2023-12-03

### Added

- Add analyzer "Dispose resource asynchronously" ([RCS1261](https://josefpihrt.github.io/docs/roslynator/analyzers/RCS1261)) ([PR](https://github.com/dotnet/roslynator/pull/1285))
- Add analyzer "Unnecessary raw string literal" ([RCS1262](https://josefpihrt.github.io/docs/roslynator/analyzers/RCS1262)) ([PR](https://github.com/dotnet/roslynator/pull/1293))
- Add analyzer "Invalid reference in a documentation comment" ([RCS1263](https://josefpihrt.github.io/docs/roslynator/analyzers/RCS1263)) ([PR](https://github.com/dotnet/roslynator/pull/1295))

### Changed

- Improve refactoring "Remove comment" [RR0098](https://josefpihrt.github.io/docs/roslynator/refactorings/RR0098) ([PR](https://github.com/dotnet/roslynator/pull/1284))
- Remove trailing dot from analyzer's title ([PR](https://github.com/dotnet/roslynator/pull/1298))
- Group code fix "Change accessibility to ..." ([PR](https://github.com/dotnet/roslynator/pull/1305))
- [CLI] Bump Roslyn to 4.8.0 ([PR](https://github.com/dotnet/roslynator/pull/1307)).
- Group refactoring "Remove members above/below" ([PR](https://github.com/dotnet/roslynator/pull/1308))
- Rename analyzers ([PR](https://github.com/dotnet/roslynator/pull/1314))
  - "Add new line before embedded statement" -> "Put embedded statement on its own line" ([RCS0030](https://josefpihrt.github.io/docs/roslynator/analyzers/RCS0030))
  - "Add new line before statement" -> "Put statement on its own line" ([RCS0033](https://josefpihrt.github.io/docs/roslynator/analyzers/RCS0033))
- Group refactoring "Wrap in ..." ([PR](https://github.com/dotnet/roslynator/pull/1317))

### Fixed

- Fix analyzer [RCS1124](https://josefpihrt.github.io/docs/roslynator/analyzers/RCS1124) ([PR](https://github.com/dotnet/roslynator/pull/1279))
- Fix analyzer [RCS0058](https://josefpihrt.github.io/docs/roslynator/analyzers/RCS0058) ([PR](https://github.com/dotnet/roslynator/pull/1281))
- Fix analyzer [RCS1163](https://josefpihrt.github.io/docs/roslynator/analyzers/RCS1163) ([PR](https://github.com/dotnet/roslynator/pull/1280))
- Fix analyzer [RCS1203](https://josefpihrt.github.io/docs/roslynator/analyzers/RCS1203) ([PR](https://github.com/dotnet/roslynator/pull/1282))
- Fix analyzer [RCS1046](https://josefpihrt.github.io/docs/roslynator/analyzers/RCS1046) ([PR](https://github.com/dotnet/roslynator/pull/1283))
- Fix analyzer [RCS1158](https://josefpihrt.github.io/docs/roslynator/analyzers/RCS1158) ([PR](https://github.com/dotnet/roslynator/pull/1288))
- Fix analyzer [RCS1032](https://josefpihrt.github.io/docs/roslynator/analyzers/RCS1032) ([PR](https://github.com/dotnet/roslynator/pull/1289))
- Fix analyzer [RCS1176](https://josefpihrt.github.io/docs/roslynator/analyzers/RCS1176) ([PR](https://github.com/dotnet/roslynator/pull/1291))
- Fix analyzer [RCS1197](https://josefpihrt.github.io/docs/roslynator/analyzers/RCS1197) ([PR](https://github.com/dotnet/roslynator/pull/1166))
- Fix analyzer [RCS1093](https://josefpihrt.github.io/docs/roslynator/analyzers/RCS1093) ([PR](https://github.com/dotnet/roslynator/pull/1296))
- [Testing] Detect nested code actions ([PR](https://github.com/dotnet/roslynator/pull/1305))

## [4.6.4] - 2023-11-24

## [4.6.3] - 2023-11-23

### Added

- [CLI] Add command `find-symbol` ([PR](https://github.com/dotnet/roslynator/pull/1255))
  - This command can be used not only to find symbols but also to find unused symbols and optionally remove them.
  - Example: `roslynator find-symbol --symbol-kind type --visibility internal private --unused --remove`

### Changed

- Bump Roslyn to 4.6.0 ([PR](https://github.com/dotnet/roslynator/pull/1248)).
- [CLI] Add support for .NET 8 ([PR](https://github.com/josefpihrt/roslynator/pull/1251)).

### Fixed

- Fix analyzer [RCS1228](https://josefpihrt.github.io/docs/roslynator/analyzers/RCS1228) ([PR](https://github.com/dotnet/roslynator/pull/1249))
- Fix analyzer [RCS1213](https://josefpihrt.github.io/docs/roslynator/analyzers/RCS1213) ([PR](https://github.com/dotnet/roslynator/pull/1254))
- Fix analyzer [RCS1055](https://josefpihrt.github.io/docs/roslynator/analyzers/RCS1055) ([PR](https://github.com/dotnet/roslynator/pull/1253))
- Fix analyzer [RCS1196](https://josefpihrt.github.io/docs/roslynator/analyzers/RCS1196) ([PR](https://github.com/dotnet/roslynator/pull/1235))
- Fix analyzer [RCS1257](https://josefpihrt.github.io/docs/roslynator/analyzers/RCS1257) ([PR](https://github.com/dotnet/roslynator/pull/1264))
- Fix analyzer [RCS1259](https://josefpihrt.github.io/docs/roslynator/analyzers/RCS1259) ([PR](https://github.com/dotnet/roslynator/pull/1268))
- [CLI] Fix reading of non-existent redirected input on git bash ([PR](https://github.com/dotnet/roslynator/pull/1265), [PR](https://github.com/dotnet/roslynator/pull/1274), [PR](https://github.com/dotnet/roslynator/pull/1275))
- [CLI] Fix exit code for `roslynator --version` ([PR](https://github.com/dotnet/roslynator/pull/1273))

## [4.6.2] - 2023-11-10

### Added

- [CLI] Add note to docs that Roslynator CLI does not contain any analyzers itself ([PR](https://github.com/dotnet/roslynator/pull/1241))

### Fixed

- Fix [RCS1234](https://josefpihrt.github.io/docs/roslynator/analyzers/RCS1234) ([PR](https://github.com/dotnet/roslynator/pull/1233))
- Fix refactoring [Inline method](https://josefpihrt.github.io/docs/roslynator/refactorings/RR0062) ([PR](https://github.com/dotnet/roslynator/pull/1234))
- [CLI] Fix globbing ([PR](https://github.com/dotnet/roslynator/pull/1238))
- [CLI] Remove assembly resolving ([PR](https://github.com/dotnet/roslynator/pull/1237))

## [4.6.1] - 2023-10-23

### Fixed

- Fix [RCS1197](https://josefpihrt.github.io/docs/roslynator/analyzers/RCS1197) ([PR](https://github.com/dotnet/roslynator/pull/1226)).

## [4.6.0] - 2023-10-18

### Added

- Add social card ([PR](https://github.com/dotnet/roslynator/pull/1212)).
- Add nullable annotation to public API ([PR](https://github.com/dotnet/roslynator/pull/1198)).
- Add refactoring "Remove directive (including content)" ([PR](https://github.com/dotnet/roslynator/pull/1224)).

### Changed

- Update logo ([PR](https://github.com/dotnet/roslynator/pull/1208), [PR](https://github.com/dotnet/roslynator/pull/1210)).
- Migrate to .NET Foundation ([PR](https://github.com/dotnet/roslynator/pull/1206), [PR](https://github.com/dotnet/roslynator/pull/1207), [PR](https://github.com/dotnet/roslynator/pull/1219)).
- Bump Roslyn to 4.7.0 ([PR](https://github.com/dotnet/roslynator/pull/1218)).
  - Applies to CLI and testing library. 
- Bump Microsoft.Build.Locator to 1.6.1 ([PR](https://github.com/dotnet/roslynator/pull/1194))
- Improve testing framework ([PR](https://github.com/dotnet/roslynator/pull/1214))
  - Add methods to `DiagnosticVerifier`, `RefactoringVerifier` and `CompilerDiagnosticFixVerifier`.
  - Add property `DiagnosticVerifier.Descriptor` (BREAKING CHANGE).
  - Add property `CompilerDiagnosticFixVerifier.DiagnosticId` (BREAKING CHANGE).
  - Make property `DiagnosticTestData.Descriptor` obsolete.
  - Make property `CompilerDiagnosticFixTestData.DiagnosticId` obsolete.

### Fixed

- Fix [RCS1164](https://josefpihrt.github.io/docs/roslynator/analyzers/RCS1164) ([PR](https://github.com/dotnet/roslynator/pull/1196)).
- Fix [RCS1241](https://josefpihrt.github.io/docs/roslynator/analyzers/RCS1241) ([PR](https://github.com/dotnet/roslynator/pull/1197)).
- Fix [RCS1250](https://josefpihrt.github.io/docs/roslynator/analyzers/RCS1250) ([PR](https://github.com/dotnet/roslynator/pull/1205)).
- [CLI] Fix globbing ([PR](https://github.com/dotnet/roslynator/pull/1215)).
- [CLI] Fix generation of root file ([PR](https://github.com/dotnet/roslynator/pull/1221)).

## [4.5.0] - 2023-08-27

### Added

- Add SECURITY.md ([PR](https://github.com/dotnet/roslynator/pull/1147))
- Add custom FixAllProvider for [RCS1014](https://josefpihrt.github.io/docs/roslynator/analyzers/RCS1014) ([PR](https://github.com/dotnet/roslynator/pull/1070)).
- Add more cases to [RCS1097](https://josefpihrt.github.io/docs/roslynator/analyzers/RCS1097) ([PR](https://github.com/dotnet/roslynator/pull/1160)).
- Add analyzer "Use enum field explicitly" ([RCS1257](https://josefpihrt.github.io/docs/roslynator/analyzers/RCS1257)) ([PR](https://github.com/dotnet/roslynator/pull/889)).
  - Enabled by default.
- Add analyzer "Unnecessary enum flag" [RCS1258](https://josefpihrt.github.io/docs/roslynator/analyzers/RCS1258) ([PR](https://github.com/dotnet/roslynator/pull/886)).
  - Enabled by default.
- Make `Roslynator.Rename.SymbolRenamer` public ([PR](https://github.com/dotnet/roslynator/pull/1161))
- Analyzer 'Remove empty syntax' ([RCS1259](https://josefpihrt.github.io/docs/roslynator/analyzers/RCS1259)) ([PR](https://github.com/dotnet/roslynator/pull/913)).
  - This analyzer replaces following analyzers:
    - Remove empty statement ([RCS1038](https://josefpihrt.github.io/docs/roslynator/analyzers/RCS1038))
    - Remove empty 'else' clause ([RCS1040](https://josefpihrt.github.io/docs/roslynator/analyzers/RCS1040))
    - Remove empty object initializer ([RCS1041](https://josefpihrt.github.io/docs/roslynator/analyzers/RCS1041))
    - Remove empty 'finally' clause ([RCS1066](https://josefpihrt.github.io/docs/roslynator/analyzers/RCS1066))
    - Remove empty namespace declaration ([RCS1072](https://josefpihrt.github.io/docs/roslynator/analyzers/RCS1072))
    - Remove empty region directive ([RCS1091](https://josefpihrt.github.io/docs/roslynator/analyzers/RCS1091))
    - Remove empty destructor ([RCS1106](https://josefpihrt.github.io/docs/roslynator/analyzers/RCS1106))
- [CLI] Add glob pattern matching (`--include` or/and `--exclude`) ([PR](https://github.com/dotnet/roslynator/pull/1178), [PR](https://github.com/dotnet/roslynator/pull/1183)).
- Add analyzer "Include/omit trailing comma" ([RCS1256](https://github.com/dotnet/roslynator/blob/main/docs/analyzers/RCS1256.md)) ([PR](https://github.com/dotnet/roslynator/pull/931)).
  - Required option: `roslynator_trailing_comma_style = include|omit|omit_when_single_line`
  - Not enabled by default

### Changed

- [CLI] Open help in web browser when running command `roslynator help <COMMAND>` ([PR](https://github.com/dotnet/roslynator/pull/1179))

### Fixed

- Fix [RCS1187](https://josefpihrt.github.io/docs/roslynator/analyzers/RCS1187) ([PR](https://github.com/dotnet/roslynator/pull/1150)).
- Fix [RCS1056](https://josefpihrt.github.io/docs/roslynator/analyzers/RCS1056) ([PR](https://github.com/dotnet/roslynator/pull/1154)).
- Fix [RCS1208](https://josefpihrt.github.io/docs/roslynator/analyzers/RCS1208) ([PR](https://github.com/dotnet/roslynator/pull/1153)).
- Fix [RCS1043](https://josefpihrt.github.io/docs/roslynator/analyzers/RCS1043) ([PR](https://github.com/dotnet/roslynator/pull/1176)).
- [CLI] Fix exit code of `spellcheck` command ([PR](https://github.com/dotnet/roslynator/pull/1177)).
- Improve indentation analysis ([PR](https://github.com/dotnet/roslynator/pull/1188)).

## [4.4.0] - 2023-08-01

### Added

- Add GitHub workflow ([#1112](https://github.com/josefpihrt/roslynator/pull/1112))

### Changed

- [CLI] Bump Roslyn to 4.6.0 ([#1106](https://github.com/josefpihrt/roslynator/pull/1106)).
- Bump Roslyn to 4.4.0 ([#1116](https://github.com/josefpihrt/roslynator/pull/1116)).
- Migrate documentation to [Docusaurus](https://josefpihrt.github.io/docs/roslynator) ([#922](https://github.com/josefpihrt/roslynator/pull/922)).
- [Testing Framework] Bump Roslyn to 4.6.0 ([#1144](https://github.com/josefpihrt/roslynator/pull/1144)).

### Fixed

- Fix [RCS1016](https://josefpihrt.github.io/docs/roslynator/analyzers/RCS1016) ([#1090](https://github.com/josefpihrt/roslynator/pull/1090)).
- Improve inversion of logical expressions to handling additional cases ([#1086](https://github.com/josefpihrt/roslynator/pull/1086)).
- Fix [RCS1084](https://josefpihrt.github.io/docs/roslynator/analyzers/RCS1084) ([#1085](https://github.com/josefpihrt/roslynator/pull/1085)).
- Fix [RCS1169](https://josefpihrt.github.io/docs/roslynator/analyzers/RCS1169) ([#1092](https://github.com/JosefPihrt/Roslynator/pull/1092)).
- Recognize more shapes of IAsyncEnumerable as being Async ([RCS1047](https://josefpihrt.github.io/docs/roslynator/analyzers/RCS1047)) ([#1084](https://github.com/josefpihrt/roslynator/pull/1084)).
- Fix [RCS1197](https://josefpihrt.github.io/docs/roslynator/analyzers/RCS1197) ([#1093](https://github.com/JosefPihrt/Roslynator/pull/1093)).
- Fix [RCS1056](https://josefpihrt.github.io/docs/roslynator/analyzers/RCS1056) ([#1096](https://github.com/JosefPihrt/Roslynator/pull/1096)).
- Fix [RCS1216](https://josefpihrt.github.io/docs/roslynator/analyzers/RCS1216) ([#1094](https://github.com/JosefPihrt/Roslynator/pull/1094)).
- Fix [RCS1146](https://josefpihrt.github.io/docs/roslynator/analyzers/RCS1146) ([#1098](https://github.com/JosefPihrt/Roslynator/pull/1098)).
- Fix [RCS1154](https://josefpihrt.github.io/docs/roslynator/analyzers/RCS1154) ([#1105](https://github.com/JosefPihrt/Roslynator/pull/1105)).
- Fix [RCS1211](https://josefpihrt.github.io/docs/roslynator/analyzers/RCS1211) ([#1095](https://github.com/JosefPihrt/Roslynator/pull/1095)).
- Fix [RCS0005](https://josefpihrt.github.io/docs/roslynator/analyzers/RCS0005) ([#1114](https://github.com/JosefPihrt/Roslynator/pull/1114)).
- Fix [RCS1176](https://josefpihrt.github.io/docs/roslynator/analyzers/RCS1176) ([#1122](https://github.com/JosefPihrt/Roslynator/pull/1122), [#1140](https://github.com/JosefPihrt/Roslynator/pull/1140)).
- Fix [RCS1085](https://josefpihrt.github.io/docs/roslynator/analyzers/RCS1085) ([#1120](https://github.com/josefpihrt/roslynator/pull/1120)).
- Fix [RCS1208](https://josefpihrt.github.io/docs/roslynator/analyzers/RCS1208) ([#1119](https://github.com/JosefPihrt/Roslynator/pull/1119)).
- [CLI] Fix member full declaration in generated documentation (command `generate-doc`) ([#1130](https://github.com/josefpihrt/roslynator/pull/1130)).
  - Append `?` to nullable reference types.
- Fix [RCS1179](https://josefpihrt.github.io/docs/roslynator/analyzers/RCS1179) ([#1129](https://github.com/JosefPihrt/Roslynator/pull/1129)).
- Fix [RCS0060](https://josefpihrt.github.io/docs/roslynator/analyzers/RCS0060) ([#1139](https://github.com/JosefPihrt/Roslynator/pull/1139)).

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
- [CLI] Add support for .NET 7 and remove support for .NET 5 ([#985](https://github.com/josefpihrt/roslynator/pull/985)).

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
- Do not simplify default expression if it would change semantics ([RCS1244](https://josefpihrt.github.io/docs/roslynator/analyzers/RCS1244)) ([#939](https://github.com/josefpihrt/roslynator/pull/939)).
- Fix NullReferenceException in [RCS1198](https://josefpihrt.github.io/docs/roslynator/analyzers/RCS1198) ([#940](https://github.com/josefpihrt/roslynator/pull/940).
- Order named arguments even if optional arguments are not specified [RCS1205](https://josefpihrt.github.io/docs/roslynator/analyzers/RCS1205) ([#941](https://github.com/josefpihrt/roslynator/pull/941)), ([#965](https://github.com/josefpihrt/roslynator/pull/965)).
- Prefix identifier with `@` if necessary ([RCS1220](https://josefpihrt.github.io/docs/roslynator/analyzers/RCS1220)) ([#943](https://github.com/josefpihrt/roslynator/pull/943)).
- Do not suggest to make local variable a const when it is used in ref extension method ([RCS1118](https://josefpihrt.github.io/docs/roslynator/analyzers/RCS1118)) ([#948](https://github.com/josefpihrt/roslynator/pull/948)).
- Fix formatting of argument list ([#952](https://github.com/josefpihrt/roslynator/pull/952)).
- Do not remove async/await when 'using declaration' is used ([#953](https://github.com/josefpihrt/roslynator/pull/953)).
- Convert if-else to return statement when pattern matching is used ([RCS1073](https://josefpihrt.github.io/docs/roslynator/analyzers/RCS1073)) ([#956](https://github.com/josefpihrt/roslynator/pull/956)).
- [CLI] Include compiler diagnostics in the xml output file of the `roslynator analyze` command ([#964](https://github.com/JosefPihrt/Roslynator/pull/964) by @PeterKaszab).
- Do not simplify 'default' expression if the type is inferred ([RCS1244](https://josefpihrt.github.io/docs/roslynator/analyzers/RCS1244)) ([#966](https://github.com/josefpihrt/roslynator/pull/966)).
- Use explicit type from lambda expression ([RCS1008](https://josefpihrt.github.io/docs/roslynator/analyzers/RCS1008)) ([#967](https://github.com/josefpihrt/roslynator/pull/967).
- Do not remove constructor if it is decorated with 'UsedImplicitlyAttribute' ([RCS1074](https://josefpihrt.github.io/docs/roslynator/analyzers/RCS1074)) ([#968](https://github.com/josefpihrt/roslynator/pull/968)).
- Detect argument null check in the form of `ArgumentNullException.ThrowIfNull` ([RR0025](https://josefpihrt.github.io/docs/roslynator/refactorings/RR0025), [RCS1227](https://josefpihrt.github.io/docs/roslynator/analyzers/RCS1227)) ([#974](https://github.com/josefpihrt/roslynator/pull/974)).
- Do not make generic class static if it's inherited ([RCS1102](https://josefpihrt.github.io/docs/roslynator/analyzers/RCS1102)) ([#978](https://github.com/josefpihrt/roslynator/pull/978)).

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
* Analyzers that require option to be set should be disabled by default (RCS1018, RCS1096, RCS1250) ([commit](https://github.com/JosefPihrt/Roslynator/commit/de374858f9d8120a6f6d705ad685101ed1bab699))

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
* Add analyzer option [RCS1078i](https://josefpihrt.github.io/docs/roslynator/analyzers/RCS1078i) (Use `string.Empty` instead of `""`)
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
