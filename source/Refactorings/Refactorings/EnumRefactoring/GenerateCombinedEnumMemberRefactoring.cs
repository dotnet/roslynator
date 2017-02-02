// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using Roslynator.Extensions;

namespace Roslynator.CSharp.Refactorings.EnumWithFlagsAttribute
{
    internal static class GenerateCombinedEnumMemberRefactoring
    {
        public static async Task ComputeRefactoringAsync(RefactoringContext context, EnumDeclarationSyntax enumDeclaration)
        {
            EnumMemberDeclarationSyntax[] selectedMembers = GetSelectedMembers(enumDeclaration, context.Span).ToArray();

            if (selectedMembers.Length > 1)
            {
                SemanticModel semanticModel = await context.GetSemanticModelAsync().ConfigureAwait(false);

                List<IFieldSymbol> fieldSymbols = GetFieldSymbols(selectedMembers, semanticModel, context.CancellationToken);

                if (fieldSymbols.Count > 1)
                {
                    var enumSymbol = semanticModel.GetDeclaredSymbol(enumDeclaration, context.CancellationToken) as INamedTypeSymbol;

                    if (enumSymbol != null)
                    {
                        object combinedValue = GetCombinedValue(fieldSymbols, enumSymbol);

                        if (combinedValue != null
                            && !EnumHelper.IsValueDefined(combinedValue, enumSymbol, semanticModel, context.CancellationToken))
                        {
                            string name = GetCombinedName(fieldSymbols);

                            EnumMemberDeclarationSyntax newEnumMember = EnumHelper.CreateEnumMember(enumSymbol, name, combinedValue);

                            int insertIndex = enumDeclaration.Members.IndexOf(selectedMembers.Last()) + 1;

                            context.RegisterRefactoring(
                                $"Generate enum member '{name}'",
                                cancellationToken => RefactorAsync(context.Document, enumDeclaration, newEnumMember, insertIndex, cancellationToken));
                        }
                    }
                }
            }
        }

        private static List<IFieldSymbol> GetFieldSymbols(
            EnumMemberDeclarationSyntax[] enumMembers,
            SemanticModel semanticModel,
            CancellationToken cancellationToken)
        {
            var fieldSymbols = new List<IFieldSymbol>();

            foreach (EnumMemberDeclarationSyntax enumMember in enumMembers)
            {
                IFieldSymbol fieldSymbol = semanticModel.GetDeclaredSymbol(enumMember, cancellationToken);

                if (fieldSymbol.HasConstantValue)
                {
                    object value = fieldSymbol.ConstantValue;

                    if (value is sbyte)
                    {
                        if (FlagsGenerator.IsPowerOfTwo((sbyte)value))
                            fieldSymbols.Add(fieldSymbol);
                    }
                    else if (value is byte)
                    {
                        if (FlagsGenerator.IsPowerOfTwo((byte)value))
                            fieldSymbols.Add(fieldSymbol);
                    }
                    else if (value is ushort)
                    {
                        if (FlagsGenerator.IsPowerOfTwo((ushort)value))
                            fieldSymbols.Add(fieldSymbol);
                    }
                    else if (value is short)
                    {
                        if (FlagsGenerator.IsPowerOfTwo((short)value))
                            fieldSymbols.Add(fieldSymbol);
                    }
                    else if (value is uint)
                    {
                        if (FlagsGenerator.IsPowerOfTwo((uint)value))
                            fieldSymbols.Add(fieldSymbol);
                    }
                    else if (value is int)
                    {
                        if (FlagsGenerator.IsPowerOfTwo((int)value))
                            fieldSymbols.Add(fieldSymbol);
                    }
                    else if (value is ulong)
                    {
                        if (FlagsGenerator.IsPowerOfTwo((ulong)value))
                            fieldSymbols.Add(fieldSymbol);
                    }
                    else if (value is long)
                    {
                        if (FlagsGenerator.IsPowerOfTwo((long)value))
                            fieldSymbols.Add(fieldSymbol);
                    }
                }
            }

            return fieldSymbols;
        }

        private static object GetCombinedValue(IEnumerable<IFieldSymbol> fieldSymbols, INamedTypeSymbol enumSymbol)
        {
            switch (enumSymbol.EnumUnderlyingType.SpecialType)
            {
                case SpecialType.System_SByte:
                    {
                        return fieldSymbols
                            .Where(f => f.HasConstantValue)
                            .Select(f => f.ConstantValue)
                            .OfType<sbyte>().Aggregate((f, g) => (sbyte)(f + g));
                    }
                case SpecialType.System_Byte:
                    {
                        return fieldSymbols
                            .Where(f => f.HasConstantValue)
                            .Select(f => f.ConstantValue)
                            .OfType<byte>().Aggregate((f, g) => (byte)(f + g));
                    }
                case SpecialType.System_Int16:
                    {
                        return fieldSymbols
                            .Where(f => f.HasConstantValue)
                            .Select(f => f.ConstantValue)
                            .OfType<short>().Aggregate((f, g) => (short)(f + g));
                    }
                case SpecialType.System_UInt16:
                    {
                        return fieldSymbols
                            .Where(f => f.HasConstantValue)
                            .Select(f => f.ConstantValue)
                            .OfType<ushort>().Aggregate((f, g) => (ushort)(f + g));
                    }
                case SpecialType.System_Int32:
                    {
                        return fieldSymbols
                            .Where(f => f.HasConstantValue)
                            .Select(f => f.ConstantValue)
                            .OfType<int>().Aggregate((f, g) => f + g);
                    }
                case SpecialType.System_UInt32:
                    {
                        return fieldSymbols
                            .Where(f => f.HasConstantValue)
                            .Select(f => f.ConstantValue)
                            .OfType<uint>().Aggregate((f, g) => f + g);
                    }
                case SpecialType.System_Int64:
                    {
                        return fieldSymbols
                            .Where(f => f.HasConstantValue)
                            .Select(f => f.ConstantValue)
                            .OfType<long>().Aggregate((f, g) => f + g);
                    }
                case SpecialType.System_UInt64:
                    {
                        return fieldSymbols
                            .Where(f => f.HasConstantValue)
                            .Select(f => f.ConstantValue)
                            .OfType<ulong>().Aggregate((f, g) => f + g);
                    }
            }

            return null;
        }

        public static string GetCombinedName(IEnumerable<IFieldSymbol> fieldSymbols)
        {
            string s = "";

            foreach (IFieldSymbol fieldSymbol in fieldSymbols)
            {
                string name = fieldSymbol.Name;

                if (name != null)
                    s += fieldSymbol.Name;
            }

            return s;
        }

        private static async Task<Document> RefactorAsync(
            Document document,
            EnumDeclarationSyntax enumDeclaration,
            EnumMemberDeclarationSyntax newEnumMember,
            int insertIndex,
            CancellationToken cancellationToken)
        {
            SemanticModel semanticModel = await document.GetSemanticModelAsync(cancellationToken).ConfigureAwait(false);

            EnumDeclarationSyntax newNode = enumDeclaration.WithMembers(enumDeclaration.Members.Insert(insertIndex, newEnumMember));

            return await document.ReplaceNodeAsync(enumDeclaration, newNode, cancellationToken).ConfigureAwait(false);
        }

        private static IEnumerable<EnumMemberDeclarationSyntax> GetSelectedMembers(EnumDeclarationSyntax enumDeclaration, TextSpan span)
        {
            return enumDeclaration.Members
                .SkipWhile(f => span.Start > f.Span.Start)
                .TakeWhile(f => span.End >= f.Span.End);
        }
    }
}