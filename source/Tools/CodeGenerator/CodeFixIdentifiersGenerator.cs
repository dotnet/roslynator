// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.CSharp;
using Roslynator.CSharp.CodeFixes;
using Roslynator.Metadata;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static Roslynator.CSharp.CSharpFactory;

namespace CodeGenerator
{
    public class CodeFixIdentifiersGenerator : Generator
    {
        public StringComparer InvariantComparer { get; } = StringComparer.InvariantCulture;

        public override string DefaultNamespace
        {
            get { return "Roslynator.CSharp.CodeFixes"; }
        }

        public CompilationUnitSyntax Generate(IEnumerable<CodeFixDescriptor> codeFixes)
        {
            return CompilationUnit()
                .WithUsings(List(new UsingDirectiveSyntax[] { }))
                .WithMembers(
                    NamespaceDeclaration(IdentifierName(DefaultNamespace))
                        .WithMembers(
                            ClassDeclaration("CodeFixIdentifiers")
                                .WithModifiers(Modifiers.PublicStaticPartial())
                                .WithMembers(
                                    CreateMembers(codeFixes.OrderBy(f => f.Id, InvariantComparer)))));
        }

        private static IEnumerable<MemberDeclarationSyntax> CreateMembers(IEnumerable<CodeFixDescriptor> codeFixes)
        {
            foreach (CodeFixDescriptor codeFix in codeFixes)
                yield return FieldDeclaration(Modifiers.PublicConst(), StringType(), codeFix.Identifier, AddExpression(IdentifierName("Prefix"), StringLiteralExpression(codeFix.Id.Substring(CodeFixIdentifiers.Prefix.Length))));
        }
    }
}
