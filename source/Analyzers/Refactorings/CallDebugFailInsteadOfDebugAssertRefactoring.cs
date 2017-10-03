// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Roslynator.CSharp.Refactorings
{
    internal static class CallDebugFailInsteadOfDebugAssertRefactoring
    {
        public static void AnalyzeInvocationExpression(SyntaxNodeAnalysisContext context)
        {
            var invocation = (InvocationExpressionSyntax)context.Node;

            ExpressionSyntax expression = invocation.Expression;

            if (expression != null
                && CanRefactor(invocation, context.SemanticModel, context.CancellationToken)
                && !invocation.SpanContainsDirectives()
                && (expression.IsKind(SyntaxKind.SimpleMemberAccessExpression) || WillBindToDebugFail(context, invocation)))
            {
                context.ReportDiagnostic(
                    DiagnosticDescriptors.CallDebugFailInsteadOfDebugAssert,
                    GetName(expression));
            }
        }

        private static bool WillBindToDebugFail(SyntaxNodeAnalysisContext context, InvocationExpressionSyntax invocation)
        {
            InvocationExpressionSyntax newInvocation = GetNewInvocation(invocation);

            IMethodSymbol methodSymbol = context.SemanticModel.GetSpeculativeMethodSymbol(invocation.SpanStart, newInvocation);

            return methodSymbol?
                .ContainingType?
                .Equals(context.GetTypeByMetadataName(MetadataNames.System_Diagnostics_Debug)) == true;
        }

        public static bool CanRefactor(InvocationExpressionSyntax invocation, SemanticModel semanticModel, CancellationToken cancellationToken)
        {
            ArgumentListSyntax argumentList = invocation.ArgumentList;

            if (argumentList != null)
            {
                SeparatedSyntaxList<ArgumentSyntax> arguments = argumentList.Arguments;

                if (arguments.Count >= 2
                    && arguments.Count <= 3
                    && arguments[0].Expression?.IsKind(SyntaxKind.FalseLiteralExpression) == true)
                {
                    MethodInfo info;
                    if (semanticModel.TryGetMethodInfo(invocation, out info, cancellationToken)
                        && info.IsContainingType(MetadataNames.System_Diagnostics_Debug)
                        && info.IsName("Assert")
                        && info.IsStatic
                        && info.ReturnsVoid
                        && !info.IsGenericMethod)
                    {
                        INamedTypeSymbol containingType = info.ContainingType;

                        ImmutableArray<IParameterSymbol> parameters = info.Parameters;

                        int length = parameters.Length;

                        if (length > 1
                            && parameters[0].Type.IsBoolean()
                            && parameters[1].Type.IsString())
                        {
                            if (length == 2)
                            {
                                return ContainsFailMethod(containingType, methodSymbol => methodSymbol.Parameters.SingleOrDefault(throwException: false)?.Type.IsString() == true);
                            }
                            else if (length == 3
                                && parameters[2].Type.IsString())
                            {
                                return ContainsFailMethod(containingType, methodSymbol =>
                                {
                                    ImmutableArray<IParameterSymbol> parameters2 = methodSymbol.Parameters;

                                    return parameters2.Length == 2
                                        && parameters2[0].Type.IsString()
                                        && parameters2[1].Type.IsString();
                                });
                            }
                        }
                    }
                }
            }

            return false;
        }

        private static bool ContainsFailMethod(ITypeSymbol containingType, Func<IMethodSymbol, bool> predicate)
        {
            foreach (ISymbol symbol in containingType.GetMembers("Fail"))
            {
                if (symbol.IsMethod())
                {
                    var methodSymbol = (IMethodSymbol)symbol;

                    if (methodSymbol.IsPublic()
                        && methodSymbol.IsStatic
                        && methodSymbol.ReturnsVoid
                        && !methodSymbol.IsGenericMethod
                        && predicate(methodSymbol))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        private static ExpressionSyntax GetName(ExpressionSyntax expression)
        {
            SyntaxKind kind = expression.Kind();

            if (kind == SyntaxKind.SimpleMemberAccessExpression)
                return ((MemberAccessExpressionSyntax)expression).Name;

            Debug.Assert(expression.IsKind(SyntaxKind.IdentifierName), kind.ToString());

            return expression;
        }

        public static Task<Document> RefactorAsync(
            Document document,
            InvocationExpressionSyntax invocation,
            CancellationToken cancellationToken)
        {
            InvocationExpressionSyntax newNode = GetNewInvocation(invocation)
                .WithFormatterAnnotation();

            return document.ReplaceNodeAsync(invocation, newNode, cancellationToken);
        }

        private static InvocationExpressionSyntax GetNewInvocation(InvocationExpressionSyntax invocation)
        {
            ArgumentListSyntax argumentList = invocation.ArgumentList;
            SeparatedSyntaxList<ArgumentSyntax> arguments = argumentList.Arguments;

            return RefactoringHelper.ChangeInvokedMethodName(invocation, "Fail")
                .WithArgumentList(argumentList.WithArguments(arguments.RemoveAt(0)));
        }
    }
}
