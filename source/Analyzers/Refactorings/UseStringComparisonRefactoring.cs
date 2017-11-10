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
using Roslynator.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static Roslynator.CSharp.CSharpFactory;

namespace Roslynator.CSharp.Refactorings
{
    internal static class UseStringComparisonRefactoring
    {
        public static void Analyze(SyntaxNodeAnalysisContext context, MemberInvocationExpressionInfo invocationInfo)
        {
            ExpressionSyntax expression = invocationInfo.InvocationExpression.WalkUpParentheses();

            SyntaxNode parent = expression.Parent;

            SyntaxKind kind = parent.Kind();

            if (kind == SyntaxKind.SimpleMemberAccessExpression)
            {
                MemberInvocationExpressionInfo invocationInfo2 = SyntaxInfo.MemberInvocationExpressionInfo(parent.Parent);

                if (!invocationInfo2.Success)
                    return;

                Analyze(context, invocationInfo, invocationInfo2);
            }
            else if (kind == SyntaxKind.Argument)
            {
                Analyze(context, invocationInfo, (ArgumentSyntax)parent);
            }
            else if (kind == SyntaxKind.EqualsExpression
                || kind == SyntaxKind.NotEqualsExpression)
            {
                Analyze(context, invocationInfo, expression, (BinaryExpressionSyntax)parent);
            }
        }

        private static void Analyze(
            SyntaxNodeAnalysisContext context,
            MemberInvocationExpressionInfo invocationInfo,
            MemberInvocationExpressionInfo invocationInfo2)
        {
            if (invocationInfo2.InvocationExpression.SpanContainsDirectives())
                return;

            string name2 = invocationInfo2.NameText;

            if (name2 != "Equals"
                && name2 != "StartsWith"
                && name2 != "EndsWith"
                && name2 != "IndexOf"
                && name2 != "LastIndexOf"
                && name2 != "Contains")
            {
                return;
            }

            SeparatedSyntaxList<ArgumentSyntax> arguments = invocationInfo2.Arguments;

            if (arguments.Count != 1)
                return;

            ExpressionSyntax argumentExpression = arguments[0].Expression.WalkDownParentheses();

            string name = invocationInfo.NameText;

            MemberInvocationExpressionInfo invocationInfo3;

            bool isStringLiteral = argumentExpression.IsKind(SyntaxKind.StringLiteralExpression);

            if (!isStringLiteral)
            {
                invocationInfo3 = SyntaxInfo.MemberInvocationExpressionInfo(argumentExpression);

                if (!invocationInfo3.Success)
                    return;

                string name3 = invocationInfo3.NameText;

                if (name != name3)
                    return;
            }

            SemanticModel semanticModel = context.SemanticModel;
            CancellationToken cancellationToken = context.CancellationToken;

            if (!CheckSymbol(invocationInfo, semanticModel, cancellationToken))
                return;

            if (!(semanticModel.TryGetMethodInfo(invocationInfo2.InvocationExpression, out MethodInfo info, cancellationToken)
                && info.IsPublicInstanceStringMethod(name2)
                && info.ReturnType.SpecialType == ((name2.EndsWith("IndexOf", StringComparison.Ordinal)) ? SpecialType.System_Int32 : SpecialType.System_Boolean)
                && info.HasParameter(SpecialType.System_String)))
            {
                return;
            }

            if (!isStringLiteral
                && !CheckSymbol(invocationInfo3, semanticModel, cancellationToken))
            {
                return;
            }

            ReportDiagnostic(context, invocationInfo2);
        }

        private static void Analyze(
            SyntaxNodeAnalysisContext context,
            MemberInvocationExpressionInfo invocationInfo,
            ArgumentSyntax argument)
        {
            if (!(argument.Parent is ArgumentListSyntax argumentList))
                return;

            SeparatedSyntaxList<ArgumentSyntax> arguments = argumentList.Arguments;

            if (arguments.Count != 2)
                return;

            MemberInvocationExpressionInfo equalsInvocation = SyntaxInfo.MemberInvocationExpressionInfo(argumentList.Parent);

            if (!equalsInvocation.Success)
                return;

            if (equalsInvocation.NameText != "Equals")
                return;

            if (!IsFixable(context, invocationInfo, argument, arguments))
                return;

            if (!context.SemanticModel.TryGetMethodInfo(equalsInvocation.InvocationExpression, out MethodInfo info, context.CancellationToken)
                || !info.IsPublicStaticStringMethod("Equals")
                || !info.ReturnsBoolean
                || !info.HasParameters(SpecialType.System_String, SpecialType.System_String))
            {
                return;
            }

            ReportDiagnostic(context, equalsInvocation);
        }

