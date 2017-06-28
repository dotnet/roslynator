// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.CSharp.Comparers;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static Roslynator.CSharp.CSharpFactory;

namespace Roslynator.CSharp.Refactorings
{
    internal static class MakeMemberAbstractRefactoring
    {
        public static bool CanRefactor(PropertyDeclarationSyntax propertyDeclaration)
        {
            SyntaxTokenList modifiers = propertyDeclaration.Modifiers;

            return !modifiers.Contains(SyntaxKind.AbstractKeyword)
                && !modifiers.Contains(SyntaxKind.StaticKeyword)
                && IsAbstractClass(propertyDeclaration.Parent);
        }

        public static bool CanRefactor(MethodDeclarationSyntax methodDeclaration)
        {
            SyntaxTokenList modifiers = methodDeclaration.Modifiers;

            return !modifiers.Contains(SyntaxKind.AbstractKeyword)
                && !modifiers.Contains(SyntaxKind.StaticKeyword)
                && IsAbstractClass(methodDeclaration.Parent);
        }

        public static bool CanRefactor(IndexerDeclarationSyntax indexerDeclaration)
        {
            SyntaxTokenList modifiers = indexerDeclaration.Modifiers;

            return !modifiers.Contains(SyntaxKind.AbstractKeyword)
                && !modifiers.Contains(SyntaxKind.StaticKeyword)
                && IsAbstractClass(indexerDeclaration.Parent);
        }

        private static bool IsAbstractClass(SyntaxNode node)
        {
            return node?.IsKind(SyntaxKind.ClassDeclaration) == true
                && ((ClassDeclarationSyntax)node).Modifiers.Contains(SyntaxKind.AbstractKeyword);
        }

        public static Task<Document> RefactorAsync(
            Document document,
            MemberDeclarationSyntax memberDeclaration,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            MemberDeclarationSyntax newMemberDeclaration = MakeAbstract(memberDeclaration)
                .WithTriviaFrom(memberDeclaration)
                .WithFormatterAnnotation();

            return document.ReplaceNodeAsync(memberDeclaration, newMemberDeclaration, cancellationToken);
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
                    .AddAccessors(AutoGetAccessorDeclaration());
            }
            else
            {
                AccessorDeclarationSyntax getter = propertyDeclaration.Getter();
                if (getter != null)
                {
                    accessorList = accessorList.AddAccessors(getter
                       .WithBody(null)
                       .WithSemicolonToken(SemicolonToken()));
                }

                AccessorDeclarationSyntax setter = propertyDeclaration.Setter();
                if (setter != null)
                {
                    accessorList = accessorList.AddAccessors(setter
                       .WithBody(null)
                       .WithSemicolonToken(SemicolonToken()));
                }
            }

            propertyDeclaration = propertyDeclaration
                .WithExpressionBody(null)
                .WithSemicolonToken(default(SyntaxToken))
                .WithAccessorList(accessorList);

            return UpdateModifiers(propertyDeclaration, propertyDeclaration.Modifiers);
        }

        private static MethodDeclarationSyntax MakeAbstract(MethodDeclarationSyntax methodDeclaration)
        {
            methodDeclaration = methodDeclaration
                .WithExpressionBody(null)
                .WithSemicolonToken(default(SyntaxToken))
                .WithBody(null)
                .WithSemicolonToken(SemicolonToken());

            return UpdateModifiers(methodDeclaration, methodDeclaration.Modifiers);
        }

        private static IndexerDeclarationSyntax MakeAbstract(IndexerDeclarationSyntax indexerDeclaration)
        {
            AccessorListSyntax accessorList = AccessorList();

            if (indexerDeclaration.ExpressionBody != null)
            {
                accessorList = accessorList
                    .AddAccessors(
                        AutoGetAccessorDeclaration());
            }
            else
            {
                AccessorDeclarationSyntax getter = indexerDeclaration.Getter();
                if (getter != null)
                {
                    accessorList = accessorList.AddAccessors(getter
                       .WithBody(null)
                       .WithSemicolonToken(SemicolonToken()));
                }

                AccessorDeclarationSyntax setter = indexerDeclaration.Setter();
                if (setter != null)
                {
                    accessorList = accessorList.AddAccessors(setter
                       .WithBody(null)
                       .WithSemicolonToken(SemicolonToken()));
                }
            }

            indexerDeclaration = indexerDeclaration
                .WithExpressionBody(null)
                .WithSemicolonToken(default(SyntaxToken))
                .WithAccessorList(accessorList);

            return UpdateModifiers(indexerDeclaration, indexerDeclaration.Modifiers);
        }

        private static TNode UpdateModifiers<TNode>(TNode node, SyntaxTokenList modifiers) where TNode : SyntaxNode
        {
            if (!modifiers.Contains(SyntaxKind.AbstractKeyword))
            {
                modifiers = modifiers.InsertModifier(SyntaxKind.AbstractKeyword, ModifierComparer.Instance);

                node = (TNode)node.WithModifiers(modifiers);
            }

            return node.RemoveModifier(SyntaxKind.VirtualKeyword);
        }
    }
}
