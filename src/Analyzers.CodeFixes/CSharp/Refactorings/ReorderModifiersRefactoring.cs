// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.CSharp.Syntax;

namespace Roslynator.CSharp.Refactorings
{
    internal static class ReorderModifiersRefactoring
    {
        public static Task<Document> RefactorAsync(
            Document document,
            MemberDeclarationSyntax declaration,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            ModifierListInfo info = SyntaxInfo.ModifierListInfo(declaration);

            SyntaxTokenList modifiers = info.Modifiers;

            SyntaxToken[] newModifiers = modifiers.OrderBy(f => f, ModifierComparer.Default).ToArray();

            for (int i = 0; i < modifiers.Count; i++)
                newModifiers[i] = newModifiers[i].WithTriviaFrom(modifiers[i]);

            return document.ReplaceModifiersAsync(info, newModifiers, cancellationToken);
        }
    }
}
