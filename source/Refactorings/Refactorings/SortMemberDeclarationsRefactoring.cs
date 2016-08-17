// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Pihrtsoft.CodeAnalysis.CSharp.Refactorings
{
    internal static class SortMemberDeclarationsRefactoring
    {
        public static void ComputeRefactorings(RefactoringContext context, MemberDeclarationSyntax memberDeclaration)
        {
            if (memberDeclaration.IsKind(SyntaxKind.NamespaceDeclaration))
            {
                if (((NamespaceDeclarationSyntax)memberDeclaration).Name.Span.Contains(context.Span))
                    RegisterRefactoring(context, memberDeclaration);
            }
            else if (memberDeclaration.IsKind(SyntaxKind.ClassDeclaration))
            {
                if (((ClassDeclarationSyntax)memberDeclaration).Identifier.Span.Contains(context.Span))
                    RegisterRefactoring(context, memberDeclaration);
            }
            else if (memberDeclaration.IsKind(SyntaxKind.StructDeclaration))
            {
                if (((StructDeclarationSyntax)memberDeclaration).Identifier.Span.Contains(context.Span))
                    RegisterRefactoring(context, memberDeclaration);
            }
            else if (memberDeclaration.IsKind(SyntaxKind.InterfaceDeclaration))
            {
                if (((InterfaceDeclarationSyntax)memberDeclaration).Identifier.Span.Contains(context.Span))
                    RegisterRefactoring(context, memberDeclaration);
            }
        }

        private static void RegisterRefactoring(RefactoringContext context, MemberDeclarationSyntax memberDeclaration)
        {
            context.RegisterRefactoring(
                "Sort declarations",
                cancellationToken => SortMembersAsync(context.Document, memberDeclaration, cancellationToken));
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
                .OrderBy(f => f, MemberDeclarationComparer.Instance);

            return SyntaxFactory.List(newMembers);
        }

        private class MemberDeclarationComparer : IComparer<MemberDeclarationSyntax>
        {
            public static readonly MemberDeclarationComparer Instance = new MemberDeclarationComparer();

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