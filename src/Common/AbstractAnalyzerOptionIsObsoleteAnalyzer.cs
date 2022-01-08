// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Roslynator
{
    public abstract class AbstractAnalyzerOptionIsObsoleteAnalyzer : DiagnosticAnalyzer
    {
        protected static bool TryReportObsoleteOption(
            SyntaxTreeAnalysisContext context,
            AnalyzerConfigOptions configOptions,
            LegacyConfigOptionDescriptor legacyOption,
            ConfigOptionDescriptor newOption,
            string newValue)
        {
            if (configOptions.IsEnabled(legacyOption))
            {
                DiagnosticHelpers.ReportDiagnostic(
                    context,
                    CommonDiagnosticRules.AnalyzerOptionIsObsolete,
                    Location.None,
                    legacyOption.Key,
                    $", use option '{newOption.Key} = {newValue}' instead");

                return true;
            }

            return false;
        }
    }
}
