// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Roslynator.CSharp.Refactorings
{
    internal static class ThrowingOfNewNotImplementedExceptionRefactoring
    {
        public static void Analyze(SyntaxNodeAnalysisContext context, ThrowStatementSyntax throwStatement)
        {
            Analyze(context, throwStatement.Expression);
        }

        internal static void Analyze(SyntaxNodeAnalysisContext context, ThrowExpressionSyntax throwExpression)
        {
            Analyze(context, throwExpression.Expression);
        }

        private static void Analyze(SyntaxNodeAnalysisContext context, ExpressionSyntax expression)
        {
            if (expression?.IsKind(SyntaxKind.ObjectCreationExpression) == true)
            {
                var objectCreationExpression = (ObjectCreationExpressionSyntax)expression;

                ITypeSymbol typeSymbol = context.SemanticModel.GetTypeSymbol(objectCreationExpression, context.CancellationToken);

                if (typeSymbol != null)
                {
                    INamedTypeSymbol exceptionSymbol = context.GetTypeByMetadataName(MetadataNames.System_NotImplementedException);

                    if (typeSymbol.Equals(exceptionSymbol))
                    {
                        context.ReportDiagnostic(
                            DiagnosticDescriptors.ThrowingOfNewNotImplementedException,
                            expression);
                    }
                }
            }
        }
    }
}
