// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Diagnostics;

namespace Roslynator.FindSymbols
{
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    internal abstract class FilterRule<T>
    {
        public abstract SymbolFilterReason Reason { get; }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string DebuggerDisplay => $"{Reason}";

        public abstract bool IsApplicable(T value);

        public abstract bool IsMatch(T value);
    }
}
