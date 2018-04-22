// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using Roslynator.CodeFixes;
using Roslynator.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static Roslynator.CSharp.CSharpFactory;
using static Roslynator.CSharp.SyntaxInfo;

namespace Roslynator.CSharp.CodeFixes
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(SimplifyLinqMethodChainCodeFixProvider))]
    [Shared]
    public class SimplifyLinqMethodChainCodeFixProvider : BaseCodeFixProvider
    {
        private const string Title = "Simplify method chain";

        public sealed override ImmutableArray<string> FixableDiagnosticIds
        {
            get { return ImmutableArray.Create(DiagnosticIdentifiers.SimplifyLinqMethodChain); }
        }

        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            SyntaxNode root = await context.GetSyntaxRootAsync().ConfigureAwait(false);

            if (!TryFindFirstAncestorOrSelf(root, context.Span, out SyntaxNode node, predicate: f => f.IsKind(
                SyntaxKind.InvocationExpression,
                SyntaxKind.EqualsExpression,
                SyntaxKind.NotEqualsExpression,
                SyntaxKind.IsPatternExpression)))
            {
                return;
            }

            Diagnostic diagnostic = context.Diagnostics[0];

            switch (node.Kind())
            {
                case SyntaxKind.InvocationExpression:
                    {
                        var invocation = (InvocationExpressionSyntax)node;

                        var memberAccess = (MemberAccessExpressionSyntax)invocation.Expression;

                        string name = memberAccess.Name.Identifier.ValueText;

                        if (name == "Cast")
                        {
                            CodeAction codeAction = CodeAction.Create(
                                Title,
                                cancellationToken => CallOfTypeInsteadOfWhereAndCastAsync(context.Document, invocation, cancellationToken),
                                GetEquivalenceKey(diagnostic));

                            context.RegisterCodeFix(codeAction, diagnostic);
                        }
                        else if (name == "Any"
                            && invocation.ArgumentList.Arguments.Count == 1)
                        {
                            CodeAction codeAction = CodeAction.Create(
                                Title,
                                cancellationToken => CombineEnumerableWhereAndAnyAsync(context.Document, invocation, cancellationToken),
                                GetEquivalenceKey(diagnostic));

                            context.RegisterCodeFix(codeAction, diagnostic);
                        }
                        else
                        {
                            CodeAction codeAction = CodeAction.Create(
                                Title,
                                cancellationToken => SimplifyLinqMethodChainAsync(context.Document, invocation, cancellationToken),
                                GetEquivalenceKey(diagnostic));

                            context.RegisterCodeFix(codeAction, diagnostic);
                        }

                        break;
                    }
                case SyntaxKind.EqualsExpression:
                case SyntaxKind.NotEqualsExpression:
                case SyntaxKind.IsPatternExpression:
                    {
                        CodeAction codeAction = CodeAction.Create(
                            Title,
                            cancellationToken => SimplifyNullChckWithFirstOrDefault(context.Document, node, cancellationToken),
                            base.GetEquivalenceKey(diagnostic));

                        context.RegisterCodeFix(codeAction, diagnostic);
                        break;
                    }
            }
        }

        private static Task<Document> CallOfTypeInsteadOfWhereAndCastAsync(
            Document document,
            InvocationExpressionSyntax invocation,
            CancellationToken cancellationToken)
        {
            var memberAccess = (MemberAccessExpressionSyntax)invocation.Expression;

            var invocation2 = (InvocationExpressionSyntax)memberAccess.Expression;

            var memberAccess2 = (MemberAccessExpressionSyntax)invocation2.Expression;

            var genericName = (GenericNameSyntax)memberAccess.Name;

            InvocationExpressionSyntax newInvocation = invocation2.Update(
                memberAccess2.WithName(genericName.WithIdentifier(Identifier("OfType"))),
                invocation.ArgumentList.WithArguments(SeparatedList<ArgumentSyntax>()));

            return document.ReplaceNodeAsync(invocation, newInvocation, cancellationToken);
        }

        private static Task<Document> CombineEnumerableWhereAndAnyAsync(
            Document document,
            InvocationExpressionSyntax invocationExpression,
            CancellationToken cancellationToken)
        {
            SimpleMemberInvocationExpressionInfo invocationInfo = SimpleMemberInvocationExpressionInfo(invocationExpression);
            SimpleMemberInvocationExpressionInfo invocationInfo2 = SimpleMemberInvocationExpressionInfo(invocationInfo.Expression);

            SingleParameterLambdaExpressionInfo lambda = SingleParameterLambdaExpressionInfo((LambdaExpressionSyntax)invocationInfo.Arguments.First().Expression);
            SingleParameterLambdaExpressionInfo lambda2 = SingleParameterLambdaExpressionInfo((LambdaExpressionSyntax)invocationInfo2.Arguments.First().Expression);

            BinaryExpressionSyntax logicalAnd = LogicalAndExpression(
                ((ExpressionSyntax)lambda2.Body).Parenthesize(),
                ((ExpressionSyntax)lambda.Body).Parenthesize());

            InvocationExpressionSyntax newNode = invocationInfo2.InvocationExpression
                .ReplaceNode(invocationInfo2.Name, invocationInfo.Name.WithTriviaFrom(invocationInfo2.Name))
                .WithArgumentList(invocationInfo2.ArgumentList.ReplaceNode((ExpressionSyntax)lambda2.Body, logicalAnd));

            return document.ReplaceNodeAsync(invocationExpression, newNode, cancellationToken);
        }

        private static Task<Document> SimplifyLinqMethodChainAsync(
            Document document,
            InvocationExpressionSyntax invocation,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            var memberAccess = (MemberAccessExpressionSyntax)invocation.Expression;

            var invocation2 = (InvocationExpressionSyntax)memberAccess.Expression;

            var memberAccess2 = (MemberAccessExpressionSyntax)invocation2.Expression;

            InvocationExpressionSyntax newNode = invocation2.WithExpression(
                memberAccess2.WithName(memberAccess.Name.WithTriviaFrom(memberAccess2.Name)));

            IEnumerable<SyntaxTrivia> trivia = invocation.DescendantTrivia(TextSpan.FromBounds(invocation2.Span.End, invocation.Span.End));

            if (trivia.Any(f => !f.IsWhitespaceOrEndOfLineTrivia()))
            {
                newNode = newNode.WithTrailingTrivia(trivia.Concat(invocation.GetTrailingTrivia()));
            }
            else
            {
                newNode = newNode.WithTrailingTrivia(invocation.GetTrailingTrivia());
            }

            return document.ReplaceNodeAsync(invocation, newNode, cancellationToken);
        }

        private static Task<Document> SimplifyNullChckWithFirstOrDefault(
            Document document,
            SyntaxNode node,
            CancellationToken cancellationToken)
        {
            NullCheckExpressionInfo nullCheck = NullCheckExpressionInfo(node, NullCheckStyles.ComparisonToNull | NullCheckStyles.IsNull);

            var invocation = (InvocationExpressionSyntax)nullCheck.Expression;

            ExpressionSyntax newNode = RefactoringUtility.ChangeInvokedMethodName(invocation, "Any");

            if (node.IsKind(SyntaxKind.EqualsExpression, SyntaxKind.IsPatternExpression))
                newNode = LogicalNotExpression(newNode.TrimTrivia().Parenthesize());

            newNode = newNode.WithTriviaFrom(node);

            return document.ReplaceNodeAsync(node, newNode, cancellationToken);
        }
    }
}
