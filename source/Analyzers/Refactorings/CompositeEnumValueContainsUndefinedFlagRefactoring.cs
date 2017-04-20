// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Roslynator.Utilities;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static Roslynator.CSharp.CSharpFactory;

namespace Roslynator.CSharp.Refactorings
{
    internal static class CompositeEnumValueContainsUndefinedFlagRefactoring
    {
        public static void AnalyzeNamedType(SymbolAnalysisContext context)
        {
            var namedType = (INamedTypeSymbol)context.Symbol;

            if (namedType.IsEnumWithFlagsAttribute(context.Compilation))
                Analyze(context, namedType);
        }

        private static void Analyze(SymbolAnalysisContext context, INamedTypeSymbol enumType)
        {
            IFieldSymbol[] fields = enumType
                .GetMembers()
                .Where(f => f.IsField())
                .Cast<IFieldSymbol>()
                .ToArray();

            switch (enumType.EnumUnderlyingType.SpecialType)
            {
                case SpecialType.System_SByte:
                    {
                        sbyte[] values = GetValues<sbyte>(fields);

                        for (int i = 0; i < values.Length; i++)
                        {
                            if (values[i] != 0
                                && FlagsUtility.IsComposite(values[i]))
                            {
                                foreach (sbyte value in FlagsUtility.Decompose(values[i]))
                                {
                                    if (Array.IndexOf(values, value) == -1)
                                        ReportDiagnostic(context, fields[i], value.ToString());
                                }
                            }
                        }

                        break;
                    }
                case SpecialType.System_Byte:
                    {
                        byte[] values = GetValues<byte>(fields);

                        for (int i = 0; i < values.Length; i++)
                        {
                            if (values[i] != 0
                                && FlagsUtility.IsComposite(values[i]))
                            {
                                foreach (byte value in FlagsUtility.Decompose(values[i]))
                                {
                                    if (Array.IndexOf(values, value) == -1)
                                        ReportDiagnostic(context, fields[i], value.ToString());
                                }
                            }
                        }

                        break;
                    }
                case SpecialType.System_Int16:
                    {
                        short[] values = GetValues<short>(fields);

                        for (int i = 0; i < values.Length; i++)
                        {
                            if (values[i] != 0
                                && FlagsUtility.IsComposite(values[i]))
                            {
                                foreach (short value in FlagsUtility.Decompose(values[i]))
                                {
                                    if (Array.IndexOf(values, value) == -1)
                                        ReportDiagnostic(context, fields[i], value.ToString());
                                }
                            }
                        }

                        break;
                    }
                case SpecialType.System_UInt16:
                    {
                        ushort[] values = GetValues<ushort>(fields);

                        for (int i = 0; i < values.Length; i++)
                        {
                            if (values[i] != 0
                                && FlagsUtility.IsComposite(values[i]))
                            {
                                foreach (ushort value in FlagsUtility.Decompose(values[i]))
                                {
                                    if (Array.IndexOf(values, value) == -1)
                                        ReportDiagnostic(context, fields[i], value.ToString());
                                }
                            }
                        }

                        break;
                    }
                case SpecialType.System_Int32:
                    {
                        int[] values = GetValues<int>(fields);

                        for (int i = 0; i < values.Length; i++)
                        {
                            if (values[i] != 0
                                && FlagsUtility.IsComposite(values[i]))
                            {
                                foreach (int value in FlagsUtility.Decompose(values[i]))
                                {
                                    if (Array.IndexOf(values, value) == -1)
                                        ReportDiagnostic(context, fields[i], value.ToString());
                                }
                            }
                        }

                        break;
                    }
                case SpecialType.System_UInt32:
                    {
                        uint[] values = GetValues<uint>(fields);

                        for (int i = 0; i < values.Length; i++)
                        {
                            if (values[i] != 0
                                && FlagsUtility.IsComposite(values[i]))
                            {
                                foreach (uint value in FlagsUtility.Decompose(values[i]))
                                {
                                    if (Array.IndexOf(values, value) == -1)
                                        ReportDiagnostic(context, fields[i], value.ToString());
                                }
                            }
                        }

                        break;
                    }
                case SpecialType.System_Int64:
                    {
                        long[] values = GetValues<long>(fields);

                        for (int i = 0; i < values.Length; i++)
                        {
                            if (values[i] != 0
                                && FlagsUtility.IsComposite(values[i]))
                            {
                                foreach (long value in FlagsUtility.Decompose(values[i]))
                                {
                                    if (Array.IndexOf(values, value) == -1)
                                        ReportDiagnostic(context, fields[i], value.ToString());
                                }
                            }
                        }

                        break;
                    }
                case SpecialType.System_UInt64:
                    {
                        ulong[] values = GetValues<ulong>(fields);

                        for (int i = 0; i < values.Length; i++)
                        {
                            if (values[i] != 0
                                && FlagsUtility.IsComposite(values[i]))
                            {
                                foreach (ulong value in FlagsUtility.Decompose(values[i]))
                                {
                                    if (Array.IndexOf(values, value) == -1)
                                        ReportDiagnostic(context, fields[i], value.ToString());
                                }
                            }
                        }

                        break;
                    }
                default:
                    {
                        Debug.Fail(enumType.EnumUnderlyingType.SpecialType.ToString());
                        break;
                    }
            }
        }

