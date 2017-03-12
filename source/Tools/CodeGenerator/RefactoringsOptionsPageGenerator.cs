// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator;
using Roslynator.CSharp;
using Roslynator.CSharp.Extensions;
using Roslynator.Metadata;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static Roslynator.CSharp.CSharpFactory;

namespace CodeGenerator
{
    public class OptionsPagePropertiesGenerator : Generator
    {
        public OptionsPagePropertiesGenerator()
        {
            DefaultNamespace = "Roslynator.VisualStudio";
        }

        public CompilationUnitSyntax Generate(IEnumerable<RefactoringDescriptor> refactorings)
        {
            return CompilationUnit()
                .WithUsings(List(new UsingDirectiveSyntax[] {
                    UsingDirective(ParseName(MetadataNames.System_ComponentModel)),
                    UsingDirective(ParseName("Roslynator.CSharp.Refactorings")),
                    UsingDirective(ParseName("Roslynator.VisualStudio.TypeConverters"))}))
                .WithMembers(
                    NamespaceDeclaration(DefaultNamespace)
                        .WithMembers(
                            ClassDeclaration("RefactoringsOptionsPage")
                                .WithModifiers(ModifierFactory.PublicPartial())
                                .WithMembers(
                                    CreateMembers(refactorings))));
        }

        private IEnumerable<MemberDeclarationSyntax> CreateMembers(IEnumerable<RefactoringDescriptor> refactorings)
        {
            yield return ConstructorDeclaration("RefactoringsOptionsPage")
                .WithModifiers(ModifierFactory.Public())
                .WithBody(Block(refactorings.Select(refactoring =>
                    {
                        return SimpleAssignmentStatement(
                                IdentifierName(refactoring.Identifier),
                                (refactoring.IsEnabledByDefault) ? TrueLiteralExpression() : FalseLiteralExpression());
                    })));

            yield return MethodDeclaration(VoidType(), "Apply")
                .WithModifiers(ModifierFactory.Public())
                .WithBody(
                    Block(refactorings.Select(refactoring =>
                    {
                        return ExpressionStatement(
                            InvocationExpression(
                                IdentifierName("SetIsEnabled"),
                                ArgumentList(
                                    Argument(
                                        SimpleMemberAccessExpression(
                                            IdentifierName("RefactoringIdentifiers"),
                                            IdentifierName(refactoring.Identifier))),
                                    Argument(IdentifierName(refactoring.Identifier)))));
                    })));

            foreach (RefactoringDescriptor info in refactorings)
                yield return CreateRefactoringProperty(info);
        }

        private PropertyDeclarationSyntax CreateRefactoringProperty(RefactoringDescriptor refactoring)
        {
            return PropertyDeclaration(BoolType(), refactoring.Identifier)
                .WithAttributeLists(
                    SingletonAttributeList(Attribute(IdentifierName("Category"), IdentifierName("RefactoringCategory"))),
                    SingletonAttributeList(Attribute(IdentifierName("DisplayName"), StringLiteralExpression(refactoring.Title))),
                    SingletonAttributeList(Attribute(IdentifierName("Description"), StringLiteralExpression(CreateDescription(refactoring)))),
                    SingletonAttributeList(Attribute(IdentifierName("TypeConverter"), TypeOfExpression(IdentifierName("EnabledDisabledConverter")))))
                .WithModifiers(ModifierFactory.Public())
                .WithAccessorList(
                    AccessorList(
                        AutoGetAccessorDeclaration(),
                        AutoSetAccessorDeclaration()));
        }

        private static string CreateDescription(RefactoringDescriptor refactoring)
        {
            string s = "";

            if (refactoring.Syntaxes.Count > 0)
                s = "Syntax: " + string.Join(", ", refactoring.Syntaxes.Select(f => f.Name));

            if (!string.IsNullOrEmpty(refactoring.Scope))
            {
                if (!string.IsNullOrEmpty(s))
                    s += "\r\n";

                s += "Scope: " + refactoring.Scope;
            }

            return s;
        }
    }
}
