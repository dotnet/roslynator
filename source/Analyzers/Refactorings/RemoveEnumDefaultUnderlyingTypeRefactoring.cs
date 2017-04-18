// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Roslynator.CSharp.Refactorings
{
    internal static class RemoveEnumDefaultUnderlyingTypeRefactoring
    {
        public static void Analyze(SyntaxNodeAnalysisContext context, EnumDeclarationSyntax enumDeclaration)
        {
            BaseTypeSyntax baseType = GetRedundantBaseType(context, enumDeclaration);

            if (baseType != null)
                context.ReportDiagnostic(DiagnosticDescriptors.RemoveEnumDefaultUnderlyingType, baseType);
        }

        private static BaseTypeSyntax GetRedundantBaseType(SyntaxNodeAnalysisContext context, EnumDeclarationSyntax enumDeclaration)
        {
            if (enumDeclaration.BaseList != null)
            {
                foreach (BaseTypeSyntax baseType in enumDeclaration.BaseList.Types)
                {
                    if (baseType.IsKind(SyntaxKind.SimpleBaseType))
                    {
                        var simpleBaseType = (SimpleBaseTypeSyntax)baseType;

                        if (simpleBaseType.Type?.IsKind(SyntaxKind.PredefinedType) == true)
                        {
                            var symbol = context.SemanticModel.GetSymbol(simpleBaseType.Type, context.CancellationToken) as INamedTypeSymbol;

                            if (symbol?.IsInt() == true)
                                return baseType;
                        }
                    }
                }
            }

            return null;
        }

        public static Task<Document> RefactorAsync(
            Document document,
            BaseTypeSyntax baseType,
            CancellationToken cancellationToken)
        {
            var baseList = (BaseListSyntax)baseType.Parent;
            var enumDeclaration = (EnumDeclarationSyntax)baseList.Parent;

            EnumDeclarationSyntax newEnumDeclaration = enumDeclaration
                .RemoveNode(GetNodeToRemove(baseType, baseList), SyntaxRemoveOptions.KeepExteriorTrivia)
                .WithFormatterAnnotation();

            return document.ReplaceNodeAsync(enumDeclaration, newEnumDeclaration, cancellationToken);
        }

        private static SyntaxNode GetNodeToRemove(BaseTypeSyntax baseType, BaseListSyntax baseList)
        {
            if (baseList.Types.Count == 1)
                return baseList;

            return baseType;
        }
    }
}
