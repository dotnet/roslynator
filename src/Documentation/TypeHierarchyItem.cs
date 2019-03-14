// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.CodeAnalysis;

namespace Roslynator.Documentation
{
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    internal class TypeHierarchyItem
    {
        internal TypeHierarchyItem content;
        internal TypeHierarchyItem next;

        internal TypeHierarchyItem(INamedTypeSymbol symbol, bool isExternal = false)
        {
            Symbol = symbol;
            IsExternal = isExternal;
        }

        public INamedTypeSymbol Symbol { get; }

        public TypeHierarchyItem Parent { get; internal set; }

        public bool IsExternal { get; }

        public int Depth
        {
            get
            {
                int depth = 0;

                TypeHierarchyItem parent = Parent;

                while (parent != null)
                {
                    depth++;
                    parent = parent.Parent;
                }

                return depth;
            }
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string DebuggerDisplay
        {
            get { return $"Depth = {Depth} {Symbol.ToDisplayString(Roslynator.SymbolDisplayFormats.Test)}"; }
        }

        public bool HasChildren => content != null;

        public IEnumerable<TypeHierarchyItem> Children()
        {
            TypeHierarchyItem e = content;

            if (e != null)
            {
                do
                {
                    e = e.next;
                    yield return e;

                } while (e.Parent == this && e != content);
            }
        }

        public IEnumerable<TypeHierarchyItem> Ancestors()
        {
            return GetAncestors(self: false);
        }

        public IEnumerable<TypeHierarchyItem> AncestorsAndSelf()
        {
            return GetAncestors(self: true);
        }

        internal IEnumerable<TypeHierarchyItem> GetAncestors(bool self)
        {
            TypeHierarchyItem c = ((self) ? this : Parent);

            while (c != null)
            {
                yield return c;

                c = c.Parent;
            }
        }

        public IEnumerable<TypeHierarchyItem> Descendants()
        {
            return GetDescendants(self: false);
        }

        public IEnumerable<TypeHierarchyItem> DescendantsAndSelf()
        {
            return GetDescendants(self: true);
        }

        internal IEnumerable<TypeHierarchyItem> GetDescendants(bool self)
        {
            var e = this;

            if (self)
                yield return e;

            var c = this;

            while (true)
            {
                TypeHierarchyItem first = c?.content?.next;

                if (first != null)
                {
                    e = first;
                }
                else
                {
                    while (e != this
                        && e == e.Parent.content)
                    {
                        e = e.Parent;
                    }

                    if (e == this)
                        break;

                    e = e.next;
                }

                if (e != null)
                    yield return e;

                c = e;
            }
        }
    }
}
