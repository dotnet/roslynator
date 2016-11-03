// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Text;

namespace Roslynator.CSharp.DiagnosticAnalyzers
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class AccessorListDiagnosticAnalyzer : BaseDiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
            => ImmutableArray.Create(DiagnosticDescriptors.FormatAccessorList);

        public override void Initialize(AnalysisContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            context.RegisterSyntaxNodeAction(f => AnalyzeAccessorList(f), SyntaxKind.AccessorList);
        }

        private void AnalyzeAccessorList(SyntaxNodeAnalysisContext context)
        {
            if (GeneratedCodeAnalyzer?.IsGeneratedCode(context) == true)
                return;

            var accessorList = (AccessorListSyntax)context.Node;

            if (accessorList.Accessors.Any(f => f.Body != null))
            {
                if (accessorList.IsSingleLine(includeExteriorTrivia: false))
                {
                    context.ReportDiagnostic(
                        DiagnosticDescriptors.FormatAccessorList,
                        accessorList.GetLocation());
                }
                else
                {
                    foreach (AccessorDeclarationSyntax accessor in accessorList.Accessors)
                    {
                        if (ShouldBeFormatted(accessor))
                        {
                            context.ReportDiagnostic(
                                DiagnosticDescriptors.FormatAccessorList,
                                accessor.GetLocation());
                        }
                    }
                }
            }
            else if (accessorList.Parent?.IsKind(SyntaxKind.PropertyDeclaration) == true
                && accessorList.Accessors.All(f => f.AttributeLists.Count == 0)
                && !accessorList.IsSingleLine(includeExteriorTrivia: false))
            {
                var propertyDeclaration = (PropertyDeclarationSyntax)accessorList.Parent;

                if (!propertyDeclaration.Identifier.IsMissing
                    && !accessorList.CloseBraceToken.IsMissing)
                {
                    TextSpan span = TextSpan.FromBounds(
                        propertyDeclaration.Identifier.Span.End,
                        accessorList.CloseBraceToken.Span.Start);

                    if (propertyDeclaration
                        .DescendantTrivia(span)
                        .All(f => f.IsWhitespaceOrEndOfLineTrivia()))
                    {
                        context.ReportDiagnostic(
                            DiagnosticDescriptors.FormatAccessorList,
                            accessorList.GetLocation());
                    }
                }
            }
        }

        internal static bool ShouldBeFormatted(AccessorDeclarationSyntax accessor)
        {
            BlockSyntax body = accessor.Body;

            if (body != null
                && body.Statements.Count <= 1
                && accessor.SyntaxTree.IsMultiLineSpan(TextSpan.FromBounds(accessor.Keyword.Span.Start, accessor.Span.End))
                && (body.Statements.Count == 0 || body.Statements[0].IsSingleLine()))
            {
                return accessor
                    .DescendantTrivia(accessor.Span, descendIntoTrivia: true)
                    .All(f => f.IsWhitespaceOrEndOfLineTrivia());
            }

            return false;
        }
    }
}
