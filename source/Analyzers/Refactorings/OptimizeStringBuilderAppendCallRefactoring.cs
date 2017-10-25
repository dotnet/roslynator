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
using Roslynator.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static Roslynator.CSharp.CSharpFactory;

namespace Roslynator.CSharp.Refactorings
{
    internal static class OptimizeStringBuilderAppendCallRefactoring
    {
        public static void Analyze(SyntaxNodeAnalysisContext context, MemberInvocationExpressionInfo invocationInfo)
        {
            INamedTypeSymbol stringBuilderSymbol = context.GetTypeByMetadataName(MetadataNames.System_Text_StringBuilder);

            if (stringBuilderSymbol != null)
            {
                InvocationExpressionSyntax invocationExpression = invocationInfo.InvocationExpression;
                MethodInfo methodInfo;
                if (context.SemanticModel.TryGetMethodInfo(invocationExpression, out methodInfo, context.CancellationToken)
                    && !methodInfo.IsExtensionMethod
                    && methodInfo.ContainingType?.Equals(stringBuilderSymbol) == true)
                {
                    ImmutableArray<IParameterSymbol> parameters = methodInfo.Parameters;
                    SeparatedSyntaxList<ArgumentSyntax> arguments = invocationInfo.Arguments;

                    if (parameters.Length == 1
                        && arguments.Count == 1
                        && methodInfo.IsName("Append", "AppendLine"))
                    {
                        ArgumentSyntax argument = arguments.First();

                        ExpressionSyntax expression = argument.Expression;

                        SyntaxKind expressionKind = expression.Kind();

                        switch (expressionKind)
                        {
                            case SyntaxKind.InterpolatedStringExpression:
                                {
                                    context.ReportDiagnostic(DiagnosticDescriptors.OptimizeStringBuilderAppendCall, argument, methodInfo.Name);
                                    return;
                                }
                            case SyntaxKind.AddExpression:
                                {
                                    StringConcatenationExpressionInfo concatenationInfo = SyntaxInfo.StringConcatenationExpressionInfo((BinaryExpressionSyntax)expression, context.SemanticModel, context.CancellationToken);
                                    if (concatenationInfo.Success)
                                    {
                                        context.ReportDiagnostic(DiagnosticDescriptors.OptimizeStringBuilderAppendCall, argument, methodInfo.Name);
                                        return;
                                    }

                                    break;
                                }
                            default:
                                {
                                    if (expressionKind == SyntaxKind.InvocationExpression
                                        && IsFixable((InvocationExpressionSyntax)expression, context.SemanticModel, context.CancellationToken))
                                    {
                                        context.ReportDiagnostic(DiagnosticDescriptors.OptimizeStringBuilderAppendCall, argument, methodInfo.Name);
                                        return;
                                    }

                                    if (methodInfo.IsName("Append")
                                        && parameters.Length == 1
                                        && parameters[0].Type.IsObject()
                                        && context.SemanticModel.GetTypeSymbol(argument.Expression, context.CancellationToken).IsValueType)
                                    {
                                        context.ReportDiagnostic(DiagnosticDescriptors.AvoidBoxingOfValueType, argument);
                                        return;
                                    }

                                    break;
                                }
                        }
                    }
                    else if (parameters.Length > 1
                        && methodInfo.IsName("Insert")
                        && methodInfo.HasParameters(SpecialType.System_Int32, SpecialType.System_Object)
                        && context.SemanticModel
                            .GetTypeSymbol(arguments[1].Expression, context.CancellationToken)
                            .IsValueType)
                    {
                        context.ReportDiagnostic(DiagnosticDescriptors.AvoidBoxingOfValueType, arguments[1]);
                    }
                }
            }
        }

        private static bool IsFixable(InvocationExpressionSyntax invocationExpression, SemanticModel semanticModel, CancellationToken cancellationToken)
        {
            MemberInvocationExpressionInfo invocationInfo = SyntaxInfo.MemberInvocationExpressionInfo(invocationExpression);
            if (invocationInfo.Success)
            {
                MethodInfo methodInfo;
                if (semanticModel.TryGetMethodInfo(invocationInfo.InvocationExpression, out methodInfo, cancellationToken)
                    && methodInfo.IsContainingType(SpecialType.System_String)
                    && methodInfo.IsReturnType(SpecialType.System_String))
                {
                    switch (methodInfo.Name)
                    {
                        case "Substring":
                            {
                                if (methodInfo.HasParameters(SpecialType.System_Int32, SpecialType.System_Int32))
                                    return true;

                                break;
                            }
                        case "Remove":
                            {
                                if (methodInfo.HasParameter(SpecialType.System_Int32))
                                    return true;

                                break;
                            }
                        case "Format":
                            {
                                return true;
                            }
                    }
                }
            }

            return false;
        }

