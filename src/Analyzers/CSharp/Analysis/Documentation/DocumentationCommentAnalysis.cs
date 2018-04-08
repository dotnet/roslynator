// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.CSharp;
using Roslynator.CSharp.Syntax;

namespace Roslynator.CSharp.Analysis.Documentation
{
    internal static class DocumentationCommentAnalysis
    {
        public static ImmutableArray<string> GetAttributeValues(DocumentationCommentTriviaSyntax comment, string elementName1, string elementName2, string attributeName)
        {
            HashSet<string> values = null;
            bool containsIncludeOrExclude = false;
            bool isFirst = true;

            foreach (XmlNodeSyntax node in comment.Content)
            {
                XmlElementInfo info = SyntaxInfo.XmlElementInfo(node);
                if (info.Success)
                {
                    switch (info.ElementKind)
                    {
                        case XmlElementKind.Include:
                        case XmlElementKind.Exclude:
                            {
                                if (isFirst)
                                    containsIncludeOrExclude = true;

                                break;
                            }
                        case XmlElementKind.InheritDoc:
                            {
                                return default(ImmutableArray<string>);
                            }
                        default:
                            {
                                if (!info.IsEmptyElement
                                    && info.IsLocalName(elementName1, elementName2))
                                {
                                    string value = GetAttributeValue((XmlElementSyntax)info.Element, attributeName);

                                    if (value != null)
                                        (values ?? (values = new HashSet<string>())).Add(value);
                                }

                                break;
                            }
                    }

                    if (isFirst)
                    {
                        isFirst = false;
                    }
                    else
                    {
                        containsIncludeOrExclude = false;
                    }
                }
            }

            if (!containsIncludeOrExclude)
                return values?.ToImmutableArray() ?? ImmutableArray<string>.Empty;

            return default(ImmutableArray<string>);
        }

        public static string GetAttributeValue(XmlElementSyntax element, string attributeName)
        {
            XmlElementStartTagSyntax startTag = element.StartTag;

            if (startTag != null)
            {
                foreach (XmlAttributeSyntax attribute in startTag.Attributes)
                {
                    if (attribute.IsKind(SyntaxKind.XmlNameAttribute))
                    {
                        var nameAttribute = (XmlNameAttributeSyntax)attribute;

                        if (nameAttribute.Name?.LocalName.ValueText == attributeName)
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