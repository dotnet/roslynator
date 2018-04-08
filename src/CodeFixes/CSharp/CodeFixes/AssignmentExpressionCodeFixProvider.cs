// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Immutable;
using System.Composition;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.CodeFixes;

namespace Roslynator.CSharp.CodeFixes
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(AssignmentExpressionCodeFixProvider))]
    [Shared]
    public class AssignmentExpressionCodeFixProvider : BaseCodeFixProvider
    {
        public sealed override ImmutableArray<string> FixableDiagnosticIds
        {
            get { return ImmutableArray.Create(CompilerDiagnosticIdentifiers.AssignmentMadeToSameVariable); }
        }

        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            if (!Settings.IsCodeFixEnabled(CodeFixIdentifiers.RemoveRedundantAssignment))
                return;

            SyntaxNode root = await context.GetSyntaxRootAsync().ConfigureAwait(false);

            if (!TryFindFirstAncestorOrSelf(root, context.Span, out AssignmentExpressionSyntax assignmentExpression))
                return;

            foreach (Diagnostic diagnostic in context.Diagnostics)
            {
                switch (diagnostic.Id)
                {
                    case CompilerDiagnosticIdentifiers.AssignmentMadeToSameVariable:
                        {
                            if (!Settings.IsCodeFixEnabled(CodeFixIdentifiers.RemoveRedundantAssignment))
                                break;

                            if (!(assignmentExpression.Parent is ExpressionStatementSyntax expressionStatement))
                                break;

                            CodeFixRegistrator.RemoveStatement(context, diagnostic, expressionStatement, title: "Remove assignment");
                            break;
                        }
                }
            }
        }
    }
}
