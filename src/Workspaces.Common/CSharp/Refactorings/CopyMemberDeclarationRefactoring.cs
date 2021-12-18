// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.CSharp.Syntax;
using static Roslynator.CSharp.CSharpFactory;

namespace Roslynator.CSharp.Refactorings
{
    internal static class CopyMemberDeclarationRefactoring
    {
        public static async Task<Document> RefactorAsync(
            Document document,
            MemberDeclarationSyntax member,
            bool copyAfter = true,
            CancellationToken cancellationToken = default)
        {
            MemberDeclarationSyntax newMember = member;

            SyntaxToken identifier = GetIdentifier(member);

            if (identifier != default)
            {
                SemanticModel semanticModel = await document.GetSemanticModelAsync(cancellationToken).ConfigureAwait(false);

                string newName = identifier.ValueText;

                if (!member.IsKind(SyntaxKind.ConstructorDeclaration))
                {
                    newName = NameGenerator.Default.EnsureUniqueName(newName, semanticModel, member.SpanStart);

                    ISymbol symbol = semanticModel.GetDeclaredSymbol(member, cancellationToken);

                    ImmutableArray<SyntaxNode> references = await SyntaxFinder.FindReferencesAsync(symbol, document.Solution(), documents: ImmutableHashSet.Create(document), cancellationToken: cancellationToken).ConfigureAwait(false);

                    SyntaxToken newIdentifier = SyntaxFactory.Identifier(newName);

                    newMember = member.ReplaceNodes(
                        references.Where(n => member.Contains(n)),
                        (n, _) =>
                        {
                            if (n is IdentifierNameSyntax identifierName)
                            {
                                return identifierName.WithIdentifier(newIdentifier.WithTriviaFrom(identifierName.Identifier));
                            }
                            else
                            {
                                SyntaxDebug.Assert(n.IsKind(SyntaxKind.ThisConstructorInitializer, SyntaxKind.BaseConstructorInitializer), n);

                                return n;
                            }
                        });

                    newMember = SetIdentifier(newMember, newIdentifier.WithRenameAnnotation());
                }
            }
            else
            {
                newMember = newMember.WithNavigationAnnotation();
            }

            MemberDeclarationListInfo memberList = SyntaxInfo.MemberDeclarationListInfo(member.Parent);

            int index = memberList.IndexOf(member);

            if (index == 0)
            {
                if (copyAfter)
                {
                    SyntaxToken? openBrace = memberList.OpenBraceToken;

                    if (openBrace != null
                        && openBrace.Value.GetFullSpanEndLine() == member.GetFullSpanStartLine())
                    {
                        newMember = newMember.WithLeadingTrivia(member.GetLeadingTrivia().Insert(0, NewLine()));
                    }
                }
                else
                {
                    SyntaxToken? closeBrace = memberList.CloseBraceToken;

                    if (closeBrace != null)
                        newMember = newMember.WithTrailingTrivia(member.GetTrailingTrivia().Add(NewLine()));
                }
            }

            int insertIndex = (copyAfter) ? index + 1 : index;

            return await document.ReplaceMembersAsync(memberList, memberList.Members.Insert(insertIndex, newMember), cancellationToken).ConfigureAwait(false);
        }

        public static Task<Document> RefactorAsync(
            Document document,
            LocalFunctionStatementSyntax localFunction,
            bool copyAfter = true,
            CancellationToken cancellationToken = default)
        {
            StatementListInfo statementsInfos = SyntaxInfo.StatementListInfo(localFunction);

            SyntaxList<StatementSyntax> statements = statementsInfos.Statements;
            int index = statements.IndexOf(localFunction);

            if (index == 0)
            {
                if (copyAfter)
                {
                    if (statementsInfos.ParentAsBlock.OpenBraceToken.GetFullSpanEndLine() == localFunction.GetFullSpanStartLine())
                    {
                        localFunction = localFunction.WithLeadingTrivia(localFunction.GetLeadingTrivia().Insert(0, NewLine()));
                    }
                }
                else
                {
                    localFunction = localFunction.WithTrailingTrivia(localFunction.GetTrailingTrivia().Add(NewLine()));
                }
            }

            int insertIndex = (copyAfter) ? index + 1 : index;

            SyntaxList<StatementSyntax> newStatements = statements.Insert(insertIndex, localFunction.WithNavigationAnnotation());

            return document.ReplaceStatementsAsync(statementsInfos, newStatements, cancellationToken);
        }

        private static SyntaxToken GetIdentifier(SyntaxNode node)
        {
            switch (node.Kind())
            {
                case SyntaxKind.ClassDeclaration:
                    return ((ClassDeclarationSyntax)node).Identifier;
                case SyntaxKind.RecordDeclaration:
                case SyntaxKind.RecordStructDeclaration:
                    return ((RecordDeclarationSyntax)node).Identifier;
                case SyntaxKind.StructDeclaration:
                    return ((StructDeclarationSyntax)node).Identifier;
                case SyntaxKind.InterfaceDeclaration:
                    return ((InterfaceDeclarationSyntax)node).Identifier;
                case SyntaxKind.EnumDeclaration:
                    return ((EnumDeclarationSyntax)node).Identifier;
                case SyntaxKind.DelegateDeclaration:
                    return ((DelegateDeclarationSyntax)node).Identifier;
                case SyntaxKind.MethodDeclaration:
                    return ((MethodDeclarationSyntax)node).Identifier;
                case SyntaxKind.ConstructorDeclaration:
                    return ((ConstructorDeclarationSyntax)node).Identifier;
                case SyntaxKind.PropertyDeclaration:
                    return ((PropertyDeclarationSyntax)node).Identifier;
                case SyntaxKind.EventDeclaration:
                    return ((EventDeclarationSyntax)node).Identifier;
            }

            return default;
        }

        private static MemberDeclarationSyntax SetIdentifier(MemberDeclarationSyntax member, SyntaxToken identifier)
        {
            switch (member.Kind())
            {
                case SyntaxKind.ClassDeclaration:
                    return ((ClassDeclarationSyntax)member).WithIdentifier(identifier);
                case SyntaxKind.RecordDeclaration:
                case SyntaxKind.RecordStructDeclaration:
                    return ((RecordDeclarationSyntax)member).WithIdentifier(identifier);
                case SyntaxKind.StructDeclaration:
                    return ((StructDeclarationSyntax)member).WithIdentifier(identifier);
                case SyntaxKind.InterfaceDeclaration:
                    return ((InterfaceDeclarationSyntax)member).WithIdentifier(identifier);
                case SyntaxKind.EnumDeclaration:
                    return ((EnumDeclarationSyntax)member).WithIdentifier(identifier);
                case SyntaxKind.DelegateDeclaration:
                    return ((DelegateDeclarationSyntax)member).WithIdentifier(identifier);
                case SyntaxKind.MethodDeclaration:
                    return ((MethodDeclarationSyntax)member).WithIdentifier(identifier);
                case SyntaxKind.ConstructorDeclaration:
                    return ((ConstructorDeclarationSyntax)member).WithIdentifier(identifier);
                case SyntaxKind.PropertyDeclaration:
                    return ((PropertyDeclarationSyntax)member).WithIdentifier(identifier);
                case SyntaxKind.EventDeclaration:
                    return ((EventDeclarationSyntax)member).WithIdentifier(identifier);
            }

            throw new InvalidOperationException();
        }
    }
}
