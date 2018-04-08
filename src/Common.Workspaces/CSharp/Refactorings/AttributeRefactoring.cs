// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Roslynator.CSharp.CSharpFactory;

namespace Roslynator.CSharp.Refactorings
{
    internal static class AttributeRefactoring
    {
        public static IEnumerable<AttributeListSyntax> SplitAttributes(AttributeListSyntax attributeList)
        {
            SeparatedSyntaxList<AttributeSyntax> attributes = attributeList.Attributes;

            for (int i = 0; i < attributes.Count; i++)
            {
                AttributeListSyntax list = AttributeList(attributes[i]);

                if (i == 0)
                    list = list.WithLeadingTrivia(attributeList.GetLeadingTrivia());

                if (i == attributes.Count - 1)
                    list = list.WithTrailingTrivia(attributeList.GetTrailingTrivia());

                yield return list.WithFormatterAnnotation();
            }
        }

        public static AttributeListSyntax MergeAttributes(AttributeListSyntax[] lists)
        {
            AttributeListSyntax list = lists[0];

            for (int i = 1; i < lists.Length; i++)
                list = list.AddAttributes(lists[i].Attributes.ToArray());

            return list
                .WithLeadingTrivia(lists[0].GetLeadingTrivia())
                .WithTrailingTrivia(lists.Last().GetTrailingTrivia())
                .WithFormatterAnnotation();
        }
    }
}
