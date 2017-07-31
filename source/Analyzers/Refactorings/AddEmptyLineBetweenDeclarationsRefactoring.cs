// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Roslynator.CSharp;

namespace Roslynator.CSharp.Refactorings
{
    internal static class AddEmptyLineBetweenDeclarationsRefactoring
    {
        public static void AnalyzeMemberDeclaration(SyntaxNodeAnalysisContext context)
        {
            var declaration = (MemberDeclarationSyntax)context.Node;

            if (!declaration.IsParentKind(SyntaxKind.CompilationUnit))
            {
                TokenPair tokenPair = GetTokenPair(declaration);
                SyntaxToken openToken = tokenPair.OpenToken;
                SyntaxToken closeToken = tokenPair.CloseToken;

                if (!openToken.IsKind(SyntaxKind.None)
                    && !openToken.IsMissing
                    && !closeToken.IsKind(SyntaxKind.None)
                    && !closeToken.IsMissing)
                {
                    int closeTokenLine = closeToken.GetSpanEndLine();

                    if (openToken.GetSpanEndLine() != closeTokenLine)
                    {
                        MemberDeclarationSyntax nextDeclaration = GetNextDeclaration(declaration);

                        if (nextDeclaration != null)
                        {
                            int diff = nextDeclaration.GetSpanStartLine() - closeTokenLine;

                            if (diff < 2)
                            {
                                SyntaxTrivia trivia = declaration.GetTrailingTrivia().LastOrDefault();

                                if (trivia.IsEndOfLineTrivia())
                                {
                                    context.ReportDiagnostic(
                                        DiagnosticDescriptors.AddEmptyLineBetweenDeclarations,
                                        trivia);
                                }
                                else
                                {
                                    context.ReportDiagnostic(
                                        DiagnosticDescriptors.AddEmptyLineBetweenDeclarations,
                                        closeToken);
                                }
                            }
                        }
                    }
                }
            }
        }

        private static MemberDeclarationSyntax GetNextDeclaration(MemberDeclarationSyntax declaration)
        {
            var containingDeclaration = declaration.Parent as MemberDeclarationSyntax;
            if (containingDeclaration != null)
            {
                SyntaxList<MemberDeclarationSyntax> members = containingDeclaration.GetMembers();

                if (members.Count > 1)
                {
                    int index = members.IndexOf(declaration);

                    if (index != (members.Count - 1))
                        return members[index + 1];
                }
            }

            return null;
        }

        private static TokenPair GetTokenPair(SyntaxNode node)
        {
            switch (node.Kind())
            {
                case SyntaxKind.ConstructorDeclaration:
                    {
                        var declaration = (ConstructorDeclarationSyntax)node;

                        if (declaration.Body != null)
                            return new TokenPair(declaration.Body);

                        return default(TokenPair);
                    }
                case SyntaxKind.DestructorDeclaration:
                    {
                        var declaration = (DestructorDeclarationSyntax)node;

                        if (declaration.Body != null)
                            return new TokenPair(declaration.Body);

                        return default(TokenPair);
                    }
                case SyntaxKind.EventDeclaration:
                    {
                        var declaration = (EventDeclarationSyntax)node;

                        if (declaration.AccessorList != null)
                            return new TokenPair(declaration.AccessorList);

                        return default(TokenPair);
                    }
                case SyntaxKind.PropertyDeclaration:
                    {
                        var declaration = (PropertyDeclarationSyntax)node;

                        if (declaration.AccessorList != null)
                            return new TokenPair(declaration.AccessorList);

                        return default(TokenPair);
                    }
                case SyntaxKind.IndexerDeclaration:
                    {
                        var declaration = (IndexerDeclarationSyntax)node;

                        if (declaration.AccessorList != null)
                            return new TokenPair(declaration.AccessorList);

                        return default(TokenPair);
                    }
                case SyntaxKind.MethodDeclaration:
                    {
                        var declaration = (MethodDeclarationSyntax)node;

                        if (declaration.Body != null)
                            return new TokenPair(declaration.Body);

                        return default(TokenPair);
                    }
                case SyntaxKind.ConversionOperatorDeclaration:
                    {
                        var declaration = (ConversionOperatorDeclarationSyntax)node;

                        if (declaration.Body != null)
                            return new TokenPair(declaration.Body);

                        return default(TokenPair);
                    }
                case SyntaxKind.OperatorDeclaration:
                    {
                        var declaration = (OperatorDeclarationSyntax)node;

                        if (declaration.Body != null)
                            return new TokenPair(declaration.Body);

                        return default(TokenPair);
                    }
                case SyntaxKind.EnumDeclaration:
                    {
                        var declaration = (EnumDeclarationSyntax)node;

                        return new TokenPair(declaration.OpenBraceToken, declaration.CloseBraceToken);
                    }
                case SyntaxKind.InterfaceDeclaration:
                    {
                        var declaration = (InterfaceDeclarationSyntax)node;

                        return new TokenPair(declaration.OpenBraceToken, declaration.CloseBraceToken);
                    }
                case SyntaxKind.StructDeclaration:
                    {
                        var declaration = (StructDeclarationSyntax)node;

                        return new TokenPair(declaration.OpenBraceToken, declaration.CloseBraceToken);
                    }
                case SyntaxKind.ClassDeclaration:
                    {
                        var declaration = (ClassDeclarationSyntax)node;

                        return new TokenPair(declaration.OpenBraceToken, declaration.CloseBraceToken);
                    }
                case SyntaxKind.NamespaceDeclaration:
                    {
                        var declaration = (NamespaceDeclarationSyntax)node;

                        return new TokenPair(declaration.OpenBraceToken, declaration.CloseBraceToken);
                    }
                default:
                    {
                        Debug.Fail(node.Kind().ToString());
                        return default(TokenPair);
                    }
            }
        }

        public static Task<Document> RefactorAsync(
            Document document,
            MemberDeclarationSyntax memberDeclaration,
            CancellationToken cancellationToken)
        {
            MemberDeclarationSyntax newNode = memberDeclaration
                .WithTrailingTrivia(memberDeclaration.GetTrailingTrivia().Add(CSharpFactory.NewLine()));

            return document.ReplaceNodeAsync(memberDeclaration, newNode, cancellationToken);
        }

        private struct TokenPair
        {
            public TokenPair(BlockSyntax block)
            {
                OpenToken = block.OpenBraceToken;
                CloseToken = block.CloseBraceToken;
            }

            public TokenPair(AccessorListSyntax accessorList)
            {
                OpenToken = accessorList.OpenBraceToken;
                CloseToken = accessorList.CloseBraceToken;
            }

            public TokenPair(SyntaxToken openToken, SyntaxToken closeToken)
            {
                OpenToken = openToken;
                CloseToken = closeToken;
            }

            public SyntaxToken OpenToken { get; }
            public SyntaxToken CloseToken { get; }
        }
    }
}
