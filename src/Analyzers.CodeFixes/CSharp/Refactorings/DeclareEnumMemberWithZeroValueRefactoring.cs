// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static Roslynator.CSharp.CSharpFactory;

namespace Roslynator.CSharp.Refactorings
{
    internal static class DeclareEnumMemberWithZeroValueRefactoring
    {
        public static async Task<Document> RefactorAsync(
            Document document,
            EnumDeclarationSyntax enumDeclaration,
            CancellationToken cancellationToken)
        {
            SemanticModel semanticModel = await document.GetSemanticModelAsync(cancellationToken).ConfigureAwait(false);

            INamedTypeSymbol symbol = semanticModel.GetDeclaredSymbol(enumDeclaration, cancellationToken);

            string name = NameGenerator.Default.EnsureUniqueMemberName("None", symbol);

            EnumMemberDeclarationSyntax enumMember = EnumMemberDeclaration(
                Identifier(name).WithRenameAnnotation(),
                NumericLiteralExpression(0));

            enumMember = enumMember.WithTrailingTrivia(NewLine());

            EnumDeclarationSyntax newNode = enumDeclaration.WithMembers(enumDeclaration.Members.Insert(0, enumMember));

            return await document.ReplaceNodeAsync(enumDeclaration, newNode, cancellationToken).ConfigureAwait(false);
        }
    }
}
