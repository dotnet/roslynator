// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

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
    public class DiagnosticRulesGenerator
    {
        public CompilationUnitSyntax Generate(
            IEnumerable<AnalyzerMetadata> analyzers,
            bool obsolete,
            IComparer<string> comparer,
            string @namespace,
            string className,
            string identifiersClassName,
            string categoryName)
        {
            analyzers = analyzers
                .Where(f => f.IsObsolete == obsolete)
                .OrderBy(f => f.Id, comparer);

            ClassDeclarationSyntax classDeclaration = CreateClassDeclaration(
                analyzers,
                className,
                identifiersClassName,
                categoryName);

            CompilationUnitSyntax compilationUnit = CompilationUnit(
                UsingDirectives("System", "Microsoft.CodeAnalysis"),
                NamespaceDeclaration(
                    @namespace,
                    classDeclaration));

            compilationUnit = compilationUnit.NormalizeWhitespace();

            var rewriter = new WrapRewriter(WrapRewriterOptions.WrapArguments);

            return (CompilationUnitSyntax)rewriter.Visit(compilationUnit);
        }

        private IEnumerable<MemberDeclarationSyntax> CreateMembers(
            IEnumerable<AnalyzerMetadata> analyzers,
            string identifiersClassName,
            string categoryName,
            bool useParentProperties = false)
        {
            foreach (AnalyzerMetadata analyzer in analyzers)
            {
                if (analyzer.Id == null)
                    continue;

                string identifier = analyzer.Identifier;

                yield return CreateMember(
                    analyzer,
                    identifiersClassName,
                    categoryName,
                    useParentProperties);

                if (analyzer.SupportsFadeOutAnalyzer)
                {
                    yield return FieldDeclaration(
                        Modifiers.Public_Static_ReadOnly(),
                        IdentifierName("DiagnosticDescriptor"),
                        identifier + "FadeOut",
                        SimpleMemberInvocationExpression(
                            IdentifierName("DiagnosticDescriptorFactory"),
                            IdentifierName("CreateFadeOut"),
                            ArgumentList(Argument(IdentifierName(identifier)))))
                        .AddObsoleteAttributeIf(analyzer.IsObsolete, error: true);
                }
            }
        }

        protected virtual ClassDeclarationSyntax CreateClassDeclaration(
            IEnumerable<AnalyzerMetadata> analyzers,
            string className,
            string identifiersClassName,
            string categoryName,
            bool useParentProperties = false)
        {
            return ClassDeclaration(
                Modifiers.Public_Static_Partial(),
                className,
                List(
                    CreateMembers(
                        analyzers,
                        identifiersClassName,
                        categoryName,
                        useParentProperties)));
        }

        private MemberDeclarationSyntax CreateMember(
            AnalyzerMetadata analyzer,
            string identifiersClassName,
            string categoryName,
            bool useParentProperties = false)
        {
            AnalyzerMetadata parent = (useParentProperties) ? analyzer.Parent : null;

            ExpressionSyntax idExpression = SimpleMemberAccessExpression(IdentifierName(identifiersClassName), IdentifierName(parent?.Identifier ?? analyzer.Identifier));

            idExpression = ModifyIdExpression(idExpression);

            FieldDeclarationSyntax fieldDeclaration = FieldDeclaration(
                (analyzer.IsObsolete) ? Modifiers.Internal_Static_ReadOnly() : Modifiers.Public_Static_ReadOnly(),
                IdentifierName("DiagnosticDescriptor"),
                analyzer.Identifier,
                SimpleMemberInvocationExpression(
                    IdentifierName("DiagnosticDescriptorFactory"),
                    IdentifierName("Create"),
                    ArgumentList(
                        Argument(
                            NameColon("id"),
                            idExpression),
                        Argument(
                            NameColon("title"),
                            StringLiteralExpression(parent?.Title ?? analyzer.Title)),
                        Argument(
                            NameColon("messageFormat"),
                            StringLiteralExpression(analyzer.MessageFormat)),
                        Argument(
                            NameColon("category"),
                            SimpleMemberAccessExpression(IdentifierName("DiagnosticCategories"), IdentifierName(categoryName ?? parent?.Category ?? analyzer.Category))),
                        Argument(
                            NameColon("defaultSeverity"),
                            SimpleMemberAccessExpression(IdentifierName("DiagnosticSeverity"), IdentifierName(parent?.DefaultSeverity ?? analyzer.DefaultSeverity))),
                        Argument(
                            NameColon("isEnabledByDefault"),
                            BooleanLiteralExpression(parent?.IsEnabledByDefault ?? analyzer.IsEnabledByDefault)),
                        Argument(
                            NameColon("description"),
                            NullLiteralExpression()),
                        Argument(
                            NameColon("helpLinkUri"),
                            idExpression),
                        Argument(
                            NameColon("customTags"),
                            (analyzer.SupportsFadeOut)
                                ? SimpleMemberAccessExpression(IdentifierName("WellKnownDiagnosticTags"), IdentifierName(WellKnownDiagnosticTags.Unnecessary))
                                : ParseExpression("Array.Empty<string>()"))
                        )))
                .AddObsoleteAttributeIf(analyzer.IsObsolete, error: true);

            if (!analyzer.IsObsolete)
            {
                var settings = new DocumentationCommentGeneratorSettings(
                    summary: new string[] { analyzer.Id },
                    ignoredTags: new[] { "returns", "value" },
                    indentation: "        ",
                    singleLineSummary: true);

                fieldDeclaration = fieldDeclaration.WithNewSingleLineDocumentationComment(settings);
            }

            return fieldDeclaration;
        }

        protected virtual ExpressionSyntax ModifyIdExpression(ExpressionSyntax expression)
        {
            return expression;
        }


    }
}
