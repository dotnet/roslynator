// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Xunit;

namespace Roslynator.Tests
{
    internal static class CompilerDiagnosticVerifier
    {
        public static void VerifyCompilerDiagnostics(
            ImmutableArray<Diagnostic> diagnostics,
            CodeVerificationOptions options)
        {
            DiagnosticSeverity maxAllowedSeverity = options.MaxAllowedCompilerDiagnosticSeverity;

            ImmutableArray<string> allowedDiagnosticIds = options.AllowedCompilerDiagnosticIds;

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
                    if (diagnostic.Severity > maxAllowedSeverity
                        && !IsAllowed(diagnostic))
                    {
                        return true;
                    }
                }

                return false;
            }

            bool IsAllowed(Diagnostic diagnostic)
            {
                foreach (string diagnosticId in allowedDiagnosticIds)
                {
                    if (diagnostic.Id == diagnosticId)
                        return true;
                }

                return false;
            }
        }

        public static void VerifyNoNewCompilerDiagnostics(
            ImmutableArray<Diagnostic> diagnostics,
            ImmutableArray<Diagnostic> newDiagnostics,
            CodeVerificationOptions options)
        {
            ImmutableArray<string> allowedDiagnosticIds = options.AllowedCompilerDiagnosticIds;

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
                    if (!IsAllowed(newDiagnostic)
                        && !EqualsAny(newDiagnostic))
                    {
                        return true;
                    }
                }

                return false;
            }

            bool IsAllowed(Diagnostic diagnostic)
            {
                foreach (string diagnosticId in allowedDiagnosticIds)
                {
                    if (diagnostic.Id == diagnosticId)
                        return true;
                }

                return false;
            }

            bool EqualsAny(Diagnostic newDiagnostic)
            {
                foreach (Diagnostic diagnostic in diagnostics)
                {
                    if (DiagnosticDeepEqualityComparer.Instance.Equals(diagnostic, newDiagnostic))
                        return true;
                }

                return false;
            }
        }
    }
}
