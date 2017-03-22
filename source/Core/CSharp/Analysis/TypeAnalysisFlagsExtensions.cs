// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace Roslynator.CSharp.Analysis
{
    internal static class TypeAnalysisFlagsExtensions
    {
        public static bool IsImplicit(this TypeAnalysisFlags flags)
        {
            return (flags & TypeAnalysisFlags.Implicit) != 0;
        }

        public static bool IsExplicit(this TypeAnalysisFlags flags)
        {
            return (flags & TypeAnalysisFlags.Explicit) != 0;
        }

        public static bool SupportsImplicit(this TypeAnalysisFlags flags)
        {
            return (flags & TypeAnalysisFlags.SupportsImplicit) != 0;
        }

        public static bool SupportsExplicit(this TypeAnalysisFlags flags)
        {
            return (flags & TypeAnalysisFlags.SupportsExplicit) != 0;
        }

        public static bool IsValidSymbol(this TypeAnalysisFlags flags)
        {
            return (flags & TypeAnalysisFlags.ValidSymbol) != 0;
        }

        public static bool IsTypeObvious(this TypeAnalysisFlags flags)
        {
            return (flags & TypeAnalysisFlags.TypeObvious) != 0;
        }
    }
}
