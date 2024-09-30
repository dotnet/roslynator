// Copyright (c) .NET Foundation and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;

namespace Roslynator;

internal static class SymbolUtility
{
    public static bool IsPublicStaticReadOnly(IFieldSymbol fieldSymbol, string? name = null)
    {
        return fieldSymbol?.DeclaredAccessibility == Accessibility.Public
            && fieldSymbol.IsStatic
            && fieldSymbol.IsReadOnly
            && StringUtility.IsNullOrEquals(name, fieldSymbol.Name);
    }

    public static bool IsPublicStaticNonGeneric(IMethodSymbol methodSymbol, string? name = null)
    {
        return methodSymbol?.DeclaredAccessibility == Accessibility.Public
            && methodSymbol.IsStatic
            && !methodSymbol.IsGenericMethod
            && StringUtility.IsNullOrEquals(name, methodSymbol.Name);
    }

    public static bool IsPublicInstanceNonGeneric(IMethodSymbol methodSymbol, string? name = null)
    {
        return methodSymbol?.DeclaredAccessibility == Accessibility.Public
            && !methodSymbol.IsStatic
            && !methodSymbol.IsGenericMethod
            && StringUtility.IsNullOrEquals(name, methodSymbol.Name);
    }

    public static bool IsPublicInstance(IPropertySymbol propertySymbol, string? name = null)
    {
        return propertySymbol?.DeclaredAccessibility == Accessibility.Public
            && !propertySymbol.IsStatic
            && StringUtility.IsNullOrEquals(name, propertySymbol.Name);
    }

    public static bool IsStringAdditionOperator(IMethodSymbol? methodSymbol)
    {
        return methodSymbol?.MethodKind == MethodKind.BuiltinOperator
            && methodSymbol.Name == WellKnownMemberNames.AdditionOperatorName
            && methodSymbol.IsContainingType(SpecialType.System_String);
    }

    public static bool IsEventHandlerMethod(IMethodSymbol methodSymbol, INamedTypeSymbol eventArgsSymbol)
    {
        if (methodSymbol?.ReturnsVoid == true)
        {
            ImmutableArray<IParameterSymbol> parameters = methodSymbol.Parameters;

            if (parameters.Length == 2
                && parameters[0].Type.SpecialType == SpecialType.System_Object)
            {
                ITypeSymbol type = parameters[1].Type;

                if (type.Kind == SymbolKind.TypeParameter)
                    return type.Name.EndsWith("EventArgs", StringComparison.Ordinal);

                return type.EqualsOrInheritsFrom(eventArgsSymbol);
            }
        }

        return false;
    }

    public static bool IsEventHandlerMethod(IMethodSymbol methodSymbol)
    {
        if (methodSymbol?.ReturnsVoid == true)
        {
            ImmutableArray<IParameterSymbol> parameters = methodSymbol.Parameters;

            if (parameters.Length == 2
                && parameters[0].Type.SpecialType == SpecialType.System_Object)
            {
                ITypeSymbol type = parameters[1].Type;

                if (type.Kind == SymbolKind.TypeParameter)
                    return type.Name.EndsWith("EventArgs", StringComparison.Ordinal);

                return type.EqualsOrInheritsFrom(MetadataNames.System_EventArgs)
                    || type.HasMetadataName(MetadataNames.System_Windows_DependencyPropertyChangedEventArgs);
            }
        }

        return false;
    }

