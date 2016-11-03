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
    public class ThrowStatementDiagnosticAnalyzer : BaseDiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
            => ImmutableArray.Create(DiagnosticDescriptors.ThrowingOfNewNotImplementedException);

        public override void Initialize(AnalysisContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            context.RegisterSyntaxNodeAction(f => AnalyzeThrowStatement(f), SyntaxKind.ThrowStatement);
        }

        private void AnalyzeThrowStatement(SyntaxNodeAnalysisContext context)
        {
            if (GeneratedCodeAnalyzer?.IsGeneratedCode(context) == true)
                return;

            var throwStatement = (ThrowStatementSyntax)context.Node;

            if (throwStatement.Expression?.IsKind(SyntaxKind.ObjectCreationExpression) == true)
            {
                var objectCreationExpression = (ObjectCreationExpressionSyntax)throwStatement.Expression;

                if (objectCreationExpression.Type != null)
                {
                    ITypeSymbol typeSymbol = context
                        .SemanticModel
                        .GetTypeInfo(objectCreationExpression, context.CancellationToken)
                        .Type;

                    if (typeSymbol != null)
                    {
                        INamedTypeSymbol notImplementedExceptionSymbol = context.GetTypeByMetadataName("System.NotImplementedException");

                        if (typeSymbol.Equals(notImplementedExceptionSymbol))
                        {
                            context.ReportDiagnostic(
                                DiagnosticDescriptors.ThrowingOfNewNotImplementedException,
                                throwStatement.GetLocation());
                        }
                    }
                }
            }
        }
    }
}
