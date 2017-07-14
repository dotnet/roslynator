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
        public static void Analyze(SyntaxNodeAnalysisContext context, MemberInvocationExpression memberInvocation)
        {
            INamedTypeSymbol stringBuilderSymbol = context.GetTypeByMetadataName(MetadataNames.System_Text_StringBuilder);

            if (stringBuilderSymbol != null)
            {
                InvocationExpressionSyntax invocationExpression = memberInvocation.InvocationExpression;
                MethodInfo methodInfo;
                if (context.SemanticModel.TryGetMethodInfo(invocationExpression, out methodInfo, context.CancellationToken)
                    && !methodInfo.IsExtensionMethod
                    && methodInfo.ContainingType?.Equals(stringBuilderSymbol) == true)
                {
                    ImmutableArray<IParameterSymbol> parameters = methodInfo.Parameters;
                    SeparatedSyntaxList<ArgumentSyntax> arguments = memberInvocation.ArgumentList.Arguments;

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
                                    StringConcatenationExpression concatenation;
                                    if (StringConcatenationExpression.TryCreate((BinaryExpressionSyntax)expression, context.SemanticModel, out concatenation))
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
            MemberInvocationExpression memberInvocation;
            if (MemberInvocationExpression.TryCreate(invocationExpression, out memberInvocation))
            {
                MethodInfo methodInfo;
                if (semanticModel.TryGetMethodInfo(memberInvocation.InvocationExpression, out methodInfo, cancellationToken)
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

        public static Task<Document> RefactorAsync(
            Document document,
            ArgumentSyntax argument,
            MemberInvocationExpression memberInvocation,
            CancellationToken cancellationToken)
        {
            InvocationExpressionSyntax invocation = memberInvocation.InvocationExpression;
            InvocationExpressionSyntax newInvocation = null;

            bool isAppendLine = string.Equals(memberInvocation.NameText, "AppendLine", StringComparison.Ordinal);

            ExpressionSyntax expression = argument.Expression;

            switch (expression.Kind())
            {
                case SyntaxKind.InterpolatedStringExpression:
                    {
                        newInvocation = ConvertInterpolatedStringExpressionToInvocationExpression((InterpolatedStringExpressionSyntax)argument.Expression, memberInvocation);
                        break;
                    }
                case SyntaxKind.AddExpression:
                    {
                        ImmutableArray<ExpressionSyntax> expressions = BinaryExpressionChain.Create((BinaryExpressionSyntax)expression).Expressions;

                        newInvocation = invocation
                            .ReplaceNode(memberInvocation.Name, IdentifierName("Append").WithTriviaFrom(memberInvocation.Name))
                            .WithArgumentList(invocation.ArgumentList.WithArguments(SingletonSeparatedList(Argument(expressions[0]))).WithoutTrailingTrivia());

                        for (int i = 1; i < expressions.Length; i++)
                        {
                            string name = (i == expressions.Length - 1 && isAppendLine)
                                ? "AppendLine"
                                : "Append";

                            newInvocation = SimpleMemberInvocationExpression(
                                newInvocation,
                                IdentifierName(name),
                                ArgumentList(Argument(expressions[i])));
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

            return document.ReplaceNodeAsync(invocation, newInvocation, cancellationToken);
        }

        private static InvocationExpressionSyntax ConvertInterpolatedStringExpressionToInvocationExpression(
            InterpolatedStringExpressionSyntax interpolatedString,
            MemberInvocationExpression memberInvocation)
        {
            bool isVerbatim = interpolatedString.IsVerbatim();

            bool isAppendLine = string.Equals(memberInvocation.NameText, "AppendLine", StringComparison.Ordinal);

            InvocationExpressionSyntax invocation = memberInvocation.InvocationExpression;

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
                        .ReplaceNode(memberInvocation.Name, IdentifierName(name).WithTriviaFrom(memberInvocation.Name))
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
            MemberInvocationExpression memberInvocation = MemberInvocationExpression.Create(innerInvocationExpression);

            switch (memberInvocation.NameText)
            {
                case "Substring":
                    {
                        SeparatedSyntaxList<ArgumentSyntax> arguments = memberInvocation.ArgumentList.Arguments;

                        ArgumentListSyntax argumentList = ArgumentList(
                            Argument(memberInvocation.Expression),
                            arguments[0],
                            arguments[1]
                        );

                        return CreateNewInvocationExpression(outerInvocationExpression, "Append", argumentList);
                    }
                case "Remove":
                    {
                        SeparatedSyntaxList<ArgumentSyntax> arguments = memberInvocation.ArgumentList.Arguments;

                        ArgumentListSyntax argumentList = ArgumentList(
                            Argument(memberInvocation.Expression),
                            Argument(NumericLiteralExpression(0)),
                            arguments[0]
                        );

                        return CreateNewInvocationExpression(outerInvocationExpression, "Append", argumentList);
                    }
                case "Format":
                    {
                        return CreateNewInvocationExpression(outerInvocationExpression, "AppendFormat", memberInvocation.ArgumentList);
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