// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using static Roslynator.CSharp.Refactorings.RemoveRedundantEmptyLineRefactoring;

namespace Roslynator.CSharp.DiagnosticAnalyzers
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class RedundantEmptyLineDiagnosticAnalyzer : BaseDiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        {
            get { return ImmutableArray.Create(DiagnosticDescriptors.RemoveRedundantEmptyLine); }
        }

        public override void Initialize(AnalysisContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            base.Initialize(context);
            context.EnableConcurrentExecution();

            context.RegisterSyntaxNodeAction(f => Analyze(f, (ClassDeclarationSyntax)f.Node), SyntaxKind.ClassDeclaration);
            context.RegisterSyntaxNodeAction(f => Analyze(f, (StructDeclarationSyntax)f.Node), SyntaxKind.StructDeclaration);
            context.RegisterSyntaxNodeAction(f => Analyze(f, (InterfaceDeclarationSyntax)f.Node), SyntaxKind.InterfaceDeclaration);
            context.RegisterSyntaxNodeAction(f => Analyze(f, (NamespaceDeclarationSyntax)f.Node), SyntaxKind.NamespaceDeclaration);
            context.RegisterSyntaxNodeAction(f => Analyze(f, (SwitchStatementSyntax)f.Node), SyntaxKind.SwitchStatement);
            context.RegisterSyntaxNodeAction(f => Analyze(f, (TryStatementSyntax)f.Node), SyntaxKind.TryStatement);
            context.RegisterSyntaxNodeAction(f => Analyze(f, (ElseClauseSyntax)f.Node), SyntaxKind.ElseClause);

            context.RegisterSyntaxNodeAction(f => Analyze(f, (IfStatementSyntax)f.Node), SyntaxKind.IfStatement);
            context.RegisterSyntaxNodeAction(f => Analyze(f, (ForEachStatementSyntax)f.Node), SyntaxKind.ForEachStatement);
            context.RegisterSyntaxNodeAction(f => Analyze(f, (ForStatementSyntax)f.Node), SyntaxKind.ForStatement);
            context.RegisterSyntaxNodeAction(f => Analyze(f, (UsingStatementSyntax)f.Node), SyntaxKind.UsingStatement);
            context.RegisterSyntaxNodeAction(f => Analyze(f, (WhileStatementSyntax)f.Node), SyntaxKind.WhileStatement);
            context.RegisterSyntaxNodeAction(f => Analyze(f, (DoStatementSyntax)f.Node), SyntaxKind.DoStatement);
            context.RegisterSyntaxNodeAction(f => Analyze(f, (LockStatementSyntax)f.Node), SyntaxKind.LockStatement);
            context.RegisterSyntaxNodeAction(f => Analyze(f, (FixedStatementSyntax)f.Node), SyntaxKind.FixedStatement);

            context.RegisterSyntaxNodeAction(f => Analyze(f, (AccessorListSyntax)f.Node), SyntaxKind.AccessorList);
        }
    }
}
