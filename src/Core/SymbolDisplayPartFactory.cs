// Copyright (c) .NET Foundation and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Microsoft.CodeAnalysis;

namespace Roslynator;

internal static class SymbolDisplayPartFactory
{
    public static SymbolDisplayPart Text(string text)
    {
        return new(SymbolDisplayPartKind.Text, null, text);
    }

    public static SymbolDisplayPart Keyword(string keyword)
    {
        return new(SymbolDisplayPartKind.Keyword, null, keyword);
    }

    public static SymbolDisplayPart Punctuation(string value)
    {
        return new(SymbolDisplayPartKind.Punctuation, null, value);
    }

    public static SymbolDisplayPart Space(string text = " ")
    {
        return new(SymbolDisplayPartKind.Space, null, text);
    }

    public static SymbolDisplayPart Indentation(string indentChars = "    ")
    {
        return Space(indentChars);
    }

    public static SymbolDisplayPart LineBreak()
    {
        return new(SymbolDisplayPartKind.LineBreak, null, Environment.NewLine);
    }

    public static SymbolDisplayPart PropertyName(string name, ISymbol symbol)
    {
        return new(SymbolDisplayPartKind.PropertyName, symbol, name);
    }

    public static SymbolDisplayPart MethodName(string name, ISymbol symbol)
    {
        return new(SymbolDisplayPartKind.MethodName, symbol, name);
    }
}
