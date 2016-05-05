// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Pihrtsoft.CodeAnalysis.CSharp.DiagnosticAnalyzers
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class QualifiedNameDiagnosticAnalyzer : BaseDiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        {
            get
            {
                return ImmutableArray.Create(
                    DiagnosticDescriptors.UsePredefinedType,
                    DiagnosticDescriptors.SimplifyNullableOfT);
            }
        }

        public override void Initialize(AnalysisContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            context.RegisterSyntaxNodeAction(f => AnalyzeIdentifierName(f), SyntaxKind.QualifiedName);
        }

        private void AnalyzeIdentifierName(SyntaxNodeAnalysisContext context)
        {
            if (GeneratedCodeAnalyzer?.IsGeneratedCode(context) == true)
                return;

            if (context.Node.Parent?.IsKind(SyntaxKind.UsingDirective) == true)
                return;

            var type = (QualifiedNameSyntax)context.Node;

            var namedTypeSymbol = context.SemanticModel.GetSymbolInfo(type, context.CancellationToken).Symbol as INamedTypeSymbol;

            if (namedTypeSymbol == null)
                return;

            if (namedTypeSymbol.IsPredefinedType())
            {
                Diagnostic diagnostic = Diagnostic.Create(
                    DiagnosticDescriptors.UsePredefinedType,
                    context.Node.GetLocation());

                context.ReportDiagnostic(diagnostic);
            }
            else if (namedTypeSymbol.ConstructedFrom.SpecialType == SpecialType.System_Nullable_T)
            {
                context.ReportDiagnostic(
                    DiagnosticDescriptors.SimplifyNullableOfT,
                    context.Node.GetLocation());
            }
        }
    }
}
