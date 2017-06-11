// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Roslynator.CSharp.Comparers;

namespace Roslynator.CSharp.Refactorings
{
    internal static class MarkFieldAsConstRefactoring
    {
        public static void AnalyzeFieldDeclaration(SyntaxNodeAnalysisContext context)
        {
            SyntaxNode node = context.Node;

            if (node.ContainsDiagnostics)
                return;

            if (node.SpanContainsDirectives())
                return;

            var fieldDeclaration = (FieldDeclarationSyntax)node;

            SyntaxTokenList modifiers = fieldDeclaration.Modifiers;

            if (modifiers.Contains(SyntaxKind.StaticKeyword)
                && modifiers.Contains(SyntaxKind.ReadOnlyKeyword)
                && !modifiers.Contains(SyntaxKind.NewKeyword)
                && IsFixable(fieldDeclaration.Declaration, context.SemanticModel, context.CancellationToken))
            {
                context.ReportDiagnostic(DiagnosticDescriptors.MarkFieldAsConst, fieldDeclaration);
            }
        }

        private static bool IsFixable(
            VariableDeclarationSyntax variableDeclaration,
            SemanticModel semanticModel,
            CancellationToken cancellationToken)
        {
            foreach (VariableDeclaratorSyntax declarator in variableDeclaration.Variables)
            {
                if (!IsFixable(variableDeclaration.Type, declarator, semanticModel, cancellationToken))
                    return false;
            }

            return true;
        }

        private static bool IsFixable(
            TypeSyntax type,
            VariableDeclaratorSyntax declarator,
            SemanticModel semanticModel,
            CancellationToken cancellationToken)
        {
            ExpressionSyntax value = declarator.Initializer?.Value;

            return value != null
                && semanticModel.GetTypeSymbol(type, cancellationToken)?.SupportsConstantValue() == true
                && semanticModel.GetConstantValue(value, cancellationToken).HasValue;
        }

        public static Task<Document> RefactorAsync(
            Document document,
            FieldDeclarationSyntax fieldDeclaration,
            CancellationToken cancellationToken)
        {
            FieldDeclarationSyntax newNode = fieldDeclaration
                .InsertModifier(SyntaxKind.ConstKeyword, ModifierComparer.Instance)
                .RemoveModifier(SyntaxKind.StaticKeyword)
                .RemoveModifier(SyntaxKind.ReadOnlyKeyword);

            return document.ReplaceNodeAsync(fieldDeclaration, newNode, cancellationToken);
        }
    }
}
