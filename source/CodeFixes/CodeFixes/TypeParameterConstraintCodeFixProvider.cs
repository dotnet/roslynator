// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Immutable;
using System.Composition;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Roslynator.CSharp.CodeFixes
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(TypeParameterConstraintCodeFixProvider))]
    [Shared]
    public class TypeParameterConstraintCodeFixProvider : BaseCodeFixProvider
    {
        public sealed override ImmutableArray<string> FixableDiagnosticIds
        {
            get
            {
                return ImmutableArray.Create(
                    CompilerDiagnosticIdentifiers.NewConstraintMustBeLastConstraintSpecified,
                    CompilerDiagnosticIdentifiers.DuplicateConstraintForTypeParameter,
                    CompilerDiagnosticIdentifiers.ClassOrStructConstraintMustComeBeforeAnyOtherConstraints,
                    CompilerDiagnosticIdentifiers.CannotSpecifyBothConstraintClassAndClassOrStructConstraint,
                    CompilerDiagnosticIdentifiers.NewConstraintCannotBeUsedWithStructConstraint);
            }
        }

        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            if (!Settings.IsAnyCodeFixEnabled(
                CodeFixIdentifiers.RemoveConstraint,
                CodeFixIdentifiers.MoveConstraint))
            {
                return;
            }

            SyntaxNode root = await context.GetSyntaxRootAsync().ConfigureAwait(false);

            if (!TryFindFirstAncestorOrSelf(root, context.Span, out TypeParameterConstraintSyntax constraint))
                return;

            foreach (Diagnostic diagnostic in context.Diagnostics)
            {
                switch (diagnostic.Id)
                {
                    case CompilerDiagnosticIdentifiers.NewConstraintMustBeLastConstraintSpecified:
                        {
                            if (!Settings.IsCodeFixEnabled(CodeFixIdentifiers.MoveConstraint))
                                break;

                            SeparatedSyntaxList<TypeParameterConstraintSyntax> constraints;

                            if (GenericDeclarationHelper.TryGetContainingList(constraint, out constraints))
                                MoveConstraint(context, diagnostic, constraint, constraints, constraints.Count - 1);

                            break;
                        }
                    case CompilerDiagnosticIdentifiers.DuplicateConstraintForTypeParameter:
                        {
                            if (Settings.IsCodeFixEnabled(CodeFixIdentifiers.RemoveConstraint))
                                RemoveConstraint(context, diagnostic, constraint);

                            break;
                        }
                    case CompilerDiagnosticIdentifiers.ClassOrStructConstraintMustComeBeforeAnyOtherConstraints:
                        {
                            if (!Settings.IsCodeFixEnabled(CodeFixIdentifiers.MoveConstraint))
                                break;

                            SeparatedSyntaxList<TypeParameterConstraintSyntax> constraints;
                            if (GenericDeclarationHelper.TryGetContainingList(constraint, out constraints))
                            {
                                if (IsDuplicateConstraint(constraint, constraints))
                                {
                                    RemoveConstraint(context, diagnostic, constraint);
                                }
                                else
                                {
                                    MoveConstraint(context, diagnostic, constraint, constraints, 0);
                                }
                            }

                            break;
                        }
                    case CompilerDiagnosticIdentifiers.CannotSpecifyBothConstraintClassAndClassOrStructConstraint:
                        {
                            if (!Settings.IsCodeFixEnabled(CodeFixIdentifiers.RemoveConstraint))
                                break;

                            RemoveConstraint(context, diagnostic, constraint);

                            SeparatedSyntaxList<TypeParameterConstraintSyntax> constraintClauses;
                            if (GenericDeclarationHelper.TryGetContainingList(constraint, out constraintClauses))
                            {
                                TypeParameterConstraintSyntax classConstraint = constraintClauses.Find(SyntaxKind.ClassConstraint);

                                if (classConstraint != null)
                                    RemoveConstraint(context, diagnostic, classConstraint);

                                TypeParameterConstraintSyntax structConstraint = constraintClauses.Find(SyntaxKind.StructConstraint);

                                if (structConstraint != null)
                                    RemoveConstraint(context, diagnostic, structConstraint);
                            }

                            break;
                        }
                    case CompilerDiagnosticIdentifiers.NewConstraintCannotBeUsedWithStructConstraint:
                        {
                            if (!Settings.IsCodeFixEnabled(CodeFixIdentifiers.RemoveConstraint))
                                break;

                            RemoveConstraint(context, diagnostic, constraint);

                            SeparatedSyntaxList<TypeParameterConstraintSyntax> constraintClauses;
                            if (GenericDeclarationHelper.TryGetContainingList(constraint, out constraintClauses))
                            {
                                TypeParameterConstraintSyntax structConstraint = constraintClauses.Find(SyntaxKind.StructConstraint);
                                RemoveConstraint(context, diagnostic, structConstraint);
                            }

                            break;
                        }
                }
            }
        }

        private static bool IsDuplicateConstraint(TypeParameterConstraintSyntax constraint, SeparatedSyntaxList<TypeParameterConstraintSyntax> constraints)
        {
            int index = constraints.IndexOf(constraint);

            SyntaxKind kind = constraint.Kind();

            switch (kind)
            {
                case SyntaxKind.ClassConstraint:
                case SyntaxKind.StructConstraint:
                    {
                        for (int i = 0; i < index; i++)
                        {
                            if (constraints[i].Kind() == kind)
                                return true;
                        }

                        break;
                    }
            }

            return false;
        }

        private void MoveConstraint(
            CodeFixContext context,
            Diagnostic diagnostic,
            TypeParameterConstraintSyntax constraint,
            SeparatedSyntaxList<TypeParameterConstraintSyntax> constraints,
            int index)
        {
            CodeAction codeAction = CodeAction.Create(
                $"Move constraint '{constraint}'",
                cancellationToken =>
                {
                    var constraintClause = (TypeParameterConstraintClauseSyntax)constraint.Parent;

                    SeparatedSyntaxList<TypeParameterConstraintSyntax> newConstraints = constraints.Remove(constraint).Insert(index, constraint);

                    TypeParameterConstraintClauseSyntax newNode = constraintClause
                        .WithConstraints(newConstraints)
                        .WithFormatterAnnotation();

                    return context.Document.ReplaceNodeAsync(constraintClause, newNode, cancellationToken);
                },
                GetEquivalenceKey(diagnostic));

            context.RegisterCodeFix(codeAction, diagnostic);
        }

        private void RemoveConstraint(
            CodeFixContext context,
            Diagnostic diagnostic,
            TypeParameterConstraintSyntax constraint)
        {
            SeparatedSyntaxList<TypeParameterConstraintSyntax> constraints;
            if (GenericDeclarationHelper.TryGetContainingList(constraint, out constraints))
            {
                CodeAction codeAction = CodeAction.Create(
                    $"Remove constraint '{constraint}'",
                    cancellationToken => context.Document.RemoveNodeAsync(constraint, RemoveHelper.GetRemoveOptions(constraint), cancellationToken),
                    GetEquivalenceKey(diagnostic, constraint.Kind().ToString()));

                context.RegisterCodeFix(codeAction, diagnostic);
            }
        }
    }
}
