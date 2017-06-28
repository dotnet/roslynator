// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Roslynator.CSharp.Helpers.ModifierHelpers
{
    internal class InterfaceDeclarationModifierHelper : AbstractModifierHelper<InterfaceDeclarationSyntax>
    {
        private InterfaceDeclarationModifierHelper()
        {
        }

        public static InterfaceDeclarationModifierHelper Instance { get; } = new InterfaceDeclarationModifierHelper();

        public override SyntaxNodeOrToken FindNodeOrTokenAfterModifiers(InterfaceDeclarationSyntax node)
        {
            return node.Keyword;
        }

        public override SyntaxTokenList GetModifiers(InterfaceDeclarationSyntax node)
        {
            return node.Modifiers;
        }

        public override InterfaceDeclarationSyntax WithModifiers(InterfaceDeclarationSyntax node, SyntaxTokenList modifiers)
        {
            return node.WithModifiers(modifiers);
        }
    }
}
