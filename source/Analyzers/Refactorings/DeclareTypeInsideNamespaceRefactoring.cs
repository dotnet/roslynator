// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Roslynator.CSharp.Extensions;
using Roslynator.Extensions;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Roslynator.CSharp.Refactorings
{
    internal static class DeclareTypeInsideNamespaceRefactoring
    {
        public static void Analyze(SymbolAnalysisContext context, INamedTypeSymbol symbol)
        {
            if (symbol.ContainingNamespace?.IsGlobalNamespace == true)
            {
                ImmutableArray<SyntaxReference> syntaxReferences = symbol.DeclaringSyntaxReferences;

                foreach (SyntaxReference syntaxReference in syntaxReferences)
                {
                    SyntaxNode node = syntaxReference.GetSyntax(context.CancellationToken);

                    if (node != null)
                    {
                        SyntaxToken identifier = GetDeclarationIdentifier(symbol, node);

                        if (!identifier.IsKind(SyntaxKind.None))
                        {
                            context.ReportDiagnostic(
                                DiagnosticDescriptors.DeclareTypeInsideNamespace,
                                identifier,
                                identifier.ValueText);
                        }
                    }
                }
            }
        }

        private static SyntaxToken GetDeclarationIdentifier(INamedTypeSymbol symbol, SyntaxNode node)
        {
            switch (symbol.TypeKind)
            {
                case TypeKind.Class:
                    return ((ClassDeclarationSyntax)node).Identifier;
                case TypeKind.Struct:
                    return ((StructDeclarationSyntax)node).Identifier;
                case TypeKind.Interface:
                    return ((InterfaceDeclarationSyntax)node).Identifier;
                case TypeKind.Delegate:
                    return ((DelegateDeclarationSyntax)node).Identifier;
                case TypeKind.Enum:
                    return ((EnumDeclarationSyntax)node).Identifier;
                default:
                    {
                        Debug.Assert(false, symbol.TypeKind.ToString());
                        return default(SyntaxToken);
                    }
            }
        }

        public static async Task<Document> RefactorAsync(
            Document document,
            MemberDeclarationSyntax member,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            Debug.Assert(member.IsParentKind(SyntaxKind.CompilationUnit), member.Parent?.Kind().ToString());

            if (member.IsParentKind(SyntaxKind.CompilationUnit))
            {
                SemanticModel semanticModel = await document.GetSemanticModelAsync(cancellationToken).ConfigureAwait(false);

                IEnumerable<string> reservedNames = semanticModel
                    .LookupNamespacesAndTypes(member.SpanStart)
                    .Select(f => f.Name);

                string name = Identifier.EnsureUniqueName("Namespace", reservedNames);

                NamespaceDeclarationSyntax namespaceDeclaration = NamespaceDeclaration(
                    IdentifierName(Identifier(name).WithRenameAnnotation()),
                    default(SyntaxList<ExternAliasDirectiveSyntax>),
                    default(SyntaxList<UsingDirectiveSyntax>),
                    SingletonList(member));

                return await document.ReplaceNodeAsync(member, namespaceDeclaration.WithFormatterAnnotation(), cancellationToken).ConfigureAwait(false);
            }

            return document;
        }
    }
}
