// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Roslynator.CSharp.Analysis
{
    internal static class SplitVariableDeclarationAnalysis
    {
        public static bool IsFixable(VariableDeclarationSyntax variableDeclaration)
        {
            return variableDeclaration.IsParentKind(SyntaxKind.LocalDeclarationStatement, SyntaxKind.FieldDeclaration, SyntaxKind.EventFieldDeclaration)
                && variableDeclaration.Variables.Count > 1;
        }

        public static string GetTitle(VariableDeclarationSyntax variableDeclaration)
        {
            return $"Split {GetName()} declaration";

            string GetName()
            {
                switch (variableDeclaration.Parent?.Kind())
                {
                    case SyntaxKind.LocalDeclarationStatement:
                        return "local";
                    case SyntaxKind.FieldDeclaration:
                        return "field";
                    case SyntaxKind.EventFieldDeclaration:
                        return "event";
                }

                return "variable";
            }
        }
    }
}
