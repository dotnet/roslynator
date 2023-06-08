// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Roslynator.CSharp.Analysis;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class UnnecessaryUnsafeContextAnalyzer : BaseDiagnosticAnalyzer
{
    private static ImmutableArray<DiagnosticDescriptor> _supportedDiagnostics;

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
    {
        get
        {
            if (_supportedDiagnostics.IsDefault)
                Immutable.InterlockedInitialize(ref _supportedDiagnostics, DiagnosticRules.UnnecessaryUnsafeContext);

            return _supportedDiagnostics;
        }
    }

    public override void Initialize(AnalysisContext context)
    {
        base.Initialize(context);

        context.RegisterSyntaxNodeAction(
            f => AnalyzeTypeDeclaration(f),
            SyntaxKind.ClassDeclaration,
            SyntaxKind.StructDeclaration,
            SyntaxKind.RecordStructDeclaration,
            SyntaxKind.InterfaceDeclaration);

        context.RegisterSyntaxNodeAction(f => AnalyzeUnsafeStatement(f), SyntaxKind.UnsafeStatement);
        context.RegisterSyntaxNodeAction(f => AnalyzeDelegateDeclaration(f), SyntaxKind.DelegateDeclaration);
        context.RegisterSyntaxNodeAction(f => AnalyzeLocalFunctionStatement(f), SyntaxKind.LocalFunctionStatement);
        context.RegisterSyntaxNodeAction(f => AnalyzeMethodDeclaration(f), SyntaxKind.MethodDeclaration);
        context.RegisterSyntaxNodeAction(f => AnalyzeOperatorDeclaration(f), SyntaxKind.OperatorDeclaration);
        context.RegisterSyntaxNodeAction(f => AnalyzeConversionOperatorDeclaration(f), SyntaxKind.ConversionOperatorDeclaration);
        context.RegisterSyntaxNodeAction(f => AnalyzeConstructorDeclaration(f), SyntaxKind.ConstructorDeclaration);
        context.RegisterSyntaxNodeAction(f => AnalyzeDestructorDeclaration(f), SyntaxKind.DestructorDeclaration);
        context.RegisterSyntaxNodeAction(f => AnalyzePropertyDeclaration(f), SyntaxKind.PropertyDeclaration);
        context.RegisterSyntaxNodeAction(f => AnalyzeIndexerDeclaration(f), SyntaxKind.IndexerDeclaration);
        context.RegisterSyntaxNodeAction(f => AnalyzeEventDeclaration(f), SyntaxKind.EventDeclaration);
        context.RegisterSyntaxNodeAction(f => AnalyzeEventFieldDeclaration(f), SyntaxKind.EventFieldDeclaration);
        context.RegisterSyntaxNodeAction(f => AnalyzeFieldDeclaration(f), SyntaxKind.FieldDeclaration);
    }

    private static void AnalyzeUnsafeStatement(SyntaxNodeAnalysisContext context)
    {
        var unsafeStatement = (UnsafeStatementSyntax)context.Node;

        if (!unsafeStatement.Block.Statements.Any())
            return;

        if (!AncestorContainsUnsafeModifier(unsafeStatement.Parent))
            return;

        DiagnosticHelpers.ReportDiagnostic(context, DiagnosticRules.UnnecessaryUnsafeContext, unsafeStatement.UnsafeKeyword);
    }

    private static void AnalyzeLocalFunctionStatement(SyntaxNodeAnalysisContext context)
    {
        var localFunctionStatement = (LocalFunctionStatementSyntax)context.Node;

        AnalyzeMemberDeclaration(context, localFunctionStatement, localFunctionStatement.Modifiers);
    }

    private static void AnalyzeTypeDeclaration(SyntaxNodeAnalysisContext context)
    {
        var typeDeclaration = (TypeDeclarationSyntax)context.Node;

        AnalyzeMemberDeclaration(context, typeDeclaration, typeDeclaration.Modifiers);
    }

    private static void AnalyzeDelegateDeclaration(SyntaxNodeAnalysisContext context)
    {
        var delegateDeclaration = (DelegateDeclarationSyntax)context.Node;

        AnalyzeMemberDeclaration(context, delegateDeclaration, delegateDeclaration.Modifiers);
    }

    private static void AnalyzeMethodDeclaration(SyntaxNodeAnalysisContext context)
    {
        var methodDeclaration = (MethodDeclarationSyntax)context.Node;

        AnalyzeMemberDeclaration(context, methodDeclaration, methodDeclaration.Modifiers);
    }

    private static void AnalyzeOperatorDeclaration(SyntaxNodeAnalysisContext context)
    {
        var operatorDeclaration = (OperatorDeclarationSyntax)context.Node;

        AnalyzeMemberDeclaration(context, operatorDeclaration, operatorDeclaration.Modifiers);
    }

    private static void AnalyzeConversionOperatorDeclaration(SyntaxNodeAnalysisContext context)
    {
        var conversionOperatorDeclaration = (ConversionOperatorDeclarationSyntax)context.Node;

        AnalyzeMemberDeclaration(context, conversionOperatorDeclaration, conversionOperatorDeclaration.Modifiers);
    }

    private static void AnalyzeConstructorDeclaration(SyntaxNodeAnalysisContext context)
    {
        var constructorDeclaration = (ConstructorDeclarationSyntax)context.Node;

        AnalyzeMemberDeclaration(context, constructorDeclaration, constructorDeclaration.Modifiers);
    }

    private static void AnalyzeDestructorDeclaration(SyntaxNodeAnalysisContext context)
    {
        var destructorDeclaration = (DestructorDeclarationSyntax)context.Node;

        AnalyzeMemberDeclaration(context, destructorDeclaration, destructorDeclaration.Modifiers);
    }

    private static void AnalyzeEventDeclaration(SyntaxNodeAnalysisContext context)
    {
        var eventDeclaration = (EventDeclarationSyntax)context.Node;

        AnalyzeMemberDeclaration(context, eventDeclaration, eventDeclaration.Modifiers);
    }

    private static void AnalyzeEventFieldDeclaration(SyntaxNodeAnalysisContext context)
    {
        var eventFieldDeclaration = (EventFieldDeclarationSyntax)context.Node;

        AnalyzeMemberDeclaration(context, eventFieldDeclaration, eventFieldDeclaration.Modifiers);
    }

    private static void AnalyzeFieldDeclaration(SyntaxNodeAnalysisContext context)
    {
        var fieldDeclaration = (FieldDeclarationSyntax)context.Node;

        AnalyzeMemberDeclaration(context, fieldDeclaration, fieldDeclaration.Modifiers);
    }

    private static void AnalyzePropertyDeclaration(SyntaxNodeAnalysisContext context)
    {
        var propertyDeclaration = (PropertyDeclarationSyntax)context.Node;

        AnalyzeMemberDeclaration(context, propertyDeclaration, propertyDeclaration.Modifiers);
    }

    private static void AnalyzeIndexerDeclaration(SyntaxNodeAnalysisContext context)
    {
        var indexerDeclaration = (IndexerDeclarationSyntax)context.Node;

        AnalyzeMemberDeclaration(context, indexerDeclaration, indexerDeclaration.Modifiers);
    }

    private static void AnalyzeMemberDeclaration(
        SyntaxNodeAnalysisContext context,
        SyntaxNode node,
        SyntaxTokenList modifiers)
    {
        int index = modifiers.IndexOf(SyntaxKind.UnsafeKeyword);

        if (index == -1)
            return;

        if (!AncestorContainsUnsafeModifier(node.Parent))
            return;

        DiagnosticHelpers.ReportDiagnostic(context, DiagnosticRules.UnnecessaryUnsafeContext, modifiers[index]);
    }

    private static bool AncestorContainsUnsafeModifier(SyntaxNode node)
    {
        while (node is not null)
        {
            switch (node)
            {
                case UnsafeStatementSyntax:
                    return true;
                case MemberDeclarationSyntax memberDeclarationSyntax:
                    {
                        if (memberDeclarationSyntax.Modifiers.Contains(SyntaxKind.UnsafeKeyword))
                            return true;
                        break;
                    }
            }

            node = node.Parent;
        }
        return false;
    }
}
