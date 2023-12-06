// Copyright (c) .NET Foundation and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Immutable;
using System.Composition;
using System.Linq;
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

[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(UseImplicitOrExplicitObjectCreationCodeFixProvider))]
[Shared]
public class UseImplicitOrExplicitObjectCreationCodeFixProvider : BaseCodeFixProvider
{
    public sealed override ImmutableArray<string> FixableDiagnosticIds
    {
        get { return ImmutableArray.Create(DiagnosticIdentifiers.UseImplicitOrExplicitObjectCreation); }
    }

    public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
    {
        SyntaxNode root = await context.GetSyntaxRootAsync().ConfigureAwait(false);

        if (!TryFindFirstAncestorOrSelf(
            root,
            context.Span,
            out SyntaxNode node,
            predicate: f => f.IsKind(
                SyntaxKind.ObjectCreationExpression,
                SyntaxKind.ImplicitObjectCreationExpression,
                SyntaxKind.CollectionExpression,
                SyntaxKind.VariableDeclaration)))
        {
            return;
        }

        Document document = context.Document;
        Diagnostic diagnostic = context.Diagnostics[0];

        if (node is ObjectCreationExpressionSyntax objectCreation)
        {
            bool useCollectionExpression = diagnostic.Properties.ContainsKey(DiagnosticPropertyKeys.ExplicitToCollectionExpression);

            CodeAction codeAction = CodeAction.Create(
                (useCollectionExpression)
                    ? "Use collection expression"
                    : "Use implicit object creation",
                ct =>
                {
                    SyntaxNode newNode;

                    if (useCollectionExpression)
                    {
                        newNode = ConvertInitializerToCollectionExpression(objectCreation.Initializer).WithFormatterAnnotation();
                    }
                    else
                    {
                        newNode = ImplicitObjectCreationExpression(
                            objectCreation.NewKeyword.WithTrailingTrivia(objectCreation.NewKeyword.TrailingTrivia.EmptyIfWhitespace()),
                            objectCreation.ArgumentList ?? ArgumentList().WithTrailingTrivia(objectCreation.Type.GetTrailingTrivia()),
                            objectCreation.Initializer);
                    }

                    if (objectCreation.IsParentKind(SyntaxKind.EqualsValueClause)
                        && objectCreation.Parent.IsParentKind(SyntaxKind.VariableDeclarator)
                        && objectCreation.Parent.Parent.Parent is VariableDeclarationSyntax variableDeclaration
                        && variableDeclaration.Type.IsVar)
                    {
                        VariableDeclarationSyntax newVariableDeclaration = variableDeclaration
                            .ReplaceNode(objectCreation, newNode)
                            .WithType(objectCreation.Type.WithTriviaFrom(variableDeclaration.Type));

                        return document.ReplaceNodeAsync(variableDeclaration, newVariableDeclaration, ct);
                    }
                    else
                    {
                        return document.ReplaceNodeAsync(objectCreation, newNode, ct);
                    }
                },
                GetEquivalenceKey(diagnostic));

            context.RegisterCodeFix(codeAction, diagnostic);
        }
        else if (node is ImplicitObjectCreationExpressionSyntax implicitObjectCreation)
        {
            if (diagnostic.Properties.ContainsKey(DiagnosticPropertyKeys.ImplicitToCollectionExpression))
            {
                CodeAction codeAction = CodeAction.Create(
                    "Use collection expression",
                    async ct =>
                    {
                        CollectionExpressionSyntax collectionExpression = ConvertInitializerToCollectionExpression(implicitObjectCreation.Initializer)
                            .PrependToLeadingTrivia(implicitObjectCreation.NewKeyword.LeadingTrivia);

                        if (implicitObjectCreation.Initializer is null)
                            collectionExpression = collectionExpression.WithTrailingTrivia(implicitObjectCreation.ArgumentList.GetTrailingTrivia());

                        collectionExpression = collectionExpression.WithFormatterAnnotation();

                        return await document.ReplaceNodeAsync(implicitObjectCreation, collectionExpression, ct).ConfigureAwait(false);
                    },
                    GetEquivalenceKey(diagnostic));

                context.RegisterCodeFix(codeAction, diagnostic);
            }
            else
            {
                CodeAction codeAction = CodeAction.Create(
                    "Use explicit object creation",
                    async ct =>
                    {
                        SemanticModel semanticModel = await context.Document.GetSemanticModelAsync(ct).ConfigureAwait(false);
                        ITypeSymbol typeSymbol = semanticModel.GetTypeSymbol(implicitObjectCreation, ct);
                        TypeSyntax type = typeSymbol.ToTypeSyntax().WithSimplifierAnnotation();

                        SyntaxToken newKeyword = implicitObjectCreation.NewKeyword;

                        if (!newKeyword.TrailingTrivia.Any())
                            newKeyword = newKeyword.WithTrailingTrivia(ElasticSpace);

                        ObjectCreationExpressionSyntax objectCreation = ObjectCreationExpression(
                            newKeyword,
                            type,
                            implicitObjectCreation.ArgumentList,
                            implicitObjectCreation.Initializer);

                        return await document.ReplaceNodeAsync(implicitObjectCreation, objectCreation, ct).ConfigureAwait(false);
                    },
                    GetEquivalenceKey(diagnostic));

                context.RegisterCodeFix(codeAction, diagnostic);
            }
        }
        else if (node is CollectionExpressionSyntax collectionExpression)
        {
            if (diagnostic.Properties.ContainsKey(DiagnosticPropertyKeys.CollectionExpressionToImplicit))
            {
                CodeAction codeAction = CodeAction.Create(
                    "Use implicit object creation",
                    async ct =>
                    {
                        SemanticModel semanticModel = await context.Document.GetSemanticModelAsync(ct).ConfigureAwait(false);
                        ITypeSymbol typeSymbol = semanticModel.GetTypeInfo(collectionExpression, ct).ConvertedType;

                        ImplicitObjectCreationExpressionSyntax objectCreation = ImplicitObjectCreationExpression(
                            Token(SyntaxKind.NewKeyword),
                            ArgumentList(),
                            ConvertCollectionExpressionToInitializer(
                                collectionExpression,
                                SyntaxKind.CollectionInitializerExpression))
                            .WithTriviaFrom(collectionExpression);

                        return await document.ReplaceNodeAsync(collectionExpression, objectCreation, ct).ConfigureAwait(false);
                    },
                    GetEquivalenceKey(diagnostic));

                context.RegisterCodeFix(codeAction, diagnostic);
            }
            else
            {
                CodeAction codeAction = CodeAction.Create(
                    "Use explicit object creation",
                    async ct =>
                    {
                        SemanticModel semanticModel = await context.Document.GetSemanticModelAsync(ct).ConfigureAwait(false);
                        ITypeSymbol typeSymbol = semanticModel.GetTypeInfo(collectionExpression, ct).ConvertedType;
                        TypeSyntax type = typeSymbol.ToTypeSyntax().WithSimplifierAnnotation();

                        ObjectCreationExpressionSyntax objectCreation = ObjectCreationExpression(
                            Token(SyntaxKind.NewKeyword),
                            type,
                            ArgumentList(),
                            ConvertCollectionExpressionToInitializer(
                                collectionExpression,
                                SyntaxKind.CollectionInitializerExpression));

                        objectCreation = objectCreation
                            .WithLeadingTrivia(collectionExpression.GetLeadingTrivia())
                            .WithTrailingTrivia(collectionExpression.GetTrailingTrivia());

                        return await document.ReplaceNodeAsync(collectionExpression, objectCreation, ct).ConfigureAwait(false);
                    },
                    GetEquivalenceKey(diagnostic));

                context.RegisterCodeFix(codeAction, diagnostic);
            }
        }
        else
        {
            var variableDeclaration = (VariableDeclarationSyntax)node;

            CodeAction codeAction = CodeAction.Create(
                "Use explicit object creation",
                ct =>
                {
                    var implicitObjectCreation = (ImplicitObjectCreationExpressionSyntax)variableDeclaration.Variables.Single().Initializer.Value;

                    SyntaxToken newKeyword = implicitObjectCreation.NewKeyword;

                    if (!newKeyword.TrailingTrivia.Any())
                        newKeyword = newKeyword.WithTrailingTrivia(ElasticSpace);

                    ObjectCreationExpressionSyntax objectCreation = ObjectCreationExpression(
                        newKeyword,
                        variableDeclaration.Type.WithoutTrivia(),
                        implicitObjectCreation.ArgumentList,
                        implicitObjectCreation.Initializer);

                    VariableDeclarationSyntax newVariableDeclaration = variableDeclaration.ReplaceNode(implicitObjectCreation, objectCreation)
                        .WithType(CSharpFactory.VarType().WithTriviaFrom(variableDeclaration.Type));

                    return document.ReplaceNodeAsync(variableDeclaration, newVariableDeclaration, ct);
                },
                GetEquivalenceKey(diagnostic));

            context.RegisterCodeFix(codeAction, diagnostic);
        }
    }
}
