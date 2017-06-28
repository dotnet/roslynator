// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Immutable;
using System.Composition;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Roslynator.CSharp.CodeFixes
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(TypeParameterConstraintClauseCodeFixProvider))]
    [Shared]
    public class TypeParameterConstraintClauseCodeFixProvider : BaseCodeFixProvider
    {
        public sealed override ImmutableArray<string> FixableDiagnosticIds
        {
            get { return ImmutableArray.Create(CompilerDiagnosticIdentifiers.ConstraintsAreNotAllowedOnNonGenericDeclarations); }
        }

        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            if (!Settings.IsCodeFixEnabled(CodeFixIdentifiers.RemoveConstraintClause))
                return;

            SyntaxNode root = await context.GetSyntaxRootAsync().ConfigureAwait(false);

            TypeParameterConstraintClauseSyntax constraintClause = root
                .FindNode(context.Span, getInnermostNodeForTie: true)?
                .FirstAncestorOrSelf<TypeParameterConstraintClauseSyntax>();

            Debug.Assert(constraintClause != null, $"{nameof(constraintClause)} is null");

            if (constraintClause == null)
                return;

            foreach (Diagnostic diagnostic in context.Diagnostics)
            {
                switch (diagnostic.Id)
                {
                    case CompilerDiagnosticIdentifiers.ConstraintsAreNotAllowedOnNonGenericDeclarations:
                        {
                            CodeAction codeAction = CodeAction.Create(
                                "Remove constraint",
                                cancellationToken => RefactorAsync(context.Document, constraintClause, cancellationToken),
                                GetEquivalenceKey(diagnostic));

                            context.RegisterCodeFix(codeAction, diagnostic);
                            break;
                        }
                }
            }
        }

        private static Task<Document> RefactorAsync(
            Document document,
            TypeParameterConstraintClauseSyntax constraintClause,
            CancellationToken cancellationToken)
        {
            SyntaxNode parent = constraintClause.Parent;

            SyntaxNode newNode = GetNewNode(parent, constraintClause);

            return document.ReplaceNodeAsync(parent, newNode, cancellationToken);
        }

        private static SyntaxNode GetNewNode(
            SyntaxNode node,
            TypeParameterConstraintClauseSyntax constraintClause)
        {
            SyntaxToken token = constraintClause.WhereKeyword.GetPreviousToken();

            switch (node.Kind())
            {
                case SyntaxKind.ClassDeclaration:
                    {
                        var classDeclaration = (ClassDeclarationSyntax)node;

                        SyntaxTriviaList trivia = token.TrailingTrivia.EmptyIfWhitespace()
                            .AddRange(constraintClause.GetLeadingTrivia().EmptyIfWhitespace())
                            .AddRange(classDeclaration.ConstraintClauses.Last().GetTrailingTrivia());

                        return classDeclaration
                            .ReplaceToken(token, token.WithTrailingTrivia(trivia))
                            .WithConstraintClauses(default(SyntaxList<TypeParameterConstraintClauseSyntax>));
                    }
                case SyntaxKind.InterfaceDeclaration:
                    {
                        var interfaceDeclaration = (InterfaceDeclarationSyntax)node;

                        SyntaxTriviaList trivia = token.TrailingTrivia.EmptyIfWhitespace()
                            .AddRange(constraintClause.GetLeadingTrivia().EmptyIfWhitespace())
                            .AddRange(interfaceDeclaration.ConstraintClauses.Last().GetTrailingTrivia());

                        return interfaceDeclaration
                            .ReplaceToken(token, token.WithTrailingTrivia(trivia))
                            .WithConstraintClauses(default(SyntaxList<TypeParameterConstraintClauseSyntax>));
                    }
                case SyntaxKind.StructDeclaration:
                    {
                        var structDeclaration = (StructDeclarationSyntax)node;

                        SyntaxTriviaList trivia = token.TrailingTrivia.EmptyIfWhitespace()
                            .AddRange(constraintClause.GetLeadingTrivia().EmptyIfWhitespace())
                            .AddRange(structDeclaration.ConstraintClauses.Last().GetTrailingTrivia());

                        return structDeclaration
                            .ReplaceToken(token, token.WithTrailingTrivia(trivia))
                            .WithConstraintClauses(default(SyntaxList<TypeParameterConstraintClauseSyntax>));
                    }
                case SyntaxKind.DelegateDeclaration:
                    {
                        var delegateDeclaration = (DelegateDeclarationSyntax)node;

                        SyntaxTriviaList trivia = token.TrailingTrivia.EmptyIfWhitespace()
                            .AddRange(constraintClause.GetLeadingTrivia().EmptyIfWhitespace())
                            .AddRange(delegateDeclaration.ConstraintClauses.Last().GetTrailingTrivia());

                        return delegateDeclaration
                            .ReplaceToken(token, token.WithTrailingTrivia(trivia))
                            .WithConstraintClauses(default(SyntaxList<TypeParameterConstraintClauseSyntax>));
                    }
                case SyntaxKind.MethodDeclaration:
                    {
                        var methodDeclaration = (MethodDeclarationSyntax)node;

                        SyntaxTriviaList trivia = token.TrailingTrivia.EmptyIfWhitespace()
                            .AddRange(constraintClause.GetLeadingTrivia().EmptyIfWhitespace())
                            .AddRange(methodDeclaration.ConstraintClauses.Last().GetTrailingTrivia());

                        return methodDeclaration
                            .ReplaceToken(token, token.WithTrailingTrivia(trivia))
                            .WithConstraintClauses(default(SyntaxList<TypeParameterConstraintClauseSyntax>));
                    }
                case SyntaxKind.LocalFunctionStatement:
                    {
                        var localFunctionStatement = (LocalFunctionStatementSyntax)node;

                        SyntaxTriviaList trivia = token.TrailingTrivia.EmptyIfWhitespace()
                            .AddRange(constraintClause.GetLeadingTrivia().EmptyIfWhitespace())
                            .AddRange(localFunctionStatement.ConstraintClauses.Last().GetTrailingTrivia());

                        return localFunctionStatement
                            .ReplaceToken(token, token.WithTrailingTrivia(trivia))
                            .WithConstraintClauses(default(SyntaxList<TypeParameterConstraintClauseSyntax>));
                    }
            }

            return node;
        }
    }
}
