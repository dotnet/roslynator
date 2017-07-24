// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace Roslynator.Metadata
{
    public class CompilerDiagnosticDescriptor
    {
        public CompilerDiagnosticDescriptor(
            string id,
            string identifier,
            string helpUrl)
        {
            Id = id;
            Identifier = identifier;
            HelpUrl = helpUrl;
        }

        public string Id { get; }

        public string Identifier { get; }

        public string HelpUrl { get; }
    }
}
