// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Roslynator.CSharp.Analysis
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class DeclareEnumValueAsCombinationOfNamesAnalyzer : BaseDiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        {
            get { return ImmutableArray.Create(DiagnosticDescriptors.DeclareEnumValueAsCombinationOfNames); }
        }

        public override void Initialize(AnalysisContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            base.Initialize(context);

            context.RegisterSymbolAction(AnalyzeNamedType, SymbolKind.NamedType);
        }

        public static void AnalyzeNamedType(SymbolAnalysisContext context)
        {
            var enumSymbol = (INamedTypeSymbol)context.Symbol;

            if (enumSymbol.TypeKind != TypeKind.Enum)
                return;

            if (!enumSymbol.HasAttribute(MetadataNames.System_FlagsAttribute))
                return;

            var infos = default(ImmutableArray<EnumFieldInfo>);

            foreach (ISymbol member in enumSymbol.GetMembers())
            {
                if (member is IFieldSymbol fieldSymbol)
                {
                    if (!fieldSymbol.HasConstantValue)
                        return;

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
                                    return;
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

        internal readonly struct EnumFieldInfo
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
                switch (UnderlyingType)
                {
                    case SpecialType.System_SByte:
                        return FlagsUtility<sbyte>.Instance.IsComposite((sbyte)Value);
                    case SpecialType.System_Byte:
                        return FlagsUtility<byte>.Instance.IsComposite((byte)Value);
                    case SpecialType.System_Int16:
                        return FlagsUtility<short>.Instance.IsComposite((short)Value);
                    case SpecialType.System_UInt16:
                        return FlagsUtility<ushort>.Instance.IsComposite((ushort)Value);
                    case SpecialType.System_Int32:
                        return FlagsUtility<int>.Instance.IsComposite((int)Value);
                    case SpecialType.System_UInt32:
                        return FlagsUtility<uint>.Instance.IsComposite((uint)Value);
                    case SpecialType.System_Int64:
                        return FlagsUtility<long>.Instance.IsComposite((long)Value);
                    case SpecialType.System_UInt64:
                        return FlagsUtility<ulong>.Instance.IsComposite((ulong)Value);
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

            private static List<EnumFieldInfo> Decompose(long value, ImmutableArray<EnumFieldInfo> infos)
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

            private static List<EnumFieldInfo> Decompose(ulong value, ImmutableArray<EnumFieldInfo> infos)
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

                return infos.Sort((f, g) => ((IComparable)f.Value).CompareTo(g.Value));
            }
        }
    }
}