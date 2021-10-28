// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;

namespace Roslynator
{
    public readonly struct SourceReference
    {
        public SourceReference(Version version, string url)
        {
            Version = version;
            Url = url;
        }

        public Version Version { get; }

        public string Url { get; }
    }
}
