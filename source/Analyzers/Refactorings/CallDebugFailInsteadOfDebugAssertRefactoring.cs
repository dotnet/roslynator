// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using static Roslynator.CSharp.CSharpFactory;

namespace Roslynator.CSharp.Refactorings
{
    internal static class CallDebugFailInsteadOfDebugAssertRefactoring
    {
        public static void AnalyzeInvocationExpression(SyntaxNodeAnalysisContext context, INamedTypeSymbol debugSymbol)
        {
            var invocation = (InvocationExpressionSyntax)context.Node;

            ExpressionSyntax expression = invocation.Expression;

            if (expression == null)
                return;

            if (invocation.SpanContainsDirectives())
                return;

            if (!CanRefactor(invocation, debugSymbol, context.SemanticModel, context.CancellationToken))
                return;

            if (expression.Kind() != SyntaxKind.SimpleMemberAccessExpression
                && context.SemanticModel
                    .GetSpeculativeMethodSymbol(invocation.SpanStart, GetNewInvocation(invocation))?
                    .ContainingType?
                    .Equals(debugSymbol) != true)
            {
                return;
            }

            SyntaxKind kind = expression.Kind();

            if (kind == SyntaxKind.SimpleMemberAccessExpression)
                expression = ((MemberAccessExpressionSyntax)expression).Name;

            Debug.Assert(expression.Kind() == SyntaxKind.IdentifierName, kind.ToString());

            context.ReportDiagnostic(DiagnosticDescriptors.CallDebugFailInsteadOfDebugAssert, expression);
        }

        public static bool CanRefactor(
            InvocationExpressionSyntax invocation,
            INamedTypeSymbol debugSymbol,
            SemanticModel semanticModel,
            CancellationToken cancellationToken)
        {
            ArgumentListSyntax argumentList = invocation.ArgumentList;

            if (argumentList == null)
                return false;

            SeparatedSyntaxList<ArgumentSyntax> arguments = argumentList.Arguments;

            if (arguments.Count >= 1
                && arguments.Count <= 3
                && arguments[0].Expression?.Kind() == SyntaxKind.FalseLiteralExpression
                && semanticModel.TryGetMethodInfo(invocation, out MethodInfo info, cancellationToken)
                && info.ContainingType?.Equals(debugSymbol) == true
                && info.IsName("Assert")
                && info.IsStatic
                && info.ReturnsVoid
                && !info.IsGenericMethod)
            {
                ImmutableArray<IParameterSymbol> assertParameters = info.Parameters;

                int length = assertParameters.Length;

                if (assertParameters[0].Type.IsBoolean())
                {
                    for (int i = 1; i < length; i++)
                    {
                        if (!assertParameters[i].Type.IsString())
                            return false;
                    }

                    int parameterCount = (length == 1) ? 1 : length - 1;

                    foreach (ISymbol symbol in info.ContainingType.GetMembers("Fail"))
                    {
                        if (symbol is IMethodSymbol methodSymbol
                            && methodSymbol.IsPublic()
                            && methodSymbol.IsStatic
                            && methodSymbol.ReturnsVoid
                            && !methodSymbol.IsGenericMethod)
                        {
                            ImmutableArray<IParameterSymbol> failParameters = methodSymbol.Parameters;

                            if (failParameters.Length == parameterCount
                                && failParameters.All(f => f.Type.IsString()))
                            {
                                return true;
                            }
                        }
                    }
                }
            }

            return false;
        }

        public static Task<Document> RefactorAsync(
            Document document,
            InvocationExpressionSyntax invocation,
            CancellationToken cancellationToken)
        {
            InvocationExpressionSyntax newNode = GetNewInvocation(invocation).WithFormatterAnnotation();

            return document.ReplaceNodeAsync(invocation, newNode, cancellationToken);
        }

        private static InvocationExpressionSyntax GetNewInvocation(InvocationExpressionSyntax invocation)
        {
            ArgumentListSyntax argumentList = invocation.ArgumentList;
            SeparatedSyntaxList<ArgumentSyntax> arguments = argumentList.Arguments;

            if (arguments.Count == 1)
            {
                ArgumentSyntax argument = arguments[0];
                arguments = arguments.ReplaceAt(0, argument.WithExpression(StringLiteralExpression("").WithTriviaFrom(argument.Expression)));
            }
            else
            {
                arguments = arguments.RemoveAt(0);
            }

            return RefactoringHelper.ChangeInvokedMethodName(invocation, "Fail")
                .WithArgumentList(argumentList.WithArguments(arguments));
        }
    }
}
