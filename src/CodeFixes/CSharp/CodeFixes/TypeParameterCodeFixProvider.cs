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
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(TypeParameterCodeFixProvider))]
    [Shared]
    public sealed class TypeParameterCodeFixProvider : CompilerDiagnosticCodeFixProvider
    {
        public override ImmutableArray<string> FixableDiagnosticIds
        {
            get { return ImmutableArray.Create(CompilerDiagnosticIdentifiers.CS0693_TypeParameterHasSameNameAsTypeParameterFromOuterType); }
        }

        public override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            Diagnostic diagnostic = context.Diagnostics[0];

            SyntaxNode root = await context.GetSyntaxRootAsync().ConfigureAwait(false);

            if (!IsEnabled(diagnostic.Id, CodeFixIdentifiers.RemoveTypeParameter, context.Document, root.SyntaxTree))
                return;

            if (!TryFindFirstAncestorOrSelf(root, context.Span, out TypeParameterSyntax typeParameter))
                return;

            string name = typeParameter.Identifier.ValueText;

            if (string.IsNullOrEmpty(name))
                return;

            CodeAction codeAction = CodeAction.Create(
                $"Remove type parameter '{name}'",
                ct =>
                {
                    GenericInfo genericInfo = SyntaxInfo.GenericInfo(typeParameter);

                    GenericInfo newGenericInfo = genericInfo.RemoveTypeParameter(typeParameter);

                    TypeParameterConstraintClauseSyntax constraintClause = genericInfo.FindConstraintClause(name);

                    if (constraintClause != null)
                        newGenericInfo = newGenericInfo.RemoveConstraintClause(constraintClause);

                    return context.Document.ReplaceNodeAsync(genericInfo.Node, newGenericInfo.Node, ct);
                },
                GetEquivalenceKey(diagnostic));

            context.RegisterCodeFix(codeAction, diagnostic);
        }
    }
}