        private static T[] GetValues<T>(IFieldSymbol[] fields)
        {
            var values = new T[fields.Length];

            for (int i = 0; i < fields.Length; i++)
            {
                if (fields[i].HasConstantValue)
                {
                    values[i] = (T)fields[i].ConstantValue;
                }
                else
                {
                    values[i] = default(T);
                }
            }

            return values;
        }

        private static void ReportDiagnostic(SymbolAnalysisContext context, IFieldSymbol field, string value)
        {
            SyntaxReference syntaxReference = field.DeclaringSyntaxReferences.FirstOrDefault();

            Debug.Assert(syntaxReference != null, "");

            if (syntaxReference != null)
            {
                SyntaxNode node = syntaxReference.GetSyntax(context.CancellationToken);

                Debug.Assert(node.IsKind(SyntaxKind.EnumMemberDeclaration), node.Kind().ToString());

                if (node.IsKind(SyntaxKind.EnumMemberDeclaration))
                {
                    var enumMember = (EnumMemberDeclarationSyntax)node;

                    Diagnostic diagnostic = Diagnostic.Create(
                        DiagnosticDescriptors.CompositeEnumValueContainsUndefinedFlag,
                        enumMember.GetLocation(),
                        ImmutableDictionary.CreateRange(new KeyValuePair<string, string>[] { new KeyValuePair<string, string>("Value", value) }),
                        value);

                    context.ReportDiagnostic(diagnostic);
                }
            }
        }

        public static async Task<Document> RefactorAsync(
            Document document,
            EnumDeclarationSyntax enumDeclaration,
            string value,
            CancellationToken cancellationToken)
        {
            SemanticModel semanticModel = await document.GetSemanticModelAsync(cancellationToken).ConfigureAwait(false);

            INamedTypeSymbol symbol = semanticModel.GetDeclaredSymbol(enumDeclaration, cancellationToken);

            string name = NameGenerator.Default.EnsureUniqueEnumMemberName(DefaultNames.EnumMember, symbol);

            EnumMemberDeclarationSyntax enumMember = EnumMemberDeclaration(
                Identifier(name).WithRenameAnnotation(),
                ParseExpression(value));

            enumMember = enumMember.WithTrailingTrivia(NewLine());

            EnumDeclarationSyntax newNode = enumDeclaration.WithMembers(enumDeclaration.Members.Add(enumMember));

            return await document.ReplaceNodeAsync(enumDeclaration, newNode, cancellationToken).ConfigureAwait(false);
        }
    }
}
