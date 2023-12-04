﻿// Copyright (c) .NET Foundation and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using Roslynator.CSharp.SyntaxRewriters;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static Roslynator.CSharp.CSharpFactory;

namespace Roslynator.CSharp;

internal static class SyntaxRefactorings
{
    public static SyntaxRemoveOptions DefaultRemoveOptions
    {
        get { return SyntaxRemoveOptions.KeepExteriorTrivia | SyntaxRemoveOptions.KeepUnbalancedDirectives; }
    }

    public static T AddAttributeLists<T>(
        T node,
        params AttributeListSyntax[] attributeLists) where T : SyntaxNode
    {
        return AddAttributeLists(node, keepDocumentationCommentOnTop: false, attributeLists);
    }

    public static T AddAttributeLists<T>(
        T node,
        bool keepDocumentationCommentOnTop,
        params AttributeListSyntax[] attributeLists) where T : SyntaxNode
    {
        if (node is null)
            throw new ArgumentNullException(nameof(node));

        if (attributeLists is null)
            throw new ArgumentNullException(nameof(attributeLists));

        switch (node.Kind())
        {
            case SyntaxKind.EnumDeclaration:
                return (T)(SyntaxNode)AddAttributeLists((EnumDeclarationSyntax)(SyntaxNode)node, keepDocumentationCommentOnTop, attributeLists, f => f.AttributeLists.Any(), (f, g) => f.WithAttributeLists(g), (f, g) => f.AddAttributeLists(g));
            case SyntaxKind.DelegateDeclaration:
                return (T)(SyntaxNode)AddAttributeLists((DelegateDeclarationSyntax)(SyntaxNode)node, keepDocumentationCommentOnTop, attributeLists, f => f.AttributeLists.Any(), (f, g) => f.WithAttributeLists(g), (f, g) => f.AddAttributeLists(g));
            case SyntaxKind.ClassDeclaration:
                return (T)(SyntaxNode)AddAttributeLists((ClassDeclarationSyntax)(SyntaxNode)node, keepDocumentationCommentOnTop, attributeLists, f => f.AttributeLists.Any(), (f, g) => f.WithAttributeLists(g), (f, g) => f.AddAttributeLists(g));
            case SyntaxKind.TypeParameter:
                return (T)(SyntaxNode)AddAttributeLists((TypeParameterSyntax)(SyntaxNode)node, keepDocumentationCommentOnTop, attributeLists, f => f.AttributeLists.Any(), (f, g) => f.WithAttributeLists(g), (f, g) => f.AddAttributeLists(g));
            case SyntaxKind.RecordDeclaration:
            case SyntaxKind.RecordStructDeclaration:
                return (T)(SyntaxNode)AddAttributeLists((RecordDeclarationSyntax)(SyntaxNode)node, keepDocumentationCommentOnTop, attributeLists, f => f.AttributeLists.Any(), (f, g) => f.WithAttributeLists(g), (f, g) => f.AddAttributeLists(g));
            case SyntaxKind.StructDeclaration:
                return (T)(SyntaxNode)AddAttributeLists((StructDeclarationSyntax)(SyntaxNode)node, keepDocumentationCommentOnTop, attributeLists, f => f.AttributeLists.Any(), (f, g) => f.WithAttributeLists(g), (f, g) => f.AddAttributeLists(g));
            case SyntaxKind.PropertyDeclaration:
                return (T)(SyntaxNode)AddAttributeLists((PropertyDeclarationSyntax)(SyntaxNode)node, keepDocumentationCommentOnTop, attributeLists, f => f.AttributeLists.Any(), (f, g) => f.WithAttributeLists(g), (f, g) => f.AddAttributeLists(g));
            case SyntaxKind.Parameter:
                return (T)(SyntaxNode)AddAttributeLists((ParameterSyntax)(SyntaxNode)node, keepDocumentationCommentOnTop, attributeLists, f => f.AttributeLists.Any(), (f, g) => f.WithAttributeLists(g), (f, g) => f.AddAttributeLists(g));
            case SyntaxKind.OperatorDeclaration:
                return (T)(SyntaxNode)AddAttributeLists((OperatorDeclarationSyntax)(SyntaxNode)node, keepDocumentationCommentOnTop, attributeLists, f => f.AttributeLists.Any(), (f, g) => f.WithAttributeLists(g), (f, g) => f.AddAttributeLists(g));
            case SyntaxKind.MethodDeclaration:
                return (T)(SyntaxNode)AddAttributeLists((MethodDeclarationSyntax)(SyntaxNode)node, keepDocumentationCommentOnTop, attributeLists, f => f.AttributeLists.Any(), (f, g) => f.WithAttributeLists(g), (f, g) => f.AddAttributeLists(g));
            case SyntaxKind.InterfaceDeclaration:
                return (T)(SyntaxNode)AddAttributeLists((InterfaceDeclarationSyntax)(SyntaxNode)node, keepDocumentationCommentOnTop, attributeLists, f => f.AttributeLists.Any(), (f, g) => f.WithAttributeLists(g), (f, g) => f.AddAttributeLists(g));
            case SyntaxKind.IndexerDeclaration:
                return (T)(SyntaxNode)AddAttributeLists((IndexerDeclarationSyntax)(SyntaxNode)node, keepDocumentationCommentOnTop, attributeLists, f => f.AttributeLists.Any(), (f, g) => f.WithAttributeLists(g), (f, g) => f.AddAttributeLists(g));
            case SyntaxKind.FieldDeclaration:
                return (T)(SyntaxNode)AddAttributeLists((FieldDeclarationSyntax)(SyntaxNode)node, keepDocumentationCommentOnTop, attributeLists, f => f.AttributeLists.Any(), (f, g) => f.WithAttributeLists(g), (f, g) => f.AddAttributeLists(g));
            case SyntaxKind.EventFieldDeclaration:
                return (T)(SyntaxNode)AddAttributeLists((EventFieldDeclarationSyntax)(SyntaxNode)node, keepDocumentationCommentOnTop, attributeLists, f => f.AttributeLists.Any(), (f, g) => f.WithAttributeLists(g), (f, g) => f.AddAttributeLists(g));
            case SyntaxKind.EventDeclaration:
                return (T)(SyntaxNode)AddAttributeLists((EventDeclarationSyntax)(SyntaxNode)node, keepDocumentationCommentOnTop, attributeLists, f => f.AttributeLists.Any(), (f, g) => f.WithAttributeLists(g), (f, g) => f.AddAttributeLists(g));
            case SyntaxKind.EnumMemberDeclaration:
                return (T)(SyntaxNode)AddAttributeLists((EnumMemberDeclarationSyntax)(SyntaxNode)node, keepDocumentationCommentOnTop, attributeLists, f => f.AttributeLists.Any(), (f, g) => f.WithAttributeLists(g), (f, g) => f.AddAttributeLists(g));
            case SyntaxKind.DestructorDeclaration:
                return (T)(SyntaxNode)AddAttributeLists((DestructorDeclarationSyntax)(SyntaxNode)node, keepDocumentationCommentOnTop, attributeLists, f => f.AttributeLists.Any(), (f, g) => f.WithAttributeLists(g), (f, g) => f.AddAttributeLists(g));
            case SyntaxKind.ConversionOperatorDeclaration:
                return (T)(SyntaxNode)AddAttributeLists((ConversionOperatorDeclarationSyntax)(SyntaxNode)node, keepDocumentationCommentOnTop, attributeLists, f => f.AttributeLists.Any(), (f, g) => f.WithAttributeLists(g), (f, g) => f.AddAttributeLists(g));
            case SyntaxKind.ConstructorDeclaration:
                return (T)(SyntaxNode)AddAttributeLists((ConstructorDeclarationSyntax)(SyntaxNode)node, keepDocumentationCommentOnTop, attributeLists, f => f.AttributeLists.Any(), (f, g) => f.WithAttributeLists(g), (f, g) => f.AddAttributeLists(g));
            case SyntaxKind.IncompleteMember:
                return (T)(SyntaxNode)AddAttributeLists((IncompleteMemberSyntax)(SyntaxNode)node, keepDocumentationCommentOnTop, attributeLists, f => f.AttributeLists.Any(), (f, g) => f.WithAttributeLists(g), (f, g) => f.AddAttributeLists(g));
            case SyntaxKind.GetAccessorDeclaration:
            case SyntaxKind.SetAccessorDeclaration:
            case SyntaxKind.AddAccessorDeclaration:
            case SyntaxKind.RemoveAccessorDeclaration:
                return (T)(SyntaxNode)AddAttributeLists((AccessorDeclarationSyntax)(SyntaxNode)node, keepDocumentationCommentOnTop, attributeLists, f => f.AttributeLists.Any(), (f, g) => f.WithAttributeLists(g), (f, g) => f.AddAttributeLists(g));
            default:
                throw new ArgumentException($"Cannot add attribute list to '{node.Kind()}'.", nameof(node));
        }
    }

