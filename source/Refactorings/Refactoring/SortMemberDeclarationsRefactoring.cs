// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Pihrtsoft.CodeAnalysis.CSharp.Refactoring
{
    internal static class SortMemberDeclarationsRefactoring
    {
        public static void ComputeRefactorings(RefactoringContext context, MemberDeclarationSyntax memberDeclaration)
        {
            switch (memberDeclaration.Kind())
            {
                case SyntaxKind.NamespaceDeclaration:
                case SyntaxKind.ClassDeclaration:
                case SyntaxKind.StructDeclaration:
                case SyntaxKind.InterfaceDeclaration:
                    {
                        context.RegisterRefactoring(
                            "Sort declarations",
                            cancellationToken => SortMembersAsync(context.Document, memberDeclaration, cancellationToken));

                        break;
                    }
            }
        }

        private static async Task<Document> SortMembersAsync(
            Document document,
            MemberDeclarationSyntax memberDeclaration,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            SyntaxNode root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);

            MemberDeclarationSyntax newNode = memberDeclaration
                .SetMembers(SortMembers(memberDeclaration.GetMembers()))
                .WithFormatterAnnotation();

            root = root.ReplaceNode(memberDeclaration, newNode);

            return document.WithSyntaxRoot(root);
        }

        private static SyntaxList<MemberDeclarationSyntax> SortMembers(IEnumerable<MemberDeclarationSyntax> memberDeclarations)
        {
            IEnumerable<MemberDeclarationSyntax> newMembers = memberDeclarations
                .OrderBy(f => f, MemberDeclarationSorter.Instance);

            return SyntaxFactory.List(newMembers);
        }

        private class MemberDeclarationSorter : IComparer<MemberDeclarationSyntax>
        {
            public static readonly MemberDeclarationSorter Instance = new MemberDeclarationSorter();

            public int Compare(MemberDeclarationSyntax x, MemberDeclarationSyntax y)
            {
                if (object.ReferenceEquals(x, y))
                    return 0;

                if (x == null)
                    return -1;

                if (y == null)
                    return 1;

                return GetOrderIndex(x).CompareTo(GetOrderIndex(y));
            }

            private static int GetOrderIndex(SyntaxNode syntaxNode)
            {
                switch (syntaxNode.Kind())
                {
                    case SyntaxKind.FieldDeclaration:
                        {
                            var fieldDeclaration = (FieldDeclarationSyntax)syntaxNode;
                            if (fieldDeclaration.Modifiers.Contains(SyntaxKind.ConstKeyword))
                                return 0;
                            else
                                return 1;
                        }
                    case SyntaxKind.ConstructorDeclaration:
                        return 2;
                    case SyntaxKind.DestructorDeclaration:
                        return 3;
                    case SyntaxKind.DelegateDeclaration:
                        return 4;
                    case SyntaxKind.EventDeclaration:
                        return 5;
                    case SyntaxKind.EventFieldDeclaration:
                        return 6;
                    case SyntaxKind.PropertyDeclaration:
                        return 7;
                    case SyntaxKind.IndexerDeclaration:
                        return 8;
                    case SyntaxKind.MethodDeclaration:
                        return 9;
                    case SyntaxKind.ConversionOperatorDeclaration:
                        return 10;
                    case SyntaxKind.OperatorDeclaration:
                        return 11;
                    case SyntaxKind.EnumDeclaration:
                        return 12;
                    case SyntaxKind.InterfaceDeclaration:
                        return 13;
                    case SyntaxKind.StructDeclaration:
                        return 14;
                    case SyntaxKind.ClassDeclaration:
                        return 15;
                    case SyntaxKind.NamespaceDeclaration:
                        return 16;
                    case SyntaxKind.IncompleteMember:
                        return 17;
                    default:
                        {
                            Debug.Assert(false, $"unknown member '{syntaxNode.Kind()}'");
                            return 18;
                        }
                }
            }
        }
    }
}