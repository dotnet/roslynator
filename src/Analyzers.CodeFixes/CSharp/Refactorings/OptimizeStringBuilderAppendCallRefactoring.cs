// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static Roslynator.CSharp.CSharpFactory;

namespace Roslynator.CSharp.Refactorings
{
    internal static class OptimizeStringBuilderAppendCallRefactoring
    {
        public static Task<Document> RefactorAsync(
            Document document,
            SimpleMemberInvocationExpressionInfo invocationInfo,
            CancellationToken cancellationToken)
        {
            SimpleMemberInvocationExpressionInfo invocationInfo2 = SyntaxInfo.SimpleMemberInvocationExpressionInfo(invocationInfo.Expression);

            SyntaxTriviaList trivia = invocationInfo2.InvocationExpression
                .GetTrailingTrivia()
                .EmptyIfWhitespace()
                .AddRange(invocationInfo.InvocationExpression.GetTrailingTrivia());

            InvocationExpressionSyntax newNode = invocationInfo2
                .WithName("AppendLine")
                .InvocationExpression
                .WithTrailingTrivia(trivia);

            return document.ReplaceNodeAsync(invocationInfo.InvocationExpression, newNode, cancellationToken);
        }

        public static async Task<Document> RefactorAsync(
            Document document,
            ArgumentSyntax argument,
            SimpleMemberInvocationExpressionInfo invocationInfo,
            CancellationToken cancellationToken)
        {
            SemanticModel semanticModel = await document.GetSemanticModelAsync(cancellationToken).ConfigureAwait(false);

            InvocationExpressionSyntax invocation = invocationInfo.InvocationExpression;
            InvocationExpressionSyntax newInvocation = null;

            bool isAppendLine = string.Equals(invocationInfo.NameText, "AppendLine", StringComparison.Ordinal);

            ExpressionSyntax expression = argument.Expression;

            switch (expression.Kind())
            {
                case SyntaxKind.InterpolatedStringExpression:
                    {
                        newInvocation = ConvertInterpolatedStringExpressionToInvocationExpression((InterpolatedStringExpressionSyntax)argument.Expression, invocationInfo, semanticModel);
                        break;
                    }
                case SyntaxKind.AddExpression:
                    {
                        ImmutableArray<ExpressionSyntax> expressions = SyntaxInfo.BinaryExpressionInfo((BinaryExpressionSyntax)expression)
                            .Expressions(leftToRight: true)
                            .ToImmutableArray();

                        newInvocation = invocation
                            .ReplaceNode(invocationInfo.Name, IdentifierName("Append").WithTriviaFrom(invocationInfo.Name))
                            .WithArgumentList(invocation.ArgumentList.WithArguments(SingletonSeparatedList(Argument(expressions[0]))).WithoutTrailingTrivia());

                        for (int i = 1; i < expressions.Length; i++)
                        {
                            ExpressionSyntax argumentExpression = expressions[i];

                            string methodName;
                            if (i == expressions.Length - 1
                                && isAppendLine
                                && semanticModel
                                    .GetTypeInfo(argumentExpression, cancellationToken)
                                    .ConvertedType?
                                    .SpecialType == SpecialType.System_String)
                            {
                                methodName = "AppendLine";
                            }
                            else
                            {
                                methodName = "Append";
                            }

                            newInvocation = SimpleMemberInvocationExpression(
                                newInvocation,
                                IdentifierName(methodName),
                                ArgumentList(Argument(argumentExpression)));

                            if (i == expressions.Length - 1
                                && isAppendLine
                                && !string.Equals(methodName, "AppendLine", StringComparison.Ordinal))
                            {
                                newInvocation = SimpleMemberInvocationExpression(
                                    newInvocation,
                                    IdentifierName("AppendLine"),
                                    ArgumentList());
                            }
                        }

                        break;
                    }
                default:
                    {
                        newInvocation = CreateInvocationExpression(
                            (InvocationExpressionSyntax)expression,
                            invocation);

                        if (isAppendLine)
                            newInvocation = SimpleMemberInvocationExpression(newInvocation, IdentifierName("AppendLine"), ArgumentList());

                        break;
                    }
            }

            newInvocation = newInvocation
                .WithTriviaFrom(invocation)
                .WithFormatterAnnotation();

            return await document.ReplaceNodeAsync(invocation, newInvocation, cancellationToken).ConfigureAwait(false);
        }

