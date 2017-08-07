// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Collections.Immutable;
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

                        if (assignment.IsParentKind(SyntaxKind.ExpressionStatement)
                            && assignment.Right == expression)
                        {
                            RegisterRefactoring(context, concatenation, (StatementSyntax)assignment.Parent);
                        }

                        break;
                    }
                case SyntaxKind.EqualsValueClause:
                    {
                        var equalsValue = (EqualsValueClauseSyntax)expression.Parent;

                        if (equalsValue.IsParentKind(SyntaxKind.VariableDeclarator))
                        {
                            var variableDeclarator = (VariableDeclaratorSyntax)equalsValue.Parent;

                            if (variableDeclarator.IsParentKind(SyntaxKind.VariableDeclaration))
                            {
                                var variableDeclaration = (VariableDeclarationSyntax)variableDeclarator.Parent;

                                if (variableDeclaration.IsParentKind(SyntaxKind.LocalDeclarationStatement)
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

            IdentifierNameSyntax stringBuilderName = IdentifierName(name);

            TypeSyntax type = semanticModel.GetTypeByMetadataName(MetadataNames.System_Text_StringBuilder).ToMinimalTypeSyntax(semanticModel, statement.SpanStart);

            var statements = new List<StatementSyntax>()
            {
                LocalDeclarationStatement(VarType(), Identifier(name).WithRenameAnnotation(), ObjectCreationExpression(type, ArgumentList())).WithLeadingTrivia(statement.GetLeadingTrivia())
            };

            ImmutableArray<ExpressionSyntax> expressions = concatenation.Expressions;

            ExpressionSyntax newInvocation = null;
            for (int i = 0; i < expressions.Length; i++)
            {
                if (expressions[i].IsKind(SyntaxKind.InterpolatedStringExpression))
                {
                    var interpolatedString = (InterpolatedStringExpressionSyntax)expressions[i];

                    bool isVerbatim = interpolatedString.IsVerbatim();

                    SyntaxList<InterpolatedStringContentSyntax> contents = interpolatedString.Contents;

                    for (int j = 0; j < contents.Count; j++)
                    {
                        InterpolatedStringContentConversion conversion = InterpolatedStringContentConversion.Create(contents[j], isVerbatim);

                        newInvocation = SimpleMemberInvocationExpression(
                            newInvocation ?? stringBuilderName,
                            IdentifierName(conversion.Name),
                            ArgumentList(conversion.Arguments));
                    }
                }
                else
                {
                    newInvocation = SimpleMemberInvocationExpression(
                        newInvocation ?? stringBuilderName,
                        IdentifierName("Append"),
                        Argument(expressions[i].WithoutTrivia()));
                }
            }

            statements.Add(ExpressionStatement(newInvocation));

            statements.Add(statement
                .ReplaceNode(concatenation.OriginalExpression, SimpleMemberInvocationExpression(stringBuilderName, IdentifierName("ToString")))
                .WithTrailingTrivia(statement.GetTrailingTrivia())
                .WithoutLeadingTrivia());

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
    }
}
