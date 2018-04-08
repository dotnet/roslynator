// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Immutable;
using System.Composition;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.CodeFixes;
using Roslynator.CSharp.Helpers;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Roslynator.CSharp.CodeFixes
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(IdentifierNameCodeFixProvider))]
    [Shared]
    public class IdentifierNameCodeFixProvider : BaseCodeFixProvider
    {
        public sealed override ImmutableArray<string> FixableDiagnosticIds
        {
            get
            {
                return ImmutableArray.Create(
                    CompilerDiagnosticIdentifiers.UseOfUnassignedLocalVariable,
                    CompilerDiagnosticIdentifiers.NameDoesNotExistInCurrentContext);
            }
        }

        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            if (!Settings.IsAnyCodeFixEnabled(
                CodeFixIdentifiers.InitializeLocalVariableWithDefaultValue,
                CodeFixIdentifiers.AddVariableType))
            {
                return;
            }

            SyntaxNode root = await context.GetSyntaxRootAsync().ConfigureAwait(false);

            if (!TryFindFirstAncestorOrSelf(root, context.Span, out IdentifierNameSyntax identifierName))
                return;

            foreach (Diagnostic diagnostic in context.Diagnostics)
            {
                switch (diagnostic.Id)
                {
                    case CompilerDiagnosticIdentifiers.UseOfUnassignedLocalVariable:
                        {
                            CodeAction codeAction = CodeAction.Create(
                                $"Initialize '{identifierName.Identifier.ValueText}' with default value",
                                cancellationToken => RefactorAsync(context.Document, identifierName, cancellationToken),
                                GetEquivalenceKey(diagnostic));

                            context.RegisterCodeFix(codeAction, diagnostic);
                            break;
                        }
                    case CompilerDiagnosticIdentifiers.NameDoesNotExistInCurrentContext:
                        {
                            if (!(identifierName.Parent is ArgumentSyntax argument))
                                break;

                            if (argument.RefOrOutKeyword.Kind() != SyntaxKind.OutKeyword)
                                break;

                            SemanticModel semanticModel = await context.GetSemanticModelAsync().ConfigureAwait(false);

                            foreach (ITypeSymbol typeSymbol in DetermineParameterTypeHelper.DetermineParameterTypes(argument, semanticModel, context.CancellationToken))
                            {
                                if (typeSymbol.Kind == SymbolKind.TypeParameter)
                                    continue;

                                string typeName = SymbolDisplay.ToMinimalDisplayString(typeSymbol, semanticModel, identifierName.SpanStart, SymbolDisplayFormats.Default);

                                CodeAction codeAction = CodeAction.Create(
                                    $"Add variable type '{typeName}'",
                                    cancellationToken =>
                                    {
                                        DeclarationExpressionSyntax newNode = DeclarationExpression(
                                            ParseName(typeName),
                                            SingleVariableDesignation(identifierName.Identifier.WithoutTrivia()).WithLeadingTrivia(Space));

                                        newNode = newNode
                                            .WithTriviaFrom(identifierName)
                                            .WithFormatterAnnotation();

                                        return context.Document.ReplaceNodeAsync(identifierName, newNode, cancellationToken);
                                    },
                                    GetEquivalenceKey(diagnostic, typeName));

                                context.RegisterCodeFix(codeAction, diagnostic);
                                break;
                            }

                            break;
                        }
                }
            }
        }

        public static async Task<Document> RefactorAsync(
            Document document,
            IdentifierNameSyntax identifierName,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            SemanticModel semanticModel = await document.GetSemanticModelAsync(cancellationToken).ConfigureAwait(false);

            var localSymbol = semanticModel.GetSymbol(identifierName, cancellationToken) as ILocalSymbol;

            if (localSymbol?.Type?.IsErrorType() == false
                && localSymbol.GetSyntax(cancellationToken) is VariableDeclaratorSyntax variableDeclarator)
            {
                SyntaxToken identifier = variableDeclarator.Identifier;

                var variableDeclaration = (VariableDeclarationSyntax)variableDeclarator.Parent;

                ExpressionSyntax value = localSymbol.Type.GetDefaultValueSyntax(variableDeclaration.Type.WithoutTrivia());

                EqualsValueClauseSyntax newEqualsValue = EqualsValueClause(value)
                    .WithLeadingTrivia(TriviaList(Space))
                    .WithTrailingTrivia(identifier.TrailingTrivia);

                VariableDeclaratorSyntax newNode = variableDeclarator
                    .WithInitializer(newEqualsValue)
                    .WithIdentifier(identifier.WithoutTrailingTrivia());

                return await document.ReplaceNodeAsync(variableDeclarator, newNode, cancellationToken).ConfigureAwait(false);
            }

            Debug.Fail(identifierName.ToString());

            return document;
        }
    }
}
