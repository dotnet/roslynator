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

namespace Roslynator.CSharp.Refactorings
{
    internal static class MarkClassAsStaticRefactoring
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
                            DiagnosticDescriptors.MarkClassAsStatic,
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

        public static Task<Document> RefactorAsync(
            Document document,
            ClassDeclarationSyntax classDeclaration,
            CancellationToken cancellationToken)
        {
            return InsertModifierRefactoring.RefactorAsync(document, classDeclaration, SyntaxKind.StaticKeyword, cancellationToken);
        }
    }
}
