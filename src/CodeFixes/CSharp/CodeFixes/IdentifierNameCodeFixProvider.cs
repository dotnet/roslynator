// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Immutable;
using System.Composition;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.CodeFixes;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Roslynator.CSharp.CodeFixes
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(IdentifierNameCodeFixProvider))]
    [Shared]
    public sealed class IdentifierNameCodeFixProvider : CompilerDiagnosticCodeFixProvider
    {
        public override ImmutableArray<string> FixableDiagnosticIds
        {
            get
            {
                return ImmutableArray.Create(
                    CompilerDiagnosticIdentifiers.CS0165_UseOfUnassignedLocalVariable,
                    CompilerDiagnosticIdentifiers.CS0103_NameDoesNotExistInCurrentContext);
            }
        }

        public override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            SyntaxNode root = await context.GetSyntaxRootAsync().ConfigureAwait(false);

            if (!TryFindFirstAncestorOrSelf(root, context.Span, out SimpleNameSyntax simpleName))
                return;

            if (simpleName is not IdentifierNameSyntax identifierName)
                return;

            Document document = context.Document;
            CancellationToken cancellationToken = context.CancellationToken;

            foreach (Diagnostic diagnostic in context.Diagnostics)
            {
                switch (diagnostic.Id)
                {
                    case CompilerDiagnosticIdentifiers.CS0165_UseOfUnassignedLocalVariable:
                        {
                            if (!IsEnabled(diagnostic.Id, CodeFixIdentifiers.InitializeLocalVariableWithDefaultValue, context.Document, root.SyntaxTree))
                                return;

                            SemanticModel semanticModel = await document.GetSemanticModelAsync(cancellationToken).ConfigureAwait(false);

                            if (semanticModel.GetSymbol(identifierName, cancellationToken) is not ILocalSymbol localSymbol)
                                break;

                            ITypeSymbol typeSymbol = localSymbol.Type;

                            if (typeSymbol.Kind == SymbolKind.ErrorType)
                                break;

                            if (localSymbol.GetSyntax(cancellationToken) is not VariableDeclaratorSyntax variableDeclarator)
                                break;

                            CodeAction codeAction = CodeAction.Create(
                                $"Initialize '{identifierName.Identifier.ValueText}' with default value",
                                ct =>
                                {
                                    SyntaxToken identifier = variableDeclarator.Identifier;

                                    var variableDeclaration = (VariableDeclarationSyntax)variableDeclarator.Parent;

                                    ExpressionSyntax value = typeSymbol.GetDefaultValueSyntax(variableDeclaration.Type.WithoutTrivia(), document.GetDefaultSyntaxOptions());

                                    if (value.IsKind(SyntaxKind.DefaultExpression)
                                        && document.SupportsLanguageFeature(CSharpLanguageFeature.DefaultLiteral))
                                    {
                                        value = CSharpFactory.DefaultLiteralExpression().WithTriviaFrom(value);
                                    }

                                    EqualsValueClauseSyntax newEqualsValue = EqualsValueClause(value)
                                        .WithLeadingTrivia(TriviaList(Space))
                                        .WithTrailingTrivia(identifier.TrailingTrivia);

                                    VariableDeclaratorSyntax newNode = variableDeclarator
                                        .WithInitializer(newEqualsValue)
                                        .WithIdentifier(identifier.WithoutTrailingTrivia());

                                    return document.ReplaceNodeAsync(variableDeclarator, newNode, ct);
                                },
                                GetEquivalenceKey(diagnostic));

                            context.RegisterCodeFix(codeAction, diagnostic);
                            break;
                        }
                    case CompilerDiagnosticIdentifiers.CS0103_NameDoesNotExistInCurrentContext:
                        {
                            if (!IsEnabled(diagnostic.Id, CodeFixIdentifiers.AddVariableType, context.Document, root.SyntaxTree))
                                return;

                            if (identifierName.Parent is not ArgumentSyntax argument)
                                break;

                            if (!argument.RefOrOutKeyword.IsKind(SyntaxKind.OutKeyword))
                                break;

                            SemanticModel semanticModel = await context.GetSemanticModelAsync().ConfigureAwait(false);

                            foreach (ITypeSymbol typeSymbol in DetermineParameterTypeHelper.DetermineParameterTypes(argument, semanticModel, cancellationToken))
                            {
                                if (typeSymbol.Kind == SymbolKind.TypeParameter)
                                    continue;

                                string typeName = SymbolDisplay.ToMinimalDisplayString(typeSymbol, semanticModel, identifierName.SpanStart, SymbolDisplayFormats.DisplayName);

                                CodeAction codeAction = CodeAction.Create(
                                    $"Add variable type '{typeName}'",
                                    ct =>
                                    {
                                        DeclarationExpressionSyntax newNode = DeclarationExpression(
                                            typeSymbol.ToTypeSyntax(),
                                            SingleVariableDesignation(identifierName.Identifier.WithoutTrivia()).WithLeadingTrivia(Space));

                                        newNode = newNode
                                            .WithTriviaFrom(identifierName)
                                            .WithFormatterAnnotation();

                                        return document.ReplaceNodeAsync(identifierName, newNode, ct);
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
    }
}
