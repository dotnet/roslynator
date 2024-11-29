// Copyright (c) .NET Foundation and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Immutable;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Host.Mef;
using Roslynator.FindSymbols;

namespace Roslynator.CSharp.FindSymbols;

[ExportLanguageService(typeof(IFindSymbolService), LanguageNames.CSharp)]
internal class CSharpFindSymbolService : FindSymbolService
{
    public override ISyntaxFactsService SyntaxFacts => CSharpSyntaxFactsService.Instance;

    public override SyntaxNode? FindDeclaration(SyntaxNode node)
    {
        return node.FirstAncestorOrSelf(
            n =>
            {
                switch (n.Kind())
                {
                    case SyntaxKind.ClassDeclaration:
                    case SyntaxKind.ConstructorDeclaration:
                    case SyntaxKind.DelegateDeclaration:
                    case SyntaxKind.EnumDeclaration:
                    case SyntaxKind.EnumMemberDeclaration:
                    case SyntaxKind.EventDeclaration:
                    case SyntaxKind.IndexerDeclaration:
                    case SyntaxKind.InterfaceDeclaration:
                    case SyntaxKind.MethodDeclaration:
                    case SyntaxKind.Parameter:
                    case SyntaxKind.PropertyDeclaration:
                    case SyntaxKind.RecordDeclaration:
                    case SyntaxKind.StructDeclaration:
#if ROSLYN_4_0
                    case SyntaxKind.RecordStructDeclaration:
#endif
                    case SyntaxKind.TypeParameter:
                    case SyntaxKind.UsingDirective:
                    case SyntaxKind.VariableDeclarator:
                    case SyntaxKind.SimpleLambdaExpression:
                    case SyntaxKind.ParenthesizedLambdaExpression:
                    case SyntaxKind.AnonymousMethodExpression:
                    case SyntaxKind.AnonymousObjectMemberDeclarator:
                    case SyntaxKind.LocalFunctionStatement:
                    case SyntaxKind.SingleVariableDesignation:
                    case SyntaxKind.CatchDeclaration:
                    case SyntaxKind.ForEachStatement:
                        return true;
                }

                return false;
            },
            ascendOutOfTrivia: false);
    }

    public override bool CanBeRenamed(SyntaxToken token)
    {
        switch (token.Kind())
        {
            case SyntaxKind.IdentifierToken:
                return true;
            case SyntaxKind.ThisKeyword:
            case SyntaxKind.BaseKeyword:
            case SyntaxKind.GetKeyword:
            case SyntaxKind.SetKeyword:
            case SyntaxKind.AddKeyword:
            case SyntaxKind.RemoveKeyword:
                return false;
        }

        SyntaxDebug.Fail(token);

        return false;
    }

    public override ImmutableArray<ISymbol> FindLocalSymbols(
        SyntaxNode node,
        SemanticModel semanticModel,
        CancellationToken cancellationToken = default)
    {
        return LocalSymbolFinder.FindLocalSymbols(node, semanticModel, cancellationToken);
    }
}
