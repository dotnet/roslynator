// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Roslynator.CSharp.Refactorings
{
    internal static class RemoveOriginalExceptionFromThrowStatementRefactoring
    {
        public static void Analyze(SyntaxNodeAnalysisContext context, CatchClauseSyntax catchClause)
        {
            CatchDeclarationSyntax declaration = catchClause.Declaration;

            if (declaration != null)
            {
                BlockSyntax block = catchClause.Block;

                if (block != null)
                {
                    ILocalSymbol symbol = context
                        .SemanticModel
                        .GetDeclaredSymbol(catchClause.Declaration, context.CancellationToken);

                    if (symbol != null)
                    {
                        foreach (SyntaxNode node in block.DescendantNodes(f => !f.IsKind(SyntaxKind.CatchClause)))
                        {
                            if (node.IsKind(SyntaxKind.ThrowStatement))
                            {
                                var throwStatement = (ThrowStatementSyntax)node;
                                if (throwStatement.Expression != null)
                                {
                                    ISymbol expressionSymbol = context
                                        .SemanticModel
                                        .GetSymbol(throwStatement.Expression, context.CancellationToken);

                                    if (expressionSymbol != null
                                        && symbol.Equals(expressionSymbol))
                                    {
                                        context.ReportDiagnostic(
                                            DiagnosticDescriptors.RemoveOriginalExceptionFromThrowStatement,
                                            throwStatement.Expression);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        public static Task<Document> RefactorAsync(
            Document document,
            ThrowStatementSyntax throwStatement,
            CancellationToken cancellationToken)
        {
            ThrowStatementSyntax newThrowStatement = throwStatement
                .RemoveNode(throwStatement.Expression, SyntaxRemoveOptions.KeepExteriorTrivia)
                .WithFormatterAnnotation();

            return document.ReplaceNodeAsync(throwStatement, newThrowStatement, cancellationToken);
        }
    }
}
