// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Roslynator.Tests.Text;

namespace Roslynator.Tests
{
    public abstract class CodeVerifier
    {
        public abstract CodeVerificationOptions Options { get; }

        protected abstract WorkspaceFactory WorkspaceFactory { get; }

        protected virtual TextParser TextParser { get; } = TextParser.Default;
    }
}
