// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Roslynator.CSharp.Analysis
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class CompositeEnumValueContainsUndefinedFlagAnalyzer : BaseDiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        {
            get { return ImmutableArray.Create(DiagnosticDescriptors.CompositeEnumValueContainsUndefinedFlag); }
        }

        public override void Initialize(AnalysisContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            base.Initialize(context);

            context.RegisterSymbolAction(AnalyzeNamedType, SymbolKind.NamedType);
        }

        //XPERF:
        public static void AnalyzeNamedType(SymbolAnalysisContext context)
        {
            var namedType = (INamedTypeSymbol)context.Symbol;

            if (namedType.TypeKind != TypeKind.Enum)
                return;

            if (!namedType.HasAttribute(MetadataNames.System_FlagsAttribute))
                return;

            ImmutableArray<ISymbol> fields = namedType.GetMembers();

            switch (namedType.EnumUnderlyingType.SpecialType)
            {
                case SpecialType.System_SByte:
                    {
                        ImmutableArray<sbyte> values = GetValues<sbyte>(fields);

                        for (int i = 0; i < values.Length; i++)
                        {
                            if (values[i] != 0
                                && FlagsUtility<sbyte>.Instance.IsComposite(values[i]))
                            {
                                foreach (sbyte value in FlagsUtility<sbyte>.Instance.Decompose(values[i]))
                                {
                                    if (values.IndexOf(value) == -1)
                                        ReportDiagnostic(context, fields[i], value.ToString());
                                }
                            }
                        }

                        break;
                    }
                case SpecialType.System_Byte:
                    {
                        ImmutableArray<byte> values = GetValues<byte>(fields);

                        for (int i = 0; i < values.Length; i++)
                        {
                            if (values[i] != 0
                                && FlagsUtility<byte>.Instance.IsComposite(values[i]))
                            {
                                foreach (byte value in FlagsUtility<byte>.Instance.Decompose(values[i]))
                                {
                                    if (values.IndexOf(value) == -1)
                                        ReportDiagnostic(context, fields[i], value.ToString());
                                }
                            }
                        }

                        break;
                    }
                case SpecialType.System_Int16:
                    {
                        ImmutableArray<short> values = GetValues<short>(fields);

                        for (int i = 0; i < values.Length; i++)
                        {
                            if (values[i] != 0
                                && FlagsUtility<short>.Instance.IsComposite(values[i]))
                            {
                                foreach (short value in FlagsUtility<short>.Instance.Decompose(values[i]))
                                {
                                    if (values.IndexOf(value) == -1)
                                        ReportDiagnostic(context, fields[i], value.ToString());
                                }
                            }
                        }

                        break;
                    }
                case SpecialType.System_UInt16:
                    {
                        ImmutableArray<ushort> values = GetValues<ushort>(fields);

                        for (int i = 0; i < values.Length; i++)
                        {
                            if (values[i] != 0
                                && FlagsUtility<ushort>.Instance.IsComposite(values[i]))
                            {
                                foreach (ushort value in FlagsUtility<ushort>.Instance.Decompose(values[i]))
                                {
                                    if (values.IndexOf(value) == -1)
                                        ReportDiagnostic(context, fields[i], value.ToString());
                                }
                            }
                        }

                        break;
                    }
                case SpecialType.System_Int32:
                    {
                        ImmutableArray<int> values = GetValues<int>(fields);

                        for (int i = 0; i < values.Length; i++)
                        {
                            if (values[i] != 0
                                && FlagsUtility<int>.Instance.IsComposite(values[i]))
                            {
                                foreach (int value in FlagsUtility<int>.Instance.Decompose(values[i]))
                                {
                                    if (values.IndexOf(value) == -1)
                                        ReportDiagnostic(context, fields[i], value.ToString());
                                }
                            }
                        }

                        break;
                    }
                case SpecialType.System_UInt32:
                    {
                        ImmutableArray<uint> values = GetValues<uint>(fields);

                        for (int i = 0; i < values.Length; i++)
                        {
                            if (values[i] != 0
                                && FlagsUtility<uint>.Instance.IsComposite(values[i]))
                            {
                                foreach (uint value in FlagsUtility<uint>.Instance.Decompose(values[i]))
                                {
                                    if (values.IndexOf(value) == -1)
                                        ReportDiagnostic(context, fields[i], value.ToString());
                                }
                            }
                        }

                        break;
                    }
                case SpecialType.System_Int64:
                    {
                        ImmutableArray<long> values = GetValues<long>(fields);

                        for (int i = 0; i < values.Length; i++)
                        {
                            if (values[i] != 0
                                && FlagsUtility<long>.Instance.IsComposite(values[i]))
                            {
                                foreach (long value in FlagsUtility<long>.Instance.Decompose(values[i]))
                                {
                                    if (values.IndexOf(value) == -1)
                                        ReportDiagnostic(context, fields[i], value.ToString());
                                }
                            }
                        }

                        break;
                    }
                case SpecialType.System_UInt64:
                    {
                        ImmutableArray<ulong> values = GetValues<ulong>(fields);

                        for (int i = 0; i < values.Length; i++)
                        {
                            if (values[i] != 0
                                && FlagsUtility<ulong>.Instance.IsComposite(values[i]))
                            {
                                foreach (ulong value in FlagsUtility<ulong>.Instance.Decompose(values[i]))
                                {
                                    if (values.IndexOf(value) == -1)
                                        ReportDiagnostic(context, fields[i], value.ToString());
                                }
                            }
                        }

                        break;
                    }
                default:
                    {
                        Debug.Fail(namedType.EnumUnderlyingType.SpecialType.ToString());
                        break;
                    }
            }
        }

        private static ImmutableArray<T> GetValues<T>(ImmutableArray<ISymbol> members)
        {
            return ImmutableArray.CreateRange(members, member =>
            {
                if (!member.IsImplicitlyDeclared
                    && member is IFieldSymbol fieldSymbol
                    && fieldSymbol.HasConstantValue)
                {
                    return (T)fieldSymbol.ConstantValue;
                }
                else
                {
                    return default(T);
                }
            });
        }

        private static void ReportDiagnostic(SymbolAnalysisContext context, ISymbol fieldSymbol, string value)
        {
            var enumMember = (EnumMemberDeclarationSyntax)fieldSymbol.GetSyntax(context.CancellationToken);

            context.ReportDiagnostic(
                DiagnosticDescriptors.CompositeEnumValueContainsUndefinedFlag,
                enumMember.GetLocation(),
                ImmutableDictionary.CreateRange(new KeyValuePair<string, string>[] { new KeyValuePair<string, string>("Value", value) }),
                value);
        }
    }
}