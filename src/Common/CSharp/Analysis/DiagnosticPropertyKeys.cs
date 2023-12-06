// Copyright (c) .NET Foundation and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace Roslynator.CSharp.Analysis;

internal static class DiagnosticPropertyKeys
{
    internal static readonly string UseCollectionExpression = nameof(UseCollectionExpression);
    internal static readonly string ImplicitToCollectionExpression = nameof(ImplicitToCollectionExpression);
    internal static readonly string CollectionExpressionToImplicit = nameof(CollectionExpressionToImplicit);
    internal static readonly string ExplicitToCollectionExpression = nameof(ExplicitToCollectionExpression);
}
