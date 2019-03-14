// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using Microsoft.CodeAnalysis;
using Roslynator.FindSymbols;

namespace Roslynator.Documentation
{
    internal sealed class TypeHierarchy
    {
        private TypeHierarchyItem _enumRoot;
        private TypeHierarchyItem _interfaceRoot;
        private TypeHierarchyItem _valueTypeRoot;

        private TypeHierarchy(TypeHierarchyItem root, ImmutableArray<TypeHierarchyItem> interfaces)
        {
            Root = root;
            Interfaces = interfaces;
        }

        public TypeHierarchyItem Root { get; }

        public ImmutableArray<TypeHierarchyItem> Interfaces { get; }

        public TypeHierarchyItem InterfaceRoot
        {
            get
            {
                if (_interfaceRoot == null)
                    Interlocked.CompareExchange(ref _interfaceRoot, LoadInterfaceHierarchy(), null);

                return _interfaceRoot;
            }
        }

        internal TypeHierarchyItem EnumRoot
        {
            get
            {
                if (_enumRoot == null)
                    Interlocked.CompareExchange(ref _enumRoot, GetEnumRoot(), null);

                return _enumRoot;

                TypeHierarchyItem GetEnumRoot()
                {
                    TypeHierarchyItem valueType = ValueTypeRoot;

                    foreach (TypeHierarchyItem item in valueType.Children())
                    {
                        if (item.Symbol.HasMetadataName(MetadataNames.System_Enum))
                            return item;
                    }

                    return new TypeHierarchyItem(null);
                }
            }
        }

        internal TypeHierarchyItem ValueTypeRoot
        {
            get
            {
                if (_valueTypeRoot == null)
                    Interlocked.CompareExchange(ref _valueTypeRoot, GetValueTypeRoot(), null);

                return _valueTypeRoot;

                TypeHierarchyItem GetValueTypeRoot()
                {
                    foreach (TypeHierarchyItem item in Root.Children())
                    {
                        if (item.Symbol.HasMetadataName(MetadataNames.System_ValueType))
                            return item;
                    }

                    return new TypeHierarchyItem(null);
                }
            }
        }

        public IEnumerable<TypeHierarchyItem> GetStructs()
        {
            foreach (TypeHierarchyItem item in _valueTypeRoot.Children())
            {
                if (item.Symbol.TypeKind == TypeKind.Struct)
                    yield return item;
            }
        }

        public IEnumerable<TypeHierarchyItem> GetEnums()
        {
            foreach (TypeHierarchyItem item in _enumRoot.Children())
                yield return item;
        }

        public TypeHierarchyItem LoadInterfaceHierarchy()
        {
            var interfaceRoot = new TypeHierarchyItem(null);

            if (Interfaces.Any())
            {
                var rootInterfaces = new List<TypeHierarchyItem>();

                for (int i = Interfaces.Length - 1; i >= 0; i--)
                {
                    if (IsRootInterface(Interfaces[i].Symbol))
                        rootInterfaces.Add(Interfaces[i]);
                }

                FillHierarchyItems(rootInterfaces, interfaceRoot, FillHierarchyItem);
            }

            return interfaceRoot;

            bool IsRootInterface(INamedTypeSymbol interfaceSymbol)
            {
                foreach (INamedTypeSymbol interfaceSymbol2 in interfaceSymbol.Interfaces)
                {
                    foreach (TypeHierarchyItem interfaceItem in Interfaces)
                    {
                        if (interfaceItem.Symbol == interfaceSymbol2)
                            return false;
                    }
                }

                return true;
            }

            TypeHierarchyItem FillHierarchyItem(TypeHierarchyItem item, TypeHierarchyItem parent)
            {
                INamedTypeSymbol symbol = item.Symbol;

                item = new TypeHierarchyItem(symbol) { Parent = parent };

                TypeHierarchyItem[] derivedInterfaces = Interfaces
                    .Where(f => f.Symbol.Interfaces.Any(i => i.OriginalDefinition == symbol.OriginalDefinition))
                    .ToArray();

                if (derivedInterfaces.Length > 0)
                {
                    Array.Reverse(derivedInterfaces);
                    FillHierarchyItems(derivedInterfaces, item, FillHierarchyItem);
                }

                return item;
            }
        }