    private static T AddAttributeLists<T>(
        this T node,
        bool keepDocumentationCommentOnTop,
        AttributeListSyntax[] attributeLists,
        Func<T, bool> hasAttributeLists,
        Func<T, SyntaxList<AttributeListSyntax>, T> withAttributeLists,
        Func<T, AttributeListSyntax[], T> addAttributeLists) where T : SyntaxNode
    {
        if (keepDocumentationCommentOnTop
            && !hasAttributeLists(node)
            && attributeLists.Length > 0)
        {
            SyntaxTriviaList leadingTrivia = node.GetLeadingTrivia();

            for (int i = 0; i < leadingTrivia.Count; i++)
            {
                if (leadingTrivia[i].IsDocumentationCommentTrivia())
                {
                    attributeLists[0] = attributeLists[0].PrependToLeadingTrivia(leadingTrivia.Take(i + 1));

                    node = node.WithLeadingTrivia(leadingTrivia.Skip(i + 1));

                    return withAttributeLists(node, List(attributeLists));
                }
            }
        }

        return addAttributeLists(node, attributeLists);
    }

    public static TRoot? RemoveNode<TRoot>(TRoot root, SyntaxNode node) where TRoot : SyntaxNode
    {
        return root.RemoveNode(node, GetRemoveOptions(node));
    }

