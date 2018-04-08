// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Collections.Immutable;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Roslynator.CSharp.Analysis.MakeMemberReadOnly
{
    internal abstract class MakeMemberReadOnlyAnalysis
    {
        public abstract HashSet<ISymbol> GetAnalyzableSymbols(
            SymbolAnalysisContext context,
            INamedTypeSymbol containingType);

        public abstract void ReportFixableSymbols(
            SymbolAnalysisContext context,
            INamedTypeSymbol containingType,
            HashSet<ISymbol> symbols);

        public virtual HashSet<ISymbol> GetFixableSymbols(
            SymbolAnalysisContext context,
            INamedTypeSymbol containingType,
            HashSet<ISymbol> symbols)
        {
            CancellationToken cancellationToken = context.CancellationToken;
            ImmutableArray<SyntaxReference> syntaxReferences = containingType.DeclaringSyntaxReferences;

            for (int i = 0; i < syntaxReferences.Length; i++)
            {
                var typeDeclaration = (TypeDeclarationSyntax)syntaxReferences[i].GetSyntax(cancellationToken);

                SemanticModel semanticModel = context.Compilation.GetSemanticModel(typeDeclaration.SyntaxTree);

                foreach (MemberDeclarationSyntax memberDeclaration in typeDeclaration.Members)
                {
                    MakeMemberReadOnlyWalker walker = MakeMemberReadOnlyWalkerCache.GetInstance();

                    walker.Visit(memberDeclaration);

                    HashSet<AssignedInfo> assigned = MakeMemberReadOnlyWalkerCache.GetAssignedAndFree(walker);

                    if (assigned != null)
                    {
                        foreach (AssignedInfo info in assigned)
                        {
                            foreach (ISymbol symbol in symbols)
                            {
                                if (symbol.Name == info.NameText
                                    && ((symbol.IsStatic) ? !info.IsInStaticConstructor : !info.IsInInstanceConstructor))
                                {
                                    ISymbol nameSymbol = semanticModel.GetSymbol(info.Name, cancellationToken);

                                    if (ValidateSymbol(nameSymbol)
                                        && symbols.Remove(nameSymbol.OriginalDefinition))
                                    {
                                        if (symbols.Count == 0)
                                            return symbols;

                                        break;
                                    }
                                }
                            }
                        }
                    }
                }
            }

            return symbols;
        }

        protected virtual bool ValidateSymbol(ISymbol symbol)
        {
            return symbol != null;
        }

        public void AnalyzeNamedType(SymbolAnalysisContext context)
        {
            var typeSymbol = (INamedTypeSymbol)context.Symbol;

            if (typeSymbol.TypeKind.Is(TypeKind.Class, TypeKind.Struct))
            {
                HashSet<ISymbol> symbols = GetAnalyzableSymbols(context, typeSymbol);

                if (symbols != null)
                {
                    symbols = GetFixableSymbols(context, typeSymbol, symbols);

                    if (symbols.Count > 0)
                        ReportFixableSymbols(context, typeSymbol, symbols);
                }
            }
        }
    }
}
