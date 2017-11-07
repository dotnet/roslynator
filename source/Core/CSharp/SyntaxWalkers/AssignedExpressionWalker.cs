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

        public override void VisitAssignmentExpression(AssignmentExpressionSyntax node)
        {
            ExpressionSyntax left = node.Left;

            if (node.Kind() == SyntaxKind.SimpleAssignmentExpression
                && (left is TupleExpressionSyntax tupleExpression))
            {
                foreach (ArgumentSyntax argument in tupleExpression.Arguments)
                {
                    ExpressionSyntax expression = argument.Expression;

                    if (expression?.IsKind(SyntaxKind.DeclarationExpression) == false)
                        VisitAssignedExpression(expression);

                    VisitArgument(argument);
                }
            }
            else
            {
                VisitAssignedExpression(left);
                Visit(left);
            }

            Visit(node.Right);
        }

        public override void VisitPrefixUnaryExpression(PrefixUnaryExpressionSyntax node)
        {
            if (node.IsKind(SyntaxKind.PreIncrementExpression, SyntaxKind.PreDecrementExpression))
            {
                ExpressionSyntax operand = node.Operand;

                VisitAssignedExpression(operand);
                Visit(operand);
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
                ExpressionSyntax operand = node.Operand;

                VisitAssignedExpression(operand);
                Visit(operand);
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
                ExpressionSyntax expression = node.Expression;

                if (expression?.IsKind(SyntaxKind.DeclarationExpression) == false)
                    VisitAssignedExpression(expression);
            }

            base.VisitArgument(node);
        }
    }
}
