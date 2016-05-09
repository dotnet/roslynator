// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Pihrtsoft.CodeAnalysis;
using Pihrtsoft.CodeAnalysis.CSharp.Analysis;

namespace Pihrtsoft.CodeAnalysis.CSharp
{
    internal class IfElseChainAnalysisResult
    {
        internal IfElseChainAnalysisResult(IfStatementSyntax ifStatement)
        {
            if (ifStatement == null)
                throw new ArgumentNullException(nameof(ifStatement));

            bool anyHasEmbedded = false;
            bool anyHasBlock = false;
            bool allSupportsEmbedded = true;

            int cnt = 0;

            foreach (SyntaxNode node in IfElseChainAnalysis.GetChain(ifStatement))
            {
                cnt++;

                StatementSyntax statement = GetStatement(node);

                if (!anyHasEmbedded && !statement.IsKind(SyntaxKind.Block))
                    anyHasEmbedded = true;

                if (!anyHasBlock && statement.IsKind(SyntaxKind.Block))
                    anyHasBlock = true;

                if (allSupportsEmbedded && !SupportsEmbedded(statement))
                    allSupportsEmbedded = false;

                if (cnt > 1 && anyHasEmbedded && !allSupportsEmbedded)
                {
                    AddBracesToChain = true;
                    return;
                }
            }

            if (cnt > 1
                && allSupportsEmbedded
                && anyHasBlock)
            {
                RemoveBracesFromChain = true;

                if (anyHasEmbedded)
                    AddBracesToChain = true;
            }
        }

        private static bool SupportsEmbedded(StatementSyntax statement)
        {
            if (statement.Parent.IsKind(SyntaxKind.IfStatement)
                && ((IfStatementSyntax)statement.Parent).Condition?.IsMultiline() == true)
            {
                return false;
            }

            if (statement.IsKind(SyntaxKind.Block))
            {
                var block = (BlockSyntax)statement;

                if (block.Statements.Count != 1)
                    return false;

                statement = block.Statements[0];
            }

            return !statement.IsKind(SyntaxKind.LocalDeclarationStatement)
                && !statement.IsKind(SyntaxKind.LabeledStatement)
                && statement.IsSingleline();
        }

        private static StatementSyntax GetStatement(SyntaxNode node)
        {
            if (node.IsKind(SyntaxKind.IfStatement))
            {
                return ((IfStatementSyntax)node).Statement;
            }
            else
            {
                return ((ElseClauseSyntax)node).Statement;
            }
        }

        public bool AddBracesToChain { get; }

        public bool RemoveBracesFromChain { get; }
    }
}
