// Copyright (c) .NET Foundation and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Collections.Immutable;
using System.Composition;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.CodeFixes;
using static Roslynator.CSharp.CSharp.Analysis.UseTimeProviderInsteadOfDateTimeNowAnalyzer;

namespace Roslynator.CSharp.CSharp.CodeFixes;

[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(UseTimeProviderInsteadOfDateTimeNowCodeFixProvider))]
[Shared]
public sealed class UseTimeProviderInsteadOfDateTimeNowCodeFixProvider : BaseCodeFixProvider
{
    public override ImmutableArray<string> FixableDiagnosticIds
        => ImmutableArray.Create(DiagnosticIdentifiers.UseTimeProviderInsteadOfDateTimeNow);

    public override async Task RegisterCodeFixesAsync(CodeFixContext context)
    {
        SyntaxNode root = await context.GetSyntaxRootAsync().ConfigureAwait(false);

        if (!TryFindFirstAncestorOrSelf(root, context.Span, out MemberAccessExpressionSyntax memberAccess))
            return;

        Document document = context.Document;
        SemanticModel semanticModel = await document.GetSemanticModelAsync(context.CancellationToken).ConfigureAwait(false);

        CurrentTimeProperty property = GetCurrentTimeProperty(memberAccess, semanticModel, context.CancellationToken);

        if (property == CurrentTimeProperty.None)
            return;

        INamedTypeSymbol timeProviderSymbol = semanticModel.Compilation.GetTypeByMetadataName(TimeProviderMetadataName);

        if (timeProviderSymbol is null)
            return;

        ISymbol timeProvider = FindTimeProviderInScope(memberAccess, semanticModel, timeProviderSymbol, context.CancellationToken);

        if (timeProvider is null)
            return;

        Diagnostic diagnostic = context.Diagnostics[0];

        CodeAction codeAction = CodeAction.Create(
            $"Use '{timeProvider.Name}'",
            ct => RefactorAsync(document, memberAccess, timeProvider, property, ct),
            GetEquivalenceKey(diagnostic));

        context.RegisterCodeFix(codeAction, diagnostic);
    }

    private static Task<Document> RefactorAsync(
        Document document,
        MemberAccessExpressionSyntax memberAccess,
        ISymbol timeProvider,
        CurrentTimeProperty property,
        CancellationToken cancellationToken)
    {
        string name = timeProvider.Name;

        if (SyntaxFacts.GetKeywordKind(name) != SyntaxKind.None)
            name = "@" + name;

        string text = property switch
        {
            CurrentTimeProperty.DateTimeNow => $"{name}.GetLocalNow().DateTime",
            CurrentTimeProperty.DateTimeUtcNow => $"{name}.GetUtcNow().UtcDateTime",
            CurrentTimeProperty.DateTimeToday => $"{name}.GetLocalNow().Date",
            CurrentTimeProperty.DateTimeOffsetNow => $"{name}.GetLocalNow()",
            CurrentTimeProperty.DateTimeOffsetUtcNow => $"{name}.GetUtcNow()",
            _ => throw new System.InvalidOperationException(),
        };

        ExpressionSyntax newExpression = SyntaxFactory.ParseExpression(text)
            .WithTriviaFrom(memberAccess);

        return document.ReplaceNodeAsync(memberAccess, newExpression, cancellationToken);
    }

