# Analyzer and Code Fix Implementation

Apache license header on every new file. File-scoped namespaces. `sealed` classes.

**Namespace:** match neighboring analyzers in the same package (`Roslynator.CSharp.Analysis` vs `Roslynator.CSharp.CSharp.Analysis` — check recent files in that folder).

## Analyzer (`BaseDiagnosticAnalyzer`)

Location by prefix:

- RCS1 → `src/Analyzers/CSharp/Analysis/`
- RCS0 → `src/Formatting.Analyzers/CSharp/`
- RCS9 → `src/CodeAnalysis.Analyzers/CSharp/`

```csharp
[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class MyAnalyzer : BaseDiagnosticAnalyzer
{
    private static ImmutableArray<DiagnosticDescriptor> _supportedDiagnostics;

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
    {
        get
        {
            if (_supportedDiagnostics.IsDefault)
                Immutable.InterlockedInitialize(ref _supportedDiagnostics, DiagnosticRules.MyRule);
            return _supportedDiagnostics;
        }
    }

    public override void Initialize(AnalysisContext context)
    {
        base.Initialize(context);
        context.RegisterSyntaxNodeAction(f => Analyze(f), SyntaxKind.SomeKind);
    }

    private static void Analyze(SyntaxNodeAnalysisContext context)
    {
        DiagnosticHelpers.ReportDiagnostic(context, DiagnosticRules.MyRule, node);
    }
}
```

Use `DiagnosticHelpers.ReportDiagnostic`, not `context.ReportDiagnostic`.

## Code Fix (`BaseCodeFixProvider`)

Matching `.CodeFixes` project, `CSharp/CodeFixes/`:

```csharp
[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MyCodeFixProvider))]
[Shared]
public sealed class MyCodeFixProvider : BaseCodeFixProvider
{
    public override ImmutableArray<string> FixableDiagnosticIds =>
        ImmutableArray.Create(DiagnosticIdentifiers.MyRule);

    public override async Task RegisterCodeFixesAsync(CodeFixContext context)
    {
        SyntaxNode root = await context.GetSyntaxRootAsync().ConfigureAwait(false);
        if (!TryFindFirstAncestorOrSelf(root, context.Span, out SomeSyntax node))
            return;

        foreach (Diagnostic diagnostic in context.Diagnostics)
        {
            CodeAction codeAction = CodeAction.Create(
                "Fix title",
                ct => RefactorAsync(context.Document, node, ct),
                GetEquivalenceKey(diagnostic));
            context.RegisterCodeFix(codeAction, diagnostic);
        }
    }
}
```

Reference: `AddBracesAnalyzer.cs`, `AddBracesCodeFixProvider.cs`, `RCS1007AddBracesTests.cs`.

## Tests

In-repo base class only — not public `XunitDiagnosticVerifier`:

```csharp
public class RCS1234MyRuleTests
    : AbstractCSharpDiagnosticVerifier<MyAnalyzer, MyCodeFixProvider>
{
    public override DiagnosticDescriptor Descriptor { get; } = DiagnosticRules.MyRule;

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.MyRule)]
    public async Task Test()
    {
        await VerifyDiagnosticAndFixAsync(@"
class C { void M() { [|bad|]; } }
", @"
class C { void M() { fixed; } }
");
    }
}
```

- Spans: `[|` … `|]`
- Verbatim strings `@"..."`, not raw string literals
- No code fix: `EmptyCodeFixProvider` + `VerifyDiagnosticAsync`
- Config: `options: Options.AddConfigOption(ConfigOptionKeys.X, ConfigOptionValues.Y)`
