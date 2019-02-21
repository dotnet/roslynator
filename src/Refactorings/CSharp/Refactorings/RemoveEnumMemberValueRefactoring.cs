// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace Roslynator.CSharp.Refactorings
{
    internal static class RemoveEnumMemberValueRefactoring
    {
        public static void ComputeRefactoring(
            RefactoringContext context,
            EnumDeclarationSyntax enumDeclaration,
            SeparatedSyntaxListSelection<EnumMemberDeclarationSyntax> selectedMembers)
        {
            int count = 0;

            for (int i = 0; i < selectedMembers.Count; i++)
            {
                if (selectedMembers[i].EqualsValue?.Value != null)
                {
                    count++;

                    if (count == 2)
                        break;
                }
            }

            if (count == 0)
                return;

            context.RegisterRefactoring(
                (count == 1) ? "Remove enum value" : "Remove enum values",
                ct => RefactorAsync(context.Document, enumDeclaration, selectedMembers, keepCompositeValue: false, ct),
                RefactoringIdentifiers.RemoveEnumMemberValue);
        }

        public static void ComputeRefactoring(
            RefactoringContext context,
            EnumDeclarationSyntax enumDeclaration)
        {
            int count = 0;

            SeparatedSyntaxList<EnumMemberDeclarationSyntax> members = enumDeclaration.Members;

            for (int i = 0; i < members.Count; i++)
            {
                if (members[i].EqualsValue?.Value != null)
                {
                    count++;

                    if (count == 2)
                        break;
                }
            }

            if (count == 0)
                return;

            context.RegisterRefactoring(
                (count == 1) ? "Remove explicit value" : "Remove explicit values",
                ct => RefactorAsync(context.Document, enumDeclaration, members, keepCompositeValue: true, ct),
                RefactoringIdentifiers.RemoveEnumMemberValue);
        }

        private static async Task<Document> RefactorAsync(
            Document document,
            EnumDeclarationSyntax enumDeclaration,
            IEnumerable<EnumMemberDeclarationSyntax> enumMembers,
            bool keepCompositeValue,
            CancellationToken cancellationToken)
        {
            SemanticModel semanticModel = null;

            if (keepCompositeValue)
            {
                semanticModel = await document.GetSemanticModelAsync(cancellationToken).ConfigureAwait(false);

                INamedTypeSymbol enumSymbol = semanticModel.GetDeclaredSymbol(enumDeclaration, cancellationToken);

                keepCompositeValue = enumSymbol.HasAttribute(MetadataNames.System_FlagsAttribute);
            }

            IEnumerable<TextChange> textChanges = enumMembers
                .Where(enumMember =>
                {
                    ExpressionSyntax expression = enumMember.EqualsValue?.Value;

                    if (expression == null)
                        return false;

                    if (keepCompositeValue
                        && !(expression is LiteralExpressionSyntax))
                    {
                        IFieldSymbol fieldSymbol = semanticModel.GetDeclaredSymbol(enumMember, cancellationToken);

                        if (!fieldSymbol.HasConstantValue)
                            return false;

                        ulong value = SymbolUtility.GetEnumValueAsUInt64(fieldSymbol.ConstantValue, fieldSymbol.ContainingType);

                        if (FlagsUtility<ulong>.Instance.IsComposite(value))
                            return false;
                    }

                    return true;
                })
                .Select(f => new TextChange(TextSpan.FromBounds(f.Identifier.Span.End, f.EqualsValue.Span.End), ""));

            return await document.WithTextChangesAsync(textChanges, cancellationToken).ConfigureAwait(false);
        }
    }
}