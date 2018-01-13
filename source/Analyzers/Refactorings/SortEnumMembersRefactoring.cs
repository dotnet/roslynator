// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Roslynator.CSharp.Comparers;

namespace Roslynator.CSharp.Refactorings
{
    internal static class SortEnumMembersRefactoring
    {
        public static void AnalyzeEnumDeclaration(SyntaxNodeAnalysisContext context)
        {
            var enumDeclaration = (EnumDeclarationSyntax)context.Node;

            if (IsListUnsorted(enumDeclaration.Members, context.SemanticModel, context.CancellationToken)
                && !enumDeclaration.ContainsDirectives(enumDeclaration.BracesSpan()))
            {
                SyntaxToken identifier = enumDeclaration.Identifier;
                context.ReportDiagnostic(DiagnosticDescriptors.SortEnumMembers, identifier, identifier);
            }
        }

        private static bool IsListUnsorted(
            SeparatedSyntaxList<EnumMemberDeclarationSyntax> members,
            SemanticModel semanticModel,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            int count = members.Count;

            if (count > 1)
            {
                IFieldSymbol firstField = semanticModel.GetDeclaredSymbol(members.First(), cancellationToken);

                if (firstField?.HasConstantValue == true)
                {
                    object previousValue = firstField.ConstantValue;

                    for (int i = 1; i < count - 1; i++)
                    {
                        IFieldSymbol field = semanticModel.GetDeclaredSymbol(members[i], cancellationToken);

                        if (field?.HasConstantValue != true)
                            return false;

                        object value = field.ConstantValue;

                        if (EnumMemberValueComparer.Instance.Compare(previousValue, value) > 0)
                        {
                            i++;

                            while (i < count)
                            {
                                if (semanticModel.GetDeclaredSymbol(members[i], cancellationToken)?.HasConstantValue != true)
                                    return false;

                                i++;
                            }

                            return true;
                        }

                        previousValue = value;
                    }
                }
            }

            return false;
        }

        public static async Task<Document> RefactorAsync(
            Document document,
            EnumDeclarationSyntax enumDeclaration,
            CancellationToken cancellationToken)
        {
            SemanticModel semanticModel = await document.GetSemanticModelAsync(cancellationToken).ConfigureAwait(false);

            SeparatedSyntaxList<EnumMemberDeclarationSyntax> newMembers = enumDeclaration.Members
                .OrderBy(f => GetConstantValue(f, semanticModel, cancellationToken), EnumMemberValueComparer.Instance)
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
