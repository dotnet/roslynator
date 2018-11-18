// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Roslynator.CSharp.Analysis
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class RemoveRedundantParenthesesAnalyzer : BaseDiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        {
            get
            {
                return ImmutableArray.Create(
                    DiagnosticDescriptors.RemoveRedundantParentheses,
                    DiagnosticDescriptors.RemoveRedundantParenthesesFadeOut);
            }
        }

        public override void Initialize(AnalysisContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            base.Initialize(context);

            context.RegisterCompilationStartAction(startContext =>
            {
                if (startContext.IsAnalyzerSuppressed(DiagnosticDescriptors.RemoveRedundantParentheses))
                    return;

                startContext.RegisterSyntaxNodeAction(AnalyzeParenthesizedExpression, SyntaxKind.ParenthesizedExpression);
            });
        }

        private static void AnalyzeParenthesizedExpression(SyntaxNodeAnalysisContext context)
        {
            var parenthesizedExpression = (ParenthesizedExpressionSyntax)context.Node;

            ExpressionSyntax expression = parenthesizedExpression.Expression;

            if (expression?.IsMissing != false)
                return;

            SyntaxToken openParen = parenthesizedExpression.OpenParenToken;

            if (openParen.IsMissing)
                return;

            SyntaxToken closeParen = parenthesizedExpression.CloseParenToken;

            if (closeParen.IsMissing)
                return;

            SyntaxNode parent = parenthesizedExpression.Parent;

            SyntaxKind parentKind = parent.Kind();

            switch (parentKind)
            {
                case SyntaxKind.ParenthesizedExpression:
                case SyntaxKind.ArrowExpressionClause:
                case SyntaxKind.AttributeArgument:
                case SyntaxKind.Argument:
                case SyntaxKind.ExpressionStatement:
                case SyntaxKind.ReturnStatement:
                case SyntaxKind.YieldReturnStatement:
                case SyntaxKind.WhileStatement:
                case SyntaxKind.DoStatement:
                case SyntaxKind.UsingStatement:
                case SyntaxKind.LockStatement:
                case SyntaxKind.IfStatement:
                case SyntaxKind.SwitchStatement:
                case SyntaxKind.ArrayRankSpecifier:
                    {
                        ReportDiagnostic();
                        break;
                    }
                case SyntaxKind.LessThanExpression:
                case SyntaxKind.GreaterThanExpression:
                case SyntaxKind.LessThanOrEqualExpression:
                case SyntaxKind.GreaterThanOrEqualExpression:
                case SyntaxKind.EqualsExpression:
                case SyntaxKind.NotEqualsExpression:
                    {
                        if (expression.IsKind(SyntaxKind.IdentifierName)
                            || expression is LiteralExpressionSyntax)
                        {
                            ReportDiagnostic();
                        }

                        break;
                    }
                case SyntaxKind.MultiplyExpression:
                case SyntaxKind.DivideExpression:
                case SyntaxKind.ModuloExpression:
                case SyntaxKind.AddExpression:
                case SyntaxKind.SubtractExpression:
                case SyntaxKind.LeftShiftExpression:
                case SyntaxKind.RightShiftExpression:
                case SyntaxKind.BitwiseAndExpression:
                case SyntaxKind.ExclusiveOrExpression:
                case SyntaxKind.BitwiseOrExpression:
                case SyntaxKind.LogicalAndExpression:
                case SyntaxKind.LogicalOrExpression:
                    {
                        SyntaxKind kind = expression.Kind();

                        if (kind == SyntaxKind.IdentifierName
                            || expression is LiteralExpressionSyntax)
                        {
                            ReportDiagnostic();
                        }
                        else if (kind == parentKind
                            && ((BinaryExpressionSyntax)parent).Left == parenthesizedExpression)
                        {
                            ReportDiagnostic();
                        }

                        break;
                    }
                case SyntaxKind.LogicalNotExpression:
                    {
                        if (expression.Kind().Is(
                          SyntaxKind.IdentifierName,
                          SyntaxKind.GenericName,
                          SyntaxKind.InvocationExpression,
                          SyntaxKind.SimpleMemberAccessExpression,
                          SyntaxKind.ElementAccessExpression,
                          SyntaxKind.ConditionalAccessExpression))
                        {
                            ReportDiagnostic();
                        }

                        break;
                    }
                case SyntaxKind.SimpleAssignmentExpression:
                case SyntaxKind.AddAssignmentExpression:
                case SyntaxKind.SubtractAssignmentExpression:
                case SyntaxKind.MultiplyAssignmentExpression:
                case SyntaxKind.DivideAssignmentExpression:
                case SyntaxKind.ModuloAssignmentExpression:
                case SyntaxKind.AndAssignmentExpression:
                case SyntaxKind.ExclusiveOrAssignmentExpression:
                case SyntaxKind.OrAssignmentExpression:
                case SyntaxKind.LeftShiftAssignmentExpression:
                case SyntaxKind.RightShiftAssignmentExpression:
                    {
                        if (((AssignmentExpressionSyntax)parent).Left == parenthesizedExpression)
                        {
                            ReportDiagnostic();
                        }
                        else if (expression.IsKind(SyntaxKind.IdentifierName)
                            || expression is LiteralExpressionSyntax)
                        {
                            ReportDiagnostic();
                        }

                        break;
                    }
                case SyntaxKind.Interpolation:
                    {
                        if (expression.Kind() != SyntaxKind.ConditionalExpression
                            && ((InterpolationSyntax)parent).Expression == parenthesizedExpression)
                        {
                            ReportDiagnostic();
                        }

                        break;
                    }
                case SyntaxKind.AwaitExpression:
                    {
                        if (CSharpFacts.GetOperatorPrecedence(expression.Kind()) <= CSharpFacts.GetOperatorPrecedence(SyntaxKind.AwaitExpression))
                            ReportDiagnostic();

                        break;
                    }
                case SyntaxKind.ArrayInitializerExpression:
                case SyntaxKind.CollectionInitializerExpression:
                    {
                        if (!(expression is AssignmentExpressionSyntax))
                            ReportDiagnostic();

                        break;
                    }
            }

            void ReportDiagnostic()
            {
                context.ReportDiagnostic(
                   DiagnosticDescriptors.RemoveRedundantParentheses,
                   openParen.GetLocation(),
                   additionalLocations: ImmutableArray.Create(closeParen.GetLocation()));

                context.ReportToken(DiagnosticDescriptors.RemoveRedundantParenthesesFadeOut, openParen);
                context.ReportToken(DiagnosticDescriptors.RemoveRedundantParenthesesFadeOut, closeParen);
            }
        }
    }
}
