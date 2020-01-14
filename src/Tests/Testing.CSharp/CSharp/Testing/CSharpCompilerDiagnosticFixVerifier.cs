// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading;
using Roslynator.Testing;

namespace Roslynator.CSharp.Testing
{
    public abstract class CSharpCompilerDiagnosticFixVerifier : CompilerDiagnosticFixVerifier
    {
        private CSharpCodeVerificationOptions _options;

        protected CSharpCompilerDiagnosticFixVerifier() : base(CSharpWorkspaceFactory.Instance)
        {
        }

        new public CSharpCodeVerificationOptions Options
        {
            get
            {
                if (_options == null)
                    Interlocked.CompareExchange(ref _options, CreateAndUpdateOptions(), null);

                return _options;
            }
        }

        protected override CodeVerificationOptions CommonOptions => Options;

        private CSharpCodeVerificationOptions CreateAndUpdateOptions()
        {
            CSharpCodeVerificationOptions options = CreateOptions();

            return UpdateOptions(options);
        }

        protected virtual CSharpCodeVerificationOptions CreateOptions()
        {
            return CSharpCodeVerificationOptions.Default;
        }

        protected virtual CSharpCodeVerificationOptions UpdateOptions(CSharpCodeVerificationOptions options)
        {
            return options;
        }
    }
}
