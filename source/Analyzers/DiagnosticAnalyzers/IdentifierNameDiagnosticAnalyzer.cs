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
    public class IdentifierNameDiagnosticAnalyzer : BaseDiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        {
            get { return ImmutableArray.Create(DiagnosticDescriptors.UsePredefinedType); }
        }

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
                && !identifierName.IsParentKind(
                    SyntaxKind.SimpleMemberAccessExpression,
                    SyntaxKind.QualifiedName,
                    SyntaxKind.UsingDirective))
            {
                var namedTypeSymbol = context.SemanticModel
                    .GetSymbolInfo(identifierName, context.CancellationToken)
                    .Symbol as INamedTypeSymbol;

                if (namedTypeSymbol?.IsPredefinedType() == true)
                {
                    IAliasSymbol aliasSymbol = context.SemanticModel.GetAliasInfo(identifierName, context.CancellationToken);

                    if (aliasSymbol == null)
                    {
                        context.ReportDiagnostic(
                            DiagnosticDescriptors.UsePredefinedType,
                            identifierName.GetLocation());
                    }
                }
            }
        }
    }
}
