// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Roslynator.CSharp;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static Roslynator.CSharp.CSharpFactory;

namespace Roslynator.CSharp.Refactorings
{
    internal static class UseStringComparisonRefactoring
    {
        public static void AnalyzeEqualsExpression(SyntaxNodeAnalysisContext context)
        {
            var equalsExpression = (BinaryExpressionSyntax)context.Node;

            if (Analyze(equalsExpression, context.SemanticModel, context.CancellationToken))
                ReportDiagnostic(context, equalsExpression);
        }

        public static void AnalyzeNotEqualsExpression(SyntaxNodeAnalysisContext context)
        {
            var notEqualsExpression = (BinaryExpressionSyntax)context.Node;

            if (Analyze(notEqualsExpression, context.SemanticModel, context.CancellationToken))
                ReportDiagnostic(context, notEqualsExpression);
        }

        private static void ReportDiagnostic(SyntaxNodeAnalysisContext context, SyntaxNode node)
        {
            if (!node.SpanContainsDirectives())
                context.ReportDiagnostic(DiagnosticDescriptors.UseStringComparison, node);
        }

        private static bool Analyze(
            BinaryExpressionSyntax binaryExpression,
            SemanticModel semanticModel,
            CancellationToken cancellationToken)
        {
            ExpressionSyntax left = binaryExpression.Left;

            switch (left?.Kind())
            {
                case SyntaxKind.StringLiteralExpression:
                    {
                        ExpressionSyntax right = binaryExpression.Right;

                        string name = GetMethodName(right);

                        return IsToLowerOrToUpper(name)
                            && IsPublicInstanceStringMethodThatReturnsString(right, name, semanticModel, cancellationToken);
                    }
                case SyntaxKind.InvocationExpression:
                    {
                        string name = GetMethodName((InvocationExpressionSyntax)left);

                        if (IsToLowerOrToUpper(name))
                        {
                            ExpressionSyntax right = binaryExpression.Right;

                            switch (right?.Kind())
                            {
                                case SyntaxKind.StringLiteralExpression:
                                    {
                                        return IsPublicInstanceStringMethodThatReturnsString(left, name, semanticModel, cancellationToken);
                                    }
                                case SyntaxKind.InvocationExpression:
                                    {
                                        string name2 = GetMethodName((InvocationExpressionSyntax)right);

                                        if (name == name2)
                                        {
                                            return IsPublicInstanceStringMethodThatReturnsString(left, name, semanticModel, cancellationToken)
                                                && IsPublicInstanceStringMethodThatReturnsString(right, name2, semanticModel, cancellationToken);
                                        }

                                        break;
                                    }
                            }
                        }

                        break;
                    }
            }

            return false;
        }

