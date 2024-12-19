// Copyright (c) .NET Foundation and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.CSharp;
using Roslynator.Documentation;
using Roslynator.Metadata;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static Roslynator.CSharp.CSharpFactory;

namespace Roslynator.CodeGeneration.CSharp;

public class DiagnosticRulesGenerator
{
    public CompilationUnitSyntax Generate(
        IEnumerable<AnalyzerMetadata> analyzers,
        IComparer<string> comparer,
        string @namespace,
        string className)
    {
        analyzers = analyzers
            .OrderBy(f => f.Id, comparer);

        ClassDeclarationSyntax classDeclaration = CreateClassDeclaration(analyzers, className);

        CompilationUnitSyntax compilationUnit = CompilationUnit(
            UsingDirectives("System", "Microsoft.CodeAnalysis"),
            NamespaceDeclaration(
                @namespace,
                classDeclaration));

        compilationUnit = compilationUnit.NormalizeWhitespace();

        var rewriter = new WrapRewriter(WrapRewriterOptions.WrapArguments);

        return (CompilationUnitSyntax)rewriter.Visit(compilationUnit);
    }

    private IEnumerable<MemberDeclarationSyntax> CreateMembers(IEnumerable<AnalyzerMetadata> analyzers)
    {
        foreach (AnalyzerMetadata analyzer in analyzers)
        {
            if (analyzer.Id is null)
                continue;

            string identifier = analyzer.Identifier;

            yield return CreateMember(analyzer);

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
                    .AddObsoleteAttributeIf(analyzer.Status == AnalyzerStatus.Disabled, error: true);
            }
        }
    }

    protected virtual ClassDeclarationSyntax CreateClassDeclaration(
        IEnumerable<AnalyzerMetadata> analyzers,
        string className)
    {
        return ClassDeclaration(
            Modifiers.Public_Static_Partial(),
            className,
            List(
                CreateMembers(analyzers)));
    }

    private FieldDeclarationSyntax CreateMember(
        AnalyzerMetadata analyzer)
    {
        FieldDeclarationSyntax fieldDeclaration = FieldDeclaration(
            (analyzer.Status == AnalyzerStatus.Disabled)
                ? Modifiers.Internal_Static_ReadOnly()
                : Modifiers.Public_Static_ReadOnly(),
            IdentifierName("DiagnosticDescriptor"),
            analyzer.Identifier,
            ParseName($"DiagnosticDescriptors.{analyzer.Id}_{analyzer.Identifier}"));

        if (analyzer.Status != AnalyzerStatus.Disabled)
        {
            var settings = new DocumentationCommentGeneratorSettings(
                summary: [analyzer.Id],
                ignoredTags: ["returns", "value"],
                indentation: "        ",
                singleLineSummary: true);

            fieldDeclaration = fieldDeclaration.WithNewSingleLineDocumentationComment(settings);
        }

        return fieldDeclaration;
    }
}
