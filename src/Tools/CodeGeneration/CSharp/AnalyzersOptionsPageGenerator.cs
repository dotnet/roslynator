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
    public static class AnalyzersOptionsPageGenerator
    {
        public static CompilationUnitSyntax Generate(IEnumerable<AnalyzerDescriptor> analyzers, IComparer<string> comparer)
        {
            return CompilationUnit(
                UsingDirectives(
                    "System.Collections.Generic",
                    "Roslynator.CSharp"),
                NamespaceDeclaration(
                    "Roslynator.VisualStudio",
                    ClassDeclaration(
                        Modifiers.Public_Partial(),
                        "AnalyzersOptionsPage",
                        CreateMembers(analyzers, comparer).ToSyntaxList())));
        }

        private static IEnumerable<MemberDeclarationSyntax> CreateMembers(IEnumerable<AnalyzerDescriptor> analyzers, IComparer<string> comparer)
        {
            yield return PropertyDeclaration(
                Modifiers.Protected_Override(),
                PredefinedStringType(),
                Identifier("MaxId"),
                AccessorList(AutoGetAccessorDeclaration()),
                ParseExpression($"DiagnosticIdentifiers.{analyzers.OrderBy(f => f.Id, comparer).Last().Identifier}"));

            yield return MethodDeclaration(
                Modifiers.Protected_Override(),
                VoidType(),
                Identifier("Fill"),
                ParameterList(Parameter(ParseTypeName("ICollection<BaseModel>"), Identifier("analyzers"))),
                Block(
                    SingletonList(ExpressionStatement(ParseExpression("analyzers.Clear()")))
                        .AddRange(analyzers
                            .OrderBy(f => f.Id, comparer)
                            .Select(analyzer =>
                            {
                                return ExpressionStatement(
                                    ParseExpression($"analyzers.Add(new BaseModel(DiagnosticIdentifiers.{analyzer.Identifier}, \"{StringUtility.EscapeQuote(analyzer.Title)}\", !IsEnabled(DiagnosticIdentifiers.{analyzer.Identifier})))"));
                            }))));
        }
    }
}
