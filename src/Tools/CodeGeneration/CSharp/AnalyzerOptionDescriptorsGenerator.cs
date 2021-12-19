// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.CSharp;
using Roslynator.Metadata;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static Roslynator.CSharp.CSharpFactory;

namespace Roslynator.CodeGeneration.CSharp
{
    public static class AnalyzerOptionDescriptorsGenerator
    {
        public static CompilationUnitSyntax Generate(
            IEnumerable<AnalyzerMetadata> analyzers,
            bool obsolete,
            IComparer<string> comparer,
            string @namespace,
            string className)
        {
            return CompilationUnit(
                UsingDirectives(),
                NamespaceDeclaration(
                    @namespace,
                    ClassDeclaration(
                        Modifiers.Public_Static_Partial(),
                        className,
                        analyzers
                            .Where(f => !f.IsObsolete)
                            .SelectMany(f => f.Options)
                            .Where(f => f.IsObsolete == obsolete)
                            .OrderBy(f => f.Id, comparer)
                            .Select(f => CreateMember(f, analyzers.Single(a => a.Id == f.ParentId)))
                            .ToSyntaxList<MemberDeclarationSyntax>())));
        }

        private static FieldDeclarationSyntax CreateMember(AnalyzerOptionMetadata analyzer, AnalyzerMetadata parent)
        {
            string optionKey = analyzer.OptionKey;

            if (!optionKey.StartsWith("roslynator.", System.StringComparison.Ordinal))
                optionKey = $"roslynator.{analyzer.ParentId}.{optionKey}";

            FieldDeclarationSyntax fieldDeclaration = FieldDeclaration(
                Modifiers.Internal_Static_ReadOnly(),
                IdentifierName(nameof(AnalyzerOptionDescriptor)),
                analyzer.Identifier,
                ObjectCreationExpression(
                    IdentifierName(nameof(AnalyzerOptionDescriptor)),
                    ArgumentList(
                        Argument(StringLiteralExpression(optionKey)),
                        Argument(SimpleMemberAccessExpression(IdentifierName("DiagnosticRules"), IdentifierName(parent.Identifier))))));

            if (analyzer.IsObsolete)
                fieldDeclaration = fieldDeclaration.AddObsoleteAttributeIf(analyzer.IsObsolete, error: true);

            return fieldDeclaration;
        }
    }
}