    public static bool HasAccessibleIndexer(
        ITypeSymbol typeSymbol,
        SemanticModel semanticModel,
        int position)
    {
        if (typeSymbol is null)
            return false;

        SymbolKind symbolKind = typeSymbol.Kind;

        if (symbolKind == SymbolKind.ErrorType)
            return false;

        if (symbolKind == SymbolKind.ArrayType)
            return true;

        bool? hasIndexer = HasIndexer(typeSymbol.SpecialType);

        if (hasIndexer is not null)
            return hasIndexer.Value;

        ITypeSymbol originalDefinition = typeSymbol.OriginalDefinition;

        if (!SymbolEqualityComparer.Default.Equals(typeSymbol, originalDefinition))
        {
            hasIndexer = HasIndexer(originalDefinition.SpecialType);

            if (hasIndexer is not null)
                return hasIndexer.Value;
        }

        if (originalDefinition.EqualsOrInheritsFrom(MetadataNames.System_Collections_Generic_List_T))
            return true;

        if (originalDefinition.ImplementsAny(
            SpecialType.System_Collections_Generic_IList_T,
            SpecialType.System_Collections_Generic_IReadOnlyList_T,
            allInterfaces: true))
        {
            if (originalDefinition.TypeKind == TypeKind.Interface)
                return true;

            foreach (ISymbol symbol in typeSymbol.GetMembers("this[]"))
            {
                if (semanticModel.IsAccessible(position, symbol))
                    return true;
            }
        }

        return false;

        static bool? HasIndexer(SpecialType specialType)
        {
            switch (specialType)
            {
                case SpecialType.System_String:
                case SpecialType.System_Array:
                case SpecialType.System_Collections_Generic_IList_T:
                case SpecialType.System_Collections_Generic_IReadOnlyList_T:
                    return true;
                case SpecialType.None:
                    return null;
            }

            return false;
        }
    }

    public static string? GetCountOrLengthPropertyName(
        ITypeSymbol typeSymbol,
        SemanticModel semanticModel,
        int position)
    {
        SymbolKind kind = typeSymbol.Kind;

        if (kind == SymbolKind.ErrorType)
            return null;

        if (kind == SymbolKind.ArrayType)
            return "Length";

        string? propertyName = GetCountOrLengthPropertyName(typeSymbol.SpecialType);

        if (propertyName is not null)
            return (propertyName.Length > 0) ? propertyName : null;

        ITypeSymbol originalDefinition = typeSymbol.OriginalDefinition;

        if (!SymbolEqualityComparer.Default.Equals(typeSymbol, originalDefinition))
        {
            propertyName = GetCountOrLengthPropertyName(originalDefinition.SpecialType);

            if (propertyName is not null)
                return (propertyName.Length > 0) ? propertyName : null;
        }

        if (originalDefinition.ImplementsAny(
            SpecialType.System_Collections_Generic_ICollection_T,
            SpecialType.System_Collections_Generic_IReadOnlyCollection_T,
            allInterfaces: true))
        {
            if (originalDefinition.TypeKind == TypeKind.Interface)
                return "Count";

            while (typeSymbol is not null
                && typeSymbol.SpecialType != SpecialType.System_Object)
            {
                foreach (ISymbol symbol in typeSymbol.GetMembers("Count"))
                {
                    if (symbol.Kind == SymbolKind.Property
                        && semanticModel.IsAccessible(position, symbol))
                    {
                        return symbol.Name;
                    }
                }

                foreach (ISymbol symbol in typeSymbol.GetMembers("Length"))
                {
                    if (symbol.Kind == SymbolKind.Property
                        && semanticModel.IsAccessible(position, symbol))
                    {
                        return symbol.Name;
                    }
                }

                typeSymbol = typeSymbol.BaseType!;
            }
        }

        return null;

        static string? GetCountOrLengthPropertyName(SpecialType specialType)
        {
            switch (specialType)
            {
                case SpecialType.System_String:
                case SpecialType.System_Array:
                    return "Length";
                case SpecialType.System_Collections_Generic_IList_T:
                case SpecialType.System_Collections_Generic_ICollection_T:
                case SpecialType.System_Collections_Generic_IReadOnlyList_T:
                case SpecialType.System_Collections_Generic_IReadOnlyCollection_T:
                    return "Count";
                case SpecialType.None:
                    return null;
            }

            return "";
        }
    }

    public static bool IsFunc(ISymbol symbol, ITypeSymbol parameter1, ITypeSymbol parameter2)
    {
        if (!symbol.OriginalDefinition.HasMetadataName(MetadataNames.System_Func_T2))
            return false;

        ImmutableArray<ITypeSymbol> typeArguments = ((INamedTypeSymbol)symbol).TypeArguments;

        return typeArguments.Length == 2
            && SymbolEqualityComparer.Default.Equals(typeArguments[0], parameter1)
            && SymbolEqualityComparer.Default.Equals(typeArguments[1], parameter2);
    }

