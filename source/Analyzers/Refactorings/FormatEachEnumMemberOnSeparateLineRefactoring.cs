// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Text;

namespace Roslynator.CSharp.Refactorings
{
    internal static class FormatEachEnumMemberOnSeparateLineRefactoring
    {
        public static void Analyze(SyntaxNodeAnalysisContext context, EnumDeclarationSyntax enumDeclaration)
        {
            SeparatedSyntaxList<EnumMemberDeclarationSyntax> members = enumDeclaration.Members;

            if (members.Count > 1)
            {
                int previousIndex = members[0].GetSpanStartLine();

                for (int i = 1; i < members.Count; i++)
                {
                    if (members[i].GetSpanStartLine() == previousIndex)
                    {
                        TextSpan span = TextSpan.FromBounds(
                            members.First().Span.Start,
                            members.Last().Span.End);

                        context.ReportDiagnostic(
                            DiagnosticDescriptors.FormatEachEnumMemberOnSeparateLine,
                            Location.Create(enumDeclaration.SyntaxTree, span));

                        return;
                    }

                    previousIndex = members[i].GetSpanEndLine();
                }
            }
        }

        public static async Task<Document> RefactorAsync(
            Document document,
            EnumDeclarationSyntax enumDeclaration,
            CancellationToken cancellationToken)
        {
            var rewriter = new EnumDeclarationSyntaxRewriter(enumDeclaration);

            SyntaxNode newNode = rewriter.Visit(enumDeclaration)
                .WithFormatterAnnotation();

            return await document.ReplaceNodeAsync(enumDeclaration, newNode).ConfigureAwait(false);
        }

        private class EnumDeclarationSyntaxRewriter : CSharpSyntaxRewriter
        {
            private readonly SyntaxToken[] _separators;

            public EnumDeclarationSyntaxRewriter(EnumDeclarationSyntax enumDeclaration)
            {
                _separators = enumDeclaration.Members.GetSeparators().ToArray();
            }

            public override SyntaxToken VisitToken(SyntaxToken token)
            {
                if (_separators.Contains(token))
                {
                    SyntaxTriviaList triviaList = token.TrailingTrivia;

                    if (!triviaList.Contains(SyntaxKind.EndOfLineTrivia))
                        return token.WithTrailingTrivia(triviaList.TrimEnd().Add(CSharpFactory.NewLineTrivia()));
                }

                return base.VisitToken(token);
            }
        }
    }
}
