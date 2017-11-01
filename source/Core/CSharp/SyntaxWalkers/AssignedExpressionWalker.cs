// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Roslynator.CSharp.SyntaxWalkers
{
    internal class AssignedExpressionWalker : CSharpSyntaxWalker
    {
        public virtual void VisitAssignedExpression(ExpressionSyntax expression)
        {
        }

        private void VisitAssignedExpressionCore(ExpressionSyntax expression)
        {
            if (expression == null)
                return;

            if (expression is TupleExpressionSyntax tupleExpression)
            {
                foreach (ArgumentSyntax argument in tupleExpression.Arguments)
                {
                    ExpressionSyntax argumentExpression = argument.Expression;

                    if (argumentExpression?.IsKind(SyntaxKind.DeclarationExpression) == false)
                        VisitAssignedExpression(argumentExpression);
                }
            }
            else
            {
                VisitAssignedExpression(expression);
            }
        }

        public override void VisitAssignmentExpression(AssignmentExpressionSyntax node)
        {
            switch (node.Kind())
            {
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
                        VisitAssignedExpressionCore(node.Left);
                        VisitToken(node.OperatorToken);
                        Visit(node.Right);
                        break;
                    }
                default:
                    {
                        base.VisitAssignmentExpression(node);
                        break;
                    }
            }
        }

        public override void VisitPrefixUnaryExpression(PrefixUnaryExpressionSyntax node)
        {
            if (node.IsKind(SyntaxKind.PreIncrementExpression, SyntaxKind.PreDecrementExpression))
            {
                VisitToken(node.OperatorToken);
                VisitAssignedExpressionCore(node.Operand);
            }
            else
            {
                base.VisitPrefixUnaryExpression(node);
            }
        }

        public override void VisitPostfixUnaryExpression(PostfixUnaryExpressionSyntax node)
        {
            if (node.IsKind(SyntaxKind.PostIncrementExpression, SyntaxKind.PostDecrementExpression))
            {
                VisitAssignedExpressionCore(node.Operand);
                VisitToken(node.OperatorToken);
            }
            else
            {
                base.VisitPostfixUnaryExpression(node);
            }
        }

        public override void VisitArgument(ArgumentSyntax node)
        {
            if (node.RefOrOutKeyword.IsKind(SyntaxKind.RefKeyword, SyntaxKind.OutKeyword))
            {
                VisitToken(node.RefOrOutKeyword);
                Visit(node.NameColon);
                VisitAssignedExpressionCore(node.Expression);
            }
            else
            {
                base.VisitArgument(node);
            }
        }
    }
}
