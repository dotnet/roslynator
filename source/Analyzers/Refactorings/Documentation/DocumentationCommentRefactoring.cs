// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.CSharp.Extensions;
using Roslynator.Extensions;

namespace Roslynator.CSharp.Refactorings.DocumentationComment
{
    internal static class DocumentationCommentRefactoring
    {
        public static IEnumerable<string> GetNameAttributeValues(DocumentationCommentTriviaSyntax comment, string localName)
        {
            foreach (XmlElementSyntax element in comment.Elements(localName))
            {
                string name = GetNameAttributeValue(element);

                if (name != null)
                    yield return name;
            }
        }

        public static string GetNameAttributeValue(XmlElementSyntax element)
        {
            XmlElementStartTagSyntax startTag = element.StartTag;

            if (startTag != null)
            {
                SyntaxList<XmlAttributeSyntax> attributes = startTag.Attributes;

                foreach (XmlAttributeSyntax attribute in attributes)
                {
                    if (attribute.IsKind(SyntaxKind.XmlNameAttribute))
                    {
                        var nameAttribute = (XmlNameAttributeSyntax)attribute;

                        if (nameAttribute.Name?.LocalName.ValueText == "name")
                        {
                            IdentifierNameSyntax identifierName = nameAttribute.Identifier;

                            if (identifierName != null)
                                return identifierName.Identifier.ValueText;
                        }
                    }
                }
            }

            return null;
        }
    }
}