    public static SyntaxRemoveOptions GetRemoveOptions(SyntaxNode node)
    {
        SyntaxRemoveOptions removeOptions = DefaultRemoveOptions;

        if (node.GetLeadingTrivia().IsEmptyOrWhitespace())
            removeOptions &= ~SyntaxRemoveOptions.KeepLeadingTrivia;

        if (node.GetTrailingTrivia().IsEmptyOrWhitespace())
            removeOptions &= ~SyntaxRemoveOptions.KeepTrailingTrivia;

        return removeOptions;
    }

    public static SyntaxRemoveOptions GetRemoveOptions(CSharpSyntaxNode node)
    {
        SyntaxRemoveOptions removeOptions = DefaultRemoveOptions;

        if (node.GetLeadingTrivia().IsEmptyOrWhitespace())
            removeOptions &= ~SyntaxRemoveOptions.KeepLeadingTrivia;

        if (node.GetTrailingTrivia().IsEmptyOrWhitespace())
            removeOptions &= ~SyntaxRemoveOptions.KeepTrailingTrivia;

        return removeOptions;
    }

    internal static TMemberDeclaration RemoveSingleLineDocumentationComment<TMemberDeclaration>(TMemberDeclaration declaration) where TMemberDeclaration : MemberDeclarationSyntax
    {
        if (declaration is null)
            throw new ArgumentNullException(nameof(declaration));

        SyntaxTriviaList leadingTrivia = declaration.GetLeadingTrivia();

        SyntaxTriviaList.Reversed.Enumerator en = leadingTrivia.Reverse().GetEnumerator();

        int i = 0;
        while (en.MoveNext())
        {
            SyntaxKind kind = en.Current.Kind();

            if (kind == SyntaxKind.WhitespaceTrivia
                || kind == SyntaxKind.EndOfLineTrivia)
            {
                i++;
            }
            else if (kind == SyntaxKind.SingleLineDocumentationCommentTrivia)
            {
                return declaration.WithLeadingTrivia(leadingTrivia.Take(leadingTrivia.Count - (i + 1)));
            }
            else
            {
                return declaration;
            }
        }

        return declaration;
    }

    internal static TNode RemoveSingleLineDocumentationComment<TNode>(TNode node, DocumentationCommentTriviaSyntax documentationComment) where TNode : SyntaxNode
    {
        if (node is null)
            throw new ArgumentNullException(nameof(node));

        if (!documentationComment.IsKind(SyntaxKind.SingleLineDocumentationCommentTrivia))
            throw new ArgumentException($"Documentation comment's kind must be '{nameof(SyntaxKind.SingleLineDocumentationCommentTrivia)}'.", nameof(documentationComment));

        SyntaxTrivia trivia = documentationComment.ParentTrivia;

        SyntaxToken token = trivia.Token;

        SyntaxTriviaList leadingTrivia = token.LeadingTrivia;

        int index = leadingTrivia.IndexOf(trivia);

        if (index >= 0
            && index < leadingTrivia.Count - 1
            && leadingTrivia[index + 1].IsWhitespaceTrivia())
        {
            SyntaxTriviaList newLeadingTrivia = leadingTrivia.RemoveRange(index, 2);

            SyntaxToken newToken = token.WithLeadingTrivia(newLeadingTrivia);

            return node.ReplaceToken(token, newToken);
        }

        return node.RemoveNode(documentationComment, SyntaxRemoveOptions.KeepNoTrivia)!;
    }

