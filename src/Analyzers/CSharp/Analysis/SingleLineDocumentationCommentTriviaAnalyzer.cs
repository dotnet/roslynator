// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Immutable;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Roslynator.CSharp.Syntax;

namespace Roslynator.CSharp.Analysis
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class SingleLineDocumentationCommentTriviaAnalyzer : BaseDiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        {
            get
            {
                return ImmutableArray.Create(
                    DiagnosticDescriptors.AddSummaryToDocumentationComment,
                    DiagnosticDescriptors.AddSummaryElementToDocumentationComment,
                    DiagnosticDescriptors.UnusedElementInDocumentationComment);
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

            if (!documentationComment.IsPartOfMemberDeclaration())
                return;

            bool containsInheritDoc = false;
            bool containsIncludeOrExclude = false;
            bool containsSummaryElement = false;
            bool containsContentElement = false;
            bool isFirst = true;

            CancellationToken cancellationToken = context.CancellationToken;

            foreach (XmlNodeSyntax node in documentationComment.Content)
            {
                cancellationToken.ThrowIfCancellationRequested();

                XmlElementInfo info = SyntaxInfo.XmlElementInfo(node);
                if (info.Success)
                {
                    switch (info.GetElementKind())
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
                        case XmlElementKind.Content:
                            {
                                containsContentElement = true;
                                break;
                            }
                        case XmlElementKind.Summary:
                            {
                                if (!context.IsAnalyzerSuppressed(DiagnosticDescriptors.AddSummaryToDocumentationComment)
                                    && info.IsContentEmptyOrWhitespace)
                                {
                                    context.ReportDiagnostic(
                                        DiagnosticDescriptors.AddSummaryToDocumentationComment,
                                        info.Element);
                                }

                                containsSummaryElement = true;
                                break;
                            }
                        case XmlElementKind.Code:
                        case XmlElementKind.Example:
                        case XmlElementKind.Remarks:
                        case XmlElementKind.Returns:
                        case XmlElementKind.Value:
                            {
                                if (!context.IsAnalyzerSuppressed(DiagnosticDescriptors.UnusedElementInDocumentationComment)
                                    && info.IsContentEmptyOrWhitespace)
                                {
                                    context.ReportDiagnostic(
                                        DiagnosticDescriptors.UnusedElementInDocumentationComment,
                                        info.Element);
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

            if (!containsSummaryElement
                && !containsInheritDoc
                && !containsIncludeOrExclude
                && !containsContentElement
                && !context.IsAnalyzerSuppressed(DiagnosticDescriptors.AddSummaryElementToDocumentationComment))
            {
                context.ReportDiagnostic(
                    DiagnosticDescriptors.AddSummaryElementToDocumentationComment,
                    documentationComment);
            }
        }
    }
}
