// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Roslynator.Utilities;

namespace Roslynator.CSharp.Refactorings
{
    internal static class DeclareEnumValueAsCombinationOfNamesRefactoring
    {
        public static void AnalyzeNamedType(SymbolAnalysisContext context, INamedTypeSymbol flagsAttribute)
        {
            var enumSymbol = (INamedTypeSymbol)context.Symbol;

            if (enumSymbol.IsEnum()
                && enumSymbol.HasAttribute(flagsAttribute))
            {
                var infos = default(ImmutableArray<EnumFieldInfo>);

                foreach (ISymbol member in enumSymbol.GetMembers())
                {
                    if (member.IsField())
                    {
                        var fieldSymbol = (IFieldSymbol)member;

                        if (!fieldSymbol.HasConstantValue)
                            break;

                        var info = new EnumFieldInfo(fieldSymbol);

                        if (info.IsComposite())
                        {
                            var declaration = (EnumMemberDeclarationSyntax)info.Symbol.GetSyntax(context.CancellationToken);

                            ExpressionSyntax valueExpression = declaration.EqualsValue?.Value;

                            if (valueExpression != null
                                && (valueExpression.IsKind(SyntaxKind.NumericLiteralExpression)
                                    || valueExpression
                                        .DescendantNodes()
                                        .Any(f => f.IsKind(SyntaxKind.NumericLiteralExpression))))
                            {
                                if (infos.IsDefault)
                                {
                                    infos = EnumFieldInfo.CreateRange(enumSymbol);

                                    if (infos.IsDefault)
                                        break;
                                }

                                List<EnumFieldInfo> values = info.Decompose(infos);

                                if (values?.Count > 1)
                                {
                                    context.ReportDiagnostic(
                                        DiagnosticDescriptors.DeclareEnumValueAsCombinationOfNames,
                                        valueExpression);
                                }
                            }
                        }
                    }
                }
            }
        }

        public static async Task<Document> RefactorAsync(
            Document document,
            EnumMemberDeclarationSyntax enumMemberDeclaration,
            CancellationToken cancellationToken)
        {
            SemanticModel semanticModel = await document.GetSemanticModelAsync(cancellationToken).ConfigureAwait(false);

            IFieldSymbol enumMemberSymbol = semanticModel.GetDeclaredSymbol(enumMemberDeclaration, cancellationToken);

            ImmutableArray<EnumFieldInfo> infos = EnumFieldInfo.CreateRange(enumMemberSymbol.ContainingType);

            ExpressionSyntax value = enumMemberDeclaration.EqualsValue?.Value;

            var info = new EnumFieldInfo(enumMemberSymbol);

            List<EnumFieldInfo> values = info.Decompose(infos);

            values.Sort((f, g) =>
            {
                if (f.IsComposite())
                {
                    if (g.IsComposite())
                    {
                        return ((IComparable)f.Value).CompareTo((IComparable)g.Value);
                    }
                    else
                    {
                        return -1;
                    }
                }
                else if (g.IsComposite())
                {
                    return 1;
                }

                return ((IComparable)f.Value).CompareTo((IComparable)g.Value);
            });

            BinaryExpressionSyntax newValue = CSharpFactory.BitwiseOrExpression(values[0].ToIdentifierName(), values[1].ToIdentifierName());

            for (int i = 2; i < values.Count; i++)
                newValue = CSharpFactory.BitwiseOrExpression(newValue, values[i].ToIdentifierName());

            newValue = newValue.WithFormatterAnnotation();

            return await document.ReplaceNodeAsync(value, newValue, cancellationToken).ConfigureAwait(false);
        }

        private struct EnumFieldInfo
        {
            public EnumFieldInfo(IFieldSymbol symbol)
            {
                Symbol = symbol;
                UnderlyingType = symbol.ContainingType.EnumUnderlyingType.SpecialType;
                Value = symbol.ConstantValue;
            }

            public IFieldSymbol Symbol { get; }
            public object Value { get; }
            public SpecialType UnderlyingType { get; }

