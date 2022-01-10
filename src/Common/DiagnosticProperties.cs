// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Collections.Immutable;
using System.Threading;

namespace Roslynator
{
    internal static class DiagnosticProperties
    {
        public static ImmutableDictionary<string, string> NewLinePosition_After = ImmutableDictionary.CreateRange(new[] { new KeyValuePair<string, string>("NewLinePosition", "After") });

        private static ImmutableDictionary<string, string> _analyzerOption_Invert;

        private const string AnalyzerOptionKey = "AnalyzerOption";

        private const string InvertValue = "Invert";

        public static ImmutableDictionary<string, string> AnalyzerOption_Invert
        {
            get
            {
                if (_analyzerOption_Invert == null)
                {
                    Interlocked.CompareExchange(
                        ref _analyzerOption_Invert,
                        ImmutableDictionary.CreateRange(new[] { new KeyValuePair<string, string>(AnalyzerOptionKey, InvertValue) }),
                        null);
                }

                return _analyzerOption_Invert;
            }
        }

        public static bool ContainsInvert(ImmutableDictionary<string, string> properties)
        {
            return properties.TryGetValue(AnalyzerOptionKey, out string value)
                && value == InvertValue;
        }
    }
}