    public static TNode RemoveComments<TNode>(TNode node, CommentFilter comments) where TNode : SyntaxNode
    {
        if (node is null)
            throw new ArgumentNullException(nameof(node));

        return RemoveComments(node, node.FullSpan, comments);
    }

    public static TNode RemoveComments<TNode>(TNode node, TextSpan span, CommentFilter comments) where TNode : SyntaxNode
    {
        if (node is null)
            throw new ArgumentNullException(nameof(node));

        List<SyntaxTrivia>? commentsToRemove = null;

        foreach (SyntaxTrivia trivia in node.DescendantTrivia(span, descendIntoTrivia: true))
        {
            if (span.Contains(trivia.Span))
            {
                switch (trivia.Kind())
                {
                    case SyntaxKind.SingleLineCommentTrivia:
                        {
                            if ((comments & CommentFilter.SingleLine) == 0)
                                break;

                            AddTrivia(trivia);

                            SyntaxTriviaList triviaList = trivia.GetContainingList();

                            int index = triviaList.IndexOf(trivia);

                            if (index > 0)
                            {
                                SyntaxTrivia previousTrivia = triviaList[index - 1];

                                if (previousTrivia.IsKind(SyntaxKind.WhitespaceTrivia))
                                    AddTrivia(previousTrivia);
                            }

                            if (index < triviaList.Count - 1)
                            {
                                SyntaxTrivia nextTrivia = triviaList[index + 1];

                                if (nextTrivia.IsKind(SyntaxKind.EndOfLineTrivia))
                                    AddTrivia(nextTrivia);
                            }

                            break;
                        }
                    case SyntaxKind.SingleLineDocumentationCommentTrivia:
                        {
                            if ((comments & CommentFilter.SingleLineDocumentation) == 0)
                                break;

                            AddTrivia(trivia);

                            SyntaxTriviaList triviaList = trivia.GetContainingList();

                            int index = triviaList.IndexOf(trivia);

                            if (index > 0)
                            {
                                SyntaxTrivia previousTrivia = triviaList[index - 1];

                                if (previousTrivia.IsKind(SyntaxKind.WhitespaceTrivia))
                                    AddTrivia(previousTrivia);
                            }

                            break;
                        }
                    case SyntaxKind.MultiLineCommentTrivia:
                        {
                            if ((comments & CommentFilter.MultiLine) != 0)
                                RemoveMultiline(trivia);

                            break;
                        }
                    case SyntaxKind.MultiLineDocumentationCommentTrivia:
                        {
                            if ((comments & CommentFilter.MultiLineDocumentation) != 0)
                                RemoveMultiline(trivia);

                            break;
                        }
                }
            }
        }

        return (commentsToRemove is not null)
            ? node.ReplaceTrivia(commentsToRemove, (_, __) => EmptyWhitespace())
            : node;

        void AddTrivia(SyntaxTrivia trivia)
        {
            (commentsToRemove ??= new List<SyntaxTrivia>()).Add(trivia);
        }

        void RemoveMultiline(SyntaxTrivia trivia)
        {
            AddTrivia(trivia);

            SyntaxTriviaList triviaList = trivia.GetContainingList();

            int index = triviaList.IndexOf(trivia);

            if (index > 0)
            {
                SyntaxTrivia previousTrivia = triviaList[index - 1];

                if (previousTrivia.IsKind(SyntaxKind.WhitespaceTrivia))
                    AddTrivia(previousTrivia);
            }

            if (index < triviaList.Count - 1)
            {
                SyntaxTrivia nextTrivia = triviaList[index + 1];

                if (nextTrivia.IsKind(SyntaxKind.WhitespaceTrivia))
                {
                    index++;
                    AddTrivia(nextTrivia);
                }
            }

            if (index < triviaList.Count - 1)
            {
                SyntaxTrivia nextTrivia = triviaList[index + 1];

                if (nextTrivia.IsKind(SyntaxKind.EndOfLineTrivia))
                    AddTrivia(nextTrivia);
            }
        }
    }

