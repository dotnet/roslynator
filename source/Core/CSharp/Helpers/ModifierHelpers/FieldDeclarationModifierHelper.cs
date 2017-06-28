// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Roslynator.CSharp.Helpers.ModifierHelpers
{
    internal class FieldDeclarationModifierHelper : AbstractModifierHelper<FieldDeclarationSyntax>
    {
        private FieldDeclarationModifierHelper()
        {
        }

        public static FieldDeclarationModifierHelper Instance { get; } = new FieldDeclarationModifierHelper();

        public override SyntaxNodeOrToken FindNodeOrTokenAfterModifiers(FieldDeclarationSyntax node)
        {
            return node.Declaration?.Type;
        }

        public override SyntaxTokenList GetModifiers(FieldDeclarationSyntax node)
        {
            return node.Modifiers;
        }

        public override FieldDeclarationSyntax WithModifiers(FieldDeclarationSyntax node, SyntaxTokenList modifiers)
        {
            return node.WithModifiers(modifiers);
        }
    }
}
