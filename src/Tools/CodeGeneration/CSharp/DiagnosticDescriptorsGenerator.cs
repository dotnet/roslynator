// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.CSharp;
using Roslynator.Documentation;
using Roslynator.Metadata;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static Roslynator.CSharp.CSharpFactory;

namespace Roslynator.CodeGeneration.CSharp
{
    public static class DiagnosticDescriptorsGenerator
    {
        public static CompilationUnitSyntax Generate(
            IEnumerable<AnalyzerMetadata> analyzers,
            bool obsolete,
            IComparer<string> comparer,
            string @namespace,
            string className,
            string identifiersClassName)
        {
            CompilationUnitSyntax compilationUnit = CompilationUnit(
                UsingDirectives("System", "Microsoft.CodeAnalysis"),
                NamespaceDeclaration(@namespace,
                    ClassDeclaration(
                        Modifiers.Public_Static_Partial(),
                        className,
                        List(
                            CreateMembers(
                                analyzers
                                    .Where(f => f.IsObsolete == obsolete)
                                    .OrderBy(f => f.Id, comparer),
                                identifiersClassName)))));

            compilationUnit = compilationUnit.NormalizeWhitespace();

            return (CompilationUnitSyntax)Rewriter.Instance.Visit(compilationUnit);
        }

        private static IEnumerable<MemberDeclarationSyntax> CreateMembers(IEnumerable<AnalyzerMetadata> analyzers, string identifiersClassName)
        {
            foreach (AnalyzerMetadata analyzer in analyzers)
            {
                string identifier = analyzer.Identifier;
                string title = analyzer.Title;
                string messageFormat = analyzer.MessageFormat;
                bool isEnabledByDefault = analyzer.IsEnabledByDefault;

                yield return CreateMember(
                    analyzer,
                    identifiersClassName);

                if (analyzer.SupportsFadeOutAnalyzer)
                {
                    yield return FieldDeclaration(
                        Modifiers.Public_Static_ReadOnly(),
                        IdentifierName("DiagnosticDescriptor"),
                        identifier + "FadeOut",
                        SimpleMemberInvocationExpression(
                            IdentifierName("DiagnosticDescriptorFactory"),
                            IdentifierName("CreateFadeOut"),
                            ArgumentList(Argument(IdentifierName(identifier))))).AddObsoleteAttributeIf(analyzer.IsObsolete, error: true);
                }

                foreach (AnalyzerMetadata optionAnalyzer in analyzer.OptionAnalyzers)
                {
                    yield return CreateMember(optionAnalyzer, identifiersClassName);
                }
            }
        }

        private static MemberDeclarationSyntax CreateMember(
            AnalyzerMetadata analyzer,
            string identifiersClassName)
        {
            FieldDeclarationSyntax fieldDeclaration = FieldDeclaration(
                (analyzer.IsObsolete) ? Modifiers.Internal_Static_ReadOnly() : Modifiers.Public_Static_ReadOnly(),
                IdentifierName("DiagnosticDescriptor"),
                analyzer.Identifier,
                SimpleMemberInvocationExpression(
                    IdentifierName("Factory"),
                    IdentifierName("Create"),
                    ArgumentList(
                        Argument(
                            NameColon("id"),
                            SimpleMemberAccessExpression(IdentifierName(identifiersClassName), IdentifierName(analyzer.Identifier))),
                        Argument(
                            NameColon("title"),
                            StringLiteralExpression(analyzer.Title)),
                        Argument(
                            NameColon("messageFormat"),
                            StringLiteralExpression(analyzer.MessageFormat)),
                        Argument(
                            NameColon("category"),
                            SimpleMemberAccessExpression(IdentifierName("DiagnosticCategories"), IdentifierName(analyzer.Category))),
                        Argument(
                            NameColon("defaultSeverity"),
                            SimpleMemberAccessExpression(IdentifierName("DiagnosticSeverity"), IdentifierName(analyzer.DefaultSeverity))),
                        Argument(
                            NameColon("isEnabledByDefault"),
                            BooleanLiteralExpression(analyzer.IsEnabledByDefault)),
                        Argument(
                            NameColon("description"),
                            NullLiteralExpression()),
                        Argument(
                            NameColon("helpLinkUri"),
                            SimpleMemberAccessExpression(IdentifierName(identifiersClassName), IdentifierName(analyzer.Identifier))),
                        Argument(
                            NameColon("customTags"),
                            (analyzer.SupportsFadeOut)
                                ? SimpleMemberAccessExpression(IdentifierName("WellKnownDiagnosticTags"), IdentifierName(WellKnownDiagnosticTags.Unnecessary))
                                : ParseExpression("Array.Empty<string>()"))
                        ))).AddObsoleteAttributeIf(analyzer.IsObsolete, error: true);

            if (!analyzer.IsObsolete)
            {
                var settings = new DocumentationCommentGeneratorSettings(
                    summary: new string[] { analyzer.Id },
                    indentation: "        ",
                    singleLineSummary: true);

                fieldDeclaration = fieldDeclaration.WithNewSingleLineDocumentationComment(settings);
            }

            return fieldDeclaration;
        }

        private class Rewriter : CSharpSyntaxRewriter
        {
            public static Rewriter Instance { get; } = new Rewriter();

            public override SyntaxNode VisitFieldDeclaration(FieldDeclarationSyntax node)
            {
                node = (FieldDeclarationSyntax)base.VisitFieldDeclaration(node);

                return node.AppendToTrailingTrivia(NewLine());
            }

            public override SyntaxNode VisitArgument(ArgumentSyntax node)
            {
                if (node.NameColon != null)
                {
                    return node
                        .WithNameColon(node.NameColon.AppendToLeadingTrivia(TriviaList(NewLine(), Whitespace("            "))))
                        .WithExpression(node.Expression.PrependToLeadingTrivia(Whitespace(new string(' ', 18 - node.NameColon.Name.Identifier.ValueText.Length))));
                }

                return node;
            }

            public override SyntaxNode VisitAttribute(AttributeSyntax node)
            {
                return node;
            }
        }
    }
}