    public static TNode RemoveTrivia<TNode>(TNode node, TextSpan? span = null) where TNode : SyntaxNode
    {
        if (node is null)
            throw new ArgumentNullException(nameof(node));

        return (TNode)TriviaRemover.GetInstance(span).Visit(node);
    }

    public static TNode RemoveWhitespace<TNode>(TNode node, TextSpan? span = null) where TNode : SyntaxNode
    {
        if (node is null)
            throw new ArgumentNullException(nameof(node));

        return (TNode)WhitespaceRemover.GetInstance(span).Visit(node);
    }

    public static ClassDeclarationSyntax RemoveMember(ClassDeclarationSyntax classDeclaration, MemberDeclarationSyntax member)
    {
        if (classDeclaration is null)
            throw new ArgumentNullException(nameof(classDeclaration));

        if (member is null)
            throw new ArgumentNullException(nameof(member));

        int index = classDeclaration.Members.IndexOf(member);

        MemberDeclarationSyntax newMember = RemoveSingleLineDocumentationComment(member);

        classDeclaration = classDeclaration.WithMembers(classDeclaration.Members.ReplaceAt(index, newMember));

        return RemoveNode(classDeclaration, f => f.Members, index, GetRemoveOptions(newMember));
    }

    public static CompilationUnitSyntax RemoveMember(CompilationUnitSyntax compilationUnit, MemberDeclarationSyntax member)
    {
        if (compilationUnit is null)
            throw new ArgumentNullException(nameof(compilationUnit));

        if (member is null)
            throw new ArgumentNullException(nameof(member));

        int index = compilationUnit.Members.IndexOf(member);

        MemberDeclarationSyntax newMember = RemoveSingleLineDocumentationComment(member);

        compilationUnit = compilationUnit.WithMembers(compilationUnit.Members.ReplaceAt(index, newMember));

        return compilationUnit.RemoveNode(compilationUnit.Members[index], GetRemoveOptions(newMember))!;
    }

    public static InterfaceDeclarationSyntax RemoveMember(InterfaceDeclarationSyntax interfaceDeclaration, MemberDeclarationSyntax member)
    {
        if (interfaceDeclaration is null)
            throw new ArgumentNullException(nameof(interfaceDeclaration));

        if (member is null)
            throw new ArgumentNullException(nameof(member));

        int index = interfaceDeclaration.Members.IndexOf(member);

        MemberDeclarationSyntax newMember = RemoveSingleLineDocumentationComment(member);

        interfaceDeclaration = interfaceDeclaration.WithMembers(interfaceDeclaration.Members.ReplaceAt(index, newMember));

        return RemoveNode(interfaceDeclaration, f => f.Members, index, GetRemoveOptions(newMember));
    }

    public static BaseNamespaceDeclarationSyntax RemoveMember(BaseNamespaceDeclarationSyntax namespaceDeclaration, MemberDeclarationSyntax member)
    {
        if (namespaceDeclaration is null)
            throw new ArgumentNullException(nameof(namespaceDeclaration));

        if (member is null)
            throw new ArgumentNullException(nameof(member));

        int index = namespaceDeclaration.Members.IndexOf(member);

        MemberDeclarationSyntax newMember = RemoveSingleLineDocumentationComment(member);

        namespaceDeclaration = namespaceDeclaration.WithMembers(namespaceDeclaration.Members.ReplaceAt(index, newMember));

        if (namespaceDeclaration.IsKind(SyntaxKind.FileScopedNamespaceDeclaration))
        {
            return namespaceDeclaration.RemoveNode(namespaceDeclaration.Members[index], GetRemoveOptions(newMember))!;
        }
        else
        {
            return RemoveNode(namespaceDeclaration, f => f.Members, index, GetRemoveOptions(newMember));
        }
    }

    public static StructDeclarationSyntax RemoveMember(StructDeclarationSyntax structDeclaration, MemberDeclarationSyntax member)
    {
        if (structDeclaration is null)
            throw new ArgumentNullException(nameof(structDeclaration));

        if (member is null)
            throw new ArgumentNullException(nameof(member));

        int index = structDeclaration.Members.IndexOf(member);

        MemberDeclarationSyntax newMember = RemoveSingleLineDocumentationComment(member);

        structDeclaration = structDeclaration.WithMembers(structDeclaration.Members.ReplaceAt(index, newMember));

        return RemoveNode(structDeclaration, f => f.Members, index, GetRemoveOptions(newMember));
    }

