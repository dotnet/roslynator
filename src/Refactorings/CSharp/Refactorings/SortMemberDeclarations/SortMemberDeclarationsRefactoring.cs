﻿// Copyright (c) .NET Foundation and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.CSharp.Syntax;

namespace Roslynator.CSharp.Refactorings.SortMemberDeclarations;

internal static class SortMemberDeclarationsRefactoring
{
    public static void ComputeRefactoring(RefactoringContext context, NamespaceDeclarationSyntax namespaceDeclaration)
    {
        if (MemberDeclarationListSelection.TryCreate(namespaceDeclaration, context.Span, out MemberDeclarationListSelection selectedMembers))
            ComputeRefactoring(context, selectedMembers);
    }

    public static void ComputeRefactoring(RefactoringContext context, ClassDeclarationSyntax classDeclaration)
    {
        if (MemberDeclarationListSelection.TryCreate(classDeclaration, context.Span, out MemberDeclarationListSelection selectedMembers))
            ComputeRefactoring(context, selectedMembers);
    }

    public static void ComputeRefactoring(RefactoringContext context, RecordDeclarationSyntax recordDeclaration)
    {
        if (MemberDeclarationListSelection.TryCreate(recordDeclaration, context.Span, out MemberDeclarationListSelection selectedMembers))
            ComputeRefactoring(context, selectedMembers);
    }

    public static void ComputeRefactoring(RefactoringContext context, StructDeclarationSyntax structDeclaration)
    {
        if (MemberDeclarationListSelection.TryCreate(structDeclaration, context.Span, out MemberDeclarationListSelection selectedMembers))
            ComputeRefactoring(context, selectedMembers);
    }

    public static void ComputeRefactoring(RefactoringContext context, InterfaceDeclarationSyntax interfaceDeclaration)
    {
        if (MemberDeclarationListSelection.TryCreate(interfaceDeclaration, context.Span, out MemberDeclarationListSelection selectedMembers))
            ComputeRefactoring(context, selectedMembers);
    }

    private static void ComputeRefactoring(RefactoringContext context, MemberDeclarationListSelection selectedMembers)
    {
        if (selectedMembers.Count <= 1)
            return;

        SyntaxKind kind = GetSingleKindOrDefault(selectedMembers);

        if (kind != SyntaxKind.None)
        {
            if (MemberDeclarationComparer.CanBeSortedByName(kind))
            {
                ComputeRefactoring(
                    context,
                    MemberDeclarationComparer.ByKindThenByName,
                    "Sort members by name",
                    selectedMembers);
            }
        }
        else
        {
            ComputeRefactoring(
                context,
                MemberDeclarationComparer.ByKind,
                "Sort members by kind",
                selectedMembers);

            ComputeRefactoring(
                context,
                MemberDeclarationComparer.ByKindThenByName,
                "Sort members by kind then by name",
                selectedMembers);
        }
    }

    private static void ComputeRefactoring(
        RefactoringContext context,
        MemberDeclarationComparer comparer,
        string title,
        MemberDeclarationListSelection selectedMembers)
    {
        if (selectedMembers.IsSorted(comparer))
            return;

        context.RegisterRefactoring(
            title,
            ct => RefactorAsync(context.Document, selectedMembers, comparer, ct),
            RefactoringDescriptors.SortMemberDeclarations);
    }

    private static Task<Document> RefactorAsync(
        Document document,
        MemberDeclarationListSelection selectedMembers,
        MemberDeclarationComparer comparer,
        CancellationToken cancellationToken)
    {
        IEnumerable<MemberDeclarationSyntax> sorted = selectedMembers.OrderBy(f => f, comparer);

        MemberDeclarationListInfo info = SyntaxInfo.MemberDeclarationListInfo(selectedMembers);

        SyntaxList<MemberDeclarationSyntax> newMembers = info
            .Members
            .ReplaceRange(selectedMembers.FirstIndex, selectedMembers.Count, sorted);

        return document.ReplaceMembersAsync(info, newMembers, cancellationToken);
    }

    private static SyntaxKind GetSingleKindOrDefault(MemberDeclarationListSelection members)
    {
        SyntaxKind kind = members[0].Kind();

        for (int i = 1; i < members.Count; i++)
        {
            if (members[i].Kind() != kind)
                return SyntaxKind.None;
        }

        return kind;
    }
}
