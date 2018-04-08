// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Collections.Immutable;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static Roslynator.CSharp.CSharpFactory;

namespace Roslynator.CSharp.Refactorings
{
    internal static class UseStringBuilderInsteadOfConcatenationRefactoring
    {
        public static void ComputeRefactoring(RefactoringContext context, StringConcatenationExpressionInfo concatenationInfo)
        {
            BinaryExpressionSyntax binaryExpression = concatenationInfo.BinaryExpression;

            if (binaryExpression.IsParentKind(SyntaxKind.SimpleAssignmentExpression, SyntaxKind.AddAssignmentExpression))
            {
                var assignment = (AssignmentExpressionSyntax)binaryExpression.Parent;

                if (assignment.IsParentKind(SyntaxKind.ExpressionStatement)
                    && assignment.Right == binaryExpression)
                {
                    RegisterRefactoring(context, concatenationInfo, (StatementSyntax)assignment.Parent);
                }
            }
            else
            {
                SingleLocalDeclarationStatementInfo info = SyntaxInfo.SingleLocalDeclarationStatementInfo(binaryExpression);

                if (info.Success)
                    RegisterRefactoring(context, concatenationInfo, info.Statement);
            }
        }

        private static void RegisterRefactoring(RefactoringContext context, StringConcatenationExpressionInfo concatenationInfo, StatementSyntax statement)
        {
            context.RegisterRefactoring(
                "Use StringBuilder instead of concatenation",
                cancellationToken => RefactorAsync(context.Document, concatenationInfo, statement, cancellationToken));
        }

        private static async Task<Document> RefactorAsync(
            Document document,
            StringConcatenationExpressionInfo concatenationInfo,
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

            ExpressionSyntax newInvocation = null;
            foreach (ExpressionSyntax expression in concatenationInfo.Expressions(leftToRight: true))
            {
                if (expression.IsKind(SyntaxKind.InterpolatedStringExpression))
                {
                    var interpolatedString = (InterpolatedStringExpressionSyntax)expression;

                    bool isVerbatim = interpolatedString.IsVerbatim();

                    SyntaxList<InterpolatedStringContentSyntax> contents = interpolatedString.Contents;

                    for (int j = 0; j < contents.Count; j++)
                    {
                        (SyntaxKind contentKind, string methodName, ImmutableArray<ArgumentSyntax> arguments) = RefactoringUtility.ConvertInterpolatedStringToStringBuilderMethod(contents[j], isVerbatim);

                        newInvocation = SimpleMemberInvocationExpression(
                            newInvocation ?? stringBuilderName,
                            IdentifierName(methodName),
                            ArgumentList(arguments.ToSeparatedSyntaxList()));
                    }
                }
                else
                {
                    newInvocation = SimpleMemberInvocationExpression(
                        newInvocation ?? stringBuilderName,
                        IdentifierName("Append"),
                        Argument(expression.WithoutTrivia()));
                }
            }

            statements.Add(ExpressionStatement(newInvocation));

            statements.Add(statement
                .ReplaceNode(concatenationInfo.BinaryExpression, SimpleMemberInvocationExpression(stringBuilderName, IdentifierName("ToString")))
                .WithTrailingTrivia(statement.GetTrailingTrivia())
                .WithoutLeadingTrivia());

            if (statement.IsEmbedded())
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
