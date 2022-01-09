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

namespace Roslynator.CSharp.CodeFixes
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(UnsafeCodeFixProvider))]
    [Shared]
    public sealed class UnsafeCodeFixProvider : CompilerDiagnosticCodeFixProvider
    {
        public override ImmutableArray<string> FixableDiagnosticIds
        {
            get { return ImmutableArray.Create(CompilerDiagnosticIdentifiers.CS0214_PointersAndFixedSizeBuffersMayOnlyBeUsedInUnsafeContext); }
        }

        public override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            SyntaxNode root = await context.GetSyntaxRootAsync().ConfigureAwait(false);

            if (!TryFindNode(root, context.Span, out SyntaxNode node))
                return;

            Document document = context.Document;

            foreach (Diagnostic diagnostic in context.Diagnostics)
            {
                switch (diagnostic.Id)
                {
                    case CompilerDiagnosticIdentifiers.CS0214_PointersAndFixedSizeBuffersMayOnlyBeUsedInUnsafeContext:
                        {
                            if (IsEnabled(diagnostic.Id, CodeFixIdentifiers.UseExplicitTypeInsteadOfVar, document, root.SyntaxTree)
                                && node is StackAllocArrayCreationExpressionSyntax stackAllocArrayCreation
                                && node.IsParentKind(SyntaxKind.EqualsValueClause)
                                && node.Parent.IsParentKind(SyntaxKind.VariableDeclarator)
                                && node.Parent.Parent.Parent is VariableDeclarationSyntax variableDeclaration
                                && variableDeclaration.Type.IsVar)
                            {
                                TypeSyntax type = stackAllocArrayCreation.Type;

                                if (type is ArrayTypeSyntax arrayType)
                                    type = arrayType.ElementType;

                                CodeAction spanCodeAction = CodeAction.Create(
                                    "Use Span<T>",
                                    ct =>
                                    {
                                        TypeSyntax spanOfT = SyntaxFactory.ParseTypeName($"global::System.Span<{type}>")
                                            .WithSimplifierAnnotation();

                                        return document.ReplaceNodeAsync(variableDeclaration.Type, spanOfT, ct);
                                    },
                                    GetEquivalenceKey(diagnostic, CodeFixIdentifiers.UseExplicitTypeInsteadOfVar, "System_Span_T"));

                                context.RegisterCodeFix(spanCodeAction, diagnostic);

                                CodeAction readOnlySpanCodeAction = CodeAction.Create(
                                    "Use ReadOnlySpan<T>",
                                    ct =>
                                    {
                                        TypeSyntax spanOfT = SyntaxFactory.ParseTypeName($"global::System.ReadOnlySpan<{type}>")
                                            .WithSimplifierAnnotation();

                                        return document.ReplaceNodeAsync(variableDeclaration.Type, spanOfT, ct);
                                    },
                                    GetEquivalenceKey(diagnostic, CodeFixIdentifiers.UseExplicitTypeInsteadOfVar, "System_ReadOnlySpan_T"));

                                context.RegisterCodeFix(readOnlySpanCodeAction, diagnostic);
                            }

                            var fStatement = false;
                            var fMemberDeclaration = false;

                            foreach (SyntaxNode ancestor in node.AncestorsAndSelf())
                            {
                                if (fStatement
                                    && fMemberDeclaration)
                                {
                                    break;
                                }

                                if (!fStatement
                                    && ancestor is StatementSyntax)
                                {
                                    fStatement = true;

                                    if (!IsEnabled(diagnostic.Id, CodeFixIdentifiers.WrapInUnsafeStatement, document, root.SyntaxTree))
                                        continue;

                                    var statement = (StatementSyntax)ancestor;

                                    if (statement.IsKind(SyntaxKind.Block)
                                        && statement.Parent is StatementSyntax statement2)
                                    {
                                        statement = statement2;
                                    }

                                    if (statement.IsKind(SyntaxKind.UnsafeStatement))
                                        break;

                                    CodeAction codeAction = CodeAction.Create(
                                        "Wrap in unsafe block",
                                        ct =>
                                        {
                                            BlockSyntax block = (statement.IsKind(SyntaxKind.Block))
                                                ? (BlockSyntax)statement
                                                : SyntaxFactory.Block(statement);

                                            UnsafeStatementSyntax unsafeStatement = SyntaxFactory.UnsafeStatement(block).WithFormatterAnnotation();

                                            return document.ReplaceNodeAsync(statement, unsafeStatement, ct);
                                        },
                                        GetEquivalenceKey(diagnostic, CodeFixIdentifiers.WrapInUnsafeStatement));

                                    context.RegisterCodeFix(codeAction, diagnostic);
                                }
                                else if (!fMemberDeclaration
                                    && ancestor is MemberDeclarationSyntax)
                                {
                                    fMemberDeclaration = true;

                                    if (!IsEnabled(diagnostic.Id, CodeFixIdentifiers.MakeContainingDeclarationUnsafe, document, root.SyntaxTree))
                                        continue;

                                    if (!CSharpFacts.CanHaveModifiers(ancestor.Kind()))
                                        continue;

                                    ModifiersCodeFixRegistrator.AddModifier(
                                        context,
                                        diagnostic,
                                        ancestor,
                                        SyntaxKind.UnsafeKeyword,
                                        title: "Make containing declaration unsafe",
                                        additionalKey: CodeFixIdentifiers.MakeContainingDeclarationUnsafe);
                                }
                            }

                            break;
                        }
                }
            }
        }
    }
}
