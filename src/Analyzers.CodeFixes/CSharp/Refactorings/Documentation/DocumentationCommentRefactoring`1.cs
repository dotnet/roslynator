﻿// Copyright (c) .NET Foundation and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.CSharp.Analysis.Documentation;

namespace Roslynator.CSharp.Refactorings.Documentation;

internal abstract class DocumentationCommentRefactoring<TNode> where TNode : SyntaxNode
{
    public abstract XmlTag Tag { get; }

    public abstract bool ShouldBeBefore(XmlTag tag);

    public abstract string GetName(TNode node);

    public abstract ElementInfo<TNode> CreateInfo(TNode node, int insertIndex, NewLinePosition newLinePosition);

    public async Task<Document> RefactorAsync(
        Document document,
        DocumentationCommentTriviaSyntax comment,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        MemberDeclarationSyntax memberDeclaration = comment.FirstAncestor<MemberDeclarationSyntax>();

        SeparatedSyntaxList<TNode> typeParameters = GetSyntaxList(memberDeclaration);

        List<ElementInfo<TNode>> infos = GetElementInfos(comment, typeParameters);

        string newTrivia = GetNewTrivia(comment, infos);

        SyntaxTriviaList triviaList = SyntaxFactory.ParseLeadingTrivia(newTrivia);

        if (triviaList.Any())
        {
            SyntaxTrivia firstTrivia = triviaList[0];

            if (firstTrivia.HasStructure
                && (firstTrivia.GetStructure() is DocumentationCommentTriviaSyntax newComment))
            {
                newComment = newComment.WithFormatterAnnotation();

                return await document.ReplaceNodeAsync(comment, newComment, cancellationToken).ConfigureAwait(false);
            }
        }

        Debug.Fail("");

        return document;
    }

    protected abstract SeparatedSyntaxList<TNode> GetSyntaxList(SyntaxNode node);

    private string GetNewTrivia(
        DocumentationCommentTriviaSyntax comment,
        List<ElementInfo<TNode>> elementInfos)
    {
        var sb = new StringBuilder();
        string text = comment.ToFullString();
        int start = comment.FullSpan.Start;
        int startIndex = 0;

        string elementName = XmlTagMapper.GetName(Tag);

        foreach (IGrouping<int, ElementInfo<TNode>> grouping in elementInfos
            .OrderBy(f => f.InsertIndex)
            .GroupBy(f => f.InsertIndex))
        {
            int endIndex = grouping.Key - start;

            sb.Append(text, startIndex, endIndex - startIndex);

            foreach (ElementInfo<TNode> elementInfo in grouping)
            {
                if (elementInfo.NewLinePosition == NewLinePosition.Beginning)
                    sb.AppendLine();

                sb.Append("/// <")
                    .Append(elementName)
                    .Append(" name=\"")
                    .Append(elementInfo.Name)
                    .Append("\"></")
                    .Append(elementName)
                    .Append('>');

                if (elementInfo.NewLinePosition == NewLinePosition.End)
                    sb.AppendLine();
            }

            startIndex = endIndex;
        }

        sb.Append(text, startIndex, text.Length - startIndex);

        return sb.ToString();
    }

    private List<ElementInfo<TNode>> GetElementInfos(
        DocumentationCommentTriviaSyntax comment,
        SeparatedSyntaxList<TNode> nodes)
    {
        Dictionary<string, XmlElementSyntax> dic = CreateNameElementMap(comment);

        var elementInfos = new List<ElementInfo<TNode>>();

        for (int i = 0; i < nodes.Count; i++)
        {
            if (!dic.ContainsKey(GetName(nodes[i])))
            {
                int insertIndex = -1;
                var newLinePosition = NewLinePosition.Beginning;

                for (int j = i - 1; j >= 0; j--)
                {
                    if (dic.TryGetValue(GetName(nodes[j]), out XmlElementSyntax element))
                    {
                        insertIndex = element.FullSpan.End;
                        break;
                    }
                }

                if (insertIndex == -1)
                {
                    for (int j = i + 1; j < nodes.Count; j++)
                    {
                        if (dic.TryGetValue(GetName(nodes[j]), out XmlElementSyntax element))
                        {
                            XmlElementSyntax previousElement = GetPreviousElement(comment, element);

                            if (previousElement is not null)
                            {
                                insertIndex = previousElement.FullSpan.End;
                            }
                            else
                            {
                                insertIndex = comment.FullSpan.Start;
                                newLinePosition = NewLinePosition.End;
                            }

                            break;
                        }
                    }
                }

                if (insertIndex == -1)
                {
                    insertIndex = GetDefaultIndex(comment);

                    if (insertIndex == comment.FullSpan.Start)
                        newLinePosition = NewLinePosition.End;
                }

                ElementInfo<TNode> elementInfo = CreateInfo(nodes[i], insertIndex, newLinePosition);

                elementInfos.Add(elementInfo);
            }
        }

        return elementInfos;
    }

    private static XmlElementSyntax GetPreviousElement(DocumentationCommentTriviaSyntax comment, XmlElementSyntax element)
    {
        SyntaxList<XmlNodeSyntax> content = comment.Content;

        int index = content.IndexOf(element);

        for (int i = index - 1; i >= 0; i--)
        {
            if (content[i].IsKind(SyntaxKind.XmlElement))
                return (XmlElementSyntax)content[i];
        }

        return null;
    }

    public Dictionary<string, XmlElementSyntax> CreateNameElementMap(DocumentationCommentTriviaSyntax comment)
    {
        var dic = new Dictionary<string, XmlElementSyntax>();

        foreach (XmlElementSyntax element in comment.Elements(Tag))
        {
            string name = element.GetAttributeValueText("name");

            if (!dic.ContainsKey(name))
                dic.Add(name, element);
        }

        return dic;
    }

    private int GetDefaultIndex(DocumentationCommentTriviaSyntax comment)
    {
        SyntaxList<XmlNodeSyntax> content = comment.Content;

        for (int i = content.Count - 1; i >= 0; i--)
        {
            if (content[i] is XmlElementSyntax xmlElement)
            {
                XmlTag tag = xmlElement.GetTag();

                if (tag == Tag
                    || ShouldBeBefore(tag))
                {
                    return content[i].FullSpan.End;
                }
            }
        }

        return comment.FullSpan.Start;
    }
}
