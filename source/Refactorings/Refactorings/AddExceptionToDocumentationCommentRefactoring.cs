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

namespace Roslynator.CSharp.Refactorings
{
    internal static class AddExceptionToDocumentationCommentRefactoring
    {
        public static async Task ComputeRefactoringAsync(RefactoringContext context, ThrowStatementSyntax throwStatement)
        {
            ExpressionSyntax expression = throwStatement.Expression;

            if (expression?.IsMissing == false
                && context.Span.IsContainedInSpanOrBetweenSpans(throwStatement))
            {
                SemanticModel semanticModel = await context.GetSemanticModelAsync().ConfigureAwait(false);

                ITypeSymbol exceptionSymbol = semanticModel.GetTypeSymbol(expression, context.CancellationToken);

                if (exceptionSymbol?.IsErrorType() == false
                    && Symbol.IsException(exceptionSymbol, semanticModel))
                {
                    ISymbol declarationSymbol = GetDeclarationSymbol(throwStatement, semanticModel, context.CancellationToken);

                    if (declarationSymbol != null)
                    {
                        SyntaxNode node = await declarationSymbol
                            .DeclaringSyntaxReferences[0]
                            .GetSyntaxAsync(context.CancellationToken)
                            .ConfigureAwait(false);

                        var containingMember = node as MemberDeclarationSyntax;

                        if (containingMember != null)
                        {
                            SyntaxTrivia trivia = containingMember.GetSingleLineDocumentationComment();

                            if (trivia.IsKind(SyntaxKind.SingleLineDocumentationCommentTrivia))
                            {
                                var comment = trivia.GetStructure() as DocumentationCommentTriviaSyntax;

                                if (comment?.IsKind(SyntaxKind.SingleLineDocumentationCommentTrivia) == true
                                    && !ContainsException(comment, exceptionSymbol, semanticModel, context.CancellationToken))
                                {
                                    IParameterSymbol parameterSymbol = GetParameterSymbol(throwStatement, exceptionSymbol, declarationSymbol, semanticModel, context.CancellationToken);

                                    context.RegisterRefactoring(
                                        "Add exception to documentation comment",
                                        cancellationToken => RefactorAsync(context.Document, trivia, exceptionSymbol, parameterSymbol, cancellationToken));

                                    foreach (ThrowInfo info in GetOtherUndocumentedExceptions(throwStatement, containingMember, semanticModel, context.CancellationToken))
                                    {
                                        if (info.IsValid
                                            && info.ExceptionSymbol != exceptionSymbol)
                                        {
                                            var infos = new List<ThrowInfo>();
                                            infos.Add(new ThrowInfo(throwStatement, exceptionSymbol));
                                            infos.Add(info);

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
        }

        private static IParameterSymbol GetParameterSymbol(
            ThrowStatementSyntax throwStatement,
            ITypeSymbol exceptionSymbol,
            ISymbol declarationSymbol,
            SemanticModel semanticModel,
            CancellationToken cancellationToken)
        {
            INamedTypeSymbol argumentExceptionSymbol = semanticModel.Compilation.GetTypeByMetadataName(MetadataNames.System_ArgumentException);

            if (exceptionSymbol.EqualsOrDerivedFrom(argumentExceptionSymbol))
            {
                SyntaxNode parent = throwStatement.Parent;

                if (parent != null)
                {
                    if (parent.IsKind(SyntaxKind.Block))
                        parent = parent.Parent;

                    if (parent?.IsKind(SyntaxKind.IfStatement) == true)
                    {
                        var ifStatement = (IfStatementSyntax)parent;

                        ExpressionSyntax condition = ifStatement.Condition;

                        if (condition?.IsKind(SyntaxKind.EqualsExpression) == true)
                        {
                            var equalsExpression = (BinaryExpressionSyntax)condition;

                            ExpressionSyntax left = equalsExpression.Left;

                            if (left != null)
                            {
                                ISymbol leftSymbol = semanticModel.GetSymbol(left, cancellationToken);

                                if (leftSymbol?.IsParameter() == true
                                    && leftSymbol.ContainingSymbol?.Equals(declarationSymbol) == true)
                                {
                                    return (IParameterSymbol)leftSymbol;
                                }
                            }
                        }
                    }
                }
            }

            return null;
        }

        private static IEnumerable<ThrowInfo> GetOtherUndocumentedExceptions(
            ThrowStatementSyntax throwStatement,
            MemberDeclarationSyntax containingMember,
            SemanticModel semanticModel,
            CancellationToken cancellationToken)
        {
            foreach (SyntaxNode node in containingMember.DescendantNodes())
            {
                if (node.IsKind(SyntaxKind.ThrowStatement)
                    && node != throwStatement)
                {
                    ThrowInfo info = GetUndocumentedExceptionInfo((ThrowStatementSyntax)node, containingMember, semanticModel, cancellationToken);

                    if (info.IsValid)
                        yield return info;
                }
            }
        }

        private static ThrowInfo GetUndocumentedExceptionInfo(
            ThrowStatementSyntax throwStatement,
            MemberDeclarationSyntax containingMember,
            SemanticModel semanticModel,
            CancellationToken cancellationToken)
        {
            ExpressionSyntax expression = throwStatement.Expression;

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
                            return new ThrowInfo(throwStatement, typeSymbol);
                        }
                    }
                }
            }

            return default(ThrowInfo);
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
            StatementSyntax statement,
            SemanticModel semanticModel,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            ISymbol symbol = semanticModel.GetEnclosingSymbol(statement.SpanStart, cancellationToken);

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

            foreach (SyntaxNode node in containingMember.DescendantNodes())
            {
                if (node.IsKind(SyntaxKind.ThrowStatement)
                    && !infos.Any(f => f.ThrowStatement == node))
                {
                    ThrowInfo info = GetUndocumentedExceptionInfo((ThrowStatementSyntax)node, containingMember, semanticModel, cancellationToken);

                    if (info.IsValid
                        && !infos.Any(f => f.ExceptionSymbol == info.ExceptionSymbol))
                    {
                        infos.Add(info);
                    }
                }
            }

            string indent = GetIndent(trivia, sourceText, cancellationToken);

            var sb = new StringBuilder();

            foreach (ThrowInfo info in infos)
            {
                sb.Append(indent);

                IParameterSymbol parameterSymbol = GetParameterSymbol(info.ThrowStatement, info.ExceptionSymbol, declarationSymbol, semanticModel, cancellationToken);

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

                if (exceptionSymbol.Equals(semanticModel.Compilation.GetTypeByMetadataName(MetadataNames.System_ArgumentNullException)))
                    sb.Append("\"/> is <c>null</c>.");
            }

            sb.Append("</exception>");
            sb.AppendLine();
        }

        private struct ThrowInfo
        {
            public ThrowInfo(ThrowStatementSyntax throwStatement, ITypeSymbol exceptionSymbol)
            {
                ThrowStatement = throwStatement;
                ExceptionSymbol = exceptionSymbol;
            }

            public ThrowStatementSyntax ThrowStatement { get; }
            public ITypeSymbol ExceptionSymbol { get; }

            public bool IsValid
            {
                get
                {
                    return ThrowStatement != null
                        && ExceptionSymbol != null;
                }
            }
        }
    }
}