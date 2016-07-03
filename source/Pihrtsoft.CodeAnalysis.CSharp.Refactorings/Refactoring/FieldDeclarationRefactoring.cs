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
                if (context.Settings.IsRefactoringEnabled(RefactoringIdentifiers.ReplaceConstantWithField))
                {
                    context.RegisterRefactoring(
                        "Replace constant with field",
                        cancellationToken => ReplaceConstantWithFieldRefactoring.RefactorAsync(context.Document, node, cancellationToken));
                }
            }
            else if (context.Settings.IsRefactoringEnabled(RefactoringIdentifiers.ReplaceFieldWithConstant)
                && node.Modifiers.Contains(SyntaxKind.ReadOnlyKeyword)
                && node.Modifiers.Contains(SyntaxKind.StaticKeyword)
                && context.SupportsSemanticModel)
            {
                if (await ReplaceFieldWithConstantRefactoring.CanRefactorAsync(context, node))
                {
                    context.RegisterRefactoring(
                        "Replace field with constant",
                        cancellationToken => ReplaceFieldWithConstantRefactoring.RefactorAsync(context.Document, node, cancellationToken));
                }
            }

            if (context.Settings.IsRefactoringEnabled(RefactoringIdentifiers.MarkMemberAsStatic)
                && MarkMemberAsStaticRefactoring.CanRefactor(node))
            {
                context.RegisterRefactoring(
                    "Mark field as static",
                    cancellationToken => MarkMemberAsStaticRefactoring.RefactorAsync(context.Document, node, cancellationToken));

                MarkAllMembersAsStaticRefactoring.RegisterRefactoring(context, (ClassDeclarationSyntax)node.Parent);
            }
        }
    }
}
