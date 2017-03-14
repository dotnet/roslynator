// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
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
        public static HashSet<string> GetAttributeValues(DocumentationCommentTriviaSyntax comment, string elementName, string attributeName, out bool containsInheritDoc)
        {
            containsInheritDoc = false;
            HashSet<string> elements = null;

            foreach (XmlNodeSyntax node in comment.Content)
            {
                SyntaxKind kind = node.Kind();

                if (kind == SyntaxKind.XmlElement)
                {
                    var element = (XmlElementSyntax)node;

                    string name = element.StartTag?.Name?.LocalName.ValueText;

                    if (name != null)
                    {
                        if (IsInheritDoc(name))
                        {
                            containsInheritDoc = true;
                            return null;
                        }
                        else if (string.Equals(name, elementName, StringComparison.Ordinal))
                        {
                            string value = GetAttributeValue(element, attributeName);

                            if (value != null)
                                (elements ?? (elements = new HashSet<string>())).Add(value);
                        }
                    }
                }

                if (kind == SyntaxKind.XmlEmptyElement)
                {
                    var element = (XmlEmptyElementSyntax)node;

                    string name = element?.Name?.LocalName.ValueText;

                    if (name != null
                        && IsInheritDoc(name))
                    {
                        containsInheritDoc = true;
                        return null;
                    }
                }
            }

            return elements;
        }

        private static bool IsInheritDoc(string name)
        {
            return string.Equals(name, "inheritdoc", StringComparison.Ordinal);
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