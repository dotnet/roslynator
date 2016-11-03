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
    public class GenericNameDiagnosticAnalyzer : BaseDiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
            => ImmutableArray.Create(DiagnosticDescriptors.SimplifyNullableOfT);

        public override void Initialize(AnalysisContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            context.RegisterSyntaxNodeAction(f => AnalyzeIdentifierName(f), SyntaxKind.GenericName);
        }

        private void AnalyzeIdentifierName(SyntaxNodeAnalysisContext context)
        {
            if (GeneratedCodeAnalyzer?.IsGeneratedCode(context) == true)
                return;

            if (context.Node.Parent?.IsKind(SyntaxKind.QualifiedName) == true)
                return;

            if (context.Node.Parent?.IsKind(SyntaxKind.UsingDirective) == true)
                return;

            var type = (GenericNameSyntax)context.Node;

            if (type.TypeArgumentList?.Arguments.Count != 1)
                return;

            if (type.TypeArgumentList.Arguments[0].IsKind(SyntaxKind.OmittedTypeArgument))
                return;

            var namedTypeSymbol = context.SemanticModel.GetSymbolInfo(type, context.CancellationToken).Symbol as INamedTypeSymbol;

            if (namedTypeSymbol == null)
                return;

            if (namedTypeSymbol.ConstructedFrom.SpecialType == SpecialType.System_Nullable_T)
            {
                context.ReportDiagnostic(
                    DiagnosticDescriptors.SimplifyNullableOfT,
                    context.Node.GetLocation());
            }
        }
    }
}
