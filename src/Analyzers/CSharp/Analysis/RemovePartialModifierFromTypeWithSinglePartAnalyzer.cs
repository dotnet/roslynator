﻿// Copyright (c) .NET Foundation and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Roslynator.CSharp.Analysis;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class RemovePartialModifierFromTypeWithSinglePartAnalyzer : BaseDiagnosticAnalyzer
{
    private static ImmutableArray<DiagnosticDescriptor> _supportedDiagnostics;

    private static readonly MetadataName[] _metadataNames = [
        // ASP.NET Core
        MetadataName.Parse("Microsoft.AspNetCore.Components.ComponentBase"),
        // WPF
        MetadataName.Parse("System.Windows.FrameworkElement"),
    ];

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
    {
        get
        {
            if (_supportedDiagnostics.IsDefault)
                Immutable.InterlockedInitialize(ref _supportedDiagnostics, DiagnosticRules.RemovePartialModifierFromTypeWithSinglePart);

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
            SyntaxKind.RecordDeclaration,
            SyntaxKind.InterfaceDeclaration);
    }

    private static void AnalyzeTypeDeclaration(SyntaxNodeAnalysisContext context)
    {
        var typeDeclaration = (TypeDeclarationSyntax)context.Node;

        if (!typeDeclaration.Modifiers.Contains(SyntaxKind.PartialKeyword))
            return;

        INamedTypeSymbol symbol = context.SemanticModel.GetDeclaredSymbol(typeDeclaration, context.CancellationToken);

        if (symbol?.DeclaringSyntaxReferences.SingleOrDefault(shouldThrow: false) is null)
            return;

        if (_metadataNames.Any(c => symbol.InheritsFrom(c)))
            return;

        foreach (ISymbol member in symbol.GetMembers())
        {
            if (member.HasAttribute(MetadataNames.System_Text_RegularExpressions_GeneratedRegexAttribute))
                return;
        }

        foreach (MemberDeclarationSyntax member in typeDeclaration.Members)
        {
            if (member is MethodDeclarationSyntax methodDeclaration
                && methodDeclaration.Modifiers.Contains(SyntaxKind.PartialKeyword)
                && methodDeclaration.BodyOrExpressionBody() is null
                && methodDeclaration.ContainsUnbalancedIfElseDirectives(methodDeclaration.Span))
            {
                return;
            }
        }

        DiagnosticHelpers.ReportDiagnostic(
            context,
            DiagnosticRules.RemovePartialModifierFromTypeWithSinglePart,
            typeDeclaration.Modifiers.Find(SyntaxKind.PartialKeyword));
    }
}
