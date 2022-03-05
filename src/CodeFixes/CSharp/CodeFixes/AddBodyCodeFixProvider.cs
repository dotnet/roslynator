// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

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
using Roslynator.CodeFixes;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Roslynator.CSharp.CodeFixes
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(AddBodyCodeFixProvider))]
    [Shared]
    public sealed class AddBodyCodeFixProvider : CompilerDiagnosticCodeFixProvider
    {
        public override ImmutableArray<string> FixableDiagnosticIds
        {
            get
            {
                return ImmutableArray.Create(
                    CompilerDiagnosticIdentifiers.CS0756_PartialMethodMayNotHaveMultipleDefiningDeclarations,
                    CompilerDiagnosticIdentifiers.CS0501_MemberMustDeclareBodyBecauseItIsNotMarkedAbstractExternOrPartial,
                    CompilerDiagnosticIdentifiers.CS8112_LocalFunctionMustAlwaysHaveBody);
            }
        }

        public override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            Diagnostic diagnostic = context.Diagnostics[0];

            SyntaxNode root = await context.GetSyntaxRootAsync().ConfigureAwait(false);

            if (!IsEnabled(diagnostic.Id, CodeFixIdentifiers.AddBody, context.Document, root.SyntaxTree))
                return;

            if (!TryFindFirstAncestorOrSelf(
                root,
                context.Span,
                out SyntaxNode node,
                predicate: f => f is MemberDeclarationSyntax || f is AccessorDeclarationSyntax || f is LocalFunctionStatementSyntax))
            {
                return;
            }

            Func<CancellationToken, Task<Document>> createChangedDocument = GetCreateChangedDocument(context, node);

            if (createChangedDocument == null)
                return;

            CodeAction codeAction = CodeAction.Create(
                "Add body",
                createChangedDocument,
                GetEquivalenceKey(diagnostic, CodeFixIdentifiers.AddBody));

            context.RegisterCodeFix(codeAction, diagnostic);
        }

        private static Func<CancellationToken, Task<Document>> GetCreateChangedDocument(CodeFixContext context, SyntaxNode node)
        {
            switch (node.Kind())
            {
                case SyntaxKind.MethodDeclaration:
                    {
                        var methodDeclaration = (MethodDeclarationSyntax)node;

                        SyntaxToken semicolonToken = methodDeclaration.SemicolonToken;

                        if (semicolonToken.IsKind(SyntaxKind.None))
                            break;

                        ParameterListSyntax parameterList = methodDeclaration.ParameterList;

                        if (parameterList == null)
                            break;

                        return ct =>
                        {
                            MethodDeclarationSyntax newNode = methodDeclaration
                                .WithParameterList(parameterList.AppendToTrailingTrivia(semicolonToken.GetAllTrivia()))
                                .WithSemicolonToken(default(SyntaxToken))
                                .WithBody(Block())
                                .WithFormatterAnnotation();

                            return context.Document.ReplaceNodeAsync(node, newNode, ct);
                        };
                    }
                case SyntaxKind.ConstructorDeclaration:
                    {
                        var constructorDeclaration = (ConstructorDeclarationSyntax)node;

                        SyntaxToken semicolonToken = constructorDeclaration.SemicolonToken;

                        if (semicolonToken.IsKind(SyntaxKind.None))
                            break;

                        ParameterListSyntax parameterList = constructorDeclaration.ParameterList;

                        if (parameterList == null)
                            break;

                        return ct =>
                        {
                            ConstructorDeclarationSyntax newNode = constructorDeclaration
                                .WithParameterList(parameterList.AppendToTrailingTrivia(semicolonToken.GetAllTrivia()))
                                .WithSemicolonToken(default(SyntaxToken))
                                .WithBody(Block())
                                .WithFormatterAnnotation();

                            return context.Document.ReplaceNodeAsync(node, newNode, ct);
                        };
                    }
                case SyntaxKind.DestructorDeclaration:
                    {
                        var destructorDeclaration = (DestructorDeclarationSyntax)node;

                        SyntaxToken semicolonToken = destructorDeclaration.SemicolonToken;

                        if (semicolonToken.IsKind(SyntaxKind.None))
                            break;

                        ParameterListSyntax parameterList = destructorDeclaration.ParameterList;

                        if (parameterList == null)
                            break;

                        return ct =>
                        {
                            DestructorDeclarationSyntax newNode = destructorDeclaration
                                .WithParameterList(parameterList.AppendToTrailingTrivia(semicolonToken.GetAllTrivia()))
                                .WithSemicolonToken(default(SyntaxToken))
                                .WithBody(Block())
                                .WithFormatterAnnotation();

                            return context.Document.ReplaceNodeAsync(node, newNode, ct);
                        };
                    }
                case SyntaxKind.OperatorDeclaration:
                    {
                        var operatorDeclaration = (OperatorDeclarationSyntax)node;

                        SyntaxToken semicolonToken = operatorDeclaration.SemicolonToken;

                        if (semicolonToken.IsKind(SyntaxKind.None))
                            break;

                        ParameterListSyntax parameterList = operatorDeclaration.ParameterList;

                        if (parameterList == null)
                            break;

                        return ct =>
                        {
                            OperatorDeclarationSyntax newNode = operatorDeclaration
                                .WithParameterList(parameterList.AppendToTrailingTrivia(semicolonToken.GetAllTrivia()))
                                .WithSemicolonToken(default(SyntaxToken))
                                .WithBody(Block())
                                .WithFormatterAnnotation();

                            return context.Document.ReplaceNodeAsync(node, newNode, ct);
                        };
                    }
                case SyntaxKind.ConversionOperatorDeclaration:
                    {
                        var conversionOperatorDeclaration = (ConversionOperatorDeclarationSyntax)node;

                        SyntaxToken semicolonToken = conversionOperatorDeclaration.SemicolonToken;

                        if (semicolonToken.IsKind(SyntaxKind.None))
                            break;

                        ParameterListSyntax parameterList = conversionOperatorDeclaration.ParameterList;

                        if (parameterList == null)
                            break;

                        return ct =>
                        {
                            ConversionOperatorDeclarationSyntax newNode = conversionOperatorDeclaration
                                .WithParameterList(parameterList.AppendToTrailingTrivia(semicolonToken.GetAllTrivia()))
                                .WithSemicolonToken(default(SyntaxToken))
                                .WithBody(Block())
                                .WithFormatterAnnotation();

                            return context.Document.ReplaceNodeAsync(node, newNode, ct);
                        };
                    }
                case SyntaxKind.GetAccessorDeclaration:
                case SyntaxKind.SetAccessorDeclaration:
                    {
                        var accessorDeclaration = (AccessorDeclarationSyntax)node;

                        SyntaxToken semicolonToken = accessorDeclaration.SemicolonToken;

                        if (semicolonToken.IsKind(SyntaxKind.None))
                            break;

                        return ct =>
                        {
                            AccessorDeclarationSyntax newNode = accessorDeclaration
                                .WithSemicolonToken(default(SyntaxToken))
                                .WithBody(Block(
                                    Token(default(SyntaxTriviaList), SyntaxKind.OpenBraceToken, TriviaList(ElasticSpace)),
                                    default(SyntaxList<StatementSyntax>),
                                    Token(default(SyntaxTriviaList), SyntaxKind.CloseBraceToken, semicolonToken.LeadingAndTrailingTrivia())));

                            SyntaxToken keyword = newNode.Keyword;

                            if (!keyword.HasTrailingTrivia)
                                newNode = newNode.WithKeyword(keyword.WithTrailingTrivia(ElasticSpace));

                            return context.Document.ReplaceNodeAsync(node, newNode, ct);
                        };
                    }
                case SyntaxKind.LocalFunctionStatement:
                    {
                        var localFunction = (LocalFunctionStatementSyntax)node;

                        SyntaxToken semicolonToken = localFunction.SemicolonToken;

                        if (semicolonToken.IsKind(SyntaxKind.None))
                            break;

                        ParameterListSyntax parameterList = localFunction.ParameterList;

                        if (parameterList == null)
                            break;

                        return ct =>
                        {
                            LocalFunctionStatementSyntax newNode = localFunction
                                .WithParameterList(parameterList.AppendToTrailingTrivia(semicolonToken.GetAllTrivia()))
                                .WithSemicolonToken(default(SyntaxToken))
                                .WithBody(Block())
                                .WithFormatterAnnotation();

                            return context.Document.ReplaceNodeAsync(node, newNode, ct);
                        };
                    }
            }

            SyntaxDebug.Fail(node);

            return null;
        }
    }
}
