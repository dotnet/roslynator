// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Diagnostics;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Pihrtsoft.CodeAnalysis.CSharp
{
    internal static class SyntaxHelper
    {
        public static bool AreParenthesesUnnecessary(ExpressionSyntax expression)
        {
            switch (expression.Kind())
            {
                case SyntaxKind.ParenthesizedExpression:
                case SyntaxKind.Argument:
                case SyntaxKind.AttributeArgument:
                    return true;
            }

            SyntaxNode parent = expression.Parent;

            switch (parent?.Kind())
            {
                case SyntaxKind.ParenthesizedExpression:
                case SyntaxKind.Argument:
                case SyntaxKind.AttributeArgument:
                case SyntaxKind.SimpleMemberAccessExpression:
                case SyntaxKind.InvocationExpression:
                case SyntaxKind.ConditionalAccessExpression:
                case SyntaxKind.ReturnStatement:
                case SyntaxKind.YieldReturnStatement:
                case SyntaxKind.ExpressionStatement:
                    return true;
                case SyntaxKind.ForEachStatement:
                    return expression == ((ForEachStatementSyntax)parent).Expression;
                case SyntaxKind.WhileStatement:
                    return expression == ((WhileStatementSyntax)parent).Condition;
                case SyntaxKind.DoStatement:
                    return expression == ((DoStatementSyntax)parent).Condition;
                case SyntaxKind.UsingStatement:
                    return expression == ((UsingStatementSyntax)parent).Expression;
                case SyntaxKind.LockStatement:
                    return expression == ((LockStatementSyntax)parent).Expression;
                case SyntaxKind.IfStatement:
                    return expression == ((IfStatementSyntax)parent).Condition;
                case SyntaxKind.SwitchStatement:
                    return expression == ((SwitchStatementSyntax)parent).Expression;
            }

            if (parent is AssignmentExpressionSyntax)
                return true;

            if (parent?.IsKind(SyntaxKind.EqualsValueClause) == true)
            {
                parent = parent.Parent;

                if (parent?.IsKind(SyntaxKind.VariableDeclarator) == true)
                {
                    parent = parent.Parent;

                    if (parent?.IsKind(SyntaxKind.VariableDeclaration) == true)
                        return true;
                }
            }

            return false;
        }

        public static string GetSyntaxNodeTitle(SyntaxNode node)
        {
            switch (node.Kind())
            {
                case SyntaxKind.IfStatement:
                    return "if statement";
                case SyntaxKind.ElseClause:
                    return "else clause";
                case SyntaxKind.ForEachStatement:
                    return "foreach statement";
                case SyntaxKind.ForStatement:
                    return "for statement";
                case SyntaxKind.UsingStatement:
                    return "using statement";
                case SyntaxKind.WhileStatement:
                    return "while statement";
                case SyntaxKind.DoStatement:
                    return "do statement";
                case SyntaxKind.LockStatement:
                    return "lock statement";
                case SyntaxKind.FixedStatement:
                    return "fixed statement";
                case SyntaxKind.CheckedStatement:
                    return "checked statement";
                case SyntaxKind.UncheckedStatement:
                    return "unchecked statement";
                case SyntaxKind.TryStatement:
                    return "try statement";
                case SyntaxKind.UnsafeStatement:
                    return "unsafe statement";
                case SyntaxKind.MethodDeclaration:
                    return "method";
                case SyntaxKind.OperatorDeclaration:
                    return "operator method";
                case SyntaxKind.ConversionOperatorDeclaration:
                    return "conversion method";
                case SyntaxKind.ConstructorDeclaration:
                    return "constructor";
                case SyntaxKind.PropertyDeclaration:
                    return "property";
                case SyntaxKind.IndexerDeclaration:
                    return "indexer";
                case SyntaxKind.EventDeclaration:
                case SyntaxKind.EventFieldDeclaration:
                    return "event";
                case SyntaxKind.FieldDeclaration:
                    return "field";
                case SyntaxKind.NamespaceDeclaration:
                    return "namespace";
                case SyntaxKind.ClassDeclaration:
                    return "class";
                case SyntaxKind.StructDeclaration:
                    return "struct";
                case SyntaxKind.InterfaceDeclaration:
                    return "interface";
                case SyntaxKind.EnumDeclaration:
                    return "enum";
                default:
                    Debug.Assert(false, node.Kind().ToString());
                    return string.Empty;
            }
        }
    }
}
