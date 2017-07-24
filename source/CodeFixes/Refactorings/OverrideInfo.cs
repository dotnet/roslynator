// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Diagnostics;
using Microsoft.CodeAnalysis;

namespace Roslynator.CSharp.Refactorings
{
    internal class OverrideInfo
    {
        public OverrideInfo(ISymbol symbol, ISymbol overriddenSymbol)
        {
            Symbol = symbol;
            OverriddenSymbol = overriddenSymbol;
        }

        public ISymbol Symbol { get; }
        public ISymbol OverriddenSymbol { get; }

        public string DeclaredAccessibilityText
        {
            get
            {
                switch (Symbol.DeclaredAccessibility)
                {
                    case Accessibility.Private:
                        return "private";
                    case Accessibility.Protected:
                        return "protected";
                    case Accessibility.Internal:
                        return "internal";
                    case Accessibility.ProtectedOrInternal:
                        return "protected internal";
                    case Accessibility.Public:
                        return "public";
                    default:
                        {
                            Debug.Fail(Symbol.DeclaredAccessibility.ToString());
                            return "";
                        }
                }
            }
        }
    }
}
