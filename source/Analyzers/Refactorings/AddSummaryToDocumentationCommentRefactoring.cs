// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Text;
using Roslynator.CSharp;
using Roslynator.CSharp.Syntax;
using Roslynator.Utilities;

namespace Roslynator.CSharp.Refactorings
{
    internal static class AddSummaryToDocumentationCommentRefactoring
    {
        public static void AnalyzeSingleLineDocumentationCommentTrivia(SyntaxNodeAnalysisContext context)
        {
            var documentationComment = (DocumentationCommentTriviaSyntax)context.Node;

            if (!IsPartOfMemberDeclaration(documentationComment))
                return;

            bool containsInheritDoc = false;
            bool containsIncludeOrExclude = false;
            bool containsSummaryElement = false;
            bool isFirst = true;

            foreach (XmlNodeSyntax node in documentationComment.Content)
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
                                containsInheritDoc = true;
                                break;
                            }
                        case XmlElementKind.Summary:
                            {
                                if (info.IsXmlEmptyElement || IsSummaryMissing((XmlElementSyntax)info.Element))
                                {
                                    context.ReportDiagnostic(
                                        DiagnosticDescriptors.AddSummaryToDocumentationComment,
                                        info.Element);
                                }

                                containsSummaryElement = true;
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

                    if (containsInheritDoc && containsSummaryElement)
                        break;
                }
            }

            if (!containsSummaryElement
                && !containsInheritDoc
                && !containsIncludeOrExclude)
            {
                context.ReportDiagnostic(
                    DiagnosticDescriptors.AddSummaryElementToDocumentationComment,
                    documentationComment);
            }
        }

        private static bool IsPartOfMemberDeclaration(DocumentationCommentTriviaSyntax documentationComment)
        {
            return (documentationComment as IStructuredTriviaSyntax)?.ParentTrivia.Token.Parent is MemberDeclarationSyntax;
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

                    return xmlText.TextTokens.All(IsWhitespaceOrNewLine);
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
            SyntaxList<XmlNodeSyntax> content = documentationComment.Content;

            SourceText sourceText = await document.GetTextAsync(cancellationToken).ConfigureAwait(false);

            TextLine line = sourceText.Lines[documentationComment.GetFullSpanStartLine(cancellationToken)];

            string indent = StringUtility.GetIndent(line.ToString());

            TextChange textChange;

            if (content.Count == 1
                && content[0].IsKind(SyntaxKind.XmlText))
            {
                string text = content[0].ToString().Trim();

                string newText = CreateSummaryElement(indent, text);

                textChange = new TextChange(documentationComment.FullSpan, newText);
            }
            else
            {
                string newText = CreateSummaryElement(indent);

                textChange = new TextChange(new TextSpan(documentationComment.FullSpan.Start, 0), newText);
            }

            return await document.WithTextChangeAsync(textChange).ConfigureAwait(false);
        }

        private static string CreateSummaryElement(string indent, string text = null)
        {
            var sb = new StringBuilder();

            sb.AppendLine("/// <summary>");
            sb.Append(indent);
            sb.Append("/// ");
            sb.AppendLine(text);
            sb.Append(indent);
            sb.AppendLine("/// </summary>");

            if (text == null)
                sb.Append(indent);

            return sb.ToString();
        }
    }
}