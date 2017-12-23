// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.CSharp.Syntax;

namespace Roslynator.CSharp.Refactorings.ExtractCondition
{
    internal static class ExtractConditionRefactoring
    {
        internal static void ComputeRefactoring(RefactoringContext context, BinaryExpressionSelection binaryExpressionSelection)
        {
            BinaryExpressionSyntax binaryExpression = binaryExpressionSelection.BinaryExpression;

            SyntaxKind kind = binaryExpression.Kind();

            if (kind == SyntaxKind.LogicalAndExpression
                || kind == SyntaxKind.LogicalOrExpression)
            {
                BinaryExpressionSyntax condition = GetCondition(binaryExpression);

                if (condition != null)
                {
                    SyntaxNode parent = condition.Parent;

                    switch (parent?.Kind())
                    {
                        case SyntaxKind.IfStatement:
                            {
                                if (kind == SyntaxKind.LogicalAndExpression)
                                {
                                    var refactoring = new ExtractConditionFromIfToNestedIfRefactoring();
                                    context.RegisterRefactoring(
                                        refactoring.Title,
                                        cancellationToken => refactoring.RefactorAsync(context.Document, (IfStatementSyntax)parent, condition, binaryExpressionSelection, cancellationToken));
                                }
                                else if (kind == SyntaxKind.LogicalOrExpression)
                                {
                                    StatementsInfo statementsInfo = SyntaxInfo.StatementsInfo((StatementSyntax)parent);
                                    if (statementsInfo.Success)
                                    {
                                        var refactoring = new ExtractConditionFromIfToIfRefactoring();
                                        context.RegisterRefactoring(
                                            refactoring.Title,
                                            cancellationToken => refactoring.RefactorAsync(context.Document, statementsInfo, condition, binaryExpressionSelection, cancellationToken));
                                    }
                                }

                                break;
                            }
                        case SyntaxKind.WhileStatement:
                            {
                                if (kind == SyntaxKind.LogicalAndExpression)
                                {
                                    var refactoring = new ExtractConditionFromWhileToNestedIfRefactoring();
                                    context.RegisterRefactoring(
                                        refactoring.Title,
                                        cancellationToken => refactoring.RefactorAsync(context.Document, (WhileStatementSyntax)parent, condition, binaryExpressionSelection, cancellationToken));
                                }

                                break;
                            }
                    }
                }
            }
        }

        public static void ComputeRefactoring(RefactoringContext context, ExpressionSyntax expression)
        {
            SyntaxNode parent = expression.Parent;

            if (parent != null)
            {
                SyntaxKind kind = parent.Kind();

                if (kind == SyntaxKind.LogicalAndExpression
                    || kind == SyntaxKind.LogicalOrExpression)
                {
                    BinaryExpressionSyntax binaryExpression = GetCondition((BinaryExpressionSyntax)parent);

                    if (binaryExpression != null)
                    {
                        parent = binaryExpression.Parent;

                        switch (parent?.Kind())
                        {
                            case SyntaxKind.IfStatement:
                                {
                                    if (kind == SyntaxKind.LogicalAndExpression)
                                    {
                                        var refactoring = new ExtractConditionFromIfToNestedIfRefactoring();
                                        context.RegisterRefactoring(
                                            refactoring.Title,
                                            cancellationToken => refactoring.RefactorAsync(context.Document, binaryExpression, expression, cancellationToken));
                                    }
                                    else if (kind == SyntaxKind.LogicalOrExpression)
                                    {
                                        StatementsInfo statementsInfo = SyntaxInfo.StatementsInfo((StatementSyntax)parent);
                                        if (statementsInfo.Success)
                                        {
                                            var refactoring = new ExtractConditionFromIfToIfRefactoring();
                                            context.RegisterRefactoring(
                                                refactoring.Title,
                                                cancellationToken => refactoring.RefactorAsync(context.Document, statementsInfo, binaryExpression, expression, cancellationToken));
                                        }
                                    }

                                    break;
                                }
                            case SyntaxKind.WhileStatement:
                                {
                                    if (kind == SyntaxKind.LogicalAndExpression)
                                    {
                                        StatementsInfo statementsInfo = SyntaxInfo.StatementsInfo((StatementSyntax)parent);
                                        if (statementsInfo.Success)
                                        {
                                            var refactoring = new ExtractConditionFromWhileToNestedIfRefactoring();
                                            context.RegisterRefactoring(
                                                refactoring.Title,
                                                cancellationToken => refactoring.RefactorAsync(context.Document, (WhileStatementSyntax)parent, binaryExpression, expression, cancellationToken));
                                        }
                                    }

                                    break;
                                }
                        }
                    }
                }
            }
        }

        private static BinaryExpressionSyntax GetCondition(BinaryExpressionSyntax binaryExpression)
        {
            SyntaxKind kind = binaryExpression.Kind();

            for (SyntaxNode parent = binaryExpression.Parent; parent != null; parent = parent.Parent)
            {
                SyntaxKind parentKind = parent.Kind();

                if (parentKind == kind)
                {
                    binaryExpression = (BinaryExpressionSyntax)parent;
                }
                else if (parentKind == SyntaxKind.IfStatement || parentKind == SyntaxKind.WhileStatement)
                {
                    return binaryExpression;
                }
                else
                {
                    return null;
                }
            }

            return null;
        }
    }
}
