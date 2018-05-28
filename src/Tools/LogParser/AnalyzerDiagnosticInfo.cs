// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace Roslynator.Diagnostics
{
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    [SuppressMessage("Performance", "CA1815:Override equals and operator equals on value types", Justification = "<Pending>")]
    public readonly struct AnalyzerDiagnosticInfo
    {
        public AnalyzerDiagnosticInfo(string fullName, int elapsed, int percent)
        {
            Elapsed = elapsed;
            Percent = percent;
            FullName = fullName;
        }

        public string FullName { get; }

        public string Name
        {
            get
            {
                if (FullName == null)
                    return null;

                int index = FullName.LastIndexOf('.');

                if (index == -1)
                    return null;

                if (index == FullName.Length - 1)
                    return null;

                return FullName.Substring(index + 1);
            }
        }

        public string Namespace
        {
            get
            {
                if (FullName == null)
                    return null;

                int index = FullName.LastIndexOf('.');

                if (index == -1)
                    return null;

                if (index == 0)
                    return null;

                return FullName.Remove(index);
            }
        }

        public string RootNamespace
        {
            get
            {
                if (FullName == null)
                    return null;

                int index = FullName.IndexOf('.');

                if (index == -1)
                    return null;

                return FullName.Remove(index);
            }
        }

        public int Elapsed { get; }

        public int Percent { get; }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string DebuggerDisplay
        {
            get { return $"{Elapsed}ms {Percent}% {FullName}"; }
        }
    }
}
