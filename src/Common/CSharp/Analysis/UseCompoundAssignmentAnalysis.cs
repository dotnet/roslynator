// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.CSharp.Syntax;
using static Roslynator.CSharp.CSharpFacts;

namespace Roslynator.CSharp.Analysis
{
    internal static class UseCompoundAssignmentAnalysis
    {
        public static bool IsFixable(AssignmentExpressionSyntax assignmentExpression)
        {
            SimpleAssignmentExpressionInfo assignmentInfo = SyntaxInfo.SimpleAssignmentExpressionInfo(assignmentExpression);

            if (!assignmentInfo.Success)
                return false;

            if (assignmentExpression.IsParentKind(SyntaxKind.ObjectInitializerExpression))
                return false;

            if (!IsFixableBinaryExpression(assignmentInfo.Right.Kind()))
                return false;

            BinaryExpressionInfo binaryInfo = SyntaxInfo.BinaryExpressionInfo((BinaryExpressionSyntax)assignmentInfo.Right);

            if (!binaryInfo.Success)
                return false;

            if (!CSharpFactory.AreEquivalent(assignmentInfo.Left, binaryInfo.Left))
                return false;

            return true;
        }

        private static bool IsFixableBinaryExpression(SyntaxKind kind)
        {
            switch (kind)
            {
                case SyntaxKind.AddExpression:
                case SyntaxKind.SubtractExpression:
                case SyntaxKind.MultiplyExpression:
                case SyntaxKind.DivideExpression:
                case SyntaxKind.ModuloExpression:
                case SyntaxKind.BitwiseAndExpression:
                case SyntaxKind.ExclusiveOrExpression:
                case SyntaxKind.BitwiseOrExpression:
                case SyntaxKind.LeftShiftExpression:
                case SyntaxKind.RightShiftExpression:
                    return true;
                default:
                    return false;
            }
        }

        public static string GetCompoundOperatorText(BinaryExpressionSyntax binaryExpression)
        {
            SyntaxKind compoundAssignmentKind = GetCompoundAssignmentKind(binaryExpression.Kind());

            SyntaxKind compoundAssignmentOperatorKind = GetCompoundAssignmentOperatorKind(compoundAssignmentKind);

            return SyntaxFacts.GetText(compoundAssignmentOperatorKind);
        }
    }
}