        public static void AnalyzeInvocationExpression(SyntaxNodeAnalysisContext context)
        {
            var invocation = (InvocationExpressionSyntax)context.Node;

            ArgumentListSyntax argumentList = invocation.ArgumentList;

            if (argumentList != null)
            {
                SeparatedSyntaxList<ArgumentSyntax> arguments = argumentList.Arguments;

                if (arguments.Count == 2)
                {
                    ExpressionSyntax expression = invocation.Expression;

                    if (expression?.IsKind(SyntaxKind.SimpleMemberAccessExpression) == true)
                    {
                        var memberAccess = (MemberAccessExpressionSyntax)expression;

                        if (memberAccess.Name?.Identifier.ValueText == "Equals")
                        {
                            ExpressionSyntax firstArgumentExpression = arguments[0].Expression;

                            switch (firstArgumentExpression?.Kind())
                            {
                                case SyntaxKind.StringLiteralExpression:
                                    {
                                        string name = GetMethodName(arguments[1].Expression);

                                        if (IsToLowerOrToUpper(name)
                                            && IsPublicStaticStringEqualsWithTwoStringParameters(invocation, context.SemanticModel, context.CancellationToken))
                                        {
                                            ReportDiagnostic(context, invocation);
                                        }

                                        break;
                                    }
                                case SyntaxKind.InvocationExpression:
                                    {
                                        string name = GetMethodName((InvocationExpressionSyntax)firstArgumentExpression);

                                        if (IsToLowerOrToUpper(name))
                                        {
                                            ExpressionSyntax secondArgumentExpression = arguments[1].Expression;

                                            switch (secondArgumentExpression.Kind())
                                            {
                                                case SyntaxKind.StringLiteralExpression:
                                                    {
                                                        if (IsPublicStaticStringEqualsWithTwoStringParameters(invocation, context.SemanticModel, context.CancellationToken))
                                                            ReportDiagnostic(context, invocation);

                                                        break;
                                                    }
                                                case SyntaxKind.InvocationExpression:
                                                    {
                                                        string name2 = GetMethodName((InvocationExpressionSyntax)secondArgumentExpression);

                                                        if (name == name2
                                                            && IsPublicStaticStringEqualsWithTwoStringParameters(invocation, context.SemanticModel, context.CancellationToken))
                                                        {
                                                            ReportDiagnostic(context, invocation);
                                                        }

                                                        break;
                                                    }
                                            }
                                        }

                                        break;
                                    }
                            }
                        }
                    }
                }
                else if (arguments.Count == 1)
                {
                    ExpressionSyntax invocationExpression = invocation.Expression;

                    if (invocationExpression?.IsKind(SyntaxKind.SimpleMemberAccessExpression) == true)
                    {
                        var memberAccess = (MemberAccessExpressionSyntax)invocationExpression;

                        if (memberAccess.Name?.Identifier.ValueText == "Equals")
                        {
                            string name = GetMethodName(memberAccess.Expression);

                            if (IsToLowerOrToUpper(name))
                            {
                                ExpressionSyntax expression = arguments[0].Expression;

                                switch (expression?.Kind())
                                {
                                    case SyntaxKind.StringLiteralExpression:
                                        {
                                            if (IsInstanceStringEqualsWithOneStringParameter(invocation, context.SemanticModel, context.CancellationToken))
                                                ReportDiagnostic(context, invocation);

                                            break;
                                        }
                                    case SyntaxKind.InvocationExpression:
                                        {
                                            string name2 = GetMethodName((InvocationExpressionSyntax)expression);

                                            if (name == name2
                                                && IsInstanceStringEqualsWithOneStringParameter(invocation, context.SemanticModel, context.CancellationToken))
                                            {
                                                ReportDiagnostic(context, invocation);
                                            }

                                            break;
                                        }
                                }
                            }
                        }
                    }
                }
            }
        }

        private static bool IsToLowerOrToUpper(string name)
        {
            if (name != null)
            {
                return name == "ToUpper"
                    || name == "ToLower";
            }

            return false;
        }

        private static string GetMethodName(ExpressionSyntax expression)
        {
            if (expression?.IsKind(SyntaxKind.InvocationExpression) == true)
            {
                return GetMethodName((InvocationExpressionSyntax)expression);
            }
            else
            {
                return null;
            }
        }

        private static string GetMethodName(InvocationExpressionSyntax invocation)
        {
            if (invocation.ArgumentList?.Arguments.Count == 0)
            {
                ExpressionSyntax invocationExpression = invocation.Expression;

                if (invocationExpression?.IsKind(SyntaxKind.SimpleMemberAccessExpression) == true)
                {
                    var memberAccess = (MemberAccessExpressionSyntax)invocationExpression;

                    return memberAccess.Name?.Identifier.ValueText;
                }
            }

            return null;
        }

        private static bool IsPublicInstanceStringMethodThatReturnsString(
            ExpressionSyntax expression,
            string name,
            SemanticModel semanticModel,
            CancellationToken cancellationToken)
        {
            MethodInfo info = semanticModel.GetMethodInfo(expression, cancellationToken);

            return info.IsValid
                && info.IsPublicInstanceStringMethod(name)
                && info.ReturnsString
                && !info.Parameters.Any();
        }

        private static bool IsPublicStaticStringEqualsWithTwoStringParameters(InvocationExpressionSyntax invocation, SemanticModel semanticModel, CancellationToken cancellationToken)
        {
            MethodInfo info = semanticModel.GetMethodInfo(invocation, cancellationToken);

            return info.IsValid
                && info.IsPublicStaticStringMethod("Equals")
                && info.ReturnsBoolean
                && info.HasParameters(SpecialType.System_String, SpecialType.System_String);
        }

        private static bool IsInstanceStringEqualsWithOneStringParameter(InvocationExpressionSyntax invocation, SemanticModel semanticModel, CancellationToken cancellationToken)
        {
            MethodInfo info = semanticModel.GetMethodInfo(invocation, cancellationToken);

            return info.IsValid
                && info.IsPublicInstanceStringMethod("Equals")
                && info.ReturnsBoolean
                && info.HasParameter(SpecialType.System_String);
        }

