// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static Roslynator.CSharp.CSharpFactory;

namespace Roslynator.CSharp.Refactorings
{
    internal static class MakeMemberVirtualRefactoring
    {
        public static void ComputeRefactoring(RefactoringContext context, MethodDeclarationSyntax methodDeclaration)
        {
            if (methodDeclaration.Modifiers.Contains(SyntaxKind.AbstractKeyword)
                && IsContainedInNonSealedClass(methodDeclaration))
            {
                context.RegisterRefactoring(
                    GetTitle("method"),
                    cancellationToken => RefactorAsync(context.Document, methodDeclaration, cancellationToken));
            }
        }

        public static void ComputeRefactoring(RefactoringContext context, IndexerDeclarationSyntax indexerDeclaration)
        {
            if (indexerDeclaration.Modifiers.Contains(SyntaxKind.AbstractKeyword)
                && IsContainedInNonSealedClass(indexerDeclaration))
            {
                context.RegisterRefactoring(
                    GetTitle("indexer"),
                    cancellationToken => RefactorAsync(context.Document, indexerDeclaration, cancellationToken));
            }
        }

        private static bool IsContainedInNonSealedClass(MemberDeclarationSyntax memberDeclaration)
        {
            SyntaxNode parent = memberDeclaration.Parent;

            Debug.Assert(parent?.IsKind(SyntaxKind.ClassDeclaration) == true, parent?.Kind().ToString());

            return parent?.IsKind(SyntaxKind.ClassDeclaration) == true
                && !((ClassDeclarationSyntax)parent).Modifiers.Contains(SyntaxKind.SealedKeyword);
        }

        private static string GetTitle(string member)
        {
            return $"Make {member} virtual";
        }

        private static async Task<Document> RefactorAsync(
            Document document,
            MethodDeclarationSyntax methodDeclaration,
            CancellationToken cancellationToken)
        {
            BlockSyntax body = Block();

            TypeSyntax returnType = methodDeclaration.ReturnType;

            if (returnType?.IsVoid() == false)
            {
                SemanticModel semanticModel = await document.GetSemanticModelAsync(cancellationToken).ConfigureAwait(false);

                IMethodSymbol methodSymbol = semanticModel.GetDeclaredSymbol(methodDeclaration, cancellationToken);

                ExpressionSyntax expression = DefaultValue(methodSymbol.ReturnType, returnType);

                body = body.AddStatements(ReturnStatement(expression));
            }

            body = body.WithFormatterAnnotation();

            MethodDeclarationSyntax newNode = methodDeclaration
                .WithModifiers(ReplaceAbstractWithVirtual(methodDeclaration.Modifiers))
                .WithBody(body)
                .WithSemicolonToken(default(SyntaxToken))
                .WithTrailingTrivia(methodDeclaration.GetTrailingTrivia());

            return await document.ReplaceNodeAsync(methodDeclaration, newNode, cancellationToken).ConfigureAwait(false);
        }

        private static async Task<Document> RefactorAsync(
            Document document,
            IndexerDeclarationSyntax indexerDeclaration,
            CancellationToken cancellationToken)
        {
            IndexerDeclarationSyntax newNode = ExpandIndexer(indexerDeclaration);

            newNode = newNode
                .WithModifiers(ReplaceAbstractWithVirtual(newNode.Modifiers))
                .WithTriviaFrom(indexerDeclaration)
                .WithFormatterAnnotation();

            return await document.ReplaceNodeAsync(indexerDeclaration, newNode, cancellationToken).ConfigureAwait(false);
        }

        private static IndexerDeclarationSyntax ExpandIndexer(IndexerDeclarationSyntax indexerDeclaration)
        {
            IEnumerable<AccessorDeclarationSyntax> accessors = indexerDeclaration
                .AccessorList
                .Accessors
                .Select(f => f.WithBody(Block()).WithoutSemicolonToken());

            AccessorListSyntax accessorList = AccessorList(accessors);

            accessorList = SyntaxRemover.RemoveWhitespaceOrEndOfLine(accessorList)
                .WithCloseBraceToken(accessorList.CloseBraceToken.WithLeadingTrivia(NewLineTrivia()));

            return indexerDeclaration
                .WithSemicolonToken(default(SyntaxToken))
                .WithAccessorList(accessorList);
        }

        private static SyntaxTokenList ReplaceAbstractWithVirtual(SyntaxTokenList modifiers)
        {
            int index = modifiers.IndexOf(SyntaxKind.AbstractKeyword);

            return modifiers.ReplaceAt(index, VirtualToken().WithTriviaFrom(modifiers[index]));
        }
    }
}