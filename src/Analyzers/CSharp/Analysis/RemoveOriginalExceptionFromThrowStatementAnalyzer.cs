// Copyright (c) .NET Foundation and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Immutable;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Roslynator.CSharp.Analysis;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class RemoveOriginalExceptionFromThrowStatementAnalyzer : BaseDiagnosticAnalyzer
{
    private static ImmutableArray<DiagnosticDescriptor> _supportedDiagnostics;

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
    {
        get
        {
            if (_supportedDiagnostics.IsDefault)
                Immutable.InterlockedInitialize(ref _supportedDiagnostics, DiagnosticRules.RemoveOriginalExceptionFromThrowStatement);

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

        CatchDeclarationSyntax declaration = catchClause.Declaration;

        if (declaration is null)
            return;

        SemanticModel semanticModel = context.SemanticModel;
        CancellationToken cancellationToken = context.CancellationToken;

        ILocalSymbol symbol = semanticModel.GetDeclaredSymbol(declaration, cancellationToken);

        if (symbol?.IsErrorType() != false)
            return;

        var walker = new Walker(symbol, semanticModel, cancellationToken);

        walker.VisitBlock(catchClause.Block);

        ExpressionSyntax expression = walker.ThrowStatement?.Expression;

        if (expression is not null)
        {
            DiagnosticHelpers.ReportDiagnostic(
                context,
                DiagnosticRules.RemoveOriginalExceptionFromThrowStatement,
                expression);
        }
    }

    private class Walker : CSharpSyntaxWalker
    {
        public Walker(
            ISymbol symbol,
            SemanticModel semanticModel,
            CancellationToken cancellationToken)
        {
            Symbol = symbol;
            SemanticModel = semanticModel;
            CancellationToken = cancellationToken;
        }

        public ThrowStatementSyntax ThrowStatement { get; private set; }

        public ISymbol Symbol { get; }

        public SemanticModel SemanticModel { get; }

        public CancellationToken CancellationToken { get; }

        public override void VisitCatchClause(CatchClauseSyntax node)
        {
        }

        public override void VisitThrowStatement(ThrowStatementSyntax node)
        {
            ExpressionSyntax expression = node.Expression;

            if (expression is not null)
            {
                ISymbol symbol = SemanticModel.GetSymbol(expression, CancellationToken);

                if (SymbolEqualityComparer.Default.Equals(Symbol, symbol))
                    ThrowStatement = node;
            }

            base.VisitThrowStatement(node);
        }
    }
}
