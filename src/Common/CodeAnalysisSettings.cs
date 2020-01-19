// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Diagnostics;

namespace Roslynator
{
    public abstract class CodeAnalysisSettings<T>
    {
        protected CodeAnalysisSettings()
        {
            Disabled = new HashSet<T>();
        }

        public HashSet<T> Disabled { get; }

        public abstract void Reset();

        public bool IsEnabled(T item)
        {
            return !Disabled.Contains(item);
        }

        public bool IsAnyEnabled(T item, T item2)
        {
            return IsEnabled(item)
                || IsEnabled(item2);
        }

        public bool IsAnyEnabled(T item, T item2, T item3)
        {
            return IsEnabled(item)
                || IsEnabled(item2)
                || IsEnabled(item3);
        }

        public bool IsAnyEnabled(T item, T item2, T item3, T item4)
        {
            return IsEnabled(item)
                || IsEnabled(item2)
                || IsEnabled(item3)
                || IsEnabled(item4);
        }

        public bool IsAnyEnabled(T item, T item2, T item3, T item4, T item5)
        {
            return IsEnabled(item)
                || IsEnabled(item2)
                || IsEnabled(item3)
                || IsEnabled(item4)
                || IsEnabled(item5);
        }

        public bool IsAnyEnabled(T item, T item2, T item3, T item4, T item5, T item6)
        {
            return IsEnabled(item)
                || IsEnabled(item2)
                || IsEnabled(item3)
                || IsEnabled(item4)
                || IsEnabled(item5)
                || IsEnabled(item6);
        }

        public void Disable(T item)
        {
            Debug.WriteLineIf(Disabled.Add(item), $"{item} disabled");

            Disabled.Add(item);
        }

        public void Enable(T item)
        {
            Debug.WriteLineIf(Disabled.Remove(item), $"{item} enabled");

            Disabled.Remove(item);
        }

        public void Set(T item, bool isEnabled)
        {
            if (isEnabled)
            {
                Enable(item);
            }
            else
            {
                Disable(item);
            }
        }
    }
}
