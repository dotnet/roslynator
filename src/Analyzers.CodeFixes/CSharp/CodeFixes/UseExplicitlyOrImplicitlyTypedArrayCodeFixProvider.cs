// Copyright (c) .NET Foundation and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.CodeFixes;
using Roslynator.CSharp.Analysis;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static Roslynator.CSharp.SyntaxRefactorings;

namespace Roslynator.CSharp.CodeFixes;

[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(UseExplicitlyOrImplicitlyTypedArrayCodeFixProvider))]
[Shared]
public sealed class UseExplicitlyOrImplicitlyTypedArrayCodeFixProvider : BaseCodeFixProvider
{
    private const string ToImplicitTitle = "Use implicitly typed array";
    private const string ToExplicitTitle = "Use explicitly typed array";
    private const string ToCollectionExpressionTitle = "Use collection expression";

    public override ImmutableArray<string> FixableDiagnosticIds
    {
        get { return ImmutableArray.Create(DiagnosticIdentifiers.UseExplicitlyOrImplicitlyTypedArray); }
    }

    public override FixAllProvider GetFixAllProvider()
    {
        return FixAllProvider.Create(async (context, document, diagnostics) => await FixAllAsync(document, diagnostics, context.CancellationToken).ConfigureAwait(false));

        static async Task<Document> FixAllAsync(
            Document document,
            ImmutableArray<Diagnostic> diagnostics,
            CancellationToken cancellationToken)
        {
            foreach (Diagnostic diagnostic in diagnostics.OrderByDescending(d => d.Location.SourceSpan.Start))
            {
                (Func<CancellationToken, Task<Document>> CreateChangedDocument, string) result
                    = await GetChangedDocumentAsync(document, diagnostic, cancellationToken).ConfigureAwait(false);

                document = await result.CreateChangedDocument(cancellationToken).ConfigureAwait(false);
            }

            return document;
        }
    }

    public override async Task RegisterCodeFixesAsync(CodeFixContext context)
    {
        Document document = context.Document;
        Diagnostic diagnostic = context.Diagnostics[0];

        (Func<CancellationToken, Task<Document>> CreateChangedDocument, string title)
            = await GetChangedDocumentAsync(document, diagnostic, context.CancellationToken).ConfigureAwait(false);

        CodeAction codeAction = CodeAction.Create(
            title,
            ct => CreateChangedDocument(ct),
            GetEquivalenceKey(diagnostic));

        context.RegisterCodeFix(codeAction, diagnostic);
    }

    private static async Task<(Func<CancellationToken, Task<Document>>, string)> GetChangedDocumentAsync(
        Document document,
        Diagnostic diagnostic,
        CancellationToken cancellationToken)
    {
        SyntaxNode root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);

        if (!TryFindFirstAncestorOrSelf(
            root,
            diagnostic.Location.SourceSpan,
            out SyntaxNode node,
            predicate: f => f.IsKind(
                SyntaxKind.ImplicitArrayCreationExpression,
                SyntaxKind.ArrayCreationExpression,
                SyntaxKind.CollectionExpression)))
        {
            throw new InvalidOperationException();
        }

