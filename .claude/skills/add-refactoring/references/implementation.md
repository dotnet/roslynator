# Refactoring Implementation

## Registration

Find site in `src/Refactorings/CSharp/Refactorings/RefactoringContext.cs` or a helper (e.g. `AddOrRemoveArgumentNameRefactoring.cs`):

```csharp
if (context.IsRefactoringEnabled(RefactoringDescriptors.AddArgumentName))
    AddArgumentNameRefactoring.ComputeRefactoring(context, argumentList, selection, semanticModel);
```

`<OptionKey>` in XML is required — becomes `roslynator_refactoring.<key>.enabled`.

## Test (`AbstractCSharpRefactoringVerifier`)

Not public `XunitRefactoringVerifier`. File: `RR####IdentifierTests.cs`.

```csharp
public class RR0011AddArgumentNameTests : AbstractCSharpRefactoringVerifier
{
    public override string RefactoringId { get; } = RefactoringIdentifiers.AddArgumentName;

    [Fact, Trait(Traits.Refactoring, RefactoringIdentifiers.AddArgumentName)]
    public async Task Test_MultilineArgumentListInArrayInitializer()
    {
        await VerifyRefactoringAsync(@"
class C
{
    void M()
    {
        var arr = new[]
        {
            new string(
[|                ' ',
                1|])
        };
    }
}
", @"
class C
{
    void M()
    {
        var arr = new[]
        {
            new string(
                c: ' ',
                count: 1)
        };
    }
}
", equivalenceKey: EquivalenceKey.Create(RefactoringId));
    }
}
```

`[|` / `|]` mark **selection** (not diagnostic span). Config: `options: Options.AddConfigOption("key", "value")`.

References: `RR0011AddArgumentNameTests.cs`, `AddOrRemoveArgumentNameRefactoring.cs`.

## Metadata Example

```xml
<Refactoring Id="RR0011" Identifier="AddArgumentName" Title="Add argument name">
  <OptionKey>add_argument_name</OptionKey>
  <Syntaxes><Syntax>argument</Syntax></Syntaxes>
  <Span>argument list</Span>
  <Samples>
    <Sample>
      <Before><![CDATA[new string(' ', 1)]]></Before>
      <After><![CDATA[new string(c: ' ', count: 1)]]></After>
    </Sample>
  </Samples>
</Refactoring>
```

Schema: https://josefpihrt.github.io/docs/roslynator/refactoring-metadata
