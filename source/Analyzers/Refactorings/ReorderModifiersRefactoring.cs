// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Roslynator.CSharp.Comparers;

namespace Roslynator.CSharp.Refactorings
{
    internal static class ReorderModifiersRefactoring
    {
        public static void Analyze(SyntaxNodeAnalysisContext context, MemberDeclarationSyntax declaration)
        {
            SyntaxTokenList modifiers = declaration.GetModifiers();

            if (modifiers.Count > 1
                && !ModifierComparer.Instance.IsListSorted(modifiers)
                && !declaration.ContainsDirectives(modifiers.Span))
            {
                context.ReportDiagnostic(
                    DiagnosticDescriptors.ReorderModifiers,
                    Location.Create(context.Node.SyntaxTree, modifiers.Span));
            }
        }

        public static Task<Document> RefactorAsync(
            Document document,
            MemberDeclarationSyntax declaration,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            SyntaxTokenList modifiers = declaration.GetModifiers();

            SyntaxToken[] newModifiers = modifiers.OrderBy(f => f, ModifierComparer.Instance).ToArray();

            for (int i = 0; i < modifiers.Count; i++)
                newModifiers[i] = newModifiers[i].WithTriviaFrom(modifiers[i]);

            SyntaxNode newDeclaration = declaration.WithModifiers(SyntaxFactory.TokenList(newModifiers));

            return document.ReplaceNodeAsync(declaration, newDeclaration, cancellationToken);
        }
    }
}
