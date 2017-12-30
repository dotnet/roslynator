// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Roslynator.CSharp.Comparers
{
    internal class EnumMemberDeclarationValueComparer : IComparer<EnumMemberDeclarationSyntax>
    {
        private readonly SemanticModel _semanticModel;
        private readonly CancellationToken _cancellationToken;

        public EnumMemberDeclarationValueComparer(SemanticModel semanticModel, CancellationToken cancellationToken = default(CancellationToken))
        {
            _semanticModel = semanticModel ?? throw new ArgumentNullException(nameof(semanticModel));
            _cancellationToken = cancellationToken;
        }

        public int Compare(EnumMemberDeclarationSyntax x, EnumMemberDeclarationSyntax y)
        {
            if (object.ReferenceEquals(x, y))
                return 0;

            if (x == null)
                return -1;

            if (y == null)
                return 1;

            return Compare(
                _semanticModel.GetDeclaredSymbol(x, _cancellationToken),
                _semanticModel.GetDeclaredSymbol(y, _cancellationToken));
        }

        private static int Compare(IFieldSymbol fieldSymbol1, IFieldSymbol fieldSymbol2)
        {
            if (fieldSymbol1?.HasConstantValue == true
                && fieldSymbol2?.HasConstantValue == true)
            {
                return CompareConstantValue(fieldSymbol1.ConstantValue, fieldSymbol2.ConstantValue);
            }
            else
            {
                return 0;
            }
        }

        public static int CompareConstantValue(object x, object y)
        {
            if (x is sbyte)
            {
                if (y is sbyte)
                    return ((sbyte)x).CompareTo((sbyte)y);
            }
            else if (x is byte)
            {
                if (y is byte)
                    return ((byte)x).CompareTo((byte)y);
            }
            else if (x is ushort)
            {
                if (y is ushort)
                    return ((ushort)x).CompareTo((ushort)y);
            }
            else if (x is short)
            {
                if (y is short)
                    return ((short)x).CompareTo((short)y);
            }
            else if (x is uint)
            {
                if (y is uint)
                    return ((uint)x).CompareTo((uint)y);
            }
            else if (x is int)
            {
                if (y is int)
                    return ((int)x).CompareTo((int)y);
            }
            else if (x is ulong)
            {
                if (y is ulong)
                    return ((ulong)x).CompareTo((ulong)y);
            }
            else if (x is long)
            {
                if (y is long)
                    return ((long)x).CompareTo((long)y);
            }

            Debug.Fail($"{nameof(EnumMemberDeclarationValueComparer)} cannot compare {x.GetType()} with {y.GetType()}");

            return 0;
        }

        public static bool IsSorted(
            IEnumerable<EnumMemberDeclarationSyntax> enumMembers,
            SemanticModel semanticModel,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (enumMembers == null)
                throw new ArgumentNullException(nameof(enumMembers));

            if (semanticModel == null)
                throw new ArgumentNullException(nameof(semanticModel));

            using (IEnumerator<EnumMemberDeclarationSyntax> en = enumMembers.GetEnumerator())
            {
                if (en.MoveNext())
                {
                    IFieldSymbol fieldSymbol1 = semanticModel.GetDeclaredSymbol(en.Current, cancellationToken);

                    while (en.MoveNext())
                    {
                        IFieldSymbol fieldSymbol2 = semanticModel.GetDeclaredSymbol(en.Current, cancellationToken);

                        if (Compare(fieldSymbol1, fieldSymbol2) > 0)
                            return false;

                        fieldSymbol1 = fieldSymbol2;
                    }
                }
            }

            return true;
        }
    }
}
