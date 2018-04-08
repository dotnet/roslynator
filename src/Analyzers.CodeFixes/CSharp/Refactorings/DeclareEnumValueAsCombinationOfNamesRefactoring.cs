// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Roslynator.CSharp.CSharpFactory;
using static Roslynator.CSharp.Analysis.DeclareEnumValueAsCombinationOfNamesAnalyzer;

namespace Roslynator.CSharp.Refactorings
{
    internal static class DeclareEnumValueAsCombinationOfNamesRefactoring
    {
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

            BinaryExpressionSyntax newValue = BitwiseOrExpression(values[0].ToIdentifierName(), values[1].ToIdentifierName());

            for (int i = 2; i < values.Count; i++)
                newValue = BitwiseOrExpression(newValue, values[i].ToIdentifierName());

            newValue = newValue.WithFormatterAnnotation();

            return await document.ReplaceNodeAsync(value, newValue, cancellationToken).ConfigureAwait(false);
        }
    }
}
