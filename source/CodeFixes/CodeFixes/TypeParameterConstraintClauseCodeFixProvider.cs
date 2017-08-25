// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Roslynator.CSharp.CodeFixes
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(TypeParameterConstraintClauseCodeFixProvider))]
    [Shared]
    public class TypeParameterConstraintClauseCodeFixProvider : BaseCodeFixProvider
    {
        public sealed override ImmutableArray<string> FixableDiagnosticIds
        {
            get
            {
                return ImmutableArray.Create(
                    CompilerDiagnosticIdentifiers.ConstraintsAreNotAllowedOnNonGenericDeclarations,
                    CompilerDiagnosticIdentifiers.ConstraintClauseHasAlreadyBeenSpecified);
            }
        }

        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            if (!Settings.IsAnyCodeFixEnabled(
                CodeFixIdentifiers.RemoveConstraintClauses,
                CodeFixIdentifiers.CombineConstraintClauses))
            {
                return;
            }

            SyntaxNode root = await context.GetSyntaxRootAsync().ConfigureAwait(false);

            if (!TryFindFirstAncestorOrSelf(root, context.Span, out TypeParameterConstraintClauseSyntax constraintClause))
                return;

            foreach (Diagnostic diagnostic in context.Diagnostics)
            {
                switch (diagnostic.Id)
                {
                    case CompilerDiagnosticIdentifiers.ConstraintsAreNotAllowedOnNonGenericDeclarations:
                        {
                            if (!Settings.IsCodeFixEnabled(CodeFixIdentifiers.RemoveConstraintClauses))
                                break;

                            CodeAction codeAction = CodeAction.Create(
                                "Remove constraints",
                                cancellationToken =>
                                {
                                    SyntaxNode node = constraintClause.Parent;

                                    SyntaxNode newNode = GenericSyntax.RemoveConstraintClauses(node);

                                    return context.Document.ReplaceNodeAsync(node, newNode, cancellationToken);
                                },
                                GetEquivalenceKey(diagnostic));

                            context.RegisterCodeFix(codeAction, diagnostic);
                            break;
                        }
                    case CompilerDiagnosticIdentifiers.ConstraintClauseHasAlreadyBeenSpecified:
                        {
                            if (!Settings.IsCodeFixEnabled(CodeFixIdentifiers.CombineConstraintClauses))
                                break;

                            SyntaxList<TypeParameterConstraintClauseSyntax> constraintClauses = GenericSyntax.GetContainingList(constraintClause);

                            int index = constraintClauses.IndexOf(constraintClause);

                            string name = constraintClause.NameText();

                            TypeParameterConstraintClauseSyntax constraintClause2 = constraintClauses.FirstOrDefault(f => string.Equals(name, f.NameText(), StringComparison.Ordinal));

                            if (constraintClause2 == null)
                                break;

                            CodeAction codeAction = CodeAction.Create(
                                $"Combine constraints for '{name}'",
                                cancellationToken =>
                                {
                                    SyntaxNode node = constraintClause.Parent;

                                    TypeParameterConstraintClauseSyntax newConstraintClause = constraintClause2.WithConstraints(constraintClause2.Constraints.AddRange(constraintClause.Constraints));

                                    SyntaxList<TypeParameterConstraintClauseSyntax> newConstraintClauses = constraintClauses
                                        .Replace(constraintClause2, newConstraintClause)
                                        .RemoveAt(index);

                                    SyntaxNode newNode = GenericSyntax.WithConstraintClauses(node, newConstraintClauses);

                                    return context.Document.ReplaceNodeAsync(node, newNode, cancellationToken);
                                },
                                GetEquivalenceKey(diagnostic));

                            context.RegisterCodeFix(codeAction, diagnostic);

                            break;
                        }
                }
            }
        }
    }
}
