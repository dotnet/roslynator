// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.CSharp.Comparers;

namespace Roslynator.CSharp.Refactorings
{
    internal static class SortEnumMembersRefactoring
    {
        public static async Task<Document> RefactorAsync(
            Document document,
            EnumDeclarationSyntax enumDeclaration,
            CancellationToken cancellationToken)
        {
            SemanticModel semanticModel = await document.GetSemanticModelAsync(cancellationToken).ConfigureAwait(false);

            SpecialType enumSpecialType = semanticModel.GetDeclaredSymbol(enumDeclaration).EnumUnderlyingType.SpecialType;

            SeparatedSyntaxList<EnumMemberDeclarationSyntax> newMembers = enumDeclaration.Members
                .OrderBy(f => GetConstantValue(f, semanticModel, cancellationToken), EnumValueComparer.GetInstance(enumSpecialType))
                .ToSeparatedSyntaxList();

            MemberDeclarationSyntax newNode = enumDeclaration
                .WithMembers(newMembers)
                .WithFormatterAnnotation();

            return await document.ReplaceNodeAsync(enumDeclaration, newNode, cancellationToken).ConfigureAwait(false);
        }

        private static object GetConstantValue(
            EnumMemberDeclarationSyntax enumMemberDeclaration,
            SemanticModel semanticModel,
            CancellationToken cancellationToken)
        {
            return semanticModel.GetDeclaredSymbol(enumMemberDeclaration, cancellationToken)?.ConstantValue;
        }
    }
}