        public static async Task<Document> RefactorAsync(
            Document document,
            ArgumentSyntax argument,
            MemberInvocationExpressionInfo invocationInfo,
            CancellationToken cancellationToken)
        {
            InvocationExpressionSyntax invocation = invocationInfo.InvocationExpression;
            InvocationExpressionSyntax newInvocation = null;

            bool isAppendLine = string.Equals(invocationInfo.NameText, "AppendLine", StringComparison.Ordinal);

            ExpressionSyntax expression = argument.Expression;

            switch (expression.Kind())
            {
                case SyntaxKind.InterpolatedStringExpression:
                    {
                        newInvocation = ConvertInterpolatedStringExpressionToInvocationExpression((InterpolatedStringExpressionSyntax)argument.Expression, invocationInfo);
                        break;
                    }
                case SyntaxKind.AddExpression:
                    {
                        ImmutableArray<ExpressionSyntax> expressions = SyntaxInfo.BinaryExpressionChainInfo((BinaryExpressionSyntax)expression).Expressions;

                        newInvocation = invocation
                            .ReplaceNode(invocationInfo.Name, IdentifierName("Append").WithTriviaFrom(invocationInfo.Name))
                            .WithArgumentList(invocation.ArgumentList.WithArguments(SingletonSeparatedList(Argument(expressions[0]))).WithoutTrailingTrivia());

                        SemanticModel semanticModel = null;

                        for (int i = 1; i < expressions.Length; i++)
                        {
                            string name = (i == expressions.Length - 1 && isAppendLine)
                                ? "AppendLine"
                                : "Append";

                            ExpressionSyntax argumentExpression = expressions[i];

                            if (isAppendLine)
                            {
                                if (semanticModel == null)
                                    semanticModel = await document.GetSemanticModelAsync(cancellationToken).ConfigureAwait(false);

                                if (semanticModel
                                    .GetTypeInfo(argumentExpression, cancellationToken)
                                    .ConvertedType?
                                    .IsString() != true)
                                {
                                    argumentExpression = SimpleMemberInvocationExpression(argumentExpression.Parenthesize(), IdentifierName("ToString"));
                                }
                            }

                            newInvocation = SimpleMemberInvocationExpression(
                                newInvocation,
                                IdentifierName(name),
                                ArgumentList(Argument(argumentExpression)));
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
            MemberInvocationExpressionInfo invocationInfo)
        {
            bool isVerbatim = interpolatedString.IsVerbatim();

            bool isAppendLine = string.Equals(invocationInfo.NameText, "AppendLine", StringComparison.Ordinal);

            InvocationExpressionSyntax invocation = invocationInfo.InvocationExpression;

            InvocationExpressionSyntax newExpression = null;

            SyntaxList<InterpolatedStringContentSyntax> contents = interpolatedString.Contents;

            for (int i = 0; i < contents.Count; i++)
            {
                InterpolatedStringContentConversion conversion = InterpolatedStringContentConversion.Create(contents[i], isVerbatim);

                string name = conversion.Name;
                SeparatedSyntaxList<ArgumentSyntax> arguments = conversion.Arguments;

                if (i == contents.Count - 1
                    && isAppendLine
                    && !string.Equals(name, "AppendFormat", StringComparison.Ordinal))
                {
                    name = "AppendLine";
                }

                if (newExpression == null)
                {
                    newExpression = invocation
                        .ReplaceNode(invocationInfo.Name, IdentifierName(name).WithTriviaFrom(invocationInfo.Name))
                        .WithArgumentList(invocation.ArgumentList.WithArguments(arguments).WithoutTrailingTrivia());
                }
                else
                {
                    newExpression = SimpleMemberInvocationExpression(
                        newExpression,
                        IdentifierName(name),
                        ArgumentList(arguments));
                }

                if (i == contents.Count - 1
                    && isAppendLine
                    && string.Equals(name, "AppendFormat", StringComparison.Ordinal))
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
            MemberInvocationExpressionInfo invocationInfo = SyntaxInfo.MemberInvocationExpressionInfo(innerInvocationExpression);

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