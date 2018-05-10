// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Roslynator.CSharp.Syntax
{
    internal static class SyntaxInfoHelpers
    {
        public static ExpressionSyntax Walk(SyntaxNode node, bool walkDownParentheses)
        {
            return Walk(node as ExpressionSyntax, walkDownParentheses);
        }

        public static ExpressionSyntax Walk(ExpressionSyntax expression, bool walkDownParentheses)
        {
            return expression?.WalkDownParenthesesIf(walkDownParentheses);
        }

        public static ExpressionSyntax WalkAndCheck(SyntaxNode node, bool walkDownParentheses, bool allowMissing)
        {
            return WalkAndCheck(Walk(node, walkDownParentheses), allowMissing);
        }

        public static ExpressionSyntax WalkAndCheck(ExpressionSyntax expression, bool walkDownParentheses, bool allowMissing)
        {
            return WalkAndCheck(Walk(expression, walkDownParentheses), allowMissing);
        }

        private static ExpressionSyntax WalkAndCheck(ExpressionSyntax expression, bool allowMissing)
        {
            return (Check(expression, allowMissing)) ? expression : null;
        }

        public static bool Check(SyntaxNode node, bool allowMissing)
        {
            return node != null
                && (allowMissing || !node.IsMissing);
        }

        public static string ToDebugString<TInfo>(bool success, TInfo info, SyntaxNode node, string text = null) where TInfo : struct
        {
            return (success)
                ? $"{info.GetType().Name} {node.Kind()} {text ?? node.ToString()}"
                : "Uninitialized";
        }

        public static string ToDebugString<TNode>(bool success, SyntaxList<TNode> list) where TNode : SyntaxNode
        {
            return (success)
                ? $"Count = {list.Count} {list}"
                : "Uninitialized";
        }

        public static string ToDebugString<TNode>(bool success, SeparatedSyntaxList<TNode> list) where TNode : SyntaxNode
        {
            return (success)
                ? $"Count = {list.Count} {list}"
                : "Uninitialized";
        }
    }
}