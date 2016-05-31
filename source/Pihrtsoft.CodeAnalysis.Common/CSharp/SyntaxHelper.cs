// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Diagnostics;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Pihrtsoft.CodeAnalysis.Text;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Pihrtsoft.CodeAnalysis.CSharp
{
    public static class SyntaxHelper
    {
        private static readonly SyntaxTrivia _newLine = CreateNewLine();

        private static readonly SyntaxTrivia _emptyTrivia = SyntaxTrivia(SyntaxKind.WhitespaceTrivia, string.Empty);

        public static SyntaxTrivia EmptyTrivia => _emptyTrivia;

        public static SyntaxTrivia NewLine => _newLine;

        public static SyntaxTrivia DefaultIndent => Whitespace(TextUtility.DefaultIndent);

        private static SyntaxTrivia CreateNewLine()
        {
            switch (Environment.NewLine)
            {
                case "\r":
                    return CarriageReturn;
                case "\n":
                    return LineFeed;
                default:
                    return CarriageReturnLineFeed;
            }
        }

        public static InvocationExpressionSyntax NameOf(string identifier)
        {
            return InvocationExpression(
                IdentifierName("nameof"),
                ArgumentList(
                    SingletonSeparatedList(
                        Argument(
                            IdentifierName(identifier)))));
        }

        public static string GetSyntaxNodeName(SyntaxNode node)
        {
            if (node == null)
                throw new ArgumentNullException(nameof(node));

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
                default:
                    Debug.Assert(false, node.Kind().ToString());
                    return string.Empty;
            }
        }
    }
}
