// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Text;

namespace Roslynator.CSharp.Analyzers
{
    internal static class MergeLocalDeclarationWithReturnStatementAnalyzer
    {
        public static void Analyze(SyntaxNodeAnalysisContext context)
        {
            var returnStatement = (ReturnStatementSyntax)context.Node;

            if (returnStatement.Expression?.IsKind(SyntaxKind.IdentifierName) != true)
                return;

            LocalDeclarationStatementSyntax localDeclaration = GetLocalDeclaration(returnStatement);

            if (localDeclaration == null)
                return;

            VariableDeclaratorSyntax declarator = GetVariableDeclarator(localDeclaration);

            if (declarator == null)
                return;

            ISymbol symbol = context.SemanticModel.GetSymbolInfo(returnStatement.Expression, context.CancellationToken).Symbol;

            if (symbol?.IsLocal() != true)
                return;

            ISymbol symbol2 = context.SemanticModel.GetDeclaredSymbol(declarator, context.CancellationToken);

            if (symbol.Equals(symbol2))
            {
                TextSpan span = TextSpan.FromBounds(localDeclaration.Span.Start, returnStatement.Span.End);

                if (returnStatement.Parent
                    .DescendantTrivia(span, descendIntoTrivia: false)
                    .All(f => f.IsWhitespaceOrEndOfLineTrivia()))
                {
                    context.ReportDiagnostic(
                        DiagnosticDescriptors.MergeLocalDeclarationWithReturnStatement,
                        Location.Create(context.Node.SyntaxTree, span));
                }

                context.FadeOutNode(DiagnosticDescriptors.MergeLocalDeclarationWithReturnStatementFadeOut, localDeclaration.Declaration.Type);
                context.FadeOutToken(DiagnosticDescriptors.MergeLocalDeclarationWithReturnStatementFadeOut, declarator.Identifier);
                context.FadeOutToken(DiagnosticDescriptors.MergeLocalDeclarationWithReturnStatementFadeOut, declarator.Initializer.EqualsToken);
                context.FadeOutToken(DiagnosticDescriptors.MergeLocalDeclarationWithReturnStatementFadeOut, localDeclaration.SemicolonToken);
                context.FadeOutNode(DiagnosticDescriptors.MergeLocalDeclarationWithReturnStatementFadeOut, returnStatement.Expression);
            }
        }

        private static LocalDeclarationStatementSyntax GetLocalDeclaration(ReturnStatementSyntax returnStatement)
        {
            if (returnStatement.Parent?.IsKind(SyntaxKind.Block) == true)
            {
                var block = (BlockSyntax)returnStatement.Parent;

                if (block.Statements.Count > 1)
                {
                    int index = block.Statements.IndexOf(returnStatement);

                    if (index > 0)
                    {
                        StatementSyntax statement = block.Statements[index - 1];

                        if (statement.IsKind(SyntaxKind.LocalDeclarationStatement))
                            return (LocalDeclarationStatementSyntax)statement;
                    }
                }
            }

            return null;
        }

        private static VariableDeclaratorSyntax GetVariableDeclarator(LocalDeclarationStatementSyntax localDeclaration)
        {
            if (localDeclaration.Declaration?.Variables.Count == 1)
            {
                VariableDeclaratorSyntax declarator = localDeclaration.Declaration.Variables[0];

                if (declarator.Initializer?.Value != null)
                    return declarator;
            }

            return null;
        }
    }
}
