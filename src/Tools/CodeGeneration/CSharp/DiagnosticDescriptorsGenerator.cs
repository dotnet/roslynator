// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.CSharp;
using Roslynator.CSharp.Documentation;
using Roslynator.Metadata;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static Roslynator.CSharp.CSharpFactory;

namespace Roslynator.CodeGeneration.CSharp
{
    public static class DiagnosticDescriptorsGenerator
    {
        public static CompilationUnitSyntax Generate(IEnumerable<AnalyzerDescriptor> analyzers, bool obsolete, IComparer<string> comparer)
        {
            CompilationUnitSyntax compilationUnit = CompilationUnit(
                UsingDirectives("System", "Microsoft.CodeAnalysis"),
                NamespaceDeclaration("Roslynator.CSharp",
                    ClassDeclaration(
                        Modifiers.PublicStaticPartial(),
                        "DiagnosticDescriptors",
                        List(
                            CreateMembers(
                                analyzers
                                    .Where(f => f.IsObsolete == obsolete)
                                    .OrderBy(f => f.Id, comparer))))));

            compilationUnit = compilationUnit.NormalizeWhitespace();

            return (CompilationUnitSyntax)Rewriter.Instance.Visit(compilationUnit);
        }

        private static IEnumerable<MemberDeclarationSyntax> CreateMembers(IEnumerable<AnalyzerDescriptor> analyzers)
        {
            foreach (AnalyzerDescriptor analyzer in analyzers)
            {
                FieldDeclarationSyntax fieldDeclaration = FieldDeclaration(
                    Modifiers.PublicStaticReadOnly(),
                    IdentifierName("DiagnosticDescriptor"),
                    analyzer.Identifier,
                    ObjectCreationExpression(
                        IdentifierName("DiagnosticDescriptor"),
                        ArgumentList(
                            Argument(
                                NameColon("id"),
                                SimpleMemberAccessExpression(IdentifierName("DiagnosticIdentifiers"), IdentifierName(analyzer.Identifier))),
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
                                ParseExpression($"$\"{{HelpLinkUriRoot}}{{DiagnosticIdentifiers.{analyzer.Identifier}}}\"")),
                            Argument(
                                NameColon("customTags"),
                                (analyzer.SupportsFadeOut)
                                    ? SimpleMemberAccessExpression(IdentifierName("WellKnownDiagnosticTags"), IdentifierName("Unnecessary"))
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

                yield return fieldDeclaration;

                if (analyzer.SupportsFadeOutAnalyzer)
                {
                    yield return FieldDeclaration(
                        Modifiers.PublicStaticReadOnly(),
                        IdentifierName("DiagnosticDescriptor"),
                        analyzer.Identifier + "FadeOut",
                        SimpleMemberInvocationExpression(
                            IdentifierName(analyzer.Identifier),
                            IdentifierName("CreateFadeOut"))).AddObsoleteAttributeIf(analyzer.IsObsolete, error: true);
                }
            }
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
                return node
                    .WithNameColon(node.NameColon.AppendToLeadingTrivia(TriviaList(NewLine(), Whitespace("            "))))
                    .WithExpression(node.Expression.PrependToLeadingTrivia(Whitespace(new string(' ', 18 - node.NameColon.Name.Identifier.ValueText.Length))));
            }

            public override SyntaxNode VisitAttribute(AttributeSyntax node)
            {
                return node;
            }
        }
    }
}
