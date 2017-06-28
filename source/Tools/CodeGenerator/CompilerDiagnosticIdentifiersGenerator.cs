// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.CSharp;
using Roslynator.Metadata;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static Roslynator.CSharp.CSharpFactory;

namespace CodeGenerator
{
    public class CompilerDiagnosticIdentifiersGenerator : Generator
    {
        public StringComparer InvariantComparer { get; } = StringComparer.InvariantCulture;

        public override string DefaultNamespace
        {
            get { return "Roslynator.CSharp"; }
        }

        public CompilationUnitSyntax Generate(IEnumerable<CompilerDiagnosticDescriptor> descriptors)
        {
            return CompilationUnit()
                .WithUsings(List(new UsingDirectiveSyntax[] { }))
                .WithMembers(
                    NamespaceDeclaration(IdentifierName(DefaultNamespace))
                        .WithMembers(
                            ClassDeclaration("CompilerDiagnosticIdentifiers")
                                .WithModifiers(Modifiers.InternalStatic())
                                .WithMembers(
                                    CreateMembers(descriptors.OrderBy(f => f.Id, InvariantComparer)))));
        }

        private static IEnumerable<MemberDeclarationSyntax> CreateMembers(IEnumerable<CompilerDiagnosticDescriptor> descriptors)
        {
            foreach (CompilerDiagnosticDescriptor descriptor in descriptors)
                yield return FieldDeclaration(Modifiers.PublicConst(), StringType(), descriptor.Identifier, StringLiteralExpression(descriptor.Id));
        }
    }
}
