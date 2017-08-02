// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Immutable;
using System.Composition;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Roslynator.CSharp.CodeFixes
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(TypeParameterCodeFixProvider))]
    [Shared]
    public class TypeParameterCodeFixProvider : BaseCodeFixProvider
    {
        public sealed override ImmutableArray<string> FixableDiagnosticIds
        {
            get { return ImmutableArray.Create(CompilerDiagnosticIdentifiers.TypeParameterHasSameNameAsTypeParameterFromOuterType); }
        }

        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            if (!Settings.IsCodeFixEnabled(CodeFixIdentifiers.RemoveTypeParameter))
                return;

            SyntaxNode root = await context.GetSyntaxRootAsync().ConfigureAwait(false);

            if (!TryFindFirstAncestorOrSelf(root, context.Span, out TypeParameterSyntax typeParameter))
                return;

            foreach (Diagnostic diagnostic in context.Diagnostics)
            {
                switch (diagnostic.Id)
                {
                    case CompilerDiagnosticIdentifiers.TypeParameterHasSameNameAsTypeParameterFromOuterType:
                        {
                            TypeParameterInfo info;
                            if (TypeParameterInfo.TryCreate(typeParameter, out info))
                            {
                                CodeAction codeAction = CodeAction.Create(
                                    $"Remove type parameter '{info.Name}'",
                                    cancellationToken =>
                                    {
                                        SeparatedSyntaxList<TypeParameterSyntax> parameters = info.TypeParameterList.Parameters;

                                        TypeParameterListSyntax newTypeParameterList = (parameters.Count == 1)
                                            ? default(TypeParameterListSyntax)
                                            : info.TypeParameterList.WithParameters(parameters.Remove(typeParameter));

                                        SyntaxNode newNode = GenericDeclarationHelper.WithTypeParameterList(info.Declaration, newTypeParameterList);

                                        TypeParameterConstraintClauseSyntax constraintClause = info.ConstraintClause;

                                        if (constraintClause != null)
                                            newNode = GenericDeclarationHelper.WithConstraintClauses(newNode, info.ConstraintClauses.Remove(constraintClause));

                                        return context.Document.ReplaceNodeAsync(info.Declaration, newNode, cancellationToken);
                                    },
                                    GetEquivalenceKey(diagnostic));

                                context.RegisterCodeFix(codeAction, diagnostic);
                            }

                            break;
                        }
                }
            }
        }
    }
}
