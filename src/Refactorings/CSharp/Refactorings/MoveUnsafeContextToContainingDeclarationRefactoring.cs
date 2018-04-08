// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.CSharp.Syntax;

namespace Roslynator.CSharp.Refactorings
{
    internal static class MoveUnsafeContextToContainingDeclarationRefactoring
    {
        public static void ComputeRefactoring(RefactoringContext context, MemberDeclarationSyntax memberDeclaration)
        {
            ModifierListInfo modifiersInfo = SyntaxInfo.ModifierListInfo(memberDeclaration);

            if (!modifiersInfo.Success)
                return;

            SyntaxToken unsafeModifier = modifiersInfo.Modifiers.Find(SyntaxKind.UnsafeKeyword);

            ComputeRefactoring(context, memberDeclaration, unsafeModifier);
        }

        public static void ComputeRefactoring(RefactoringContext context, LocalFunctionStatementSyntax localFunction)
        {
            SyntaxToken unsafeModifier = localFunction.Modifiers.Find(SyntaxKind.UnsafeKeyword);

            ComputeRefactoring(context, localFunction, unsafeModifier);
        }

        private static void ComputeRefactoring(RefactoringContext context, SyntaxNode node, SyntaxToken unsafeModifier)
        {
            if (unsafeModifier.Kind() != SyntaxKind.UnsafeKeyword)
                return;

            if (!context.Span.IsEmptyAndContainedInSpan(unsafeModifier))
                return;

            SyntaxNode parent = node.FirstAncestor(f => CSharpFacts.CanHaveUnsafeModifier(f.Kind()));

            if (parent == null)
                return;

            ModifierListInfo modifiersInfo = SyntaxInfo.ModifierListInfo(parent);

            if (!modifiersInfo.Success)
                return;

            if (modifiersInfo.IsUnsafe)
                return;

            context.RegisterRefactoring(
                GetTitle(parent.Kind()),
                ct => RefactorAsync(context.Document, node, parent, ct));
        }

        public static void ComputeRefactoring(RefactoringContext context, UnsafeStatementSyntax unsafeStatement)
        {
            SyntaxToken unsafeKeyword = unsafeStatement.UnsafeKeyword;

            if (!context.Span.IsEmptyAndContainedInSpan(unsafeKeyword))
                return;

            SyntaxNode parent = unsafeStatement.FirstAncestor(f => CSharpFacts.CanHaveUnsafeModifier(f.Kind()));

            if (parent == null)
                return;

            ModifierListInfo modifiersInfo = SyntaxInfo.ModifierListInfo(parent);

            if (modifiersInfo.IsUnsafe)
                return;

            context.RegisterRefactoring(
                GetTitle(parent.Kind()),
                ct => RefactorAsync(context.Document, unsafeStatement, parent, ct));
        }

        private static string GetTitle(SyntaxKind kind)
        {
            switch (kind)
            {
                case SyntaxKind.LocalFunctionStatement:
                    return "Move unsafe context to containing local function";
                case SyntaxKind.GetAccessorDeclaration:
                case SyntaxKind.SetAccessorDeclaration:
                case SyntaxKind.AddAccessorDeclaration:
                case SyntaxKind.RemoveAccessorDeclaration:
                case SyntaxKind.UnknownAccessorDeclaration:
                    return "Move unsafe context to containing accessor";
                default:
                    return "Move unsafe context to containing declaration";
            }
        }

        public static Task<Document> RefactorAsync(
            Document document,
            SyntaxNode node,
            SyntaxNode containingNode,
            CancellationToken cancellationToken)
        {
            SyntaxNode newNode = containingNode
                .ReplaceNode(node, node.RemoveModifier(SyntaxKind.UnsafeKeyword))
                .InsertModifier(SyntaxKind.UnsafeKeyword);

            return document.ReplaceNodeAsync(containingNode, newNode, cancellationToken);
        }

        public static Task<Document> RefactorAsync(
            Document document,
            UnsafeStatementSyntax unsafeStatement,
            SyntaxNode containingNode,
            CancellationToken cancellationToken)
        {
            SyntaxToken keyword = unsafeStatement.UnsafeKeyword;

            BlockSyntax block = unsafeStatement.Block;

            SyntaxList<StatementSyntax> statements = block.Statements;

            SyntaxNode newNode = null;

            int count = statements.Count;

            if (count == 0)
            {
                newNode = containingNode.RemoveNode(unsafeStatement);
            }
            else
            {
                StatementSyntax first = statements.First();
                StatementSyntax last = statements.Last();

                SyntaxTriviaList leadingTrivia = keyword.LeadingTrivia
                    .AddRange(keyword.TrailingTrivia.EmptyIfWhitespace())
                    .AddRange(block.GetLeadingTrivia().EmptyIfWhitespace())
                    .AddRange(block.OpenBraceToken.TrailingTrivia.EmptyIfWhitespace())
                    .AddRange(first.GetLeadingTrivia().EmptyIfWhitespace());

                SyntaxTriviaList trailingTrivia = last.GetTrailingTrivia().EmptyIfWhitespace()
                    .AddRange(block.CloseBraceToken.LeadingTrivia.EmptyIfWhitespace())
                    .AddRange(block.GetTrailingTrivia());

                statements = statements
                    .ReplaceAt(0, first.WithLeadingTrivia(leadingTrivia))
                    .ReplaceAt(count - 1, last.WithTrailingTrivia(trailingTrivia));

                newNode = containingNode.ReplaceNode(unsafeStatement, statements.Select(f => f.WithFormatterAnnotation()));
            }

            newNode = newNode.InsertModifier(SyntaxKind.UnsafeKeyword);

            return document.ReplaceNodeAsync(containingNode, newNode, cancellationToken);
        }
    }
}
