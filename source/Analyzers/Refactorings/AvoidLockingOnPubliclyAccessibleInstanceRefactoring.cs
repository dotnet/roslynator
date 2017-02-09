// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Roslynator.CSharp.Extensions;
using Roslynator.Extensions;

namespace Roslynator.CSharp.Refactorings
{
    internal static class AvoidLockingOnPubliclyAccessibleInstanceRefactoring
    {
        public static void Analyze(SyntaxNodeAnalysisContext context, LockStatementSyntax lockStatement)
        {
            ExpressionSyntax expression = lockStatement.Expression;

            if (expression?.IsKind(SyntaxKind.ThisExpression, SyntaxKind.TypeOfExpression) == true)
            {
                ITypeSymbol typeSymbol = context.SemanticModel.GetTypeSymbol(expression, context.CancellationToken);

                if (typeSymbol != null)
                {
                    Accessibility accessibility = typeSymbol.DeclaredAccessibility;

                    if (accessibility == Accessibility.Public
                        || accessibility == Accessibility.Protected
                        || accessibility == Accessibility.ProtectedOrInternal)
                    {
                        context.ReportDiagnostic(
                            DiagnosticDescriptors.AvoidLockingOnPubliclyAccessibleInstance,
                            expression,
                            expression.ToString());
                    }
                }
            }
        }

        public static async Task<Document> RefactorAsync(
            Document document,
            LockStatementSyntax lockStatement,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            return await IntroduceFieldToLockOnRefactoring.RefactorAsync(document, lockStatement, cancellationToken).ConfigureAwait(false);
        }
    }
}
