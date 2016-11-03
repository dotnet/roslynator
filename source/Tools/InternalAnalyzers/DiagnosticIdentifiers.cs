// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace Roslynator.CSharp.Internal
{
    public static class DiagnosticIdentifiers
    {
        public const string Prefix = "RCS";

        public const string AddCodeFileHeader = Prefix + "9001";
        public const string ReplaceIsKindMethodInvocation = Prefix + "9002";
        public const string AddDiagnosticAnalyzerSuffix = Prefix + "9003";
        public const string AddCodeFixProviderSuffix = Prefix + "9004";
        public const string AddCodeRefactoringProviderSuffix = Prefix + "9005";
    }
}
