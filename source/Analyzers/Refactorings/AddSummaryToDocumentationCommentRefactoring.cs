// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Text;
using Roslynator.CSharp;
using Roslynator.Utilities;

namespace Roslynator.CSharp.Refactorings
{
    internal static class AddSummaryToDocumentationCommentRefactoring
    {
        public static void AnalyzeSingleLineDocumentationCommentTrivia(SyntaxNodeAnalysisContext context)
        {
            var documentationComment = (DocumentationCommentTriviaSyntax)context.Node;

            bool containsInheritDoc = false;
            bool containsInclude = false;
            bool containsSummaryElement = false;

            bool isFirst = true;

            foreach (XmlNodeSyntax node in documentationComment.Content)
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
                            if (IsInclude(name))
                                containsInclude = true;

                            isFirst = false;
                        }
                        else
                        {
                            containsInclude = false;
                        }

                        if (IsInheritDoc(name))
                        {
                            containsInheritDoc = true;
                        }
                        else if (IsSummary(name))
                        {
                            if (IsSummaryMissing(element))
                            {
                                context.ReportDiagnostic(
                                    DiagnosticDescriptors.AddSummaryToDocumentationComment,
                                    element);
                            }

                            containsSummaryElement = true;
                        }

                        if (containsInheritDoc && containsSummaryElement)
                            break;
                    }
                }
                else if (kind == SyntaxKind.XmlEmptyElement)
                {
                    var element = (XmlEmptyElementSyntax)node;

                    string name = element.Name?.LocalName.ValueText;

                    if (name != null)
                    {
                        if (isFirst)
                        {
                            if (IsInclude(name))
                                containsInclude = true;

                            isFirst = false;
                        }
                        else
                        {
                            containsInclude = false;
                        }

                        if (IsInheritDoc(name))
                        {
                            containsInheritDoc = true;
                        }
                        else if (IsSummary(name))
                        {
                            context.ReportDiagnostic(
                                DiagnosticDescriptors.AddSummaryToDocumentationComment,
                                element);

                            containsSummaryElement = true;
                        }

                        if (containsInheritDoc && containsSummaryElement)
                            break;
                    }
                }
            }

            if (!containsSummaryElement
                && !containsInheritDoc
                && !containsInclude)
            {
                context.ReportDiagnostic(
                    DiagnosticDescriptors.AddSummaryElementToDocumentationComment,
                    documentationComment);
            }
        }

        private static bool IsSummary(string name)
        {
            return string.Equals(name, "summary", StringComparison.Ordinal);
        }

        private static bool IsInheritDoc(string name)
        {
            return string.Equals(name, "inheritdoc", StringComparison.Ordinal);
        }

        private static bool IsInclude(string name)
        {
            return string.Equals(name, "include", StringComparison.Ordinal);
        }

        private static bool IsSummaryMissing(XmlElementSyntax summaryElement)
        {
            SyntaxList<XmlNodeSyntax> content = summaryElement.Content;

            if (content.Count == 0)
            {
                return true;
            }
            else if (content.Count == 1)
            {
                XmlNodeSyntax node = content.First();

                if (node.IsKind(SyntaxKind.XmlText))
                {
                    var xmlText = (XmlTextSyntax)node;

                    return xmlText.TextTokens.All(f => IsWhitespaceOrNewLine(f));
                }
            }

            return false;
        }

        private static bool IsWhitespaceOrNewLine(SyntaxToken token)
        {
            switch (token.Kind())
            {
                case SyntaxKind.XmlTextLiteralNewLineToken:
                    return true;
                case SyntaxKind.XmlTextLiteralToken:
                    return string.IsNullOrWhiteSpace(token.ValueText);
                default:
                    return false;
            }
        }

        public static async Task<Document> RefactorAsync(
            Document document,
            DocumentationCommentTriviaSyntax documentationComment,
            CancellationToken cancellationToken)
        {
            XmlElementSyntax summaryElement = documentationComment.SummaryElement();

            if (summaryElement == null)
            {
                SyntaxList<XmlNodeSyntax> content = documentationComment.Content;

                SourceText text = await document.GetTextAsync(cancellationToken).ConfigureAwait(false);

                TextLine line = text.Lines[documentationComment.GetFullSpanStartLine(cancellationToken)];

                string indent = StringUtility.GetIndent(line.ToString());

                string newText = CreateSummaryElement(indent);

                var textChange = new TextChange(new TextSpan(documentationComment.FullSpan.Start, 0), newText);

                return await document.WithTextChangeAsync(textChange).ConfigureAwait(false);
            }

            Debug.Assert(false, "");

            return document;
        }

        private static string CreateSummaryElement(string indent)
        {
            var sb = new StringBuilder();

            sb.AppendLine("/// <summary>");
            sb.Append(indent);
            sb.AppendLine("/// ");
            sb.Append(indent);
            sb.AppendLine("/// </summary>");
            sb.Append(indent);

            return sb.ToString();
        }
    }
}