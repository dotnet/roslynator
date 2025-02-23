﻿// Copyright (c) .NET Foundation and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Roslynator.FindSymbols;

namespace Roslynator.Documentation;

internal abstract class SymbolDefinitionWriter : IDisposable
{
    private bool _disposed;

    private SymbolDisplayFormat _definitionFormat;
    private readonly SymbolDisplayFormat _definitionNameFormat;

    protected SymbolDefinitionWriter(
        SymbolFilterOptions filter = null,
        DefinitionListFormat format = null,
        SymbolDocumentationProvider documentationProvider = null,
        INamedTypeSymbol hierarchyRoot = null)
    {
        Filter = filter ?? SymbolFilterOptions.Default;
        Format = format ?? DefinitionListFormat.Default;
        DocumentationProvider = documentationProvider;
        HierarchyRoot = hierarchyRoot;

        _definitionNameFormat = new SymbolDisplayFormat(typeQualificationStyle: SymbolDisplayTypeQualificationStyle.NameOnly);
    }

    public SymbolFilterOptions Filter { get; }

    public DefinitionListFormat Format { get; }

    public SymbolDocumentationProvider DocumentationProvider { get; }

    public INamedTypeSymbol HierarchyRoot { get; }

    public SymbolDefinitionComparer Comparer
    {
        get
        {
            return (Format.Includes(SymbolDefinitionPartFilter.ContainingNamespace))
                ? SymbolDefinitionComparer.SystemFirst
                : SymbolDefinitionComparer.SystemFirstOmitContainingNamespace;
        }
    }

    public abstract bool SupportsDocumentationComments { get; }

    public abstract bool SupportsMultilineDefinitions { get; }

    protected int Depth { get; private set; }

    private protected ImmutableHashSet<INamedTypeSymbol> TypeSymbols { get; private set; }

    private protected List<ISymbol> SymbolHierarchy { get; } = [];

    internal SymbolDefinitionListLayout Layout => Format.Layout;

    public SymbolDisplayFormat DefinitionFormat
    {
        get
        {
            if (_definitionFormat is null)
            {
                var format = new SymbolDisplayFormat(
                    globalNamespaceStyle: SymbolDisplayGlobalNamespaceStyle.Included,
                    typeQualificationStyle: SymbolDisplayTypeQualificationStyle.NameAndContainingTypesAndNamespaces,
                    genericsOptions: SymbolDisplayGenericsOptions.IncludeTypeParameters
                        | SymbolDisplayGenericsOptions.IncludeTypeConstraints
                        | SymbolDisplayGenericsOptions.IncludeVariance,
                    memberOptions: SymbolDisplayMemberOptions.IncludeType
                        | SymbolDisplayMemberOptions.IncludeModifiers
                        | SymbolDisplayMemberOptions.IncludeAccessibility
                        | SymbolDisplayMemberOptions.IncludeContainingType
                        | SymbolDisplayMemberOptions.IncludeParameters
                        | SymbolDisplayMemberOptions.IncludeConstantValue
                        | SymbolDisplayMemberOptions.IncludeRef,
                    delegateStyle: SymbolDisplayDelegateStyle.NameAndSignature,
                    parameterOptions: SymbolDisplayParameterOptions.IncludeExtensionThis
                        | SymbolDisplayParameterOptions.IncludeParamsRefOut
                        | SymbolDisplayParameterOptions.IncludeType
                        | SymbolDisplayParameterOptions.IncludeName
                        | SymbolDisplayParameterOptions.IncludeDefaultValue,
                    propertyStyle: SymbolDisplayPropertyStyle.ShowReadWriteDescriptor,
                    kindOptions: SymbolDisplayKindOptions.IncludeNamespaceKeyword
                        | SymbolDisplayKindOptions.IncludeTypeKeyword
                        | SymbolDisplayKindOptions.IncludeMemberKeyword,
                    miscellaneousOptions: SymbolDisplayMiscellaneousOptions.EscapeKeywordIdentifiers
                        | SymbolDisplayMiscellaneousOptions.IncludeNullableReferenceTypeModifier);

                format = Format.Update(format);

                Interlocked.CompareExchange(ref _definitionFormat, CreateDefinitionFormat(format), null);
            }

            return _definitionFormat;
        }
    }

    protected void IncreaseDepth()
    {
        Depth++;
    }

    protected void DecreaseDepth()
    {
        if (Depth == 0)
            throw new InvalidOperationException("Cannot decrease depth.");

        Depth--;
    }