        private static bool IsFixable(
            SyntaxNodeAnalysisContext context,
            MemberInvocationExpressionInfo invocationInfo,
            ArgumentSyntax argument,
            SeparatedSyntaxList<ArgumentSyntax> arguments)
        {
            if (object.ReferenceEquals(argument, arguments[0]))
            {
                ExpressionSyntax expression = arguments[1].Expression?.WalkDownParentheses();

                if (expression != null)
                {
                    SyntaxKind kind = expression.Kind();

                    if (kind == SyntaxKind.InvocationExpression)
                    {
                        return TryCreateCaseChangingInvocation(expression, out MemberInvocationExpressionInfo invocationInfo2)
                            && invocationInfo.NameText == invocationInfo2.NameText
                            && CheckSymbol(invocationInfo, context.SemanticModel, context.CancellationToken)
                            && CheckSymbol(invocationInfo2, context.SemanticModel, context.CancellationToken);
                    }
                    else if (kind == SyntaxKind.StringLiteralExpression)
                    {
                        return CheckSymbol(invocationInfo, context.SemanticModel, context.CancellationToken);
                    }
                }
            }
            else
            {
                return arguments[0].Expression?.WalkDownParentheses().Kind() == SyntaxKind.StringLiteralExpression
                    && CheckSymbol(invocationInfo, context.SemanticModel, context.CancellationToken);
            }

            return false;
        }

        private static void Analyze(
            SyntaxNodeAnalysisContext context,
            MemberInvocationExpressionInfo invocationInfo,
            ExpressionSyntax leftOrRight,
            BinaryExpressionSyntax binaryExpression)
        {
            if (object.ReferenceEquals(leftOrRight, binaryExpression.Left))
            {
                ExpressionSyntax right = binaryExpression.Right?.WalkDownParentheses();

                if (right != null)
                {
                    SyntaxKind kind = right.Kind();

                    if (kind == SyntaxKind.InvocationExpression)
                    {
                        if (TryCreateCaseChangingInvocation(right, out MemberInvocationExpressionInfo invocationInfo2)
                            && invocationInfo.NameText == invocationInfo2.NameText
                            && CheckSymbol(invocationInfo, context.SemanticModel, context.CancellationToken)
                            && CheckSymbol(invocationInfo2, context.SemanticModel, context.CancellationToken))
                        {
                            ReportDiagnostic(context, binaryExpression);
                        }
                    }
                    else if (kind == SyntaxKind.StringLiteralExpression)
                    {
                        if (CheckSymbol(invocationInfo, context.SemanticModel, context.CancellationToken))
                        {
                            ReportDiagnostic(context, binaryExpression);
                        }
                    }
                }
            }
            else if (binaryExpression.Left?.WalkDownParentheses().Kind() == SyntaxKind.StringLiteralExpression
                && CheckSymbol(invocationInfo, context.SemanticModel, context.CancellationToken))
            {
                ReportDiagnostic(context, binaryExpression);
            }
        }

        private static bool TryCreateCaseChangingInvocation(ExpressionSyntax expression, out MemberInvocationExpressionInfo invocationInfo)
        {
            invocationInfo = SyntaxInfo.MemberInvocationExpressionInfo(expression);

            if (invocationInfo.Success
                && !invocationInfo.Arguments.Any())
            {
                string name = invocationInfo.NameText;

                return name == "ToLower"
                    || name == "ToLowerInvariant"
                    || name == "ToUpper"
                    || name == "ToUpperInvariant";
            }

            invocationInfo = default(MemberInvocationExpressionInfo);
            return false;
        }

