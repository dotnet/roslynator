// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Roslynator.CSharp.Analysis
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class MakeClassStaticAnalyzer : BaseDiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        {
            get { return ImmutableArray.Create(DiagnosticDescriptors.MakeClassStatic); }
        }

        public override void Initialize(AnalysisContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            base.Initialize(context);

            context.RegisterSymbolAction(AnalyzeNamedType, SymbolKind.NamedType);
        }

        public static void AnalyzeNamedType(SymbolAnalysisContext context)
        {
            var symbol = (INamedTypeSymbol)context.Symbol;

            if (symbol.TypeKind != TypeKind.Class)
                return;

            if (symbol.IsStatic)
                return;

            if (symbol.IsAbstract)
                return;

            if (symbol.IsImplicitClass)
                return;

            if (symbol.IsImplicitlyDeclared)
                return;

            if (symbol.BaseType?.IsObject() != true)
                return;

            var syntaxReferences = default(ImmutableArray<SyntaxReference>);

            if (symbol.IsSealed)
            {
                syntaxReferences = symbol.DeclaringSyntaxReferences;

                if (syntaxReferences.Length != 1)
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

            if (!members.Any())
                return false;

            bool areAllImplicitlyDeclared = true;

            foreach (ISymbol memberSymbol in members)
            {
                if (!memberSymbol.IsImplicitlyDeclared)
                    areAllImplicitlyDeclared = false;

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

            return !areAllImplicitlyDeclared;
        }
    }
}
