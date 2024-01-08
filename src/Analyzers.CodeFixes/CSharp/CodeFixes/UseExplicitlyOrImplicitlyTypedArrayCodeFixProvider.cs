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
    private const string UseExplicitlyTypedArrayTitle = "Use explicit type";
    private const string UseImplicitlyTypedArrayTitle = "Use implicit type";
    private const string UseCollectionExpressionTitle = "Use collection expression";

    public override ImmutableArray<string> FixableDiagnosticIds
    {
        get { return ImmutableArray.Create(DiagnosticIdentifiers.UseExplicitlyOrImplicitlyTypedArray); }
    }

#if ROSLYN_4_0
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
#endif

    public override async Task RegisterCodeFixesAsync(CodeFixContext context)
    {
        Document document = context.Document;
        Diagnostic diagnostic = context.Diagnostics[0];

        (Func<CancellationToken, Task<Document>> CreateChangedDocument, string Title)
            = await GetChangedDocumentAsync(document, diagnostic, context.CancellationToken).ConfigureAwait(false);

        CodeAction codeAction = CodeAction.Create(
            Title,
            ct => CreateChangedDocument(ct),
            GetEquivalenceKey(diagnostic, (Title == UseCollectionExpressionTitle) ? "UseCollectionExpression" : null));

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
#if ROSLYN_4_7
                SyntaxKind.CollectionExpression,
#endif
                SyntaxKind.ArrayCreationExpression)))
        {
            throw new InvalidOperationException();
        }

        if (node is ArrayCreationExpressionSyntax arrayCreation)
        {
#if ROSLYN_4_7
            if (diagnostic.Properties.ContainsKey(DiagnosticPropertyKeys.ExplicitToCollectionExpression))
            {
                return (ct => ConvertToCollectionExpressionAsync(document, arrayCreation, ct), UseCollectionExpressionTitle);
            }
            else
            {
                return (ct => ConvertToImplicitAsync(document, arrayCreation, ct), UseImplicitlyTypedArrayTitle);
            }
#else
            return (ct => ConvertToImplicitAsync(document, arrayCreation, ct), UseImplicitlyTypedArrayTitle);
#endif
        }
        else if (node is ImplicitArrayCreationExpressionSyntax implicitArrayCreation)
        {
#if ROSLYN_4_7
            if (diagnostic.Properties.ContainsKey(DiagnosticPropertyKeys.ImplicitToCollectionExpression))
            {
                return (ct => ConvertToCollectionExpressionAsync(document, implicitArrayCreation, ct), UseCollectionExpressionTitle);
            }
            else
            {
                return (ct => ConvertToExplicitAsync(document, implicitArrayCreation, ct), UseExplicitlyTypedArrayTitle);
            }
#else
            return (ct => ConvertToExplicitAsync(document, implicitArrayCreation, ct), UseExplicitlyTypedArrayTitle);
#endif
        }
#if ROSLYN_4_7
        else if (node is CollectionExpressionSyntax collectionExpression)
        {
            if (diagnostic.Properties.ContainsKey(DiagnosticPropertyKeys.CollectionExpressionToImplicit))
            {
                return (ct => ConvertToImplicitAsync(document, collectionExpression, ct), UseImplicitlyTypedArrayTitle);
            }
            else
            {
                return (ct => ConvertToExplicitAsync(document, collectionExpression, ct), UseExplicitlyTypedArrayTitle);
            }
        }
#endif
        else
        {
            throw new InvalidOperationException();
        }
    }

    private static async Task<Document> ConvertToExplicitAsync(
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

    private static async Task<Document> ConvertToImplicitAsync(
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

#if ROSLYN_4_7
    private static async Task<Document> ConvertToExplicitAsync(
        Document document,
        CollectionExpressionSyntax collectionExpression,
        CancellationToken cancellationToken)
    {
        SemanticModel semanticModel = await document.GetSemanticModelAsync(cancellationToken).ConfigureAwait(false);
        ITypeSymbol typeSymbol = semanticModel.GetTypeInfo(collectionExpression, cancellationToken).ConvertedType;

        ArrayCreationExpressionSyntax arrayCreation = ArrayCreationExpression(
            Token(SyntaxKind.NewKeyword),
            (ArrayTypeSyntax)typeSymbol.ToTypeSyntax().WithSimplifierAnnotation(),
            ConvertCollectionExpressionToInitializer(collectionExpression, SyntaxKind.ArrayInitializerExpression))
            .WithTriviaFrom(collectionExpression);

        return await document.ReplaceNodeAsync(collectionExpression, arrayCreation, cancellationToken).ConfigureAwait(false);
    }

    private static async Task<Document> ConvertToImplicitAsync(
        Document document,
        CollectionExpressionSyntax collectionExpression,
        CancellationToken cancellationToken)
    {
        InitializerExpressionSyntax initializer = ConvertCollectionExpressionToInitializer(collectionExpression, SyntaxKind.ArrayInitializerExpression);

        ImplicitArrayCreationExpressionSyntax implicitArrayCreation = ImplicitArrayCreationExpression(initializer)
            .WithTriviaFrom(collectionExpression);

        return await document.ReplaceNodeAsync(collectionExpression, implicitArrayCreation, cancellationToken).ConfigureAwait(false);
    }

    private static async Task<Document> ConvertToCollectionExpressionAsync(
        Document document,
        ArrayCreationExpressionSyntax arrayCreation,
        CancellationToken cancellationToken)
    {
        CollectionExpressionSyntax collectionExpression = ConvertInitializerToCollectionExpression(arrayCreation.Initializer)
            .WithTriviaFrom(arrayCreation)
            .WithFormatterAnnotation();

        return await document.ReplaceNodeAsync(arrayCreation, collectionExpression, cancellationToken).ConfigureAwait(false);
    }

    private static async Task<Document> ConvertToCollectionExpressionAsync(
        Document document,
        ImplicitArrayCreationExpressionSyntax implicitArrayCreation,
        CancellationToken cancellationToken)
    {
        CollectionExpressionSyntax collectionExpression = ConvertInitializerToCollectionExpression(implicitArrayCreation.Initializer)
            .WithTriviaFrom(implicitArrayCreation)
            .WithFormatterAnnotation();

        return await document.ReplaceNodeAsync(implicitArrayCreation, collectionExpression, cancellationToken).ConfigureAwait(false);
    }
#endif
}
