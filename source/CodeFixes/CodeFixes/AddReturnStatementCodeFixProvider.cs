// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Immutable;
using System.Composition;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Roslynator.CSharp.CodeFixes
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(AddReturnStatementCodeFixProvider))]
    [Shared]
    public class AddReturnStatementCodeFixProvider : BaseCodeFixProvider
    {
        public sealed override ImmutableArray<string> FixableDiagnosticIds
        {
            get
            {
                return ImmutableArray.Create(
                    CompilerDiagnosticIdentifiers.NotAllCodePathsReturnValue,
                    CompilerDiagnosticIdentifiers.NotAllCodePathsReturnValueInAnonymousFunction);
            }
        }

        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            if (!Settings.IsCodeFixEnabled(CodeFixIdentifiers.AddReturnStatementThatReturnsDefaultValue))
                return;

            SyntaxNode root = await context.GetSyntaxRootAsync().ConfigureAwait(false);

            SyntaxNode node = root.FindNode(context.Span, getInnermostNodeForTie: true);

            SemanticModel semanticModel = await context.GetSemanticModelAsync().ConfigureAwait(false);

            foreach (SyntaxNode ancestor in node.AncestorsAndSelf())
            {
                switch (ancestor.Kind())
                {
                    case SyntaxKind.MethodDeclaration:
                        {
                            var methodDeclaration = (MethodDeclarationSyntax)ancestor;

                            ComputeCodeFix(context, context.Diagnostics[0], methodDeclaration.Body, methodDeclaration.ReturnType, semanticModel);
                            return;
                        }
                    case SyntaxKind.OperatorDeclaration:
                        {
                            var operatorDeclaration = (OperatorDeclarationSyntax)ancestor;

                            ComputeCodeFix(context, context.Diagnostics[0], operatorDeclaration.Body, operatorDeclaration.ReturnType, semanticModel);
                            return;
                        }
                    case SyntaxKind.ConversionOperatorDeclaration:
                        {
                            var conversionOperatorDeclaration = (ConversionOperatorDeclarationSyntax)ancestor;

                            ComputeCodeFix(context, context.Diagnostics[0], conversionOperatorDeclaration.Body, conversionOperatorDeclaration.Type, semanticModel);
                            return;
                        }
                    case SyntaxKind.LocalFunctionStatement:
                        {
                            var localFunction = (LocalFunctionStatementSyntax)ancestor;

                            ComputeCodeFix(context, context.Diagnostics[0], localFunction.Body, localFunction.ReturnType, semanticModel);
                            return;
                        }
                    case SyntaxKind.GetAccessorDeclaration:
                        {
                            var accessor = (AccessorDeclarationSyntax)ancestor;

                            switch (accessor.Parent.Parent.Kind())
                            {
                                case SyntaxKind.PropertyDeclaration:
                                    {
                                        var propertyDeclaration = (PropertyDeclarationSyntax)accessor.Parent.Parent;

                                        ComputeCodeFix(context, context.Diagnostics[0], accessor.Body, propertyDeclaration.Type, semanticModel);
                                        break;
                                    }
                                case SyntaxKind.IndexerDeclaration:
                                    {
                                        var indexerDeclaration = (IndexerDeclarationSyntax)accessor.Parent.Parent;

                                        ComputeCodeFix(context, context.Diagnostics[0], accessor.Body, indexerDeclaration.Type, semanticModel);
                                        break;
                                    }
                            }

                            return;
                        }
                    case SyntaxKind.AnonymousMethodExpression:
                    case SyntaxKind.SimpleLambdaExpression:
                    case SyntaxKind.ParenthesizedLambdaExpression:
                        {
                            var anonymousFunction = (AnonymousFunctionExpressionSyntax)ancestor;

                            var methodSymbol = semanticModel.GetSymbol(anonymousFunction, context.CancellationToken) as IMethodSymbol;

                            if (methodSymbol?.IsErrorType() == false)
                                ComputeCodeFix(context, context.Diagnostics[0], anonymousFunction.Body as BlockSyntax, methodSymbol.ReturnType, semanticModel);

                            return;
                        }
                }
            }
        }

        private void ComputeCodeFix(
            CodeFixContext context,
            Diagnostic diagnostic,
            BlockSyntax body,
            TypeSyntax type,
            SemanticModel semanticModel)
        {
            if (body == null)
                return;

            if (type == null)
                return;

            ITypeSymbol typeSymbol = semanticModel.GetTypeSymbol(type, context.CancellationToken);

            ComputeCodeFix(context, diagnostic, body, typeSymbol, semanticModel);
        }

        private void ComputeCodeFix(
            CodeFixContext context,
            Diagnostic diagnostic,
            BlockSyntax body,
            ITypeSymbol typeSymbol,
            SemanticModel semanticModel)
        {
            if (typeSymbol?.IsErrorType() == false
                && !typeSymbol.IsVoid()
                && !typeSymbol.IsIEnumerableOrConstructedFromIEnumerableOfT())
            {
                CodeAction codeAction = CodeAction.Create(
                    "Add return statement that returns default value",
                    cancellationToken => RefactorAsync(context.Document, body, typeSymbol, semanticModel, cancellationToken),
                    GetEquivalenceKey(diagnostic));

                context.RegisterCodeFix(codeAction, diagnostic);
            }
        }

        private static Task<Document> RefactorAsync(
            Document document,
            BlockSyntax body,
            ITypeSymbol typeSymbol,
            SemanticModel semanticModel,
            CancellationToken cancellationToken)
        {
            int position = -1;
            SyntaxList<StatementSyntax> statements = body.Statements;

            if (statements.Any())
            {
                position = statements.Last().FullSpan.End;
            }
            else
            {
                position = body.OpenBraceToken.FullSpan.End;
            }

            BlockSyntax newBody = body.AddStatements(SyntaxFactory.ReturnStatement(typeSymbol.ToDefaultValueSyntax(semanticModel, position)));

            if (!statements.Any())
                newBody = newBody.WithFormatterAnnotation();

            return document.ReplaceNodeAsync(body, newBody, cancellationToken);
        }
    }
}
