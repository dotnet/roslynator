// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Roslynator.CSharp.CSharpFactory;
using Microsoft.CodeAnalysis.Text;

namespace Roslynator.CSharp.Refactorings
{
    internal static class AddBooleanComparisonRefactoring
    {
        public static void ComputeRefactoring(RefactoringContext context, ExpressionSyntax expression, SemanticModel semanticModel)
        {
            foreach (Diagnostic diagnostic in semanticModel.GetDiagnostics(expression.Span, context.CancellationToken))
            {
                TextSpan span = diagnostic.Location.SourceSpan;

                if (diagnostic.Id == CSharpErrorCodes.CannotImplicitlyConvertTypeExplicitConversionExists
                    && diagnostic.IsCompilerDiagnostic())
                {
                    if (context.Span.IsEmpty || span == expression.Span)
                    {
                        var expression2 = expression
                            .AncestorsAndSelf()
                            .FirstOrDefault(f => f.Span == span) as ExpressionSyntax;

                        if (expression2 != null)
                        {
                            TypeInfo typeInfo = semanticModel.GetTypeInfo(expression2, context.CancellationToken);

                            if (typeInfo.Type?.IsNullableOf(SpecialType.System_Boolean) == true)
                            {
                                if (typeInfo.ConvertedType?.IsBoolean() == true
                                    || IsCondition(expression2))
                                {
                                    RegisterRefactoring(context, expression2);
                                    break;
                                }
                            }
                        }
                    }
                }
                else if (diagnostic.Id == CSharpErrorCodes.OperatorCannotBeAppliedToOperands
                    && diagnostic.IsCompilerDiagnostic())
                {
                    if (context.Span.IsEmpty || span == expression.Span)
                    {
                        var binaryExpression = expression
                            .Ancestors()
                            .FirstOrDefault(f => f.Span == span) as BinaryExpressionSyntax;

                        if (binaryExpression != null)
                        {
                            ExpressionSyntax left = binaryExpression.Left;

                            if (left?.Span.Contains(context.Span) == true)
                            {
                                if (semanticModel.GetTypeSymbol(left, context.CancellationToken)?.IsNullableOf(SpecialType.System_Boolean) == true)
                                {
                                    RegisterRefactoring(context, left);
                                    break;
                                }
                            }
                            else
                            {
                                ExpressionSyntax right = binaryExpression.Right;

                                if (right?.Span.Contains(context.Span) == true
                                    && semanticModel.GetTypeSymbol(right, context.CancellationToken)?.IsNullableOf(SpecialType.System_Boolean) == true)
                                {
                                    RegisterRefactoring(context, right);
                                    break;
                                }
                            }
                        }
                    }
                }
            }
        }

        private static bool IsCondition(ExpressionSyntax expression)
        {
            SyntaxNode parent = expression.Parent;

            switch (parent?.Kind())
            {
                case SyntaxKind.IfStatement:
                    return ((IfStatementSyntax)parent).Condition == expression;
                case SyntaxKind.DoStatement:
                    return ((DoStatementSyntax)parent).Condition == expression;
                case SyntaxKind.WhileStatement:
                    return ((WhileStatementSyntax)parent).Condition == expression;
                default:
                    return false;
            }
        }

        private static void RegisterRefactoring(RefactoringContext context, ExpressionSyntax expression)
        {
            context.RegisterRefactoring(
                (expression.IsKind(SyntaxKind.LogicalNotExpression)) ? "Add ' == false'" : "Add ' == true'",
                cancellationToken => RefactorAsync(context.Document, expression, cancellationToken));
        }

        public static Task<Document> RefactorAsync(
            Document document,
            ExpressionSyntax expression,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            BinaryExpressionSyntax newNode = CreateNewExpression(expression)
                .WithTriviaFrom(expression)
                .WithFormatterAnnotation();

            return document.ReplaceNodeAsync(expression, newNode, cancellationToken);
        }

        private static BinaryExpressionSyntax CreateNewExpression(ExpressionSyntax expression)
        {
            if (expression.IsKind(SyntaxKind.LogicalNotExpression))
            {
                var logicalNot = (PrefixUnaryExpressionSyntax)expression;

                return EqualsExpression(
                    logicalNot.Operand.WithoutTrivia(),
                    FalseLiteralExpression());
            }
            else
            {
                return EqualsExpression(
                    expression.WithoutTrivia(),
                    TrueLiteralExpression());
            }
        }
    }
}
