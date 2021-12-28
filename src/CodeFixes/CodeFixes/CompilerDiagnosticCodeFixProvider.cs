// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Concurrent;
using Microsoft.CodeAnalysis;
using Roslynator.Configuration;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Roslynator.CodeFixes
{
    public abstract class CompilerDiagnosticCodeFixProvider : AbstractCodeFixProvider
    {
        private static readonly ConcurrentDictionary<string, string> _optionKeysMap = new();

        protected bool IsEnabled(string compilerDiagnosticId, string codeFixId, Document document, SyntaxTree syntaxTree)
        {
            AnalyzerConfigOptions configOptions = document.Project.AnalyzerOptions.AnalyzerConfigOptionsProvider.GetOptions(syntaxTree);

            string optionKey = _optionKeysMap.GetOrAdd(compilerDiagnosticId, id => CreateOptionKey(id));

            if (configOptions
                .TryGetValue(optionKey, out string enabledRaw)
                && bool.TryParse(enabledRaw, out bool enabled))
            {
                return enabled;
            }

            if (configOptions.TryGetValue(ConfigOptionKeys.CompilerDiagnosticFixEnabled, out string globalEnabledRaw)
                && bool.TryParse(globalEnabledRaw, out bool globalEnabled))
            {
                return globalEnabled;
            }

            return CompilerCodeFixOptions.Current.IsEnabled(compilerDiagnosticId, codeFixId);
        }

        private static string CreateOptionKey(string compilerDiagnosticId)
        {
            return ConfigOptionKeys.CompilerDiagnosticFixPrefix + compilerDiagnosticId + ".enabled";
        }
    }
}
