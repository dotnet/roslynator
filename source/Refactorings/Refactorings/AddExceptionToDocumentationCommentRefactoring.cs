// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
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

                ITypeSymbol typeSymbol = semanticModel.GetTypeSymbol(expression, context.CancellationToken);

                if (typeSymbol?.IsErrorType() == false
                    && SymbolAnalyzer.IsException(typeSymbol, semanticModel))
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
                                    && !ContainsException(comment, typeSymbol, semanticModel, context.CancellationToken))
                                {
                                    IParameterSymbol parameterSymbol = GetParameterSymbol(throwStatement, typeSymbol, declarationSymbol, semanticModel, context.CancellationToken);

                                    context.RegisterRefactoring(
                                        "Add exception to documentation comment",
                                        cancellationToken => RefactorAsync(context.Document, trivia, typeSymbol, parameterSymbol, cancellationToken));
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
            INamedTypeSymbol argumentNullExceptionSymbol = semanticModel.Compilation.GetTypeByMetadataName(MetadataNames.System_ArgumentNullException);

            if (exceptionSymbol.EqualsOrDerivedFrom(argumentNullExceptionSymbol))
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
                                ISymbol symbol = semanticModel.GetSymbolInfo(cref, cancellationToken).Symbol;

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

            int lineIndex = trivia.GetSpanStartLine(cancellationToken);

            string exceptionName = exceptionSymbol
                .ToMinimalDisplayString(semanticModel, trivia.FullSpan.End)
                .Replace('<', '{')
                .Replace('>', '}');

            string content = (parameterSymbol != null)
                ? $"<paramref name=\"{parameterSymbol.Name}\"/> is <c>null</c>."
                : null;

            string newText = new string(' ', trivia.FullSpan.Start - sourceText.Lines[lineIndex].Start)
                + $"/// <exception cref=\"{exceptionName}\">{content}</exception>"
                + Environment.NewLine;

            SourceText newSourceText = sourceText.WithChanges(
                new TextChange(new TextSpan(trivia.FullSpan.End, 0), newText));

            return document.WithText(newSourceText);
        }
    }
}