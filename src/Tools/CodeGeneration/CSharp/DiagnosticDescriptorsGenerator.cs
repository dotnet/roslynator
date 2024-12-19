using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.CSharp;
using Roslynator.Documentation;
using Roslynator.Metadata;

namespace Roslynator.CodeGeneration.CSharp;

public class DiagnosticDescriptorsGenerator
{
    public CompilationUnitSyntax Generate(
        IEnumerable<AnalyzerMetadata> analyzers,
        IComparer<string> comparer,
        string @namespace,
        string className,
        string categoryName)
    {
        analyzers = analyzers
            .OrderBy(f => f.Id, comparer);

        ClassDeclarationSyntax classDeclaration = CreateClassDeclaration(
            analyzers,
            className,
            categoryName);

        CompilationUnitSyntax compilationUnit = CSharpFactory.CompilationUnit(
            CSharpFactory.UsingDirectives("System", "Microsoft.CodeAnalysis"),
            CSharpFactory.NamespaceDeclaration(
                @namespace,
                classDeclaration));

        compilationUnit = compilationUnit.NormalizeWhitespace();

        var rewriter = new WrapRewriter(WrapRewriterOptions.WrapArguments);

        return (CompilationUnitSyntax)rewriter.Visit(compilationUnit);
    }

    private IEnumerable<MemberDeclarationSyntax> CreateMembers(
        IEnumerable<AnalyzerMetadata> analyzers,
        string categoryName,
        bool useParentProperties = false)
    {
        foreach (AnalyzerMetadata analyzer in analyzers)
        {
            if (analyzer.Id is null)
                continue;

            string identifier = analyzer.Id + "_" + analyzer.Identifier;

            yield return CreateMember(
                analyzer,
                categoryName,
                useParentProperties);

            if (analyzer.SupportsFadeOutAnalyzer)
            {
                yield return CSharpFactory.FieldDeclaration(
                        Modifiers.Public_Static_ReadOnly(),
                        SyntaxFactory.IdentifierName("DiagnosticDescriptor"),
                        identifier + "FadeOut",
                        CSharpFactory.SimpleMemberInvocationExpression(
                            SyntaxFactory.IdentifierName("DiagnosticDescriptorFactory"),
                            SyntaxFactory.IdentifierName("CreateFadeOut"),
                            CSharpFactory.ArgumentList(SyntaxFactory.Argument(SyntaxFactory.IdentifierName(identifier)))))
                    .AddObsoleteAttributeIf(analyzer.Status == AnalyzerStatus.Disabled, error: true);
            }
        }
    }

    protected virtual ClassDeclarationSyntax CreateClassDeclaration(
        IEnumerable<AnalyzerMetadata> analyzers,
        string className,
        string categoryName,
        bool useParentProperties = false)
    {
        return CSharpFactory.ClassDeclaration(
            Modifiers.Internal_Static_Partial(),
            className,
            SyntaxFactory.List(
                CreateMembers(
                    analyzers,
                    categoryName,
                    useParentProperties)));
    }

    private FieldDeclarationSyntax CreateMember(
        AnalyzerMetadata analyzer,
        string categoryName,
        bool useParentProperties = false)
    {
        AnalyzerMetadata parent = (useParentProperties) ? analyzer.Parent : null;

        ExpressionSyntax idExpression = CSharpFactory.StringLiteralExpression(parent?.Id ?? analyzer.Id);

        string title = parent?.Title ?? analyzer.Title;

        if (analyzer.Status == AnalyzerStatus.Obsolete)
            title = "[deprecated] " + title;

        FieldDeclarationSyntax fieldDeclaration = CSharpFactory.FieldDeclaration(
                (analyzer.Status == AnalyzerStatus.Disabled) ? Modifiers.Internal_Static_ReadOnly() : Modifiers.Public_Static_ReadOnly(),
                SyntaxFactory.IdentifierName("DiagnosticDescriptor"),
                analyzer.Id + "_" + analyzer.Identifier,
                CSharpFactory.SimpleMemberInvocationExpression(
                    SyntaxFactory.IdentifierName("DiagnosticDescriptorFactory"),
                    SyntaxFactory.IdentifierName("Create"),
                    CSharpFactory.ArgumentList(
                        CSharpFactory.Argument(
                            SyntaxFactory.NameColon("id"),
                            idExpression),
                        CSharpFactory.Argument(
                            SyntaxFactory.NameColon("title"),
                            CSharpFactory.StringLiteralExpression(title)),
                        CSharpFactory.Argument(
                            SyntaxFactory.NameColon("messageFormat"),
                            CSharpFactory.StringLiteralExpression(analyzer.MessageFormat)),
                        CSharpFactory.Argument(
                            SyntaxFactory.NameColon("category"),
                            CSharpFactory.SimpleMemberAccessExpression(SyntaxFactory.IdentifierName("DiagnosticCategories"), SyntaxFactory.IdentifierName(categoryName ?? parent?.Category ?? analyzer.Category))),
                        CSharpFactory.Argument(
                            SyntaxFactory.NameColon("defaultSeverity"),
                            CSharpFactory.SimpleMemberAccessExpression(SyntaxFactory.IdentifierName("DiagnosticSeverity"), SyntaxFactory.IdentifierName(parent?.DefaultSeverity ?? analyzer.DefaultSeverity))),
                        CSharpFactory.Argument(
                            SyntaxFactory.NameColon("isEnabledByDefault"),
                            CSharpFactory.BooleanLiteralExpression(parent?.IsEnabledByDefault ?? analyzer.IsEnabledByDefault)),
                        CSharpFactory.Argument(
                            SyntaxFactory.NameColon("description"),
                            CSharpFactory.NullLiteralExpression()),
                        CSharpFactory.Argument(
                            SyntaxFactory.NameColon("helpLinkUri"),
                            idExpression),
                        CSharpFactory.Argument(
                            SyntaxFactory.NameColon("customTags"),
                            (analyzer.SupportsFadeOut)
                                ? CSharpFactory.SimpleMemberAccessExpression(SyntaxFactory.IdentifierName("WellKnownDiagnosticTags"), SyntaxFactory.IdentifierName(WellKnownDiagnosticTags.Unnecessary))
                                : SyntaxFactory.ParseExpression("Array.Empty<string>()"))
                    )))
            .AddObsoleteAttributeIf(analyzer.Status == AnalyzerStatus.Disabled, error: true);

        if (analyzer.Status != AnalyzerStatus.Disabled)
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
}