            public bool IsComposite()
            {
                if (UnderlyingType == SpecialType.System_Int32)
                    return FlagsUtility.IsComposite((int)Value);

                switch (UnderlyingType)
                {
                    case SpecialType.System_SByte:
                        return FlagsUtility.IsComposite((sbyte)Value);
                    case SpecialType.System_Byte:
                        return FlagsUtility.IsComposite((byte)Value);
                    case SpecialType.System_Int16:
                        return FlagsUtility.IsComposite((short)Value);
                    case SpecialType.System_UInt16:
                        return FlagsUtility.IsComposite((ushort)Value);
                    case SpecialType.System_UInt32:
                        return FlagsUtility.IsComposite((uint)Value);
                    case SpecialType.System_Int64:
                        return FlagsUtility.IsComposite((long)Value);
                    case SpecialType.System_UInt64:
                        return FlagsUtility.IsComposite((ulong)Value);
                }

                return false;
            }

            public List<EnumFieldInfo> Decompose(ImmutableArray<EnumFieldInfo> infos)
            {
                if (UnderlyingType == SpecialType.System_Int32)
                    return Decompose((int)Value, infos);

                switch (UnderlyingType)
                {
                    case SpecialType.System_SByte:
                        return Decompose((sbyte)Value, infos);
                    case SpecialType.System_Byte:
                        return Decompose((byte)Value, infos);
                    case SpecialType.System_Int16:
                        return Decompose((short)Value, infos);
                    case SpecialType.System_UInt16:
                        return Decompose((ushort)Value, infos);
                    case SpecialType.System_UInt32:
                        return Decompose((uint)Value, infos);
                    case SpecialType.System_Int64:
                        return Decompose((long)Value, infos);
                    case SpecialType.System_UInt64:
                        return Decompose((ulong)Value, infos);
                }

                return null;
            }

            private static List<EnumFieldInfo> Decompose(int value, ImmutableArray<EnumFieldInfo> infos)
            {
                List<EnumFieldInfo> values = null;

                int i = infos.Length - 1;

                while (i >= 0
                    && Convert.ToInt32(infos[i].Value) >= value)
                {
                    i--;
                }

                while (i >= 0)
                {
                    int value2 = Convert.ToInt32(infos[i].Value);

                    if ((value & value2) == value2)
                    {
                        (values ?? (values = new List<EnumFieldInfo>())).Add(infos[i]);

                        value &= ~value2;

                        if (value == 0)
                            return values;
                    }

                    i--;
                }

                return null;
            }

            private List<EnumFieldInfo> Decompose(long value, ImmutableArray<EnumFieldInfo> infos)
            {
                List<EnumFieldInfo> values = null;

                int i = infos.Length - 1;

                while (i >= 0
                    && Convert.ToInt64(infos[i].Value) >= value)
                {
                    i--;
                }

                while (i >= 0)
                {
                    long value2 = Convert.ToInt64(infos[i].Value);

                    if ((value & value2) == value2)
                    {
                        (values ?? (values = new List<EnumFieldInfo>())).Add(infos[i]);

                        value &= ~value2;

                        if (value == 0)
                            return values;
                    }

                    i--;
                }

                return null;
            }

            private List<EnumFieldInfo> Decompose(ulong value, ImmutableArray<EnumFieldInfo> infos)
            {
                List<EnumFieldInfo> values = null;

                int i = infos.Length - 1;

                while (i >= 0
                    && Convert.ToUInt64(infos[i].Value) >= value)
                {
                    i--;
                }

                while (i >= 0)
                {
                    ulong value2 = Convert.ToUInt64(infos[i].Value);

                    if ((value & value2) == value2)
                    {
                        (values ?? (values = new List<EnumFieldInfo>())).Add(infos[i]);

                        value &= ~value2;

                        if (value == 0)
                            return values;
                    }

                    i--;
                }

                return null;
            }

            public IdentifierNameSyntax ToIdentifierName()
            {
                return SyntaxFactory.IdentifierName(Symbol.Name);
            }

            public static ImmutableArray<EnumFieldInfo> CreateRange(INamedTypeSymbol enumSymbol)
            {
                ImmutableArray<EnumFieldInfo> infos = enumSymbol
                    .GetMembers()
                    .OfType<IFieldSymbol>()
                    .Select(symbol => new EnumFieldInfo(symbol))
                    .ToImmutableArray();

                foreach (EnumFieldInfo info in infos)
                {
                    if (!info.Symbol.HasConstantValue)
                        return default(ImmutableArray<EnumFieldInfo>);
                }

                infos = infos.Sort((f, g) => ((IComparable)f.Value).CompareTo(g.Value));

                return infos;
            }
        }
    }
}