    public static bool IsPredicateFunc(ISymbol symbol, ITypeSymbol parameter)
    {
        if (!symbol.HasMetadataName(MetadataNames.System_Func_T2))
            return false;

        ImmutableArray<ITypeSymbol> typeArguments = ((INamedTypeSymbol)symbol).TypeArguments;

        return typeArguments.Length == 2
            && SymbolEqualityComparer.Default.Equals(typeArguments[0], parameter)
            && typeArguments[1].SpecialType == SpecialType.System_Boolean;
    }

    public static bool IsPredicateFunc(ISymbol symbol, ITypeSymbol parameter1, ITypeSymbol parameter2)
    {
        if (!symbol.HasMetadataName(MetadataNames.System_Func_T3))
            return false;

        ImmutableArray<ITypeSymbol> typeArguments = ((INamedTypeSymbol)symbol).TypeArguments;

        return typeArguments.Length == 3
            && SymbolEqualityComparer.Default.Equals(typeArguments[0], parameter1)
            && SymbolEqualityComparer.Default.Equals(typeArguments[1], parameter2)
            && typeArguments[2].SpecialType == SpecialType.System_Boolean;
    }

    internal static bool IsPropertyOfNullableOfT(ISymbol? symbol, string name)
    {
        return symbol?.Kind == SymbolKind.Property
            && symbol.ContainingType?.ConstructedFrom.SpecialType == SpecialType.System_Nullable_T
            && string.Equals(symbol.Name, name, StringComparison.Ordinal);
    }

    internal static bool IsLinqExtensionOfIEnumerableOfTWithoutParameters(
        IMethodSymbol methodSymbol,
        string name,
        bool allowImmutableArrayExtension = false)
    {
        return IsLinqExtensionOfIEnumerableOfT(methodSymbol, name, parameterCount: 1, allowImmutableArrayExtension: allowImmutableArrayExtension);
    }

    internal static bool IsLinqElementAt(
        IMethodSymbol methodSymbol,
        bool allowImmutableArrayExtension = false)
    {
        return IsLinqExtensionOfIEnumerableOfT(methodSymbol, "ElementAt", parameterCount: 2, allowImmutableArrayExtension: allowImmutableArrayExtension)
            && methodSymbol.Parameters[1].Type.SpecialType == SpecialType.System_Int32;
    }

    internal static bool IsLinqWhere(
        IMethodSymbol methodSymbol,
        bool allowImmutableArrayExtension = false)
    {
        return IsLinqExtensionOfIEnumerableOfTWithPredicate(methodSymbol, "Where", parameterCount: 2, allowImmutableArrayExtension: allowImmutableArrayExtension);
    }

    internal static bool IsLinqWhereWithIndex(IMethodSymbol methodSymbol)
    {
        if (!IsLinqExtensionOfIEnumerableOfT(methodSymbol, "Where", parameterCount: 2, allowImmutableArrayExtension: false))
            return false;

        ITypeSymbol typeSymbol = methodSymbol.Parameters[1].Type;

        if (!typeSymbol.HasMetadataName(MetadataNames.System_Func_T3))
            return false;

        ImmutableArray<ITypeSymbol> typeArguments = ((INamedTypeSymbol)typeSymbol).TypeArguments;

        return typeArguments.Length == 3
            && SymbolEqualityComparer.Default.Equals(typeArguments[0], methodSymbol.TypeArguments[0])
            && typeArguments[1].SpecialType == SpecialType.System_Int32
            && typeArguments[2].SpecialType == SpecialType.System_Boolean;
    }