    public static RecordDeclarationSyntax RemoveMember(RecordDeclarationSyntax recordDeclaration, MemberDeclarationSyntax member)
    {
        if (recordDeclaration is null)
            throw new ArgumentNullException(nameof(recordDeclaration));

        if (member is null)
            throw new ArgumentNullException(nameof(member));

        int index = recordDeclaration.Members.IndexOf(member);

        MemberDeclarationSyntax newMember = RemoveSingleLineDocumentationComment(member);

        recordDeclaration = recordDeclaration.WithMembers(recordDeclaration.Members.ReplaceAt(index, newMember));

        return RemoveNode(recordDeclaration, f => f.Members, index, GetRemoveOptions(newMember));
    }

    public static TypeDeclarationSyntax RemoveMember(TypeDeclarationSyntax typeDeclaration, MemberDeclarationSyntax member)
    {
        if (typeDeclaration is null)
            throw new ArgumentNullException(nameof(typeDeclaration));

        if (member is null)
            throw new ArgumentNullException(nameof(member));

        int index = typeDeclaration.Members.IndexOf(member);

        MemberDeclarationSyntax newMember = RemoveSingleLineDocumentationComment(member);

        typeDeclaration = typeDeclaration.WithMembers(typeDeclaration.Members.ReplaceAt(index, newMember));

        return RemoveNode(typeDeclaration, f => f.Members, index, GetRemoveOptions(newMember));
    }

    public static EnumDeclarationSyntax RemoveMember(EnumDeclarationSyntax typeDeclaration, EnumMemberDeclarationSyntax member)
    {
        if (typeDeclaration is null)
            throw new ArgumentNullException(nameof(typeDeclaration));

        if (member is null)
            throw new ArgumentNullException(nameof(member));

        int index = typeDeclaration.Members.IndexOf(member);

        EnumMemberDeclarationSyntax newMember = RemoveSingleLineDocumentationComment(member);

        typeDeclaration = typeDeclaration.WithMembers(typeDeclaration.Members.ReplaceAt(index, newMember));

        return RemoveNode(typeDeclaration, f => f.Members, index, GetRemoveOptions(newMember));
    }

    private static T RemoveNode<T>(
        T declaration,
        Func<T, SyntaxList<MemberDeclarationSyntax>> getMembers,
        int index,
        SyntaxRemoveOptions removeOptions) where T : SyntaxNode
    {
        SyntaxList<MemberDeclarationSyntax> members = getMembers(declaration);

        T newDeclaration = declaration.RemoveNode(members[index], removeOptions)!;

        if (index == 0
            && index < members.Count - 1)
        {
            members = getMembers(newDeclaration);

            MemberDeclarationSyntax nextMember = members[index];

            SyntaxTriviaList leadingTrivia = nextMember.GetLeadingTrivia();

            SyntaxTrivia trivia = leadingTrivia.FirstOrDefault();

            if (trivia.IsEndOfLineTrivia())
            {
                MemberDeclarationSyntax newNextMember = nextMember.WithLeadingTrivia(leadingTrivia.RemoveAt(0));

                newDeclaration = newDeclaration.ReplaceNode(nextMember, newNextMember);
            }
        }

        return newDeclaration;
    }

    private static T RemoveNode<T>(
        T declaration,
        Func<T, SeparatedSyntaxList<EnumMemberDeclarationSyntax>> getMembers,
        int index,
        SyntaxRemoveOptions removeOptions) where T : SyntaxNode
    {
        SeparatedSyntaxList<EnumMemberDeclarationSyntax> members = getMembers(declaration);

        T newDeclaration = declaration.RemoveNode(members[index], removeOptions)!;

        if (index == 0
            && index < members.Count - 1)
        {
            members = getMembers(newDeclaration);

            EnumMemberDeclarationSyntax nextMember = members[index];

            SyntaxTriviaList leadingTrivia = nextMember.GetLeadingTrivia();

            SyntaxTrivia trivia = leadingTrivia.FirstOrDefault();

            if (trivia.IsEndOfLineTrivia())
            {
                EnumMemberDeclarationSyntax newNextMember = nextMember.WithLeadingTrivia(leadingTrivia.RemoveAt(0));

                newDeclaration = newDeclaration.ReplaceNode(nextMember, newNextMember);
            }
        }

        return newDeclaration;
    }

