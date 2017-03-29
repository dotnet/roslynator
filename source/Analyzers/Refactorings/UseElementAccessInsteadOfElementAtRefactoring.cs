// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Text;
using Roslynator.CSharp.Extensions;
using Roslynator.Diagnostics.Extensions;
using Roslynator.Extensions;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static Roslynator.CSharp.CSharpFactory;

namespace Roslynator.CSharp.Refactorings
{
    internal static class UseElementAccessInsteadOfElementAtRefactoring
    {
        internal static void Analyze(SyntaxNodeAnalysisContext context, InvocationExpressionSyntax invocation, ArgumentListSyntax argumentList, MemberAccessExpressionSyntax memberAccess)
        {
            ExpressionSyntax argumentExpression = argumentList.Arguments[0].Expression;
            if (argumentExpression?.IsMissing == false)
            {
                ExpressionSyntax memberAccessExpression = memberAccess.Expression;
                if (memberAccessExpression?.IsMissing == false)
                {
                    SemanticModel semanticModel = context.SemanticModel;
                    CancellationToken cancellationToken = context.CancellationToken;

                    if (semanticModel
                        .GetExtensionMethodInfo(invocation, cancellationToken)
                        .IsLinqElementAt(allowImmutableArrayExtension: true))
                    {
                        ITypeSymbol typeSymbol = semanticModel.GetTypeSymbol(memberAccessExpression, cancellationToken);

                        if (typeSymbol?.IsErrorType() == false
                            && (typeSymbol.IsArrayType() || ExistsApplicableIndexer(invocation, argumentExpression, typeSymbol, semanticModel)))
                        {
                            context.ReportDiagnostic(DiagnosticDescriptors.UseElementAccessInsteadOfElementAt, memberAccess.Name);
                        }
                    }
                }
            }
        }

        private static bool ExistsApplicableIndexer(
            ExpressionSyntax expression,
            ExpressionSyntax argumentExpression,
            ITypeSymbol containingType,
            SemanticModel semanticModel)
        {
            foreach (ISymbol member in containingType.GetMembers("this[]"))
            {
                var propertySymbol = (IPropertySymbol)member;

                if (!propertySymbol.IsWriteOnly
                    && semanticModel.IsAccessible(expression.SpanStart, propertySymbol.GetMethod)
                    && semanticModel.IsImplicitConversion(expression, propertySymbol.Type))
                {
                    ImmutableArray<IParameterSymbol> parameters = propertySymbol.Parameters;

                    if (parameters.Length == 1
                        && semanticModel.IsImplicitConversion(argumentExpression, parameters[0].Type))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public static Task<Document> RefactorAsync(
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

            return document.ReplaceNodeAsync(invocation, elementAccess, cancellationToken);
        }
    }
}