    private static ISymbol FindTimeProviderInScope(
        ExpressionSyntax expression,
        SemanticModel semanticModel,
        INamedTypeSymbol timeProviderSymbol,
        CancellationToken cancellationToken)
    {
        int position = expression.SpanStart;

        AccessContext accessContext = GetAccessContext(expression, semanticModel, cancellationToken);

        ISymbol local = null;
        int localCount = 0;
        ISymbol member = null;
        int memberCount = 0;

        foreach (ISymbol symbol in semanticModel.LookupSymbols(position))
        {
            switch (symbol)
            {
                case ILocalSymbol localSymbol:
                {
                    if (IsTimeProvider(localSymbol.Type)
                        && IsDeclaredBefore(localSymbol, position, cancellationToken)
                        && accessContext.CanCapture(localSymbol.ContainingSymbol))
                    {
                        local = symbol;
                        localCount++;
                    }

                    break;
                }
                case IParameterSymbol parameterSymbol:
                {
                    if (!IsTimeProvider(parameterSymbol.Type))
                        break;

                    if (IsPrimaryConstructorParameter(parameterSymbol, cancellationToken))
                    {
                        if (accessContext.CanAccessPrimaryConstructorParameters)
                        {
                            member = symbol;
                            memberCount++;
                        }
                    }
                    else if (accessContext.CanCapture(parameterSymbol.ContainingSymbol))
                    {
                        local = symbol;
                        localCount++;
                    }

                    break;
                }
                case IFieldSymbol fieldSymbol:
                {
                    if (IsTimeProvider(fieldSymbol.Type)
                        && (fieldSymbol.IsStatic || accessContext.CanAccessInstanceMembers))
                    {
                        member = symbol;
                        memberCount++;
                    }

                    break;
                }
                case IPropertySymbol propertySymbol:
                {
                    if (!propertySymbol.IsIndexer
                        && propertySymbol.GetMethod is not null
                        && IsTimeProvider(propertySymbol.Type)
                        && (propertySymbol.IsStatic || accessContext.CanAccessInstanceMembers))
                    {
                        member = symbol;
                        memberCount++;
                    }

                    break;
                }
            }
        }

        ISymbol candidate;

        if (localCount == 1)
        {
            candidate = local;
        }
        else if (localCount == 0 && memberCount == 1)
        {
            candidate = member;
        }
        else
        {
            return null;
        }

        ImmutableArray<ISymbol> symbolsWithSameName = semanticModel.LookupSymbols(position, name: candidate.Name);

        if (symbolsWithSameName.Length != 1
            || !SymbolEqualityComparer.Default.Equals(symbolsWithSameName[0], candidate))
        {
            return null;
        }

        return candidate;

        bool IsTimeProvider(ITypeSymbol type)
        {
            return type?.EqualsOrInheritsFrom(timeProviderSymbol) == true;
        }
    }

    private static bool IsDeclaredBefore(ILocalSymbol localSymbol, int position, CancellationToken cancellationToken)
    {
        foreach (SyntaxReference reference in localSymbol.DeclaringSyntaxReferences)
        {
            if (reference.GetSyntax(cancellationToken).Span.End <= position)
                return true;
        }

        return false;
    }

    private static bool IsPrimaryConstructorParameter(IParameterSymbol parameterSymbol, CancellationToken cancellationToken)
    {
        if (parameterSymbol.ContainingSymbol is not IMethodSymbol { MethodKind: MethodKind.Constructor } constructorSymbol)
            return false;

        foreach (SyntaxReference reference in constructorSymbol.DeclaringSyntaxReferences)
        {
            if (reference.GetSyntax(cancellationToken) is TypeDeclarationSyntax)
                return true;
        }

        return false;
    }

    private static AccessContext GetAccessContext(ExpressionSyntax expression, SemanticModel semanticModel, CancellationToken cancellationToken)
    {
        ISymbol enclosingSymbol = semanticModel.GetEnclosingSymbol(expression.SpanStart, cancellationToken);
        ISymbol symbol = enclosingSymbol;

        while (symbol is IMethodSymbol { MethodKind: MethodKind.AnonymousFunction or MethodKind.LocalFunction } functionSymbol)
        {
            if (functionSymbol.IsStatic)
            {
                var capturableSymbols = new HashSet<ISymbol>(SymbolEqualityComparer.Default);

                for (ISymbol s = enclosingSymbol; !SymbolEqualityComparer.Default.Equals(s, functionSymbol.ContainingSymbol); s = s.ContainingSymbol)
                    capturableSymbols.Add(s);

                return new AccessContext(capturableSymbols, canAccessInstanceMembers: false, canAccessPrimaryConstructorParameters: false);
            }

            symbol = functionSymbol.ContainingSymbol;
        }

        if (symbol?.IsStatic != false)
            return new AccessContext(null, canAccessInstanceMembers: false, canAccessPrimaryConstructorParameters: false);

        bool isInConstructorInitializer = expression.FirstAncestor<ConstructorInitializerSyntax>() is not null;
        bool isInFieldOrPropertyInitializer = symbol is IFieldSymbol or IPropertySymbol;

        return new AccessContext(
            capturableSymbols: null,
            canAccessInstanceMembers: !isInConstructorInitializer && !isInFieldOrPropertyInitializer,
            canAccessPrimaryConstructorParameters: !isInConstructorInitializer);
    }

    private readonly struct AccessContext
    {
        private readonly HashSet<ISymbol> _capturableSymbols;

        public AccessContext(HashSet<ISymbol> capturableSymbols, bool canAccessInstanceMembers, bool canAccessPrimaryConstructorParameters)
        {
            _capturableSymbols = capturableSymbols;
            CanAccessInstanceMembers = canAccessInstanceMembers;
            CanAccessPrimaryConstructorParameters = canAccessPrimaryConstructorParameters;
        }

        public bool CanAccessInstanceMembers { get; }

        public bool CanAccessPrimaryConstructorParameters { get; }

        public bool CanCapture(ISymbol containingSymbol)
        {
            return _capturableSymbols?.Contains(containingSymbol) != false;
        }
    }
}
