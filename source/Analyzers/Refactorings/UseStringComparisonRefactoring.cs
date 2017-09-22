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
        public static void Analyze(SyntaxNodeAnalysisContext context, MemberInvocationExpression memberInvocation)
        {
            ExpressionSyntax expression = memberInvocation.InvocationExpression.WalkUpParentheses();

            SyntaxNode parent = expression.Parent;

            SyntaxKind kind = parent.Kind();

            if (kind == SyntaxKind.SimpleMemberAccessExpression)
            {
                if (!MemberInvocationExpression.TryCreate(parent.Parent, out MemberInvocationExpression memberInvocation2))
                    return;

                Analyze(context, memberInvocation, memberInvocation2);
            }
            else if (kind == SyntaxKind.Argument)
            {
                Analyze(context, memberInvocation, (ArgumentSyntax)parent);
            }
            else if (kind == SyntaxKind.EqualsExpression
                || kind == SyntaxKind.NotEqualsExpression)
            {
                Analyze(context, memberInvocation, expression, (BinaryExpressionSyntax)parent);
            }
        }

        private static void Analyze(
            SyntaxNodeAnalysisContext context,
            MemberInvocationExpression invocation,
            MemberInvocationExpression invocation2)
        {
            if (invocation2.InvocationExpression.SpanContainsDirectives())
                return;

            string name2 = invocation2.NameText;

            if (name2 != "Equals"
                && name2 != "StartsWith"
                && name2 != "EndsWith"
                && name2 != "IndexOf"
                && name2 != "LastIndexOf"
                && name2 != "Contains")
            {
                return;
            }

            SeparatedSyntaxList<ArgumentSyntax> arguments = invocation2.ArgumentList.Arguments;

            if (arguments.Count != 1)
                return;

            ExpressionSyntax argumentExpression = arguments[0].Expression.WalkDownParentheses();

            string name = invocation.NameText;

            MemberInvocationExpression invocation3;
            string name3 = null;

            bool isStringLiteral = argumentExpression.IsKind(SyntaxKind.StringLiteralExpression);

            if (!isStringLiteral)
            {
                if (!MemberInvocationExpression.TryCreate(argumentExpression, out invocation3))
                    return;

                name3 = invocation3.NameText;

                if (name != name3)
                    return;
            }

            SemanticModel semanticModel = context.SemanticModel;
            CancellationToken cancellationToken = context.CancellationToken;

            if (!CheckSymbol(invocation, semanticModel, cancellationToken))
                return;

            if (!(semanticModel.TryGetMethodInfo(invocation2.InvocationExpression, out MethodInfo info, cancellationToken)
                && info.IsPublicInstanceStringMethod(name2)
                && info.ReturnType.SpecialType == ((name2.EndsWith("IndexOf", StringComparison.Ordinal)) ? SpecialType.System_Int32 : SpecialType.System_Boolean)
                && info.HasParameter(SpecialType.System_String)))
            {
                return;
            }

            if (!isStringLiteral
                && !CheckSymbol(invocation3, semanticModel, cancellationToken))
            {
                return;
            }

            ReportDiagnostic(context, invocation2);
        }

        private static void Analyze(
            SyntaxNodeAnalysisContext context,
            MemberInvocationExpression invocation,
            ArgumentSyntax argument)
        {
            if (!(argument.Parent is ArgumentListSyntax argumentList))
                return;

            SeparatedSyntaxList<ArgumentSyntax> arguments = argumentList.Arguments;

            if (arguments.Count != 2)
                return;

            if (!(MemberInvocationExpression.TryCreate(argumentList.Parent, out MemberInvocationExpression equalsInvocation)))
                return;

            if (equalsInvocation.NameText != "Equals")
                return;

            if (!IsFixable(context, invocation, argument, arguments))
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
            MemberInvocationExpression invocation,
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
                        return TryCreateCaseChangingInvocation(expression, out MemberInvocationExpression invocation2)
                            && invocation.NameText == invocation2.NameText
                            && CheckSymbol(invocation, context.SemanticModel, context.CancellationToken)
                            && CheckSymbol(invocation2, context.SemanticModel, context.CancellationToken);
                    }
                    else if (kind == SyntaxKind.StringLiteralExpression)
                    {
                        return CheckSymbol(invocation, context.SemanticModel, context.CancellationToken);
                    }
                }
            }
            else
            {
                return arguments[0].Expression?.WalkDownParentheses().Kind() == SyntaxKind.StringLiteralExpression
                    && CheckSymbol(invocation, context.SemanticModel, context.CancellationToken);
            }

            return false;
        }

        private static void Analyze(
            SyntaxNodeAnalysisContext context,
            MemberInvocationExpression invocation,
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
                        if (TryCreateCaseChangingInvocation(right, out MemberInvocationExpression invocation2)
                            && invocation.NameText == invocation2.NameText
                            && CheckSymbol(invocation, context.SemanticModel, context.CancellationToken)
                            && CheckSymbol(invocation2, context.SemanticModel, context.CancellationToken))
                        {
                            ReportDiagnostic(context, binaryExpression);
                        }
                    }
                    else if (kind == SyntaxKind.StringLiteralExpression)
                    {
                        if (CheckSymbol(invocation, context.SemanticModel, context.CancellationToken))
                        {
                            ReportDiagnostic(context, binaryExpression);
                        }
                    }
                }
            }
            else if (binaryExpression.Left?.WalkDownParentheses().Kind() == SyntaxKind.StringLiteralExpression
                && CheckSymbol(invocation, context.SemanticModel, context.CancellationToken))
            {
                ReportDiagnostic(context, binaryExpression);
            }
        }

        private static bool TryCreateCaseChangingInvocation(ExpressionSyntax expression, out MemberInvocationExpression invocation)
        {
            if (MemberInvocationExpression.TryCreate(expression, out invocation)
                && !invocation.ArgumentList.Arguments.Any())
            {
                string name = invocation.NameText;

                return name == "ToLower"
                    || name == "ToLowerInvariant"
                    || name == "ToUpper"
                    || name == "ToUpperInvariant";
            }

            invocation = default(MemberInvocationExpression);
            return false;
        }

        private static bool CheckSymbol(
            MemberInvocationExpression invocation,
            SemanticModel semanticModel,
            CancellationToken cancellationToken)
        {
            return semanticModel.TryGetMethodInfo(invocation.InvocationExpression, out MethodInfo info, cancellationToken)
                && info.IsPublicInstanceStringMethod()
                && info.ReturnsString
                && !info.Parameters.Any();
        }

        private static void ReportDiagnostic(SyntaxNodeAnalysisContext context, MemberInvocationExpression invocation)
        {
            ReportDiagnostic(context, invocation.InvocationExpression);
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
            MemberInvocationExpression memberInvocation,
            string comparisonName,
            CancellationToken cancellationToken)
        {
            SeparatedSyntaxList<ArgumentSyntax> arguments = memberInvocation.ArgumentList.Arguments;

            InvocationExpressionSyntax invocationExpression = memberInvocation.InvocationExpression;

            if (arguments.Count == 2)
            {
                ArgumentListSyntax newArgumentList = ArgumentList(
                    CreateArgument(arguments[0]),
                    CreateArgument(arguments[1]),
                    Argument(CreateStringComparison(comparisonName)));

                InvocationExpressionSyntax newNode = invocationExpression.WithArgumentList(newArgumentList);

                return document.ReplaceNodeAsync(invocationExpression, newNode, cancellationToken);
            }
            else
            {
                var memberAccess = (MemberAccessExpressionSyntax)invocationExpression.Expression;
                var invocation2 = (InvocationExpressionSyntax)memberAccess.Expression;
                var memberAccess2 = (MemberAccessExpressionSyntax)invocation2.Expression;

                MemberAccessExpressionSyntax newMemberAccess = memberAccess.WithExpression(memberAccess2.Expression);

                bool isContains = memberAccess.Name.Identifier.ValueText == "Contains";

                if (isContains)
                    newMemberAccess = newMemberAccess.WithName(newMemberAccess.Name.WithIdentifier(Identifier("IndexOf").WithTriviaFrom(newMemberAccess.Name.Identifier)));

                ArgumentListSyntax newArgumentList = ArgumentList(
                    CreateArgument(arguments[0]),
                    Argument(CreateStringComparison(comparisonName)));

                ExpressionSyntax newNode = invocationExpression
                    .WithExpression(newMemberAccess)
                    .WithArgumentList(newArgumentList);

                if (isContains)
                    newNode = GreaterThanOrEqualExpression(newNode.Parenthesize(), NumericLiteralExpression(0)).Parenthesize();

                return document.ReplaceNodeAsync(invocationExpression, newNode, cancellationToken);
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
