// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Immutable;
using System.Composition;
using System.Diagnostics;
using System.Linq;
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
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MarkLocalVariableAsConstCodeFixProvider))]
    [Shared]
    public sealed class MarkLocalVariableAsConstCodeFixProvider : BaseCodeFixProvider
    {
        public override ImmutableArray<string> FixableDiagnosticIds
        {
            get { return ImmutableArray.Create(DiagnosticIdentifiers.MarkLocalVariableAsConst); }
        }

        public override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            SyntaxNode root = await context.GetSyntaxRootAsync().ConfigureAwait(false);

            if (!TryFindFirstAncestorOrSelf(root, context.Span, out LocalDeclarationStatementSyntax localDeclaration))
                return;

            Diagnostic diagnostic = context.Diagnostics[0];

            CodeAction codeAction = CodeAction.Create(
                $"Mark {GetNames(localDeclaration)} as const",
                ct => MarkLocalVariableAsConstAsync(context.Document, localDeclaration, ct),
                GetEquivalenceKey(diagnostic));

            context.RegisterCodeFix(codeAction, diagnostic);
        }

        private static string GetNames(LocalDeclarationStatementSyntax localDeclaration)
        {
            VariableDeclarationSyntax declaration = localDeclaration.Declaration;

            SeparatedSyntaxList<VariableDeclaratorSyntax> variables = declaration.Variables;

            if (variables.Count == 1)
            {
                return $"'{variables[0].Identifier.ValueText}'";
            }
            else
            {
                return string.Join(", ", variables.Select(f => $"'{f.Identifier.ValueText}'"));
            }
        }

        private static async Task<Document> MarkLocalVariableAsConstAsync(
            Document document,
            LocalDeclarationStatementSyntax localDeclaration,
            CancellationToken cancellationToken)
        {
            TypeSyntax type = localDeclaration.Declaration.Type;

            LocalDeclarationStatementSyntax newNode = localDeclaration;

            if (type.IsVar)
            {
                SemanticModel semanticModel = await document.GetSemanticModelAsync(cancellationToken).ConfigureAwait(false);

                ITypeSymbol typeSymbol = semanticModel.GetTypeSymbol(type, cancellationToken);

                TypeSyntax newType = typeSymbol.ToMinimalTypeSyntax(semanticModel, localDeclaration.SpanStart, SymbolDisplayFormats.FullName_WithoutNullableReferenceTypeModifier);

                newNode = newNode.ReplaceNode(type, newType.WithTriviaFrom(type));
            }

            Debug.Assert(!newNode.Modifiers.Any(), newNode.Modifiers.ToString());

            if (newNode.Modifiers.Any())
            {
                newNode = newNode.InsertModifier(SyntaxKind.ConstKeyword);
            }
            else
            {
                newNode = newNode
                    .WithoutLeadingTrivia()
                    .WithModifiers(TokenList(Token(SyntaxKind.ConstKeyword).WithLeadingTrivia(newNode.GetLeadingTrivia())));
            }

            return await document.ReplaceNodeAsync(localDeclaration, newNode, cancellationToken).ConfigureAwait(false);
        }
    }
}
