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

        context.RegisterSyntaxNodeAction(f => AnalyzeCatchClause(f), SyntaxKind.TryStatement);
    }

    private static void AnalyzeCatchClause(SyntaxNodeAnalysisContext context)
    {
        var tryStatement = (TryStatementSyntax)context.Node;

        if (!tryStatement.Catches.Any())
            return;

        CatchClauseSyntax lastCatchClause = tryStatement.Catches.Last();

        if (lastCatchClause.Declaration is not null)
            return;

        if (lastCatchClause.Block?.Statements.Count != 1)
            return;

        if (lastCatchClause.Block.Statements[0] is not ThrowStatementSyntax throwStatement || throwStatement.Expression is not null)
            return;

        if (tryStatement.Catches.Count > 1 || tryStatement.Finally is not null)
        {
            DiagnosticHelpers.ReportDiagnostic(context, DiagnosticRules.RemoveRedundantCatchBlock, lastCatchClause);
        }
        else
        {
            BlockSyntax tryBlock = tryStatement.Block;

            if (tryBlock?.Statements.Any() != true)
                return;

            if (!SyntaxTriviaAnalysis.IsExteriorTriviaEmptyOrWhitespace(tryBlock.OpenBraceToken))
                return;

            if (!SyntaxTriviaAnalysis.IsExteriorTriviaEmptyOrWhitespace(tryBlock.CloseBraceToken))
                return;

            if (!lastCatchClause.CatchKeyword.LeadingTrivia.IsEmptyOrWhitespace())
                return;

            DiagnosticHelpers.ReportDiagnostic(context, DiagnosticRules.RemoveRedundantCatchBlock, lastCatchClause);
        }
    }
}
