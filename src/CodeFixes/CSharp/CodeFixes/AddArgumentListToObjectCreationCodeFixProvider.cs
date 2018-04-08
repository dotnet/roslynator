// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Immutable;
using System.Composition;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using Roslynator.CodeFixes;

namespace Roslynator.CSharp.CodeFixes
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(AddArgumentListToObjectCreationCodeFixProvider))]
    [Shared]
    public class AddArgumentListToObjectCreationCodeFixProvider : BaseCodeFixProvider
    {
        public sealed override ImmutableArray<string> FixableDiagnosticIds
        {
            get { return ImmutableArray.Create(CompilerDiagnosticIdentifiers.NewExpressionRequiresParenthesesOrBracketsOrBracesAfterType); }
        }

        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            if (!Settings.IsCodeFixEnabled(CodeFixIdentifiers.AddArgumentList))
                return;

            SyntaxNode root = await context.GetSyntaxRootAsync().ConfigureAwait(false);

            if (!TryFindFirstAncestorOrSelf(root, new TextSpan(context.Span.Start - 1, 0), out ObjectCreationExpressionSyntax objectCreation))
                return;

            foreach (Diagnostic diagnostic in context.Diagnostics)
            {
                switch (diagnostic.Id)
                {
                    case CompilerDiagnosticIdentifiers.NewExpressionRequiresParenthesesOrBracketsOrBracesAfterType:
                        {
                            SemanticModel semanticModel = await context.GetSemanticModelAsync().ConfigureAwait(false);

                            ISymbol symbol = semanticModel.GetSymbol(objectCreation.Type, context.CancellationToken);

                            if (symbol?.IsErrorType() != false)
                                break;

                            CodeAction codeAction = CodeAction.Create(
                                "Add argument list",
                                cancellationToken =>
                                {
                                    ObjectCreationExpressionSyntax newNode = objectCreation.Update(
                                        objectCreation.NewKeyword,
                                        objectCreation.Type.WithoutTrailingTrivia(),
                                        SyntaxFactory.ArgumentList().WithTrailingTrivia(objectCreation.Type.GetTrailingTrivia()),
                                        objectCreation.Initializer);

                                    return context.Document.ReplaceNodeAsync(objectCreation, newNode, cancellationToken);
                                },
                                GetEquivalenceKey(diagnostic));

                            context.RegisterCodeFix(codeAction, diagnostic);
                            break;
                        }
                }
            }
        }
    }
}
