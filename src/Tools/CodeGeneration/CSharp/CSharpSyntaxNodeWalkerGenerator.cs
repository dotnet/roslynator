// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.CSharp;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static Roslynator.CSharp.CSharpFactory;

namespace Roslynator.CodeGeneration.CSharp
{
    public static class CSharpSyntaxNodeWalkerGenerator
    {
        public static CompilationUnitSyntax Generate()
        {
            var generator = new CSharpSyntaxWalkerGenerator(
                depth: SyntaxWalkerDepth.Node,
                useCustomVisitMethod: true,
                eliminateDefaultVisit: true);

            return CompilationUnit(
                UsingDirectives(
                    "System",
                    "System.Diagnostics",
                    "Microsoft.CodeAnalysis",
                    "Microsoft.CodeAnalysis.CSharp",
                    "Microsoft.CodeAnalysis.CSharp.Syntax"),
                NamespaceDeclaration("Roslynator.CSharp.SyntaxWalkers",
                    ClassDeclaration(
                        default(SyntaxList<AttributeListSyntax>),
                        Modifiers.Internal_Abstract(),
                        Identifier("CSharpSyntaxNodeWalker"),
                        default(TypeParameterListSyntax),
                        BaseList(SimpleBaseType(IdentifierName("CSharpSyntaxWalker"))),
                        default(SyntaxList<TypeParameterConstraintClauseSyntax>),
                        generator.CreateMemberDeclarations().ToSyntaxList())));
        }
    }
}