    public abstract void WriteStartDocument();

    public abstract void WriteEndDocument();

    public abstract void WriteStartAssemblies();

    public abstract void WriteEndAssemblies();

    public abstract void WriteStartAssembly(IAssemblySymbol assemblySymbol);

    public abstract void WriteAssemblyDefinition(IAssemblySymbol assemblySymbol);

    public abstract void WriteEndAssembly(IAssemblySymbol assemblySymbol);

    public abstract void WriteAssemblySeparator();

    public abstract void WriteStartNamespaces();

    public abstract void WriteEndNamespaces();

    public abstract void WriteStartNamespace(INamespaceSymbol namespaceSymbol);

    public abstract void WriteNamespaceDefinition(INamespaceSymbol namespaceSymbol, SymbolDisplayFormat format = null);

    public abstract void WriteEndNamespace(INamespaceSymbol namespaceSymbol);

    public abstract void WriteNamespaceSeparator();

    public abstract void WriteStartTypes();

    public abstract void WriteEndTypes();

    public abstract void WriteStartType(INamedTypeSymbol typeSymbol);

    public abstract void WriteTypeDefinition(INamedTypeSymbol typeSymbol, SymbolDisplayFormat format = null);

    public abstract void WriteEndType(INamedTypeSymbol typeSymbol);

    public abstract void WriteTypeSeparator();

    public abstract void WriteStartMembers();

    public abstract void WriteEndMembers();

    public abstract void WriteStartMember(ISymbol symbol);

    public abstract void WriteMemberDefinition(ISymbol symbol, SymbolDisplayFormat format = null);

    public abstract void WriteEndMember(ISymbol symbol);

    public abstract void WriteMemberSeparator();

    public abstract void WriteStartEnumMembers();

    public abstract void WriteEndEnumMembers();

    public abstract void WriteStartEnumMember(ISymbol symbol);

    public abstract void WriteEnumMemberDefinition(ISymbol symbol, SymbolDisplayFormat format = null);

    public abstract void WriteEndEnumMember(ISymbol symbol);

    public abstract void WriteEnumMemberSeparator();

    public abstract void WriteStartAttributes(ISymbol symbol);

    public abstract void WriteEndAttributes(ISymbol symbol);

    public abstract void WriteStartAttribute(AttributeData attribute, ISymbol symbol);

    public abstract void WriteEndAttribute(AttributeData attribute, ISymbol symbol);

    public abstract void WriteAttributeSeparator(ISymbol symbol);

    public abstract void WriteDocumentationComment(ISymbol symbol);

    protected virtual SymbolDisplayTypeDeclarationOptions GetTypeDeclarationOptions()
    {
        var options = SymbolDisplayTypeDeclarationOptions.None;

        if (Format.Includes(SymbolDefinitionPartFilter.Accessibility))
            options |= SymbolDisplayTypeDeclarationOptions.IncludeAccessibility;

        if (Format.Includes(SymbolDefinitionPartFilter.Modifiers))
            options |= SymbolDisplayTypeDeclarationOptions.IncludeModifiers;

        if (Format.Includes(SymbolDefinitionPartFilter.BaseType))
            options |= SymbolDisplayTypeDeclarationOptions.BaseType;

        if (Format.Includes(SymbolDefinitionPartFilter.BaseInterfaces))
            options |= SymbolDisplayTypeDeclarationOptions.Interfaces;

        return options;
    }

    protected virtual SymbolDisplayAdditionalOptions GetAdditionalOptions()
    {
        var options = SymbolDisplayAdditionalOptions.None;

        if (Format.Includes(SymbolDefinitionPartFilter.Attributes))
            options |= SymbolDisplayAdditionalOptions.IncludeParameterAttributes | SymbolDisplayAdditionalOptions.IncludeAccessorAttributes;

        if (Format.Includes(SymbolDefinitionPartFilter.AttributeArguments))
            options |= SymbolDisplayAdditionalOptions.IncludeAttributeArguments;

        if (SupportsMultilineDefinitions)
        {
            if (Format.Includes(WrapListOptions.BaseTypes))
                options |= SymbolDisplayAdditionalOptions.WrapBaseTypes;

            if (Format.Includes(WrapListOptions.Constraints))
                options |= SymbolDisplayAdditionalOptions.WrapConstraints;

            if (Format.Includes(WrapListOptions.Parameters))
                options |= SymbolDisplayAdditionalOptions.FormatParameters;

            if (Format.Includes(WrapListOptions.Attributes))
                options |= SymbolDisplayAdditionalOptions.FormatAttributes;
        }

        if (Format.OmitIEnumerable)
            options |= SymbolDisplayAdditionalOptions.OmitIEnumerable;

        if (Format.Includes(SymbolDefinitionPartFilter.TrailingSemicolon))
            options |= SymbolDisplayAdditionalOptions.IncludeTrailingSemicolon;

        return options;
    }

