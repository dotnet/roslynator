// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.CodeAnalysis.CSharp;

namespace Roslynator.CSharp
{
    public static class SyntaxKindExtensions
    {
        internal static bool IsNestedMethod(this SyntaxKind kind)
        {
            return kind.IsKind(
                SyntaxKind.SimpleLambdaExpression,
                SyntaxKind.ParenthesizedLambdaExpression,
                SyntaxKind.AnonymousMethodExpression,
                SyntaxKind.LocalFunctionStatement);
        }

        internal static bool IsLoop(this SyntaxKind kind)
        {
            return kind.IsKind(
                SyntaxKind.ForStatement,
                SyntaxKind.ForEachStatement,
                SyntaxKind.WhileStatement,
                SyntaxKind.DoStatement);
        }

        internal static bool IsJumpStatement(this SyntaxKind kind)
        {
            return kind.IsKind(
                SyntaxKind.BreakStatement,
                SyntaxKind.ContinueStatement,
                SyntaxKind.GotoCaseStatement,
                SyntaxKind.GotoDefaultStatement,
                SyntaxKind.ReturnStatement,
                SyntaxKind.ThrowStatement);
        }

        internal static bool IsJumpStatementOrYieldBreakStatement(this SyntaxKind kind)
        {
            return kind.IsJumpStatement()
                || kind == SyntaxKind.YieldBreakStatement;
        }

        internal static bool IsBooleanLiteralExpression(this SyntaxKind kind)
        {
            return kind.IsKind(
                SyntaxKind.TrueLiteralExpression,
                SyntaxKind.FalseLiteralExpression);
        }

        internal static bool IsKind(this SyntaxKind kind, SyntaxKind kind1, SyntaxKind kind2)
        {
            return kind == kind1
                || kind == kind2;
        }

        public static bool IsKind(this SyntaxKind kind, SyntaxKind kind1, SyntaxKind kind2, SyntaxKind kind3)
        {
            return kind == kind1
                || kind == kind2
                || kind == kind3;
        }

        public static bool IsKind(this SyntaxKind kind, SyntaxKind kind1, SyntaxKind kind2, SyntaxKind kind3, SyntaxKind kind4)
        {
            return kind == kind1
                || kind == kind2
                || kind == kind3
                || kind == kind4;
        }

        public static bool IsKind(this SyntaxKind kind, SyntaxKind kind1, SyntaxKind kind2, SyntaxKind kind3, SyntaxKind kind4, SyntaxKind kind5)
        {
            return kind == kind1
                || kind == kind2
                || kind == kind3
                || kind == kind4
                || kind == kind5;
        }

        public static bool IsKind(this SyntaxKind kind, SyntaxKind kind1, SyntaxKind kind2, SyntaxKind kind3, SyntaxKind kind4, SyntaxKind kind5, SyntaxKind kind6)
        {
            return kind == kind1
                || kind == kind2
                || kind == kind3
                || kind == kind4
                || kind == kind5
                || kind == kind6;
        }

        public static bool IsSingleTokenExpression(this SyntaxKind kind)
        {
            switch (kind)
            {
                case SyntaxKind.IdentifierName:
                case SyntaxKind.PredefinedType:
                case SyntaxKind.ThisExpression:
                case SyntaxKind.BaseExpression:
                case SyntaxKind.NumericLiteralExpression:
                case SyntaxKind.StringLiteralExpression:
                case SyntaxKind.CharacterLiteralExpression:
                case SyntaxKind.TrueLiteralExpression:
                case SyntaxKind.FalseLiteralExpression:
                case SyntaxKind.NullLiteralExpression:
                    return true;
                default:
                    return false;
            }
        }
    }
}
