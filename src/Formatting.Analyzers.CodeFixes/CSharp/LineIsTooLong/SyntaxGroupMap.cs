// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Immutable;
using Microsoft.CodeAnalysis.CSharp;
using KeyValuePair = System.Collections.Generic.KeyValuePair<
    Microsoft.CodeAnalysis.CSharp.SyntaxKind,
    Roslynator.Formatting.CodeFixes.LineIsTooLong.SyntaxGroup>;

namespace Roslynator.Formatting.CodeFixes.LineIsTooLong
{
    internal static class SyntaxGroupMap
    {
        internal static ImmutableDictionary<SyntaxKind, SyntaxGroup> Value { get; } = ImmutableDictionary.CreateRange(
            new[]
            {
                new KeyValuePair(SyntaxKind.ArrowExpressionClause,SyntaxGroup.ArrowExpressionClause),
                new KeyValuePair(SyntaxKind.ForStatement, SyntaxGroup.ForStatement),
                new KeyValuePair(SyntaxKind.ConditionalExpression,SyntaxGroup.ConditionalExpression),
                new KeyValuePair(SyntaxKind.AttributeList, SyntaxGroup.AttributeList),
                new KeyValuePair(SyntaxKind.BaseList, SyntaxGroup.BaseList),

                new KeyValuePair(SyntaxKind.ArrayInitializerExpression,SyntaxGroup.InitializerExpression),
                new KeyValuePair(SyntaxKind.CollectionInitializerExpression,SyntaxGroup.InitializerExpression),
                new KeyValuePair(SyntaxKind.ComplexElementInitializerExpression,SyntaxGroup.InitializerExpression),
                new KeyValuePair(SyntaxKind.ObjectInitializerExpression,SyntaxGroup.InitializerExpression),

                new KeyValuePair(SyntaxKind.ParameterList, SyntaxGroup.ParameterList),
                new KeyValuePair(SyntaxKind.BracketedParameterList, SyntaxGroup.ParameterList),

                new KeyValuePair(SyntaxKind.LogicalOrExpression,SyntaxGroup.BinaryExpression),
                new KeyValuePair(SyntaxKind.LogicalAndExpression,SyntaxGroup.BinaryExpression),
                new KeyValuePair(SyntaxKind.BitwiseOrExpression,SyntaxGroup.BinaryExpression),
                new KeyValuePair(SyntaxKind.BitwiseAndExpression,SyntaxGroup.BinaryExpression),
                new KeyValuePair(SyntaxKind.CoalesceExpression,SyntaxGroup.BinaryExpression),
                new KeyValuePair(SyntaxKind.AddExpression, SyntaxGroup.BinaryExpression),
                new KeyValuePair(SyntaxKind.SubtractExpression, SyntaxGroup.BinaryExpression),
                new KeyValuePair(SyntaxKind.MultiplyExpression, SyntaxGroup.BinaryExpression),
                new KeyValuePair(SyntaxKind.DivideExpression, SyntaxGroup.BinaryExpression),
                new KeyValuePair(SyntaxKind.ModuloExpression, SyntaxGroup.BinaryExpression),
                new KeyValuePair(SyntaxKind.LeftShiftExpression, SyntaxGroup.BinaryExpression),
                new KeyValuePair(SyntaxKind.RightShiftExpression, SyntaxGroup.BinaryExpression),
                new KeyValuePair(SyntaxKind.ExclusiveOrExpression, SyntaxGroup.BinaryExpression),

                new KeyValuePair(SyntaxKind.SimpleMemberAccessExpression,SyntaxGroup.MemberExpression),
                new KeyValuePair(SyntaxKind.MemberBindingExpression, SyntaxGroup.MemberExpression),

                new KeyValuePair(SyntaxKind.ArgumentList, SyntaxGroup.ArgumentList),
                new KeyValuePair(SyntaxKind.BracketedArgumentList, SyntaxGroup.ArgumentList),
                new KeyValuePair(SyntaxKind.AttributeArgumentList, SyntaxGroup.ArgumentList),

                new KeyValuePair(SyntaxKind.EqualsExpression, SyntaxGroup.Is_As_EqualityExpression),
                new KeyValuePair(SyntaxKind.NotEqualsExpression,SyntaxGroup.Is_As_EqualityExpression),
                new KeyValuePair(SyntaxKind.LessThanExpression,SyntaxGroup.Is_As_EqualityExpression),
                new KeyValuePair(SyntaxKind.LessThanExpression,SyntaxGroup.Is_As_EqualityExpression),
                new KeyValuePair(SyntaxKind.GreaterThanExpression,SyntaxGroup.Is_As_EqualityExpression),
                new KeyValuePair(SyntaxKind.GreaterThanOrEqualExpression,SyntaxGroup.Is_As_EqualityExpression),
                new KeyValuePair(SyntaxKind.IsExpression, SyntaxGroup.Is_As_EqualityExpression),
                new KeyValuePair(SyntaxKind.AsExpression, SyntaxGroup.Is_As_EqualityExpression),

                new KeyValuePair(SyntaxKind.AddAssignmentExpression,SyntaxGroup.AssignmentExpression),
                new KeyValuePair(SyntaxKind.AndAssignmentExpression,SyntaxGroup.AssignmentExpression),
                new KeyValuePair(SyntaxKind.CoalesceAssignmentExpression,SyntaxGroup.AssignmentExpression),
                new KeyValuePair(SyntaxKind.DivideAssignmentExpression,SyntaxGroup.AssignmentExpression),
                new KeyValuePair(SyntaxKind.ExclusiveOrAssignmentExpression,SyntaxGroup.AssignmentExpression),
                new KeyValuePair(SyntaxKind.LeftShiftAssignmentExpression,SyntaxGroup.AssignmentExpression),
                new KeyValuePair(SyntaxKind.ModuloAssignmentExpression,SyntaxGroup.AssignmentExpression),
                new KeyValuePair(SyntaxKind.MultiplyAssignmentExpression,SyntaxGroup.AssignmentExpression),
                new KeyValuePair(SyntaxKind.OrAssignmentExpression,SyntaxGroup.AssignmentExpression),
                new KeyValuePair(SyntaxKind.RightShiftAssignmentExpression,SyntaxGroup.AssignmentExpression),
                new KeyValuePair(SyntaxKind.SimpleAssignmentExpression,SyntaxGroup.AssignmentExpression),
                new KeyValuePair(SyntaxKind.SubtractAssignmentExpression,SyntaxGroup.AssignmentExpression),
            });
    }
}

