// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Roslynator.CSharp.Refactorings
{
    internal static class RenamePrivateFieldAccordingToCamelCaseWithUnderscoreRefactoring
    {
        public static void Analyze(SymbolAnalysisContext context, IFieldSymbol fieldSymbol)
        {
            if (!fieldSymbol.IsConst
                && !fieldSymbol.IsImplicitlyDeclared
                && fieldSymbol.IsPrivate()
                && !string.IsNullOrEmpty(fieldSymbol.Name)
                && !IdentifierUtility.IsCamelCasePrefixedWithUnderscore(fieldSymbol.Name))
            {
                context.ReportDiagnostic(
                    DiagnosticDescriptors.RenamePrivateFieldAccordingToCamelCaseWithUnderscore,
                    fieldSymbol.Locations[0]);
            }
        }

        public static Task<Solution> RefactorAsync(
            Document document,
            ISymbol symbol,
            string newName,
            CancellationToken cancellationToken)
        {
            return SymbolRenamer.RenameSymbolAsync(document, symbol, newName, cancellationToken);
        }
    }
}
