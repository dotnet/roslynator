// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Text.RegularExpressions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Roslynator.CSharp.Refactorings.FormatSummary
{
    internal static class FormatSummaryRefactoring
    {
        public static Regex Regex { get; } = new Regex(
            @"
            ^
            (
                [\s-[\r\n]]*
                \r?\n
                [\s-[\r\n]]*
                ///
                [\s-[\r\n]]*
            )?
            (?<1>[^\r\n]*)
            (
                [\s-[\r\n]]*
                \r?\n
                [\s-[\r\n]]*
                ///
                [\s-[\r\n]]*
            )?
            $
            ", RegexOptions.IgnorePatternWhitespace | RegexOptions.ExplicitCapture);

        public static XmlElementSyntax GetSummaryElement(DocumentationCommentTriviaSyntax documentationComment)
        {
            return GetSummaryElement(documentationComment.Content);
        }

        public static XmlElementSyntax GetSummaryElement(SyntaxList<XmlNodeSyntax> content)
        {
            foreach (XmlNodeSyntax node in content)
            {
                if (node.IsKind(SyntaxKind.XmlElement))
                {
                    var element = (XmlElementSyntax)node;

                    string name = element.StartTag?.Name?.LocalName.ValueText;

                    if (string.Equals(name, "summary", StringComparison.Ordinal))
                        return element;
                }
            }

            return null;
        }
    }
}