// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static Roslynator.CSharp.CSharpFactory;

namespace Roslynator.CSharp.Refactorings
{
    internal static class UseStringBuilderInsteadOfConcatenationRefactoring
    {
        public static void ComputeRefactoring(RefactoringContext context, StringConcatenationExpression concatenation)
        {
            BinaryExpressionSyntax expression = concatenation.OriginalExpression;

            switch (expression.Parent.Kind())
            {
                case SyntaxKind.SimpleAssignmentExpression:
                case SyntaxKind.AddAssignmentExpression:
                    {
                        var assignment = (AssignmentExpressionSyntax)expression.Parent;

                        if (assignment.Parent.IsKind(SyntaxKind.ExpressionStatement)
                            && assignment.Right == expression)
                        {
                            RegisterRefactoring(context, concatenation, (StatementSyntax)assignment.Parent);
                        }

                        break;
                    }
                case SyntaxKind.EqualsValueClause:
                    {
                        var equalsValue = (EqualsValueClauseSyntax)expression.Parent;

                        if (equalsValue.Parent.IsKind(SyntaxKind.VariableDeclarator))
                        {
                            var variableDeclarator = (VariableDeclaratorSyntax)equalsValue.Parent;

                            if (variableDeclarator.Parent.IsKind(SyntaxKind.VariableDeclaration))
                            {
                                var variableDeclaration = (VariableDeclarationSyntax)variableDeclarator.Parent;

                                if (variableDeclaration.Parent.IsKind(SyntaxKind.LocalDeclarationStatement)
                                    && variableDeclaration.Variables.Count == 1)
                                {
                                    RegisterRefactoring(context, concatenation, (StatementSyntax)variableDeclaration.Parent);
                                }
                            }
                        }

                        break;
                    }
            }
        }

        private static void RegisterRefactoring(RefactoringContext context, StringConcatenationExpression concatenation, StatementSyntax statement)
        {
            context.RegisterRefactoring(
                "Use StringBuilder instead of concatenation",
                cancellationToken => RefactorAsync(context.Document, concatenation, statement, cancellationToken));
        }

        private static async Task<Document> RefactorAsync(
            Document document,
            StringConcatenationExpression concatenation,
            StatementSyntax statement,
            CancellationToken cancellationToken)
        {
            SemanticModel semanticModel = await document.GetSemanticModelAsync(cancellationToken).ConfigureAwait(false);

            string name = NameGenerator.Default.EnsureUniqueLocalName(DefaultNames.StringBuilderVariable, semanticModel, statement.SpanStart, cancellationToken: cancellationToken);

            IdentifierNameSyntax identifierName = IdentifierName(name);

            var statements = new List<StatementSyntax>();

            TypeSyntax type = semanticModel.GetTypeByMetadataName(MetadataNames.System_Text_StringBuilder).ToMinimalTypeSyntax(semanticModel, statement.SpanStart);

            statements.Add(LocalDeclarationStatement(VarType(), name, ObjectCreationExpression(type, ArgumentList())).WithLeadingTrivia(statement.GetLeadingTrivia()));

            foreach (ExpressionSyntax expression in concatenation.Expressions)
            {
                switch (expression.Kind())
                {
                    case SyntaxKind.InterpolatedStringExpression:
                        {
                            var interpolatedString = (InterpolatedStringExpressionSyntax)expression;

                            bool isVerbatim = interpolatedString.IsVerbatim();

                            foreach (InterpolatedStringContentSyntax content in interpolatedString.Contents)
                            {
                                switch (content.Kind())
                                {
                                    case SyntaxKind.Interpolation:
                                        {
                                            var interpolation = (InterpolationSyntax)content;

                                            InterpolationAlignmentClauseSyntax alignmentClause = interpolation.AlignmentClause;
                                            InterpolationFormatClauseSyntax formatClause = interpolation.FormatClause;

                                            if (alignmentClause != null
                                                || formatClause != null)
                                            {
                                                var sb = new StringBuilder();
                                                sb.Append("\"{0");

                                                if (alignmentClause != null)
                                                {
                                                    sb.Append(',');
                                                    sb.Append(alignmentClause.Value.ToString());
                                                }

                                                if (formatClause != null)
                                                {
                                                    sb.Append(':');
                                                    sb.Append(formatClause.FormatStringToken.Text);
                                                }

                                                sb.Append("}\"");

                                                ExpressionStatementSyntax appendFormatStatement = ExpressionStatement(
                                                    SimpleMemberInvocationExpression(
                                                        identifierName,
                                                        IdentifierName("AppendFormat"),
                                                        ArgumentList(Argument(ParseExpression(sb.ToString())), Argument(interpolation.Expression))));

                                                statements.Add(appendFormatStatement);
                                            }
                                            else
                                            {
                                                statements.Add(CreateStatement(interpolation.Expression, identifierName));
                                            }

                                            break;
                                        }
                                    case SyntaxKind.InterpolatedStringText:
                                        {
                                            var interpolatedStringText = (InterpolatedStringTextSyntax)content;

                                            string text = interpolatedStringText.TextToken.Text;

                                            text = (isVerbatim)
                                                ? "@\"" + text + "\""
                                                : "\"" + text + "\"";

                                            ExpressionSyntax stringLiteral = ParseExpression(text);

                                            statements.Add(CreateStatement(stringLiteral, identifierName));
                                            break;
                                        }
                                }
                            }

                            break;
                        }
                    default:
                        {
                            statements.Add(CreateStatement(expression, identifierName));
                            break;
                        }
                }
            }

            statements.Add(statement.ReplaceNode(concatenation.OriginalExpression, SimpleMemberInvocationExpression(identifierName, IdentifierName("ToString"))).WithTrailingTrivia(statement.GetTrailingTrivia()));

            for (int i = 0; i < statements.Count; i++)
            {
                SyntaxTriviaList trailingTrivia = statements[i].GetTrailingTrivia();

                if (!trailingTrivia.Any(f => f.IsEndOfLineTrivia()))
                    statements[i] = statements[i].WithTrailingTrivia(trailingTrivia.Add(NewLine()));
            }

            if (EmbeddedStatementHelper.IsEmbeddedStatement(statement))
            {
                BlockSyntax block = Block(statements).WithFormatterAnnotation();

                return await document.ReplaceNodeAsync(statement, block, cancellationToken).ConfigureAwait(false);
            }
            else
            {
                for (int i = 0; i < statements.Count; i++)
                    statements[i] = statements[i].WithFormatterAnnotation();

                return await document.ReplaceNodeAsync(statement, statements, cancellationToken).ConfigureAwait(false);
            }
        }

        private static ExpressionStatementSyntax CreateStatement(ExpressionSyntax expression, IdentifierNameSyntax identifierName)
        {
            ExpressionStatementSyntax statement = ExpressionStatement(
                SimpleMemberInvocationExpression(
                    identifierName,
                    IdentifierName("Append"),
                    Argument(expression.WithoutTrivia())));

            return statement.WithTriviaFrom(expression);
        }
    }
}
