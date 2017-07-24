// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator;
using Roslynator.CSharp;
using Roslynator.Metadata;
using Roslynator.Utilities;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static Roslynator.CSharp.CSharpFactory;

namespace CodeGenerator
{
    public class CodeFixesOptionsPageGenerator : Generator
    {
        public StringComparer InvariantComparer { get; } = StringComparer.InvariantCulture;

        public override string DefaultNamespace
        {
            get { return "Roslynator.VisualStudio"; }
        }

        public CompilationUnitSyntax Generate(IEnumerable<CodeFixDescriptor> codeFixes)
        {
            return CompilationUnit()
                .WithUsings(List(new UsingDirectiveSyntax[] {
                    UsingDirective(ParseName(MetadataNames.System_Collections_Generic)),
                    UsingDirective(ParseName("Roslynator.CSharp.CodeFixes"))}))
                .WithMembers(
                    NamespaceDeclaration(IdentifierName(DefaultNamespace))
                        .WithMembers(
                            ClassDeclaration("CodeFixesOptionsPage")
                                .WithModifiers(Modifiers.PublicPartial())
                                .WithMembers(
                                    CreateMembers(codeFixes))));
        }

        private IEnumerable<MemberDeclarationSyntax> CreateMembers(IEnumerable<CodeFixDescriptor> codeFixes)
        {
            yield return ConstructorDeclaration("CodeFixesOptionsPage")
                .WithModifiers(Modifiers.Public())
                .WithBody(
                    Block(new StatementSyntax[]
                    {
                        SimpleAssignmentStatement(
                            IdentifierName("DisabledCodeFixes"),
                            ParseExpression(
                                "$\"" +
                                string.Join(",", codeFixes
                                    .Where(f => !f.IsEnabledByDefault)
                                    .OrderBy(f => f.Identifier, InvariantComparer)
                                    .Select(f => $"{{CodeFixIdentifiers.{f.Identifier}}}")) +
                                "\""))
                    }));

            yield return MethodDeclaration(VoidType(), "Fill")
                .WithModifiers(Modifiers.Public())
                .WithParameterList(ParameterList(Parameter(ParseTypeName("ICollection<BaseModel>"), Identifier("codeFixes"))))
                .WithBody(
                    Block((new StatementSyntax[] { ExpressionStatement(ParseExpression("codeFixes.Clear()")) })
                        .Concat(codeFixes
                            .OrderBy(f => f.Id, InvariantComparer)
                            .Select(codeFix =>
                            {
                                return ExpressionStatement(
                                    ParseExpression($"codeFixes.Add(new BaseModel(CodeFixIdentifiers.{codeFix.Identifier}, \"{StringUtility.EscapeQuote(codeFix.Title)} (fixes {string.Join(", ", codeFix.FixableDiagnosticIds)})\", IsEnabled(CodeFixIdentifiers.{codeFix.Identifier})))"));
                            }))));
        }
    }
}
