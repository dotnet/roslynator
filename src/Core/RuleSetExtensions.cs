// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.CodeAnalysis;

namespace Roslynator
{
    internal static class RuleSetExtensions
    {
        public static DiagnosticSeverity GetDiagnosticSeverityOrDefault(this RuleSet ruleSet, string id, DiagnosticSeverity defaultValue)
        {
            if (!ruleSet.SpecificDiagnosticOptions.TryGetValue(id, out ReportDiagnostic reportDiagnostic))
                reportDiagnostic = ruleSet.GeneralDiagnosticOption;

            if (reportDiagnostic != ReportDiagnostic.Default
                && reportDiagnostic != ReportDiagnostic.Suppress)
            {
                return reportDiagnostic.ToDiagnosticSeverity();
            }

            return defaultValue;
        }

        public static bool IsDiagnosticEnabledOrDefault(this RuleSet ruleSet, string id, bool defaultValue)
        {
            if (ruleSet.SpecificDiagnosticOptions.TryGetValue(id, out ReportDiagnostic reportDiagnostic))
            {
                if (reportDiagnostic != ReportDiagnostic.Default)
                    return reportDiagnostic != ReportDiagnostic.Suppress;
            }
            else if (ruleSet.GeneralDiagnosticOption == ReportDiagnostic.Suppress)
            {
                return false;
            }

            return defaultValue;
        }
    }
}