// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace Pihrtsoft.CodeAnalysis.CSharp.Refactoring
{
    public static class AttributeRefactoring
    {
        public static AttributeListSyntax MergeAttributes(AttributeListSyntax[] attributeLists)
        {
            if (attributeLists == null)
                throw new ArgumentNullException(nameof(attributeLists));

            AttributeListSyntax attributeList = attributeLists[0];

            for (int i = 1; i < attributeLists.Length; i++)
                attributeList = attributeList.AddAttributes(attributeLists[i].Attributes.ToArray());

            return attributeList
                .WithLeadingTrivia(attributeLists.First().GetLeadingTrivia())
                .WithTrailingTrivia(attributeLists.Last().GetTrailingTrivia());
        }

        public static IEnumerable<AttributeListSyntax> SplitAttributes(AttributeListSyntax[] attributeLists)
        {
            if (attributeLists == null)
                throw new ArgumentNullException(nameof(attributeLists));

            foreach (AttributeListSyntax attributeList in attributeLists)
            {
                foreach (AttributeListSyntax list in SplitAttributes(attributeList))
                    yield return list;
            }
        }

        public static IEnumerable<AttributeListSyntax> SplitAttributes(AttributeListSyntax attributeList)
        {
            if (attributeList == null)
                throw new ArgumentNullException(nameof(attributeList));

            for (int i = 0; i < attributeList.Attributes.Count; i++)
            {
                AttributeListSyntax list = SyntaxFactory.AttributeList(
                    SyntaxFactory.SingletonSeparatedList(attributeList.Attributes[i]));

                if (i == 0)
                    list = list.WithLeadingTrivia(attributeList.GetLeadingTrivia());

                if (i == attributeList.Attributes.Count - 1)
                    list = list.WithTrailingTrivia(attributeList.GetTrailingTrivia());

                yield return list;
            }
        }

        public static IEnumerable<AttributeListSyntax> GetSelectedAttributeLists(SyntaxList<AttributeListSyntax> attributeLists, TextSpan span)
        {
            if (attributeLists.Count > 0)
            {
                SyntaxList<AttributeListSyntax>.Enumerator en = attributeLists.GetEnumerator();

                while (en.MoveNext())
                {
                    if (span.Start >= en.Current.FullSpan.Start)
                    {
                        if (span.Start <= en.Current.Span.Start)
                        {
                            do
                            {
                                if (span.End >= en.Current.Span.End)
                                {
                                    yield return en.Current;
                                }
                                else
                                {
                                    yield break;
                                }
                            } while (en.MoveNext());
                        }
                        else if (span.Start <= en.Current.Span.End)
                        {
                            yield break;
                        }
                    }
                }
            }
        }
    }
}
