// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Options;
using Microsoft.CodeAnalysis.Rename;
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
                        string name = NameGenerator.CreateName(parameterSymbol.Type, firstCharToLower: true);

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
                string oldName = parameter.Identifier.ValueText;

                string newName = NameGenerator.Default.CreateUniqueParameterName(
                    oldName,
                    parameterSymbol,
                    semanticModel,
                    cancellationToken: context.CancellationToken);

                if (newName != null)
                {
                    context.RegisterRefactoring(
                        $"Rename '{oldName}' to '{newName}'",
                        cancellationToken => Renamer.RenameSymbolAsync(context.Solution, parameterSymbol, newName, default(OptionSet), cancellationToken));
                }
            }
        }

        private static Task<Document> AddParameterNameToParameterAsync(
            Document document,
            ParameterSyntax parameter,
            string name,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            ParameterSyntax newParameter = parameter
                .WithType(parameter.Type.WithoutTrailingTrivia())
                .WithIdentifier(SyntaxFactory.Identifier(name).WithTrailingTrivia(parameter.Type.GetTrailingTrivia()))
                .WithFormatterAnnotation();

            return document.ReplaceNodeAsync(parameter, newParameter, cancellationToken);
        }
    }
}
