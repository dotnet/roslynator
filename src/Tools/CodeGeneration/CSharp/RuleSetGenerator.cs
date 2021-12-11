// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.CSharp;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static Roslynator.CSharp.CSharpFactory;

namespace Roslynator.CodeGeneration.CSharp
{
    public static class RuleSetGenerator
    {
        public static CompilationUnitSyntax Generate(string xml)
        {
            CompilationUnitSyntax compilationUnit = CompilationUnit(
                NamespaceDeclaration(
                    "Roslynator.VisualStudio",
                    ClassDeclaration(
                        Modifiers.Internal_Static_Partial(),
                        "DefaultRuleSet",
                        SingletonList<MemberDeclarationSyntax>(
                            FieldDeclaration(
                                Modifiers.Private_Const(),
                                PredefinedStringType(),
                                "RuleSetXml",
                                ParseExpression("@\"" + xml.Replace("\"", "\"\"") + "\""))))));

            return compilationUnit.NormalizeWhitespace();
        }
    }
}