    protected virtual SymbolDisplayFormat CreateDefinitionFormat(SymbolDisplayFormat format)
    {
        return format;
    }

    public virtual void WriteDocument(IEnumerable<IAssemblySymbol> assemblies, CancellationToken cancellationToken = default)
    {
        TypeSymbols = assemblies.SelectMany(a => a.GetTypes(f => Filter.IsMatch(f))).ToImmutableHashSet();

        WriteStartDocument();

        if (Format.Includes(SymbolDefinitionPartFilter.Assemblies))
            WriteAssemblies(assemblies, cancellationToken);

        if (!Format.GroupByAssembly)
        {
            if (Layout == SymbolDefinitionListLayout.TypeHierarchy)
            {
                WriteTypeHierarchy(TypeSymbols, cancellationToken);
            }
            else
            {
                WriteNamespaces(new OneOrMany<IAssemblySymbol>(assemblies.ToImmutableArray()), cancellationToken);
            }
        }

        WriteEndDocument();

        TypeSymbols = default;
    }

    private void WriteAssemblies(IEnumerable<IAssemblySymbol> assemblies, CancellationToken cancellationToken = default)
    {
        WriteStartAssemblies();

        using (IEnumerator<IAssemblySymbol> en = assemblies
            .OrderBy(f => f.Name)
            .ThenBy(f => f.Identity.Version)
            .GetEnumerator())
        {
            if (en.MoveNext())
            {
                while (true)
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    IAssemblySymbol assembly = en.Current;

                    WriteStartAssembly(assembly);
                    WriteAssemblyDefinition(assembly);

                    if (Format.GroupByAssembly)
                    {
                        if (Layout == SymbolDefinitionListLayout.TypeHierarchy)
                        {
                            WriteTypeHierarchy(assembly.GetTypes(f => Filter.IsMatch(f)), cancellationToken);
                        }
                        else
                        {
                            WriteNamespaces(new OneOrMany<IAssemblySymbol>(assembly), cancellationToken);
                        }
                    }

                    WriteEndAssembly(assembly);

                    if (en.MoveNext())
                    {
                        WriteAssemblySeparator();
                    }
                    else
                    {
                        break;
                    }
                }
            }
        }