    internal static bool IsLinqSelect(IMethodSymbol methodSymbol, bool allowImmutableArrayExtension = false)
    {
        if (methodSymbol.DeclaredAccessibility != Accessibility.Public)
            return false;

        if (methodSymbol.ReturnType.OriginalDefinition.SpecialType != SpecialType.System_Collections_Generic_IEnumerable_T)
            return false;

        if (!methodSymbol.IsName("Select"))
            return false;

        if (methodSymbol.Arity != 2)
            return false;

        INamedTypeSymbol containingType = methodSymbol.ContainingType;

        if (containingType is null)
            return false;

        if (containingType.HasMetadataName(MetadataNames.System_Linq_Enumerable))
        {
            ImmutableArray<IParameterSymbol> parameters = methodSymbol.Parameters;

            return parameters.Length == 2
                && parameters[0].Type.OriginalDefinition.SpecialType == SpecialType.System_Collections_Generic_IEnumerable_T
                && IsFunc(parameters[1].Type, methodSymbol.TypeArguments[0], methodSymbol.TypeArguments[1]);
        }
        else if (allowImmutableArrayExtension
            && containingType.HasMetadataName(MetadataNames.System_Linq_ImmutableArrayExtensions))
        {
            ImmutableArray<IParameterSymbol> parameters = methodSymbol.Parameters;

            return parameters.Length == 2
                && IsImmutableArrayOfT(parameters[0].Type)
                && IsFunc(parameters[1].Type, methodSymbol.TypeArguments[0], methodSymbol.TypeArguments[1]);
        }

        return false;
    }

    internal static bool IsLinqCast(IMethodSymbol methodSymbol)
    {
        return methodSymbol.DeclaredAccessibility == Accessibility.Public
            && methodSymbol.ReturnType.OriginalDefinition.SpecialType == SpecialType.System_Collections_Generic_IEnumerable_T
            && methodSymbol.IsName("Cast")
            && methodSymbol.Arity == 1
            && methodSymbol.HasSingleParameter(SpecialType.System_Collections_IEnumerable)
            && methodSymbol.ContainingType?.HasMetadataName(MetadataNames.System_Linq_Enumerable) == true;
    }

    internal static bool IsLinqOfType(IMethodSymbol methodSymbol)
    {
        return methodSymbol.DeclaredAccessibility == Accessibility.Public
            && methodSymbol.ReturnType.OriginalDefinition.SpecialType == SpecialType.System_Collections_Generic_IEnumerable_T
            && methodSymbol.IsName("OfType")
            && methodSymbol.Arity == 1
            && methodSymbol.HasSingleParameter(SpecialType.System_Collections_IEnumerable)
            && methodSymbol.ContainingType?.HasMetadataName(MetadataNames.System_Linq_Enumerable) == true;
    }

    internal static bool IsLinqExtensionOfIEnumerableOfT(
        IMethodSymbol methodSymbol,
        string? name = null,
        int parameterCount = 1,
        bool allowImmutableArrayExtension = false)
    {
        if (parameterCount < 1)
            throw new ArgumentOutOfRangeException(nameof(parameterCount), parameterCount, "");

        return IsLinqExtensionOfIEnumerableOfT(
            methodSymbol: methodSymbol,
            name: name,
            interval: new Interval(parameterCount, parameterCount),
            allowImmutableArrayExtension: allowImmutableArrayExtension);
    }

    internal static bool IsLinqExtensionOfIEnumerableOfT(
        IMethodSymbol methodSymbol,
        string? name,
        Interval interval,
        bool allowImmutableArrayExtension = false)
    {
        if (interval.Min < 1)
            throw new ArgumentOutOfRangeException(nameof(interval), interval, "");

        if (methodSymbol.DeclaredAccessibility != Accessibility.Public)
            return false;

        if (!StringUtility.IsNullOrEquals(name, methodSymbol.Name))
            return false;

        INamedTypeSymbol containingType = methodSymbol.ContainingType;

        if (containingType is null)
            return false;

        if (containingType.HasMetadataName(MetadataNames.System_Linq_Enumerable))
        {
            ImmutableArray<IParameterSymbol> parameters = methodSymbol.Parameters;

            return interval.Contains(parameters)
                && parameters[0].Type.OriginalDefinition.SpecialType == SpecialType.System_Collections_Generic_IEnumerable_T;
        }
        else if (allowImmutableArrayExtension
            && containingType.HasMetadataName(MetadataNames.System_Linq_ImmutableArrayExtensions))
        {
            ImmutableArray<IParameterSymbol> parameters = methodSymbol.Parameters;

            return interval.Contains(parameters)
                && IsImmutableArrayOfT(parameters[0].Type);
        }

        return false;
    }

    internal static bool IsLinqExtensionOfIEnumerableOfTWithPredicate(
        IMethodSymbol methodSymbol,
        string? name = null,
        bool allowImmutableArrayExtension = false)
    {
        return IsLinqExtensionOfIEnumerableOfTWithPredicate(methodSymbol, name, parameterCount: 2, allowImmutableArrayExtension: allowImmutableArrayExtension);
    }

