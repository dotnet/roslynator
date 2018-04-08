// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Roslynator.CSharp.Refactorings
{
    internal static class ImplementExceptionConstructorsRefactoring
    {
        public static async Task<Document> RefactorAsync(
            Document document,
            ClassDeclarationSyntax classDeclaration,
            CancellationToken cancellationToken)
        {
            SemanticModel semanticModel = await document.GetSemanticModelAsync(cancellationToken).ConfigureAwait(false);

            //List<IMethodSymbol> constructors = GenerateBaseConstructorsRefactoring.GetMissingBaseConstructors(classDeclaration, semanticModel, cancellationToken);

            List<IMethodSymbol> constructors = null;

            return await GenerateBaseConstructorsRefactoring.RefactorAsync(document, classDeclaration, constructors.ToArray(), semanticModel, cancellationToken).ConfigureAwait(false);
        }
    }
}
