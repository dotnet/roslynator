// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Linq;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Text;
using Roslynator.CSharp;

namespace Roslynator.CSharp.Refactorings
{
    internal static class RemoveRedundantStringToCharArrayCallRefactoring
    {
        public static void Analyze(SyntaxNodeAnalysisContext context, InvocationExpressionSyntax invocation)
        {
            if (CanRefactor(invocation, context.SemanticModel, context.CancellationToken))
            {
                var memberAccess = (MemberAccessExpressionSyntax)invocation.Expression;

                TextSpan span = TextSpan.FromBounds(memberAccess.OperatorToken.Span.Start, invocation.Span.End);

                if (!invocation.ContainsDirectives(span))
                    context.ReportDiagnostic(DiagnosticDescriptors.RemoveRedundantStringToCharArrayCall, Location.Create(invocation.SyntaxTree, span));
            }
        }

        public static bool CanRefactor(
            InvocationExpressionSyntax invocation,
            SemanticModel semanticModel,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (ParentIsElementAccessOrForEachExpression(invocation)
                && invocation.ArgumentList?.Arguments.Any() == false)
            {
                ExpressionSyntax expression = invocation.Expression;

                if (expression?.IsKind(SyntaxKind.SimpleMemberAccessExpression) == true)
                {
                    var memberAccess = (MemberAccessExpressionSyntax)expression;

                    if (memberAccess.Name?.Identifier.ValueText.Equals("ToCharArray", StringComparison.Ordinal) == true)
                    {
                        MethodInfo info;
                        if (semanticModel.TryGetMethodInfo(invocation, out info, cancellationToken)
                            && info.IsName("ToCharArray")
                            && info.IsPublic
                            && !info.IsStatic
                            && !info.IsGenericMethod
                            && !info.Parameters.Any()
                            && info.IsContainingType(SpecialType.System_String))
                        {
                            ITypeSymbol returnType = info.ReturnType;

                            if (returnType?.IsArrayType() == true)
                            {
                                var arrayType = (IArrayTypeSymbol)returnType;

                                if (arrayType.ElementType?.IsChar() == true)
                                    return true;
                            }
                        }
                    }
                }
            }

            return false;
        }

        private static bool ParentIsElementAccessOrForEachExpression(InvocationExpressionSyntax invocation)
        {
            if (invocation.IsParentKind(SyntaxKind.ElementAccessExpression))
                return true;

            if (invocation.IsParentKind(SyntaxKind.ForEachStatement))
            {
                var forEachStatement = (ForEachStatementSyntax)invocation.Parent;

                if (invocation.Equals(forEachStatement.Expression))
                    return true;
            }

            return false;
        }
    }
}