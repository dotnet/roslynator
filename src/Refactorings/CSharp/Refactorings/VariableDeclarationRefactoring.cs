// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Linq;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Options;
using Microsoft.CodeAnalysis.Rename;
using Roslynator.CSharp.Analysis;

namespace Roslynator.CSharp.Refactorings
{
    internal static class VariableDeclarationRefactoring
    {
        public static async Task ComputeRefactoringsAsync(RefactoringContext context, VariableDeclarationSyntax variableDeclaration)
        {
            if (context.IsRefactoringEnabled(RefactoringDescriptors.RenameIdentifierAccordingToTypeName))
                await RenameVariableAccordingToTypeNameAsync(context, variableDeclaration).ConfigureAwait(false);

            await ChangeVariableDeclarationTypeRefactoring.ComputeRefactoringsAsync(context, variableDeclaration).ConfigureAwait(false);

            if (context.IsRefactoringEnabled(RefactoringDescriptors.CheckExpressionForNull))
                await CheckExpressionForNullRefactoring.ComputeRefactoringAsync(context, variableDeclaration).ConfigureAwait(false);

            if (context.IsRefactoringEnabled(RefactoringDescriptors.SplitVariableDeclaration)
                && SplitVariableDeclarationAnalysis.IsFixable(variableDeclaration))
            {
                context.RegisterRefactoring(
                    SplitVariableDeclarationRefactoring.GetTitle(variableDeclaration),
                    ct => SplitVariableDeclarationRefactoring.RefactorAsync(context.Document, variableDeclaration, ct),
                    RefactoringDescriptors.SplitVariableDeclaration);
            }
        }

        private static async Task RenameVariableAccordingToTypeNameAsync(
            RefactoringContext context,
            VariableDeclarationSyntax variableDeclaration)
        {
            TypeSyntax type = variableDeclaration.Type;

            if (type == null)
                return;

            if (variableDeclaration.IsParentKind(SyntaxKind.EventFieldDeclaration))
                return;

            VariableDeclaratorSyntax variable = variableDeclaration.Variables.SingleOrDefault(shouldThrow: false);

            if (variable == null)
                return;

            SyntaxToken identifier = variable.Identifier;

            if (!identifier.Span.Contains(context.Span))
                return;

            SemanticModel semanticModel = await context.GetSemanticModelAsync().ConfigureAwait(false);

            if (semanticModel.GetDeclaredSymbol(variable, context.CancellationToken) is not ILocalSymbol localSymbol)
                return;

            string oldName = identifier.ValueText;

            string newName = NameGenerator.Default.CreateUniqueLocalName(
                localSymbol.Type,
                oldName,
                semanticModel,
                variable.SpanStart,
                cancellationToken: context.CancellationToken);

            if (newName == null)
                return;

            context.RegisterRefactoring(
                $"Rename '{oldName}' to '{newName}'",
                ct => Renamer.RenameSymbolAsync(context.Solution, localSymbol, newName, default(OptionSet), ct),
                RefactoringDescriptors.RenameIdentifierAccordingToTypeName);
        }
    }
}
