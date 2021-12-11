// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.CSharp;
using Roslynator.Metadata;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static Roslynator.CSharp.CSharpFactory;

namespace Roslynator.CodeGeneration.CSharp
{
    public static class RefactoringDescriptorsGenerator
    {
        public static CompilationUnitSyntax Generate(IEnumerable<RefactoringMetadata> refactorings, IComparer<string> comparer)
        {
            return CompilationUnit(
                NamespaceDeclaration(
                    "Roslynator.CSharp.Refactorings",
                    ClassDeclaration(
                        Modifiers.Public_Static(),
                        "RefactoringDescriptors",
                        refactorings
                            .OrderBy(f => f.Identifier, comparer)
                            .Select(r => ParseMemberDeclaration($"public static RefactoringDescriptor {r.Identifier} = new RefactoringDescriptor(\"{r.Id}\", \"roslynator.refactoring.{r.OptionKey}.enabled\");"))
                            .ToSyntaxList())));
        }
    }
}
