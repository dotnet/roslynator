// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Options;
using Microsoft.CodeAnalysis.Rename;

namespace Roslynator.CSharp.Refactorings
{
    internal static class DeclarationPatternRefactoring
    {
        internal static async Task ComputeRefactoringAsync(RefactoringContext context, DeclarationPatternSyntax declarationPattern)
        {
            if (context.IsRefactoringEnabled(RefactoringIdentifiers.RenameIdentifierAccordingToTypeName))
            {
                VariableDesignationSyntax designation = declarationPattern.Designation;

                if (designation?.Kind() == SyntaxKind.SingleVariableDesignation)
                {
                    var singleVariableDesignation = (SingleVariableDesignationSyntax)designation;

                    SyntaxToken identifier = singleVariableDesignation.Identifier;

                    if (identifier.Span.Contains(context.Span))
                    {
                        SemanticModel semanticModel = await context.GetSemanticModelAsync().ConfigureAwait(false);

                        ISymbol symbol = semanticModel.GetDeclaredSymbol(singleVariableDesignation, context.CancellationToken);

                        if (symbol?.Kind == SymbolKind.Local)
                        {
                            var localSymbol = (ILocalSymbol)symbol;

                            string oldName = identifier.ValueText;

                            string newName = NameGenerator.Default.CreateUniqueLocalName(
                                localSymbol.Type,
                                oldName,
                                semanticModel,
                                singleVariableDesignation.SpanStart,
                                cancellationToken: context.CancellationToken);

                            if (newName != null)
                            {
                                context.RegisterRefactoring(
                                    $"Rename '{oldName}' to '{newName}'",
                                    cancellationToken => Renamer.RenameSymbolAsync(context.Solution, symbol, newName, default(OptionSet), cancellationToken));
                            }
                        }
                    }
                }
            }
        }
    }
}