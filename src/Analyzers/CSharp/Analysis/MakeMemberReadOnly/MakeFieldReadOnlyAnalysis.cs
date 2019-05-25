// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Roslynator.CSharp.Analysis.MakeMemberReadOnly
{
    internal sealed class MakeFieldReadOnlyAnalysis : MakeMemberReadOnlyAnalysis
    {
        private MakeFieldReadOnlyAnalysis()
        {
        }

        public static MakeFieldReadOnlyAnalysis Instance { get; } = new MakeFieldReadOnlyAnalysis();

        public override HashSet<ISymbol> GetAnalyzableSymbols(SymbolAnalysisContext context, INamedTypeSymbol containingType)
        {
            HashSet<ISymbol> analyzableFields = null;

            foreach (ISymbol member in containingType.GetMembers())
            {
                if (member.Kind == SymbolKind.Field)
                {
                    var fieldSymbol = (IFieldSymbol)member;

                    if (!fieldSymbol.IsConst
                        && fieldSymbol.DeclaredAccessibility == Accessibility.Private
                        && !fieldSymbol.IsReadOnly
                        && !fieldSymbol.IsVolatile
                        && !fieldSymbol.IsImplicitlyDeclared
                        && MakeMemberReadOnlyHelpers.ValidateType(fieldSymbol.Type))
                    {
                        (analyzableFields ?? (analyzableFields = new HashSet<ISymbol>())).Add(fieldSymbol);
                    }
                }
            }

            return analyzableFields;
        }

        protected override bool ValidateSymbol(ISymbol symbol)
        {
            return symbol?.Kind == SymbolKind.Field;
        }

        public override void ReportFixableSymbols(SymbolAnalysisContext context, INamedTypeSymbol containingType, HashSet<ISymbol> symbols)
        {
            foreach (IGrouping<VariableDeclarationSyntax, SyntaxNode> grouping in symbols
                .Select(f => f.GetSyntax(context.CancellationToken))
                .GroupBy(f => (VariableDeclarationSyntax)f.Parent))
            {
                int count = grouping.Count();
                VariableDeclarationSyntax declaration = grouping.Key;

                int variablesCount = declaration.Variables.Count;

                if (variablesCount == 1
                    || variablesCount == count)
                {
                    DiagnosticHelpers.ReportDiagnostic(context,
                        DiagnosticDescriptors.MakeFieldReadOnly,
                        declaration.Parent);
                }
            }
        }
    }
}
