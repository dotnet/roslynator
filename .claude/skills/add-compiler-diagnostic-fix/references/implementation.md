# Compiler Diagnostic Fix Implementation

Contributor how-to: https://josefpihrt.github.io/docs/roslynator/how-to-add-compiler-diagnostic-fix

## Diagnostics.xml

```xml
<Diagnostic
  Id="CS0163"
  Identifier="ControlCannotFallThroughFromOneCaseLabelToAnother"
  Severity="Error"
  Title="Control cannot fall through from one case label to another"
  Message="Control cannot fall through from one case label to another"
  HelpUrl="https://learn.microsoft.com/dotnet/csharp/misc/cs0163" />
```

## CodeFixes.xml

```xml
<CodeFix Id="RCF0002" Identifier="AddBreakStatementToSwitchSection" Title="Add break statement to switch section">
  <FixableDiagnosticIds>
    <Id>CS0163</Id>
    <Id>CS8070</Id>
  </FixableDiagnosticIds>
</CodeFix>
```

One `RCF` can fix multiple `CS` ids; one `CS` can have multiple `RCF` fixes.

## Provider (`CompilerDiagnosticCodeFixProvider`)

`src/CodeFixes/CSharp/CodeFixes/`:

```csharp
[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(AddBreakStatementToSwitchSectionCodeFixProvider))]
[Shared]
public sealed class AddBreakStatementToSwitchSectionCodeFixProvider : CompilerDiagnosticCodeFixProvider
{
    public override ImmutableArray<string> FixableDiagnosticIds =>
        ImmutableArray.Create(
            CompilerDiagnosticIdentifiers.CS0163_ControlCannotFallThroughFromOneCaseLabelToAnother,
            CompilerDiagnosticIdentifiers.CS8070_ControlCannotFallOutOfSwitchFromFinalCaseLabel);

    public override async Task RegisterCodeFixesAsync(CodeFixContext context)
    {
        Diagnostic diagnostic = context.Diagnostics[0];
        SyntaxNode root = await context.GetSyntaxRootAsync().ConfigureAwait(false);

        if (!IsEnabled(diagnostic.Id, CodeFixIdentifiers.AddBreakStatementToSwitchSection, context.Document, root.SyntaxTree))
            return;

        if (!TryFindFirstAncestorOrSelf(root, context.Span, out SwitchSectionSyntax switchSection))
            return;

        CodeAction codeAction = CodeAction.Create(
            "Add 'break' statement",
            ct => RefactorAsync(context.Document, switchSection, ct),
            GetEquivalenceKey(diagnostic));

        context.RegisterCodeFix(codeAction, diagnostic);
    }
}
```

`IsEnabled` checks `roslynator_compiler_diagnostic_fix.CS####.enabled` and global compiler-fix settings.

Reference: `AddBreakStatementToSwitchSectionCodeFixProvider.cs`, `CompilerDiagnosticCodeFixProvider.cs`.

## Test (`AbstractCSharpCompilerDiagnosticFixVerifier`)

Not public `XunitCompilerDiagnosticFixVerifier`. No `[|...|]` markers — compiler location is implicit.

```csharp
public class CS0165UseOfUnassignedLocalVariableTests
    : AbstractCSharpCompilerDiagnosticFixVerifier<IdentifierNameCodeFixProvider>
{
    public override string DiagnosticId { get; } =
        CompilerDiagnosticIdentifiers.CS0165_UseOfUnassignedLocalVariable;

    [Fact, Trait(Traits.CodeFix, CompilerDiagnosticIdentifiers.CS0165_UseOfUnassignedLocalVariable)]
    public async Task Test()
    {
        await VerifyFixAsync(@"
class C
{
    void M()
    {
        TimeSpan ts;
        if (ts == default) { }
    }
}
", @"
class C
{
    void M()
    {
        TimeSpan ts = default;
        if (ts == default) { }
    }
}
", equivalenceKey: EquivalenceKey.Create(DiagnosticId));
    }
}
```

File: `CS####IdentifierTests.cs` in `Tests/CodeFixes.Tests/`.
