// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Roslynator.CSharp.Helpers.ModifierHelpers
{
    internal class ConstructorDeclarationModifierHelper : AbstractModifierHelper<ConstructorDeclarationSyntax>
    {
        private ConstructorDeclarationModifierHelper()
        {
        }

        public static ConstructorDeclarationModifierHelper Instance { get; } = new ConstructorDeclarationModifierHelper();

        public override SyntaxNodeOrToken FindNodeOrTokenAfterModifiers(ConstructorDeclarationSyntax node)
        {
            return node.Identifier;
        }

        public override SyntaxTokenList GetModifiers(ConstructorDeclarationSyntax node)
        {
            return node.Modifiers;
        }

        public override ConstructorDeclarationSyntax WithModifiers(ConstructorDeclarationSyntax node, SyntaxTokenList modifiers)
        {
            return node.WithModifiers(modifiers);
        }
    }
}
