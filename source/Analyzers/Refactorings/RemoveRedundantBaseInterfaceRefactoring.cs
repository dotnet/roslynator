// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Roslynator.CSharp.Refactorings
{
    internal static class RemoveRedundantBaseInterfaceRefactoring
    {
        public static void AnalyzeBaseList(SyntaxNodeAnalysisContext context)
        {
            var baseList = (BaseListSyntax)context.Node;

            SyntaxNode parent = baseList.Parent;

            if (parent?.IsKind(SyntaxKind.ClassDeclaration, SyntaxKind.StructDeclaration, SyntaxKind.InterfaceDeclaration) == true
                && !baseList.SpanContainsDirectives())
            {
                SeparatedSyntaxList<BaseTypeSyntax> baseTypes = baseList.Types;

                if (baseTypes.Count > 1)
                {
                    var baseClassInfo = default(SymbolInterfaceInfo);
                    List<SymbolInterfaceInfo> baseInterfaceInfos = null;

                    foreach (BaseTypeSyntax baseType in baseTypes)
                    {
                        TypeSyntax type = baseType.Type;

                        if (type?.IsMissing == false)
                        {
                            var baseSymbol = context.SemanticModel.GetSymbol(type, context.CancellationToken) as INamedTypeSymbol;

                            if (baseSymbol != null)
                            {
                                TypeKind typeKind = baseSymbol.TypeKind;

                                ImmutableArray<INamedTypeSymbol> allInterfaces = baseSymbol.AllInterfaces;

                                if (typeKind == TypeKind.Class)
                                {
                                    if (allInterfaces.Any())
                                    {
                                        baseClassInfo = new SymbolInterfaceInfo(baseType, baseSymbol, allInterfaces);

                                        if (baseInterfaceInfos != null)
                                        {
                                            foreach (SymbolInterfaceInfo baseInterfaceInfo in baseInterfaceInfos)
                                                Analyze(context, baseInterfaceInfo, baseClassInfo);
                                        }
                                    }
                                }
                                else if (typeKind == TypeKind.Interface)
                                {
                                    var baseInterfaceInfo = new SymbolInterfaceInfo(baseType, baseSymbol, allInterfaces);

                                    if (baseInterfaceInfos == null)
                                    {
                                        if (allInterfaces.Any())
                                            baseInterfaceInfos = new List<SymbolInterfaceInfo>() { baseInterfaceInfo };
                                    }
                                    else
                                    {
                                        foreach (SymbolInterfaceInfo baseInterfaceInfo2 in baseInterfaceInfos)
                                            Analyze(context, baseInterfaceInfo, baseInterfaceInfo2, checkMutually: true);
                                    }

                                    if (baseClassInfo.IsValid)
                                        Analyze(context, baseInterfaceInfo, baseClassInfo);
                                }
                            }
                        }
                    }
                }
            }
        }

        private static void Analyze(
            SyntaxNodeAnalysisContext context,
            SymbolInterfaceInfo interfaceInfo,
            SymbolInterfaceInfo interfaceInfo2,
            bool checkMutually = false)
        {
            foreach (INamedTypeSymbol interfaceSymbol in interfaceInfo2.Interfaces)
            {
                if (interfaceInfo.Symbol.Equals(interfaceSymbol))
                {
                    BaseTypeSyntax baseType = interfaceInfo.BaseType;

                    context.ReportDiagnostic(
                        DiagnosticDescriptors.RemoveRedundantBaseInterface,
                        baseType,
                        SymbolDisplay.GetMinimalString(interfaceInfo.Symbol, context.SemanticModel, baseType.SpanStart),
                        SymbolDisplay.GetMinimalString(interfaceInfo2.Symbol, context.SemanticModel, baseType.SpanStart));

                    return;
                }
            }

            if (checkMutually)
                Analyze(context, interfaceInfo2, interfaceInfo);
        }

        public static Task<Document> RefactorAsync(
            Document document,
            BaseTypeSyntax baseType,
            CancellationToken cancellationToken)
        {
            SyntaxRemoveOptions removeOptions = RemoveHelper.DefaultRemoveOptions;

            if (baseType.GetLeadingTrivia().All(f => f.IsWhitespaceTrivia()))
                removeOptions &= ~SyntaxRemoveOptions.KeepLeadingTrivia;

            if (baseType.GetTrailingTrivia().All(f => f.IsWhitespaceTrivia()))
                removeOptions &= ~SyntaxRemoveOptions.KeepTrailingTrivia;

            return document.RemoveNodeAsync(baseType, removeOptions, cancellationToken);
        }

        private struct SymbolInterfaceInfo
        {
            public SymbolInterfaceInfo(BaseTypeSyntax baseType, INamedTypeSymbol symbol, ImmutableArray<INamedTypeSymbol> interfaces)
            {
                BaseType = baseType;
                Symbol = symbol;
                Interfaces = interfaces;
            }

            public bool IsValid
            {
                get { return BaseType != null; }
            }

            public BaseTypeSyntax BaseType { get; }
            public INamedTypeSymbol Symbol { get; }
            public ImmutableArray<INamedTypeSymbol> Interfaces { get; }
        }
    }
}
