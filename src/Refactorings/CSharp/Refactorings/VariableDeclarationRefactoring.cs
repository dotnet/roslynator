// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

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
            if (context.IsRefactoringEnabled(RefactoringIdentifiers.RenameIdentifierAccordingToTypeName))
                await RenameVariableAccordingToTypeNameAsync(context, variableDeclaration).ConfigureAwait(false);

            await ChangeVariableDeclarationTypeRefactoring.ComputeRefactoringsAsync(context, variableDeclaration).ConfigureAwait(false);

            if (context.IsAnyRefactoringEnabled(RefactoringIdentifiers.AddCastExpression, RefactoringIdentifiers.CallToMethod))
                await AddCastExpressionAsync(context, variableDeclaration).ConfigureAwait(false);

            if (context.IsRefactoringEnabled(RefactoringIdentifiers.WrapInUsingStatement))
                await WrapInUsingStatementRefactoring.ComputeRefactoringAsync(context, variableDeclaration).ConfigureAwait(false);

            if (context.IsRefactoringEnabled(RefactoringIdentifiers.CheckExpressionForNull))
                await CheckExpressionForNullRefactoring.ComputeRefactoringAsync(context, variableDeclaration).ConfigureAwait(false);

            if (context.IsRefactoringEnabled(RefactoringIdentifiers.SplitVariableDeclaration)
                && SplitVariableDeclarationAnalysis.IsFixable(variableDeclaration))
            {
                context.RegisterRefactoring(
                    SplitVariableDeclarationRefactoring.GetTitle(variableDeclaration),
                    cancellationToken => SplitVariableDeclarationRefactoring.RefactorAsync(context.Document, variableDeclaration, cancellationToken));
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

            if (!(semanticModel.GetDeclaredSymbol(variable, context.CancellationToken) is ILocalSymbol localSymbol))
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
                cancellationToken => Renamer.RenameSymbolAsync(context.Solution, localSymbol, newName, default(OptionSet), cancellationToken));
        }

        private static async Task AddCastExpressionAsync(
            RefactoringContext context,
            VariableDeclarationSyntax variableDeclaration)
        {
            if (variableDeclaration.Type?.IsVar != false)
                return;

            VariableDeclaratorSyntax declarator = variableDeclaration
                .Variables
                .FirstOrDefault(f => f.Initializer?.Value?.Span.Contains(context.Span) == true);

            if (declarator == null)
                return;

            SemanticModel semanticModel = await context.GetSemanticModelAsync().ConfigureAwait(false);

            ITypeSymbol declarationType = semanticModel.GetTypeSymbol(variableDeclaration.Type, context.CancellationToken);

            if (declarationType?.IsErrorType() != false)
                return;

            ITypeSymbol expressionType = semanticModel.GetTypeSymbol(declarator.Initializer.Value, context.CancellationToken);

            if (expressionType?.IsErrorType() != false)
                return;

            if (declarationType.Equals(expressionType))
                return;

            ModifyExpressionRefactoring.ComputeRefactoring(context, declarator.Initializer.Value, declarationType, semanticModel);
        }
    }
}