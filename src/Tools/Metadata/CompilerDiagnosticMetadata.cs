// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace Roslynator.Metadata;

public record CompilerDiagnosticMetadata(string Id, string Identifier, string Title, string MessageFormat, string Severity, string HelpUrl);
