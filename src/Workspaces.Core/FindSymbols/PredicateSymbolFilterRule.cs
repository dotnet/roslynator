// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Microsoft.CodeAnalysis;

namespace Roslynator.FindSymbols
{
    internal class PredicateSymbolFilterRule : SymbolFilterRule
    {
        private readonly Func<ISymbol, bool> _isMatch;
        private readonly Func<ISymbol, bool> _isApplicable;

        public PredicateSymbolFilterRule(Func<ISymbol, bool> isMatch, Func<ISymbol, bool> isApplicable, SymbolFilterReason reason)
        {
            _isMatch = isMatch;
            _isApplicable = isApplicable;
            Reason = reason;
        }

        public override bool IsApplicable(ISymbol value) => _isApplicable(value);

        public override bool IsMatch(ISymbol value) => _isMatch(value);

        public override SymbolFilterReason Reason { get; }

        public PredicateSymbolFilterRule Invert()
        {
            return new PredicateSymbolFilterRule(f => !_isMatch(f), _isApplicable, Reason);
        }
    }
}
