// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Xunit;

namespace Roslynator.Tests
{
    public abstract class CodeVerifier
    {
        public virtual CodeVerificationOptions Options
        {
            get { return CodeVerificationOptions.Default; }
        }

        public abstract string Language { get; }

        internal void VerifyCompilerDiagnostics(ImmutableArray<Diagnostic> diagnostics)
        {
            DiagnosticSeverity maxAllowedSeverity = Options.MaxAllowedCompilerDiagnosticSeverity;

            ImmutableArray<string> allowedDiagnosticIds = Options.AllowedCompilerDiagnosticIds;

            if (IsAny())
            {
                IEnumerable<Diagnostic> notAllowed = diagnostics
                    .Where(f => f.Severity > maxAllowedSeverity && !allowedDiagnosticIds.Any(id => id == f.Id));

                Assert.True(false, $"No compiler diagnostics with severity higher than '{maxAllowedSeverity}' expected{notAllowed.ToDebugString()}");
            }

            bool IsAny()
            {
                foreach (Diagnostic diagnostic in diagnostics)
                {
                    if (diagnostic.Severity <= maxAllowedSeverity)
                        continue;

                    bool isAllowed = false;
                    foreach (string diagnosticId in allowedDiagnosticIds)
                    {
                        if (diagnostic.Id == diagnosticId)
                        {
                            isAllowed = true;
                            break;
                        }
                    }

                    if (!isAllowed)
                        return true;
                }

                return false;
            }
        }

        internal void VerifyNoNewCompilerDiagnostics(
            ImmutableArray<Diagnostic> diagnostics,
            ImmutableArray<Diagnostic> newDiagnostics)
        {
            ImmutableArray<string> allowedDiagnosticIds = Options.AllowedCompilerDiagnosticIds;

            if (allowedDiagnosticIds.IsDefault)
                allowedDiagnosticIds = ImmutableArray<string>.Empty;

            if (IsAnyNewCompilerDiagnostic())
            {
                IEnumerable<Diagnostic> diff = newDiagnostics
                    .Where(diagnostic => !allowedDiagnosticIds.Any(id => id == diagnostic.Id))
                    .Except(diagnostics, DiagnosticDeepEqualityComparer.Instance);

                Assert.True(false, $"Code fix introduced new compiler diagnostics.{diff.ToDebugString()}");
            }

            bool IsAnyNewCompilerDiagnostic()
            {
                foreach (Diagnostic newDiagnostic in newDiagnostics)
                {
                    bool isAllowed = false;
                    foreach (string diagnosticId in allowedDiagnosticIds)
                    {
                        if (newDiagnostic.Id == diagnosticId)
                        {
                            isAllowed = true;
                            break;
                        }
                    }

                    if (isAllowed)
                        continue;

                    bool isNew = true;
                    foreach (Diagnostic diagnostic in diagnostics)
                    {
                        if (DiagnosticDeepEqualityComparer.Instance.Equals(newDiagnostic, diagnostic))
                        {
                            isNew = false;
                            break;
                        }
                    }

                    if (isNew)
                        return true;
                }

                return false;
            }
        }
    }
}
