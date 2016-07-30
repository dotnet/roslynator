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
#if DEBUG
    internal static class ReorderMembersRefactoring
    {
        public static void ComputeRefactorings(RefactoringContext context, MemberDeclarationSyntax memberDeclaration)
        {
            switch (memberDeclaration.Kind())
            {
                case SyntaxKind.ClassDeclaration:
                    {
                        var classDeclaration = (ClassDeclarationSyntax)memberDeclaration;
                        if (classDeclaration.Identifier.Span.Contains(context.Span))
                        {
                            context.RegisterRefactoring(
                                "Reorder class members",
                                cancellationToken => ReorderClassMembersAsync(context.Document, classDeclaration, cancellationToken));
                        }

                        break;
                    }
                case SyntaxKind.StructDeclaration:
                    {
                        var structDeclaration = (StructDeclarationSyntax)memberDeclaration;
                        if (structDeclaration.Identifier.Span.Contains(context.Span))
                        {
                            context.RegisterRefactoring(
                                "Reorder struct members",
                                cancellationToken => ReorderStructMembersAsync(context.Document, structDeclaration, cancellationToken));
                        }

                        break;
                    }
                case SyntaxKind.InterfaceDeclaration:
                    {
                        var interfaceDeclaration = (InterfaceDeclarationSyntax)memberDeclaration;
                        if (interfaceDeclaration.Identifier.Span.Contains(context.Span))
                        {
                            context.RegisterRefactoring(
                                "Reorder interface members",
                                cancellationToken => ReorderInterfaceMembersAsync(context.Document, interfaceDeclaration, cancellationToken));
                        }

                        break;
                    }
            }
        }

        private static async Task<Document> ReorderClassMembersAsync(
            Document document,
            ClassDeclarationSyntax classDeclaration,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            SyntaxNode oldRoot = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);

            ClassDeclarationSyntax newDeclaration = classDeclaration
                .WithMembers(ProcessMemberDeclarations(classDeclaration.Members))
                .WithFormatterAnnotation();

            SyntaxNode newRoot = oldRoot.ReplaceNode(classDeclaration, newDeclaration);

            return document.WithSyntaxRoot(newRoot);
        }

        private static async Task<Document> ReorderStructMembersAsync(
            Document document,
            StructDeclarationSyntax structDeclaration,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            SyntaxNode oldRoot = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);

            StructDeclarationSyntax newDeclaration = structDeclaration
                .WithMembers(ProcessMemberDeclarations(structDeclaration.Members))
                .WithFormatterAnnotation();

            SyntaxNode newRoot = oldRoot.ReplaceNode(structDeclaration, newDeclaration);

            return document.WithSyntaxRoot(newRoot);
        }

        private static async Task<Document> ReorderInterfaceMembersAsync(
            Document document,
            InterfaceDeclarationSyntax interfaceDeclaration,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            SyntaxNode oldRoot = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);

            InterfaceDeclarationSyntax newDeclaration = interfaceDeclaration
                .WithMembers(ProcessMemberDeclarations(interfaceDeclaration.Members))
                .WithFormatterAnnotation();

            SyntaxNode newRoot = oldRoot.ReplaceNode(interfaceDeclaration, newDeclaration);

            return document.WithSyntaxRoot(newRoot);
        }

        private static SyntaxList<MemberDeclarationSyntax> ProcessMemberDeclarations(IEnumerable<MemberDeclarationSyntax> memberDeclarations)
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

            private const string ConstantName = "";

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
#endif
}