    private static bool IsLinqExtensionOfIEnumerableOfTWithPredicate(
        IMethodSymbol methodSymbol,
        string? name,
        int parameterCount,
        bool allowImmutableArrayExtension = false)
    {
        if (methodSymbol.DeclaredAccessibility != Accessibility.Public)
            return false;

        if (!StringUtility.IsNullOrEquals(name, methodSymbol.Name))
            return false;

        INamedTypeSymbol containingType = methodSymbol.ContainingType;

        if (containingType is null)
            return false;

        if (containingType.HasMetadataName(MetadataNames.System_Linq_Enumerable))
        {
            ImmutableArray<IParameterSymbol> parameters = methodSymbol.Parameters;

            return parameters.Length == parameterCount
                && parameters[0].Type.OriginalDefinition.SpecialType == SpecialType.System_Collections_Generic_IEnumerable_T
                && IsPredicateFunc(parameters[1].Type, methodSymbol.TypeArguments[0]);
        }
        else if (allowImmutableArrayExtension
            && containingType.HasMetadataName(MetadataNames.System_Linq_ImmutableArrayExtensions))
        {
            ImmutableArray<IParameterSymbol> parameters = methodSymbol.Parameters;

            return parameters.Length == parameterCount
                && IsImmutableArrayOfT(parameters[0].Type)
                && IsPredicateFunc(parameters[1].Type, methodSymbol.TypeArguments[0]);
        }

        return false;
    }

    public static bool IsImmutableArrayOfT(ITypeSymbol typeSymbol)
    {
        return typeSymbol.OriginalDefinition.HasMetadataName(MetadataNames.System_Collections_Immutable_ImmutableArray_T);
    }

    public static bool SupportsSwitchExpression(ITypeSymbol typeSymbol)
    {
        if (typeSymbol.Kind == SymbolKind.ErrorType)
            return false;

        if (typeSymbol.TypeKind == TypeKind.Enum)
            return true;

        switch (typeSymbol.SpecialType)
        {
            case SpecialType.System_Boolean:
            case SpecialType.System_Char:
            case SpecialType.System_SByte:
            case SpecialType.System_Byte:
            case SpecialType.System_Int16:
            case SpecialType.System_UInt16:
            case SpecialType.System_Int32:
            case SpecialType.System_UInt32:
            case SpecialType.System_Int64:
            case SpecialType.System_UInt64:
            case SpecialType.System_Single:
            case SpecialType.System_Double:
            case SpecialType.System_String:
                return true;
        }

        if ((typeSymbol is INamedTypeSymbol namedTypeSymbol)
            && namedTypeSymbol.IsNullableType())
        {
            switch (namedTypeSymbol.TypeArguments[0].SpecialType)
            {
                case SpecialType.System_Boolean:
                case SpecialType.System_Char:
                case SpecialType.System_SByte:
                case SpecialType.System_Byte:
                case SpecialType.System_Int16:
                case SpecialType.System_UInt16:
                case SpecialType.System_Int32:
                case SpecialType.System_UInt32:
                case SpecialType.System_Int64:
                case SpecialType.System_UInt64:
                case SpecialType.System_Single:
                case SpecialType.System_Double:
                    return true;
            }
        }

        return false;
    }

    // https://docs.microsoft.com/cs-cz/dotnet/csharp/programming-guide/main-and-command-args/
    public static bool CanBeEntryPoint(IMethodSymbol methodSymbol)
    {
        if (methodSymbol.IsStatic
            && string.Equals(methodSymbol.Name, "Main", StringComparison.Ordinal)
            && methodSymbol.ContainingType?.TypeKind.Is(TypeKind.Class, TypeKind.Struct) == true
            && !methodSymbol.TypeParameters.Any())
        {
            ImmutableArray<IParameterSymbol> parameters = methodSymbol.Parameters;

            int length = parameters.Length;

            if (length == 0)
                return true;

            if (length == 1)
            {
                IParameterSymbol parameter = parameters[0];

                ITypeSymbol type = parameter.Type;

                if (type.Kind == SymbolKind.ArrayType)
                {
                    var arrayType = (IArrayTypeSymbol)type;

                    if (arrayType.ElementType.SpecialType == SpecialType.System_String)
                        return true;
                }
            }
        }

        return false;
    }

