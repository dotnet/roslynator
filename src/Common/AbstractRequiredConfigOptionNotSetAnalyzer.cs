// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Roslynator.Configuration;

namespace Roslynator
{
    internal abstract class AbstractRequiredConfigOptionNotSetAnalyzer : DiagnosticAnalyzer
    {
        protected static bool TryReportRequiredOptionNotSet(
            SyntaxTreeAnalysisContext context,
            AnalyzerConfigOptions configOptions,
            DiagnosticDescriptor descriptor,
            ConfigOptionDescriptor option)
        {
            if (!IsOptionSet(configOptions, option))
            {
                ReportDiagnostic(context, descriptor);
                return true;
            }

            return false;
        }

        protected static bool TryReportRequiredOptionNotSet(
            SyntaxTreeAnalysisContext context,
            AnalyzerConfigOptions configOptions,
            DiagnosticDescriptor descriptor,
            params ConfigOptionDescriptor[] options)
        {
            foreach (ConfigOptionDescriptor option in options)
            {
                if (IsOptionSet(configOptions, option))
                    return false;
            }

            DiagnosticHelpers.ReportDiagnostic(
                context,
                CommonDiagnosticRules.RequiredConfigOptionNotSet,
                Location.None,
                descriptor.Id,
                ConfigOptions.GetRequiredOptions(descriptor));

            return true;
        }

        private static bool IsOptionSet(AnalyzerConfigOptions configOptions, ConfigOptionDescriptor option)
        {
            return configOptions.TryGetValue(option.Key, out string _)
                || CodeAnalysisConfig.Instance.EditorConfig.Options.ContainsKey(option.Key);
        }

        private static void ReportDiagnostic(SyntaxTreeAnalysisContext context, DiagnosticDescriptor descriptor)
        {
            DiagnosticHelpers.ReportDiagnostic(
                context,
                CommonDiagnosticRules.RequiredConfigOptionNotSet,
                Location.None,
                descriptor.Id,
                ConfigOptions.GetRequiredOptions(descriptor)
#if DEBUG
                    + $", path: {context.Tree.FilePath}"
#endif
                );
        }
    }
}
