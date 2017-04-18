// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace Roslynator.CSharp
{
    public interface IModifierComparer : IComparer<SyntaxToken>
    {
        int GetInsertIndex(SyntaxTokenList modifiers, SyntaxToken modifier);

        int GetInsertIndex(SyntaxTokenList modifiers, SyntaxKind modifierKind);
    }
}
