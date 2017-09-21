// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.CodeAnalysis;

namespace Roslynator.CSharp.CodeFixes
{
    internal static class EquivalenceKeyProvider
    {
        private const string EquivalenceKeyPrefix = "Roslynator.CSharp.CodeFixes";

        public static string GetEquivalenceKey(Diagnostic diagnostic, string additionalKey = null)
        {
            return GetEquivalenceKey(diagnostic.Id, additionalKey);
        }

        public static string GetEquivalenceKey(string key, string additionalKey = null)
        {
            if (additionalKey != null)
            {
                return $"{EquivalenceKeyPrefix}.{key}.{additionalKey}";
            }
            else
            {
                return $"{EquivalenceKeyPrefix}.{key}";
            }
        }
    }
}
