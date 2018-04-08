// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace Roslynator.CSharp
{
    internal abstract class ModifierComparer : IComparer<SyntaxToken>
    {
        internal const int MaxRank = 17;

        protected ModifierComparer()
        {
        }

        public static ModifierComparer Default { get; } = new ByKindModifierComparer();

        public abstract int Compare(SyntaxToken x, SyntaxToken y);

        public virtual int GetRank(SyntaxToken token)
        {
            return ModifierKindComparer.Default.GetRank(token.Kind());
        }

        private sealed class ByKindModifierComparer : ModifierComparer
        {
            public override int Compare(SyntaxToken x, SyntaxToken y)
            {
                return GetRank(x).CompareTo(GetRank(y));
            }
        }
    }
}
