// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Roslynator.CSharp;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static Roslynator.CSharp.CSharpFactory;

namespace Roslynator.CSharp.Refactorings
{
    internal static class UseGenericEventHandlerRefactoring
    {
        public static void AnalyzeEvent(SymbolAnalysisContext context)
        {
            var eventSymbol = (IEventSymbol)context.Symbol;

            if (!eventSymbol.IsOverride
                && eventSymbol.ExplicitInterfaceImplementations.IsDefaultOrEmpty
                && eventSymbol.FindImplementedInterfaceMember<IEventSymbol>(allInterfaces: true) == null)
            {
                var namedType = eventSymbol.Type as INamedTypeSymbol;

                if (namedType?.Arity == 0
                    && !namedType.Equals(context.GetTypeByMetadataName(MetadataNames.System_EventHandler)))
                {
                    IMethodSymbol method = namedType.DelegateInvokeMethod;

                    if (method != null)
                    {
                        ImmutableArray<IParameterSymbol> parameters = method.Parameters;

                        if (parameters.Length == 2
                            && parameters[0].Type.IsObject())
                        {
                            SyntaxNode node = eventSymbol
                                .DeclaringSyntaxReferences
                                .FirstOrDefault()?
                                .GetSyntax(context.CancellationToken);

                            TypeSyntax type = GetTypeSyntax(node);

                            Debug.Assert(type != null, "");

                            if (type != null)
                                context.ReportDiagnostic(DiagnosticDescriptors.UseGenericEventHandler, type);
                        }
                    }
                }
            }
        }

        private static TypeSyntax GetTypeSyntax(SyntaxNode node)
        {
            switch (node?.Kind())
            {
                case SyntaxKind.EventDeclaration:
                    {
                        return ((EventDeclarationSyntax)node).Type;
                    }
                case SyntaxKind.VariableDeclarator:
                    {
                        var declarator = (VariableDeclaratorSyntax)node;

                        SyntaxNode parent = declarator.Parent;

                        if (parent?.IsKind(SyntaxKind.VariableDeclaration) == true)
                        {
                            var declaration = (VariableDeclarationSyntax)parent;

                            return declaration.Type;
                        }

                        break;
                    }
            }

            Debug.Fail(node?.Kind().ToString());
            return default(TypeSyntax);
        }

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

        public static GenericNameSyntax CreateGenericEventHandler(TypeSyntax type, SemanticModel semanticModel, CancellationToken cancellationToken)
        {
            var delegateSymbol = (INamedTypeSymbol)semanticModel.GetTypeSymbol(type, cancellationToken);

            ITypeSymbol typeSymbol = delegateSymbol.DelegateInvokeMethod.Parameters[1].Type;

            INamedTypeSymbol eventHandlerSymbol = semanticModel.GetTypeByMetadataName(MetadataNames.System_EventHandler);

            return GenericName(
                Identifier(SymbolDisplay.GetMinimalString(eventHandlerSymbol, semanticModel, type.SpanStart)),
                typeSymbol.ToMinimalTypeSyntax(semanticModel, type.SpanStart));
        }
    }
}
