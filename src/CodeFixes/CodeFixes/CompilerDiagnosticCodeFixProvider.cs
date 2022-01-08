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
            AnalyzerConfigOptions configOptions = document.GetConfigOptions(syntaxTree);

            string optionKey = _optionKeysMap.GetOrAdd(compilerDiagnosticId, id => CreateOptionKey(id));

            if (configOptions.TryGetValueAsBool(optionKey, out bool enabled))
                return enabled;

            if (configOptions.TryGetValueAsBool(ConfigOptionKeys.CompilerDiagnosticFixesEnabled, out bool globalEnabled))
                return globalEnabled;

            if (CompilerCodeFixOptions.Current.Disabled.Contains(new CodeFixIdentifier(compilerDiagnosticId, codeFixId)))
                return false;

            return CodeAnalysisConfig.Instance.CompilerDiagnosticFixesEnabled ?? true;
        }

        private static string CreateOptionKey(string compilerDiagnosticId)
        {
            return ConfigOptionKeys.CompilerDiagnosticFixPrefix + compilerDiagnosticId + ".enabled";
        }
    }
}
