// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Roslynator.CSharp.Helpers.ModifierHelpers
{
    internal class LocalFunctionStatementModifierHelper : AbstractModifierHelper<LocalFunctionStatementSyntax>
    {
        private LocalFunctionStatementModifierHelper()
        {
        }

        public static LocalFunctionStatementModifierHelper Instance { get; } = new LocalFunctionStatementModifierHelper();

        public override SyntaxNodeOrToken FindNodeOrTokenAfterModifiers(LocalFunctionStatementSyntax node)
        {
            return node.ReturnType;
        }

        public override SyntaxTokenList GetModifiers(LocalFunctionStatementSyntax node)
        {
            return node.Modifiers;
        }

        public override LocalFunctionStatementSyntax WithModifiers(LocalFunctionStatementSyntax node, SyntaxTokenList modifiers)
        {
            return node.WithModifiers(modifiers);
        }
    }
}
