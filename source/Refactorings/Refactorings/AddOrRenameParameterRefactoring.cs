// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace Roslynator.CSharp.Refactorings
{
    internal static class AddOrRenameParameterRefactoring
    {
        public static async Task ComputeRefactoringsAsync(RefactoringContext context, ParameterSyntax parameter)
        {
            if (!context.IsAnyRefactoringEnabled(
                RefactoringIdentifiers.AddParameterNameToParameter,
                RefactoringIdentifiers.RenameParameterAccordingToTypeName))
            {
                return;
            }

            SemanticModel semanticModel = await context.GetSemanticModelAsync().ConfigureAwait(false);

            IParameterSymbol parameterSymbol = semanticModel.GetDeclaredSymbol(parameter, context.CancellationToken);

            if (parameterSymbol?.Type == null)
                return;

            if (parameter.Identifier.IsMissing)
            {
                if (context.IsRefactoringEnabled(RefactoringIdentifiers.AddParameterNameToParameter))
                {
                    TextSpan span = (parameter.Type != null)
                        ? TextSpan.FromBounds(parameter.Type.Span.End, parameter.Span.End)
                        : parameter.Span;

                    if (span.Contains(context.Span))
                    {
                        string name = SyntaxUtility.CreateIdentifier(parameterSymbol.Type, firstCharToLower: true);

                        if (!string.IsNullOrEmpty(name))
                        {
                            context.RegisterRefactoring(
                                $"Add parameter name '{name}'",
                                cancellationToken => AddParameterNameToParameterAsync(context.Document, parameter, name, cancellationToken));
                        }
                    }
                }
            }
            else if (context.IsRefactoringEnabled(RefactoringIdentifiers.RenameParameterAccordingToTypeName)
                && parameter.Identifier.Span.Contains(context.Span))
            {
                string name = parameter.Identifier.ValueText;
                string newName = SyntaxUtility.CreateIdentifier(parameterSymbol.Type, firstCharToLower: true);

                if (!string.IsNullOrEmpty(newName)
                    && !string.Equals(name, newName, StringComparison.Ordinal))
                {
                    ISymbol symbol = semanticModel.GetDeclaredSymbol(parameter, context.CancellationToken);

                    context.RegisterRefactoring(
                        $"Rename parameter to '{newName}'",
                        cancellationToken => SymbolRenamer.RenameAsync(context.Document, symbol, newName, cancellationToken));
                }
            }
        }

        private static async Task<Document> AddParameterNameToParameterAsync(
            Document document,
            ParameterSyntax parameter,
            string name,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            SyntaxNode oldRoot = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);

            ParameterSyntax newParameter = parameter
                .WithType(parameter.Type.WithoutTrailingTrivia())
                .WithIdentifier(SyntaxFactory.Identifier(name).WithTrailingTrivia(parameter.Type.GetTrailingTrivia()))
                .WithFormatterAnnotation();

            SyntaxNode newRoot = oldRoot.ReplaceNode(parameter, newParameter);

            return document.WithSyntaxRoot(newRoot);
        }
    }
}
