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
using Roslynator.CSharp.Syntax;

namespace Roslynator.CSharp.CodeFixes
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(TypeParameterConstraintCodeFixProvider))]
    [Shared]
    public sealed class TypeParameterConstraintCodeFixProvider : CompilerDiagnosticCodeFixProvider
    {
        public override ImmutableArray<string> FixableDiagnosticIds
        {
            get
            {
                return ImmutableArray.Create(
                    CompilerDiagnosticIdentifiers.CS0401_NewConstraintMustBeLastConstraintSpecified,
                    CompilerDiagnosticIdentifiers.CS0405_DuplicateConstraintForTypeParameter,
                    CompilerDiagnosticIdentifiers.CS0449_ClassOrStructConstraintMustComeBeforeAnyOtherConstraints,
                    CompilerDiagnosticIdentifiers.CS0450_CannotSpecifyBothConstraintClassAndClassOrStructConstraint,
                    CompilerDiagnosticIdentifiers.CS0451_NewConstraintCannotBeUsedWithStructConstraint);
            }
        }

        public override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            SyntaxNode root = await context.GetSyntaxRootAsync().ConfigureAwait(false);

            if (!TryFindFirstAncestorOrSelf(root, context.Span, out TypeParameterConstraintSyntax constraint))
                return;

            foreach (Diagnostic diagnostic in context.Diagnostics)
            {
                switch (diagnostic.Id)
                {
                    case CompilerDiagnosticIdentifiers.CS0401_NewConstraintMustBeLastConstraintSpecified:
                        {
                            if (!IsEnabled(diagnostic.Id, CodeFixIdentifiers.MoveConstraint, context.Document, root.SyntaxTree))
                                break;

                            TypeParameterConstraintInfo constraintInfo = SyntaxInfo.TypeParameterConstraintInfo(constraint);

                            if (!constraintInfo.Success)
                                break;

                            MoveConstraint(context, diagnostic, constraintInfo, constraintInfo.Constraints.Count - 1);

                            break;
                        }
                    case CompilerDiagnosticIdentifiers.CS0405_DuplicateConstraintForTypeParameter:
                        {
                            if (IsEnabled(diagnostic.Id, CodeFixIdentifiers.RemoveConstraint, context.Document, root.SyntaxTree))
                                RemoveConstraint(context, diagnostic, constraint);

                            break;
                        }
                    case CompilerDiagnosticIdentifiers.CS0449_ClassOrStructConstraintMustComeBeforeAnyOtherConstraints:
                        {
                            if (!IsEnabled(diagnostic.Id, CodeFixIdentifiers.MoveConstraint, context.Document, root.SyntaxTree))
                                break;

                            TypeParameterConstraintInfo constraintInfo = SyntaxInfo.TypeParameterConstraintInfo(constraint);

                            if (!constraintInfo.Success)
                                break;

                            if (constraintInfo.IsDuplicateConstraint)
                            {
                                RemoveConstraint(context, diagnostic, constraint);
                            }
                            else
                            {
                                MoveConstraint(context, diagnostic, constraintInfo, 0);
                            }

                            break;
                        }
                    case CompilerDiagnosticIdentifiers.CS0450_CannotSpecifyBothConstraintClassAndClassOrStructConstraint:
                        {
                            if (!IsEnabled(diagnostic.Id, CodeFixIdentifiers.RemoveConstraint, context.Document, root.SyntaxTree))
                                break;

                            TypeParameterConstraintInfo constraintInfo = SyntaxInfo.TypeParameterConstraintInfo(constraint);

                            if (!constraintInfo.Success)
                                break;

                            RemoveConstraint(context, diagnostic, constraint);

                            TypeParameterConstraintSyntax classConstraint = constraintInfo.Constraints.Find(SyntaxKind.ClassConstraint);

                            if (classConstraint != null)
                                RemoveConstraint(context, diagnostic, classConstraint);

                            TypeParameterConstraintSyntax structConstraint = constraintInfo.Constraints.Find(SyntaxKind.StructConstraint);

                            if (structConstraint != null)
                                RemoveConstraint(context, diagnostic, structConstraint);

                            break;
                        }
                    case CompilerDiagnosticIdentifiers.CS0451_NewConstraintCannotBeUsedWithStructConstraint:
                        {
                            if (!IsEnabled(diagnostic.Id, CodeFixIdentifiers.RemoveConstraint, context.Document, root.SyntaxTree))
                                break;

                            RemoveConstraint(context, diagnostic, constraint);

                            TypeParameterConstraintInfo constraintInfo = SyntaxInfo.TypeParameterConstraintInfo(constraint);

                            if (!constraintInfo.Success)
                                break;

                            TypeParameterConstraintSyntax structConstraint = constraintInfo.Constraints.Find(SyntaxKind.StructConstraint);

                            RemoveConstraint(context, diagnostic, structConstraint);
                            break;
                        }
                }
            }
        }

        private void MoveConstraint(
            CodeFixContext context,
            Diagnostic diagnostic,
            TypeParameterConstraintInfo constraintInfo,
            int index)
        {
            CodeAction codeAction = CodeAction.Create(
                $"Move constraint '{constraintInfo.Constraint}'",
                ct =>
                {
                    SeparatedSyntaxList<TypeParameterConstraintSyntax> newConstraints = constraintInfo.Constraints
                        .Remove(constraintInfo.Constraint)
                        .Insert(index, constraintInfo.Constraint);

                    TypeParameterConstraintClauseSyntax newNode = constraintInfo.ConstraintClause
                        .WithConstraints(newConstraints)
                        .WithFormatterAnnotation();

                    return context.Document.ReplaceNodeAsync(constraintInfo.ConstraintClause, newNode, ct);
                },
                GetEquivalenceKey(diagnostic));

            context.RegisterCodeFix(codeAction, diagnostic);
        }

        private void RemoveConstraint(
            CodeFixContext context,
            Diagnostic diagnostic,
            TypeParameterConstraintSyntax constraint)
        {
            CodeAction codeAction = CodeAction.Create(
                $"Remove constraint '{constraint}'",
                ct => context.Document.RemoveNodeAsync(constraint, ct),
                GetEquivalenceKey(diagnostic, constraint.Kind().ToString()));

            context.RegisterCodeFix(codeAction, diagnostic);
        }
    }
}
