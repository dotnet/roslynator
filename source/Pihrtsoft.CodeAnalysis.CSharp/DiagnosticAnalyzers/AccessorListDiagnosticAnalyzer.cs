// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Pihrtsoft.CodeAnalysis.CSharp.DiagnosticAnalyzers
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

            if (accessorList.Accessors.All(f => f.Body == null))
                return;

            if (accessorList.IsSingleline(includeExteriorTrivia: false))
            {
                Diagnostic diagnostic = Diagnostic.Create(
                    DiagnosticDescriptors.FormatAccessorList,
                    accessorList.GetLocation());

                context.ReportDiagnostic(diagnostic);
            }
            else if (accessorList.Accessors.Any(accessor => ShouldBeFormatted(accessor)))
            {
                Diagnostic diagnostic = Diagnostic.Create(
                    DiagnosticDescriptors.FormatAccessorList,
                    accessorList.GetLocation());

                context.ReportDiagnostic(diagnostic);
            }
        }

        internal static bool ShouldBeFormatted(AccessorDeclarationSyntax accessor)
        {
            BlockSyntax body = accessor.Body;

            if (body == null)
                return false;

            if (body.Statements.Count > 1)
                return false;

            if (accessor.IsSingleline(includeExteriorTrivia: false))
                return false;

            if (body.Statements.Count == 0 || body.Statements[0].IsSingleline())
            {
                return accessor
                    .DescendantTrivia(accessor.Span, descendIntoTrivia: true)
                    .All(f => f.IsWhitespaceOrEndOfLine());
            }

            return false;
        }
    }
}
