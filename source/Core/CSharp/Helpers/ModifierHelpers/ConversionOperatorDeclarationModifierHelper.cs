// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Roslynator.CSharp.Helpers.ModifierHelpers
{
    internal class ConversionOperatorDeclarationModifierHelper : AbstractModifierHelper<ConversionOperatorDeclarationSyntax>
    {
        private ConversionOperatorDeclarationModifierHelper()
        {
        }

        public static ConversionOperatorDeclarationModifierHelper Instance { get; } = new ConversionOperatorDeclarationModifierHelper();

        public override SyntaxNodeOrToken FindNodeOrTokenAfterModifiers(ConversionOperatorDeclarationSyntax node)
        {
            return node.ImplicitOrExplicitKeyword;
        }

        public override SyntaxTokenList GetModifiers(ConversionOperatorDeclarationSyntax node)
        {
            return node.Modifiers;
        }

        public override ConversionOperatorDeclarationSyntax WithModifiers(ConversionOperatorDeclarationSyntax node, SyntaxTokenList modifiers)
        {
            return node.WithModifiers(modifiers);
        }
    }
}
