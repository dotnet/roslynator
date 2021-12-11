// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Immutable;
using System.Composition;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.CodeFixes;
using Roslynator.CSharp.Refactorings;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Roslynator.CSharp.CodeFixes
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(SimpleNameCodeFixProvider))]
    [Shared]
    public sealed class SimpleNameCodeFixProvider : CompilerDiagnosticCodeFixProvider
    {
        public override ImmutableArray<string> FixableDiagnosticIds
        {
            get
            {
                return ImmutableArray.Create(
                    CompilerDiagnosticIdentifiers.CS0428_CannotConvertMethodGroupToNonDelegateType,
                    CompilerDiagnosticIdentifiers.CS0246_TypeOrNamespaceNameCouldNotBeFound,
                    CompilerDiagnosticIdentifiers.CS0119_NameIsNotValidInGivenContext);
            }
        }

        public override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            SyntaxNode root = await context.GetSyntaxRootAsync().ConfigureAwait(false);

            if (!TryFindNode(root, context.Span, out SyntaxNode node))
                return;

            if (node is not SimpleNameSyntax simpleName)
                return;

            Document document = context.Document;
            Diagnostic diagnostic = context.Diagnostics[0];
            string diagnosticId = diagnostic.Id;

            if (diagnosticId == CompilerDiagnosticIdentifiers.CS0428_CannotConvertMethodGroupToNonDelegateType
                || diagnosticId == CompilerDiagnosticIdentifiers.CS0119_NameIsNotValidInGivenContext)
            {
                if (!IsEnabled(diagnosticId, CodeFixIdentifiers.AddArgumentList, document, root.SyntaxTree))
                    return;

                if (!simpleName.IsParentKind(SyntaxKind.SimpleMemberAccessExpression))
                    return;

                var memberAccess = (MemberAccessExpressionSyntax)simpleName.Parent;

                CodeAction codeAction = CodeAction.Create(
                    "Add argument list",
                    ct =>
                    {
                        InvocationExpressionSyntax invocationExpression = InvocationExpression(
                            memberAccess.WithoutTrailingTrivia(),
                            ArgumentList().WithTrailingTrivia(memberAccess.GetTrailingTrivia()));

                        return document.ReplaceNodeAsync(memberAccess, invocationExpression, ct);
                    },
                    GetEquivalenceKey(diagnostic));

                context.RegisterCodeFix(codeAction, diagnostic);
            }
            else if (diagnosticId == CompilerDiagnosticIdentifiers.CS0246_TypeOrNamespaceNameCouldNotBeFound)
            {
                if (IsEnabled(diagnosticId, CodeFixIdentifiers.ChangeArrayType, document, root.SyntaxTree)
                    && (simpleName.Parent is ArrayTypeSyntax arrayType)
                    && (arrayType.Parent is ArrayCreationExpressionSyntax arrayCreation)
                    && object.ReferenceEquals(simpleName, arrayType.ElementType))
                {
                    ExpressionSyntax expression = arrayCreation.Initializer?.Expressions.FirstOrDefault();

                    if (expression != null)
                    {
                        SemanticModel semanticModel = await context.GetSemanticModelAsync().ConfigureAwait(false);

                        ITypeSymbol typeSymbol = semanticModel.GetTypeSymbol(expression, context.CancellationToken);

                        if (typeSymbol?.SupportsExplicitDeclaration() == true)
                        {
                            TypeSyntax newType = typeSymbol.ToTypeSyntax()
                                .WithSimplifierAnnotation()
                                .WithTriviaFrom(simpleName);

                            CodeAction codeAction = CodeAction.Create(
                                $"Change element type to '{SymbolDisplay.ToMinimalDisplayString(typeSymbol, semanticModel, simpleName.SpanStart, SymbolDisplayFormats.DisplayName)}'",
                                ct => document.ReplaceNodeAsync(simpleName, newType, ct),
                                GetEquivalenceKey(diagnostic));

                            context.RegisterCodeFix(codeAction, diagnostic);
                            return;
                        }
                    }
                }

                if (IsEnabled(diagnosticId, CodeFixIdentifiers.ChangeMemberTypeAccordingToReturnExpression, document, root.SyntaxTree))
                {
                    ExpressionSyntax expression = GetReturnExpression(simpleName);

                    if (expression != null)
                    {
                        SemanticModel semanticModel = await context.GetSemanticModelAsync().ConfigureAwait(false);

                        ChangeMemberTypeRefactoring.ComputeCodeFix(context, diagnostic, expression, semanticModel);
                    }
                }
            }
        }

        private static ExpressionSyntax GetReturnExpression(SyntaxNode node)
        {
            switch (node.Parent)
            {
                case MethodDeclarationSyntax methodDeclaration:
                    {
                        if (object.ReferenceEquals(node, methodDeclaration.ReturnType))
                        {
                            ExpressionSyntax expression = methodDeclaration.ExpressionBody?.Expression;

                            if (expression != null)
                                return expression;

                            StatementSyntax statement = methodDeclaration.Body?.SingleNonBlockStatementOrDefault();

                            return (statement as ReturnStatementSyntax)?.Expression;
                        }

                        break;
                    }
                case LocalFunctionStatementSyntax localFunction:
                    {
                        if (object.ReferenceEquals(node, localFunction.ReturnType))
                        {
                            ExpressionSyntax expression = localFunction.ExpressionBody?.Expression;

                            if (expression != null)
                                return expression;

                            StatementSyntax statement = localFunction.Body?.SingleNonBlockStatementOrDefault();

                            return (statement as ReturnStatementSyntax)?.Expression;
                        }

                        break;
                    }
                case VariableDeclarationSyntax variableDeclaration:
                    {
                        if (object.ReferenceEquals(node, variableDeclaration.Type)
                            && node.Parent.IsParentKind(SyntaxKind.FieldDeclaration))
                        {
                            return variableDeclaration
                                .Variables
                                .SingleOrDefault(shouldThrow: false)?
                                .Initializer?
                                .Value;
                        }

                        break;
                    }
            }

            return null;
        }
    }
}
