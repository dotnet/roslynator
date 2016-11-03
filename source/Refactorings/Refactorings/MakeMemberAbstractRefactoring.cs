// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Roslynator.CSharp.Refactorings
{
    internal static class MakeMemberAbstractRefactoring
    {
        public static bool CanRefactor(PropertyDeclarationSyntax propertyDeclaration)
        {
            return !propertyDeclaration.Modifiers.Contains(SyntaxKind.AbstractKeyword)
                && IsAbstractClass(propertyDeclaration.Parent);
        }

        public static bool CanRefactor(MethodDeclarationSyntax methodDeclaration)
        {
            return !methodDeclaration.Modifiers.Contains(SyntaxKind.AbstractKeyword)
                && IsAbstractClass(methodDeclaration.Parent);
        }

        public static bool CanRefactor(IndexerDeclarationSyntax indexerDeclaration)
        {
            return !indexerDeclaration.Modifiers.Contains(SyntaxKind.AbstractKeyword)
                && IsAbstractClass(indexerDeclaration.Parent);
        }

        private static bool IsAbstractClass(SyntaxNode node)
        {
            return node?.IsKind(SyntaxKind.ClassDeclaration) == true
                && ((ClassDeclarationSyntax)node).Modifiers.Contains(SyntaxKind.AbstractKeyword);
        }

        public static async Task<Document> RefactorAsync(
            Document document,
            MemberDeclarationSyntax memberDeclaration,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            SyntaxNode oldRoot = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);

            MemberDeclarationSyntax newMemberDeclaration = MakeAbstract(memberDeclaration)
                .WithTriviaFrom(memberDeclaration)
                .WithFormatterAnnotation();

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
                default:
                    return memberDeclaration;
            }
        }

        private static MemberDeclarationSyntax MakeAbstract(PropertyDeclarationSyntax propertyDeclaration)
        {
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

            return propertyDeclaration
                .WithExpressionBody(null)
                .WithSemicolonToken(Token(SyntaxKind.None))
                .WithAccessorList(accessorList)
                .WithModifiers(CreateModifiers(propertyDeclaration.Modifiers));
        }

        private static MethodDeclarationSyntax MakeAbstract(MethodDeclarationSyntax methodDeclaration)
        {
            return methodDeclaration
                .WithExpressionBody(null)
                .WithSemicolonToken(Token(SyntaxKind.None))
                .WithBody(null)
                .WithModifiers(CreateModifiers(methodDeclaration.Modifiers))
                .WithSemicolonToken(Token(SyntaxKind.SemicolonToken));
        }

        private static IndexerDeclarationSyntax MakeAbstract(IndexerDeclarationSyntax indexerDeclaration)
        {
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

            return indexerDeclaration
                .WithExpressionBody(null)
                .WithSemicolonToken(Token(SyntaxKind.None))
                .WithAccessorList(accessorList)
                .WithModifiers(CreateModifiers(indexerDeclaration.Modifiers));
        }

        private static SyntaxTokenList CreateModifiers(SyntaxTokenList modifiers)
        {
            modifiers = RemoveVirtualKeywordIfPresent(modifiers);

            modifiers = AddAbstractKeywordIfNotPresent(modifiers);

            if (!ModifierUtility.IsSorted(modifiers))
                modifiers = TokenList(modifiers.OrderBy(f => f, ModifierComparer.Instance));

            return modifiers;
        }

        private static SyntaxTokenList RemoveVirtualKeywordIfPresent(SyntaxTokenList modifiers)
        {
            int index = modifiers.IndexOf(SyntaxKind.VirtualKeyword);

            if (index != -1)
                return modifiers.RemoveAt(index);

            return modifiers;
        }

        private static SyntaxTokenList AddAbstractKeywordIfNotPresent(SyntaxTokenList modifiers)
        {
            if (!modifiers.Contains(SyntaxKind.AbstractKeyword))
                modifiers = modifiers.Add(Token(SyntaxKind.AbstractKeyword));

            return modifiers;
        }
    }
}
