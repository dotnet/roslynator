// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.CSharp;
using Roslynator.Metadata;
using Roslynator.Utilities;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static Roslynator.CSharp.CSharpFactory;

namespace Roslynator.CodeGeneration.Refactorings
{
    public static class RefactoringsOptionsPageGenerator
    {
        private static StringComparer InvariantComparer { get; } = StringComparer.InvariantCulture;

        public static CompilationUnitSyntax Generate(IEnumerable<RefactoringDescriptor> refactorings)
        {
            return CompilationUnit(
                UsingDirectives(
                    MetadataNames.System_Collections_Generic,
                    MetadataNames.System_ComponentModel,
                    "Roslynator.CSharp.Refactorings",
                    "Roslynator.VisualStudio.TypeConverters"),
                NamespaceDeclaration(
                    "Roslynator.VisualStudio",
                    ClassDeclaration(
                        Modifiers.PublicPartial(),
                        "RefactoringsOptionsPage",
                        CreateMembers(refactorings).ToSyntaxList())));
        }

        private static IEnumerable<MemberDeclarationSyntax> CreateMembers(IEnumerable<RefactoringDescriptor> refactorings)
        {
            yield return ConstructorDeclaration(
                Modifiers.Public(),
                "RefactoringsOptionsPage",
                ParameterList(),
                Block(
                    refactorings
                        .Where(f => ShouldGenerateProperty(f))
                        .Select(f => ExpressionStatement(ParseExpression($"{f.Identifier} = {TrueOrFalseLiteralExpression(f.IsEnabledByDefault)}")))
                        .ToSyntaxList()
                        .Add(
                            SimpleAssignmentStatement(
                                IdentifierName("DisabledRefactorings"),
                                ParseExpression(
                                    "$\"" +
                                    string.Join(",", refactorings
                                        .Where(f => !f.IsEnabledByDefault)
                                        .OrderBy(f => f.Identifier, InvariantComparer)
                                        .Select(f => $"{{RefactoringIdentifiers.{f.Identifier}}}")) +
                                    "\""))
                        )));

            yield return MethodDeclaration(
                Modifiers.Public(),
                VoidType(),
                "MigrateValuesFromIdentifierProperties",
                ParameterList(),
                Block(refactorings
                    .Where(f => ShouldGenerateProperty(f))
                    .OrderBy(f => f.Id, InvariantComparer)
                    .Select(refactoring => ExpressionStatement(ParseExpression($"SetIsEnabled(RefactoringIdentifiers.{refactoring.Identifier}, {refactoring.Identifier})")))));

            yield return MethodDeclaration(
                Modifiers.PublicStatic(),
                VoidType(),
                "SetRefactoringsDisabledByDefault",
                ParameterList(Parameter(IdentifierName("RefactoringSettings"), Identifier("settings"))),
                Block(refactorings
                    .Where(f => !f.IsEnabledByDefault)
                    .OrderBy(f => f.Identifier, InvariantComparer)
                    .Select(refactoring =>
                    {
                        return ExpressionStatement(
                            ParseExpression($"settings.DisableRefactoring(RefactoringIdentifiers.{refactoring.Identifier})"));
                    })));

            yield return MethodDeclaration(
                Modifiers.Public(),
                VoidType(),
                "Fill",
                ParameterList(Parameter(ParseTypeName("ICollection<BaseModel>"), Identifier("refactorings"))),
                Block(
                    SingletonList(ExpressionStatement(ParseExpression("refactorings.Clear()")))
                        .AddRange(refactorings
                            .OrderBy(f => f.Id, InvariantComparer)
                            .Select(refactoring =>
                            {
                                return ExpressionStatement(
                                    ParseExpression($"refactorings.Add(new BaseModel(RefactoringIdentifiers.{refactoring.Identifier}, \"{StringUtility.EscapeQuote(refactoring.Title)}\", IsEnabled(RefactoringIdentifiers.{refactoring.Identifier})))"));
                            }))));

            foreach (RefactoringDescriptor info in refactorings
                .Where(f => ShouldGenerateProperty(f))
                .OrderBy(f => f.Identifier, InvariantComparer))
            {
                yield return PropertyDeclaration(
                    AttributeLists(
                       AttributeList(Attribute(IdentifierName("Browsable"), AttributeArgument(FalseLiteralExpression()))),
                       AttributeList(Attribute(IdentifierName("Category"), AttributeArgument(IdentifierName("RefactoringCategory")))),
                       AttributeList(Attribute(IdentifierName("TypeConverter"), AttributeArgument(TypeOfExpression(IdentifierName("EnabledDisabledConverter")))))
                    ),
                    Modifiers.Public(),
                    BoolType(),
                    default(ExplicitInterfaceSpecifierSyntax),
                    Identifier(info.Identifier),
                    AccessorList(
                        AutoGetAccessorDeclaration(),
                        AutoSetAccessorDeclaration()));
            }
        }

        private static LiteralExpressionSyntax TrueOrFalseLiteralExpression(bool value)
        {
            return (value) ? TrueLiteralExpression() : FalseLiteralExpression();
        }

        private static bool ShouldGenerateProperty(RefactoringDescriptor refactoring)
        {
            return int.Parse(refactoring.Id.Substring(2)) <= 177;
        }
    }
}
