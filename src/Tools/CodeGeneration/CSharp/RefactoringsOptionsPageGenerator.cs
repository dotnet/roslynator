// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.CSharp;
using Roslynator.Metadata;
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
                PredefinedStringType(),
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
                PredefinedStringType(),
                Identifier("MaxId"),
                AccessorList(AutoGetAccessorDeclaration()),
                ParseExpression($"RefactoringIdentifiers.{refactorings.OrderBy(f => f.Id, comparer).Last().Identifier}"));

            yield return MethodDeclaration(
                Modifiers.InternalStatic(),
                VoidType(),
                Identifier("SetRefactoringsDisabledByDefault"),
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
                Identifier("Fill"),
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
        }
    }
}
