// Copyright (c) .NET Foundation and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Immutable;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Roslynator.CSharp.Analysis;

namespace Roslynator.CSharp.CSharp.Analysis;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class UseTimeProviderInsteadOfDateTimeNowAnalyzer : BaseDiagnosticAnalyzer
{
    internal const string TimeProviderMetadataName = "System.TimeProvider";

    private static ImmutableArray<DiagnosticDescriptor> _supportedDiagnostics;

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
    {
        get
        {
            if (_supportedDiagnostics.IsDefault)
            {
                Immutable.InterlockedInitialize(ref _supportedDiagnostics, DiagnosticRules.UseTimeProviderInsteadOfDateTimeNow);
            }

            return _supportedDiagnostics;
        }
    }

    public override void Initialize(AnalysisContext context)
    {
        base.Initialize(context);

        context.RegisterCompilationStartAction(startContext =>
        {
            INamedTypeSymbol timeProviderSymbol = startContext.Compilation.GetTypeByMetadataName(TimeProviderMetadataName);

            if (timeProviderSymbol is null)
                return;

            startContext.RegisterSyntaxNodeAction(
                f => AnalyzeSimpleMemberAccessExpression(f, timeProviderSymbol),
                SyntaxKind.SimpleMemberAccessExpression);
        });
    }

    private static void AnalyzeSimpleMemberAccessExpression(SyntaxNodeAnalysisContext context, INamedTypeSymbol timeProviderSymbol)
    {
        var memberAccess = (MemberAccessExpressionSyntax)context.Node;

        switch (memberAccess.Name.Identifier.ValueText)
        {
            case "Now":
            case "UtcNow":
            case "Today":
                break;
            default:
                return;
        }

        if (GetCurrentTimeProperty(memberAccess, context.SemanticModel, context.CancellationToken) == CurrentTimeProperty.None)
            return;

        if (IsInsideTimeProvider(context.ContainingSymbol, timeProviderSymbol))
            return;

        if (IsNameOfArgument(memberAccess, context.SemanticModel, context.CancellationToken))
            return;

        if (memberAccess.IsInExpressionTree(context.SemanticModel, context.CancellationToken))
            return;

        DiagnosticHelpers.ReportDiagnostic(
            context,
            DiagnosticRules.UseTimeProviderInsteadOfDateTimeNow,
            memberAccess,
            memberAccess.ToString());
    }

    internal static CurrentTimeProperty GetCurrentTimeProperty(
        MemberAccessExpressionSyntax memberAccess,
        SemanticModel semanticModel,
        CancellationToken cancellationToken)
    {
        if (semanticModel.GetSymbol(memberAccess, cancellationToken) is not IPropertySymbol { IsStatic: true } propertySymbol)
            return CurrentTimeProperty.None;

        INamedTypeSymbol containingType = propertySymbol.ContainingType;

        if (containingType.SpecialType == SpecialType.System_DateTime)
        {
            switch (propertySymbol.Name)
            {
                case "Now":
                    return CurrentTimeProperty.DateTimeNow;
                case "UtcNow":
                    return CurrentTimeProperty.DateTimeUtcNow;
                case "Today":
                    return CurrentTimeProperty.DateTimeToday;
            }
        }
        else if (SymbolEqualityComparer.Default.Equals(containingType, semanticModel.Compilation.GetTypeByMetadataName("System.DateTimeOffset")))
        {
            switch (propertySymbol.Name)
            {
                case "Now":
                    return CurrentTimeProperty.DateTimeOffsetNow;
                case "UtcNow":
                    return CurrentTimeProperty.DateTimeOffsetUtcNow;
            }
        }

        return CurrentTimeProperty.None;
    }

    private static bool IsInsideTimeProvider(ISymbol containingSymbol, INamedTypeSymbol timeProviderSymbol)
    {
        for (ISymbol symbol = containingSymbol; symbol is not null; symbol = symbol.ContainingSymbol)
        {
            if (symbol is INamedTypeSymbol typeSymbol
                && typeSymbol.EqualsOrInheritsFrom(timeProviderSymbol))
            {
                return true;
            }
        }

        return false;
    }

    private static bool IsNameOfArgument(
        MemberAccessExpressionSyntax memberAccess,
        SemanticModel semanticModel,
        CancellationToken cancellationToken)
    {
        return memberAccess.WalkUpParentheses().Parent is ArgumentSyntax { Parent: ArgumentListSyntax { Parent: InvocationExpressionSyntax invocation } }
            && CSharpUtility.IsNameOfExpression(invocation, semanticModel, cancellationToken);
    }

    internal enum CurrentTimeProperty
    {
        None,
        DateTimeNow,
        DateTimeUtcNow,
        DateTimeToday,
        DateTimeOffsetNow,
        DateTimeOffsetUtcNow,
    }
}
