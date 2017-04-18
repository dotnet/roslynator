// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static Roslynator.CSharp.CSharpFactory;

namespace Roslynator.CSharp.Refactorings
{
    internal static class UseElementAccessInsteadOfLastRefactoring
    {
        public static bool CanRefactor(InvocationExpressionSyntax invocation, MemberAccessExpressionSyntax memberAccess, SemanticModel semanticModel, CancellationToken cancellationToken)
        {
            ExpressionSyntax memberAccessExpression = memberAccess.Expression;

            if (memberAccessExpression?.IsMissing == false
                && semanticModel
                    .GetExtensionMethodInfo(invocation, ExtensionMethodKind.Reduced, cancellationToken)
                    .MethodInfo
                    .IsLinqExtensionOfIEnumerableOfTWithoutParameters("Last", allowImmutableArrayExtension: true))
            {
                ITypeSymbol typeSymbol = semanticModel.GetTypeSymbol(memberAccessExpression, cancellationToken);

                if (typeSymbol?.IsErrorType() == false
                    && (typeSymbol.IsArrayType() || ExistsApplicableIndexer(invocation, typeSymbol, semanticModel)))
                {
                    return true;
                }
            }

            return false;
        }

        private static bool ExistsApplicableIndexer(
            ExpressionSyntax expression,
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
                    switch (propertySymbol.SingleParameterOrDefault()?.Type.SpecialType)
                    {
                        case SpecialType.System_SByte:
                        case SpecialType.System_Byte:
                        case SpecialType.System_Int16:
                        case SpecialType.System_UInt16:
                        case SpecialType.System_Int32:
                        case SpecialType.System_UInt32:
                        case SpecialType.System_Int64:
                        case SpecialType.System_UInt64:
                            return true;
                    }
                }
            }

            return false;
        }

        public static string GetCountOrLengthPropertyName(
            ExpressionSyntax expression,
            SemanticModel semanticModel,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            ITypeSymbol typeSymbol = semanticModel.GetTypeSymbol(expression, cancellationToken);

            if (typeSymbol?.IsErrorType() == false
                && !typeSymbol.IsConstructedFromIEnumerableOfT())
            {
                if (typeSymbol.IsArrayType()
                    || typeSymbol.IsString()
                    || typeSymbol.IsConstructedFromImmutableArrayOfT(semanticModel))
                {
                    return "Length";
                }

                if (typeSymbol.ImplementsICollectionOfT())
                    return "Count";
            }

            return null;
        }

        public static Task<Document> RefactorAsync(
            Document document,
            InvocationExpressionSyntax invocation,
            string propertyName,
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

            ExpressionSyntax argumentExpression = SubtractExpression(
                SimpleMemberAccessExpression(
                    expression,
                    IdentifierName(propertyName)),
                NumericLiteralExpression(1));

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