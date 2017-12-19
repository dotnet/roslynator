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

            if (stringBuilderSymbol == null)
                return;

            InvocationExpressionSyntax invocationExpression = invocationInfo.InvocationExpression;

            if (!context.SemanticModel.TryGetMethodInfo(invocationExpression, out MethodInfo methodInfo, context.CancellationToken))
                return;

            if (methodInfo.IsExtensionMethod)
                return;

            if (methodInfo.ContainingType?.Equals(stringBuilderSymbol) != true)
                return;

            ImmutableArray<IParameterSymbol> parameters = methodInfo.Parameters;

            int parameterCount = parameters.Length;

            if (parameterCount == 0)
            {
                if (methodInfo.IsName("AppendLine"))
                {
                    MemberInvocationExpressionInfo invocationInfo2 = SyntaxInfo.MemberInvocationExpressionInfo(invocationInfo.Expression);

                    if (invocationInfo2.Success
                        && invocationInfo2.NameText == "Append"
                        && invocationInfo2.Arguments.Count == 1
                        && context.SemanticModel.TryGetMethodInfo(invocationInfo2.InvocationExpression, out MethodInfo methodInfo2, context.CancellationToken)
                        && !methodInfo2.IsStatic
                        && methodInfo2.ContainingType?.Equals(stringBuilderSymbol) == true
                        && methodInfo2.HasParameter(SpecialType.System_String))
                    {
                        context.ReportDiagnostic(DiagnosticDescriptors.OptimizeStringBuilderAppendCall, invocationInfo.Name, methodInfo.Name);
                    }
                }
            }
            else if (parameterCount == 1)
            {
                if (methodInfo.IsName("Append", "AppendLine"))
                {
                    ArgumentSyntax argument = invocationInfo.Arguments.SingleOrDefault(shouldThrow: false);

                    if (argument != null)
                    {
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
                                        && parameterCount == 1
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
                }
            }
            else if (parameterCount == 2)
            {
                if (methodInfo.IsName("Insert")
                    && parameters[0].Type.SpecialType == SpecialType.System_Int32
                    && parameters[1].Type.SpecialType == SpecialType.System_Object)
                {
                    SeparatedSyntaxList<ArgumentSyntax> arguments = invocationInfo.Arguments;

                    if (arguments.Count == 2
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

            if (!invocationInfo.Success)
                return false;

            if (!semanticModel.TryGetMethodInfo(invocationInfo.InvocationExpression, out MethodInfo methodInfo, cancellationToken))
                return false;

            if (!methodInfo.IsContainingType(SpecialType.System_String))
                return false;

            if (!methodInfo.IsReturnType(SpecialType.System_String))
                return false;

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

            return false;
        }

        public static Task<Document> RefactorAsync(
            Document document,
            MemberInvocationExpressionInfo invocationInfo,
            CancellationToken cancellationToken)
        {
            MemberInvocationExpressionInfo invocationInfo2 = SyntaxInfo.MemberInvocationExpressionInfo(invocationInfo.Expression);

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
            MemberInvocationExpressionInfo invocationInfo,
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
                        ImmutableArray<ExpressionSyntax> expressions = SyntaxInfo.BinaryExpressionChainInfo((BinaryExpressionSyntax)expression).Expressions;

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
            MemberInvocationExpressionInfo invocationInfo,
            SemanticModel semanticModel)
        {
            bool isVerbatim = interpolatedString.IsVerbatim();

            bool isAppendLine = string.Equals(invocationInfo.NameText, "AppendLine", StringComparison.Ordinal);

            InvocationExpressionSyntax invocation = invocationInfo.InvocationExpression;

            InvocationExpressionSyntax newExpression = null;

            SyntaxList<InterpolatedStringContentSyntax> contents = interpolatedString.Contents;

            for (int i = 0; i < contents.Count; i++)
            {
                InterpolatedStringContentConversion conversion = InterpolatedStringContentConversion.Create(contents[i], isVerbatim);

                string methodName = conversion.MethodName;
                SeparatedSyntaxList<ArgumentSyntax> arguments = conversion.Arguments;

                if (i == contents.Count - 1
                    && isAppendLine
                    && string.Equals(methodName, "Append", StringComparison.Ordinal)
                    && (conversion.Kind == SyntaxKind.InterpolatedStringText
                        || semanticModel.IsImplicitConversion(((InterpolationSyntax)contents[i]).Expression, semanticModel.Compilation.GetSpecialType(SpecialType.System_String))))
                {
                    methodName = "AppendLine";
                }

                if (newExpression == null)
                {
                    newExpression = invocation
                        .ReplaceNode(invocationInfo.Name, IdentifierName(methodName).WithTriviaFrom(invocationInfo.Name))
                        .WithArgumentList(invocation.ArgumentList.WithArguments(arguments).WithoutTrailingTrivia());
                }
                else
                {
                    newExpression = SimpleMemberInvocationExpression(
                        newExpression,
                        IdentifierName(methodName),
                        ArgumentList(arguments));
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