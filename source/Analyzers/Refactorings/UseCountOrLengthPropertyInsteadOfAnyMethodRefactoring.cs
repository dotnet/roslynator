// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Text;
using Roslynator.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static Roslynator.CSharp.CSharpFactory;

namespace Roslynator.CSharp.Refactorings
{
    internal static class UseCountOrLengthPropertyInsteadOfAnyMethodRefactoring
    {
        public static void Analyze(SyntaxNodeAnalysisContext context, MemberInvocationExpressionInfo invocationInfo)
        {
            InvocationExpressionSyntax invocationExpression = invocationInfo.InvocationExpression;

            if (invocationExpression.IsParentKind(SyntaxKind.SimpleMemberAccessExpression))
                return;

            SemanticModel semanticModel = context.SemanticModel;
            CancellationToken cancellationToken = context.CancellationToken;

            if (!semanticModel.TryGetExtensionMethodInfo(invocationExpression, out MethodInfo methodInfo, ExtensionMethodKind.Reduced, cancellationToken))
                return;

            if (!methodInfo.IsLinqExtensionOfIEnumerableOfTWithoutParameters("Any"))
                return;

            string propertyName = CSharpUtility.GetCountOrLengthPropertyName(invocationInfo.Expression, semanticModel, cancellationToken);

            if (propertyName == null)
                return;

            context.ReportDiagnostic(
                DiagnosticDescriptors.UseCountOrLengthPropertyInsteadOfAnyMethod,
                Location.Create(context.Node.SyntaxTree, TextSpan.FromBounds(invocationInfo.Name.Span.Start, invocationExpression.Span.End)),
                ImmutableDictionary.CreateRange(new KeyValuePair<string, string>[] { new KeyValuePair<string, string>("PropertyName", propertyName) }),
                propertyName);
        }

        public static Task<Document> RefactorAsync(
            Document document,
            InvocationExpressionSyntax invocation,
            string propertyName,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            var memberAccess = (MemberAccessExpressionSyntax)invocation.Expression;

            memberAccess = memberAccess
                .WithName(IdentifierName(propertyName).WithTriviaFrom(memberAccess.Name))
                .AppendToTrailingTrivia(invocation.ArgumentList.DescendantTrivia().Where(f => !f.IsWhitespaceOrEndOfLineTrivia()));

            if (invocation.IsParentKind(SyntaxKind.LogicalNotExpression))
            {
                var logicalNot = (PrefixUnaryExpressionSyntax)invocation.Parent;

                IEnumerable<SyntaxTrivia> leadingTrivia = logicalNot.GetLeadingTrivia()
                    .Concat(logicalNot.OperatorToken.TrailingTrivia.Where(f => !f.IsWhitespaceOrEndOfLineTrivia()))
                    .Concat(logicalNot.Operand.GetLeadingTrivia().Where(f => !f.IsWhitespaceOrEndOfLineTrivia()));

                BinaryExpressionSyntax newNode = EqualsExpression(memberAccess, NumericLiteralExpression(0))
                    .WithLeadingTrivia(leadingTrivia)
                    .WithTrailingTrivia(logicalNot.GetTrailingTrivia());

                return document.ReplaceNodeAsync(invocation.Parent, newNode, cancellationToken);
            }
            else
            {
                BinaryExpressionSyntax newNode = GreaterThanExpression(memberAccess, NumericLiteralExpression(0))
                    .WithTriviaFrom(invocation);

                return document.ReplaceNodeAsync(invocation, newNode, cancellationToken);
            }
        }
    }
}
