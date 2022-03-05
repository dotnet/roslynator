// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

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
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Roslynator.CSharp.CodeFixes
{
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
                predicate: f => f.IsKind(SyntaxKind.ObjectCreationExpression, SyntaxKind.ImplicitObjectCreationExpression, SyntaxKind.VariableDeclaration)))
            {
                return;
            }

            Document document = context.Document;
            Diagnostic diagnostic = context.Diagnostics[0];

            if (node is ObjectCreationExpressionSyntax objectCreation)
            {
                CodeAction codeAction = CodeAction.Create(
                    "Use implicit object creation",
                    ct =>
                    {
                        ImplicitObjectCreationExpressionSyntax implicitObjectCreation = ImplicitObjectCreationExpression(
                            objectCreation.NewKeyword.WithTrailingTrivia(objectCreation.NewKeyword.TrailingTrivia.EmptyIfWhitespace()),
                            objectCreation.ArgumentList ?? ArgumentList().WithTrailingTrivia(objectCreation.Type.GetTrailingTrivia()),
                            objectCreation.Initializer);

                        if (objectCreation.IsParentKind(SyntaxKind.EqualsValueClause)
                            && objectCreation.Parent.IsParentKind(SyntaxKind.VariableDeclarator)
                            && objectCreation.Parent.Parent.Parent is VariableDeclarationSyntax variableDeclaration
                            && variableDeclaration.Type.IsVar)
                        {
                            VariableDeclarationSyntax newVariableDeclaration = variableDeclaration
                                .ReplaceNode(objectCreation, implicitObjectCreation)
                                .WithType(objectCreation.Type.WithTriviaFrom(variableDeclaration.Type));

                            return document.ReplaceNodeAsync(variableDeclaration, newVariableDeclaration, ct);
                        }
                        else
                        {
                            return document.ReplaceNodeAsync(objectCreation, implicitObjectCreation, ct);
                        }
                    },
                    GetEquivalenceKey(diagnostic));

                context.RegisterCodeFix(codeAction, diagnostic);
            }
            else if (node is ImplicitObjectCreationExpressionSyntax implicitObjectCreation)
            {
                CodeAction codeAction = CodeAction.Create(
                    "Use explicit object creation",
                    async ct =>
                    {
                        SemanticModel semanticModel = await context.Document.GetSemanticModelAsync(ct).ConfigureAwait(false);

                        ITypeSymbol typeSymbol = semanticModel.GetTypeSymbol(implicitObjectCreation, ct);

                        TypeSyntax type = typeSymbol.ToTypeSyntax().WithSimplifierAnnotation();

                        SyntaxToken newKeyword = implicitObjectCreation.NewKeyword;

                        if (newKeyword.TrailingTrivia.Count == 0)
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
            else
            {
                var variableDeclaration = (VariableDeclarationSyntax)node;

                CodeAction codeAction = CodeAction.Create(
                    "Use explicit object creation",
                    ct =>
                    {
                        var implicitObjectCreation = (ImplicitObjectCreationExpressionSyntax)variableDeclaration.Variables.Single().Initializer.Value;

                        SyntaxToken newKeyword = implicitObjectCreation.NewKeyword;

                        if (newKeyword.TrailingTrivia.Count == 0)
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
}
