// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

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
    public sealed class RemovePropertyOrFieldInitializerCodeFixProvider : CompilerDiagnosticCodeFixProvider
    {
        private const string Title = "Remove initializer";

        public override ImmutableArray<string> FixableDiagnosticIds
        {
            get
            {
                return ImmutableArray.Create(
                    CompilerDiagnosticIdentifiers.CS0573_CannotHaveInstancePropertyOrFieldInitializersInStruct,
                    CompilerDiagnosticIdentifiers.CS8050_OnlyAutoImplementedPropertiesCanHaveInitializers);
            }
        }

        public override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            Diagnostic diagnostic = context.Diagnostics[0];

            SyntaxNode root = await context.GetSyntaxRootAsync().ConfigureAwait(false);

            if (!IsEnabled(diagnostic.Id, CodeFixIdentifiers.RemovePropertyOrFieldInitializer, context.Document, root.SyntaxTree))
                return;

            if (!TryFindToken(root, context.Span.Start, out SyntaxToken token))
                return;

            SyntaxDebug.Assert(token.IsKind(SyntaxKind.IdentifierToken), token);

            if (!token.IsKind(SyntaxKind.IdentifierToken))
                return;

            switch (token.Parent)
            {
                case PropertyDeclarationSyntax propertyDeclaration:
                    {
                        EqualsValueClauseSyntax initializer = propertyDeclaration.Initializer;

                        CodeAction codeAction = CodeAction.Create(
                            Title,
                            ct =>
                            {
                                PropertyDeclarationSyntax newNode = propertyDeclaration
                                    .RemoveNode(initializer)
                                    .WithSemicolonToken(default(SyntaxToken))
                                    .AppendToTrailingTrivia(propertyDeclaration.SemicolonToken.GetAllTrivia())
                                    .WithFormatterAnnotation();

                                return context.Document.ReplaceNodeAsync(propertyDeclaration, newNode, ct);
                            },
                            GetEquivalenceKey(diagnostic, CodeFixIdentifiers.RemovePropertyOrFieldInitializer));

                        context.RegisterCodeFix(codeAction, diagnostic);
                        break;
                    }
                case VariableDeclaratorSyntax variableDeclarator:
                    {
                        EqualsValueClauseSyntax initializer = variableDeclarator.Initializer;

                        CodeAction codeAction = CodeAction.Create(
                            Title,
                            ct =>
                            {
                                VariableDeclaratorSyntax newNode = variableDeclarator
                                    .RemoveNode(initializer)
                                    .WithFormatterAnnotation();

                                return context.Document.ReplaceNodeAsync(variableDeclarator, newNode, ct);
                            },
                            GetEquivalenceKey(CompilerDiagnosticIdentifiers.CS0573_CannotHaveInstancePropertyOrFieldInitializersInStruct, CodeFixIdentifiers.RemovePropertyOrFieldInitializer));

                        context.RegisterCodeFix(codeAction, diagnostic);
                        break;
                    }
            }
        }
    }
}
