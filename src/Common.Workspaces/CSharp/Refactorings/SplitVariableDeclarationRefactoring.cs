// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static Roslynator.CSharp.CSharpFactory;

namespace Roslynator.CSharp.Refactorings
{
    internal static class SplitVariableDeclarationRefactoring
    {
        public static string GetTitle(VariableDeclarationSyntax variableDeclaration)
        {
            return $"Split {GetName()} declaration";

            string GetName()
            {
                switch (variableDeclaration.Parent?.Kind())
                {
                    case SyntaxKind.LocalDeclarationStatement:
                        return "local";
                    case SyntaxKind.FieldDeclaration:
                        return "field";
                    case SyntaxKind.EventFieldDeclaration:
                        return "event";
                }

                return "variable";
            }
        }

        public static async Task<Document> RefactorAsync(
            Document document,
            VariableDeclarationSyntax variableDeclaration,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            switch (variableDeclaration.Parent.Kind())
            {
                case SyntaxKind.LocalDeclarationStatement:
                    return await SplitLocalDeclarationAsync(document, (LocalDeclarationStatementSyntax)variableDeclaration.Parent, cancellationToken).ConfigureAwait(false);
                case SyntaxKind.FieldDeclaration:
                    return await SplitFieldDeclarationAsync(document, (FieldDeclarationSyntax)variableDeclaration.Parent, cancellationToken).ConfigureAwait(false);
                case SyntaxKind.EventFieldDeclaration:
                    return await SplitEventFieldDeclarationAsync(document, (EventFieldDeclarationSyntax)variableDeclaration.Parent, cancellationToken).ConfigureAwait(false);
                default:
                    throw new InvalidOperationException();
            }
        }

        private static Task<Document> SplitLocalDeclarationAsync(
            Document document,
            LocalDeclarationStatementSyntax statement,
            CancellationToken cancellationToken)
        {
            var block = (BlockSyntax)statement.Parent;

            SyntaxList<StatementSyntax> newStatements = block.Statements.ReplaceRange(
                statement,
                SplitLocalDeclaration(statement));

            BlockSyntax newBlock = block.WithStatements(newStatements);

            return document.ReplaceNodeAsync(block, newBlock, cancellationToken);
        }

        private static Task<Document> SplitFieldDeclarationAsync(
            Document document,
            FieldDeclarationSyntax declaration,
            CancellationToken cancellationToken)
        {
            var containingMember = (TypeDeclarationSyntax)declaration.Parent;

            SyntaxList<MemberDeclarationSyntax> members = containingMember.Members;

            SyntaxList<MemberDeclarationSyntax> newMembers = members.ReplaceRange(
                declaration,
                SplitFieldDeclaration(declaration));

            MemberDeclarationSyntax newNode = containingMember.WithMembers(newMembers);

            return document.ReplaceNodeAsync(containingMember, newNode, cancellationToken);
        }

        private static Task<Document> SplitEventFieldDeclarationAsync(
            Document document,
            EventFieldDeclarationSyntax declaration,
            CancellationToken cancellationToken)
        {
            var containingMember = (TypeDeclarationSyntax)declaration.Parent;

            SyntaxList<MemberDeclarationSyntax> members = containingMember.Members;

            SyntaxList<MemberDeclarationSyntax> newMembers = members.ReplaceRange(
                declaration,
                SplitEventFieldDeclaration(declaration));

            MemberDeclarationSyntax newNode = containingMember.WithMembers(newMembers);

            return document.ReplaceNodeAsync(containingMember, newNode, cancellationToken);
        }

        private static IEnumerable<LocalDeclarationStatementSyntax> SplitLocalDeclaration(LocalDeclarationStatementSyntax statement)
        {
            SeparatedSyntaxList<VariableDeclaratorSyntax> variables = statement.Declaration.Variables;

            LocalDeclarationStatementSyntax statement2 = statement.WithoutTrivia();

            for (int i = 0; i < variables.Count; i++)
            {
                LocalDeclarationStatementSyntax newStatement = LocalDeclarationStatement(
                    statement2.Modifiers,
                    VariableDeclaration(
                        statement2.Declaration.Type,
                        variables[i]));

                if (i == 0)
                    newStatement = newStatement.WithLeadingTrivia(statement.GetLeadingTrivia());

                if (i == variables.Count - 1)
                    newStatement = newStatement.WithTrailingTrivia(statement.GetTrailingTrivia());

                yield return newStatement.WithFormatterAnnotation();
            }
        }

        private static IEnumerable<FieldDeclarationSyntax> SplitFieldDeclaration(FieldDeclarationSyntax declaration)
        {
            SeparatedSyntaxList<VariableDeclaratorSyntax> variables = declaration.Declaration.Variables;

            FieldDeclarationSyntax declaration2 = declaration.WithoutTrivia();

            for (int i = 0; i < variables.Count; i++)
            {
                FieldDeclarationSyntax newDeclaration = FieldDeclaration(
                    declaration2.AttributeLists,
                    declaration2.Modifiers,
                    VariableDeclaration(
                        declaration2.Declaration.Type,
                        variables[i]));

                if (i == 0)
                    newDeclaration = newDeclaration.WithLeadingTrivia(declaration.GetLeadingTrivia());

                if (i == variables.Count - 1)
                    newDeclaration = newDeclaration.WithTrailingTrivia(declaration.GetTrailingTrivia());

                yield return newDeclaration.WithFormatterAnnotation();
            }
        }

        private static IEnumerable<EventFieldDeclarationSyntax> SplitEventFieldDeclaration(EventFieldDeclarationSyntax fieldDeclaration)
        {
            SeparatedSyntaxList<VariableDeclaratorSyntax> variables = fieldDeclaration.Declaration.Variables;

            EventFieldDeclarationSyntax fieldDeclaration2 = fieldDeclaration.WithoutTrivia();

            for (int i = 0; i < variables.Count; i++)
            {
                EventFieldDeclarationSyntax newDeclaration = EventFieldDeclaration(
                    fieldDeclaration2.AttributeLists,
                    fieldDeclaration2.Modifiers,
                    VariableDeclaration(
                        fieldDeclaration2.Declaration.Type,
                        variables[i]));

                if (i == 0)
                    newDeclaration = newDeclaration.WithLeadingTrivia(fieldDeclaration.GetLeadingTrivia());

                if (i == variables.Count - 1)
                    newDeclaration = newDeclaration.WithTrailingTrivia(fieldDeclaration.GetTrailingTrivia());

                yield return newDeclaration.WithFormatterAnnotation();
            }
        }
    }
}
