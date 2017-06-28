// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Roslynator.CSharp.Helpers.ModifierHelpers
{
    internal class IndexerDeclarationModifierHelper : AbstractModifierHelper<IndexerDeclarationSyntax>
    {
        private IndexerDeclarationModifierHelper()
        {
        }

        public static IndexerDeclarationModifierHelper Instance { get; } = new IndexerDeclarationModifierHelper();

        public override SyntaxNodeOrToken FindNodeOrTokenAfterModifiers(IndexerDeclarationSyntax node)
        {
            return node.Type;
        }

        public override SyntaxTokenList GetModifiers(IndexerDeclarationSyntax node)
        {
            return node.Modifiers;
        }

        public override IndexerDeclarationSyntax WithModifiers(IndexerDeclarationSyntax node, SyntaxTokenList modifiers)
        {
            return node.WithModifiers(modifiers);
        }
    }
}