        public static TypeHierarchy Create(
            IEnumerable<IAssemblySymbol> assemblies,
            SymbolFilterOptions filter = null,
            IComparer<INamedTypeSymbol> comparer = null)
        {
            Func<INamedTypeSymbol, bool> predicate = null;

            if (filter != null)
                predicate = t => filter.IsMatch(t);

            IEnumerable<INamedTypeSymbol> types = assemblies.SelectMany(a => a.GetTypes(predicate));

            return Create(types, comparer);
        }

        public static TypeHierarchy Create(IEnumerable<INamedTypeSymbol> types, IComparer<INamedTypeSymbol> comparer = null)
        {
            if (comparer == null)
                comparer = SymbolDefinitionComparer.SystemFirst.TypeComparer;

            INamedTypeSymbol objectType = FindObjectType();

            if (objectType == null)
                throw new InvalidOperationException("Object type not found.");

            Dictionary<INamedTypeSymbol, TypeHierarchyItem> allItems = types
                .ToDictionary(f => f, f => new TypeHierarchyItem(f));

            allItems[objectType] = new TypeHierarchyItem(objectType, isExternal: true);

            foreach (INamedTypeSymbol type in types)
            {
                INamedTypeSymbol t = type.BaseType;

                while (t != null)
                {
                    if (!allItems.ContainsKey(t.OriginalDefinition))
                        allItems[t.OriginalDefinition] = new TypeHierarchyItem(t.OriginalDefinition, isExternal: true);

                    t = t.BaseType;
                }
            }

            TypeHierarchyItem root = FillHierarchyItem(allItems[objectType], null);

            ImmutableArray<TypeHierarchyItem> interfaces = allItems
                .Select(f => f.Value)
                .OrderBy(f => f.Symbol, comparer)
                .ToImmutableArray();

            return new TypeHierarchy(root, interfaces);

            TypeHierarchyItem FillHierarchyItem(TypeHierarchyItem item, TypeHierarchyItem parent)
            {
                INamedTypeSymbol symbol = item.Symbol;

                item.Parent = parent;

                allItems.Remove(symbol);

                TypeHierarchyItem[] derivedTypes = allItems
                    .Select(f => f.Value)
                    .Where(f => f.Symbol.BaseType?.OriginalDefinition == symbol.OriginalDefinition)
                    .ToArray();

                if (derivedTypes.Length > 0)
                {
                    if (symbol.SpecialType == SpecialType.System_Object)
                    {
                        Array.Sort(derivedTypes, (x, y) =>
                        {
                            if (x.Symbol.IsStatic)
                            {
                                if (!y.Symbol.IsStatic)
                                {
                                    return -1;
                                }
                            }
                            else if (y.Symbol.IsStatic)
                            {
                                return 1;
                            }

                            return Compare(x, y);
                        });
                    }
                    else
                    {
                        Array.Sort(derivedTypes, Compare);
                    }

                    FillHierarchyItems(derivedTypes, item, FillHierarchyItem);
                }

                return item;
            }

            INamedTypeSymbol FindObjectType()
            {
                foreach (INamedTypeSymbol type in types)
                {
                    INamedTypeSymbol t = type;

                    do
                    {
                        if (t.SpecialType == SpecialType.System_Object)
                            return t;

                        t = t.BaseType;
                    }
                    while (t != null);
                }

                return null;
            }

            int Compare(TypeHierarchyItem x, TypeHierarchyItem y)
            {
                return -comparer.Compare(x.Symbol, y.Symbol);
            }
        }

        private static void FillHierarchyItems(
            IList<TypeHierarchyItem> items,
            TypeHierarchyItem parent,
            Func<TypeHierarchyItem, TypeHierarchyItem, TypeHierarchyItem> fillHierarchyItem)
        {
            TypeHierarchyItem last = fillHierarchyItem(items[0], parent);

            TypeHierarchyItem next = last;

            TypeHierarchyItem child = null;

            for (int i = 1; i < items.Count; i++)
            {
                child = fillHierarchyItem(items[i], parent);

                child.next = next;

                next = child;
            }

            last.next = child ?? last;

            parent.content = last;
        }
    }
}
