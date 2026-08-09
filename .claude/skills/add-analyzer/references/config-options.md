# Configuration Options

## When to Add a New Option

Add a new entry in `src/ConfigOptions.xml` only when introducing a **new** EditorConfig key. If an existing option already covers your analyzer (e.g. `accessor_braces_style`), reference it from the analyzer metadata instead.

## 1. Declare Option in ConfigOptions.xml

```xml
<Option Id="MyNewOption">
  <Description>What this option controls</Description>
  <Values>
    <Value IsDefault="true">value_a</Value>
    <Value>value_b</Value>
  </Values>
</Option>
```

Or for free-form values:

```xml
<Option Id="MyBoolOption">
  <ValuePlaceholder>true|false</ValuePlaceholder>
  <Description>Enable or disable something</Description>
</Option>
```

`Id` becomes the EditorConfig key suffix: `roslynator_my_new_option`.

## 2. Link from Analyzer Metadata

In `src/Analyzers.xml`, inside the `<Analyzer>` element:

```xml
<ConfigOptions>
  <Option Key="my_new_option" IsRequired="true" />
</ConfigOptions>
```

`Key` is the snake_case form of the Option `Id` (e.g. `AccessorBracesStyle` → `accessor_braces_style`), without the `roslynator_` prefix. `IsRequired="true"` means the analyzer only runs when the option is set.

Per-sample overrides in documentation:

```xml
<Samples>
  <Sample>
    <Before>...</Before>
    <After>...</After>
    <ConfigOptions>
      <Option Key="my_new_option" Value="value_a" />
    </ConfigOptions>
  </Sample>
</Samples>
```

## 3. Codegen Outputs

After `cd tools && pwsh ./generate_code.ps1`:

| File | Content |
|------|---------|
| `src/Common/ConfigOptionKeys.Generated.cs` | `ConfigOptionKeys.MyNewOption` = `"roslynator_my_new_option"` |
| `src/Common/ConfigOptionValues.Generated.cs` | `ConfigOptionValues.MyNewOption_ValueA` etc. |
| `src/Common/ConfigOptions.Generated.cs` | Option descriptors |
| `src/VisualStudioCode/package/src/configurationFiles.generated.ts` | VS Code defaults |

## 4. Read Option in Analyzer

Use extension methods on `SyntaxNodeAnalysisContext` or `AnalyzerConfigOptions` from `Roslynator.CSharp` / `Roslynator.Configuration`.

Example from `FormatAccessorBracesAnalyzer`:

```csharp
AccessorBracesStyle style = context.GetAccessorBracesStyle();
if (style == AccessorBracesStyle.None)
    return;
```

Or check effectiveness of a rule tied to a specific option value:

```csharp
if (DiagnosticRules.SomeRule.IsEffective(context))
    ...
```

For new options you may need to add parsing in `src/Common/CSharp/Extensions/CodeStyleExtensions.cs` or similar.

## 5. Test with Config Option

```csharp
await VerifyDiagnosticAndFixAsync(
    before, after,
    options: Options.AddConfigOption(
        ConfigOptionKeys.AccessorBracesStyle,
        ConfigOptionValues.AccessorBracesStyle_MultiLine));
```

Reference: `src/Tests/Formatting.Analyzers.Tests/RCS0020FormatAccessorBracesTests.cs`

## Adding a Value to an Existing Option

If you only need a new allowed value on an existing option, edit `ConfigOptions.xml` (add a `<Value>`), run codegen, and update any parsers/tests. No new option entry required.
