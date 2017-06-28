// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Roslynator.CSharp.Helpers.ModifierHelpers
{
    internal class StructDeclarationModifierHelper : AbstractModifierHelper<StructDeclarationSyntax>
    {
        private StructDeclarationModifierHelper()
        {
        }

        public static StructDeclarationModifierHelper Instance { get; } = new StructDeclarationModifierHelper();

        public override SyntaxNodeOrToken FindNodeOrTokenAfterModifiers(StructDeclarationSyntax node)
        {
            return node.Keyword;
        }

        public override SyntaxTokenList GetModifiers(StructDeclarationSyntax node)
        {
            return node.Modifiers;
        }

        public override StructDeclarationSyntax WithModifiers(StructDeclarationSyntax node, SyntaxTokenList modifiers)
        {
            return node.WithModifiers(modifiers);
        }
    }
}