        public static Task<Document> RefactorAsync(
            Document document,
            BinaryExpressionSyntax binaryExpression,
            StringComparison stringComparison,
            CancellationToken cancellationToken)
        {
            ExpressionSyntax left = binaryExpression.Left;
            ExpressionSyntax right = binaryExpression.Right;

            string name = GetMethodName(left);

            ExpressionSyntax newNode = SimpleMemberInvocationExpression(
                StringType(),
                IdentifierName("Equals"),
                ArgumentList(
                    CreateArgument(left),
                    CreateArgument(right),
                    Argument(CreateStringComparison(stringComparison))));

            if (binaryExpression.IsKind(SyntaxKind.NotEqualsExpression))
                newNode = LogicalNotExpression(newNode);

            newNode = newNode
                .WithTriviaFrom(binaryExpression)
                .WithFormatterAnnotation();

            return document.ReplaceNodeAsync(binaryExpression, newNode, cancellationToken);
        }

        internal static async Task<Document> RefactorAsync(
            Document document,
            InvocationExpressionSyntax invocation,
            StringComparison stringComparison,
            CancellationToken cancellationToken)
        {
            ArgumentListSyntax argumentList = invocation.ArgumentList;

            SeparatedSyntaxList<ArgumentSyntax> arguments = argumentList.Arguments;

            if (arguments.Count == 2)
            {
                ArgumentListSyntax newArgumentList = ArgumentList(
                    CreateArgument(arguments[0]),
                    CreateArgument(arguments[1]),
                    Argument(CreateStringComparison(stringComparison)));

                InvocationExpressionSyntax newNode = invocation.WithArgumentList(newArgumentList);

                return await document.ReplaceNodeAsync(invocation, newNode, cancellationToken).ConfigureAwait(false);
            }
            else if (arguments.Count == 1)
            {
                ArgumentListSyntax newArgumentList = ArgumentList(
                    CreateArgument(arguments[0]),
                    Argument(CreateStringComparison(stringComparison)));

                var memberAccess = (MemberAccessExpressionSyntax)invocation.Expression;
                var invocation2 = (InvocationExpressionSyntax)memberAccess.Expression;
                var memberAccess2 = (MemberAccessExpressionSyntax)invocation2.Expression;

                InvocationExpressionSyntax newNode = invocation
                    .ReplaceNode(invocation2, memberAccess2.Expression)
                    .WithArgumentList(newArgumentList);

                return await document.ReplaceNodeAsync(invocation, newNode, cancellationToken).ConfigureAwait(false);
            }

            Debug.Assert(false, "");

            return document;
        }

        private static NameSyntax CreateStringComparison(StringComparison stringComparison)
        {
            return ParseName($"System.StringComparison.{stringComparison}").WithSimplifierAnnotation();
        }

        private static ArgumentSyntax CreateArgument(ExpressionSyntax expression)
        {
            switch (expression.Kind())
            {
                case SyntaxKind.StringLiteralExpression:
                    {
                        return Argument(expression);
                    }
                case SyntaxKind.InvocationExpression:
                    {
                        var invocation = (InvocationExpressionSyntax)expression;

                        var memberAccess = (MemberAccessExpressionSyntax)invocation.Expression;

                        return Argument(memberAccess.Expression).WithTriviaFrom(expression);
                    }
                default:
                    {
                        Debug.Assert(false, expression.Kind().ToString());
                        return Argument(expression);
                    }
            }
        }

        private static ArgumentSyntax CreateArgument(ArgumentSyntax argument)
        {
            ExpressionSyntax expression = argument.Expression;

            switch (expression.Kind())
            {
                case SyntaxKind.StringLiteralExpression:
                    {
                        return argument;
                    }
                case SyntaxKind.InvocationExpression:
                    {
                        var invocation = (InvocationExpressionSyntax)expression;

                        var memberAccess = (MemberAccessExpressionSyntax)invocation.Expression;

                        return Argument(memberAccess.Expression).WithTriviaFrom(argument);
                    }
                default:
                    {
                        Debug.Assert(false, expression.Kind().ToString());
                        return argument;
                    }
            }
        }
    }
}
