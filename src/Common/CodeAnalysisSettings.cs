// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Diagnostics;

namespace Roslynator
{
    public class CodeAnalysisSettings
    {
        public CodeAnalysisSettings()
        {
            Disabled = new IdentifierSet();
        }

        public IdentifierSet Disabled { get; }

        public virtual void Reset()
        {
            Disabled.Clear();
        }

        public bool IsEnabled(string id)
        {
            return !Disabled.Contains(id);
        }

        public bool IsAnyEnabled(string id, string id2)
        {
            return IsEnabled(id)
                || IsEnabled(id2);
        }

        public bool IsAnyEnabled(string id, string id2, string id3)
        {
            return IsEnabled(id)
                || IsEnabled(id2)
                || IsEnabled(id3);
        }

        public bool IsAnyEnabled(string id, string id2, string id3, string id4)
        {
            return IsEnabled(id)
                || IsEnabled(id2)
                || IsEnabled(id3)
                || IsEnabled(id4);
        }

        public bool IsAnyEnabled(string id, string id2, string id3, string id4, string id5)
        {
            return IsEnabled(id)
                || IsEnabled(id2)
                || IsEnabled(id3)
                || IsEnabled(id4)
                || IsEnabled(id5);
        }

        public bool IsAnyEnabled(string id, string id2, string id3, string id4, string id5, string id6)
        {
            return IsEnabled(id)
                || IsEnabled(id2)
                || IsEnabled(id3)
                || IsEnabled(id4)
                || IsEnabled(id5)
                || IsEnabled(id6);
        }

        public void Disable(string id)
        {
            Debug.WriteLineIf(Disabled.Add(id), $"{id} disabled");

            Disabled.Add(id);
        }

        public void Enable(string id)
        {
            Debug.WriteLineIf(Disabled.Remove(id), $"{id} enabled");

            Disabled.Remove(id);
        }

        public void Set(string id, bool isEnabled)
        {
            if (isEnabled)
            {
                Enable(id);
            }
            else
            {
                Disable(id);
            }
        }

        public void Set(Dictionary<string, bool> values)
        {
            foreach (KeyValuePair<string, bool> kvp in values)
                Set(kvp.Key, kvp.Value);
        }
    }
}
