// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.CSharp;
using Roslynator.CSharp.Refactorings;
using Roslynator.Metadata;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static Roslynator.CSharp.CSharpFactory;

namespace Roslynator.CodeGeneration.Refactorings
{
    public static class RefactoringIdentifiersGenerator
    {
        private static StringComparer InvariantComparer { get; } = StringComparer.InvariantCulture;

        public static CompilationUnitSyntax Generate(IEnumerable<RefactoringDescriptor> refactorings)
        {
            return CompilationUnit(
                UsingDirectives(),
                NamespaceDeclaration("Roslynator.CSharp.Refactorings",
                    ClassDeclaration(
                        Modifiers.PublicStaticPartial(),
                        "RefactoringIdentifiers",
                        refactorings
                            .OrderBy(f => f.Identifier, InvariantComparer)
                            .Select(f =>
                            {
                                return FieldDeclaration(
                                    Modifiers.PublicConst(),
                                    StringType(),
                                    f.Identifier,
                                    AddExpression(IdentifierName("Prefix"), StringLiteralExpression(f.Id.Substring(RefactoringIdentifiers.Prefix.Length))));
                            })
                            .ToSyntaxList<MemberDeclarationSyntax>())));
        }
    }
}
