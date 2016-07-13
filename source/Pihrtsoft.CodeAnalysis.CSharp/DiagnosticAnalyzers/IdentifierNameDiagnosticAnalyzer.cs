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
    public class IdentifierNameDiagnosticAnalyzer : BaseDiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
            => ImmutableArray.Create(DiagnosticDescriptors.UsePredefinedType);

        public override void Initialize(AnalysisContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            context.RegisterSyntaxNodeAction(f => AnalyzeIdentifierName(f), SyntaxKind.IdentifierName);
        }

        private void AnalyzeIdentifierName(SyntaxNodeAnalysisContext context)
        {
            if (GeneratedCodeAnalyzer?.IsGeneratedCode(context) == true)
                return;

            var identifierName = (IdentifierNameSyntax)context.Node;

            if (!identifierName.IsVar
                && identifierName.Parent != null
                && !identifierName.Parent.IsKind(SyntaxKind.SimpleMemberAccessExpression)
                && !identifierName.Parent.IsKind(SyntaxKind.QualifiedName)
                && !identifierName.Parent.IsKind(SyntaxKind.UsingDirective))
            {
                var namedTypeSymbol = context.SemanticModel.GetSymbolInfo(identifierName, context.CancellationToken).Symbol as INamedTypeSymbol;

                if (namedTypeSymbol?.IsPredefinedType() == true)
                {
                    context.ReportDiagnostic(
                        DiagnosticDescriptors.UsePredefinedType,
                        identifierName.GetLocation());
                }
            }
        }
    }
}
