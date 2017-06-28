// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Roslynator.CSharp.Comparers;

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
            HashSet<ISymbol> analyzableFields = null;

            foreach (ISymbol member in containingType.GetMembers())
            {
                if (member.IsField())
                {
                    var fieldSymbol = (IFieldSymbol)member;

                    if (!fieldSymbol.IsConst
                        && fieldSymbol.IsPrivate()
                        && !fieldSymbol.IsReadOnly
                        && !fieldSymbol.IsVolatile
                        && !fieldSymbol.IsImplicitlyDeclared
                        && (fieldSymbol.Type.IsReferenceType
                            || fieldSymbol.Type.IsPredefinedValueType()
                            || fieldSymbol.Type.IsEnum()))
                    {
                        (analyzableFields ?? (analyzableFields = new HashSet<ISymbol>())).Add(fieldSymbol);
                    }
                }
            }

            return analyzableFields;
        }

        protected override bool ValidateSymbol(ISymbol symbol)
        {
            return symbol?.IsField() == true;
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
            return document.InsertModifierAsync(
                fieldDeclaration,
                SyntaxKind.ReadOnlyKeyword,
                ModifierComparer.Instance,
                cancellationToken);
        }
    }
}
