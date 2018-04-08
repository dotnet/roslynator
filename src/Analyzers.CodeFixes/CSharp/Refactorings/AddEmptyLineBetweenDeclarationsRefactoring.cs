// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Roslynator.CSharp.Refactorings
{
    internal static class AddEmptyLineBetweenDeclarationsRefactoring
    {
        public static Task<Document> RefactorAsync(
            Document document,
            MemberDeclarationSyntax memberDeclaration,
            CancellationToken cancellationToken)
        {
            MemberDeclarationSyntax newNode = memberDeclaration
                .WithTrailingTrivia(memberDeclaration.GetTrailingTrivia().Add(CSharpFactory.NewLine()));

            return document.ReplaceNodeAsync(memberDeclaration, newNode, cancellationToken);
        }
    }
}
