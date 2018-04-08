// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static Roslynator.CSharp.CSharpFactory;

namespace Roslynator.CSharp.Refactorings
{
    internal static class UseGenericEventHandlerRefactoring
    {
        public static async Task<Document> RefactorAsync(
            Document document,
            TypeSyntax type,
            CancellationToken cancellationToken)
        {
            SemanticModel semanticModel = await document.GetSemanticModelAsync(cancellationToken).ConfigureAwait(false);

            GenericNameSyntax newNode = CreateGenericEventHandler(type, semanticModel, cancellationToken);

            newNode = newNode.WithTriviaFrom(type);

            return await document.ReplaceNodeAsync(type, newNode, cancellationToken).ConfigureAwait(false);
        }

        internal static GenericNameSyntax CreateGenericEventHandler(TypeSyntax type, SemanticModel semanticModel, CancellationToken cancellationToken)
        {
            var delegateSymbol = (INamedTypeSymbol)semanticModel.GetTypeSymbol(type, cancellationToken);

            ITypeSymbol typeSymbol = delegateSymbol.DelegateInvokeMethod.Parameters[1].Type;

            INamedTypeSymbol eventHandlerSymbol = semanticModel.GetTypeByMetadataName(MetadataNames.System_EventHandler);

            return GenericName(
                Identifier(SymbolDisplay.ToMinimalDisplayString(eventHandlerSymbol, semanticModel, type.SpanStart, SymbolDisplayFormats.Default)),
                typeSymbol.ToMinimalTypeSyntax(semanticModel, type.SpanStart));
        }
    }
}
