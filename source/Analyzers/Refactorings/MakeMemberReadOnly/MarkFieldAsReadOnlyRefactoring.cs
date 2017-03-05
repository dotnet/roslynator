// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Roslynator.Extensions;

namespace Roslynator.CSharp.Refactorings.MakeMemberReadOnly
{
    internal class MarkFieldAsReadOnlyRefactoring : MakeMemberReadOnlyRefactoring
    {
        private MarkFieldAsReadOnlyRefactoring()
        {
        }

        public static MarkFieldAsReadOnlyRefactoring Instance { get; } = new MarkFieldAsReadOnlyRefactoring();

        public override HashSet<ISymbol> GetAnalyzableSymbols(SymbolAnalysisContext context, INamedTypeSymbol containingType)
        {
            using (IEnumerator<IFieldSymbol> en = containingType
                .GetFields()
                .Where(f => !f.IsConst && f.IsPrivate() && !f.IsReadOnly && !f.IsImplicitlyDeclared)
                .GetEnumerator())
            {
                if (en.MoveNext())
                {
                    var fieldSymbols = new HashSet<ISymbol>() { en.Current };

                    while (en.MoveNext())
                        fieldSymbols.Add(en.Current);

                    return fieldSymbols;
                }
            }

            return null;
        }

        protected override bool ValidateSymbol(ISymbol symbol)
        {
            return symbol?.IsField() == true;
        }

        public override void ReportFixableSymbols(SymbolAnalysisContext context, INamedTypeSymbol containingType, HashSet<ISymbol> symbols)
        {
            foreach (IGrouping<VariableDeclarationSyntax, SyntaxNode> grouping in symbols
                .Select(f => f.GetFirstSyntax(context.CancellationToken))
                .GroupBy(f => (VariableDeclarationSyntax)f.Parent))
            {
                int count = grouping.Count();
                VariableDeclarationSyntax declaration = grouping.Key;

                int variablesCount = declaration.Variables.Count;

                if (variablesCount == 1
                    || variablesCount == count)
                {
                    context.ReportDiagnostic(
                        DiagnosticDescriptors.MarkFieldAsReadOnly,
                        declaration.Parent);
                }
            }
        }

        public static Task<Document> RefactorAsync(
            Document document,
            FieldDeclarationSyntax fieldDeclaration,
            CancellationToken cancellationToken)
        {
            FieldDeclarationSyntax newNode = Inserter.InsertModifier(fieldDeclaration, SyntaxKind.ReadOnlyKeyword);

            return document.ReplaceNodeAsync(fieldDeclaration, newNode, cancellationToken);
        }
    }
}
