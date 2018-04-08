// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Roslynator.CSharp.Comparers
{
    internal class EnumMemberDeclarationValueComparer : IComparer<EnumMemberDeclarationSyntax>
    {
        private readonly IComparer<object> _valueComparer;
        private readonly SemanticModel _semanticModel;
        private readonly CancellationToken _cancellationToken;

        public EnumMemberDeclarationValueComparer(IComparer<object> valueComparer, SemanticModel semanticModel, CancellationToken cancellationToken = default(CancellationToken))
        {
            _valueComparer = valueComparer;
            _semanticModel = semanticModel;
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
                _semanticModel.GetDeclaredSymbol(y, _cancellationToken),
                _valueComparer);
        }

        private static int Compare(IFieldSymbol fieldSymbol1, IFieldSymbol fieldSymbol2, IComparer<object> comparer)
        {
            if (fieldSymbol1?.HasConstantValue == true
                && fieldSymbol2?.HasConstantValue == true)
            {
                return comparer.Compare(fieldSymbol1.ConstantValue, fieldSymbol2.ConstantValue);
            }
            else
            {
                return 0;
            }
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

                    SpecialType enumSpecialType = fieldSymbol1.ContainingType.EnumUnderlyingType.SpecialType;

                    IComparer<object> comparer = EnumValueComparer.GetInstance(enumSpecialType);

                    while (en.MoveNext())
                    {
                        IFieldSymbol fieldSymbol2 = semanticModel.GetDeclaredSymbol(en.Current, cancellationToken);

                        if (Compare(fieldSymbol1, fieldSymbol2, comparer) > 0)
                            return false;

                        fieldSymbol1 = fieldSymbol2;
                    }
                }
            }

            return true;
        }
    }
}
