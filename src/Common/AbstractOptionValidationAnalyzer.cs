// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Roslynator.Configuration;

namespace Roslynator
{
    internal abstract class AbstractOptionValidationAnalyzer : DiagnosticAnalyzer
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
                context.ReportDiagnostic(Diagnostic.Create(
                    CommonDiagnosticRules.AnalyzerOptionIsObsolete,
                    Location.None,
                    legacyOption.Key,
                    $", use option '{newOption.Key} = {newValue}' instead"));

                return true;
            }

            return false;
        }

        protected static bool TryReportMissingRequiredOption(
            SyntaxTreeAnalysisContext context,
            AnalyzerConfigOptions configOptions,
            DiagnosticDescriptor descriptor,
            ConfigOptionDescriptor option)
        {
            if (!configOptions.TryGetValue(option.Key, out string _)
                && !CodeAnalysisConfig.Instance.EditorConfig.Options.ContainsKey(option.Key))
            {
                Diagnostic diagnostic = Diagnostic.Create(
                    CommonDiagnosticRules.RequiredConfigOptionNotSet,
                    Location.None,
                    ConfigOptions.GetRequiredOption(descriptor),
                    descriptor.Id);

                context.ReportDiagnostic(diagnostic);
                return true;
            }

            return false;
        }
    }
}
