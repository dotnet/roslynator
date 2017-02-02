// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Immutable;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Text;
using Roslynator.CSharp.Analysis;
using Roslynator.CSharp.Extensions;
using Roslynator.Extensions;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Roslynator.CSharp.Refactorings
{
    internal static class CallStringConcatInsteadOfStringJoinRefactoring
    {
        public static void Analyze(SyntaxNodeAnalysisContext context, InvocationExpressionSyntax invocation)
        {
            ExpressionSyntax expression = invocation.Expression;

            if (expression?.IsKind(SyntaxKind.SimpleMemberAccessExpression) == true)
            {
                var memberAccess = (MemberAccessExpressionSyntax)expression;

                ArgumentListSyntax argumentList = invocation.ArgumentList;

                if (argumentList != null)
                {
                    SeparatedSyntaxList<ArgumentSyntax> arguments = argumentList.Arguments;

                    if (arguments.Any())
                    {
                        SimpleNameSyntax name = memberAccess.Name;

                        if (name?.Identifier.ValueText == "Join")
                        {
                            SemanticModel semanticModel = context.SemanticModel;
                            CancellationToken cancellationToken = context.CancellationToken;

                            IMethodSymbol methodSymbol = semanticModel.GetMethodSymbol(invocation, cancellationToken);

                            INamedTypeSymbol stringSymbol = semanticModel.Compilation.GetSpecialType(SpecialType.System_String);

                            if (methodSymbol != null
                                && Symbol.IsMethod(
                                    methodSymbol: methodSymbol,
                                    containingType: stringSymbol,
                                    accessibility: Accessibility.Public,
                                    isStatic: true,
                                    returnType: stringSymbol,
                                    name: "Join",
                                    arity: 0))
                            {
                                ImmutableArray<IParameterSymbol> parameters = methodSymbol.Parameters;

                                if (parameters.Length == 2
                                    && parameters[0].Type.Equals(stringSymbol))
                                {
                                    IParameterSymbol parameter = parameters[1];

                                    if (parameter.IsParamsOf(stringSymbol)
                                        || parameter.IsParamsOf(semanticModel.Compilation.ObjectType)
                                        || parameter.Type.IsConstructedFromIEnumerableOfT())
                                    {
                                        ArgumentSyntax firstArgument = arguments.First();
                                        ExpressionSyntax argumentExpression = firstArgument.Expression;

                                        if (argumentExpression != null
                                            && CSharpAnalysis.IsEmptyString(argumentExpression, semanticModel, cancellationToken)
                                            && !invocation.ContainsDirectives(TextSpan.FromBounds(invocation.SpanStart, firstArgument.Span.End)))
                                        {
                                            context.ReportDiagnostic(
                                                DiagnosticDescriptors.CallStringConcatInsteadOfStringJoin,
                                                name.GetLocation());
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        public static async Task<Document> RefactorAsync(
            Document document,
            InvocationExpressionSyntax invocation,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            var memberAccess = (MemberAccessExpressionSyntax)invocation.Expression;

            MemberAccessExpressionSyntax newMemberAccess = memberAccess.WithName(IdentifierName("Concat").WithTriviaFrom(memberAccess.Name));

            ArgumentListSyntax argumentList = invocation.ArgumentList;
            SeparatedSyntaxList<ArgumentSyntax> arguments = argumentList.Arguments;

            ArgumentListSyntax newArgumentList = argumentList
                .WithArguments(arguments.RemoveAt(0))
                .WithOpenParenToken(argumentList.OpenParenToken.AppendToTrailingTrivia(arguments[0].GetLeadingAndTrailingTrivia()));

            InvocationExpressionSyntax newInvocation = invocation
                .WithExpression(newMemberAccess)
                .WithArgumentList(newArgumentList);

            return await document.ReplaceNodeAsync(invocation, newInvocation, cancellationToken).ConfigureAwait(false);
        }
    }
}