    public static ulong GetEnumValueAsUInt64(object value, INamedTypeSymbol enumType)
    {
        INamedTypeSymbol? enumUnderlyingType = enumType.EnumUnderlyingType;

        if (enumUnderlyingType is null)
            throw new InvalidOperationException($"Type is not an enum: {enumType.ToDisplayString(SymbolDisplayFormats.FullName)}");

        return ConvertHelpers.ConvertToUInt64(value, enumUnderlyingType.SpecialType);
    }

    public static IMethodSymbol? FindMethodThatRaisePropertyChanged(INamedTypeSymbol typeSymbol, int position, SemanticModel semanticModel)
    {
        do
        {
            IMethodSymbol? methodSymbol = FindMethod(typeSymbol.GetMembers("RaisePropertyChanged"))
                ?? FindMethod(typeSymbol.GetMembers("OnPropertyChanged"));

            if (methodSymbol is not null)
                return methodSymbol;

            typeSymbol = typeSymbol.BaseType!;
        }
        while (typeSymbol is not null
            && typeSymbol.SpecialType != SpecialType.System_Object);

        return null;

        IMethodSymbol? FindMethod(ImmutableArray<ISymbol> symbols)
        {
            foreach (ISymbol symbol in symbols)
            {
                if (symbol.Kind == SymbolKind.Method)
                {
                    var methodSymbol = (IMethodSymbol)symbol;

                    if (methodSymbol.Parameters.SingleOrDefault(shouldThrow: false)?.Type.SpecialType == SpecialType.System_String
                        && semanticModel.IsAccessible(position, methodSymbol))
                    {
                        return methodSymbol;
                    }
                }
            }

            return null;
        }
    }

    public static bool IsWellKnownTaskType(this ITypeSymbol typeSymbol)
    {
        return typeSymbol.ContainingNamespace.HasMetadataName(in MetadataNames.System_Threading_Tasks)
            && (typeSymbol.MetadataName is "Task" or "Task`1" or "ValueTask" or "ValueTask`1");
    }

    /// <summary>
    /// Determines if the symbol is an awaitable type (i.e. the <see langword="await"/> keyword can be used on it) or a method that returns one.<br/>
    /// A type is awaitable if it has an instance or extension <c>GetAwaiter</c> method that returns a correctly-shaped awaiter type.
    /// </summary>
    /// <remarks>
    /// For more information, see the <see href="https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/language-specification/expressions#12982-awaitable-expressions">C# language specification</see>.
    /// </remarks>
    /// <param name="semanticModel">Used to determine whether a candidate <c>GetAwaiter</c> method is accessible at the given <paramref name="position"/>.</param>
    /// <param name="position">The position of the token at which the <paramref name="symbol"/> is used.</param>
    /// <returns>A <see cref="bool"/> indicating whether the symbol is an awaitable type or a method that returns one.</returns>
    public static bool IsAwaitable(this ISymbol? symbol, SemanticModel semanticModel, int position)
    {
        ITypeSymbol? typeSymbol = (symbol as ITypeSymbol) ?? (symbol as IMethodSymbol)?.ReturnType;

        if (typeSymbol is null or { SpecialType: SpecialType.System_Void })
            return false;

        if (typeSymbol.IsWellKnownTaskType())
            return true;

        return HasAwaitableShape(typeSymbol, semanticModel, position);
    }

