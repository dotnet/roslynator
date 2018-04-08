// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Immutable;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Roslynator.CSharp.Analysis
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class RemoveOriginalExceptionFromThrowStatementAnalyzer : BaseDiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        {
            get { return ImmutableArray.Create(DiagnosticDescriptors.RemoveOriginalExceptionFromThrowStatement); }
        }

        public override void Initialize(AnalysisContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            base.Initialize(context);

            context.RegisterSyntaxNodeAction(AnalyzeCatchClause, SyntaxKind.CatchClause);
        }

        public static void AnalyzeCatchClause(SyntaxNodeAnalysisContext context)
        {
            var catchClause = (CatchClauseSyntax)context.Node;

            BlockSyntax block = catchClause.Block;

            if (block == null)
                return;

            CatchDeclarationSyntax declaration = catchClause.Declaration;

            if (declaration == null)
                return;

            SemanticModel semanticModel = context.SemanticModel;
            CancellationToken cancellationToken = context.CancellationToken;

            ILocalSymbol symbol = semanticModel.GetDeclaredSymbol(declaration, cancellationToken);

            if (symbol?.IsErrorType() != false)
                return;

            //XPERF: SyntaxWalker
            foreach (SyntaxNode node in block.DescendantNodes(descendIntoChildren: f => f.Kind() != SyntaxKind.CatchClause))
            {
                if (node.Kind() != SyntaxKind.ThrowStatement)
                    continue;

                var throwStatement = (ThrowStatementSyntax)node;
                ExpressionSyntax expression = throwStatement.Expression;

                if (expression == null)
                    continue;

                ISymbol expressionSymbol = semanticModel.GetSymbol(expression, cancellationToken);

                if (!symbol.Equals(expressionSymbol))
                    continue;

                context.ReportDiagnostic(
                    DiagnosticDescriptors.RemoveOriginalExceptionFromThrowStatement,
                    expression);
            }
        }
    }
}
