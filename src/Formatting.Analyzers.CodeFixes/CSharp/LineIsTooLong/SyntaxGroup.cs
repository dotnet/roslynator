// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace Roslynator.Formatting.CodeFixes.LineIsTooLong
{
    internal enum SyntaxGroup
    {
        ArrowExpressionClause = 0,
        PropertyInitializer = 1,
        ForStatement = 2,
        ConditionalExpression = 3,
        AttributeList = 4,
        BaseList = 5,
        InitializerExpression = 6,
        ParameterList = 7,
        BinaryExpression = 8,
        MemberExpression = 9,
        ArgumentList = 10,
        Is_As_EqualityExpression = 11,
        AssignmentExpression = 12,
        FieldOrLocalInitializer = 13,
    }
}

