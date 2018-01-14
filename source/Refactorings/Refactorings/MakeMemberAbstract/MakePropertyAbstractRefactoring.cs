// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.CSharp.Comparers;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static Roslynator.CSharp.CSharpFactory;

namespace Roslynator.CSharp.Refactorings.MakeMemberAbstract
{
    internal static class MakePropertyAbstractRefactoring
    {
        public static void ComputeRefactoring(RefactoringContext context, PropertyDeclarationSyntax propertyDeclaration)
        {
            if (!CanRefactor(propertyDeclaration))
                return;

            context.RegisterRefactoring(
                "Make property abstract",
                cancellationToken => RefactorAsync(context.Document, propertyDeclaration, cancellationToken));
        }

        public static bool CanRefactor(PropertyDeclarationSyntax propertyDeclaration)
        {
            SyntaxTokenList modifiers = propertyDeclaration.Modifiers;

            return !modifiers.ContainsAny(SyntaxKind.AbstractKeyword, SyntaxKind.StaticKeyword)
                && (propertyDeclaration.Parent as ClassDeclarationSyntax)?.Modifiers.Contains(SyntaxKind.AbstractKeyword) == true;
        }

        public static Task<Document> RefactorAsync(
            Document document,
            PropertyDeclarationSyntax propertyDeclaration,
            CancellationToken cancellationToken = default(CancellationToken))
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
                    if (getter.Modifiers.GetAccessibility() == Accessibility.Private)
                        getter = getter.WithAccessibility(Accessibility.Protected);

                    accessorList = accessorList.AddAccessors(getter
                       .WithBody(null)
                       .WithSemicolonToken(SemicolonToken()));
                }

                AccessorDeclarationSyntax setter = propertyDeclaration.Setter();
                if (setter != null)
                {
                    if (setter.Modifiers.GetAccessibility() == Accessibility.Private)
                        setter = setter.WithAccessibility(Accessibility.Protected);

                    accessorList = accessorList.AddAccessors(setter
                       .WithBody(null)
                       .WithSemicolonToken(SemicolonToken()));
                }
            }

            PropertyDeclarationSyntax newNode = propertyDeclaration
                .WithExpressionBody(null)
                .WithSemicolonToken(default(SyntaxToken))
                .WithAccessorList(accessorList)
                .InsertModifier(SyntaxKind.AbstractKeyword, ModifierComparer.Instance)
                .RemoveModifier(SyntaxKind.VirtualKeyword)
                .WithTriviaFrom(propertyDeclaration)
                .WithFormatterAnnotation();

            return document.ReplaceNodeAsync(propertyDeclaration, newNode, cancellationToken);
        }
    }
}
