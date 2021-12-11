// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

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
            if (!context.IsRefactoringEnabled(RefactoringDescriptors.RenameParameterAccordingToTypeName))
            {
                return;
            }

            SemanticModel semanticModel = await context.GetSemanticModelAsync().ConfigureAwait(false);

            IParameterSymbol parameterSymbol = semanticModel.GetDeclaredSymbol(parameter, context.CancellationToken);

            if (parameterSymbol?.Type == null)
                return;

            if (!parameter.Identifier.IsMissing
                && context.IsRefactoringEnabled(RefactoringDescriptors.RenameParameterAccordingToTypeName)
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
                        ct => Renamer.RenameSymbolAsync(context.Solution, parameterSymbol, newName, default(OptionSet), ct),
                        RefactoringDescriptors.RenameParameterAccordingToTypeName);
                }
            }
        }

        private static Task<Document> AddParameterNameToParameterAsync(
            Document document,
            ParameterSyntax parameter,
            string name,
            CancellationToken cancellationToken = default)
        {
            ParameterSyntax newParameter = parameter
                .WithType(parameter.Type.WithoutTrailingTrivia())
                .WithIdentifier(SyntaxFactory.Identifier(name).WithTrailingTrivia(parameter.Type.GetTrailingTrivia()))
                .WithFormatterAnnotation();

            return document.ReplaceNodeAsync(parameter, newParameter, cancellationToken);
        }
    }
}
