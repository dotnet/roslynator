// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.CSharp;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static Roslynator.CSharp.CSharpFactory;

namespace Roslynator.CodeGeneration.CSharp
{
    public static class CodeFixesOptionsPageGenerator
    {
        public static CompilationUnitSyntax Generate()
        {
            return CompilationUnit(
                UsingDirectives("Roslynator.CSharp"),
                NamespaceDeclaration(
                    "Roslynator.VisualStudio",
                    ClassDeclaration(
                        Modifiers.Public_Partial(),
                        "CodeFixesOptionsPage")));
        }
    }
}