        private static InvocationExpressionSyntax ConvertInterpolatedStringExpressionToInvocationExpression(
            InterpolatedStringExpressionSyntax interpolatedString,
            SimpleMemberInvocationExpressionInfo invocationInfo,
            SemanticModel semanticModel)
        {
            bool isVerbatim = interpolatedString.IsVerbatim();

            bool isAppendLine = string.Equals(invocationInfo.NameText, "AppendLine", StringComparison.Ordinal);

            InvocationExpressionSyntax invocation = invocationInfo.InvocationExpression;

            InvocationExpressionSyntax newExpression = null;

            SyntaxList<InterpolatedStringContentSyntax> contents = interpolatedString.Contents;

            for (int i = 0; i < contents.Count; i++)
            {
                (SyntaxKind contentKind, string methodName, ImmutableArray<ArgumentSyntax> arguments) = RefactoringUtility.ConvertInterpolatedStringToStringBuilderMethod(contents[i], isVerbatim);

                if (i == contents.Count - 1
                    && isAppendLine
                    && string.Equals(methodName, "Append", StringComparison.Ordinal)
                    && (contentKind == SyntaxKind.InterpolatedStringText
                        || semanticModel.IsImplicitConversion(((InterpolationSyntax)contents[i]).Expression, semanticModel.Compilation.GetSpecialType(SpecialType.System_String))))
                {
                    methodName = "AppendLine";
                }

                if (newExpression == null)
                {
                    newExpression = invocation
                        .ReplaceNode(invocationInfo.Name, IdentifierName(methodName).WithTriviaFrom(invocationInfo.Name))
                        .WithArgumentList(invocation.ArgumentList.WithArguments(arguments.ToSeparatedSyntaxList()).WithoutTrailingTrivia());
                }
                else
                {
                    newExpression = SimpleMemberInvocationExpression(
                        newExpression,
                        IdentifierName(methodName),
                        ArgumentList(arguments.ToSeparatedSyntaxList()));
                }

                if (i == contents.Count - 1
                    && isAppendLine
                    && !string.Equals(methodName, "AppendLine", StringComparison.Ordinal))
                {
                    newExpression = SimpleMemberInvocationExpression(
                        newExpression,
                        IdentifierName("AppendLine"),
                        ArgumentList());
                }
            }

            return newExpression;
        }

        private static InvocationExpressionSyntax CreateInvocationExpression(
            InvocationExpressionSyntax innerInvocationExpression,
            InvocationExpressionSyntax outerInvocationExpression)
        {
            SimpleMemberInvocationExpressionInfo invocationInfo = SyntaxInfo.SimpleMemberInvocationExpressionInfo(innerInvocationExpression);

            switch (invocationInfo.NameText)
            {
                case "Substring":
                    {
                        SeparatedSyntaxList<ArgumentSyntax> arguments = invocationInfo.Arguments;

                        ArgumentListSyntax argumentList = ArgumentList(
                            Argument(invocationInfo.Expression),
                            arguments[0],
                            arguments[1]
                        );

                        return CreateNewInvocationExpression(outerInvocationExpression, "Append", argumentList);
                    }
                case "Remove":
                    {
                        SeparatedSyntaxList<ArgumentSyntax> arguments = invocationInfo.Arguments;

                        ArgumentListSyntax argumentList = ArgumentList(
                            Argument(invocationInfo.Expression),
                            Argument(NumericLiteralExpression(0)),
                            arguments[0]
                        );

                        return CreateNewInvocationExpression(outerInvocationExpression, "Append", argumentList);
                    }
                case "Format":
                    {
                        return CreateNewInvocationExpression(outerInvocationExpression, "AppendFormat", invocationInfo.ArgumentList);
                    }
            }

            Debug.Fail(innerInvocationExpression.ToString());
            return outerInvocationExpression;
        }

        private static InvocationExpressionSyntax CreateNewInvocationExpression(InvocationExpressionSyntax invocationExpression, string methodName, ArgumentListSyntax argumentList)
        {
            var memberAccess = (MemberAccessExpressionSyntax)invocationExpression.Expression;

            return invocationExpression
                .WithExpression(memberAccess.WithName(IdentifierName(methodName).WithTriviaFrom(memberAccess.Name)))
                .WithArgumentList(argumentList);
        }
    }
}