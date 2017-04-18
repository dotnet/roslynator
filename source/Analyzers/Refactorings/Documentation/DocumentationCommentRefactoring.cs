// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.CSharp;

namespace Roslynator.CSharp.Refactorings.DocumentationComment
{
    internal static class DocumentationCommentRefactoring
    {
        public static ImmutableArray<string> GetAttributeValues(DocumentationCommentTriviaSyntax comment, string elementName, string attributeName)
        {
            HashSet<string> values = null;

            bool containsInclude = false;
            bool isFirst = true;

            foreach (XmlNodeSyntax node in comment.Content)
            {
                SyntaxKind kind = node.Kind();

                if (kind == SyntaxKind.XmlElement)
                {
                    var element = (XmlElementSyntax)node;

                    string name = element.StartTag?.Name?.LocalName.ValueText;

                    if (name != null)
                    {
                        if (isFirst)
                        {
                            if (NameEquals(name, "include"))
                                containsInclude = true;

                            isFirst = false;
                        }
                        else
                        {
                            containsInclude = false;
                        }

                        if (NameEquals(name, "inheritdoc"))
                        {
                            return default(ImmutableArray<string>);
                        }
                        else if (NameEquals(name, elementName))
                        {
                            string value = GetAttributeValue(element, attributeName);

                            if (value != null)
                                (values ?? (values = new HashSet<string>())).Add(value);
                        }
                    }
                }

                if (kind == SyntaxKind.XmlEmptyElement)
                {
                    var element = (XmlEmptyElementSyntax)node;

                    string name = element?.Name?.LocalName.ValueText;

                    if (name != null)
                    {
                        if (isFirst)
                        {
                            if (NameEquals(name, "include"))
                                containsInclude = true;

                            isFirst = false;
                        }
                        else
                        {
                            containsInclude = false;
                        }

                        if (NameEquals(name, "inheritdoc"))
                            return default(ImmutableArray<string>);
                    }
                }
            }

            if (!containsInclude)
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

        private static bool NameEquals(string name1, string name2)
        {
            return string.Equals(name1, name2, StringComparison.Ordinal);
        }
    }
}