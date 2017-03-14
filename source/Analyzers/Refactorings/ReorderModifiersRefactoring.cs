// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Roslynator.CSharp.Extensions;
using Roslynator.Extensions;

namespace Roslynator.CSharp.Refactorings
{
    internal static class ReorderModifiersRefactoring
    {
        public static void Analyze(SyntaxNodeAnalysisContext context, MemberDeclarationSyntax declaration)
        {
            SyntaxTokenList modifiers = declaration.GetModifiers();

            if (modifiers.Count > 1
                && !ModifierComparer.IsListSorted(modifiers)
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

            SyntaxNode newDeclaration = SetModifiers(declaration, SyntaxFactory.TokenList(newModifiers));

            return document.ReplaceNodeAsync(declaration, newDeclaration, cancellationToken);
        }

        private static SyntaxNode SetModifiers(SyntaxNode declaration, SyntaxTokenList modifiers)
        {
            switch (declaration.Kind())
            {
                case SyntaxKind.ClassDeclaration:
                    return ((ClassDeclarationSyntax)declaration).WithModifiers(modifiers);
                case SyntaxKind.ConstructorDeclaration:
                    return ((ConstructorDeclarationSyntax)declaration).WithModifiers(modifiers);
                case SyntaxKind.ConversionOperatorDeclaration:
                    return ((ConversionOperatorDeclarationSyntax)declaration).WithModifiers(modifiers);
                case SyntaxKind.DelegateDeclaration:
                    return ((DelegateDeclarationSyntax)declaration).WithModifiers(modifiers);
                case SyntaxKind.EnumDeclaration:
                    return ((EnumDeclarationSyntax)declaration).WithModifiers(modifiers);
                case SyntaxKind.EventDeclaration:
                    return ((EventDeclarationSyntax)declaration).WithModifiers(modifiers);
                case SyntaxKind.EventFieldDeclaration:
                    return ((EventFieldDeclarationSyntax)declaration).WithModifiers(modifiers);
                case SyntaxKind.FieldDeclaration:
                    return ((FieldDeclarationSyntax)declaration).WithModifiers(modifiers);
                case SyntaxKind.IndexerDeclaration:
                    return ((IndexerDeclarationSyntax)declaration).WithModifiers(modifiers);
                case SyntaxKind.InterfaceDeclaration:
                    return ((InterfaceDeclarationSyntax)declaration).WithModifiers(modifiers);
                case SyntaxKind.MethodDeclaration:
                    return ((MethodDeclarationSyntax)declaration).WithModifiers(modifiers);
                case SyntaxKind.OperatorDeclaration:
                    return ((OperatorDeclarationSyntax)declaration).WithModifiers(modifiers);
                case SyntaxKind.PropertyDeclaration:
                    return ((PropertyDeclarationSyntax)declaration).WithModifiers(modifiers);
                case SyntaxKind.StructDeclaration:
                    return ((StructDeclarationSyntax)declaration).WithModifiers(modifiers);
            }

            Debug.Assert(false, declaration.Kind().ToString());
            return declaration;
        }
    }
}
