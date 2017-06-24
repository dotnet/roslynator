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
using Roslynator.CSharp.Comparers;

namespace Roslynator.CSharp.Refactorings
{
    internal static class MakeClassStaticRefactoring
    {
        public static void Analyze(SymbolAnalysisContext context, INamedTypeSymbol symbol)
        {
            if (CanRefactor(symbol))
            {
                foreach (SyntaxReference syntaxReference in symbol.DeclaringSyntaxReferences)
                {
                    var classDeclaration = syntaxReference.GetSyntax(context.CancellationToken) as ClassDeclarationSyntax;

                    if (classDeclaration?.IsStatic() == false)
                    {
                        context.ReportDiagnostic(
                            DiagnosticDescriptors.MakeClassStatic,
                            classDeclaration.Identifier);

                        break;
                    }
                }
            }
        }

        public static bool CanRefactor(INamedTypeSymbol symbol)
        {
            if (symbol.IsClass()
                && !symbol.IsStatic
                && !symbol.IsAbstract
                && !symbol.IsImplicitClass
                && !symbol.IsImplicitlyDeclared
                && symbol.BaseType?.IsObject() == true)
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
                                            {
                                                return false;
                                            }
                                        case TypeKind.Class:
                                        case TypeKind.Delegate:
                                        case TypeKind.Enum:
                                        case TypeKind.Interface:
                                        case TypeKind.Struct:
                                            {
                                                if (memberSymbol.IsDeclaredAccessibility(Accessibility.Protected, Accessibility.ProtectedOrInternal))
                                                    return false;

                                                break;
                                            }
                                        default:
                                            {
                                                Debug.Fail(namedTypeSymbol.TypeKind.ToString());
                                                break;
                                            }
                                    }

                                    break;
                                }
                            default:
                                {
                                    if (memberSymbol.IsDeclaredAccessibility(Accessibility.Protected, Accessibility.ProtectedOrInternal))
                                        return false;

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

        public static Task<Document> RefactorAsync(
            Document document,
            ClassDeclarationSyntax classDeclaration,
            CancellationToken cancellationToken)
        {
            ClassDeclarationSyntax newNode = UpdateModifiers(classDeclaration);

            return document.ReplaceNodeAsync(classDeclaration, newNode, cancellationToken);
        }

        public static async Task<Solution> RefactorAsync(
            Solution solution,
            ImmutableArray<ClassDeclarationSyntax> classDeclarations,
            CancellationToken cancellationToken)
        {
            var newDocuments = new List<KeyValuePair<DocumentId, SyntaxNode>>();

            foreach (SyntaxTree syntaxTree in classDeclarations.Select(f => f.SyntaxTree).Distinct())
            {
                Document document = solution.GetDocument(syntaxTree);

                SyntaxNode root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);

                SyntaxNode newRoot = root.ReplaceNodes(
                    classDeclarations.Where(f => f.SyntaxTree == syntaxTree),
                    (node, rewrittenNode) => UpdateModifiers(node));

                newDocuments.Add(new KeyValuePair<DocumentId, SyntaxNode>(document.Id, newRoot));
            }

            Solution newSolution = solution;

            foreach (KeyValuePair<DocumentId, SyntaxNode> kvp in newDocuments)
                newSolution = newSolution.WithDocumentSyntaxRoot(kvp.Key, kvp.Value);

            return newSolution;
        }

        private static ClassDeclarationSyntax UpdateModifiers(ClassDeclarationSyntax classDeclaration)
        {
            return classDeclaration
                .InsertModifier(SyntaxKind.StaticKeyword, ModifierComparer.Instance)
                .RemoveModifier(SyntaxKind.SealedKeyword);
        }
    }
}
