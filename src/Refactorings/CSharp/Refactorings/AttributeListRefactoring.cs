// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Roslynator.CSharp.Refactorings
{
    internal static class AttributeListRefactoring
    {
        public static void ComputeRefactorings(RefactoringContext context, MemberDeclarationSyntax member)
        {
            if (context.IsAnyRefactoringEnabled(
                    RefactoringIdentifiers.SplitAttributes,
                    RefactoringIdentifiers.MergeAttributes)
                && !member.IsKind(SyntaxKind.NamespaceDeclaration)
                && SyntaxListSelection<AttributeListSyntax>.TryCreate(member.GetAttributeLists(), context.Span, out SyntaxListSelection<AttributeListSyntax> selectedAttributeLists))
            {
                if (context.IsRefactoringEnabled(RefactoringIdentifiers.SplitAttributes)
                    && selectedAttributeLists.Any(f => f.Attributes.Count > 1))
                {
                    context.RegisterRefactoring(
                        "Split attributes",
                        ct => SplitAsync(context.Document, member, selectedAttributeLists.ToArray(), ct));
                }

                if (context.IsRefactoringEnabled(RefactoringIdentifiers.MergeAttributes)
                    && selectedAttributeLists.Count > 1)
                {
                    context.RegisterRefactoring(
                        "Merge attributes",
                        ct => MergeAsync(context.Document, member, selectedAttributeLists.ToArray(), ct));
                }
            }
        }

        public static Task<Document> SplitAsync(
            Document document,
            MemberDeclarationSyntax member,
            AttributeListSyntax[] attributeLists,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            SyntaxList<AttributeListSyntax> lists = member.GetAttributeLists();

            var newLists = new List<AttributeListSyntax>();

            int index = lists.IndexOf(attributeLists[0]);

            for (int i = 0; i < index; i++)
                newLists.Add(lists[i]);

            newLists.AddRange(attributeLists.SelectMany(AttributeRefactoring.SplitAttributes));

            for (int i = index + attributeLists.Length; i < lists.Count; i++)
                newLists.Add(lists[i]);

            return document.ReplaceNodeAsync(
                member,
                member.WithAttributeLists(newLists.ToSyntaxList()),
                cancellationToken);
        }

        public static Task<Document> MergeAsync(
            Document document,
            MemberDeclarationSyntax member,
            AttributeListSyntax[] attributeLists,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            SyntaxList<AttributeListSyntax> lists = member.GetAttributeLists();

            var newLists = new List<AttributeListSyntax>(lists.Count - attributeLists.Length + 1);

            int index = lists.IndexOf(attributeLists[0]);

            for (int i = 0; i < index; i++)
                newLists.Add(lists[i]);

            newLists.Add(AttributeRefactoring.MergeAttributes(attributeLists));

            for (int i = index + attributeLists.Length; i < lists.Count; i++)
                newLists.Add(lists[i]);

            return document.ReplaceNodeAsync(
                member,
                member.WithAttributeLists(newLists.ToSyntaxList()),
                cancellationToken);
        }

        public static SyntaxList<AttributeListSyntax> GetAttributeLists(this SyntaxNode node)
        {
            if (node == null)
                throw new ArgumentNullException(nameof(node));

            switch (node.Kind())
            {
                case SyntaxKind.EnumDeclaration:
                    return ((EnumDeclarationSyntax)node).AttributeLists;
                case SyntaxKind.DelegateDeclaration:
                    return ((DelegateDeclarationSyntax)node).AttributeLists;
                case SyntaxKind.ClassDeclaration:
                    return ((ClassDeclarationSyntax)node).AttributeLists;
                case SyntaxKind.TypeParameter:
                    return ((TypeParameterSyntax)node).AttributeLists;
                case SyntaxKind.StructDeclaration:
                    return ((StructDeclarationSyntax)node).AttributeLists;
                case SyntaxKind.PropertyDeclaration:
                    return ((PropertyDeclarationSyntax)node).AttributeLists;
                case SyntaxKind.Parameter:
                    return ((ParameterSyntax)node).AttributeLists;
                case SyntaxKind.OperatorDeclaration:
                    return ((OperatorDeclarationSyntax)node).AttributeLists;
                case SyntaxKind.MethodDeclaration:
                    return ((MethodDeclarationSyntax)node).AttributeLists;
                case SyntaxKind.InterfaceDeclaration:
                    return ((InterfaceDeclarationSyntax)node).AttributeLists;
                case SyntaxKind.IndexerDeclaration:
                    return ((IndexerDeclarationSyntax)node).AttributeLists;
                case SyntaxKind.FieldDeclaration:
                    return ((FieldDeclarationSyntax)node).AttributeLists;
                case SyntaxKind.EventFieldDeclaration:
                    return ((EventFieldDeclarationSyntax)node).AttributeLists;
                case SyntaxKind.EventDeclaration:
                    return ((EventDeclarationSyntax)node).AttributeLists;
                case SyntaxKind.EnumMemberDeclaration:
                    return ((EnumMemberDeclarationSyntax)node).AttributeLists;
                case SyntaxKind.DestructorDeclaration:
                    return ((DestructorDeclarationSyntax)node).AttributeLists;
                case SyntaxKind.ConversionOperatorDeclaration:
                    return ((ConversionOperatorDeclarationSyntax)node).AttributeLists;
                case SyntaxKind.ConstructorDeclaration:
                    return ((ConstructorDeclarationSyntax)node).AttributeLists;
                case SyntaxKind.IncompleteMember:
                    return ((IncompleteMemberSyntax)node).AttributeLists;
                case SyntaxKind.GetAccessorDeclaration:
                case SyntaxKind.SetAccessorDeclaration:
                case SyntaxKind.AddAccessorDeclaration:
                case SyntaxKind.RemoveAccessorDeclaration:
                    return ((AccessorDeclarationSyntax)node).AttributeLists;
                default:
                    {
                        Debug.Assert(node.Kind() == SyntaxKind.GlobalStatement, node.Kind().ToString());
                        return default(SyntaxList<AttributeListSyntax>);
                    }
            }
        }

        public static CSharpSyntaxNode WithAttributeLists(this CSharpSyntaxNode node, SyntaxList<AttributeListSyntax> attributeLists)
        {
            if (node == null)
                throw new ArgumentNullException(nameof(node));

            switch (node.Kind())
            {
                case SyntaxKind.EnumDeclaration:
                    return ((EnumDeclarationSyntax)node).WithAttributeLists(attributeLists);
                case SyntaxKind.DelegateDeclaration:
                    return ((DelegateDeclarationSyntax)node).WithAttributeLists(attributeLists);
                case SyntaxKind.ClassDeclaration:
                    return ((ClassDeclarationSyntax)node).WithAttributeLists(attributeLists);
                case SyntaxKind.TypeParameter:
                    return ((TypeParameterSyntax)node).WithAttributeLists(attributeLists);
                case SyntaxKind.StructDeclaration:
                    return ((StructDeclarationSyntax)node).WithAttributeLists(attributeLists);
                case SyntaxKind.PropertyDeclaration:
                    return ((PropertyDeclarationSyntax)node).WithAttributeLists(attributeLists);
                case SyntaxKind.Parameter:
                    return ((ParameterSyntax)node).WithAttributeLists(attributeLists);
                case SyntaxKind.OperatorDeclaration:
                    return ((OperatorDeclarationSyntax)node).WithAttributeLists(attributeLists);
                case SyntaxKind.MethodDeclaration:
                    return ((MethodDeclarationSyntax)node).WithAttributeLists(attributeLists);
                case SyntaxKind.InterfaceDeclaration:
                    return ((InterfaceDeclarationSyntax)node).WithAttributeLists(attributeLists);
                case SyntaxKind.IndexerDeclaration:
                    return ((IndexerDeclarationSyntax)node).WithAttributeLists(attributeLists);
                case SyntaxKind.FieldDeclaration:
                    return ((FieldDeclarationSyntax)node).WithAttributeLists(attributeLists);
                case SyntaxKind.EventFieldDeclaration:
                    return ((EventFieldDeclarationSyntax)node).WithAttributeLists(attributeLists);
                case SyntaxKind.EventDeclaration:
                    return ((EventDeclarationSyntax)node).WithAttributeLists(attributeLists);
                case SyntaxKind.EnumMemberDeclaration:
                    return ((EnumMemberDeclarationSyntax)node).WithAttributeLists(attributeLists);
                case SyntaxKind.DestructorDeclaration:
                    return ((DestructorDeclarationSyntax)node).WithAttributeLists(attributeLists);
                case SyntaxKind.ConversionOperatorDeclaration:
                    return ((ConversionOperatorDeclarationSyntax)node).WithAttributeLists(attributeLists);
                case SyntaxKind.ConstructorDeclaration:
                    return ((ConstructorDeclarationSyntax)node).WithAttributeLists(attributeLists);
                case SyntaxKind.IncompleteMember:
                    return ((IncompleteMemberSyntax)node).WithAttributeLists(attributeLists);
                case SyntaxKind.GetAccessorDeclaration:
                case SyntaxKind.SetAccessorDeclaration:
                case SyntaxKind.AddAccessorDeclaration:
                case SyntaxKind.RemoveAccessorDeclaration:
                    return ((AccessorDeclarationSyntax)node).WithAttributeLists(attributeLists);
                default:
                    {
                        Debug.Fail(node.Kind().ToString());
                        return node;
                    }
            }
        }
    }
}