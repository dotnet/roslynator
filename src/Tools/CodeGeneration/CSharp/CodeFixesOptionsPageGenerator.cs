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
    public static class CodeFixesOptionsPageGenerator
    {
        public static CompilationUnitSyntax Generate(IEnumerable<CodeFixMetadata> codeFixes, IEnumerable<CompilerDiagnosticMetadata> compilerDiagnostics, IComparer<string> comparer)
        {
            return CompilationUnit(
                UsingDirectives("Roslynator.CSharp"),
                NamespaceDeclaration(
                    "Roslynator.VisualStudio",
                    ClassDeclaration(
                        Modifiers.Public_Partial(),
                        "CodeFixesOptionsPage",
                        CreateMembers(codeFixes.Where(f => !f.IsObsolete), compilerDiagnostics, comparer).ToSyntaxList())));
        }

        private static IEnumerable<MemberDeclarationSyntax> CreateMembers(IEnumerable<CodeFixMetadata> codeFixes, IEnumerable<CompilerDiagnosticMetadata> compilerDiagnostics, IComparer<string> comparer)
        {
            yield return PropertyDeclaration(
                Modifiers.Protected_Override(),
                PredefinedStringType(),
                Identifier("DisabledByDefault"),
                AccessorList(AutoGetAccessorDeclaration()),
                ParseExpression(
                    "$\"" +
                    string.Join(",", codeFixes
                        .Where(f => !f.IsEnabledByDefault)
                        .OrderBy(f => f.Identifier, comparer)
                        .Select(f => $"{{CodeFixIdentifiers.{f.Identifier}}}")) +
                    "\""));

            yield return PropertyDeclaration(
                Modifiers.Protected_Override(),
                PredefinedStringType(),
                Identifier("MaxId"),
                AccessorList(AutoGetAccessorDeclaration()),
                ParseExpression($"CodeFixIdentifiers.{codeFixes.OrderBy(f => f.Id, comparer).Last().Identifier}"));

            IEnumerable<(CodeFixMetadata codeFix, CompilerDiagnosticMetadata compilerDiagnostic)> items = codeFixes
                .SelectMany(codeFix => codeFix.FixableDiagnosticIds.Select(id => (codeFix, id)))
                .Join(compilerDiagnostics, f => f.id, f => f.Id, (f, compilerDiagnostic) => (f.codeFix, compilerDiagnostic))
                .OrderBy(f => f.compilerDiagnostic.Id)
                .ThenBy(f => f.codeFix.Id);
        }
    }
}
