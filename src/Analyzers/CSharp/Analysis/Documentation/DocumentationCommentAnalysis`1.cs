// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Roslynator.CSharp.Analysis.Documentation
{
    internal abstract class DocumentationCommentAnalysis<TNode> where TNode : SyntaxNode
    {
        public abstract SeparatedSyntaxList<TNode> GetContainingList(TNode node);

        public abstract string GetName(TNode node);

        public abstract ElementInfo<TNode> CreateInfo(TNode node, int insertIndex, NewLinePosition newLinePosition);

        public virtual MemberDeclarationSyntax GetMemberDeclaration(TNode node)
        {
            return node.FirstAncestor<MemberDeclarationSyntax>();
        }

        public abstract ImmutableArray<string> ElementNames { get; }

        public abstract string ElementName { get; }

        public abstract string ElementNameUppercase { get; }

        public string GetNewTrivia(
            DocumentationCommentTriviaSyntax comment,
            List<ElementInfo<TNode>> elementInfos)
        {
            var sb = new StringBuilder();
            string text = comment.ToFullString();
            int start = comment.FullSpan.Start;
            int startIndex = 0;

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
                        .Append(ElementName)
                        .Append(" name=\"")
                        .Append(elementInfo.Name)
                        .Append("\"></")
                        .Append(ElementName)
                        .Append(">");

                    if (elementInfo.NewLinePosition == NewLinePosition.End)
                        sb.AppendLine();
                }

                startIndex = endIndex;
            }

            sb.Append(text, startIndex, text.Length - startIndex);

            return sb.ToString();
        }

        public Dictionary<string, XmlElementSyntax> CreateNameElementMap(DocumentationCommentTriviaSyntax comment)
        {
            var dic = new Dictionary<string, XmlElementSyntax>();

            foreach (XmlElementSyntax element in comment.Elements(ElementName, ElementNameUppercase))
            {
                string name = DocumentationCommentAnalysis.GetAttributeValue(element, "name");

                if (!dic.ContainsKey(name))
                    dic.Add(name, element);
            }

            return dic;
        }
    }
}