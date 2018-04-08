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
using Roslynator.CSharp.Analysis.AddExceptionToDocumentationComment;
using Roslynator.CSharp.Syntax;
using Roslynator.Text;

namespace Roslynator.CSharp.Refactorings.AddExceptionToDocumentationComment
{
    internal static class AddExceptionToDocumentationCommentRefactoring
    {
        private static IEnumerable<ThrowInfo> GetOtherUndocumentedExceptions(
            MemberDeclarationSyntax declaration,
            ISymbol declarationSymbol,
            Func<SyntaxNode, bool> predicate,
            INamedTypeSymbol exceptionSymbol,
            SemanticModel semanticModel,
            CancellationToken cancellationToken)
        {
            foreach (SyntaxNode node in declaration.DescendantNodes(f => !CSharpFacts.IsAnonymousFunctionExpression(f.Kind())))
            {
                switch (node.Kind())
                {
                    case SyntaxKind.ThrowStatement:
                        {
                            if (predicate(node))
                            {
                                var throwStatement = (ThrowStatementSyntax)node;

                                ThrowInfo info = GetUndocumentedExceptionInfo(node, throwStatement.Expression, declaration, declarationSymbol, exceptionSymbol, semanticModel, cancellationToken);

                                if (info != null)
                                    yield return info;
                            }

                            break;
                        }
                    case SyntaxKind.ThrowExpression:
                        {
                            if (predicate(node))
                            {
                                var throwExpression = (ThrowExpressionSyntax)node;
                                ThrowInfo info = GetUndocumentedExceptionInfo(node, throwExpression.Expression, declaration, declarationSymbol, exceptionSymbol, semanticModel, cancellationToken);

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
        MemberDeclarationSyntax declaration,
        ISymbol declarationSymbol,
        INamedTypeSymbol exceptionSymbol,
        SemanticModel semanticModel,
        CancellationToken cancellationToken)
        {
            if (expression == null)
                return null;

            if (!(semanticModel.GetTypeSymbol(expression, cancellationToken) is INamedTypeSymbol typeSymbol))
                return null;

            if (!InheritsFromException(typeSymbol, exceptionSymbol))
                return null;

            DocumentationCommentTriviaSyntax comment = declaration.GetSingleLineDocumentationComment();

            if (comment == null)
                return null;

            if (!CanAddExceptionToComment(comment, typeSymbol, semanticModel, cancellationToken))
                return null;

            return ThrowInfo.Create(node, typeSymbol, declarationSymbol);
        }

        private static bool CanAddExceptionToComment(
            DocumentationCommentTriviaSyntax comment,
            INamedTypeSymbol exceptionSymbol,
            SemanticModel semanticModel,
            CancellationToken cancellationToken)
        {
            bool containsException = false;
            bool containsIncludeOrExclude = false;
            bool isFirst = true;

            foreach (XmlNodeSyntax node in comment.Content)
            {
                XmlElementInfo info = SyntaxInfo.XmlElementInfo(node);
                if (info.Success)
                {
                    switch (info.ElementKind)
                    {
                        case XmlElementKind.Include:
                        case XmlElementKind.Exclude:
                            {
                                if (isFirst)
                                    containsIncludeOrExclude = true;

                                break;
                            }
                        case XmlElementKind.InheritDoc:
                            {
                                return false;
                            }
                        case XmlElementKind.Exception:
                            {
                                if (!containsException)
                                {
                                    if (info.IsEmptyElement)
                                    {
                                        containsException = ContainsException((XmlEmptyElementSyntax)info.Element, exceptionSymbol, semanticModel, cancellationToken);
                                    }
                                    else
                                    {
                                        containsException = ContainsException((XmlElementSyntax)info.Element, exceptionSymbol, semanticModel, cancellationToken);
                                    }
                                }

                                break;
                            }
                    }

                    if (isFirst)
                    {
                        isFirst = false;
                    }
                    else
                    {
                        containsIncludeOrExclude = false;
                    }
                }
            }

            return !containsIncludeOrExclude
                && !containsException;
        }

        private static bool ContainsException(XmlElementSyntax xmlElement, INamedTypeSymbol exceptionSymbol, SemanticModel semanticModel, CancellationToken cancellationToken)
        {
            XmlElementStartTagSyntax startTag = xmlElement.StartTag;

            return startTag != null
                && ContainsException(startTag.Attributes, exceptionSymbol, semanticModel, cancellationToken);
        }

        private static bool ContainsException(XmlEmptyElementSyntax xmlEmptyElement, INamedTypeSymbol exceptionSymbol, SemanticModel semanticModel, CancellationToken cancellationToken)
        {
            return ContainsException(xmlEmptyElement.Attributes, exceptionSymbol, semanticModel, cancellationToken);
        }

        private static bool ContainsException(SyntaxList<XmlAttributeSyntax> attributes, INamedTypeSymbol exceptionSymbol, SemanticModel semanticModel, CancellationToken cancellationToken)
        {
            foreach (XmlAttributeSyntax xmlAttribute in attributes)
            {
                if (xmlAttribute.Kind() == SyntaxKind.XmlCrefAttribute)
                {
                    var xmlCrefAttribute = (XmlCrefAttributeSyntax)xmlAttribute;

                    CrefSyntax cref = xmlCrefAttribute.Cref;

                    if (cref != null
                        && (semanticModel.GetSymbol(cref, cancellationToken) is INamedTypeSymbol symbol))
                    {
                        if (exceptionSymbol.Equals(symbol))
                            return true;

                        // http://github.com/dotnet/roslyn/issues/22923
                        if (exceptionSymbol.IsGenericType
                            && symbol.IsGenericType
                            && exceptionSymbol.ConstructedFrom.Equals(symbol.ConstructedFrom))
                        {
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        public static Task<Document> RefactorAsync(
            Document document,
            ThrowExpressionSyntax throwExpression,
            CancellationToken cancellationToken)
        {
            return RefactorAsync(document, throwExpression, throwExpression.Expression, cancellationToken);
        }

        public static Task<Document> RefactorAsync(
            Document document,
            ThrowStatementSyntax throwStatement,
            CancellationToken cancellationToken)
        {
            return RefactorAsync(document, throwStatement, throwStatement.Expression, cancellationToken);
        }

        private static bool InheritsFromException(ITypeSymbol typeSymbol, INamedTypeSymbol exceptionSymbol)
        {
            return typeSymbol?.TypeKind == TypeKind.Class
                && typeSymbol.BaseType?.IsObject() == false
                && typeSymbol.InheritsFrom(exceptionSymbol);
        }

        private static async Task<Document> RefactorAsync(
            Document document,
            SyntaxNode node,
            ExpressionSyntax expression,
            CancellationToken cancellationToken)
        {
            SemanticModel semanticModel = await document.GetSemanticModelAsync(cancellationToken).ConfigureAwait(false);

            ITypeSymbol exceptionSymbol = semanticModel.GetTypeSymbol(expression, cancellationToken);

            ISymbol declarationSymbol = AddExceptionToDocumentationCommentAnalysis.GetDeclarationSymbol(node.SpanStart, semanticModel, cancellationToken);

            var memberDeclaration = (MemberDeclarationSyntax)await declarationSymbol
                .GetSyntaxAsync(cancellationToken)
                .ConfigureAwait(false);

            SyntaxTrivia trivia = memberDeclaration.GetSingleLineDocumentationCommentTrivia();

            ThrowInfo throwInfo = ThrowInfo.Create(node, exceptionSymbol, declarationSymbol);

            return await RefactorAsync(
                document,
                trivia,
                throwInfo,
                memberDeclaration,
                declarationSymbol,
                semanticModel,
                cancellationToken).ConfigureAwait(false);
        }

        public static async Task<Document> RefactorAsync(
            Document document,
            AddExceptionToDocumentationCommentAnalysisResult analysis,
            CancellationToken cancellationToken)
        {
            SemanticModel semanticModel = await document.GetSemanticModelAsync().ConfigureAwait(false);

            var memberDeclaration = (MemberDeclarationSyntax)await analysis.DeclarationSymbol
                .GetSyntaxAsync(cancellationToken)
                .ConfigureAwait(false);

            return await RefactorAsync(
                document,
                analysis.DocumentationComment,
                analysis.ThrowInfo,
                memberDeclaration,
                analysis.DeclarationSymbol,
                semanticModel,
                cancellationToken).ConfigureAwait(false);
        }

        private static async Task<Document> RefactorAsync(
            Document document,
            SyntaxTrivia trivia,
            ThrowInfo throwInfo,
            MemberDeclarationSyntax memberDeclaration,
            ISymbol declarationSymbol,
            SemanticModel semanticModel,
            CancellationToken cancellationToken)
        {
            SourceText sourceText = await document.GetTextAsync(cancellationToken).ConfigureAwait(false);

            var throwInfos = new List<ThrowInfo>() { throwInfo };

            INamedTypeSymbol exceptionSymbol = semanticModel.GetTypeByMetadataName(MetadataNames.System_Exception);

            foreach (ThrowInfo info in GetOtherUndocumentedExceptions(memberDeclaration, declarationSymbol, node => node != throwInfo.Node, exceptionSymbol, semanticModel, cancellationToken))
            {
                if (!throwInfos.Any(f => f.ExceptionSymbol == info.ExceptionSymbol))
                    throwInfos.Add(info);
            }

            string indent = GetIndent(memberDeclaration.GetLeadingTrivia());

            StringBuilder sb = StringBuilderCache.GetInstance();

            foreach (ThrowInfo info in throwInfos)
            {
                sb.Append(indent);

                IParameterSymbol parameterSymbol = info.GetParameterSymbol(semanticModel, cancellationToken);

                AppendExceptionDocumentation(trivia, info.ExceptionSymbol, parameterSymbol, semanticModel, ref sb);
            }

            string newText = StringBuilderCache.GetStringAndFree(sb);

            var textChange = new TextChange(new TextSpan(trivia.FullSpan.End, 0), newText);

            SourceText newSourceText = sourceText.WithChanges(textChange);

            return document.WithText(newSourceText);
        }

        private static string GetIndent(SyntaxTriviaList leadingTrivia)
        {
            if (leadingTrivia.Any())
            {
                int index = leadingTrivia.Count;

                while (index >= 1
                    && leadingTrivia[index - 1].IsWhitespaceTrivia())
                {
                    index--;
                }

                return string.Concat(leadingTrivia.Skip(index));
            }

            return "";
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
                    sb.Append(" is <c>null</c>.");
            }

            sb.Append("</exception>");
            sb.AppendLine();
        }
    }
}
