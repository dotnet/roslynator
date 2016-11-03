// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Roslynator.CSharp.DiagnosticAnalyzers
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

            context.RegisterSyntaxNodeAction(f => AnalyzeQualifiedName(f), SyntaxKind.QualifiedName);
        }

        private void AnalyzeQualifiedName(SyntaxNodeAnalysisContext context)
        {
            if (GeneratedCodeAnalyzer?.IsGeneratedCode(context) == true)
                return;

            if (!context.Node.IsParentKind(SyntaxKind.UsingDirective))
            {
                var qualifiedName = (QualifiedNameSyntax)context.Node;

                var namedTypeSymbol = context.SemanticModel.GetSymbolInfo(qualifiedName, context.CancellationToken).Symbol as INamedTypeSymbol;

                if (namedTypeSymbol != null)
                {
                    if (namedTypeSymbol.IsPredefinedType())
                    {
                        context.ReportDiagnostic(
                            DiagnosticDescriptors.UsePredefinedType,
                            qualifiedName.GetLocation());
                    }
                    else if (namedTypeSymbol.ConstructedFrom.SpecialType == SpecialType.System_Nullable_T)
                    {
                        context.ReportDiagnostic(
                            DiagnosticDescriptors.SimplifyNullableOfT,
                            qualifiedName.GetLocation());
                    }
                }
            }
        }
    }
}
