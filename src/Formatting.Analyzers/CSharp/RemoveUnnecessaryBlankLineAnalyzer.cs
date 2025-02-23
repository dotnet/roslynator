using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Text;
using Roslynator.CSharp;

namespace Roslynator.Formatting.CSharp;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class RemoveUnnecessaryBlankLineAnalyzer : BaseDiagnosticAnalyzer
{
    private static ImmutableArray<DiagnosticDescriptor> _supportedDiagnostics;

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
    {
        get
        {
            if (_supportedDiagnostics.IsDefault)
            {
                Immutable.InterlockedInitialize(
                    ref _supportedDiagnostics,
                    DiagnosticRules.RemoveUnnecessaryBlankLine);
            }

            return _supportedDiagnostics;
        }
    }

    public override void Initialize(AnalysisContext context)
    {
        base.Initialize(context);

        context.RegisterSyntaxTreeAction(c => AnalyzeTrailingTrivia(c));

        UnnecessaryBlankLineAnalysis.Instance.Initialize(context);
    }

    private static void AnalyzeTrailingTrivia(SyntaxTreeAnalysisContext context)
    {
        if (!context.Tree.TryGetText(out SourceText sourceText))
            return;

        if (!context.Tree.TryGetRoot(out SyntaxNode root))
            return;

        var emptyLines = default(TextSpan);
        var previousLineIsEmpty = false;
        int i = 0;

        foreach (TextLine textLine in sourceText.Lines)
        {
            var lineIsEmpty = false;

            if (textLine.Span.Length == 0)
            {
                SyntaxTrivia endOfLine = root.FindTrivia(textLine.End);

                if (endOfLine.IsEndOfLineTrivia())
                {
                    lineIsEmpty = true;

                    if (previousLineIsEmpty)
                    {
                        if (emptyLines.IsEmpty)
                        {
                            emptyLines = endOfLine.Span;
                        }
                        else
                        {
                            emptyLines = TextSpan.FromBounds(emptyLines.Start, endOfLine.Span.End);
                        }
                    }
                }
                else
                {
                    emptyLines = default;
                }
            }
            else
            {
                if (!emptyLines.IsEmpty)
                {
                    DiagnosticHelpers.ReportDiagnostic(
                        context,
                        DiagnosticRules.RemoveUnnecessaryBlankLine,
                        Location.Create(context.Tree, emptyLines));
                }

                emptyLines = default;
            }

            previousLineIsEmpty = lineIsEmpty;
            i++;
        }
    }

    private class UnnecessaryBlankLineAnalysis : Roslynator.CSharp.Analysis.UnnecessaryBlankLineAnalysis
    {
        public static UnnecessaryBlankLineAnalysis Instance { get; } = new();

        protected override DiagnosticDescriptor Descriptor => DiagnosticRules.RemoveUnnecessaryBlankLine;
    }
}
