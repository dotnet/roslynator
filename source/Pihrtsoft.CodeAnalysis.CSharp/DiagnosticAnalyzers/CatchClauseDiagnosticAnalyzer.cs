// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Pihrtsoft.CodeAnalysis.CSharp.DiagnosticAnalyzers
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class CatchClauseDiagnosticAnalyzer : BaseDiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
            => ImmutableArray.Create(DiagnosticDescriptors.RemoveOriginalExceptionFromThrowStatement);

        public override void Initialize(AnalysisContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            context.RegisterSyntaxNodeAction(f => AnalyzeSyntaxNode(f), SyntaxKind.CatchClause);
        }

        private void AnalyzeSyntaxNode(SyntaxNodeAnalysisContext context)
        {
            if (GeneratedCodeAnalyzer?.IsGeneratedCode(context) == true)
                return;

            var catchClause = (CatchClauseSyntax)context.Node;

            if (catchClause.Declaration == null || catchClause.Block == null)
                return;

            CatchDeclarationSyntax declaration = catchClause.Declaration;

            ILocalSymbol symbol = context.SemanticModel.GetDeclaredSymbol(declaration, context.CancellationToken);

            if (symbol == null)
                return;

            foreach (SyntaxNode node in catchClause.Block.DescendantNodes(f => !f.IsKind(SyntaxKind.CatchClause)))
            {
                if (node.IsKind(SyntaxKind.ThrowStatement))
                {
                    var throwStatement = (ThrowStatementSyntax)node;
                    if (throwStatement.Expression != null)
                    {
                        ISymbol expressionSymbol = context.SemanticModel.GetSymbolInfo(throwStatement.Expression, context.CancellationToken).Symbol;

                        if (expressionSymbol != null
                            && symbol.Equals(expressionSymbol))
                        {
                            context.ReportDiagnostic(
                                DiagnosticDescriptors.RemoveOriginalExceptionFromThrowStatement,
                                throwStatement.Expression.GetLocation());
                        }
                    }
                }
            }
        }
    }
}
