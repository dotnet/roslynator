// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Formatting;
using Microsoft.CodeAnalysis.Text;

namespace Pihrtsoft.CodeAnalysis.CSharp.Refactoring
{
    internal static class AddOrRenameParameterRefactoring
    {
        public static async Task ComputeRefactoringsAsync(RefactoringContext context, ParameterSyntax parameter)
        {
            if (!context.Settings.IsAnyRefactoringEnabled(
                RefactoringIdentifiers.AddParameterNameToParameter,
                RefactoringIdentifiers.RenameParameterAccordingToTypeName))
            {
                return;
            }

            SemanticModel semanticModel = await context.GetSemanticModelAsync();

            IParameterSymbol parameterSymbol = semanticModel.GetDeclaredSymbol(parameter, context.CancellationToken);

            if (parameterSymbol?.Type == null)
                return;

            if (parameter.Identifier.IsMissing)
            {
                if (context.Settings.IsRefactoringEnabled(RefactoringIdentifiers.AddParameterNameToParameter))
                {
                    TextSpan span = (parameter.Type != null)
                        ? TextSpan.FromBounds(parameter.Type.Span.End, parameter.Span.End)
                        : parameter.Span;

                    if (span.Contains(context.Span))
                    {
                        string name = NamingHelper.CreateIdentifierName(parameterSymbol.Type, firstCharToLower: true);

                        if (!string.IsNullOrEmpty(name))
                        {
                            context.RegisterRefactoring(
                                $"Add parameter name '{name}'",
                                cancellationToken => AddParameterNameToParameterAsync(context.Document, parameter, name, cancellationToken));
                        }
                    }
                }
            }
            else if (context.Settings.IsRefactoringEnabled(RefactoringIdentifiers.RenameParameterAccordingToTypeName)
                && parameter.Identifier.Span.Contains(context.Span))
            {
                string name = NamingHelper.CreateIdentifierName(parameterSymbol.Type, firstCharToLower: true);

                if (!string.IsNullOrEmpty(name)
                    && !string.Equals(name, parameter.Identifier.ValueText, StringComparison.Ordinal))
                {
                    ISymbol symbol = semanticModel.GetDeclaredSymbol(parameter, context.CancellationToken);

                    context.RegisterRefactoring(
                        $"Rename parameter to '{name}'",
                        cancellationToken => symbol.RenameAsync(name, context.Document, cancellationToken));
                }
            }
        }

        private static async Task<Document> AddParameterNameToParameterAsync(
            Document document,
            ParameterSyntax parameter,
            string name,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            SyntaxNode oldRoot = await document.GetSyntaxRootAsync(cancellationToken);

            ParameterSyntax newParameter = parameter
                .WithType(parameter.Type.WithoutTrailingTrivia())
                .WithIdentifier(SyntaxFactory.Identifier(name).WithTrailingTrivia(parameter.Type.GetTrailingTrivia()))
                .WithAdditionalAnnotations(Formatter.Annotation);

            SyntaxNode newRoot = oldRoot.ReplaceNode(parameter, newParameter);

            return document.WithSyntaxRoot(newRoot);
        }
    }
}
