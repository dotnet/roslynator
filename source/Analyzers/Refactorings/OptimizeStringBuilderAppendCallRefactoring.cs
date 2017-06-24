// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
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
                                    if (invocationExpression.IsParentKind(SyntaxKind.ExpressionStatement))
                                    {
                                        context.ReportDiagnostic(DiagnosticDescriptors.OptimizeStringBuilderAppendCall, argument);
                                        return;
                                    }

                                    break;
                                }
                            case SyntaxKind.AddExpression:
                                {
                                    if (invocationExpression.IsParentKind(SyntaxKind.ExpressionStatement))
                                    {
                                        StringConcatenationExpression concatenation;
                                        if (StringConcatenationExpression.TryCreate((BinaryExpressionSyntax)expression, context.SemanticModel, out concatenation))
                                        {
                                            context.ReportDiagnostic(DiagnosticDescriptors.OptimizeStringBuilderAppendCall, argument);
                                            return;
                                        }
                                    }

                                    break;
                                }
                            default:
                                {
                                    if (expressionKind == SyntaxKind.InvocationExpression
                                        && IsFixable((InvocationExpressionSyntax)expression, context.SemanticModel, context.CancellationToken))
                                    {
                                        if (methodInfo.IsName("Append")
                                            || invocationExpression.IsParentKind(SyntaxKind.ExpressionStatement))
                                        {
                                            context.ReportDiagnostic(DiagnosticDescriptors.OptimizeStringBuilderAppendCall, argument);
                                            return;
                                        }
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
                        && methodInfo.IsName("AppendFormat", "Insert"))
                    {
                        int index = -1;

                        for (int i = 0; i < parameters.Length; i++)
                        {
                            ITypeSymbol type = parameters[i].Type;

                            if (type.IsObject())
                            {
                                index = i;
                                break;
                            }
                            else if (parameters[i].IsParams
                                && type.IsArrayType()
                                && ((IArrayTypeSymbol)type).ElementType.IsObject())
                            {
                                index = i;
                                break;
                            }
                        }

                        if (index != -1)
                        {
                            if (parameters[index].IsParams)
                            {
                                for (int i = index; i < arguments.Count; i++)
                                {
                                    if (context.SemanticModel
                                        .GetTypeSymbol(arguments[i].Expression, context.CancellationToken)
                                        .IsValueType)
                                    {
                                        context.ReportDiagnostic(DiagnosticDescriptors.AvoidBoxingOfValueType, arguments[i]);
                                    }
                                }
                            }
                            else
                            {
                                int min = Math.Min(parameters.Length, arguments.Count);

                                for (int i = index; i < min; i++)
                                {
                                    if (parameters[i].Type.IsObject()
                                        && context.SemanticModel
                                            .GetTypeSymbol(arguments[i].Expression, context.CancellationToken)
                                            .IsValueType)
                                    {
                                        context.ReportDiagnostic(DiagnosticDescriptors.AvoidBoxingOfValueType, arguments[i]);
                                    }
                                }
                            }
                        }
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

        public static async Task<Document> RefactorAsync(
            Document document,
            ArgumentSyntax argument,
            MemberInvocationExpression memberInvocation,
            CancellationToken cancellationToken)
        {
            ExpressionSyntax expression = argument.Expression;

            var expressionStatement = (ExpressionStatementSyntax)memberInvocation.InvocationExpression.Parent;

            ExpressionSyntax stringBuilderExpression = memberInvocation.Expression.WithoutTrivia();

            List<ExpressionStatementSyntax> statements = null;

            switch (expression.Kind())
            {
                case SyntaxKind.InterpolatedStringExpression:
                    {
                        statements = CSharpUtility.ConvertInterpolatedStringToStringBuilderAppend((InterpolatedStringExpressionSyntax)expression, stringBuilderExpression)
                            .Select(f => ExpressionStatement(f))
                            .ToList();

                        break;
                    }
                case SyntaxKind.AddExpression:
                    {
                        SemanticModel semanticModel = await document.GetSemanticModelAsync(cancellationToken).ConfigureAwait(false);

                        statements = StringConcatenationExpression.Create((BinaryExpressionSyntax)expression, semanticModel)
                            .Expressions
                            .Select(f =>
                            {
                                return ExpressionStatement(
                                    SimpleMemberInvocationExpression(
                                        stringBuilderExpression,
                                        IdentifierName("Append"),
                                        Argument(f)));
                            })
                            .ToList();

                        break;
                    }
                default:
                    {
                        InvocationExpressionSyntax invocationExpression = memberInvocation.InvocationExpression;

                        InvocationExpressionSyntax newNode = CreateNewInvocationExpression(
                            (InvocationExpressionSyntax)expression,
                            invocationExpression);

                        statements = new List<ExpressionStatementSyntax>() { expressionStatement.ReplaceNode(invocationExpression, newNode) };
                        break;
                    }
            }

            if (memberInvocation.NameText == "AppendLine")
                statements.Add(ExpressionStatement(SimpleMemberInvocationExpression(stringBuilderExpression, IdentifierName("AppendLine"), ArgumentList())));

            for (int i = 0; i < statements.Count; i++)
                statements[i] = statements[i].WithFormatterAnnotation();

            if (statements.Count == 1)
            {
                statements[0] = statements[0].WithTriviaFrom(expressionStatement);
            }
            else
            {
                statements[0] = statements[0]
                    .WithLeadingTrivia(expressionStatement.GetLeadingTrivia())
                    .WithTrailingTrivia(NewLine());

                statements[statements.Count - 1] = statements[statements.Count - 1].WithTrailingTrivia(expressionStatement.GetTrailingTrivia());
            }

            return await document.ReplaceNodeAsync(expressionStatement, statements, cancellationToken).ConfigureAwait(false);
        }

        private static InvocationExpressionSyntax CreateNewInvocationExpression(
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
                default:
                    {
                        Debug.Fail(innerInvocationExpression.ToString());
                        return outerInvocationExpression;
                    }
            }
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