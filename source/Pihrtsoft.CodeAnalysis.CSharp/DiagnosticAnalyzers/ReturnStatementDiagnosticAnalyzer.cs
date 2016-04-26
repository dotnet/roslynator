// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Pihrtsoft.CodeAnalysis.CSharp.DiagnosticAnalyzers
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class ReturnStatementDiagnosticAnalyzer : BaseDiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        {
            get
            {
                return ImmutableArray.Create(
                    DiagnosticDescriptors.MergeLocalDeclarationWithReturnStatement,
                    DiagnosticDescriptors.MergeLocalDeclarationWithReturnStatementFadeOut);
            }
        }

        public override void Initialize(AnalysisContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            context.RegisterSyntaxNodeAction(f => AnalyzeSyntaxNode(f), SyntaxKind.ReturnStatement);
        }

        private void AnalyzeSyntaxNode(SyntaxNodeAnalysisContext context)
        {
            if (GeneratedCodeAnalyzer?.IsGeneratedCode(context) == true)
                return;

            var returnStatement = (ReturnStatementSyntax)context.Node;

            if (returnStatement.Expression == null || returnStatement.Parent == null)
                return;

            if (!returnStatement.Parent.IsKind(SyntaxKind.Block))
                return;

            var block = (BlockSyntax)returnStatement.Parent;
            if (block.Statements.Count <= 1)
                return;

            int index = block.Statements.IndexOf(returnStatement);
            if (index == 0)
                return;

            StatementSyntax statement = block.Statements[index - 1];

            if (!statement.IsKind(SyntaxKind.LocalDeclarationStatement))
                return;

            var localDeclaration = (LocalDeclarationStatementSyntax)statement;

            ISymbol returnSymbol = context.SemanticModel.GetSymbolInfo(returnStatement.Expression, context.CancellationToken).Symbol;
            if (returnSymbol == null)
                return;

            VariableDeclaratorSyntax declarator = localDeclaration.Declaration.Variables.SingleOrDefault();
            if (declarator == null)
                return;

            if (declarator.Initializer?.Value == null)
                return;

            ISymbol declaredSymbol = context.SemanticModel.GetDeclaredSymbol(declarator, context.CancellationToken);

            if (returnSymbol.Equals(declaredSymbol))
            {
                Diagnostic diagnostic = Diagnostic.Create(
                    DiagnosticDescriptors.MergeLocalDeclarationWithReturnStatement,
                    localDeclaration.GetLocation(),
                    new Location[] { returnStatement.GetLocation() }.AsEnumerable());

                context.ReportDiagnostic(diagnostic);

                DiagnosticDescriptor descriptor = DiagnosticDescriptors.MergeLocalDeclarationWithReturnStatementFadeOut;

                DiagnosticHelper.FadeOutNode(context, localDeclaration.Declaration.Type, descriptor);
                DiagnosticHelper.FadeOutToken(context, declarator.Identifier, descriptor);
                DiagnosticHelper.FadeOutToken(context, declarator.Initializer.EqualsToken, descriptor);
                DiagnosticHelper.FadeOutToken(context, localDeclaration.SemicolonToken, descriptor);
                DiagnosticHelper.FadeOutNode(context, returnStatement.Expression, descriptor);
            }
        }
    }
}
