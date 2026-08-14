# Analyzer Metadata Schema

Source file: `src/Analyzers.xml`

Authoritative reference: https://josefpihrt.github.io/docs/roslynator/analyzer-metadata

## Required Elements

| Element | Description |
|---------|-------------|
| `Id` | `RCS` + number. Prefix: `RCS0` formatting, `RCS1` general, `RCS9` code-analysis |
| `Identifier` | PascalCase name used in generated code (`DiagnosticRules.X`, class names) |
| `Title` | Short description |
| `DefaultSeverity` | `Hidden`, `Info`, `Warning`, or `Error` |
| `Summary` | Longer description (markdown) for generated docs |

## Common Optional Elements

| Element | Description |
|---------|-------------|
| `MessageFormat` | Required when the message has parameters; otherwise same as Title |
| `IsEnabledByDefault` | Default `true`. Set `false` for opt-in analyzers (common for RCS0) |
| `MinLanguageVersion` | e.g. `9.0` |
| `SupportsFadeOut` | Fade out reported syntax in the editor |
| `SupportsFadeOutAnalyzer` | Generates an additional `RCS....FadeOut` analyzer |
| `ConfigOptions` | Links analyzer to EditorConfig keys (see config-options.md) |
| `Samples` | Before/After code for documentation |
| `Remarks` | Additional markdown for the docs page |
| `Links` | Related URLs |

Do **not** use `<Category>` — it is ignored; all analyzers use `DiagnosticCategories.Roslynator`.

Do **not** set `<Status>` on new analyzers. Lifecycle states are handled by the deprecation skill.

## Example Entry

```xml
<Analyzer Identifier="UseImplicitOrExplicitObjectCreation">
  <Id>RCS1250</Id>
  <Title>Use implicit/explicit object creation.</Title>
  <MessageFormat>Use {0} object creation.</MessageFormat>
  <DefaultSeverity>Info</DefaultSeverity>
  <IsEnabledByDefault>false</IsEnabledByDefault>
  <MinLanguageVersion>9.0</MinLanguageVersion>
  <Samples>
    <Sample>
      <Before><![CDATA[private string _value = new string(' ', 1)]]></Before>
      <After><![CDATA[private string _value = new(' ', 1)]]></After>
    </Sample>
  </Samples>
  <ConfigOptions>
    <Option Key="object_creation_type_style" IsRequired="true" />
  </ConfigOptions>
</Analyzer>
```

## Generated Outputs

After `generate_code.ps1`:

- `src/Common/DiagnosticRules.Generated.cs` — `DiagnosticRules.Identifier`
- `src/Common/DiagnosticIdentifiers.Generated.cs` — `DiagnosticIdentifiers.Identifier` = `"RCS####"`
- For RCS9: `src/Common/CodeAnalysis/CodeAnalysisDiagnostic*.Generated.cs`

## In-Repo Test Pattern

**Not** the public NuGet `XunitDiagnosticVerifier` from `roslynator.testing.csharp.xunit`.

```csharp
using Roslynator.Testing.CSharp;
using Xunit;

public class RCS1007AddBracesTests
    : AbstractCSharpDiagnosticVerifier<AddBracesAnalyzer, AddBracesCodeFixProvider>
{
    public override DiagnosticDescriptor Descriptor { get; } = DiagnosticRules.AddBraces;

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.AddBraces)]
    public async Task Test_If()
    {
        await VerifyDiagnosticAndFixAsync(@"
class C
{
    void M()
    {
        if(true)
            [|M(x, y);|]
    }
}
", @"
class C
{
    void M()
    {
        if(true)
        {
            M(x, y);
        }
    }
}
");
    }
}
```

For analyzers without a code fix, use `EmptyCodeFixProvider` and `VerifyDiagnosticAsync`.

Span markers: `[|` start, `|]` end of the reported diagnostic location.
