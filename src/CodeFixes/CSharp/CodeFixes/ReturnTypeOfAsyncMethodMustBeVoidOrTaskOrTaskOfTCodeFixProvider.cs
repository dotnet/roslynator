// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Immutable;
using System.Composition;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.CodeFixes;

namespace Roslynator.CSharp.CodeFixes
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(CodeFixProvider))]
    [Shared]
    public sealed class ReturnTypeOfAsyncMethodMustBeVoidOrTaskOrTaskOfTCodeFixProvider : CompilerDiagnosticCodeFixProvider
    {
        public override ImmutableArray<string> FixableDiagnosticIds
        {
            get { return ImmutableArray.Create(CompilerDiagnosticIdentifiers.CS1983_ReturnTypeOfAsyncMethodMustBeVoidOrTaskOrTaskOfT); }
        }

        public override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            Diagnostic diagnostic = context.Diagnostics[0];

            SyntaxNode root = await context.GetSyntaxRootAsync().ConfigureAwait(false);

            if (!IsEnabled(diagnostic.Id, CodeFixIdentifiers.ChangeMethodReturnType, context.Document, root.SyntaxTree))
                return;

            if (!TryFindFirstAncestorOrSelf(root, context.Span, out SyntaxNode node, predicate: f => f.IsKind(SyntaxKind.MethodDeclaration, SyntaxKind.LocalFunctionStatement)))
                return;

            SemanticModel semanticModel = await context.GetSemanticModelAsync().ConfigureAwait(false);

            var methodSymbol = (IMethodSymbol)semanticModel.GetDeclaredSymbol(node, context.CancellationToken);

            SyntaxDebug.Assert(methodSymbol != null, node);

            ITypeSymbol typeSymbol = methodSymbol.ReturnType;

            if (typeSymbol.IsErrorType())
                return;

            (bool containsReturnAwait, bool containsAwaitStatement) = AnalyzeAwaitExpressions(node);

            SyntaxDebug.Assert(containsAwaitStatement || containsReturnAwait, node);

            if (containsAwaitStatement)
            {
                INamedTypeSymbol taskSymbol = semanticModel.GetTypeByMetadataName("System.Threading.Tasks.Task");

                CodeFixRegistrator.ChangeTypeOrReturnType(context, diagnostic, node, taskSymbol, semanticModel, "Task");
            }

            if (containsReturnAwait)
            {
                typeSymbol = semanticModel.GetTypeByMetadataName("System.Threading.Tasks.Task`1").Construct(typeSymbol);

                CodeFixRegistrator.ChangeTypeOrReturnType(context, diagnostic, node, typeSymbol, semanticModel, "TaskOfT");
            }
        }

        private static (bool containsReturnAwait, bool containsAwaitStatement) AnalyzeAwaitExpressions(SyntaxNode node)
        {
            if (node is MethodDeclarationSyntax methodDeclaration)
            {
                ArrowExpressionClauseSyntax expressionBody = methodDeclaration.ExpressionBody;

                if (expressionBody != null)
                    return (expressionBody.Expression?.Kind() == SyntaxKind.AwaitExpression, false);

                node = methodDeclaration.Body;
            }
            else
            {
                var localFunction = (LocalFunctionStatementSyntax)node;

                ArrowExpressionClauseSyntax expressionBody = localFunction.ExpressionBody;

                if (expressionBody != null)
                    return (expressionBody.Expression?.Kind() == SyntaxKind.AwaitExpression, false);

                node = localFunction.Body;
            }

            if (node == null)
                return (false, false);

            var containsReturnAwait = false;
            var containsAwaitStatement = false;

            foreach (SyntaxNode descendant in node.DescendantNodes(node.Span, f => !CSharpFacts.IsFunction(f.Kind())))
            {
                switch (descendant)
                {
                    case ReturnStatementSyntax returnStatement:
                        {
                            if (returnStatement.Expression?.WalkDownParentheses().Kind() == SyntaxKind.AwaitExpression)
                                containsReturnAwait = true;

                            break;
                        }
                    case ExpressionStatementSyntax expressionStatement:
                        {
                            if (expressionStatement.Expression?.Kind() == SyntaxKind.AwaitExpression)
                                containsAwaitStatement = true;

                            break;
                        }
                }
            }

            return (containsReturnAwait, containsAwaitStatement);
        }
    }
}
