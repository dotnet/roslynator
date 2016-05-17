// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Text;

namespace Pihrtsoft.CodeAnalysis.CSharp.Analyzers
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

            if (symbol?.Kind != SymbolKind.Local)
                return;

            ISymbol symbol2 = context.SemanticModel.GetDeclaredSymbol(declarator, context.CancellationToken);

            if (symbol.Equals(symbol2))
            {
                TextSpan span = TextSpan.FromBounds(localDeclaration.Span.Start, returnStatement.Span.End);

                if (returnStatement.Parent
                    .DescendantTrivia(span, descendIntoTrivia: false)
                    .All(f => f.IsWhitespaceOrEndOfLine()))
                {
                    context.ReportDiagnostic(
                        DiagnosticDescriptors.MergeLocalDeclarationWithReturnStatement,
                        Location.Create(context.Node.SyntaxTree, span));
                }

                DiagnosticHelper.FadeOutNode(context, localDeclaration.Declaration.Type, DiagnosticDescriptors.MergeLocalDeclarationWithReturnStatementFadeOut);
                DiagnosticHelper.FadeOutToken(context, declarator.Identifier, DiagnosticDescriptors.MergeLocalDeclarationWithReturnStatementFadeOut);
                DiagnosticHelper.FadeOutToken(context, declarator.Initializer.EqualsToken, DiagnosticDescriptors.MergeLocalDeclarationWithReturnStatementFadeOut);
                DiagnosticHelper.FadeOutToken(context, localDeclaration.SemicolonToken, DiagnosticDescriptors.MergeLocalDeclarationWithReturnStatementFadeOut);
                DiagnosticHelper.FadeOutNode(context, returnStatement.Expression, DiagnosticDescriptors.MergeLocalDeclarationWithReturnStatementFadeOut);
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
