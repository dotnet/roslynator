// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeRefactorings;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Pihrtsoft.CodeAnalysis.CSharp.Refactoring
{
    public static class RenameParameterAccordingToTypeNameRefactoring
    {
        public static void Refactor(
            CodeRefactoringContext context,
            ParameterSyntax parameter,
            SemanticModel semanticModel)
        {
            if (parameter == null)
                throw new ArgumentNullException(nameof(parameter));

            if (semanticModel == null)
                throw new ArgumentNullException(nameof(semanticModel));

            if (parameter.Type == null)
                return;

            if (!parameter.Identifier.Span.Contains(context.Span))
                return;

            string newName = NamingHelper.CreateIdentifierName(
                parameter.Type,
                semanticModel,
                firstCharToLower: true);

            if (string.Equals(parameter.Identifier.ToString(), newName, StringComparison.Ordinal))
                return;

            ISymbol symbol = semanticModel.GetDeclaredSymbol(parameter, context.CancellationToken);

            string title = (parameter.Identifier.IsMissing)
                ? $"Add parameter name '{newName}'"
                : $"Rename parameter to '{newName}'";

            context.RegisterRefactoring(
                title,
                cancellationToken => symbol.RenameAsync(newName, context.Document, cancellationToken));
        }
    }
}
