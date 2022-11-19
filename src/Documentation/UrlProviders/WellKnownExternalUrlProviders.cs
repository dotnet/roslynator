﻿// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace Roslynator.Documentation;

public static class WellKnownExternalUrlProviders
{
    public static ExternalUrlProvider MicrosoftDocs { get; } = MicrosoftDocsUrlProvider.Instance;
}
