// Copyright (c) .NET Foundation and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Text;

namespace Roslynator.CSharp.Analysis;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class RemoveRedundantParenthesesAnalyzer : BaseDiagnosticAnalyzer
{
    private static ImmutableArray<DiagnosticDescriptor> _supportedDiagnostics;

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
    {
        get
        {
            if (_supportedDiagnostics.IsDefault)
            {
                Immutable.InterlockedInitialize(
                    ref _supportedDiagnostics,
                    DiagnosticRules.RemoveRedundantParentheses,
                    DiagnosticRules.RemoveRedundantParenthesesFadeOut);
            }

            return _supportedDiagnostics;
        }
    }

    public override void Initialize(AnalysisContext context)
    {
        base.Initialize(context);

        context.RegisterSyntaxNodeAction(
            c =>
            {
                if (DiagnosticRules.RemoveRedundantParentheses.IsEffective(c))
                    AnalyzeParenthesizedExpression(c);
            },
            SyntaxKind.ParenthesizedExpression);
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
                ReportDiagnostic(context, parenthesizedExpression);
                break;
            }
            case SyntaxKind.LessThanExpression:
            case SyntaxKind.GreaterThanExpression:
            case SyntaxKind.LessThanOrEqualExpression:
            case SyntaxKind.GreaterThanOrEqualExpression:
            case SyntaxKind.EqualsExpression:
            case SyntaxKind.NotEqualsExpression:
            case SyntaxKind.SimpleMemberAccessExpression:
            {
                if (expression.IsKind(SyntaxKind.IdentifierName)
                    || expression is LiteralExpressionSyntax)
                {
                    ReportDiagnostic(context, parenthesizedExpression);
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
                    ReportDiagnostic(context, parenthesizedExpression);
                }
                else if (kind == parentKind
                    && ((BinaryExpressionSyntax)parent).Left == parenthesizedExpression)
                {
                    ReportDiagnostic(context, parenthesizedExpression);
                }

                break;
            }
            case SyntaxKind.LogicalNotExpression:
            {
                switch (expression.Kind())
                {
                    case SyntaxKind.IdentifierName:
                    case SyntaxKind.GenericName:
                    case SyntaxKind.InvocationExpression:
                    case SyntaxKind.SimpleMemberAccessExpression:
                    case SyntaxKind.ElementAccessExpression:
                    case SyntaxKind.ConditionalAccessExpression:
                    {
                        ReportDiagnostic(context, parenthesizedExpression);
                        break;
                    }
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
                    ReportDiagnostic(context, parenthesizedExpression);
                }
                else if (expression.IsKind(SyntaxKind.IdentifierName)
                    || expression is LiteralExpressionSyntax)
                {
                    ReportDiagnostic(context, parenthesizedExpression);
                }

                break;
            }
            case SyntaxKind.Interpolation:
            {
                if (!expression.IsKind(SyntaxKind.ConditionalExpression)
                    && !expression.DescendantNodes().Any(f => f.IsKind(SyntaxKind.AliasQualifiedName))
                    && ((InterpolationSyntax)parent).Expression == parenthesizedExpression)
                {
                    ReportDiagnostic(context, parenthesizedExpression);
                }

                break;
            }
            case SyntaxKind.AwaitExpression:
            {
                if (parenthesizedExpression.Expression.IsKind(SyntaxKind.SwitchExpression))
                    return;

                if (CSharpFacts.GetOperatorPrecedence(expression.Kind()) <= CSharpFacts.GetOperatorPrecedence(SyntaxKind.AwaitExpression))
                    ReportDiagnostic(context, parenthesizedExpression);

                break;
            }
            case SyntaxKind.ArrayInitializerExpression:
            case SyntaxKind.CollectionInitializerExpression:
            {
                if (expression is not AssignmentExpressionSyntax)
                    ReportDiagnostic(context, parenthesizedExpression);

                break;
            }
            case SyntaxKind.SimpleLambdaExpression:
            case SyntaxKind.ParenthesizedLambdaExpression:
            {
                switch (parent.Parent.Kind())
                {
                    case SyntaxKind.ParenthesizedExpression:
                    case SyntaxKind.ArrowExpressionClause:
                    case SyntaxKind.Argument:
                    case SyntaxKind.ReturnStatement:
                    case SyntaxKind.YieldReturnStatement:
                    case SyntaxKind.SimpleAssignmentExpression:
                    case SyntaxKind.AddAssignmentExpression:
                    case SyntaxKind.SubtractAssignmentExpression:
                    {
                        ReportDiagnostic(context, parenthesizedExpression);
                        break;
                    }
#if DEBUG
                    default:
                    {
                        SyntaxDebug.Fail(parent.Parent);
                        break;
                    }
#endif
                }

                break;
            }
        }

        static void ReportDiagnostic(SyntaxNodeAnalysisContext context, ParenthesizedExpressionSyntax parenthesizedExpression)
        {
            if (parenthesizedExpression.Expression.IsKind(SyntaxKind.LessThanExpression)
                && parenthesizedExpression.IsParentKind(SyntaxKind.Argument)
                && parenthesizedExpression.Parent.Parent is BaseArgumentListSyntax argumentList)
            {
                SeparatedSyntaxList<ArgumentSyntax> arguments = argumentList.Arguments;
                int index = arguments.IndexOf((ArgumentSyntax)parenthesizedExpression.Parent);

                if (index < arguments.Count - 1)
                {
                    string syntax = parenthesizedExpression.Expression
                        + argumentList.ToString(TextSpan.FromBounds(arguments[index].Span.End, arguments[index + 1].Span.End));

                    NameSyntax name = SyntaxFactory.ParseName(syntax);

                    if (name.IsKind(SyntaxKind.GenericName))
                        return;
                }
            }

            DiagnosticHelpers.ReportDiagnostic(
                context,
                DiagnosticRules.RemoveRedundantParentheses,
                parenthesizedExpression.OpenParenToken.GetLocation(),
                additionalLocations: ImmutableArray.Create(parenthesizedExpression.CloseParenToken.GetLocation()));

            DiagnosticHelpers.ReportToken(context, DiagnosticRules.RemoveRedundantParenthesesFadeOut, parenthesizedExpression.OpenParenToken);
            DiagnosticHelpers.ReportToken(context, DiagnosticRules.RemoveRedundantParenthesesFadeOut, parenthesizedExpression.CloseParenToken);
        }
    }
}
