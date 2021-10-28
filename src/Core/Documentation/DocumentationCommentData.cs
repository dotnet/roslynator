// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Diagnostics;

namespace Roslynator.Documentation
{
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    internal readonly struct DocumentationCommentData
    {
        internal DocumentationCommentData(string rawXml, DocumentationCommentOrigin origin)
        {
            RawXml = rawXml;
            Origin = origin;
        }

        public string RawXml { get; }

        public DocumentationCommentOrigin Origin { get; }

        public bool Success => !string.IsNullOrEmpty(RawXml);

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string DebuggerDisplay
        {
            get { return (Success) ? $"{Origin} {RawXml}" : "Uninitialized"; }
        }
    }
}
