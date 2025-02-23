﻿// Copyright (c) .NET Foundation and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Immutable;
using System.Diagnostics;
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

        ExpressionSyntax expression = null;
        Walker walker = null;

        try
        {
            walker = Walker.GetInstance();

            walker.Symbol = symbol;
            walker.SemanticModel = semanticModel;
            walker.CancellationToken = cancellationToken;

            walker.VisitBlock(catchClause.Block);

            expression = walker.ThrowStatement?.Expression;
        }
        finally
        {
            if (walker is not null)
                Walker.Free(walker);
        }

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
        [ThreadStatic]
        private static Walker _cachedInstance;

        public ThrowStatementSyntax ThrowStatement { get; set; }

        public ISymbol Symbol { get; set; }

        public SemanticModel SemanticModel { get; set; }

        public CancellationToken CancellationToken { get; set; }

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

        public static Walker GetInstance()
        {
            Walker walker = _cachedInstance;

            if (walker is not null)
            {
                Debug.Assert(walker.Symbol is null);
                Debug.Assert(walker.SemanticModel is null);
                Debug.Assert(walker.CancellationToken == default);
                Debug.Assert(walker.ThrowStatement is null);

                _cachedInstance = null;
                return walker;
            }

            return new Walker();
        }

        public static void Free(Walker walker)
        {
            walker.Symbol = null;
            walker.SemanticModel = null;
            walker.CancellationToken = default;
            walker.ThrowStatement = null;

            _cachedInstance = walker;
        }
    }
}
