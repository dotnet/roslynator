// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Roslynator.CSharp.Helpers.ModifierHelpers
{
    internal class PropertyDeclarationModifierHelper : AbstractModifierHelper<PropertyDeclarationSyntax>
    {
        private PropertyDeclarationModifierHelper()
        {
        }

        public static PropertyDeclarationModifierHelper Instance { get; } = new PropertyDeclarationModifierHelper();

        public override SyntaxNodeOrToken FindNodeOrTokenAfterModifiers(PropertyDeclarationSyntax node)
        {
            return node.Type;
        }

        public override SyntaxTokenList GetModifiers(PropertyDeclarationSyntax node)
        {
            return node.Modifiers;
        }

        public override PropertyDeclarationSyntax WithModifiers(PropertyDeclarationSyntax node, SyntaxTokenList modifiers)
        {
            return node.WithModifiers(modifiers);
        }
    }
}
