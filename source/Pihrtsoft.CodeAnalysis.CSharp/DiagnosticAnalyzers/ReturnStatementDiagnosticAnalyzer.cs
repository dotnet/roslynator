// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Text;

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

            context.RegisterSyntaxNodeAction(f => AnalyzeReturnStatement(f), SyntaxKind.ReturnStatement);
        }

        private void AnalyzeReturnStatement(SyntaxNodeAnalysisContext context)
        {
            if (GeneratedCodeAnalyzer?.IsGeneratedCode(context) == true)
                return;

            var returnStatement = (ReturnStatementSyntax)context.Node;

            if (returnStatement.Expression?.IsKind(SyntaxKind.IdentifierName) != true)
                return;

            LocalDeclarationStatementSyntax localDeclaration = GetLocalDeclaration(returnStatement);

            if (localDeclaration == null)
                return;

            VariableDeclaratorSyntax declarator = GetVariableDeclarator(localDeclaration);

            if (declarator == null)
                return;

            ISymbol returnSymbol = context.SemanticModel.GetSymbolInfo(returnStatement.Expression, context.CancellationToken).Symbol;

            if (returnSymbol == null || returnSymbol.Kind == SymbolKind.ErrorType)
                return;

            ISymbol declaredSymbol = context.SemanticModel.GetDeclaredSymbol(declarator, context.CancellationToken);

            if (returnSymbol.Equals(declaredSymbol))
            {
                context.ReportDiagnostic(
                    DiagnosticDescriptors.MergeLocalDeclarationWithReturnStatement,
                    Location.Create(
                        returnStatement.SyntaxTree,
                        TextSpan.FromBounds(localDeclaration.Span.Start, returnStatement.Span.End)));

                DiagnosticDescriptor descriptor = DiagnosticDescriptors.MergeLocalDeclarationWithReturnStatementFadeOut;

                DiagnosticHelper.FadeOutNode(context, localDeclaration.Declaration.Type, descriptor);
                DiagnosticHelper.FadeOutToken(context, declarator.Identifier, descriptor);
                DiagnosticHelper.FadeOutToken(context, declarator.Initializer.EqualsToken, descriptor);
                DiagnosticHelper.FadeOutToken(context, localDeclaration.SemicolonToken, descriptor);
                DiagnosticHelper.FadeOutNode(context, returnStatement.Expression, descriptor);
            }
        }

        private static LocalDeclarationStatementSyntax GetLocalDeclaration(ReturnStatementSyntax returnStatement)
        {
            if (returnStatement.Parent?.IsKind(SyntaxKind.Block) == true)
            {
                var block = (BlockSyntax)returnStatement.Parent;

                if (block.Statements.Count > 1)
                {
                    int index = block.Statements.IndexOf(returnStatement);

                    if (index > 0)
                    {
                        StatementSyntax statement = block.Statements[index - 1];

                        if (statement.IsKind(SyntaxKind.LocalDeclarationStatement))
                            return (LocalDeclarationStatementSyntax)statement;
                    }
                }
            }

            return null;
        }

        private static VariableDeclaratorSyntax GetVariableDeclarator(LocalDeclarationStatementSyntax localDeclaration)
        {
            if (localDeclaration.Declaration?.Variables.Count == 1)
            {
                VariableDeclaratorSyntax declarator = localDeclaration.Declaration.Variables[0];

                if (declarator.Initializer?.Value != null)
                    return declarator;
            }

            return null;
        }
    }
}
