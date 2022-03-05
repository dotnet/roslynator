// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Immutable;
using System.Diagnostics;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Roslynator.Configuration;

namespace Roslynator
{
    public static partial class ConfigOptions
    {
        private static readonly ImmutableDictionary<string, string> _requiredOptions = GetRequiredOptions().ToImmutableDictionary(f => f.Key, f => f.Value);

        public static string GetRequiredOptions(DiagnosticDescriptor descriptor)
        {
            Debug.Assert(_requiredOptions.ContainsKey(descriptor.Id), descriptor.Id);

            return _requiredOptions.GetValueOrDefault(descriptor.Id);
        }

        private static string JoinOptionKeys(params string[] values)
        {
            return string.Join(" or ", values);
        }

        public static bool TryGetValue(AnalyzerConfigOptions configOptions, ConfigOptionDescriptor option, out string value, string defaultValue = null)
        {
            if (configOptions.TryGetValue(option.Key, out string rawValue))
            {
                value = rawValue;
                return true;
            }

            value = defaultValue
                ?? CodeAnalysisConfig.Instance.EditorConfig.Options.GetValueOrDefault(option.Key)
                ?? option.DefaultValue;

            return value != null;
        }

        public static string GetValue(AnalyzerConfigOptions configOptions, ConfigOptionDescriptor option, string defaultValue = null)
        {
            if (configOptions.TryGetValue(option.Key, out string value))
                return value;

            return defaultValue
                ?? CodeAnalysisConfig.Instance.EditorConfig.Options.GetValueOrDefault(option.Key)
                ?? option.DefaultValue;
        }

        public static bool TryGetValueAsBool(AnalyzerConfigOptions configOptions, ConfigOptionDescriptor option, out bool value, bool? defaultValue = null)
        {
            if (configOptions.TryGetValue(option.Key, out string rawValue)
                && bool.TryParse(rawValue, out bool boolValue))
            {
                value = boolValue;
                return true;
            }

            bool? maybeValue = defaultValue
                ?? CodeAnalysisConfig.Instance.GetOptionAsBool(option.Key)
                ?? option.DefaultValueAsBool;

            if (maybeValue != null)
            {
                value = maybeValue.Value;
                return true;
            }

            value = false;
            return false;
        }

        public static bool? GetValueAsBool(AnalyzerConfigOptions configOptions, ConfigOptionDescriptor option, bool? defaultValue = null)
        {
            if (configOptions.TryGetValue(option.Key, out string rawValue)
                && bool.TryParse(rawValue, out bool boolValue))
            {
                return boolValue;
            }

            return defaultValue
                ?? CodeAnalysisConfig.Instance.GetOptionAsBool(option.Key)
                ?? option.DefaultValueAsBool;
        }
    }
}