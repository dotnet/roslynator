// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.CSharp;
using Roslynator.Metadata;
using Roslynator.Utilities;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static Roslynator.CSharp.CSharpFactory;

namespace Roslynator.CodeGeneration.CSharp
{
    public static class RefactoringsOptionsPageGenerator
    {
        public static CompilationUnitSyntax Generate(IEnumerable<RefactoringDescriptor> refactorings, IComparer<string> comparer)
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
                        CreateMembers(refactorings, comparer).ToSyntaxList())));
        }

        private static IEnumerable<MemberDeclarationSyntax> CreateMembers(IEnumerable<RefactoringDescriptor> refactorings, IComparer<string> comparer)
        {
            yield return PropertyDeclaration(
                Modifiers.ProtectedOverride(),
                StringPredefinedType(),
                Identifier("DisabledByDefault"),
                AccessorList(AutoGetAccessorDeclaration()),
                ParseExpression(
                    "$\"" +
                    string.Join(",", refactorings
                        .Where(f => !f.IsEnabledByDefault)
                        .OrderBy(f => f.Identifier, comparer)
                        .Select(f => $"{{RefactoringIdentifiers.{f.Identifier}}}")) +
                    "\""));

            yield return PropertyDeclaration(
                Modifiers.ProtectedOverride(),
                StringPredefinedType(),
                Identifier("MaxId"),
                AccessorList(AutoGetAccessorDeclaration()),
                ParseExpression($"RefactoringIdentifiers.{refactorings.OrderBy(f => f.Id, comparer).Last().Identifier}"));

            yield return ConstructorDeclaration(
                Modifiers.Public(),
                "RefactoringsOptionsPage",
                ParameterList(),
                Block(
                    refactorings
                        .Where(ShouldGenerateProperty)
                        .Select(f => ExpressionStatement(ParseExpression($"{f.Identifier} = {TrueOrFalseLiteralExpression(f.IsEnabledByDefault)}")))
                        .ToSyntaxList()));

            yield return MethodDeclaration(
                Modifiers.Public(),
                VoidType(),
                "MigrateValuesFromIdentifierProperties",
                ParameterList(),
                Block(refactorings
                    .Where(ShouldGenerateProperty)
                    .OrderBy(f => f.Id, comparer)
                    .Select(refactoring => ExpressionStatement(ParseExpression($"SetIsEnabled(RefactoringIdentifiers.{refactoring.Identifier}, {refactoring.Identifier})")))));

            yield return MethodDeclaration(
                Modifiers.InternalStatic(),
                VoidType(),
                "SetRefactoringsDisabledByDefault",
                ParameterList(Parameter(IdentifierName("RefactoringSettings"), Identifier("settings"))),
                Block(refactorings
                    .Where(f => !f.IsEnabledByDefault)
                    .OrderBy(f => f.Identifier, comparer)
                    .Select(refactoring =>
                    {
                        return ExpressionStatement(
                            ParseExpression($"settings.DisableRefactoring(RefactoringIdentifiers.{refactoring.Identifier})"));
                    })));

            yield return MethodDeclaration(
                Modifiers.ProtectedOverride(),
                VoidType(),
                "Fill",
                ParameterList(Parameter(ParseTypeName("ICollection<BaseModel>"), Identifier("refactorings"))),
                Block(
                    SingletonList(ExpressionStatement(ParseExpression("refactorings.Clear()")))
                        .AddRange(refactorings
                            .OrderBy(f => f.Id, comparer)
                            .Select(refactoring =>
                            {
                                return ExpressionStatement(
                                    ParseExpression($"refactorings.Add(new BaseModel(RefactoringIdentifiers.{refactoring.Identifier}, \"{StringUtility.EscapeQuote(refactoring.Title)}\", IsEnabled(RefactoringIdentifiers.{refactoring.Identifier})))"));
                            }))));

            foreach (RefactoringDescriptor info in refactorings
                .Where(ShouldGenerateProperty)
                .OrderBy(f => f.Identifier, comparer))
            {
                yield return PropertyDeclaration(
                    List(new AttributeListSyntax[]
                    {
                       AttributeList(Attribute(IdentifierName("Browsable"), AttributeArgument(FalseLiteralExpression()))),
                       AttributeList(Attribute(IdentifierName("Category"), AttributeArgument(IdentifierName("RefactoringCategory")))),
                       AttributeList(Attribute(IdentifierName("TypeConverter"), AttributeArgument(TypeOfExpression(IdentifierName("EnabledDisabledConverter")))))
                    }),
                    Modifiers.Public(),
                    BoolPredefinedType(),
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
