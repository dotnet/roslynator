// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;

namespace Roslynator.CSharp.CodeFixes
{
    public abstract class BaseCodeFixProvider : CodeFixProvider
    {
        public const string EquivalenceKeyPrefix = "Roslynator.CSharp.CodeFixes";

        public override FixAllProvider GetFixAllProvider()
        {
            return WellKnownFixAllProviders.BatchFixer;
        }

        public string GetEquivalenceKey(Diagnostic diagnostic, string additionalKey = null)
        {
            return GetEquivalenceKey(diagnostic.Id, additionalKey);
        }

        public string GetEquivalenceKey(string key, string additionalKey = null)
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

        protected virtual CodeFixSettings Settings
        {
            get { return CodeFixSettings.Current; }
        }
    }
}
