// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.CodeAnalysis;

namespace Roslynator
{
    internal readonly struct AnalyzerOptionDescriptor
    {
        public AnalyzerOptionDescriptor(
            string optionKey,
            DiagnosticDescriptor descriptor)
        {
            Descriptor = descriptor;
            OptionKey = optionKey;
        }

        public string OptionKey { get; }

        public DiagnosticDescriptor Descriptor { get; }

        public bool IsDefault
        {
            get
            {
                return Descriptor == null
                    && OptionKey == null;
            }
        }
    }
}