        WriteEndAssemblies();
    }

    private void WriteNamespaces(in OneOrMany<IAssemblySymbol> assemblies, CancellationToken cancellationToken = default)
    {
        Dictionary<INamespaceSymbol, IEnumerable<INamedTypeSymbol>> typesByNamespace = null;

        if (Filter.SymbolGroups == SymbolGroupFilter.None)
        {
            typesByNamespace = assemblies
                .SelectMany(a => a.GetNamespaces(n => !n.IsGlobalNamespace && Filter.IsMatch(n)))
                .Distinct(MetadataNameEqualityComparer<INamespaceSymbol>.Instance)
                .ToDictionary(f => f, _ => ImmutableArray<INamedTypeSymbol>.Empty.AsEnumerable());
        }
        else
        {
            typesByNamespace = assemblies
                .SelectMany(a => a.GetTypes(t => t.ContainingType is null && Filter.IsMatch(t)))
                .GroupBy(t => t.ContainingNamespace, MetadataNameEqualityComparer<INamespaceSymbol>.Instance)
                .Where(g => Filter.IsMatch(g.Key))
                .OrderBy(g => g.Key, Comparer.NamespaceComparer)
                .ToDictionary(f => f.Key, f => f.AsEnumerable());
        }

        WriteStartNamespaces();

        if (Layout == SymbolDefinitionListLayout.NamespaceHierarchy)
        {
            WriteNamespaceHierarchy(typesByNamespace, cancellationToken);
        }
        else
        {
            Debug.Assert(Layout == SymbolDefinitionListLayout.NamespaceList, Layout.ToString());

            WriteNamespaces(typesByNamespace, cancellationToken);
        }

        WriteEndNamespaces();
    }

    private void WriteTypeHierarchy(IEnumerable<INamedTypeSymbol> types, CancellationToken cancellationToken = default)
    {
        IComparer<INamedTypeSymbol> comparer = (Format.Includes(SymbolDefinitionPartFilter.ContainingNamespaceInTypeHierarchy))
            ? SymbolDefinitionComparer.SystemFirst.TypeComparer
            : SymbolDefinitionComparer.OmitContainingNamespace.TypeComparer;

        TypeHierarchy hierarchy = TypeHierarchy.Create(types, HierarchyRoot, comparer);

        WriteTypeHierarchy(hierarchy, cancellationToken);
    }

    private void WriteTypeHierarchy(TypeHierarchy hierarchy, CancellationToken cancellationToken = default)
    {
        WriteStartTypes();
        WriteTypeHierarchyItem(hierarchy.Root, cancellationToken);

        ImmutableArray<TypeHierarchyItem>.Enumerator en = hierarchy.Interfaces.GetEnumerator();

        if (en.MoveNext())
        {
            INamedTypeSymbol symbol = hierarchy.InterfaceRoot.Symbol;

            WriteStartType(symbol);
            WriteTypeDefinition(symbol);
            WriteStartHierarchyTypes();

            do
            {
                WriteTypeHierarchyItem(en.Current, cancellationToken);
            }
            while (en.MoveNext());

            WriteEndHierarchyTypes();
            WriteEndType(symbol);
        }

        WriteEndTypes();
    }

    internal virtual void WriteTypeHierarchyItem(TypeHierarchyItem item, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        WriteStartType(item.Symbol);
        WriteTypeDefinition(item.Symbol);

        if (!item.IsExternal)
            WriteMembers(item.Symbol);

        if (item.HasChildren)
        {
            SymbolHierarchy.Add(item.Symbol);
            WriteStartHierarchyTypes();

            foreach (TypeHierarchyItem child in item.Children())
            {
                WriteTypeSeparator();
                WriteTypeHierarchyItem(child, cancellationToken);
            }

            WriteEndHierarchyTypes();
            SymbolHierarchy.RemoveAt(SymbolHierarchy.Count - 1);
        }

        WriteEndType(item.Symbol);
    }

    protected virtual void WriteStartHierarchyTypes()
    {
    }

    protected virtual void WriteEndHierarchyTypes()
    {
    }

    private void WriteNamespaces(Dictionary<INamespaceSymbol, IEnumerable<INamedTypeSymbol>> typesByNamespace, CancellationToken cancellationToken = default)
    {
        using (Dictionary<INamespaceSymbol, IEnumerable<INamedTypeSymbol>>.Enumerator en = typesByNamespace.GetEnumerator())
        {
            if (en.MoveNext())
            {
                while (true)
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    INamespaceSymbol namespaceSymbol = en.Current.Key;

                    SymbolHierarchy.Add(namespaceSymbol);

                    WriteStartNamespace(namespaceSymbol);
                    WriteNamespaceDefinition(namespaceSymbol);

                    if (Filter.Includes(SymbolGroupFilter.Type))
                        WriteTypes(en.Current.Value, cancellationToken);

                    WriteEndNamespace(namespaceSymbol);

                    SymbolHierarchy.RemoveAt(SymbolHierarchy.Count - 1);

                    if (en.MoveNext())
                    {
                        WriteNamespaceSeparator();
                    }
                    else
                    {
                        break;
                    }
                }
            }
        }
    }

    private void WriteNamespaceHierarchy(Dictionary<INamespaceSymbol, IEnumerable<INamedTypeSymbol>> typesByNamespace, CancellationToken cancellationToken = default)
    {
        var rootNamespaces = new HashSet<INamespaceSymbol>(MetadataNameEqualityComparer<INamespaceSymbol>.Instance);

        var nestedNamespaces = new HashSet<INamespaceSymbol>(MetadataNameEqualityComparer<INamespaceSymbol>.Instance);

        foreach (INamespaceSymbol namespaceSymbol in typesByNamespace.Select(f => f.Key))
        {
            if (namespaceSymbol.IsGlobalNamespace)
            {
                rootNamespaces.Add(namespaceSymbol);
            }
            else
            {
                INamespaceSymbol n = namespaceSymbol;

                while (true)
                {
                    INamespaceSymbol containingNamespace = n.ContainingNamespace;

                    if (containingNamespace.IsGlobalNamespace)
                    {
                        rootNamespaces.Add(n);
                        break;
                    }

                    nestedNamespaces.Add(n);

                    n = containingNamespace;
                }
            }
        }

        using (IEnumerator<INamespaceSymbol> en = rootNamespaces
            .OrderBy(f => f, Comparer.NamespaceComparer)
            .GetEnumerator())
        {
            if (en.MoveNext())
            {
                while (true)
                {
                    WriteNamespaceWithHierarchy(en.Current);

                    if (en.MoveNext())
                    {
                        WriteNamespaceSeparator();
                    }
                    else
                    {
                        break;
                    }
                }
            }
        }

        void WriteNamespaceWithHierarchy(INamespaceSymbol namespaceSymbol)
        {
            SymbolHierarchy.Add(namespaceSymbol);

            cancellationToken.ThrowIfCancellationRequested();

            WriteNamespaceDefinition(namespaceSymbol);

            if (Filter.Includes(SymbolGroupFilter.Type))
                WriteTypes(typesByNamespace[namespaceSymbol], cancellationToken);

            using (List<INamespaceSymbol>.Enumerator en = nestedNamespaces
                .Where(f => MetadataNameEqualityComparer<INamespaceSymbol>.Instance.Equals(f.ContainingNamespace, namespaceSymbol))
                .Distinct(MetadataNameEqualityComparer<INamespaceSymbol>.Instance)
                .OrderBy(f => f, Comparer.NamespaceComparer)
                .ToList()
                .GetEnumerator())
            {
                if (en.MoveNext())
                {
                    while (true)
                    {
                        nestedNamespaces.Remove(en.Current);

                        WriteNamespaceWithHierarchy(en.Current);

                        if (en.MoveNext())
                        {
                            WriteNamespaceSeparator();
                        }
                        else
                        {
                            break;
                        }
                    }
                }
            }

            WriteEndNamespace(namespaceSymbol);

            SymbolHierarchy.RemoveAt(SymbolHierarchy.Count - 1);
        }
    }

    private void WriteTypes(IEnumerable<INamedTypeSymbol> types, CancellationToken cancellationToken = default)
    {
        using (IEnumerator<INamedTypeSymbol> en = types
            .OrderBy(f => f, Comparer.TypeComparer)
            .GetEnumerator())
        {
            if (en.MoveNext())
            {
                WriteStartTypes();

                while (true)
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    INamedTypeSymbol type = en.Current;

                    SymbolHierarchy.Add(type);
                    WriteStartType(type);
                    WriteTypeDefinition(type);
                    WriteMembers(type);
                    WriteEndType(type);
                    SymbolHierarchy.RemoveAt(SymbolHierarchy.Count - 1);

                    if (en.MoveNext())
                    {
                        WriteTypeSeparator();
                    }
                    else
                    {
                        break;
                    }
                }

                WriteEndTypes();
            }
        }
    }

    internal void WriteMembers(INamedTypeSymbol type)
    {
        switch (type.TypeKind)
        {
            case TypeKind.Class:
            case TypeKind.Interface:
            case TypeKind.Struct:
            {
                if (Filter.Includes(SymbolGroupFilter.Member))
                    WriteMembers();

                if (Layout != SymbolDefinitionListLayout.TypeHierarchy
                    && (Filter.Includes(SymbolGroupFilter.Type)))
                {
                    WriteTypes(type.GetTypeMembers().Where(f => Filter.IsMatch(f)));
                }

                break;
            }
            case TypeKind.Enum:
            {
                if (Filter.Includes(SymbolGroupFilter.EnumField))
                    WriteEnumMembers();

                break;
            }
            default:
            {
                Debug.Assert(type.TypeKind == TypeKind.Delegate, type.TypeKind.ToString());
                break;
            }
        }

        void WriteMembers()
        {
            using (IEnumerator<ISymbol> en = type
                .GetMembers()
                .Where(f => !f.IsKind(SymbolKind.NamedType) && Filter.IsMatch(f))
                .OrderBy(f => f, Comparer.MemberComparer)
                .GetEnumerator())
            {
                if (en.MoveNext())
                {
                    WriteStartMembers();

                    ISymbol symbol = en.Current;

                    while (true)
                    {
                        WriteStartMember(symbol);
                        WriteMemberDefinition(symbol);
                        WriteEndMember(symbol);

                        if (en.MoveNext())
                        {
                            ISymbol next = en.Current;

                            if (Format.EmptyLineBetweenMembers
                                || (Format.EmptyLineBetweenMemberGroups && symbol.GetMemberDeclarationKind() != next.GetMemberDeclarationKind()))
                            {
                                WriteMemberSeparator();
                            }

                            symbol = next;
                        }
                        else
                        {
                            break;
                        }
                    }

                    WriteEndMembers();
                }
            }
        }

        void WriteEnumMembers()
        {
            using (IEnumerator<ISymbol> en = type
                .GetMembers()
                .Where(m => m.Kind == SymbolKind.Field && Filter.IsMatch((IFieldSymbol)m))
                .GetEnumerator())
            {
                if (en.MoveNext())
                {
                    WriteStartEnumMembers();

                    while (true)
                    {
                        WriteStartEnumMember(en.Current);
                        WriteEnumMemberDefinition(en.Current);
                        WriteEndEnumMember(en.Current);

                        if (en.MoveNext())
                        {
                            WriteEnumMemberSeparator();
                        }
                        else
                        {
                            break;
                        }
                    }

                    WriteEndEnumMembers();
                }
            }
        }
    }

    public void WriteAttributes(ISymbol symbol)
    {
        using (IEnumerator<AttributeData> en = symbol
            .GetAttributes()
            .Where(f => Filter.IsMatch(symbol, f))
            .OrderBy(f => f.AttributeClass, Comparer.TypeComparer)
            .GetEnumerator())
        {
            if (en.MoveNext())
            {
                WriteStartAttributes(symbol);

                while (true)
                {
                    WriteStartAttribute(en.Current, symbol);
                    WriteAttribute(en.Current);
                    WriteEndAttribute(en.Current, symbol);

                    if (en.MoveNext())
                    {
                        WriteAttributeSeparator(symbol);
                    }
                    else
                    {
                        break;
                    }
                }

                WriteEndAttributes(symbol);
            }
        }
    }

    protected virtual void WriteAttributeName(ISymbol symbol)
    {
        ImmutableArray<SymbolDisplayPart> parts = symbol.ToDisplayParts(_definitionNameFormat);

        parts = SymbolDefinitionWriterHelpers.RemoveAttributeSuffix(symbol, parts);

        Write(parts);
    }

    public virtual void WriteAttribute(AttributeData attribute)
    {
        INamedTypeSymbol attributeSymbol = attribute.AttributeClass;

        if (attributeSymbol.ContainingType is not null
            || (Format.Includes(SymbolDefinitionPartFilter.ContainingNamespace)
                && !attributeSymbol.ContainingNamespace.IsGlobalNamespace))
        {
            ISymbol symbol = attributeSymbol.ContainingType ?? (ISymbol)attributeSymbol.ContainingNamespace;

            SymbolDisplayFormat format = TypeSymbolDisplayFormats.Name_ContainingTypes_Namespaces_GlobalNamespace_TypeParameters_SpecialTypes;

            ImmutableArray<SymbolDisplayPart> parts = symbol.ToDisplayParts(format);

            WriteParts(symbol, parts);
            Write(".");
        }

        WriteAttributeName(attributeSymbol);

        if (!Format.Includes(SymbolDefinitionPartFilter.AttributeArguments))
            return;

        var hasConstructorArgument = false;
        var hasNamedArgument = false;

        WriteConstructorArguments();
        WriteNamedArguments();

        if (hasConstructorArgument || hasNamedArgument)
        {
            Write(")");
        }

        void WriteConstructorArguments()
        {
            ImmutableArray<TypedConstant>.Enumerator en = attribute.ConstructorArguments.GetEnumerator();

            if (en.MoveNext())
            {
                hasConstructorArgument = true;
                Write("(");

                while (true)
                {
                    AddConstantValue(en.Current);

                    if (en.MoveNext())
                    {
                        Write(", ");
                    }
                    else
                    {
                        break;
                    }
                }
            }
        }

        void WriteNamedArguments()
        {
            ImmutableArray<KeyValuePair<string, TypedConstant>>.Enumerator en = attribute.NamedArguments.GetEnumerator();

            if (en.MoveNext())
            {
                hasNamedArgument = true;

                if (hasConstructorArgument)
                {
                    Write(", ");
                }
                else
                {
                    Write("(");
                }

                while (true)
                {
                    Write(en.Current.Key);
                    Write(" = ");
                    AddConstantValue(en.Current.Value);

                    if (en.MoveNext())
                    {
                        Write(", ");
                    }
                    else
                    {
                        break;
                    }
                }
            }
        }

        void AddConstantValue(TypedConstant typedConstant)
        {
            switch (typedConstant.Kind)
            {
                case TypedConstantKind.Primitive:
                {
                    Write(SymbolDisplay.FormatPrimitive(typedConstant.Value, quoteStrings: true, useHexadecimalNumbers: false));
                    break;
                }
                case TypedConstantKind.Enum:
                {
                    OneOrMany<EnumFieldSymbolInfo> oneOrMany = EnumUtility.GetConstituentFields(typedConstant.Value, (INamedTypeSymbol)typedConstant.Type);

                    OneOrMany<EnumFieldSymbolInfo>.Enumerator en = oneOrMany.GetEnumerator();

                    if (en.MoveNext())
                    {
                        while (true)
                        {
                            WriteSymbol(en.Current.Symbol.ContainingType);
                            Write(".");
                            Write(en.Current.Symbol.Name);

                            if (en.MoveNext())
                            {
                                Write(" | ");
                            }
                            else
                            {
                                break;
                            }
                        }
                    }
                    else
                    {
                        Write("(");
                        WriteSymbol((INamedTypeSymbol)typedConstant.Type);
                        Write(")");
                        Write(typedConstant.Value.ToString());
                    }

                    break;
                }
                case TypedConstantKind.Type:
                {
                    if (typedConstant.Value is null)
                    {
                        Write("null");
                    }
                    else
                    {
                        Write("typeof");
                        Write("(");
                        WriteSymbol((ISymbol)typedConstant.Value);
                        Write(")");
                    }

                    break;
                }
                case TypedConstantKind.Array:
                {
                    var arrayType = (IArrayTypeSymbol)typedConstant.Type;

                    Write("new ");
                    WriteSymbol(arrayType.ElementType);

                    Write("[] { ");

                    ImmutableArray<TypedConstant>.Enumerator en = typedConstant.Values.GetEnumerator();

                    if (en.MoveNext())
                    {
                        while (true)
                        {
                            AddConstantValue(en.Current);

                            if (en.MoveNext())
                            {
                                Write(", ");
                            }
                            else
                            {
                                break;
                            }
                        }
                    }

                    Write(" }");
                    break;
                }
                default:
                {
                    throw new InvalidOperationException();
                }
            }
        }

        void WriteSymbol(ISymbol symbol)
        {
            SymbolDisplayFormat format = TypeSymbolDisplayFormats.Name_ContainingTypes_Namespaces_GlobalNamespace_TypeParameters_SpecialTypes;

            WriteParts(symbol, symbol.ToDisplayParts(format));
        }
    }

    public virtual void WriteDefinition(ISymbol symbol, SymbolDisplayFormat format)
    {
        ImmutableArray<SymbolDisplayPart> parts = SymbolDefinitionDisplay.GetDisplayParts(
            symbol,
            format ?? DefinitionFormat,
            typeDeclarationOptions: GetTypeDeclarationOptions(),
            additionalOptions: GetAdditionalOptions(),
            shouldDisplayAttribute: (s, a) => Filter.IsMatch(s, a));

        WriteDefinition(symbol, parts);
    }

    public virtual void WriteDefinition(ISymbol symbol, ImmutableArray<SymbolDisplayPart> parts)
    {
        (int startIndex, int endIndex, ISymbol s) = SymbolDefinitionWriterHelpers.FindDefinitionName(symbol, parts);

        Debug.Assert(startIndex >= 0 || (symbol.IsKind(SymbolKind.Namespace) && ((INamespaceSymbol)symbol).IsGlobalNamespace), parts.ToDisplayString());

        if (startIndex >= 0)
        {
            Debug.Assert(SymbolEqualityComparer.Default.Equals(symbol, s), parts.ToDisplayString());

            WriteParts(symbol, parts, 0, startIndex);

            ISymbol explicitImplementation = symbol.GetFirstExplicitInterfaceImplementation();

            if (explicitImplementation is not null)
            {
                INamedTypeSymbol containingType = explicitImplementation.ContainingType;

                if (containingType is not null)
                {
                    WriteParts(containingType, containingType.ToDisplayParts(TypeSymbolDisplayFormats.Name_ContainingTypes_Namespaces_GlobalNamespace_TypeParameters));
                    Write(".");
                }
            }

            WriteDefinitionName(s);

            startIndex = endIndex + 1;
        }
        else
        {
            startIndex = 0;
        }

        WriteParts(symbol, parts, startIndex, parts.Length - startIndex);
    }

    protected virtual void WriteDefinitionName(ISymbol symbol, SymbolDisplayFormat format = null)
    {
        WriteContainingNamespaceInTypeHierarchy(symbol);

        Write(symbol.ToDisplayParts(format ?? GetDefinitionNameFormat(symbol)));
    }

    protected void WriteContainingNamespaceInTypeHierarchy(ISymbol symbol)
    {
        if (symbol.IsKind(SymbolKind.NamedType)
            && Layout == SymbolDefinitionListLayout.TypeHierarchy
            && Format.Includes(SymbolDefinitionPartFilter.ContainingNamespaceInTypeHierarchy))
        {
            ImmutableArray<SymbolDisplayPart> parts = (symbol.ContainingType ?? (ISymbol)symbol.ContainingNamespace).ToDisplayParts(TypeSymbolDisplayFormats.Name_ContainingTypes_Namespaces);
            Write(parts);

            if (parts.Any())
                Write(".");
        }
    }

    protected SymbolDisplayFormat GetDefinitionNameFormat(ISymbol symbol)
    {
        return (symbol.IsKind(SymbolKind.Namespace))
            ? TypeSymbolDisplayFormats.Name_ContainingTypes_Namespaces
            : _definitionNameFormat;
    }

    protected void WriteParts(ISymbol symbol, ImmutableArray<SymbolDisplayPart> parts)
    {
        WriteParts(symbol, parts, 0, parts.Length);
    }

    protected void WriteParts(ISymbol symbol, ImmutableArray<SymbolDisplayPart> parts, int startIndex, int length)
    {
        int max = startIndex + length;

        int i = startIndex;
        int j;

        while (i < max)
        {
            if (parts[i].IsGlobalNamespace())
            {
                j = i;

                if (Peek(j).IsPunctuation("::")
                    && Peek(j + 1).IsNamespaceOrTypeName())
                {
                    j += 2;

                    while (Peek(j).IsPunctuation(".")
                        && Peek(j + 1).IsNamespaceOrTypeName())
                    {
                        j += 2;
                    }

                    if (Peek(j).IsPunctuation(".")
                        && Peek(j + 1).Kind == SymbolDisplayPartKind.EnumMemberName)
                    {
                        j += 2;
                    }

                    WriteSymbol(parts[j].Symbol.OriginalDefinition);

                    i = j + 1;
                    continue;
                }
            }
            else if (parts[i].Kind == SymbolDisplayPartKind.ParameterName)
            {
                WriteParameterName(symbol, parts[i]);
                i++;
                continue;
            }

            Write(parts[i]);

            i++;
        }

        SymbolDisplayPart Peek(int index)
        {
            if (index + 1 < max)
                return parts[index + 1];

            return default;
        }
    }

    protected virtual void WriteSymbol(
        ISymbol symbol,
        SymbolDisplayFormat format = null,
        bool removeAttributeSuffix = false)
    {
        format ??= GetSymbolFormat(symbol);

        ImmutableArray<SymbolDisplayPart> parts = symbol.ToDisplayParts(format);

        if (removeAttributeSuffix)
            parts = SymbolDefinitionWriterHelpers.RemoveAttributeSuffix(symbol, parts);

        Write(parts);
    }

    protected SymbolDisplayFormat GetSymbolFormat(ISymbol symbol)
    {
        if (symbol.Kind == SymbolKind.Field
            && symbol.ContainingType?.TypeKind == TypeKind.Enum)
        {
            return TypeSymbolDisplayFormats.Default;
        }

        if (symbol.IsKind(SymbolKind.Namespace)
            || Format.Includes(SymbolDefinitionPartFilter.ContainingNamespace))
        {
            return TypeSymbolDisplayFormats.Name_ContainingTypes_Namespaces_SpecialTypes;
        }

        return TypeSymbolDisplayFormats.Name_ContainingTypes_SpecialTypes;
    }

    protected virtual void WriteParameterName(ISymbol symbol, SymbolDisplayPart part)
    {
        Write(part);
    }

    public void Write(ImmutableArray<SymbolDisplayPart> parts)
    {
        Write(parts, 0, parts.Length);
    }

    internal void Write(ImmutableArray<SymbolDisplayPart> parts, int start, int length)
    {
        int max = start + length;

        for (int i = start; i < max; i++)
            Write(parts[i]);
    }

    public virtual void Write(SymbolDisplayPart part)
    {
        Write(part.ToString());
    }

    public abstract void Write(string value);

    public abstract void WriteLine();

    public virtual void WriteLine(string value)
    {
        Write(value);
        WriteLine();
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
                Close();

            _disposed = true;
        }
    }

    public virtual void Close()
    {
    }
}
