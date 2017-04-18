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
    public class OptionsPagePropertiesGenerator : Generator
    {
        public StringComparer InvariantComparer { get; } = StringComparer.InvariantCulture;

        public OptionsPagePropertiesGenerator()
        {
            DefaultNamespace = "Roslynator.VisualStudio";
        }

        public CompilationUnitSyntax Generate(IEnumerable<RefactoringDescriptor> refactorings)
        {
            return CompilationUnit()
                .WithUsings(List(new UsingDirectiveSyntax[] {
                    UsingDirective(ParseName(MetadataNames.System_Collections_Generic)),
                    UsingDirective(ParseName(MetadataNames.System_ComponentModel)),
                    UsingDirective(ParseName("Roslynator.CSharp.Refactorings")),
                    UsingDirective(ParseName("Roslynator.VisualStudio.TypeConverters"))}))
                .WithMembers(
                    NamespaceDeclaration(IdentifierName(DefaultNamespace))
                        .WithMembers(
                            ClassDeclaration("RefactoringsOptionsPage")
                                .WithModifiers(Modifiers.PublicPartial())
                                .WithMembers(
                                    CreateMembers(refactorings))));
        }

        private IEnumerable<MemberDeclarationSyntax> CreateMembers(IEnumerable<RefactoringDescriptor> refactorings)
        {
            yield return ConstructorDeclaration("RefactoringsOptionsPage")
                .WithModifiers(Modifiers.Public())
                .WithBody(Block(
                    refactorings
                        .Where(f => ShouldGenerateProperty(f))
                        .Select(f => ExpressionStatement(ParseExpression($"{f.Identifier} = {TrueOrFalseLiteralExpression(f.IsEnabledByDefault)}")))
                        .Concat(new StatementSyntax[]
                        {
                            SimpleAssignmentStatement(
                                IdentifierName("DisabledRefactorings"),
                                ParseExpression(
                                    "$\"" +
                                    string.Join(",", refactorings
                                        .Where(f => !f.IsEnabledByDefault)
                                        .OrderBy(f => f.Identifier, InvariantComparer)
                                        .Select(f => $"{{RefactoringIdentifiers.{f.Identifier}}}")) +
                                    "\""))
                        })));

        yield return MethodDeclaration(VoidType(), "MigrateValuesFromIdentifierProperties")
                .WithModifiers(Modifiers.Public())
                .WithParameterList(ParameterList())
                .WithBody(
                    Block(refactorings
                        .Where(f => ShouldGenerateProperty(f))
                        .OrderBy(f => f.Id, InvariantComparer)
                        .Select(refactoring => ExpressionStatement(ParseExpression($"SetIsEnabled(RefactoringIdentifiers.{refactoring.Identifier}, {refactoring.Identifier})")))));

            yield return MethodDeclaration(VoidType(), "SetRefactoringsDisabledByDefault")
                .WithModifiers(Modifiers.PublicStatic())
                .WithParameterList(ParameterList(Parameter(IdentifierName("RefactoringSettings"), Identifier("settings"))))
                .WithBody(
                    Block(refactorings
                        .Where(f => !f.IsEnabledByDefault)
                        .OrderBy(f => f.Identifier, InvariantComparer)
                        .Select(refactoring =>
                        {
                            return ExpressionStatement(
                                ParseExpression($"settings.DisableRefactoring(RefactoringIdentifiers.{refactoring.Identifier})"));
                        })));

            yield return MethodDeclaration(VoidType(), "Fill")
                .WithModifiers(Modifiers.Public())
                .WithParameterList(ParameterList(Parameter(ParseTypeName("ICollection<RefactoringModel>"), Identifier("refactorings"))))
                .WithBody(
                    Block((new StatementSyntax[] { ExpressionStatement(ParseExpression("refactorings.Clear()")) })
                        .Concat(refactorings
                            .OrderBy(f => f.Id, InvariantComparer)
                            .Select(refactoring =>
                            {
                                return ExpressionStatement(
                                    ParseExpression($"refactorings.Add(new RefactoringModel(RefactoringIdentifiers.{refactoring.Identifier}, \"{StringUtility.EscapeQuote(refactoring.Title)}\", IsEnabled(RefactoringIdentifiers.{refactoring.Identifier})))"));
                            }))));

            foreach (RefactoringDescriptor info in refactorings
                .Where(f => ShouldGenerateProperty(f))
                .OrderBy(f => f.Identifier, InvariantComparer))
            {
                yield return PropertyDeclaration(BoolType(), info.Identifier)
                   .WithAttributeLists(
                       AttributeList(Attribute(IdentifierName("Browsable"), AttributeArgument(FalseLiteralExpression()))),
                       AttributeList(Attribute(IdentifierName("Category"), AttributeArgument(IdentifierName("RefactoringCategory")))),
                       AttributeList(Attribute(IdentifierName("TypeConverter"), AttributeArgument(TypeOfExpression(IdentifierName("EnabledDisabledConverter"))))))
                   .WithModifiers(Modifiers.Public())
                   .WithAccessorList(
                       AccessorList(
                           AutoGetAccessorDeclaration(),
                           AutoSetAccessorDeclaration()));
            }
        }

        private LiteralExpressionSyntax TrueOrFalseLiteralExpression(bool value)
        {
            return (value) ? TrueLiteralExpression() : FalseLiteralExpression();
        }

        private static bool ShouldGenerateProperty(RefactoringDescriptor refactoring)
        {
            return int.Parse(refactoring.Id.Substring(2)) <= 177;
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
