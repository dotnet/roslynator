// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Roslynator.CSharp.Refactorings
{
    internal static class MakeClassStaticRefactoring
    {
        public static void Analyze(SymbolAnalysisContext context, INamedTypeSymbol symbol)
        {
            if (symbol.TypeKind != TypeKind.Class
                || symbol.IsStatic
                || symbol.IsAbstract
                || symbol.IsImplicitClass
                || symbol.IsImplicitlyDeclared
                || symbol.BaseType?.IsObject() != true)
            {
                return;
            }

            var syntaxReferences = default(ImmutableArray<SyntaxReference>);

            if (symbol.IsSealed
                && (syntaxReferences = symbol.DeclaringSyntaxReferences).Length != 1)
            {
                return;
            }

            if (!AnalyzeMembers(symbol))
                return;

            if (syntaxReferences.IsDefault)
                syntaxReferences = symbol.DeclaringSyntaxReferences;

            foreach (SyntaxReference syntaxReference in syntaxReferences)
            {
                var classDeclaration = (ClassDeclarationSyntax)syntaxReference.GetSyntax(context.CancellationToken);

                if (!classDeclaration.Modifiers.Contains(SyntaxKind.StaticKeyword))
                {
                    context.ReportDiagnostic(DiagnosticDescriptors.MakeClassStatic, classDeclaration.Identifier);
                    break;
                }
            }
        }

        public static bool AnalyzeMembers(INamedTypeSymbol symbol)
        {
            ImmutableArray<ISymbol> members = symbol.GetMembers();

            if (members.All(f => f.IsImplicitlyDeclared))
                return false;

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
                                        if (memberSymbol.DeclaredAccessibility.ContainsProtected())
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
                            Debug.Assert(memberSymbol.IsKind(SymbolKind.Event, SymbolKind.Field, SymbolKind.Method, SymbolKind.Property), memberSymbol.Kind.ToString());

                            if (memberSymbol.DeclaredAccessibility.ContainsProtected())
                                return false;

                            if (!memberSymbol.IsImplicitlyDeclared)
                            {
                                if (memberSymbol.IsStatic)
                                {
                                    if (memberSymbol.Kind == SymbolKind.Method
                                        && ((IMethodSymbol)memberSymbol).MethodKind.Is(MethodKind.UserDefinedOperator, MethodKind.Conversion))
                                    {
                                        return false;
                                    }
                                }
                                else
                                {
                                    return false;
                                }
                            }

                            break;
                        }
                }
            }

            return true;
        }
    }
}
