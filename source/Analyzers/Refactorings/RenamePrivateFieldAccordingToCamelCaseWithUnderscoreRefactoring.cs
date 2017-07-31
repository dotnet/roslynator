// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Options;
using Microsoft.CodeAnalysis.Rename;
using Roslynator.Utilities;

namespace Roslynator.CSharp.Refactorings
{
    internal static class RenamePrivateFieldAccordingToCamelCaseWithUnderscoreRefactoring
    {
        public static void AnalyzeFieldSymbol(SymbolAnalysisContext context)
        {
            var fieldSymbol = (IFieldSymbol)context.Symbol;

            if (!fieldSymbol.IsConst
                && !fieldSymbol.IsImplicitlyDeclared
                && fieldSymbol.IsPrivate()
                && !string.IsNullOrEmpty(fieldSymbol.Name)
                && !StringUtility.IsCamelCasePrefixedWithUnderscore(fieldSymbol.Name))
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
            return Renamer.RenameSymbolAsync(
                document.Solution(),
                symbol,
                newName,
                default(OptionSet),
                cancellationToken);
        }
    }
}
