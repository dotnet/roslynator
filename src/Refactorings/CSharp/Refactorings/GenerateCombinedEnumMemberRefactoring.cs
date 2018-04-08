// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static Roslynator.CSharp.CSharpFactory;

namespace Roslynator.CSharp.Refactorings
{
    internal static class GenerateCombinedEnumMemberRefactoring
    {
        public static void ComputeRefactoring(
            RefactoringContext context,
            EnumDeclarationSyntax enumDeclaration,
            SeparatedSyntaxListSelection<EnumMemberDeclarationSyntax> selection,
            SemanticModel semanticModel)
        {
            INamedTypeSymbol enumSymbol = semanticModel.GetDeclaredSymbol(enumDeclaration, context.CancellationToken);

            if (enumSymbol?.IsEnumWithFlags(semanticModel) != true)
                return;

            object[] constantValues = selection
                .Select(f => semanticModel.GetDeclaredSymbol(f, context.CancellationToken))
                .Where(f => f.HasConstantValue)
                .Select(f => f.ConstantValue)
                .ToArray();

            object combinedValue = GetCombinedValue(constantValues, enumSymbol);

            if (combinedValue == null)
                return;

            if (IsValueDefined(enumSymbol, combinedValue))
                return;

            string name = NameGenerator.Default.EnsureUniqueMemberName(
                string.Concat(selection.Select(f => f.Identifier.ValueText)),
                enumSymbol);

            EnumMemberDeclarationSyntax newEnumMember = CreateEnumMember(name, selection.ToImmutableArray());

            int insertIndex = selection.LastIndex + 1;

            context.RegisterRefactoring(
                $"Generate enum member '{name}'",
                cancellationToken => RefactorAsync(context.Document, enumDeclaration, newEnumMember, insertIndex, cancellationToken));
        }

        private static object GetCombinedValue(IEnumerable<object> constantValues, INamedTypeSymbol enumSymbol)
        {
            switch (enumSymbol.EnumUnderlyingType.SpecialType)
            {
                case SpecialType.System_SByte:
                    {
                        sbyte[] values = constantValues.OfType<sbyte>().ToArray();

                        for (int i = 0; i < values.Length; i++)
                        {
                            for (int j = 0; j < values.Length; j++)
                            {
                                if (j != i
                                    && (values[i] & values[j]) != 0)
                                {
                                    return null;
                                }
                            }
                        }

                        return values.Aggregate((f, g) => (sbyte)(f + g));
                    }
                case SpecialType.System_Byte:
                    {
                        byte[] values = constantValues.OfType<byte>().ToArray();

                        for (int i = 0; i < values.Length; i++)
                        {
                            for (int j = 0; j < values.Length; j++)
                            {
                                if (j != i
                                    && (values[i] & values[j]) != 0)
                                {
                                    return null;
                                }
                            }
                        }

                        return constantValues.OfType<byte>().Aggregate((f, g) => (byte)(f + g));
                    }
                case SpecialType.System_Int16:
                    {
                        short[] values = constantValues.OfType<short>().ToArray();

                        for (int i = 0; i < values.Length; i++)
                        {
                            for (int j = 0; j < values.Length; j++)
                            {
                                if (j != i
                                    && (values[i] & values[j]) != 0)
                                {
                                    return null;
                                }
                            }
                        }

                        return constantValues.OfType<short>().Aggregate((f, g) => (short)(f + g));
                    }
                case SpecialType.System_UInt16:
                    {
                        ushort[] values = constantValues.OfType<ushort>().ToArray();

                        for (int i = 0; i < values.Length; i++)
                        {
                            for (int j = 0; j < values.Length; j++)
                            {
                                if (j != i
                                    && (values[i] & values[j]) != 0)
                                {
                                    return null;
                                }
                            }
                        }

                        return constantValues.OfType<ushort>().Aggregate((f, g) => (ushort)(f + g));
                    }
                case SpecialType.System_Int32:
                    {
                        int[] values = constantValues.OfType<int>().ToArray();

                        for (int i = 0; i < values.Length; i++)
                        {
                            for (int j = 0; j < values.Length; j++)
                            {
                                if (j != i
                                    && (values[i] & values[j]) != 0)
                                {
                                    return null;
                                }
                            }
                        }

                        return constantValues.OfType<int>().Aggregate((f, g) => f + g);
                    }
                case SpecialType.System_UInt32:
                    {
                        uint[] values = constantValues.OfType<uint>().ToArray();

                        for (int i = 0; i < values.Length; i++)
                        {
                            for (int j = 0; j < values.Length; j++)
                            {
                                if (j != i
                                    && (values[i] & values[j]) != 0)
                                {
                                    return null;
                                }
                            }
                        }

                        return constantValues.OfType<uint>().Aggregate((f, g) => f + g);
                    }
                case SpecialType.System_Int64:
                    {
                        long[] values = constantValues.OfType<long>().ToArray();

                        for (int i = 0; i < values.Length; i++)
                        {
                            for (int j = 0; j < values.Length; j++)
                            {
                                if (j != i
                                    && (values[i] & values[j]) != 0)
                                {
                                    return null;
                                }
                            }
                        }

                        return constantValues.OfType<long>().Aggregate((f, g) => f + g);
                    }
                case SpecialType.System_UInt64:
                    {
                        ulong[] values = constantValues.OfType<ulong>().ToArray();

                        for (int i = 0; i < values.Length; i++)
                        {
                            for (int j = 0; j < values.Length; j++)
                            {
                                if (j != i
                                    && (values[i] & values[j]) != 0)
                                {
                                    return null;
                                }
                            }
                        }

                        return constantValues.OfType<ulong>().Aggregate((f, g) => f + g);
                    }
            }

            return null;
        }

        private static bool IsValueDefined(INamedTypeSymbol enumSymbol, object value)
        {
            foreach (ISymbol member in enumSymbol.GetMembers())
            {
                if (member.Kind == SymbolKind.Field)
                {
                    var fieldSymbol = (IFieldSymbol)member;

                    if (fieldSymbol.HasConstantValue
                        && object.Equals(fieldSymbol.ConstantValue, value))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        private static EnumMemberDeclarationSyntax CreateEnumMember(string name, ImmutableArray<EnumMemberDeclarationSyntax> enumMembers)
        {
            ExpressionSyntax expression = IdentifierName(enumMembers.Last().Identifier.WithoutTrivia());

            for (int i = enumMembers.Length - 2; i >= 0; i--)
                expression = BitwiseOrExpression(IdentifierName(enumMembers[i].Identifier.WithoutTrivia()), expression);

            return EnumMemberDeclaration(
                default(SyntaxList<AttributeListSyntax>),
                Identifier(name).WithRenameAnnotation(),
                EqualsValueClause(expression));
        }

        private static Task<Document> RefactorAsync(
            Document document,
            EnumDeclarationSyntax enumDeclaration,
            EnumMemberDeclarationSyntax newEnumMember,
            int insertIndex,
            CancellationToken cancellationToken)
        {
            EnumDeclarationSyntax newNode = enumDeclaration.WithMembers(enumDeclaration.Members.Insert(insertIndex, newEnumMember));

            return document.ReplaceNodeAsync(enumDeclaration, newNode, cancellationToken);
        }
    }
}