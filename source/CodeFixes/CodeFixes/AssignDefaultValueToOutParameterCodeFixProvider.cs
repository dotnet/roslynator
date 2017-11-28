// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.CSharp.Refactorings;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static Roslynator.CSharp.CSharpFactory;

namespace Roslynator.CSharp.CodeFixes
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(AssignDefaultValueToOutParameterCodeFixProvider))]
    [Shared]
    public class AssignDefaultValueToOutParameterCodeFixProvider : BaseCodeFixProvider
    {
        public sealed override ImmutableArray<string> FixableDiagnosticIds
        {
            get { return ImmutableArray.Create(CompilerDiagnosticIdentifiers.OutParameterMustBeAssignedToBeforeControlLeavesCurrentMethod); }
        }

        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            if (!Settings.IsCodeFixEnabled(CodeFixIdentifiers.AssignDefaultValueToOutParameter))
                return;

            SyntaxNode root = await context.GetSyntaxRootAsync().ConfigureAwait(false);

            if (!TryFindFirstAncestorOrSelf(root, context.Span, out SyntaxNode node, predicate: f => f.IsKind(SyntaxKind.MethodDeclaration, SyntaxKind.LocalFunctionStatement) || f is StatementSyntax))
                return;

            foreach (Diagnostic diagnostic in context.Diagnostics)
            {
                switch (diagnostic.Id)
                {
                    case CompilerDiagnosticIdentifiers.OutParameterMustBeAssignedToBeforeControlLeavesCurrentMethod:
                        {
                            SyntaxNode bodyOrExpressionBody = null;

                            StatementSyntax statement = null;

                            if (!node.IsKind(SyntaxKind.LocalFunctionStatement))
                                statement = node as StatementSyntax;

                            if (statement != null)
                                node = node.FirstAncestor(f => f.IsKind(SyntaxKind.MethodDeclaration, SyntaxKind.LocalFunctionStatement));

                            if (ContainsYield(node))
                                break;

                            bodyOrExpressionBody = GetBodyOrExpressionBody(node);

                            SemanticModel semanticModel = await context.Document.GetSemanticModelAsync().ConfigureAwait(false);

                            var methodSymbol = (IMethodSymbol)semanticModel.GetDeclaredSymbol(node);

                            DataFlowAnalysis dataFlowAnalysis = AnalyzeDataFlow(bodyOrExpressionBody, semanticModel);

                            // Flow analysis APIs do not work with local functions: https://github.com/dotnet/roslyn/issues/14214
                            if (!dataFlowAnalysis.Succeeded)
                                break;

                            ImmutableArray<ISymbol> alwaysAssigned = dataFlowAnalysis.AlwaysAssigned;

                            foreach (IParameterSymbol parameter in methodSymbol.Parameters)
                            {
                                if (parameter.RefKind == RefKind.Out
                                    && !alwaysAssigned.Contains(parameter))
                                {
                                    CodeAction codeAction = CodeAction.Create(
                                        $"Assign default value to parameter '{parameter.Name}'",
                                        cancellationToken => RefactorAsync(context.Document, node, statement, bodyOrExpressionBody, parameter, semanticModel, cancellationToken),
                                        GetEquivalenceKey(diagnostic));

                                    context.RegisterCodeFix(codeAction, diagnostic);
                                }
                            }

                            break;
                        }
                }
            }
        }

        private static DataFlowAnalysis AnalyzeDataFlow(
            SyntaxNode bodyOrExpressionBody,
            SemanticModel semanticModel)
        {
            if (bodyOrExpressionBody.IsKind(SyntaxKind.Block))
            {
                var body = (BlockSyntax)bodyOrExpressionBody;

                return semanticModel.AnalyzeDataFlow(body);
            }
            else
            {
                var arrowExpressionClause = (ArrowExpressionClauseSyntax)bodyOrExpressionBody;

                return semanticModel.AnalyzeDataFlow(arrowExpressionClause.Expression);
            }
        }

        private static Task<Document> RefactorAsync(
            Document document,
            SyntaxNode node,
            StatementSyntax statement,
            SyntaxNode bodyOrExpressionBody,
            IParameterSymbol parameter,
            SemanticModel semanticModel,
            CancellationToken cancellationToken)
        {
            ExpressionStatementSyntax expressionStatement = SimpleAssignmentStatement(
                IdentifierName(parameter.Name),
                parameter.Type.ToDefaultValueSyntax(semanticModel, bodyOrExpressionBody.Span.End));

            expressionStatement = expressionStatement.WithFormatterAnnotation();

            SyntaxNode newNode = null;

            if (bodyOrExpressionBody.IsKind(SyntaxKind.ArrowExpressionClause))
            {
                newNode = ExpandExpressionBodyRefactoring.Refactor((ArrowExpressionClauseSyntax)bodyOrExpressionBody, semanticModel, cancellationToken);

                newNode = InsertStatement(newNode, expressionStatement);
            }
            else if (statement != null)
            {
                if (statement.IsEmbedded())
                {
                    newNode = node.ReplaceNode(statement, Block(expressionStatement, statement));
                }
                else
                {
                    newNode = node.InsertNodesBefore(statement, new StatementSyntax[] { expressionStatement });
                }
            }
            else
            {
                newNode = InsertStatement(node, expressionStatement);
            }

            return document.ReplaceNodeAsync(node, newNode, cancellationToken);
        }

        private static SyntaxNode InsertStatement(
            SyntaxNode node,
            StatementSyntax statement)
        {
            var body = (BlockSyntax)GetBodyOrExpressionBody(node);

            SyntaxList<StatementSyntax> statements = body.Statements;

            StatementSyntax lastStatement = statements.LastOrDefault(f => !f.IsKind(SyntaxKind.LocalFunctionStatement, SyntaxKind.ReturnStatement));

            int index = (lastStatement != null)
                ? statements.IndexOf(lastStatement) + 1
                : 0;

            BlockSyntax newBody = body.WithStatements(statements.Insert(index, statement));

            if (node.IsKind(SyntaxKind.MethodDeclaration))
            {
                return ((MethodDeclarationSyntax)node).WithBody(newBody);
            }
            else
            {
                return ((LocalFunctionStatementSyntax)node).WithBody(newBody);
            }
        }

        private static SyntaxNode GetBodyOrExpressionBody(SyntaxNode node)
        {
            if (node.IsKind(SyntaxKind.MethodDeclaration))
            {
                return ((MethodDeclarationSyntax)node).BodyOrExpressionBody();
            }
            else
            {
                return ((LocalFunctionStatementSyntax)node).BodyOrExpressionBody();
            }
        }

        private static bool ContainsYield(SyntaxNode node)
        {
            if (node.IsKind(SyntaxKind.MethodDeclaration))
            {
                return ((MethodDeclarationSyntax)node).ContainsYield();
            }
            else
            {
                return ((LocalFunctionStatementSyntax)node).ContainsYield();
            }
        }
    }
}
