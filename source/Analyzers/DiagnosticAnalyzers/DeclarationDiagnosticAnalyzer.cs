// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Roslynator.CSharp.DiagnosticAnalyzers
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class DeclarationDiagnosticAnalyzer : BaseDiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
            => ImmutableArray.Create(DiagnosticDescriptors.AddEmptyLineBetweenDeclarations);

        public override void Initialize(AnalysisContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            context.RegisterSyntaxNodeAction(f => AnalyzeSyntaxNode(f),
                SyntaxKind.ConstructorDeclaration,
                SyntaxKind.DestructorDeclaration,
                SyntaxKind.EventDeclaration,
                SyntaxKind.PropertyDeclaration,
                SyntaxKind.IndexerDeclaration,
                SyntaxKind.MethodDeclaration,
                SyntaxKind.ConversionOperatorDeclaration,
                SyntaxKind.OperatorDeclaration,
                SyntaxKind.EnumDeclaration,
                SyntaxKind.InterfaceDeclaration,
                SyntaxKind.StructDeclaration,
                SyntaxKind.ClassDeclaration,
                SyntaxKind.NamespaceDeclaration);
        }

        private void AnalyzeSyntaxNode(SyntaxNodeAnalysisContext context)
        {
            if (GeneratedCodeAnalyzer?.IsGeneratedCode(context) == true)
                return;

            var declaration = (MemberDeclarationSyntax)context.Node;

            if (declaration.Parent?.IsKind(SyntaxKind.CompilationUnit) == false)
            {
                TokenPair tokenPair = GetTokenPair(declaration);

                if (!tokenPair.OpenToken.IsKind(SyntaxKind.None)
                    && !tokenPair.OpenToken.IsMissing
                    && !tokenPair.CloseToken.IsKind(SyntaxKind.None)
                    && !tokenPair.CloseToken.IsMissing)
                {
                    int closeTokenLine = tokenPair.CloseToken.GetSpanEndLine();

                    if (tokenPair.OpenToken.GetSpanEndLine() != closeTokenLine)
                    {
                        MemberDeclarationSyntax nextDeclaration = GetNextDeclaration(declaration);

                        if (nextDeclaration != null)
                        {
                            int diff = nextDeclaration.GetSpanStartLine() - closeTokenLine;

                            if (diff < 2)
                            {
                                SyntaxTrivia trivia = declaration.GetTrailingTrivia().LastOrDefault();

                                if (trivia.IsKind(SyntaxKind.EndOfLineTrivia))
                                {
                                    context.ReportDiagnostic(
                                        DiagnosticDescriptors.AddEmptyLineBetweenDeclarations,
                                        trivia.GetLocation());
                                }
                                else
                                {
                                    context.ReportDiagnostic(
                                        DiagnosticDescriptors.AddEmptyLineBetweenDeclarations,
                                        tokenPair.CloseToken.GetLocation());
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
                        Debug.Assert(false, node.Kind().ToString());

                        return default(TokenPair);
                    }
            }
        }
    }
}