        private static bool CheckSymbol(
            MemberInvocationExpressionInfo invocationInfo,
            SemanticModel semanticModel,
            CancellationToken cancellationToken)
        {
            return semanticModel.TryGetMethodInfo(invocationInfo.InvocationExpression, out MethodInfo info, cancellationToken)
                && info.IsPublicInstanceStringMethod()
                && info.ReturnsString
                && !info.Parameters.Any();
        }

        private static void ReportDiagnostic(SyntaxNodeAnalysisContext context, MemberInvocationExpressionInfo invocationInfo)
        {
            ReportDiagnostic(context, invocationInfo.InvocationExpression);
        }

        private static void ReportDiagnostic(SyntaxNodeAnalysisContext context, SyntaxNode node)
        {
            context.ReportDiagnostic(DiagnosticDescriptors.UseStringComparison, node);
        }

        public static Task<Document> RefactorAsync(
            Document document,
            BinaryExpressionSyntax binaryExpression,
            string comparisonName,
            CancellationToken cancellationToken)
        {
            ExpressionSyntax left = binaryExpression.Left.WalkDownParentheses();
            ExpressionSyntax right = binaryExpression.Right.WalkDownParentheses();

            ExpressionSyntax newNode = SimpleMemberInvocationExpression(
                StringType(),
                IdentifierName("Equals"),
                ArgumentList(
                    CreateArgument(left),
                    CreateArgument(right),
                    Argument(CreateStringComparison(comparisonName))));

            if (binaryExpression.IsKind(SyntaxKind.NotEqualsExpression))
                newNode = LogicalNotExpression(newNode);

            newNode = newNode
                .WithTriviaFrom(binaryExpression)
                .WithFormatterAnnotation();

            return document.ReplaceNodeAsync(binaryExpression, newNode, cancellationToken);
        }

        internal static Task<Document> RefactorAsync(
            Document document,
            MemberInvocationExpressionInfo invocationInfo,
            string comparisonName,
            CancellationToken cancellationToken)
        {
            SeparatedSyntaxList<ArgumentSyntax> arguments = invocationInfo.Arguments;

            InvocationExpressionSyntax invocation = invocationInfo.InvocationExpression;

            if (arguments.Count == 2)
            {
                ArgumentListSyntax newArgumentList = ArgumentList(
                    CreateArgument(arguments[0]),
                    CreateArgument(arguments[1]),
                    Argument(CreateStringComparison(comparisonName)));

                InvocationExpressionSyntax newNode = invocation.WithArgumentList(newArgumentList);

                return document.ReplaceNodeAsync(invocation, newNode, cancellationToken);
            }
            else
            {
                var memberAccess = (MemberAccessExpressionSyntax)invocation.Expression;
                var invocation2 = (InvocationExpressionSyntax)memberAccess.Expression;
                var memberAccess2 = (MemberAccessExpressionSyntax)invocation2.Expression;

                MemberAccessExpressionSyntax newMemberAccess = memberAccess.WithExpression(memberAccess2.Expression);

                bool isContains = memberAccess.Name.Identifier.ValueText == "Contains";

                if (isContains)
                    newMemberAccess = newMemberAccess.WithName(newMemberAccess.Name.WithIdentifier(Identifier("IndexOf").WithTriviaFrom(newMemberAccess.Name.Identifier)));

                ArgumentListSyntax newArgumentList = ArgumentList(
                    CreateArgument(arguments[0]),
                    Argument(CreateStringComparison(comparisonName)));

                ExpressionSyntax newNode = invocation
                    .WithExpression(newMemberAccess)
                    .WithArgumentList(newArgumentList);

                if (isContains)
                    newNode = GreaterThanOrEqualExpression(newNode.Parenthesize(), NumericLiteralExpression(0)).Parenthesize();

                return document.ReplaceNodeAsync(invocation, newNode, cancellationToken);
            }
        }

        private static NameSyntax CreateStringComparison(string comparisonName)
        {
            return ParseName($"System.StringComparison.{comparisonName}").WithSimplifierAnnotation();
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
                        Debug.Fail(expression.Kind().ToString());
                        return Argument(expression);
                    }
            }
        }

        private static ArgumentSyntax CreateArgument(ArgumentSyntax argument)
        {
            ExpressionSyntax expression = argument.Expression.WalkDownParentheses();

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
                        Debug.Fail(expression.Kind().ToString());
                        return argument;
                    }
            }
        }
    }
}
