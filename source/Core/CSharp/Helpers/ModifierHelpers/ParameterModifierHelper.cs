// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Roslynator.CSharp.Helpers.ModifierHelpers
{
    internal class ParameterModifierHelper : AbstractModifierHelper<ParameterSyntax>
    {
        private ParameterModifierHelper()
        {
        }

        public static ParameterModifierHelper Instance { get; } = new ParameterModifierHelper();

        public override SyntaxNodeOrToken FindNodeOrTokenAfterModifiers(ParameterSyntax node)
        {
            return node.Type;
        }

        public override SyntaxTokenList GetModifiers(ParameterSyntax node)
        {
            return node.Modifiers;
        }

        public override ParameterSyntax WithModifiers(ParameterSyntax node, SyntaxTokenList modifiers)
        {
            return node.WithModifiers(modifiers);
        }
    }
}
