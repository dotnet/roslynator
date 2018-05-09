// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;

namespace Roslynator.Tests
{
    public class CodeVerificationOptions
    {
        public CodeVerificationOptions(
            bool allowNewCompilerDiagnostics = false,
            bool enableDiagnosticsDisabledByDefault = true,
            DiagnosticSeverity maxAllowedCompilerDiagnosticSeverity = DiagnosticSeverity.Info,
            IEnumerable<string> allowedCompilerDiagnostics = null)
        {
            MaxAllowedCompilerDiagnosticSeverity = maxAllowedCompilerDiagnosticSeverity;
            EnableDiagnosticsDisabledByDefault = enableDiagnosticsDisabledByDefault;
            AllowNewCompilerDiagnostics = allowNewCompilerDiagnostics;

            AllowedCompilerDiagnosticIds = (allowedCompilerDiagnostics != null)
                ? ImmutableArray.CreateRange(allowedCompilerDiagnostics)
                : ImmutableArray<string>.Empty;
        }

        public static CodeVerificationOptions Default { get; } = new CodeVerificationOptions(allowedCompilerDiagnostics: ImmutableArray.Create(
            "CS0067", // Event is never used
            "CS0168", // Variable is declared but never used
            "CS0169", // Field is never used
            "CS0219", // Variable is assigned but its value is never used
            "CS0660", // Type defines operator == or operator != but does not override Object.Equals(object o)
            "CS0661", // Type defines operator == or operator != but does not override Object.GetHashCode()
            "CS8019" // Unnecessary using directive
        ));

        public bool AllowNewCompilerDiagnostics { get; }

        public bool EnableDiagnosticsDisabledByDefault { get; }

        public DiagnosticSeverity MaxAllowedCompilerDiagnosticSeverity { get; }

        public ImmutableArray<string> AllowedCompilerDiagnosticIds { get; }

        public CodeVerificationOptions AddAllowedCompilerDiagnosticId(string diagnosticId)
        {
            return new CodeVerificationOptions(allowedCompilerDiagnostics: AllowedCompilerDiagnosticIds.Add(diagnosticId));
        }

        public CodeVerificationOptions AddAllowedCompilerDiagnosticIds(IEnumerable<string> diagnosticIds)
        {
            return new CodeVerificationOptions(allowedCompilerDiagnostics: AllowedCompilerDiagnosticIds.AddRange(diagnosticIds));
        }
    }
}
