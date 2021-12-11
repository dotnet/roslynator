// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Immutable;
using System.Composition;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.CodeFixes;
using Roslynator.CSharp.Syntax;

namespace Roslynator.CSharp.CodeFixes
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(TypeParameterConstraintClauseCodeFixProvider))]
    [Shared]
    public sealed class TypeParameterConstraintClauseCodeFixProvider : CompilerDiagnosticCodeFixProvider
    {
        public override ImmutableArray<string> FixableDiagnosticIds
        {
            get
            {
                return ImmutableArray.Create(
                    CompilerDiagnosticIdentifiers.CS0080_ConstraintsAreNotAllowedOnNonGenericDeclarations,
                    CompilerDiagnosticIdentifiers.CS0409_ConstraintClauseHasAlreadyBeenSpecified);
            }
        }

        public override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            SyntaxNode root = await context.GetSyntaxRootAsync().ConfigureAwait(false);

            if (!TryFindFirstAncestorOrSelf(root, context.Span, out TypeParameterConstraintClauseSyntax constraintClause))
                return;

            foreach (Diagnostic diagnostic in context.Diagnostics)
            {
                switch (diagnostic.Id)
                {
                    case CompilerDiagnosticIdentifiers.CS0080_ConstraintsAreNotAllowedOnNonGenericDeclarations:
                        {
                            if (!IsEnabled(diagnostic.Id, CodeFixIdentifiers.RemoveConstraintClauses, context.Document, root.SyntaxTree))
                                break;

                            GenericInfo genericInfo = SyntaxInfo.GenericInfo(constraintClause);

                            if (!genericInfo.Success)
                                break;

                            CodeAction codeAction = CodeAction.Create(
                                "Remove constraints",
                                ct =>
                                {
                                    GenericInfo newGenericInfo = genericInfo.RemoveAllConstraintClauses();

                                    return context.Document.ReplaceNodeAsync(genericInfo.Node, newGenericInfo.Node, ct);
                                },
                                GetEquivalenceKey(diagnostic));

                            context.RegisterCodeFix(codeAction, diagnostic);
                            break;
                        }
                    case CompilerDiagnosticIdentifiers.CS0409_ConstraintClauseHasAlreadyBeenSpecified:
                        {
                            if (!IsEnabled(diagnostic.Id, CodeFixIdentifiers.CombineConstraintClauses, context.Document, root.SyntaxTree))
                                break;

                            GenericInfo genericInfo = SyntaxInfo.GenericInfo(constraintClause);

                            int index = genericInfo.ConstraintClauses.IndexOf(constraintClause);

                            string name = constraintClause.Name.Identifier.ValueText;

                            TypeParameterConstraintClauseSyntax constraintClause2 = genericInfo.FindConstraintClause(name);

                            if (constraintClause2 == null)
                                break;

                            CodeAction codeAction = CodeAction.Create(
                                $"Combine constraints for '{name}'",
                                ct =>
                                {
                                    TypeParameterConstraintClauseSyntax newConstraintClause = constraintClause2.WithConstraints(constraintClause2.Constraints.AddRange(constraintClause.Constraints));

                                    SyntaxList<TypeParameterConstraintClauseSyntax> newConstraintClauses = genericInfo.ConstraintClauses
                                        .Replace(constraintClause2, newConstraintClause)
                                        .RemoveAt(index);

                                    GenericInfo newGenericInfo = genericInfo.WithConstraintClauses(newConstraintClauses);

                                    return context.Document.ReplaceNodeAsync(genericInfo.Node, newGenericInfo.Node, ct);
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