    public static BlockSyntax RemoveUnsafeContext(UnsafeStatementSyntax unsafeStatement)
    {
        SyntaxToken keyword = unsafeStatement.UnsafeKeyword;

        BlockSyntax block = unsafeStatement.Block;

        SyntaxTriviaList leadingTrivia = keyword.LeadingTrivia
            .AddRange(keyword.TrailingTrivia.EmptyIfWhitespace())
            .AddRange(block.GetLeadingTrivia().EmptyIfWhitespace());

        return block.WithLeadingTrivia(leadingTrivia);
    }

    public static IEnumerable<AttributeListSyntax> SplitAttributeList(AttributeListSyntax attributeList)
    {
        SeparatedSyntaxList<AttributeSyntax> attributes = attributeList.Attributes;

        for (int i = 0; i < attributes.Count; i++)
        {
            AttributeListSyntax list = AttributeList(attributes[i]);

            if (i == 0)
                list = list.WithLeadingTrivia(attributeList.GetLeadingTrivia());

            if (i == attributes.Count - 1)
                list = list.WithTrailingTrivia(attributeList.GetTrailingTrivia());

            yield return list;
        }
    }

    public static AttributeListSyntax JoinAttributes(IList<AttributeListSyntax> lists)
    {
        AttributeListSyntax list = lists[0];

        for (int i = 1; i < lists.Count; i++)
            list = list.AddAttributes(lists[i].Attributes.ToArray());

        return list
            .WithLeadingTrivia(lists[0].GetLeadingTrivia())
            .WithTrailingTrivia(lists.Last().GetTrailingTrivia());
    }

    public static InvocationExpressionSyntax ChangeInvokedMethodName(InvocationExpressionSyntax invocationExpression, string newName)
    {
        ExpressionSyntax expression = invocationExpression.Expression;

        if (expression is not null)
        {
            SyntaxKind kind = expression.Kind();

            if (kind == SyntaxKind.SimpleMemberAccessExpression)
            {
                var memberAccess = (MemberAccessExpressionSyntax)expression;
                SimpleNameSyntax simpleName = memberAccess.Name;

                if (simpleName is not null)
                {
                    SimpleNameSyntax newSimpleName = ChangeName(simpleName);

                    return invocationExpression.WithExpression(memberAccess.WithName(newSimpleName));
                }
            }
            else if (kind == SyntaxKind.MemberBindingExpression)
            {
                var memberBinding = (MemberBindingExpressionSyntax)expression;
                SimpleNameSyntax simpleName = memberBinding.Name;

                if (simpleName is not null)
                {
                    SimpleNameSyntax newSimpleName = ChangeName(simpleName);

                    return invocationExpression.WithExpression(memberBinding.WithName(newSimpleName));
                }
            }
            else
            {
                if (expression is SimpleNameSyntax simpleName)
                {
                    SimpleNameSyntax newSimpleName = ChangeName(simpleName);

                    return invocationExpression.WithExpression(newSimpleName);
                }

                Debug.Fail(kind.ToString());
            }
        }

        return invocationExpression;

        SimpleNameSyntax ChangeName(SimpleNameSyntax simpleName)
        {
            return simpleName.WithIdentifier(
                Identifier(
                    simpleName.GetLeadingTrivia(),
                    newName,
                    simpleName.GetTrailingTrivia()));
        }
    }

    public static LiteralExpressionSyntax ReplaceStringLiteralWithCharacterLiteral(LiteralExpressionSyntax literalExpression)
    {
        return (LiteralExpressionSyntax)ParseExpression($"'{GetCharacterLiteralText()}'")
            .WithTriviaFrom(literalExpression);

        string GetCharacterLiteralText()
        {
            string s = literalExpression.Token.ValueText;

            switch (s[0])
            {
                case '\'':
                    return @"\'";
                case '\"':
                    return @"\""";
                case '\\':
                    return @"\\";
                case '\0':
                    return @"\0";
                case '\a':
                    return @"\a";
                case '\b':
                    return @"\b";
                case '\f':
                    return @"\f";
                case '\n':
                    return @"\n";
                case '\r':
                    return @"\r";
                case '\t':
                    return @"\t";
                case '\v':
                    return @"\v";
                default:
                    return s;
            }
        }
    }

