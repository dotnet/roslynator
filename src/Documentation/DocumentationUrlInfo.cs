// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;

namespace Roslynator.Documentation
{
    public readonly struct DocumentationUrlInfo : IEquatable<DocumentationUrlInfo>
    {
        public DocumentationUrlInfo(string url, DocumentationUrlKind kind)
        {
            Url = url;
            Kind = kind;
        }

        public string Url { get; }

        public DocumentationUrlKind Kind { get; }

        public override bool Equals(object obj)
        {
            return obj is DocumentationUrlInfo other && Equals(other);
        }

        public bool Equals(DocumentationUrlInfo other)
        {
            return Kind == other.Kind
                && Url == other.Url;
        }

        public override int GetHashCode()
        {
            return Hash.Combine(Url, (int)Kind);
        }

        public static bool operator ==(in DocumentationUrlInfo info1, in DocumentationUrlInfo info2)
        {
            return info1.Equals(info2);
        }

        public static bool operator !=(in DocumentationUrlInfo info1, in DocumentationUrlInfo info2)
        {
            return !(info1 == info2);
        }
    }
}
