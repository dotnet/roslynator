// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Roslynator.CSharp.Refactorings
{
    internal static class MakeClassStaticRefactoring
    {
        public static void Analyze(SymbolAnalysisContext context, INamedTypeSymbol symbol)
        {
            if (symbol.IsClass()
                && !symbol.IsStatic
                && !symbol.IsAbstract
                && !symbol.IsImplicitClass
                && !symbol.IsImplicitlyDeclared
                && symbol.BaseType?.IsObject() == true)
            {
                var syntaxReferences = default(ImmutableArray<SyntaxReference>);

                if (!symbol.IsSealed
                    || (syntaxReferences = symbol.DeclaringSyntaxReferences).Length == 1)
                {
                    if (AnalyzeMembers(symbol))
                    {
                        if (syntaxReferences.IsDefault)
                            syntaxReferences = symbol.DeclaringSyntaxReferences;

                        foreach (SyntaxReference syntaxReference in syntaxReferences)
                        {
                            var classDeclaration = (ClassDeclarationSyntax)syntaxReference.GetSyntax(context.CancellationToken);

                            if (!classDeclaration.IsStatic())
                            {
                                context.ReportDiagnostic(DiagnosticDescriptors.MakeClassStatic, classDeclaration.Identifier);
                                break;
                            }
                        }
                    }
                }
            }
        }

        public static bool AnalyzeMembers(INamedTypeSymbol symbol)
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

            return false;
        }
    }
}
