// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using Roslynator.Configuration;

namespace Roslynator.CodeFixes
{
    public sealed class CodeFixSettings : CodeAnalysisSettings<CodeFixIdentifier>
    {
        public static CodeFixSettings Current { get; } = LoadSettings();

        private static CodeFixSettings LoadSettings()
        {
            var settings = new CodeFixSettings();

            settings.Reset();

            return settings;
        }

        protected override void SetValues(CodeAnalysisConfiguration configuration)
        {
            if (configuration == null)
                return;

            foreach (KeyValuePair<string, bool> kvp in configuration.CodeFixes)
            {
                string id = kvp.Key;
                bool isEnabled = kvp.Value;

                if (CodeFixIdentifier.TryParse(id, out CodeFixIdentifier codeFixIdentifier))
                {
                    Set(codeFixIdentifier, isEnabled);
                }
                else if (id.StartsWith(CodeFixIdentifier.CodeFixIdPrefix, StringComparison.Ordinal))
                {
                    foreach (string compilerDiagnosticId in CodeFixMap.GetCompilerDiagnosticIds(id))
                    {
                        Set(new CodeFixIdentifier(compilerDiagnosticId, id), isEnabled);
                    }
                }
                else if (id.StartsWith("CS", StringComparison.Ordinal))
                {
                    foreach (string codeFixId in CodeFixMap.GetCodeFixIds(id))
                    {
                        Set(new CodeFixIdentifier(id, codeFixId), isEnabled);
                    }
                }
                else
                {
                    Debug.Fail(id);
                }
            }
        }

        public bool IsEnabled(string compilerDiagnosticId, string codeFixId)
        {
            return IsEnabled(new CodeFixIdentifier(compilerDiagnosticId, codeFixId));
        }
    }
}
