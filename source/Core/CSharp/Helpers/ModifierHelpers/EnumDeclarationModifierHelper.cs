// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Roslynator.CSharp.Helpers.ModifierHelpers
{
    internal class EnumDeclarationModifierHelper : AbstractModifierHelper<EnumDeclarationSyntax>
    {
        private EnumDeclarationModifierHelper()
        {
        }

        public static EnumDeclarationModifierHelper Instance { get; } = new EnumDeclarationModifierHelper();

        public override SyntaxNodeOrToken FindNodeOrTokenAfterModifiers(EnumDeclarationSyntax node)
        {
            return node.EnumKeyword;
        }

        public override SyntaxTokenList GetModifiers(EnumDeclarationSyntax node)
        {
            return node.Modifiers;
        }

        public override EnumDeclarationSyntax WithModifiers(EnumDeclarationSyntax node, SyntaxTokenList modifiers)
        {
            return node.WithModifiers(modifiers);
        }
    }
}
