// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Formatting;
using Microsoft.CodeAnalysis.Text;
using Xunit;

namespace Roslynator.Tests
{
    public static class DiagnosticVerifier
    {
        public static void VerifyDiagnostic(
            string source,
            DiagnosticAnalyzer analyzer,
            string language,
            params Diagnostic[] expectedDiagnostics)
        {
            VerifyDiagnostic(new string[] { source }, analyzer, language, expectedDiagnostics);
        }

        public static void VerifyDiagnostic(
            IEnumerable<string> sources,
            DiagnosticAnalyzer analyzer,
            string language,
            params Diagnostic[] expectedDiagnostics)
        {
            foreach (Diagnostic diagnostic in expectedDiagnostics)
            {
                Assert.True(analyzer.Supports(diagnostic.Descriptor),
                    $"Diagnostic \"{diagnostic.Descriptor.Id}\" is not supported by analyzer \"{analyzer.GetType().Name}\".");
            }

            Diagnostic[] diagnostics = DiagnosticUtility.GetSortedDiagnostics(sources, analyzer, language);

            if (diagnostics.Length > 0
                && analyzer.SupportedDiagnostics.Length > 1)
            {
                diagnostics = diagnostics
                    .Where(diagnostic => expectedDiagnostics.Any(expectedDiagnostic => DiagnosticComparer.Id.Equals(diagnostic, expectedDiagnostic)))
                    .ToArray();
            }

            VerifyDiagnostics(diagnostics, expectedDiagnostics);
        }

        private static void VerifyDiagnostics(
            Diagnostic[] actual,
            Diagnostic[] expected)
        {
            int expectedCount = expected.Length;
            int actualCount = actual.Length;

            Assert.True(expectedCount == actualCount,
                $"Mismatch between number of diagnostics returned, expected: {expectedCount} actual: {actualCount}{actual.ToDebugString()}");

            for (int i = 0; i < expectedCount; i++)
                VerifyDiagnostic(actual[i], expected[i]);
        }

        private static void VerifyDiagnostic(Diagnostic actual, Diagnostic expected)
        {
            Assert.True(actual.Id == expected.Descriptor.Id,
                $"Expected diagnostic id to be \"{expected.Descriptor.Id}\" was \"{actual.Id}\"\r\n\r\nDiagnostic:\r\n{actual}\r\n");

            VerifyLocation(actual, actual.Location, expected.Location);

            VerifyAdditionalLocations(actual, actual.AdditionalLocations, expected.AdditionalLocations);
        }

        private static void VerifyAdditionalLocations(
            Diagnostic diagnostic,
            IReadOnlyList<Location> actual,
            IReadOnlyList<Location> expected)
        {
            int actualCount = actual.Count;
            int expectedCount = expected.Count;

            Assert.True(actualCount == expectedCount,
                $"Expected {expectedCount} additional locations, actual: {actualCount}\r\n\r\nDiagnostic:\r\n{diagnostic}\r\n");

            for (int j = 0; j < actualCount; j++)
                VerifyLocation(diagnostic, actual[j], expected[j]);
        }

        private static void VerifyLocation(
            Diagnostic diagnostic,
            Location actual,
            Location expected)
        {
            VerifyFileLinePositionSpan(diagnostic, actual.GetLineSpan(), expected.GetLineSpan());
        }

        private static void VerifyFileLinePositionSpan(
            Diagnostic diagnostic,
            FileLinePositionSpan actual,
            FileLinePositionSpan expected)
        {
            Assert.True(actual.Path == expected.Path,
                $"Expected diagnostic to be in file \"{expected.Path}\", actual: \"{actual.Path}\"\r\n\r\nDiagnostic:\r\n{diagnostic}\r\n");

            VerifyLinePosition(diagnostic, actual.StartLinePosition, expected.StartLinePosition, "start");

            VerifyLinePosition(diagnostic, actual.EndLinePosition, expected.EndLinePosition, "end");
        }

        private static void VerifyLinePosition(
            Diagnostic diagnostic,
            LinePosition actual,
            LinePosition expected,
            string name)
        {
            int actualLine = actual.Line;
            int expectedLine = expected.Line;

            Assert.True(actualLine == expectedLine,
                $"Expected diagnostic to {name} on line {expectedLine}, actual: {actualLine}\r\n\r\nDiagnostic:\r\n{diagnostic}\r\n");

            int actualCharacter = actual.Character;
            int expectedCharacter = expected.Character;

            Assert.True(actualCharacter == expectedCharacter,
                $"Expected diagnostic to {name} at column {expectedCharacter}, actual: {actualCharacter}\r\n\r\nDiagnostic:\r\n{diagnostic}\r\n");
        }

        public static void VerifyNoDiagnostic(
            string source,
            DiagnosticDescriptor descriptor,
            DiagnosticAnalyzer analyzer,
            string language)
        {
            VerifyNoDiagnostic(new string[] { source }, descriptor, analyzer, language);
        }

        public static void VerifyNoDiagnostic(
            IEnumerable<string> sources,
            DiagnosticDescriptor descriptor,
            DiagnosticAnalyzer analyzer,
            string language)
        {
            Assert.True(analyzer.Supports(descriptor),
                $"Diagnostic \"{descriptor.Id}\" is not supported by analyzer \"{analyzer.GetType().Name}\".");

            IEnumerable<Document> documents = WorkspaceFactory.CreateDocuments(sources, language);

            VerifyNoCompilerError(documents);

            Diagnostic[] diagnostics = DiagnosticUtility.GetSortedDiagnostics(documents, analyzer);

            Assert.True(diagnostics.Length == 0 || diagnostics.All(f => !string.Equals(f.Id, descriptor.Id, StringComparison.Ordinal)),
                    $"No diagnostic expected{diagnostics.Where(f => string.Equals(f.Id, descriptor.Id, StringComparison.Ordinal)).ToDebugString()}");
        }

        public static void VerifyNoCompilerError(Document document)
        {
            ImmutableArray<Diagnostic> compilerDiagnostics = document.GetCompilerDiagnostics();

            VerifyNoCompilerError(compilerDiagnostics);
        }

        public static void VerifyNoCompilerError(ImmutableArray<Diagnostic> compilerDiagnostics)
        {
            Assert.False(compilerDiagnostics.Any(f => f.Severity == DiagnosticSeverity.Error),
                $"No compiler error expected{compilerDiagnostics.Where(f => f.Severity == DiagnosticSeverity.Error).ToDebugString()}");
        }

        public static void VerifyNoCompilerError(IEnumerable<Document> documents)
        {
            foreach (Document document in documents)
                VerifyNoCompilerError(document);
        }

        public static void VerifyNoNewCompilerDiagnostics(Document document, ImmutableArray<Diagnostic> compilerDiagnostics)
        {
            IEnumerable<Diagnostic> newCompilerDiagnostics = DiagnosticUtility.GetNewDiagnostics(compilerDiagnostics, document.GetCompilerDiagnostics());

            if (!newCompilerDiagnostics.Any())
                return;

            document = document.WithSyntaxRoot(Formatter.Format(document.GetSyntaxRootAsync().Result, Formatter.Annotation, document.Project.Solution.Workspace));

            newCompilerDiagnostics = DiagnosticUtility.GetNewDiagnostics(compilerDiagnostics, document.GetCompilerDiagnostics());

            Assert.True(false,
                $"Code fix introduced new compiler diagnostics{newCompilerDiagnostics.ToDebugString()}");
        }
    }
}
