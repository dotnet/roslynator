// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Options;
using Microsoft.CodeAnalysis.Rename;

namespace Roslynator.CSharp.Refactorings
{
    internal static class DeclarationExpressionRefactoring
    {
        public static async Task ComputeRefactoringsAsync(RefactoringContext context, DeclarationExpressionSyntax declarationExpression)
        {
            if (context.IsRefactoringEnabled(RefactoringIdentifiers.RenameIdentifierAccordingToTypeName))
                await RenameVariableAccordingToTypeNameAsync(context, declarationExpression).ConfigureAwait(false);

            await ChangeDeclarationExpressionTypeRefactoring.ComputeRefactoringsAsync(context, declarationExpression).ConfigureAwait(false);
        }

        private static async Task RenameVariableAccordingToTypeNameAsync(
            RefactoringContext context,
            DeclarationExpressionSyntax declarationExpression)
        {
            TypeSyntax type = declarationExpression.Type;

            if (type == null)
                return;

            VariableDesignationSyntax designation = declarationExpression.Designation;

            if (designation?.Kind() != SyntaxKind.SingleVariableDesignation)
                return;

            var singleVariableDesignation = (SingleVariableDesignationSyntax)designation;

            SyntaxToken identifier = singleVariableDesignation.Identifier;

            if (!identifier.Span.Contains(context.Span))
                return;

            SemanticModel semanticModel = await context.GetSemanticModelAsync().ConfigureAwait(false);

            var localSymbol = semanticModel.GetDeclaredSymbol(singleVariableDesignation, context.CancellationToken) as ILocalSymbol;

            if (localSymbol?.IsErrorType() != false)
                return;

            string oldName = identifier.ValueText;

            string newName = NameGenerator.Default.CreateUniqueLocalName(
                localSymbol.Type,
                oldName,
                semanticModel,
                singleVariableDesignation.SpanStart,
                cancellationToken: context.CancellationToken);

            if (newName == null)
                return;

            context.RegisterRefactoring(
                $"Rename '{oldName}' to '{newName}'",
                cancellationToken => Renamer.RenameSymbolAsync(context.Solution, localSymbol, newName, default(OptionSet), cancellationToken));
        }
   }
}