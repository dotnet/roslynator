// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.VisualBasic;
using Roslynator.Tests;

namespace Roslynator.VisualBasic.Tests
{
    public class VisualBasicCodeVerificationOptions : CodeVerificationOptions
    {
        public VisualBasicCodeVerificationOptions(
            bool allowNewCompilerDiagnostics = false,
            bool enableDiagnosticsDisabledByDefault = true,
            DiagnosticSeverity maxAllowedCompilerDiagnosticSeverity = DiagnosticSeverity.Info,
            IEnumerable<string> allowedCompilerDiagnosticIds = null,
            LanguageVersion languageVersion = LanguageVersion.Latest)
            : base(allowNewCompilerDiagnostics, enableDiagnosticsDisabledByDefault, maxAllowedCompilerDiagnosticSeverity, allowedCompilerDiagnosticIds)
        {
            LanguageVersion = languageVersion;
        }

        public LanguageVersion LanguageVersion { get; }

        //TODO: Allowed compiler diagnostic IDs for Visual Basic
        public static VisualBasicCodeVerificationOptions Default { get; } = new VisualBasicCodeVerificationOptions();

        public override CodeVerificationOptions AddAllowedCompilerDiagnosticId(string diagnosticId)
        {
            return WithAllowedCompilerDiagnosticIds(AllowedCompilerDiagnosticIds.Add(diagnosticId));
        }

        public override CodeVerificationOptions AddAllowedCompilerDiagnosticIds(IEnumerable<string> diagnosticIds)
        {
            return WithAllowedCompilerDiagnosticIds(AllowedCompilerDiagnosticIds.AddRange(diagnosticIds));
        }

        public VisualBasicCodeVerificationOptions WithAllowedCompilerDiagnosticIds(IEnumerable<string> allowedCompilerDiagnosticIds)
        {
            if (allowedCompilerDiagnosticIds == null)
                throw new ArgumentNullException(nameof(allowedCompilerDiagnosticIds));

            return new VisualBasicCodeVerificationOptions(
                allowNewCompilerDiagnostics: AllowNewCompilerDiagnostics,
                enableDiagnosticsDisabledByDefault: EnableDiagnosticsDisabledByDefault,
                maxAllowedCompilerDiagnosticSeverity: MaxAllowedCompilerDiagnosticSeverity,
                allowedCompilerDiagnosticIds: allowedCompilerDiagnosticIds,
                languageVersion: LanguageVersion);
        }

        public VisualBasicCodeVerificationOptions WithLanguageOptions(LanguageVersion languageVersion)
        {
            return new VisualBasicCodeVerificationOptions(
                allowNewCompilerDiagnostics: AllowNewCompilerDiagnostics,
                enableDiagnosticsDisabledByDefault: EnableDiagnosticsDisabledByDefault,
                maxAllowedCompilerDiagnosticSeverity: MaxAllowedCompilerDiagnosticSeverity,
                allowedCompilerDiagnosticIds: AllowedCompilerDiagnosticIds,
                languageVersion: languageVersion);
        }
    }
}
