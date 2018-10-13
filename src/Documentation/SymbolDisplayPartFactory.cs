// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Microsoft.CodeAnalysis;

namespace Roslynator.Documentation
{
    internal static class SymbolDisplayPartFactory
    {
        public static SymbolDisplayPart Text(string text)
        {
            return new SymbolDisplayPart(SymbolDisplayPartKind.Text, null, text);
        }

        public static SymbolDisplayPart Keyword(string keyword)
        {
            return new SymbolDisplayPart(SymbolDisplayPartKind.Keyword, null, keyword);
        }

        public static SymbolDisplayPart Punctuation(string value)
        {
            return new SymbolDisplayPart(SymbolDisplayPartKind.Punctuation, null, value);
        }

        public static SymbolDisplayPart Space(string text = " ")
        {
            return new SymbolDisplayPart(SymbolDisplayPartKind.Space, null, text);
        }

        public static SymbolDisplayPart Indentation(string indentChars = "    ")
        {
            return Space(indentChars);
        }

        public static SymbolDisplayPart LineBreak()
        {
            return new SymbolDisplayPart(SymbolDisplayPartKind.LineBreak, null, Environment.NewLine);
        }

        public static SymbolDisplayPart PropertyName(string name, ISymbol symbol)
        {
            return new SymbolDisplayPart(SymbolDisplayPartKind.PropertyName, symbol, name);
        }

        public static SymbolDisplayPart MethodName(string name, ISymbol symbol)
        {
            return new SymbolDisplayPart(SymbolDisplayPartKind.MethodName, symbol, name);
        }
    }
}
