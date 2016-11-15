// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Roslynator.CSharp.CSharpFactory;

namespace Roslynator.CSharp.Refactorings
{
    internal static class MarkClassAsStaticRefactoring
    {
        public static bool CanRefactor(INamedTypeSymbol symbol)
        {
            if (symbol.TypeKind == TypeKind.Class
                && !symbol.IsStatic
                && !symbol.IsImplicitClass
                && !symbol.IsImplicitlyDeclared)
            {
                ImmutableArray<ISymbol> members = symbol.GetMembers();

                if (members.Any(f => !f.IsImplicitlyDeclared))
                {
                    foreach (ISymbol memberSymbol in members)
                    {
                        switch (memberSymbol.Kind)
                        {
                            case SymbolKind.ErrorType:
                                {
                                    return false;
                                }
                            case SymbolKind.NamedType:
                                {
                                    var namedTypeSymbol = (INamedTypeSymbol)memberSymbol;

                                    switch (namedTypeSymbol.TypeKind)
                                    {
                                        case TypeKind.Unknown:
                                        case TypeKind.Error:
                                            return false;
#if DEBUG
                                        case TypeKind.Class:
                                        case TypeKind.Delegate:
                                        case TypeKind.Enum:
                                        case TypeKind.Interface:
                                        case TypeKind.Struct:
                                            break;
                                        default:
                                            {
                                                Debug.Assert(false, namedTypeSymbol.TypeKind.ToString());
                                                break;
                                            }
#endif
                                    }

                                    break;
                                }
                            default:
                                {
                                    if (!memberSymbol.IsImplicitlyDeclared
                                        && !memberSymbol.IsStatic)
                                    {
                                        return false;
                                    }

                                    break;
                                }
                        }
                    }

                    return true;
                }
            }

            return false;
        }

        public static async Task<Document> RefactorAsync(
            Document document,
            ClassDeclarationSyntax classDeclaration,
            CancellationToken cancellationToken)
        {
            SyntaxNode root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);

            SyntaxTokenList modifiers = classDeclaration.Modifiers;

            ClassDeclarationSyntax newClassDeclaration = classDeclaration;

            if (modifiers.Any())
            {
                int partialIndex = modifiers.IndexOf(SyntaxKind.PartialKeyword);

                if (partialIndex != -1)
                {
                    SyntaxToken partialToken = modifiers[partialIndex];

                    modifiers = modifiers
                        .ReplaceAt(partialIndex, partialToken.WithoutLeadingTrivia())
                        .Insert(
                            partialIndex,
                            StaticToken()
                                .WithLeadingTrivia(partialToken.LeadingTrivia)
                                .WithTrailingTrivia(SpaceTrivia()));

                    newClassDeclaration = classDeclaration.WithModifiers(modifiers);
                }
                else
                {
                    newClassDeclaration = classDeclaration
                        .AddModifiers(StaticToken().WithLeadingTrivia(SpaceTrivia()));
                }
            }
            else
            {
                newClassDeclaration = classDeclaration.AddModifiers(StaticToken());
            }

            SyntaxNode newRoot = root.ReplaceNode(classDeclaration, newClassDeclaration);

            return document.WithSyntaxRoot(newRoot);
        }
    }
}
