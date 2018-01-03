// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
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
using Roslynator.CSharp.Helpers;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Roslynator.CSharp.CodeFixes
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(IdentifierNameCodeFixProvider))]
    [Shared]
    public class IdentifierNameCodeFixProvider : BaseCodeFixProvider
    {
        public sealed override ImmutableArray<string> FixableDiagnosticIds
        {
            get
            {
                return ImmutableArray.Create(
                    CompilerDiagnosticIdentifiers.UseOfUnassignedLocalVariable);
            }
        }

        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            if (!Settings.IsCodeFixEnabled(CodeFixIdentifiers.InitializeLocalVariableWithDefaultValue))
                return;

            SyntaxNode root = await context.GetSyntaxRootAsync().ConfigureAwait(false);

            if (!TryFindFirstAncestorOrSelf(root, context.Span, out IdentifierNameSyntax identifierName))
                return;

            foreach (Diagnostic diagnostic in context.Diagnostics)
            {
                switch (diagnostic.Id)
                {
                    case CompilerDiagnosticIdentifiers.UseOfUnassignedLocalVariable:
                        {
                            CodeAction codeAction = CodeAction.Create(
                                $"Initialize '{identifierName.Identifier.ValueText}' with default value",
                                cancellationToken => RefactorAsync(context.Document, identifierName, cancellationToken),
                                GetEquivalenceKey(diagnostic));

                            context.RegisterCodeFix(codeAction, diagnostic);
                            break;
                        }
                }
            }
        }

        public static async Task<Document> RefactorAsync(
            Document document,
            IdentifierNameSyntax identifierName,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            SemanticModel semanticModel = await document.GetSemanticModelAsync(cancellationToken).ConfigureAwait(false);

            var localSymbol = semanticModel.GetSymbol(identifierName, cancellationToken) as ILocalSymbol;

            if (localSymbol?.Type?.IsErrorType() == false
                && localSymbol.GetSyntax(cancellationToken) is VariableDeclaratorSyntax variableDeclarator)
            {
                SyntaxToken identifier = variableDeclarator.Identifier;

                var variableDeclaration = (VariableDeclarationSyntax)variableDeclarator.Parent;

                ExpressionSyntax value = localSymbol.Type.ToDefaultValueSyntax(variableDeclaration.Type.WithoutTrivia());

                EqualsValueClauseSyntax newEqualsValue = EqualsValueClause(value)
                    .WithLeadingTrivia(TriviaList(Space))
                    .WithTrailingTrivia(identifier.TrailingTrivia);

                VariableDeclaratorSyntax newNode = variableDeclarator
                    .WithInitializer(newEqualsValue)
                    .WithIdentifier(identifier.WithoutTrailingTrivia());

                return await document.ReplaceNodeAsync(variableDeclarator, newNode, cancellationToken).ConfigureAwait(false);
            }

            Debug.Assert(false, identifierName.ToString());

            return document;
        }
    }
}
