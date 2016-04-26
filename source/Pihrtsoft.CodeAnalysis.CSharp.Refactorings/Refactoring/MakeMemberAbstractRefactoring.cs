// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeRefactorings;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Formatting;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Pihrtsoft.CodeAnalysis.CSharp.Refactoring
{
    internal static class MakeMemberAbstractRefactoring
    {
        public static bool CanRefactor(CodeRefactoringContext context, MemberDeclarationSyntax memberDeclaration)
        {
            if (memberDeclaration == null)
                throw new ArgumentNullException(nameof(memberDeclaration));

            switch (memberDeclaration.Kind())
            {
                case SyntaxKind.PropertyDeclaration:
                    return CanRefactor(context, (PropertyDeclarationSyntax)memberDeclaration);
                case SyntaxKind.MethodDeclaration:
                    return CanRefactor(context, (MethodDeclarationSyntax)memberDeclaration);
                case SyntaxKind.IndexerDeclaration:
                    return CanRefactor(context, (IndexerDeclarationSyntax)memberDeclaration);
                default:
                    return false;
            }
        }

        private static bool CanRefactor(CodeRefactoringContext context, PropertyDeclarationSyntax propertyDeclaration)
        {
            if (propertyDeclaration.AccessorList != null
                && context.Span.IntersectsWith(propertyDeclaration.AccessorList.Span))
            {
                return false;
            }

            if (propertyDeclaration.ExpressionBody != null
                && context.Span.IntersectsWith(propertyDeclaration.ExpressionBody.Span))
            {
                return false;
            }

            if (propertyDeclaration.Modifiers.Contains(SyntaxKind.AbstractKeyword))
                return false;

            return true;
        }

        private static bool CanRefactor(CodeRefactoringContext context, MethodDeclarationSyntax methodDeclaration)
        {
            if (methodDeclaration.Body != null
                && context.Span.IntersectsWith(methodDeclaration.Body.Span))
            {
                return false;
            }

            if (methodDeclaration.ExpressionBody != null
                && context.Span.IntersectsWith(methodDeclaration.ExpressionBody.Span))
            {
                return false;
            }

            if (methodDeclaration.Modifiers.Contains(SyntaxKind.AbstractKeyword))
                return false;

            return true;
        }

        private static bool CanRefactor(CodeRefactoringContext context, IndexerDeclarationSyntax indexerDeclaration)
        {
            if (indexerDeclaration.AccessorList != null
                && context.Span.IntersectsWith(indexerDeclaration.AccessorList.Span))
            {
                return false;
            }

            if (indexerDeclaration.ExpressionBody != null
                && context.Span.IntersectsWith(indexerDeclaration.ExpressionBody.Span))
            {
                return false;
            }

            if (indexerDeclaration.Modifiers.Contains(SyntaxKind.AbstractKeyword))
                return false;

            return true;
        }

        public static async Task<Document> RefactorAsync(
            Document document,
            MemberDeclarationSyntax memberDeclaration,
            CancellationToken cancellationToken)
        {
            SyntaxNode oldRoot = await document.GetSyntaxRootAsync(cancellationToken);

            MemberDeclarationSyntax newMemberDeclaration = MakeAbstract(memberDeclaration)
                .WithAdditionalAnnotations(Formatter.Annotation);

            SyntaxNode newRoot = oldRoot.ReplaceNode(memberDeclaration, newMemberDeclaration);

            return document.WithSyntaxRoot(newRoot);
        }

        private static MemberDeclarationSyntax MakeAbstract(MemberDeclarationSyntax memberDeclaration)
        {
            switch (memberDeclaration.Kind())
            {
                case SyntaxKind.PropertyDeclaration:
                    return MakeAbstract((PropertyDeclarationSyntax)memberDeclaration);
                case SyntaxKind.MethodDeclaration:
                    return MakeAbstract((MethodDeclarationSyntax)memberDeclaration);
                case SyntaxKind.IndexerDeclaration:
                    return MakeAbstract((IndexerDeclarationSyntax)memberDeclaration);
                case SyntaxKind.EventFieldDeclaration:
                    return MakeAbstract((EventFieldDeclarationSyntax)memberDeclaration);
                default:
                    return memberDeclaration;
            }
        }

        private static MemberDeclarationSyntax MakeAbstract(PropertyDeclarationSyntax propertyDeclaration)
        {
            if (propertyDeclaration == null)
                throw new ArgumentNullException(nameof(propertyDeclaration));

            AccessorListSyntax accessorList = AccessorList();

            if (propertyDeclaration.ExpressionBody != null)
            {
                accessorList = accessorList
                    .AddAccessors(
                        AccessorDeclaration(SyntaxKind.GetAccessorDeclaration)
                            .WithSemicolonToken(Token(SyntaxKind.SemicolonToken)));
            }
            else
            {
                AccessorDeclarationSyntax getter = propertyDeclaration.Getter();
                if (getter != null)
                {
                    accessorList = accessorList.AddAccessors(getter
                       .WithBody(null)
                       .WithSemicolonToken(Token(SyntaxKind.SemicolonToken)));
                }

                AccessorDeclarationSyntax setter = propertyDeclaration.Setter();
                if (setter != null)
                {
                    accessorList = accessorList.AddAccessors(setter
                       .WithBody(null)
                       .WithSemicolonToken(Token(SyntaxKind.SemicolonToken)));
                }
            }

            SyntaxTokenList modifiers = propertyDeclaration.Modifiers;

            if (!modifiers.Contains(SyntaxKind.AbstractKeyword))
                modifiers = modifiers.Add(Token(SyntaxKind.AbstractKeyword));

            return propertyDeclaration
                .WithExpressionBody(null)
                .WithSemicolonToken(Token(SyntaxKind.None))
                .WithAccessorList(accessorList)
                .WithModifiers(modifiers);
        }

        private static MethodDeclarationSyntax MakeAbstract(MethodDeclarationSyntax methodDeclaration)
        {
            if (methodDeclaration == null)
                throw new ArgumentNullException(nameof(methodDeclaration));

            SyntaxTokenList modifiers = methodDeclaration.Modifiers;

            if (!modifiers.Contains(SyntaxKind.AbstractKeyword))
                modifiers = modifiers.Add(Token(SyntaxKind.AbstractKeyword));

            return methodDeclaration
                .WithExpressionBody(null)
                .WithSemicolonToken(Token(SyntaxKind.None))
                .WithBody(null)
                .WithModifiers(modifiers)
                .WithSemicolonToken(Token(SyntaxKind.SemicolonToken));
        }

        private static IndexerDeclarationSyntax MakeAbstract(IndexerDeclarationSyntax indexerDeclaration)
        {
            if (indexerDeclaration == null)
                throw new ArgumentNullException(nameof(indexerDeclaration));

            AccessorListSyntax accessorList = AccessorList();

            if (indexerDeclaration.ExpressionBody != null)
            {
                accessorList = accessorList
                    .AddAccessors(
                        AccessorDeclaration(SyntaxKind.GetAccessorDeclaration)
                            .WithSemicolonToken(Token(SyntaxKind.SemicolonToken)));
            }
            else
            {
                AccessorDeclarationSyntax getter = indexerDeclaration.Getter();
                if (getter != null)
                {
                    accessorList = accessorList.AddAccessors(getter
                       .WithBody(null)
                       .WithSemicolonToken(Token(SyntaxKind.SemicolonToken)));
                }

                AccessorDeclarationSyntax setter = indexerDeclaration.Setter();
                if (setter != null)
                {
                    accessorList = accessorList.AddAccessors(setter
                       .WithBody(null)
                       .WithSemicolonToken(Token(SyntaxKind.SemicolonToken)));
                }
            }

            SyntaxTokenList modifiers = indexerDeclaration.Modifiers;

            if (!modifiers.Contains(SyntaxKind.AbstractKeyword))
                modifiers = modifiers.Add(Token(SyntaxKind.AbstractKeyword));

            return indexerDeclaration
                .WithExpressionBody(null)
                .WithSemicolonToken(Token(SyntaxKind.None))
                .WithAccessorList(accessorList)
                .WithModifiers(modifiers);
        }

        private static EventFieldDeclarationSyntax MakeAbstract(EventFieldDeclarationSyntax eventDeclaration)
        {
            if (eventDeclaration == null)
                throw new ArgumentNullException(nameof(eventDeclaration));

            SyntaxTokenList modifiers = eventDeclaration.Modifiers;

            if (!modifiers.Contains(SyntaxKind.AbstractKeyword))
                modifiers = modifiers.Add(SyntaxFactory.Token(SyntaxKind.AbstractKeyword));

            return eventDeclaration
                .WithModifiers(modifiers);
        }
    }
}
