// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Immutable;
using System.Composition;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.CodeFixes;

namespace Roslynator.CSharp.CodeFixes
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(RemovePropertyOrFieldInitializerCodeFixProvider))]
    [Shared]
    public class RemovePropertyOrFieldInitializerCodeFixProvider : BaseCodeFixProvider
    {
        private const string Title = "Remove initializer";

        public sealed override ImmutableArray<string> FixableDiagnosticIds
        {
            get { return ImmutableArray.Create(CompilerDiagnosticIdentifiers.CannotHaveInstancePropertyOrFieldInitializersInStruct); }
        }

        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            if (!Settings.IsCodeFixEnabled(CodeFixIdentifiers.RemovePropertyOrFieldInitializer))
                return;

            SyntaxNode root = await context.GetSyntaxRootAsync().ConfigureAwait(false);

            if (!TryFindToken(root, context.Span.Start, out SyntaxToken token))
                return;

            Debug.Assert(token.Kind() == SyntaxKind.IdentifierToken, token.Kind().ToString());

            if (token.Kind() != SyntaxKind.IdentifierToken)
                return;

            SyntaxNode parent = token.Parent;

            switch (parent.Kind())
            {
                case SyntaxKind.PropertyDeclaration:
                    {
                        var propertyDeclaration = (PropertyDeclarationSyntax)parent;

                        EqualsValueClauseSyntax initializer = propertyDeclaration.Initializer;

                        CodeAction codeAction = CodeAction.Create(
                            Title,
                            cancellationToken =>
                            {
                                PropertyDeclarationSyntax newNode = propertyDeclaration
                                    .RemoveNode(initializer)
                                    .WithSemicolonToken(default(SyntaxToken))
                                    .AppendToTrailingTrivia(propertyDeclaration.SemicolonToken.GetAllTrivia())
                                    .WithFormatterAnnotation();

                                return context.Document.ReplaceNodeAsync(propertyDeclaration, newNode, cancellationToken);
                            },
                            GetEquivalenceKey(CompilerDiagnosticIdentifiers.CannotHaveInstancePropertyOrFieldInitializersInStruct, CodeFixIdentifiers.RemovePropertyOrFieldInitializer));

                        context.RegisterCodeFix(codeAction, context.Diagnostics);
                        break;
                    }
                case SyntaxKind.VariableDeclarator:
                    {
                        var variableDeclarator = (VariableDeclaratorSyntax)parent;
                        EqualsValueClauseSyntax initializer = variableDeclarator.Initializer;

                        CodeAction codeAction = CodeAction.Create(
                            Title,
                            cancellationToken =>
                            {
                                VariableDeclaratorSyntax newNode = variableDeclarator
                                    .RemoveNode(initializer)
                                    .WithFormatterAnnotation();

                                return context.Document.ReplaceNodeAsync(variableDeclarator, newNode, cancellationToken);
                            },
                            GetEquivalenceKey(CompilerDiagnosticIdentifiers.CannotHaveInstancePropertyOrFieldInitializersInStruct, CodeFixIdentifiers.RemovePropertyOrFieldInitializer));

                        context.RegisterCodeFix(codeAction, context.Diagnostics);
                        break;
                    }
            }
        }
    }
}
