// Copyright (c) .NET Foundation and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Text;
using Roslynator.CSharp;

namespace Roslynator.Formatting.CSharp;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class UseSpacesInsteadOfTabAnalyzer : BaseDiagnosticAnalyzer
{
    private static ImmutableArray<DiagnosticDescriptor> _supportedDiagnostics;

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
    {
        get
        {
            if (_supportedDiagnostics.IsDefault)
                Immutable.InterlockedInitialize(ref _supportedDiagnostics, DiagnosticRules.UseSpacesInsteadOfTab);

            return _supportedDiagnostics;
        }
    }

    public override void Initialize(AnalysisContext context)
    {
        base.Initialize(context);

        context.RegisterSyntaxTreeAction(f => AnalyzeSyntaxTree(f));
    }

    private static void AnalyzeSyntaxTree(SyntaxTreeAnalysisContext context)
    {
        SyntaxTree tree = context.Tree;

        if (!tree.TryGetRoot(out SyntaxNode root))
            return;

        var walker = new UseSpacesInsteadOfTabWalker(context);

        walker.Visit(root);
    }

    private class UseSpacesInsteadOfTabWalker : CSharpSyntaxWalker
    {
        public UseSpacesInsteadOfTabWalker(SyntaxTreeAnalysisContext analysisContext)
            : base(SyntaxWalkerDepth.StructuredTrivia)
        {
            AnalysisContext = analysisContext;
        }

        public SyntaxTreeAnalysisContext AnalysisContext { get; }

        public override void VisitTrivia(SyntaxTrivia trivia)
        {
            if (!trivia.IsWhitespaceTrivia())
                return;

            string text = trivia.ToString();

            for (int i = 0; i < text.Length; i++)
            {
                if (text[i] == '\t')
                {
                    int index = i;

                    do
                    {
                        i++;
                    }
                    while (i < text.Length && text[i] == '\t');

                    AnalysisContext.ReportDiagnostic(
                        DiagnosticRules.UseSpacesInsteadOfTab,
                        Location.Create(AnalysisContext.Tree, new TextSpan(trivia.SpanStart + index, i - index)));
                }
            }
        }
    }
}
