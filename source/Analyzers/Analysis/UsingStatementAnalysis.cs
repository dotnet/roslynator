// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Roslynator.CSharp.Analysis
{
    internal static class UsingStatementAnalysis
    {
        public static bool ContainsEmbeddableUsingStatement(UsingStatementSyntax usingStatement)
        {
            if (usingStatement.Statement?.IsKind(SyntaxKind.Block) == true)
            {
                var block = (BlockSyntax)usingStatement.Statement;

                return block.Statements.Count == 1
                    && block.Statements[0].IsKind(SyntaxKind.UsingStatement)
                    && CheckTrivia(block, (UsingStatementSyntax)block.Statements[0]);
            }

            return false;
        }

        private static bool CheckTrivia(BlockSyntax block, UsingStatementSyntax usingStatement)
        {
            return block.OpenBraceToken.TrailingTrivia.All(f => f.IsWhitespaceOrEndOfLineTrivia())
                && block.CloseBraceToken.LeadingTrivia.All(f => f.IsWhitespaceOrEndOfLineTrivia())
                && usingStatement.GetLeadingTrivia().All(f => f.IsWhitespaceOrEndOfLineTrivia())
                && usingStatement.GetTrailingTrivia().All(f => f.IsWhitespaceOrEndOfLineTrivia());
        }
    }
}
