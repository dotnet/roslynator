// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Roslynator.CSharp.Refactorings
{
    internal static class GenerateEnumMemberRefactoring
    {
        public static async Task ComputeRefactoringAsync(RefactoringContext context, EnumDeclarationSyntax enumDeclaration)
        {
            SemanticModel semanticModel = await context.GetSemanticModelAsync().ConfigureAwait(false);

            INamedTypeSymbol enumSymbol = semanticModel.GetDeclaredSymbol(enumDeclaration, context.CancellationToken);

            if (enumSymbol.IsEnumWithFlags(semanticModel))
            {
                List<object> values = GetConstantValues(enumSymbol);

                SpecialType specialType = enumSymbol.EnumUnderlyingType.SpecialType;

                Optional<object> optional = FlagsUtility.GetUniquePowerOfTwo(specialType, values);

                if (optional.HasValue)
                {
                    context.RegisterRefactoring(
                        "Generate enum member",
                        cancellationToken => RefactorAsync(context.Document, enumDeclaration, enumSymbol, optional.Value, cancellationToken));

                    Optional<object> optional2 = FlagsUtility.GetUniquePowerOfTwo(specialType, values, startFromHighestExistingValue: true);

                    if (optional2.HasValue
                        && !optional.Value.Equals(optional2.Value))
                    {
                        context.RegisterRefactoring(
                            $"Generate enum member (with value {optional2.Value})",
                            cancellationToken => RefactorAsync(context.Document, enumDeclaration, enumSymbol, optional2.Value, cancellationToken));
                    }
                }
            }
            else
            {
                context.RegisterRefactoring(
                    "Generate enum member",
                    cancellationToken => RefactorAsync(context.Document, enumDeclaration, enumSymbol, null, cancellationToken));
            }
        }

        private static List<object> GetConstantValues(ITypeSymbol enumSymbol)
        {
            var values = new List<object>();

            foreach (ISymbol member in enumSymbol.GetMembers())
            {
                if (member.Kind == SymbolKind.Field)
                {
                    var fieldSymbol = (IFieldSymbol)member;

                    if (fieldSymbol.HasConstantValue)
                        values.Add(fieldSymbol.ConstantValue);
                }
            }

            return values;
        }

        private static Task<Document> RefactorAsync(
            Document document,
            EnumDeclarationSyntax enumDeclaration,
            INamedTypeSymbol enumSymbol,
            object value,
            CancellationToken cancellationToken)
        {
            EnumMemberDeclarationSyntax newEnumMember = CreateEnumMember(enumSymbol, DefaultNames.EnumMember, value);

            EnumDeclarationSyntax newNode = enumDeclaration.AddMembers(newEnumMember);

            return document.ReplaceNodeAsync(enumDeclaration, newNode, cancellationToken);
        }

        private static EnumMemberDeclarationSyntax CreateEnumMember(INamedTypeSymbol enumSymbol, string name, object value)
        {
            EqualsValueClauseSyntax equalsValue = null;

            if (value != null)
                equalsValue = EqualsValueClause(CSharpFactory.LiteralExpression(value));

            name = NameGenerator.Default.EnsureUniqueMemberName(name, enumSymbol);

            SyntaxToken identifier = Identifier(name).WithRenameAnnotation();

            return EnumMemberDeclaration(
                default(SyntaxList<AttributeListSyntax>),
                identifier,
                equalsValue);
        }
    }
}