    public static ForStatementSyntax ConvertWhileStatementToForStatement(
        WhileStatementSyntax whileStatement,
        VariableDeclarationSyntax? declaration = default,
        SeparatedSyntaxList<ExpressionSyntax> initializers = default)
    {
        var incrementors = default(SeparatedSyntaxList<ExpressionSyntax>);

        StatementSyntax statement = whileStatement.Statement;

        if (statement is BlockSyntax block)
        {
            SyntaxList<StatementSyntax> statements = block.Statements;

            if (statements.Any())
            {
                int startIndex = -1;
                int i = statements.Count - 1;

                bool fContinue = statements.Last().IsKind(SyntaxKind.ContinueStatement);

                if (fContinue)
                    i--;

                while (i >= 0)
                {
                    if (statements[i] is not ExpressionStatementSyntax expressionStatement)
                        break;

                    ExpressionSyntax expression = expressionStatement.Expression;

                    if (expression is null
                        || !CSharpFacts.IsIncrementOrDecrementExpression(expression.Kind()))
                    {
                        break;
                    }

                    startIndex = i;
                    i--;
                }

                if (startIndex >= 0)
                {
                    int count = statements.Count - startIndex;

                    if (fContinue)
                        count--;

                    incrementors = statements
                        .Skip(startIndex)
                        .Take(count)
                        .Cast<ExpressionStatementSyntax>()
                        .Select(f => f.Expression)
                        .ToSeparatedSyntaxList();

                    statement = block.WithStatements(statements.RemoveRange(startIndex, statements.Count - startIndex));
                }
                else if (fContinue)
                {
                    statement = block.WithStatements(statements.RemoveAt(statements.Count - 1));
                }
            }
        }

        ExpressionSyntax? condition = whileStatement.Condition;

        if (condition.IsKind(SyntaxKind.TrueLiteralExpression))
            condition = null;

        return ForStatement(
            forKeyword: Token(SyntaxKind.ForKeyword).WithTriviaFrom(whileStatement.WhileKeyword),
            openParenToken: Token(whileStatement.OpenParenToken.LeadingTrivia, SyntaxKind.OpenParenToken, default),
            declaration: declaration,
            initializers: initializers,
            firstSemicolonToken: SemicolonToken(),
            condition: condition,
            secondSemicolonToken: SemicolonToken(),
            incrementors: incrementors,
            closeParenToken: Token(default, SyntaxKind.CloseParenToken, whileStatement.CloseParenToken.TrailingTrivia),
            statement: statement);
    }

    public static BinaryExpressionSyntax SwapBinaryOperands(BinaryExpressionSyntax binaryExpression)
    {
        ExpressionSyntax left = binaryExpression.Left;
        ExpressionSyntax right = binaryExpression.Right;
        SyntaxToken token = binaryExpression.OperatorToken;

        SyntaxKind tokenKind = token.Kind();

        SyntaxKind newTokenKind = tokenKind switch
        {
            SyntaxKind.LessThanToken => SyntaxKind.GreaterThanToken,
            SyntaxKind.LessThanEqualsToken => SyntaxKind.GreaterThanEqualsToken,
            SyntaxKind.GreaterThanToken => SyntaxKind.LessThanToken,
            SyntaxKind.GreaterThanEqualsToken => SyntaxKind.LessThanEqualsToken,
            _ => tokenKind,
        };

        return binaryExpression.Update(
            left: right.WithTriviaFrom(left),
            operatorToken: Token(token.LeadingTrivia, newTokenKind, token.TrailingTrivia),
            right: left.WithTriviaFrom(right));
    }

    public static CollectionExpressionSyntax ConvertInitializerToCollectionExpression(InitializerExpressionSyntax? initializer)
    {
        if (initializer is not null)
        {
            return CollectionExpression(
                Token(SyntaxKind.OpenBracketToken).WithTriviaFrom(initializer.OpenBraceToken),
                initializer
                    .Expressions
                    .Select(f => ExpressionElement(f))
                    .ToSeparatedSyntaxList<CollectionElementSyntax>(),
                Token(SyntaxKind.CloseBracketToken).WithTriviaFrom(initializer.CloseBraceToken));
        }
        else
        {
            return CollectionExpression();
        }
    }

    public static InitializerExpressionSyntax? ConvertCollectionExpressionToInitializer(CollectionExpressionSyntax collectionExpression, SyntaxKind initializerKind)
    {
        if (collectionExpression.Elements.Any())
        {
            return InitializerExpression(
                initializerKind,
                collectionExpression
                    .Elements
                    .Select(element => ((ExpressionElementSyntax)element).Expression)
                    .ToSeparatedSyntaxList());
        }

        return default;
    }
}
