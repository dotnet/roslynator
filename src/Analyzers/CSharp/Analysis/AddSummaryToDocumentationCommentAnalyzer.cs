// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Roslynator.CSharp.Syntax;

namespace Roslynator.CSharp.Analysis
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class AddSummaryToDocumentationCommentAnalyzer : BaseDiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        {
            get
            {
                return ImmutableArray.Create(
                    DiagnosticDescriptors.AddSummaryToDocumentationComment,
                    DiagnosticDescriptors.AddSummaryElementToDocumentationComment);
            }
        }

        public override void Initialize(AnalysisContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            base.Initialize(context);

            context.RegisterSyntaxNodeAction(AnalyzeSingleLineDocumentationCommentTrivia, SyntaxKind.SingleLineDocumentationCommentTrivia);
        }

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
                                if (info.IsEmptyElement || IsSummaryMissing((XmlElementSyntax)info.Element))
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
            SyntaxNode node = (documentationComment as IStructuredTriviaSyntax)?.ParentTrivia.Token.Parent;

            if (node is MemberDeclarationSyntax)
                return true;

            return node is AttributeListSyntax
                && node.Parent is MemberDeclarationSyntax;
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
    }
}
