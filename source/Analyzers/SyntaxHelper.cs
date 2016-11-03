// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Diagnostics;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace Roslynator.CSharp
{
    internal static class SyntaxHelper
    {
        public static string GetNodeTitle(SyntaxNode node)
        {
            switch (node.Kind())
            {
                case SyntaxKind.IfStatement:
                    return "'if' statement";
                case SyntaxKind.ElseClause:
                    return "'else' clause";
                case SyntaxKind.DoStatement:
                    return "'do' statement";
                case SyntaxKind.ForEachStatement:
                    return "'foreach' statement";
                case SyntaxKind.ForStatement:
                    return "'for' statement";
                case SyntaxKind.UsingStatement:
                    return "'using' statement";
                case SyntaxKind.WhileStatement:
                    return "'while' statement";
                case SyntaxKind.LockStatement:
                    return "'lock' statement";
                case SyntaxKind.FixedStatement:
                    return "'fixed' statement";
            }

            Debug.Assert(false, node.Kind().ToString());
            return "";
        }
    }
}