        if (node is ArrayCreationExpressionSyntax arrayCreation)
        {
            SemanticModel semanticModel = await document.GetSemanticModelAsync(cancellationToken).ConfigureAwait(false);

            bool useCollectionExpression = document.GetConfigOptions(arrayCreation.SyntaxTree).UseCollectionExpression() == true
                && CSharpUtility.CanConvertToCollectionExpression(arrayCreation, semanticModel, cancellationToken);

            if (useCollectionExpression)
            {
                return (ct => ChangeToCollectionExpressionAsync(document, arrayCreation, ct), ToCollectionExpressionTitle);
            }
            else
            {
                return (ct => ChangeToImplicitAsync(document, arrayCreation, ct), ToImplicitTitle);
            }
        }
        else if (node is ImplicitArrayCreationExpressionSyntax implicitArrayCreation)
        {
            if (diagnostic.Properties.ContainsKey(DiagnosticPropertyKeys.ConvertImplicitToImplicit))
            {
                return (ct => ChangeToCollectionExpressionAsync(document, implicitArrayCreation, ct), ToCollectionExpressionTitle);
            }
            else
            {
                return (ct => ChangeToExplicitAsync(document, implicitArrayCreation, ct), ToExplicitTitle);
            }
        }
        else if (node is CollectionExpressionSyntax collectionExpression)
        {
            if (diagnostic.Properties.ContainsKey(DiagnosticPropertyKeys.ConvertImplicitToImplicit))
            {
                return (ct => ChangeToImplicitAsync(document, collectionExpression, ct), ToImplicitTitle);
            }
            else
            {
                return (ct => ChangeToExplicitAsync(document, collectionExpression, ct), ToExplicitTitle);
            }
        }
        else
        {
            throw new InvalidOperationException();
        }
    }

    private static async Task<Document> ChangeToExplicitAsync(
        Document document,
        ImplicitArrayCreationExpressionSyntax implicitArrayCreation,
        CancellationToken cancellationToken)
    {
        SemanticModel semanticModel = await document.GetSemanticModelAsync(cancellationToken).ConfigureAwait(false);

        ITypeSymbol typeSymbol = semanticModel.GetTypeSymbol(implicitArrayCreation, cancellationToken);

        var arrayType = (ArrayTypeSyntax)typeSymbol.ToTypeSyntax().WithSimplifierAnnotation();

        SyntaxToken newKeyword = implicitArrayCreation.NewKeyword;

        if (!newKeyword.HasTrailingTrivia)
            newKeyword = newKeyword.WithTrailingTrivia(Space);

        InitializerExpressionSyntax initializer = implicitArrayCreation.Initializer;

        InitializerExpressionSyntax newInitializer = initializer.ReplaceNodes(
            initializer.Expressions,
            (node, _) => (node.IsKind(SyntaxKind.CastExpression)) ? node.WithSimplifierAnnotation() : node);

        ArrayCreationExpressionSyntax newNode = ArrayCreationExpression(
            newKeyword,
            arrayType
                .WithLeadingTrivia(implicitArrayCreation.OpenBracketToken.LeadingTrivia)
                .WithTrailingTrivia(implicitArrayCreation.CloseBracketToken.TrailingTrivia),
            newInitializer);

        return await document.ReplaceNodeAsync(implicitArrayCreation, newNode, cancellationToken).ConfigureAwait(false);
    }

    private static async Task<Document> ChangeToExplicitAsync(
        Document document,
        CollectionExpressionSyntax collectionExpression,
        CancellationToken cancellationToken)
    {
        SemanticModel semanticModel = await document.GetSemanticModelAsync(cancellationToken).ConfigureAwait(false);

        ITypeSymbol typeSymbol = semanticModel.GetTypeInfo(collectionExpression, cancellationToken).ConvertedType;

        var arrayType = (ArrayTypeSyntax)typeSymbol.ToTypeSyntax().WithSimplifierAnnotation();

        InitializerExpressionSyntax initializer = ConvertCollectionExpressionToInitializer(collectionExpression, SyntaxKind.ArrayInitializerExpression);

        ArrayCreationExpressionSyntax arrayCreation = ArrayCreationExpression(
            Token(SyntaxKind.NewKeyword),
            arrayType,
            initializer)
            .WithTriviaFrom(collectionExpression);

        return await document.ReplaceNodeAsync(collectionExpression, arrayCreation, cancellationToken).ConfigureAwait(false);
    }

    private static async Task<Document> ChangeToImplicitAsync(
        Document document,
        ArrayCreationExpressionSyntax arrayCreation,
        CancellationToken cancellationToken)
    {
        ArrayTypeSyntax arrayType = arrayCreation.Type;
        SyntaxList<ArrayRankSpecifierSyntax> rankSpecifiers = arrayType.RankSpecifiers;
        InitializerExpressionSyntax initializer = arrayCreation.Initializer;

        TypeSyntax castType;

        if (rankSpecifiers.Count > 1)
        {
            castType = ParseTypeName(arrayType.ToString().Remove(rankSpecifiers.Last().SpanStart - arrayType.SpanStart));
        }
        else
        {
            castType = arrayType.ElementType;
        }

        InitializerExpressionSyntax newInitializer = initializer.ReplaceNodes(
            initializer.Expressions,
            (node, _) => CastExpression(castType, node.WithoutTrivia())
                .WithTriviaFrom(node)
                .WithSimplifierAnnotation());

        ImplicitArrayCreationExpressionSyntax implicitArrayCreation = ImplicitArrayCreationExpression(
            arrayCreation.NewKeyword.WithTrailingTrivia(arrayCreation.NewKeyword.TrailingTrivia.EmptyIfWhitespace()),
            rankSpecifiers[0].OpenBracketToken,
            default(SyntaxTokenList),
            rankSpecifiers.Last().CloseBracketToken,
            newInitializer);

        return await document.ReplaceNodeAsync(arrayCreation, implicitArrayCreation, cancellationToken).ConfigureAwait(false);
    }

    private static async Task<Document> ChangeToImplicitAsync(
        Document document,
        CollectionExpressionSyntax collectionExpression,
        CancellationToken cancellationToken)
    {
        InitializerExpressionSyntax initializer = ConvertCollectionExpressionToInitializer(collectionExpression, SyntaxKind.ArrayInitializerExpression);

        ImplicitArrayCreationExpressionSyntax implicitArrayCreation = ImplicitArrayCreationExpression(initializer)
            .WithTriviaFrom(collectionExpression);

        return await document.ReplaceNodeAsync(collectionExpression, implicitArrayCreation, cancellationToken).ConfigureAwait(false);
    }

    private static async Task<Document> ChangeToCollectionExpressionAsync(
        Document document,
        ArrayCreationExpressionSyntax arrayCreation,
        CancellationToken cancellationToken)
    {
        CollectionExpressionSyntax collectionExpression = ConvertInitializerToCollectionExpression(arrayCreation.Initializer)
            .WithTriviaFrom(arrayCreation)
            .WithFormatterAnnotation();

        return await document.ReplaceNodeAsync(arrayCreation, collectionExpression, cancellationToken).ConfigureAwait(false);
    }

    private static async Task<Document> ChangeToCollectionExpressionAsync(
        Document document,
        ImplicitArrayCreationExpressionSyntax implicitArrayCreation,
        CancellationToken cancellationToken)
    {
        CollectionExpressionSyntax collectionExpression = ConvertInitializerToCollectionExpression(implicitArrayCreation.Initializer)
            .WithTriviaFrom(implicitArrayCreation)
            .WithFormatterAnnotation();

        return await document.ReplaceNodeAsync(implicitArrayCreation, collectionExpression, cancellationToken).ConfigureAwait(false);
    }
}
