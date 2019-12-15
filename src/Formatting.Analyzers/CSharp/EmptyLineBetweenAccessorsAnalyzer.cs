// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Immutable;
using System.Diagnostics;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Roslynator.CSharp;

namespace Roslynator.Formatting.CSharp
{
    //TODO: sloučit s AccessorListAnalyzer
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    internal class EmptyLineBetweenAccessorsAnalyzer : BaseDiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        {
            get
            {
                return ImmutableArray.Create(
                    DiagnosticDescriptors.AddEmptyLineBetweenAccessors,
                    DiagnosticDescriptors.AddEmptyLineBetweenSingleLineAccessors,
                    DiagnosticDescriptors.RemoveEmptyLineBetweenSingleLineAccessors);
            }
        }

        public override void Initialize(AnalysisContext context)
        {
            base.Initialize(context);

            context.RegisterSyntaxNodeAction(AnalyzeAccessorList, SyntaxKind.AccessorList);
        }

        private static void AnalyzeAccessorList(SyntaxNodeAnalysisContext context)
        {
            var accessorList = (AccessorListSyntax)context.Node;

            SyntaxList<AccessorDeclarationSyntax> accessors = accessorList.Accessors;

            if (accessors.Count <= 1)
                return;

            Debug.Assert(accessors.Count == 2, accessors.Count.ToString());

            AccessorDeclarationSyntax accessor1 = accessors[0];

            if (accessor1.BodyOrExpressionBody() == null)
                return;

            AccessorDeclarationSyntax accessor2 = accessors[1];

            if (accessor2.BodyOrExpressionBody() == null)
                return;

            SyntaxTriviaList trailingTrivia = accessor1.GetTrailingTrivia();

            if (!SyntaxTriviaAnalysis.IsOptionalWhitespaceThenOptionalSingleLineCommentThenEndOfLineTrivia(trailingTrivia))
                return;

            SyntaxTriviaList leadingTrivia = accessor2.GetLeadingTrivia();

            bool isEmptyLine = SyntaxTriviaAnalysis.StartsWithOptionalWhitespaceThenEndOfLineTrivia(leadingTrivia);

            if (accessorList.SyntaxTree.IsSingleLineSpan(accessor1.Span, context.CancellationToken)
                && accessorList.SyntaxTree.IsSingleLineSpan(accessor2.Span, context.CancellationToken))
            {
                if (isEmptyLine)
                {
                    ReportDiagnostic(context, DiagnosticDescriptors.RemoveEmptyLineBetweenSingleLineAccessors, leadingTrivia[0]);
                }
                else
                {
                    ReportDiagnostic(context, DiagnosticDescriptors.AddEmptyLineBetweenSingleLineAccessors, trailingTrivia.Last());
                }
            }
            else if (!isEmptyLine)
            {
                ReportDiagnostic(context, DiagnosticDescriptors.AddEmptyLineBetweenAccessors, trailingTrivia.Last());
            }
        }

        private static void ReportDiagnostic(
            SyntaxNodeAnalysisContext context,
            DiagnosticDescriptor descriptor,
            SyntaxTrivia trivia)
        {
            if (!context.IsAnalyzerSuppressed(descriptor))
                context.ReportDiagnostic(descriptor, Location.Create(context.Node.SyntaxTree, trivia.Span.WithLength(0)));
        }
    }
}
