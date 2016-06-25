// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Pihrtsoft.CodeAnalysis.CSharp.Refactoring
{
    internal static class FieldDeclarationRefactoring
    {
        public static async Task ComputeRefactoringsAsync(RefactoringContext context, FieldDeclarationSyntax node)
        {
            if (node.Modifiers.Contains(SyntaxKind.ConstKeyword))
            {
                context.RegisterRefactoring(
                    "Convert to read-only field",
                    cancellationToken => ConvertConstantToFieldRefactoring.RefactorAsync(context.Document, node, cancellationToken));
            }
            else if (node.Modifiers.Contains(SyntaxKind.ReadOnlyKeyword)
                && node.Modifiers.Contains(SyntaxKind.StaticKeyword)
                && context.SupportsSemanticModel)
            {
                if (await ConvertFieldToConstantRefactoring.CanRefactorAsync(context, node))
                {
                    context.RegisterRefactoring(
                        "Convert to constant",
                        cancellationToken => ConvertFieldToConstantRefactoring.RefactorAsync(context.Document, node, cancellationToken));
                }
            }

            if (MarkMemberAsStaticRefactoring.CanRefactor(node))
            {
                context.RegisterRefactoring(
                    "Mark field as static",
                    cancellationToken => MarkMemberAsStaticRefactoring.RefactorAsync(context.Document, node, cancellationToken));

                MarkAllMembersAsStaticRefactoring.RegisterRefactoring(context, (ClassDeclarationSyntax)node.Parent);
            }
        }
    }
}
