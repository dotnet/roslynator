// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using Roslynator.CSharp.Extensions;
using Roslynator.Extensions;
using Roslynator.Text.Extensions;

namespace Roslynator.CSharp.Refactorings.AddExceptionToDocumentationComment
{
    internal static class AddExceptionToDocumentationCommentRefactoring
    {
        public static async Task ComputeRefactoringAsync(RefactoringContext context, ThrowStatementSyntax throwStatement)
        {
            ExpressionSyntax expression = throwStatement.Expression;

            if (expression?.IsMissing == false
                && context.Span.IsContainedInSpanOrBetweenSpans(throwStatement))
            {
                await ComputeRefactoringAsync(context, throwStatement, expression).ConfigureAwait(false);
            }
        }

        public static async Task ComputeRefactoringAsync(RefactoringContext context, ThrowExpressionSyntax throwExpression)
        {
            ExpressionSyntax expression = throwExpression.Expression;

            if (expression?.IsMissing == false
                && context.Span.IsContainedInSpanOrBetweenSpans(throwExpression))
            {
                await ComputeRefactoringAsync(context, throwExpression, expression).ConfigureAwait(false);
            }
        }

        private static async Task ComputeRefactoringAsync(RefactoringContext context, SyntaxNode node, ExpressionSyntax expression)
        {
            SemanticModel semanticModel = await context.GetSemanticModelAsync().ConfigureAwait(false);

            ITypeSymbol exceptionSymbol = semanticModel.GetTypeSymbol(expression, context.CancellationToken);

            if (exceptionSymbol?.IsErrorType() == false
                && Symbol.IsException(exceptionSymbol, semanticModel))
            {
                ISymbol declarationSymbol = GetDeclarationSymbol(node.SpanStart, semanticModel, context.CancellationToken);

                if (declarationSymbol != null)
                {
                    var containingMember = await declarationSymbol
                        .DeclaringSyntaxReferences[0]
                        .GetSyntaxAsync(context.CancellationToken)
                        .ConfigureAwait(false) as MemberDeclarationSyntax;

                    if (containingMember != null)
                    {
                        SyntaxTrivia trivia = containingMember.GetSingleLineDocumentationComment();

                        if (trivia.IsKind(SyntaxKind.SingleLineDocumentationCommentTrivia))
                        {
                            var comment = trivia.GetStructure() as DocumentationCommentTriviaSyntax;

                            if (comment?.IsKind(SyntaxKind.SingleLineDocumentationCommentTrivia) == true
                                && !ContainsException(comment, exceptionSymbol, semanticModel, context.CancellationToken))
                            {
                                ThrowInfo throwInfo1 = ThrowInfo.Create(node, exceptionSymbol);

                                IParameterSymbol parameterSymbol = throwInfo1.GetParameterSymbol(declarationSymbol, semanticModel, context.CancellationToken);

                                context.RegisterRefactoring(
                                    "Add exception to documentation comment",
                                    cancellationToken => RefactorAsync(context.Document, trivia, exceptionSymbol, parameterSymbol, cancellationToken));

                                foreach (ThrowInfo throwInfo2 in GetOtherUndocumentedExceptions(containingMember, f => node != f, semanticModel, context.CancellationToken))
                                {
                                    if (throwInfo2.ExceptionSymbol != exceptionSymbol)
                                    {
                                        var infos = new List<ThrowInfo>();
                                        infos.Add(throwInfo1);
                                        infos.Add(throwInfo2);

                                        context.RegisterRefactoring(
                                            "Add all exceptions to documentation comment",
                                            cancellationToken => RefactorAsync(context.Document, infos, containingMember, declarationSymbol, trivia, cancellationToken));

                                        break;
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        private static IEnumerable<ThrowInfo> GetOtherUndocumentedExceptions(
            MemberDeclarationSyntax containingMember,
            Func<SyntaxNode, bool> predicate,
            SemanticModel semanticModel,
            CancellationToken cancellationToken)
        {
            foreach (SyntaxNode descendant in containingMember.DescendantNodes())
            {
                switch (descendant.Kind())
                {
                    case SyntaxKind.ThrowStatement:
                        {
                            if (predicate(descendant))
                            {
                                var throwStatement = (ThrowStatementSyntax)descendant;

                                ThrowInfo info = GetUndocumentedExceptionInfo(descendant, throwStatement.Expression, containingMember, semanticModel, cancellationToken);

                                if (info != null)
                                    yield return info;
                            }

                            break;
                        }
                    case SyntaxKind.ThrowExpression:
                        {
                            if (predicate(descendant))
                            {
                                var throwExpression = (ThrowExpressionSyntax)descendant;
                                ThrowInfo info = GetUndocumentedExceptionInfo(descendant, throwExpression.Expression, containingMember, semanticModel, cancellationToken);

                                if (info != null)
                                    yield return info;
                            }

                            break;
                        }
                }
            }
        }

        private static ThrowInfo GetUndocumentedExceptionInfo(
        SyntaxNode node,
        ExpressionSyntax expression,
        MemberDeclarationSyntax containingMember,
        SemanticModel semanticModel,
        CancellationToken cancellationToken)
        {
            if (expression != null)
            {
                ITypeSymbol typeSymbol = semanticModel.GetTypeSymbol(expression, cancellationToken);

                if (typeSymbol?.IsErrorType() == false
                    && Symbol.IsException(typeSymbol, semanticModel))
                {
                    SyntaxTrivia trivia = containingMember.GetSingleLineDocumentationComment();

                    if (trivia.IsKind(SyntaxKind.SingleLineDocumentationCommentTrivia))
                    {
                        var comment = trivia.GetStructure() as DocumentationCommentTriviaSyntax;

                        if (comment?.IsKind(SyntaxKind.SingleLineDocumentationCommentTrivia) == true
                            && !ContainsException(comment, typeSymbol, semanticModel, cancellationToken))
                        {
                            return ThrowInfo.Create(node, typeSymbol);
                        }
                    }
                }
            }

            return null;
        }

        private static bool ContainsException(DocumentationCommentTriviaSyntax comment, ITypeSymbol exceptionSymbol, SemanticModel semanticModel, CancellationToken cancellationToken)
        {
            foreach (XmlElementSyntax xmlElement in comment.Exceptions())
            {
                XmlElementStartTagSyntax startTag = xmlElement.StartTag;

                if (startTag != null)
                {
                    foreach (XmlAttributeSyntax xmlAttribute in startTag.Attributes)
                    {
                        if (xmlAttribute.IsKind(SyntaxKind.XmlCrefAttribute))
                        {
                            var xmlCrefAttribute = (XmlCrefAttributeSyntax)xmlAttribute;

                            CrefSyntax cref = xmlCrefAttribute.Cref;

                            if (cref != null)
                            {
                                ISymbol symbol = semanticModel.GetSymbol(cref, cancellationToken);

                                if (exceptionSymbol.Equals(symbol))
                                    return true;
                            }
                        }
                    }
                }
            }

            return false;
        }

        internal static ISymbol GetDeclarationSymbol(
            int position,
            SemanticModel semanticModel,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            ISymbol symbol = semanticModel.GetEnclosingSymbol(position, cancellationToken);

            if (symbol?.IsMethod() == true)
            {
                var methodsymbol = (IMethodSymbol)symbol;

                if (methodsymbol.MethodKind == MethodKind.Ordinary)
                {
                    if (methodsymbol.PartialImplementationPart != null)
                        symbol = methodsymbol.PartialImplementationPart;
                }
                else if (methodsymbol.AssociatedSymbol != null)
                {
                    symbol = methodsymbol.AssociatedSymbol;
                }
            }

            return symbol;
        }

        private static async Task<Document> RefactorAsync(
            Document document,
            SyntaxTrivia trivia,
            ITypeSymbol exceptionSymbol,
            IParameterSymbol parameterSymbol,
            CancellationToken cancellationToken)
        {
            SourceText sourceText = await document.GetTextAsync(cancellationToken).ConfigureAwait(false);

            SemanticModel semanticModel = await document.GetSemanticModelAsync(cancellationToken).ConfigureAwait(false);

            string indent = GetIndent(trivia, sourceText, cancellationToken);

            var sb = new StringBuilder(indent);

            AppendExceptionDocumentation(trivia, exceptionSymbol, parameterSymbol, semanticModel, ref sb);

            var textChange = new TextChange(new TextSpan(trivia.FullSpan.End, 0), sb.ToString());

            SourceText newSourceText = sourceText.WithChanges(textChange);

            return document.WithText(newSourceText);
        }

        private static async Task<Document> RefactorAsync(
            Document document,
            List<ThrowInfo> infos,
            MemberDeclarationSyntax containingMember,
            ISymbol declarationSymbol,
            SyntaxTrivia trivia,
            CancellationToken cancellationToken)
        {
            SourceText sourceText = await document.GetTextAsync(cancellationToken).ConfigureAwait(false);

            SemanticModel semanticModel = await document.GetSemanticModelAsync(cancellationToken).ConfigureAwait(false);

            foreach (ThrowInfo info in GetOtherUndocumentedExceptions(containingMember, node => !infos.Any(f => f.Node == node), semanticModel, cancellationToken))
            {
                if (!infos.Any(f => f.ExceptionSymbol == info.ExceptionSymbol))
                {
                    infos.Add(info);
                }
            }

            string indent = GetIndent(trivia, sourceText, cancellationToken);

            var sb = new StringBuilder();

            foreach (ThrowInfo info in infos)
            {
                sb.Append(indent);

                IParameterSymbol parameterSymbol = info.GetParameterSymbol(declarationSymbol, semanticModel, cancellationToken);

                AppendExceptionDocumentation(trivia, info.ExceptionSymbol, parameterSymbol, semanticModel, ref sb);
            }

            var textChange = new TextChange(new TextSpan(trivia.FullSpan.End, 0), sb.ToString());

            SourceText newSourceText = sourceText.WithChanges(textChange);

            return document.WithText(newSourceText);
        }

        private static string GetIndent(SyntaxTrivia trivia, SourceText sourceText, CancellationToken cancellationToken)
        {
            int lineIndex = trivia.GetSpanStartLine(cancellationToken);

            return new string(' ', trivia.FullSpan.Start - sourceText.Lines[lineIndex].Start);
        }

        private static void AppendExceptionDocumentation(
            SyntaxTrivia trivia,
            ITypeSymbol exceptionSymbol,
            IParameterSymbol parameterSymbol,
            SemanticModel semanticModel,
            ref StringBuilder sb)
        {
            sb.Append("/// <exception cref=\"");

            foreach (char ch in exceptionSymbol.ToMinimalDisplayString(semanticModel, trivia.FullSpan.End))
            {
                if (ch == '<')
                {
                    sb.Append('{');
                }
                else if (ch == '>')
                {
                    sb.Append('}');
                }
                else
                {
                    sb.Append(ch);
                }
            }

            sb.Append("\">");

            if (parameterSymbol != null)
            {
                sb.Append("<paramref name=\"");
                sb.Append(parameterSymbol.Name);
                sb.Append("\"/>");

                if (exceptionSymbol.Equals(semanticModel.GetTypeByMetadataName(MetadataNames.System_ArgumentNullException)))
                    sb.Append("\"/> is <c>null</c>.");
            }

            sb.Append("</exception>");
            sb.AppendLine();
        }
    }
}