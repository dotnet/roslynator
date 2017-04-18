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
    internal static class RemoveRedundantToStringCallRefactoring
    {
        public static void Analyze(SyntaxNodeAnalysisContext context, InvocationExpressionSyntax invocation)
        {
            if (CanRefactor(invocation, context.SemanticModel, context.CancellationToken))
            {
                var memberAccess = (MemberAccessExpressionSyntax)invocation.Expression;

                TextSpan span = TextSpan.FromBounds(memberAccess.OperatorToken.Span.Start, invocation.Span.End);

                if (!invocation.ContainsDirectives(span))
                    context.ReportDiagnostic(DiagnosticDescriptors.RemoveRedundantToStringCall, Location.Create(invocation.SyntaxTree, span));
            }
        }

        public static bool CanRefactor(
            InvocationExpressionSyntax invocation,
            SemanticModel semanticModel,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (invocation.ArgumentList?.Arguments.Any() == false)
            {
                ExpressionSyntax expression = invocation.Expression;

                if (expression?.IsKind(SyntaxKind.SimpleMemberAccessExpression) == true)
                {
                    var memberAccess = (MemberAccessExpressionSyntax)expression;

                    if (memberAccess.Name?.Identifier.ValueText.Equals("ToString", StringComparison.Ordinal) == true)
                    {
                        MethodInfo info = semanticModel.GetMethodInfo(invocation, cancellationToken);

                        if (info.IsValid
                            && info.IsName("ToString")
                            && info.IsPublic
                            && info.IsInstance
                            && info.IsReturnType(SpecialType.System_String)
                            && !info.IsGenericMethod
                            && !info.Parameters.Any())
                        {
                            if (info.IsContainingType(SpecialType.System_String))
                            {
                                return true;
                            }
                            else if (invocation.IsParentKind(SyntaxKind.Interpolation))
                            {
                                if (info.IsContainingType(SpecialType.System_Object))
                                {
                                    return true;
                                }
                                else if (info.IsOverride)
                                {
                                    return info.Symbol.OverriddenMethods().Any(f => f.ContainingType?.IsObject() == true);
                                }
                            }
                        }
                    }
                }
            }

            return false;
        }
    }
}