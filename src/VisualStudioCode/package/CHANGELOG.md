## 3.0.0 (2020-06-16)

* Update references to Roslyn API to 3.5.0
* Introduce concept of "[Analyzer Options](https://github.com/JosefPihrt/Roslynator/blob/master/docs/AnalyzerOptions.md)"
* Reassign ID for some analyzers.
  * See "[How to: Migrate Analyzers to Version 3.0](https://github.com/JosefPihrt/Roslynator/blob/master/docs/HowToMigrateAnalyzersToVersion3.md)"
* Remove references to Roslynator assemblies from omnisharp.json on uninstall

## 2.9.0 (2020-03-13)

* Switch to Roslyn 3.x libraries
* Add `Directory.Build.props` file
* Add open configuration commands to Command Palette (VS Code) ([PR](https://github.com/JosefPihrt/Roslynator/pull/648))

### Bug Fixes

* Fix key duplication/handle camel case names in `omnisharp.json` ([PR](https://github.com/JosefPihrt/Roslynator/pull/645))
* Use prefix unary operator instead of postfix unary operator ([RCS1089](https://github.com/JosefPihrt/Roslynator/blob/master/docs/analyzers/RCS1089.md)) ([issue](https://github.com/JosefPihrt/Roslynator/issues/639))
* Cast of `this` to its interface cannot be null ([RCS1202](https://github.com/JosefPihrt/Roslynator/blob/master/docs/analyzers/RCS1202.md)) ([issue](https://github.com/JosefPihrt/Roslynator/issues/640))
* Do not remove braces in switch section if it contains 'using variable' ([RCS1031](https://github.com/JosefPihrt/Roslynator/blob/master/docs/analyzers/RCS1031.md)) ([issue](https://github.com/JosefPihrt/Roslynator/issues/632))

### New Analyzers

* [RCS1242](https://github.com/JosefPihrt/Roslynator/blob/master/docs/analyzers/RCS1242.md) (DoNotPassNonReadOnlyStructByReadOnlyReference).
* [RCS1243](https://github.com/JosefPihrt/Roslynator/blob/master/docs/analyzers/RCS1243.md) (DuplicateWordInComment).
* [RCS1244](https://github.com/JosefPihrt/Roslynator/blob/master/docs/analyzers/RCS1244.md) (SimplifyDefaultExpression).
* [RCS1245](https://github.com/JosefPihrt/Roslynator/blob/master/docs/analyzers/RCS1245.md) (SimplifyConditionalExpression2) ([issue](https://github.com/JosefPihrt/Roslynator/issues/612)).

### Analyzers

* Disable analyzer [RCS1057](https://github.com/JosefPihrt/Roslynator/blob/master/docs/analyzers/RCS1057.md) by default ([issue](https://github.com/JosefPihrt/Roslynator/issues/590)).
* Merge analyzer [RCS1156](https://github.com/JosefPihrt/Roslynator/blob/master/docs/analyzers/RCS1156.md) with [RCS1113](https://github.com/JosefPihrt/Roslynator/blob/master/docs/analyzers/RCS1113.md) ([issue](https://github.com/JosefPihrt/Roslynator/issues/650)).
  * `x == ""` should be replaced with `string.IsNullOrEmpty(x)`
* Improve analyzer [RCS1215](https://github.com/JosefPihrt/Roslynator/blob/master/docs/analyzers/RCS1215.md) ([commit](https://github.com/JosefPihrt/Roslynator/commit/0fdd97f9a62463f8b004abeb17a8b8509374c35a)).
  * `x == double.NaN` should be replaced with `double.IsNaN(x)`
* Enable [RCS1169](https://github.com/JosefPihrt/Roslynator/blob/master/docs/analyzers/RCS1169.md) and [RCS1170](https://github.com/JosefPihrt/Roslynator/blob/master/docs/analyzers/RCS1170.md) if the type is read-only struct ([commit](https://github.com/JosefPihrt/Roslynator/commit/f34e105433dbc65686369adf712b0b99d93eaef7)).
* Improve analyzer [RCS1077](https://github.com/JosefPihrt/Roslynator/blob/master/docs/analyzers/RCS1077.md) ([commit](https://github.com/JosefPihrt/Roslynator/commit/3ee275442cb16f6a9104b42d582ba7d76d6df88c)).
  * `x.OrderBy(y => y).Reverse()` can be simplified to `x.OrderByDescending(y => y)`
  * `x.SelectMany(y => y).Count()` can be simplified to `x.Sum(y => y.Count)` if `x` has `Count` or `Length` property
* Improve analyzer [RCS1161](https://github.com/JosefPihrt/Roslynator/blob/master/docs/analyzers/RCS1161.md) - Declare explicit enum value using `<<` operator ([commit](https://github.com/JosefPihrt/Roslynator/commit/6b78496efe1a2f2678f2ef2a71986e2bee006863)).
* Improve analyzer [RCS1036](https://github.com/JosefPihrt/Roslynator/blob/master/docs/analyzers/RCS1036.md) - remove empty line between documentation comment and declaration ([commit](https://github.com/JosefPihrt/Roslynator/commit/de0f1205671281679866e92edd9337a7416409e6)).
* Improve analyzer [RCS1037](https://github.com/JosefPihrt/Roslynator/blob/master/docs/analyzers/RCS1037.md) - remove trailing white-space from documentation comment ([commit](https://github.com/JosefPihrt/Roslynator/commit/c3f7d193ee37d04de7e2c698aab7f3e1e6350e80)).
* Improve analyzer [RCS1143](https://github.com/JosefPihrt/Roslynator/blob/master/docs/analyzers/RCS1143.md) ([commit](https://github.com/JosefPihrt/Roslynator/commit/4c4281ebdf8eb0aa1a77d5e5bfda71bc66cce1df))
  * `x?.M() ?? default(int?)` can be simplified to `x?.M()` if `x` is a nullable struct.
* Improve analyzer [RCS1206](https://github.com/JosefPihrt/Roslynator/blob/master/docs/analyzers/RCS1206.md) ([commit](https://github.com/JosefPihrt/Roslynator/commit/88dd4cea4df07f036a8296511410ccff70f8fefe))
  * `(x != null) ? x.M() : default(int?)` can be simplified to `x?.M()` if `x` is a nullable struct.

## 2.3.0 (2019-12-28)

* Automatically update configuration in omnisharp.json (VS Code) ([PR](https://github.com/JosefPihrt/Roslynator/pull/623)).

## 2.2.0 (2019-09-28)

* Enable configuration for non-Windows systems (VS Code).

### Analyzers

* Disable analyzer [RCS1029](https://github.com/JosefPihrt/Roslynator/blob/master/docs/analyzers/RCS1029.md) (FormatBinaryOperatorOnNextLine) by default.

## 2.1.4 (2019-08-13)

* Initial release of Roslynator for VS Code.
