// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Text;
using Roslynator.CSharp.Extensions;
using Roslynator.Extensions;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static Roslynator.CSharp.CSharpFactory;

namespace Roslynator.CSharp.Refactorings
{
    internal static class UseElementAccessInsteadOfElementAtRefactoring
    {
        internal static void Analyze(SyntaxNodeAnalysisContext context, InvocationExpressionSyntax invocation, MemberAccessExpressionSyntax memberAccess)
        {
            IMethodSymbol methodSymbol = context.SemanticModel.GetMethodSymbol(invocation, context.CancellationToken);

            if (methodSymbol != null
                && (Symbol.IsEnumerableOrImmutableArrayExtensionMethod(methodSymbol, "ElementAt", context.SemanticModel)
                && methodSymbol.SingleParameterOrDefault()?.Type.IsInt32() == true))
            {
                ITypeSymbol typeSymbol = context.SemanticModel.GetTypeSymbol(memberAccess.Expression, context.CancellationToken);

                if (typeSymbol != null
                    && (typeSymbol.IsArrayType() || Symbol.ContainsPublicIndexerWithInt32Parameter(typeSymbol)))
                {
                    context.ReportDiagnostic(
                        DiagnosticDescriptors.UseElementAccessInsteadOfElementAt,
                        memberAccess.Name.GetLocation());
                }
            }
        }

        public static async Task<Document> RefactorAsync(
            Document document,
            InvocationExpressionSyntax invocation,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            ArgumentListSyntax argumentList = invocation.ArgumentList;

            var memberAccess = (MemberAccessExpressionSyntax)invocation.Expression;

            ExpressionSyntax expression = memberAccess.Expression;

            IEnumerable<SyntaxTrivia> trivia = memberAccess.DescendantTrivia(TextSpan.FromBounds(memberAccess.Expression.Span.End, memberAccess.Name.FullSpan.End));

            if (trivia.All(f => f.IsWhitespaceOrEndOfLineTrivia()))
            {
                expression = expression.WithoutTrailingTrivia();
            }
            else
            {
                expression = expression.WithTrailingTrivia(trivia);
            }

            ExpressionSyntax argumentExpression = argumentList.Arguments[0].Expression;

            ElementAccessExpressionSyntax elementAccess = ElementAccessExpression(
                expression,
                BracketedArgumentList(
                    OpenBracketToken().WithTriviaFrom(argumentList.OpenParenToken),
                    SingletonSeparatedList(Argument(argumentExpression)),
                    CloseBracketToken().WithTriviaFrom(argumentList.CloseParenToken)));

            return await document.ReplaceNodeAsync(invocation, elementAccess, cancellationToken).ConfigureAwait(false);
        }
    }
}