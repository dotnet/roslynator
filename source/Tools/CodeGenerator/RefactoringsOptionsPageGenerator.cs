// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator;
using Roslynator.CSharp;
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

        public CompilationUnitSyntax Generate(IEnumerable<RefactoringInfo> refactorings)
        {
            return CompilationUnit()
                .WithUsings(
                    UsingDirective(ParseName("System.ComponentModel")),
                    UsingDirective(ParseName("Roslynator.CSharp.Refactorings")),
                    UsingDirective(ParseName("Roslynator.VisualStudio.TypeConverters"))
                    )
                .WithMembers(
                    NamespaceDeclaration(DefaultNamespace)
                        .WithMembers(
                            ClassDeclaration("RefactoringsOptionsPage")
                                .WithModifiers(Modifiers.PublicPartial())
                                .WithMembers(
                                    CreateMembers(refactorings))));
        }

        private IEnumerable<MemberDeclarationSyntax> CreateMembers(IEnumerable<RefactoringInfo> refactorings)
        {
            yield return ConstructorDeclaration("RefactoringsOptionsPage")
                .WithModifiers(Modifiers.Public())
                .WithBody(refactorings.Select(refactoring =>
                    {
                        return ExpressionStatement(
                            SimpleAssignmentExpression(
                                IdentifierName(refactoring.Identifier),
                                (refactoring.IsEnabledByDefault) ? TrueLiteralExpression() : FalseLiteralExpression()));
                    }));

            yield return MethodDeclaration(VoidType(), "Apply")
                .WithModifiers(Modifiers.Public())
                .WithBody(
                    Block(refactorings.Select(refactoring =>
                    {
                        return ExpressionStatement(
                            InvocationExpression(
                                "SetIsEnabled",
                                ArgumentList(
                                    Argument(
                                        SimpleMemberAccessExpression(
                                            IdentifierName("RefactoringIdentifiers"),
                                            IdentifierName(refactoring.Identifier))),
                                    Argument(refactoring.Identifier))));
                    })));

            foreach (RefactoringInfo info in refactorings)
                yield return CreateRefactoringProperty(info);
        }

        private PropertyDeclarationSyntax CreateRefactoringProperty(RefactoringInfo refactoring)
        {
            return PropertyDeclaration(BoolType(), refactoring.Identifier)
                .WithAttributeLists(
                    AttributeList(Attribute("Category", IdentifierName("RefactoringCategory"))),
                    AttributeList(Attribute("DisplayName", StringLiteralExpression(refactoring.Title))),
                    AttributeList(Attribute("Description", StringLiteralExpression(CreateDescription(refactoring)))),
                    AttributeList(Attribute("TypeConverter", TypeOfExpression(IdentifierName("EnabledDisabledConverter")))))
                .WithModifiers(Modifiers.Public())
                .WithAccessorList(
                    AutoGetter(),
                    AutoSetter());
        }

        private static string CreateDescription(RefactoringInfo refactoring)
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
