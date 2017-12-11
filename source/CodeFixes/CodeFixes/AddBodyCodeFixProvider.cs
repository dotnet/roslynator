// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Immutable;
using System.Composition;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Roslynator.CSharp.CodeFixes
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(AddBodyCodeFixProvider))]
    [Shared]
    public class AddBodyCodeFixProvider : BaseCodeFixProvider
    {
        public sealed override ImmutableArray<string> FixableDiagnosticIds
        {
            get
            {
                return ImmutableArray.Create(
                    CompilerDiagnosticIdentifiers.PartialMethodMayNotHaveMultipleDefiningDeclarations,
                    CompilerDiagnosticIdentifiers.MemberMustDeclareBodyBecauseItIsNotMarkedAbstractExternOrPartial);
            }
        }

        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            if (!Settings.IsCodeFixEnabled(CodeFixIdentifiers.AddBody))
                return;

            SyntaxNode root = await context.GetSyntaxRootAsync().ConfigureAwait(false);

            if (!TryFindFirstAncestorOrSelf(root, context.Span, out SyntaxNode node, predicate: f => f is MemberDeclarationSyntax || f is AccessorDeclarationSyntax))
                return;

            foreach (Diagnostic diagnostic in context.Diagnostics)
            {
                switch (diagnostic.Id)
                {
                    case CompilerDiagnosticIdentifiers.PartialMethodMayNotHaveMultipleDefiningDeclarations:
                    case CompilerDiagnosticIdentifiers.MemberMustDeclareBodyBecauseItIsNotMarkedAbstractExternOrPartial:
                        {
                            Func<CancellationToken, Task<Document>> createChangedDocument = GetCreateChangedDocument(context, node);

                            if (createChangedDocument == null)
                                break;

                            CodeAction codeAction = CodeAction.Create(
                                "Add body",
                                createChangedDocument,
                                GetEquivalenceKey(diagnostic));

                            context.RegisterCodeFix(codeAction, diagnostic);
                            break;
                        }
                }
            }
        }

        private static Func<CancellationToken, Task<Document>> GetCreateChangedDocument(CodeFixContext context, SyntaxNode node)
        {
            switch (node.Kind())
            {
                case SyntaxKind.MethodDeclaration:
                    {
                        return cancellationToken =>
                        {
                            var methodDeclaration = (MethodDeclarationSyntax)node;

                            ParameterListSyntax parameterList = methodDeclaration.ParameterList ?? ParameterList();

                            MethodDeclarationSyntax newNode = methodDeclaration
                                .WithParameterList(parameterList.AppendToTrailingTrivia(methodDeclaration.SemicolonToken.GetLeadingAndTrailingTrivia()))
                                .WithSemicolonToken(default(SyntaxToken))
                                .WithBody(Block())
                                .WithFormatterAnnotation();

                            return context.Document.ReplaceNodeAsync(node, newNode, cancellationToken);
                        };
                    }
                case SyntaxKind.ConstructorDeclaration:
                    {
                        return cancellationToken =>
                        {
                            var constructorDeclaration = (ConstructorDeclarationSyntax)node;

                            ParameterListSyntax parameterList = constructorDeclaration.ParameterList ?? ParameterList();

                            ConstructorDeclarationSyntax newNode = constructorDeclaration
                                .WithParameterList(parameterList.AppendToTrailingTrivia(constructorDeclaration.SemicolonToken.GetLeadingAndTrailingTrivia()))
                                .WithSemicolonToken(default(SyntaxToken))
                                .WithBody(Block())
                                .WithFormatterAnnotation();

                            return context.Document.ReplaceNodeAsync(node, newNode, cancellationToken);
                        };
                    }
                case SyntaxKind.DestructorDeclaration:
                    {
                        return cancellationToken =>
                        {
                            var destructorDeclaration = (DestructorDeclarationSyntax)node;

                            ParameterListSyntax parameterList = destructorDeclaration.ParameterList ?? ParameterList();

                            DestructorDeclarationSyntax newNode = destructorDeclaration
                                .WithParameterList(parameterList.AppendToTrailingTrivia(destructorDeclaration.SemicolonToken.GetLeadingAndTrailingTrivia()))
                                .WithSemicolonToken(default(SyntaxToken))
                                .WithBody(Block())
                                .WithFormatterAnnotation();

                            return context.Document.ReplaceNodeAsync(node, newNode, cancellationToken);
                        };
                    }
                case SyntaxKind.OperatorDeclaration:
                    {
                        return cancellationToken =>
                        {
                            var operatorDeclaration = (OperatorDeclarationSyntax)node;

                            ParameterListSyntax parameterList = operatorDeclaration.ParameterList ?? ParameterList();

                            OperatorDeclarationSyntax newNode = operatorDeclaration
                                .WithParameterList(parameterList.AppendToTrailingTrivia(operatorDeclaration.SemicolonToken.GetLeadingAndTrailingTrivia()))
                                .WithSemicolonToken(default(SyntaxToken))
                                .WithBody(Block())
                                .WithFormatterAnnotation();

                            return context.Document.ReplaceNodeAsync(node, newNode, cancellationToken);
                        };
                    }
                case SyntaxKind.ConversionOperatorDeclaration:
                    {
                        return cancellationToken =>
                        {
                            var conversionOperatorDeclaration = (ConversionOperatorDeclarationSyntax)node;

                            ParameterListSyntax parameterList = conversionOperatorDeclaration.ParameterList ?? ParameterList();

                            ConversionOperatorDeclarationSyntax newNode = conversionOperatorDeclaration
                                .WithParameterList(parameterList.AppendToTrailingTrivia(conversionOperatorDeclaration.SemicolonToken.GetLeadingAndTrailingTrivia()))
                                .WithSemicolonToken(default(SyntaxToken))
                                .WithBody(Block())
                                .WithFormatterAnnotation();

                            return context.Document.ReplaceNodeAsync(node, newNode, cancellationToken);
                        };
                    }
                case SyntaxKind.GetAccessorDeclaration:
                case SyntaxKind.SetAccessorDeclaration:
                    {
                        return cancellationToken =>
                        {
                            var accessorDeclaration = (AccessorDeclarationSyntax)node;

                            AccessorDeclarationSyntax newNode = accessorDeclaration
                                .WithSemicolonToken(default(SyntaxToken))
                                .WithBody(Block(
                                    Token(default(SyntaxTriviaList), SyntaxKind.OpenBraceToken, TriviaList(ElasticSpace)),
                                    default(SyntaxList<StatementSyntax>),
                                    Token(default(SyntaxTriviaList), SyntaxKind.CloseBraceToken, accessorDeclaration.SemicolonToken.GetLeadingAndTrailingTrivia())));

                            SyntaxToken keyword = newNode.Keyword;

                            if (!keyword.HasTrailingTrivia)
                                newNode = newNode.WithKeyword(keyword.WithTrailingTrivia(ElasticSpace));

                            return context.Document.ReplaceNodeAsync(node, newNode, cancellationToken);
                        };
                    }
            }

            Debug.Fail(node.Kind().ToString());

            return null;
        }
    }
}
