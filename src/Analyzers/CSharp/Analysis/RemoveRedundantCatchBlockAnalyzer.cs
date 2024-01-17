// Copyright (c) .NET Foundation and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Roslynator.CSharp.Analysis;

namespace Roslynator.CSharp.CSharp.Analysis;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class RemoveRedundantCatchBlockAnalyzer : BaseDiagnosticAnalyzer
{
    private static ImmutableArray<DiagnosticDescriptor> _supportedDiagnostics;

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
    {
        get
        {
            if (_supportedDiagnostics.IsDefault)
            {
                Immutable.InterlockedInitialize(ref _supportedDiagnostics, DiagnosticRules.RemoveRedundantCatchBlock);
            }

            return _supportedDiagnostics;
        }
    }

    public override void Initialize(AnalysisContext context)
    {
        base.Initialize(context);

        context.RegisterSyntaxNodeAction(f => AnalyzeCatchClause(f), SyntaxKind.CatchClause);
    }

    private static void AnalyzeCatchClause(SyntaxNodeAnalysisContext context)
    {
        var catchClause = (CatchClauseSyntax)context.Node;

        if (catchClause.Parent is not TryStatementSyntax tryStatement)
            return;

        if (catchClause.Declaration is not null)
            return;

        if (catchClause.Block?.Statements.Count != 1)
            return;

        if (catchClause.Block?.Statements[0] is not ThrowStatementSyntax)
            return;

        if (tryStatement.Catches.Count > 1 || tryStatement.Finally is not null)
        {
            DiagnosticHelpers.ReportDiagnostic(context, DiagnosticRules.RemoveRedundantCatchBlock, catchClause);
        }
        else
        {
            BlockSyntax tryBlock = tryStatement.Block;

            if (tryBlock?.Statements.Any() != true)
                return;

            if (!SyntaxTriviaAnalysis.IsExteriorTriviaEmptyOrWhitespace(tryStatement.TryKeyword))
                return;

            if (!SyntaxTriviaAnalysis.IsExteriorTriviaEmptyOrWhitespace(tryBlock.OpenBraceToken))
                return;

            if (!SyntaxTriviaAnalysis.IsExteriorTriviaEmptyOrWhitespace(tryBlock.CloseBraceToken))
                return;

            if (!catchClause.CatchKeyword.LeadingTrivia.IsEmptyOrWhitespace())
                return;

            DiagnosticHelpers.ReportDiagnostic(context, DiagnosticRules.RemoveRedundantCatchBlock, catchClause);
        }
    }
}
