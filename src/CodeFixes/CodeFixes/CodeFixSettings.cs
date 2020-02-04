// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Threading;
using Roslynator.Configuration;

namespace Roslynator.CodeFixes
{
    public sealed class CodeFixSettings : CodeAnalysisSettings<CodeFixIdentifier>
    {
        private static ImmutableDictionary<CodeFixIdentifier, bool> _codeFixes;

        public static CodeFixSettings Current { get; } = Load();

        public static ImmutableDictionary<CodeFixIdentifier, bool> CodeFixes
        {
            get
            {
                if (_codeFixes == null)
                    Interlocked.CompareExchange(ref _codeFixes, LoadCodeFixes().ToImmutableDictionary(), null);

                return _codeFixes;

                static IEnumerable<KeyValuePair<CodeFixIdentifier, bool>> LoadCodeFixes()
                {
                    foreach (KeyValuePair<string, bool> kvp in CodeAnalysisConfiguration.Default.CodeFixes)
                    {
                        string id = kvp.Key;
                        bool isEnabled = kvp.Value;

                        if (CodeFixIdentifier.TryParse(id, out CodeFixIdentifier codeFixIdentifier))
                        {
                            yield return new KeyValuePair<CodeFixIdentifier, bool>(codeFixIdentifier, isEnabled);
                        }
                        else if (id.StartsWith(CodeFixIdentifier.CodeFixIdPrefix, StringComparison.Ordinal))
                        {
                            foreach (string compilerDiagnosticId in CodeFixMap.GetCompilerDiagnosticIds(id))
                            {
                                yield return new KeyValuePair<CodeFixIdentifier, bool>(new CodeFixIdentifier(compilerDiagnosticId, id), isEnabled);
                            }
                        }
                        else if (id.StartsWith("CS", StringComparison.Ordinal))
                        {
                            foreach (string codeFixId in CodeFixMap.GetCodeFixIds(id))
                            {
                                yield return new KeyValuePair<CodeFixIdentifier, bool>(new CodeFixIdentifier(id, codeFixId), isEnabled);
                            }
                        }
                        else
                        {
                            Debug.Fail(id);
                        }
                    }
                }
            }
        }

        private static CodeFixSettings Load()
        {
            var settings = new CodeFixSettings();

            settings.Reset();

            return settings;
        }

        public override void Reset()
        {
            Disabled.Clear();

            foreach (KeyValuePair<CodeFixIdentifier, bool> kvp in CodeFixes)
                Set(kvp.Key, kvp.Value);
        }

        internal void Reset(CodeAnalysisConfiguration configuration1, CodeAnalysisConfiguration configuration2)
        {
            Reset();

            SetValues(configuration1);
            SetValues(configuration2);

            void SetValues(CodeAnalysisConfiguration configuration)
            {
                if (configuration != null)
                {
                    foreach (KeyValuePair<string, bool> kvp in configuration.CodeFixes)
                    {
                        if (CodeFixIdentifier.TryParse(kvp.Key, out CodeFixIdentifier codeFixIdentifier))
                        {
                            Set(codeFixIdentifier, kvp.Value);
                        }
                    }
                }
            }
        }

        public bool IsEnabled(string compilerDiagnosticId, string codeFixId)
        {
            return IsEnabled(new CodeFixIdentifier(compilerDiagnosticId, codeFixId));
        }
    }
}
