// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Roslynator.CSharp.Refactorings
{
    internal static class RemoveEnumDefaultUnderlyingTypeRefactoring
    {
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
