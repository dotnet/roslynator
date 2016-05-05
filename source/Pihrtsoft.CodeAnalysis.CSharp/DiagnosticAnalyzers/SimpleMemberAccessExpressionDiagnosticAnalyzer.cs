// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Pihrtsoft.CodeAnalysis;

namespace Pihrtsoft.CodeAnalysis.CSharp.DiagnosticAnalyzers
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class SimpleMemberAccessExpressionDiagnosticAnalyzer : BaseDiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
            => ImmutableArray.Create(DiagnosticDescriptors.UsePredefinedType);

        public override void Initialize(AnalysisContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            context.RegisterSyntaxNodeAction(f => AnalyzeSimpleMemberAccessExpression(f), SyntaxKind.SimpleMemberAccessExpression);
        }

        private void AnalyzeSimpleMemberAccessExpression(SyntaxNodeAnalysisContext context)
        {
            if (GeneratedCodeAnalyzer?.IsGeneratedCode(context) == true)
                return;

            if (context.Node.Parent?.IsKind(SyntaxKind.SimpleMemberAccessExpression) == true)
                return;

            var memberAccessExpression = (MemberAccessExpressionSyntax)context.Node;

            ExpressionSyntax expression = memberAccessExpression.Expression;

            if (expression == null)
                return;

            if (expression.IsKind(SyntaxKind.PredefinedType))
                return;

            if (!expression.IsAnyKind(SyntaxKind.SimpleMemberAccessExpression, SyntaxKind.IdentifierName))
                return;

            ISymbol symbol = context.SemanticModel.GetSymbolInfo(expression, context.CancellationToken).Symbol;

            if (symbol == null)
                return;

            if (!symbol.IsKind(SymbolKind.NamedType))
                return;

            var namedTypeSymbol = (INamedTypeSymbol)symbol;

            if (namedTypeSymbol.IsPredefinedType())
            {
                Diagnostic diagnostic = Diagnostic.Create(
                    DiagnosticDescriptors.UsePredefinedType,
                    expression.GetLocation());

                context.ReportDiagnostic(diagnostic);
            }
        }
    }
}
