﻿// Copyright (c) .NET Foundation and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Immutable;
using System.Composition;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.CSharp;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static Roslynator.CSharp.CSharpFactory;

namespace Roslynator.CodeAnalysis.CSharp;

[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(AttributeCodeFixProvider))]
[Shared]
public sealed class AttributeCodeFixProvider : BaseCodeFixProvider
{
    public override ImmutableArray<string> FixableDiagnosticIds
    {
        get
        {
            return ImmutableArray.Create(
                CodeAnalysisDiagnosticIdentifiers.SpecifyExportCodeFixProviderAttributeName,
                CodeAnalysisDiagnosticIdentifiers.SpecifyExportCodeRefactoringProviderAttributeName);
        }
    }

    public override async Task RegisterCodeFixesAsync(CodeFixContext context)
    {
        SyntaxNode root = await context.GetSyntaxRootAsync().ConfigureAwait(false);

        if (!TryFindFirstAncestorOrSelf(root, context.Span, out AttributeSyntax attribute))
            return;

        Document document = context.Document;
        Diagnostic diagnostic = context.Diagnostics[0];

        switch (diagnostic.Id)
        {
            case CodeAnalysisDiagnosticIdentifiers.SpecifyExportCodeFixProviderAttributeName:
            case CodeAnalysisDiagnosticIdentifiers.SpecifyExportCodeRefactoringProviderAttributeName:
            {
                CodeAction codeAction = CodeAction.Create(
                    "Specify name",
                    ct => SpecifyNameAsync(document, attribute, ct),
                    GetEquivalenceKey(diagnostic));

                context.RegisterCodeFix(codeAction, diagnostic);
                break;
            }
        }
    }

    private static Task<Document> SpecifyNameAsync(
        Document document,
        AttributeSyntax attribute,
        CancellationToken cancellationToken)
    {
        AttributeArgumentSyntax newAttributeArgument = AttributeArgument(
            NameEquals("Name"),
            NameOfExpression(
                IdentifierName(attribute.FirstAncestor<ClassDeclarationSyntax>().Identifier.WithNavigationAnnotation())));

        AttributeArgumentListSyntax argumentList = attribute.ArgumentList;

        SeparatedSyntaxList<AttributeArgumentSyntax> arguments = argumentList.Arguments.Add(newAttributeArgument);

        AttributeArgumentListSyntax newArgumentList = argumentList.WithArguments(arguments);

        AttributeSyntax newAttribute = attribute.WithArgumentList(newArgumentList);

        return document.ReplaceNodeAsync(attribute, newAttribute, cancellationToken);
    }
}
