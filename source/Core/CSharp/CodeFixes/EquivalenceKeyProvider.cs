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

        public static string GetEquivalenceKey(Diagnostic diagnostic, string additionalKey1, string additionalKey2)
        {
            return GetEquivalenceKey(diagnostic.Id, additionalKey1, additionalKey2);
        }

        public static string GetEquivalenceKey(string key, string additionalKey = null)
        {
            return GetEquivalenceKey(key, additionalKey, null);
        }

        public static string GetEquivalenceKey(string key, string additionalKey1, string additionalKey2)
        {
            if (additionalKey1 != null)
            {
                if (additionalKey2 != null)
                    return $"{EquivalenceKeyPrefix}.{key}.{additionalKey1}.{additionalKey2}";

                return $"{EquivalenceKeyPrefix}.{key}.{additionalKey1}";
            }
            else if (additionalKey2 != null)
            {
                return $"{EquivalenceKeyPrefix}.{key}.{additionalKey2}";
            }
            else
            {
                return $"{EquivalenceKeyPrefix}.{key}";
            }
        }
    }
}