    private static bool HasAwaitableShape(ITypeSymbol typeSymbol, SemanticModel semanticModel, int position)
    {
        // this is the same check as in Roslyn, reimplemented due to it being internal
        // https://github.com/dotnet/roslyn/blob/a182892bf997a457cfcdbece5352e1a139eb2a12/src/Compilers/CSharp/Portable/Binder/Binder_Await.cs#L281-L289

        IMethodSymbol? getAwaiter = semanticModel.LookupSymbols(position, typeSymbol, WellKnownMemberNames.GetAwaiter, includeReducedExtensionMethods: true)
            .OfType<IMethodSymbol>()
            .FirstOrDefault();

        if (getAwaiter is not { Parameters.Length: 0 })
            return false;

        var awaiterTypeDefinition = getAwaiter.ReturnType.OriginalDefinition as INamedTypeSymbol;
        if (awaiterTypeDefinition is not { SpecialType: SpecialType.None })
            return false;

        // bool IsCompleted { get; }
        IPropertySymbol? isCompletedProp = semanticModel.LookupSymbols(position, awaiterTypeDefinition, WellKnownMemberNames.IsCompleted)
            .OfType<IPropertySymbol>()
            .FirstOrDefault();

        if (isCompletedProp is not { Type.SpecialType: SpecialType.System_Boolean, GetMethod: not null })
            return false;

        if (!semanticModel.IsAccessible(position, isCompletedProp.GetMethod))
            return false;

        // implements INotifyCompletion
        if (!awaiterTypeDefinition.Implements(MetadataNames.System_Runtime_CompilerServices_INotifyCompletion, allInterfaces: true))
            return false;

        // void GetResult() || T GetResult()
        // must be an instance method
        IMethodSymbol? getResultMethod = semanticModel.LookupSymbols(position, awaiterTypeDefinition, WellKnownMemberNames.GetResult)
            .OfType<IMethodSymbol>()
            .FirstOrDefault();
        return getResultMethod is { Parameters.Length: 0, TypeParameters.Length: 0 };
    }

    internal static INamedTypeSymbol? GetPossiblyAwaitableType(ITypeSymbol typeSymbol)
    {
        if (typeSymbol.Kind == SymbolKind.TypeParameter)
        {
            var typeParameterSymbol = (ITypeParameterSymbol)typeSymbol;

            typeSymbol = typeParameterSymbol.ConstraintTypes.SingleOrDefault(f => f.TypeKind == TypeKind.Class, shouldThrow: false)!;

            if (typeSymbol is null)
                return null;
        }

        if (typeSymbol.IsTupleType)
            return null;

        if (typeSymbol.SpecialType != SpecialType.None)
            return null;

        if (!typeSymbol.TypeKind.Is(TypeKind.Class, TypeKind.Struct, TypeKind.Interface))
            return null;

        return typeSymbol as INamedTypeSymbol;
    }

    /// <summary>
    /// Determines if a type:
    /// <list type="bullet">
    /// <item>
    /// Is a task type - that is, <see cref="Task"/>, <see cref="Task{TResult}"/>, <see cref="ValueTask"/>, <see cref="ValueTask{TResult}"/>, or a user-implemented <see href="https://github.com/dotnet/roslyn/blob/main/docs/features/task-types.md">task-like type</see>.<br/>
    /// Only task types (and <see langword="void"/>) can be the return type of an <see langword="async"/> method.
    /// </item>
    /// <item>Can be awaited with the <see langword="await"/> keyword (see <see cref="IsAwaitable"/>).</item>
    /// </list>
    /// </summary>
    /// <remarks>
    /// For more information on task types, see the <see href="https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/language-specification/classes#15151-general">C# language specification</see>.
    /// </remarks>
    public static bool IsAwaitableTaskType(this ITypeSymbol typeSymbol, SemanticModel semanticModel, int position)
    {
        if (typeSymbol.OriginalDefinition is not INamedTypeSymbol definition)
            return false;

        if (definition.IsWellKnownTaskType())
            return true;

        // Roslyn checks arity, see https://github.com/dotnet/roslyn/blob/c4203b867e9c0287cabb0a0674bfca096c08fd3e/src/Compilers/CSharp/Portable/Symbols/TypeSymbolExtensions.cs#L1844
        if (definition.Arity > 1)
            return false;

        // Task (and Task<T>) are hardcoded and don't have an AsyncMethodBuilder attribute.
        // see https://github.com/dotnet/roslyn/blob/c4203b867e9c0287cabb0a0674bfca096c08fd3e/src/Compilers/CSharp/Portable/Symbols/TypeSymbolExtensions.cs#L1778-L1806
        bool isTaskLike = definition.HasAttribute(in MetadataNames.System_Runtime_CompilerServices_AsyncMethodBuilderAttribute);

        return isTaskLike && HasAwaitableShape(definition, semanticModel, position);
    }
}
