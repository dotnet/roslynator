// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Text;
using Roslynator.Tests.Text;
using Xunit;

namespace Roslynator.Tests
{
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    public abstract class DiagnosticVerifier : CodeVerifier
    {
        public abstract DiagnosticDescriptor Descriptor { get; }

        public abstract DiagnosticAnalyzer Analyzer { get; }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string DebuggerDisplay
        {
            get { return $"{Descriptor.Id} {Analyzer.GetType().Name}"; }
        }

        public async Task VerifyDiagnosticAsync(
            string source,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            TestSourceTextAnalysis analysis = TestSourceText.GetSpans(source);

            await VerifyDiagnosticAsync(
                analysis.Source,
                analysis.Spans.Select(f => CreateDiagnostic(f.Span, f.LineSpan)),
                cancellationToken).ConfigureAwait(false);
        }

        public async Task VerifyDiagnosticAsync(
            string theory,
            string fromData,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            (string source, TextSpan span) = TestSourceText.ReplaceSpan(theory, fromData);

            TestSourceTextAnalysis analysis = TestSourceText.GetSpans(source);

            if (analysis.Spans.Any())
            {
                await VerifyDiagnosticAsync(analysis.Source, analysis.Spans.Select(f => f.Span), cancellationToken).ConfigureAwait(false);
            }
            else
            {
                await VerifyDiagnosticAsync(source, span, cancellationToken).ConfigureAwait(false);
            }
        }

        public async Task VerifyDiagnosticAsync(
            string source,
            TextSpan span,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            await VerifyDiagnosticAsync(
                source,
                CreateDiagnostic(source, span),
                cancellationToken).ConfigureAwait(false);
        }

        public async Task VerifyDiagnosticAsync(
            string source,
            IEnumerable<TextSpan> spans,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            await VerifyDiagnosticAsync(
                source,
                spans.Select(span => CreateDiagnostic(source, span)),
                cancellationToken).ConfigureAwait(false);
        }

        public async Task VerifyDiagnosticAsync(
            string source,
            Diagnostic diagnostic,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            await VerifyDiagnosticAsync(
                new string[] { source },
                new Diagnostic[] { diagnostic },
                cancellationToken).ConfigureAwait(false);
        }

        public async Task VerifyDiagnosticAsync(
            string source,
            IEnumerable<Diagnostic> expectedDiagnostics,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            await VerifyDiagnosticAsync(
                new string[] { source },
                expectedDiagnostics,
                cancellationToken).ConfigureAwait(false);
        }

        public async Task VerifyDiagnosticAsync(
            IEnumerable<string> sources,
            IEnumerable<Diagnostic> expectedDiagnostics,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            Project project = WorkspaceFactory.Project(sources);

            Compilation compilation = await project.GetCompilationAsync(cancellationToken).ConfigureAwait(false);

            ImmutableArray<Diagnostic> compilerDiagnostics = compilation.GetDiagnostics(cancellationToken);

            VerifyCompilerDiagnostics(compilerDiagnostics);

            if (Options.EnableDiagnosticsDisabledByDefault)
                compilation = compilation.EnableDiagnosticsDisabledByDefault(Analyzer);

            ImmutableArray<Diagnostic> diagnostics = await compilation.GetAnalyzerDiagnosticsAsync(Analyzer, DiagnosticComparer.SpanStart, cancellationToken).ConfigureAwait(false);

            if (diagnostics.Length > 0
                && Analyzer.SupportedDiagnostics.Length > 1)
            {
                VerifyDiagnostics(FilterDiagnostics(), expectedDiagnostics);
            }
            else
            {
                VerifyDiagnostics(diagnostics, expectedDiagnostics);
            }

            IEnumerable<Diagnostic> FilterDiagnostics()
            {
                foreach (Diagnostic diagnostic in diagnostics)
                {
                    bool success = false;
                    foreach (Diagnostic expectedDiagnostic in expectedDiagnostics)
                    {
                        if (DiagnosticComparer.Id.Equals(diagnostic, expectedDiagnostic))
                        {
                            success = true;
                            break;
                        }
                    }

                    if (success)
                        yield return diagnostic;
                }
            }
        }

        public async Task VerifyNoDiagnosticAsync(
            string theory,
            string fromData,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            (string source, TextSpan span) = TestSourceText.ReplaceSpan(theory, fromData);

            await VerifyNoDiagnosticAsync(source, cancellationToken).ConfigureAwait(false);
        }

        public async Task VerifyNoDiagnosticAsync(
            string source,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            await VerifyNoDiagnosticAsync(
                new string[] { source },
                cancellationToken).ConfigureAwait(false);
        }

        public async Task VerifyNoDiagnosticAsync(
            IEnumerable<string> sources,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (!Analyzer.Supports(Descriptor))
                Assert.True(false, $"Diagnostic \"{Descriptor.Id}\" is not supported by analyzer \"{Analyzer.GetType().Name}\".");

            Project project = WorkspaceFactory.Project(sources);

            Compilation compilation = await project.GetCompilationAsync(cancellationToken).ConfigureAwait(false);

            ImmutableArray<Diagnostic> compilerDiagnostics = compilation.GetDiagnostics(cancellationToken);

            VerifyCompilerDiagnostics(compilerDiagnostics);

            if (Options.EnableDiagnosticsDisabledByDefault)
                compilation = compilation.EnableDiagnosticsDisabledByDefault(Analyzer);

            ImmutableArray<Diagnostic> analyzerDiagnostics = await compilation.GetAnalyzerDiagnosticsAsync(Analyzer, DiagnosticComparer.SpanStart, cancellationToken).ConfigureAwait(false);

            if (analyzerDiagnostics.Any(f => string.Equals(f.Id, Descriptor.Id, StringComparison.Ordinal)))
                Assert.True(false, $"No diagnostic expected{analyzerDiagnostics.Where(f => string.Equals(f.Id, Descriptor.Id, StringComparison.Ordinal)).ToDebugString()}");
        }

        private void VerifyDiagnostics(
            IEnumerable<Diagnostic> actual,
            IEnumerable<Diagnostic> expected,
            bool checkAdditionalLocations = false)
        {
            int expectedCount = 0;
            int actualCount = 0;

            using (IEnumerator<Diagnostic> expectedEnumerator = expected.GetEnumerator())
            using (IEnumerator<Diagnostic> actualEnumerator = actual.GetEnumerator())
            {
                while (expectedEnumerator.MoveNext())
                {
                    expectedCount++;

                    Diagnostic expectedDiagnostic = expectedEnumerator.Current;

                    if (!Analyzer.Supports(expectedDiagnostic.Descriptor))
                        Assert.True(false, $"Diagnostic \"{expectedDiagnostic.Id}\" is not supported by analyzer \"{Analyzer.GetType().Name}\".");

                    if (actualEnumerator.MoveNext())
                    {
                        actualCount++;

                        VerifyDiagnostic(actualEnumerator.Current, expectedDiagnostic, checkAdditionalLocations: checkAdditionalLocations);
                    }
                    else
                    {
                        while (expectedEnumerator.MoveNext())
                            expectedCount++;

                        Assert.True(false, $"Mismatch between number of diagnostics returned, expected: {expectedCount} actual: {actualCount}{actual.ToDebugString()}");
                    }
                }

                if (actualEnumerator.MoveNext())
                {
                    actualCount++;

                    while (actualEnumerator.MoveNext())
                        actualCount++;

                    Assert.True(false, $"Mismatch between number of diagnostics returned, expected: {expectedCount} actual: {actualCount}{actual.ToDebugString()}");
                }
            }
        }

        private static void VerifyDiagnostic(
            Diagnostic actualDiagnostic,
            Diagnostic expectedDiagnostic,
            bool checkAdditionalLocations = false)
        {
            if (actualDiagnostic.Id != expectedDiagnostic.Descriptor.Id)
                Assert.True(false, $"Expected diagnostic id to be \"{expectedDiagnostic.Descriptor.Id}\" was \"{actualDiagnostic.Id}\"{GetMessage()}");

            VerifyLocation(actualDiagnostic.Location, expectedDiagnostic.Location);

            if (checkAdditionalLocations)
                VerifyAdditionalLocations(actualDiagnostic.AdditionalLocations, expectedDiagnostic.AdditionalLocations);

            void VerifyLocation(
                Location actualLocation,
                Location expectedLocation)
            {
                VerifyFileLinePositionSpan(actualLocation.GetLineSpan(), expectedLocation.GetLineSpan());
            }

            void VerifyAdditionalLocations(
                IReadOnlyList<Location> actual,
                IReadOnlyList<Location> expected)
            {
                int actualCount = actual.Count;
                int expectedCount = expected.Count;

                if (actualCount != expectedCount)
                    Assert.True(false, $"Expected {expectedCount} additional location(s), actual: {actualCount}{GetMessage()}");

                for (int j = 0; j < actualCount; j++)
                    VerifyLocation(actual[j], expected[j]);
            }

            void VerifyFileLinePositionSpan(
                FileLinePositionSpan actual,
                FileLinePositionSpan expected)
            {
                if (actual.Path != expected.Path)
                    Assert.True(false, $"Expected diagnostic to be in file \"{expected.Path}\", actual: \"{actual.Path}\"{GetMessage()}");

                VerifyLinePosition(actual.StartLinePosition, expected.StartLinePosition, "start");

                VerifyLinePosition(actual.EndLinePosition, expected.EndLinePosition, "end");
            }

            void VerifyLinePosition(
                LinePosition actual,
                LinePosition expected,
                string startOrEnd)
            {
                int actualLine = actual.Line;
                int expectedLine = expected.Line;

                if (actualLine != expectedLine)
                    Assert.True(false, $"Expected diagnostic to {startOrEnd} on line {expectedLine}, actual: {actualLine}{GetMessage()}");

                int actualCharacter = actual.Character;
                int expectedCharacter = expected.Character;

                if (actualCharacter != expectedCharacter)
                    Assert.True(false, $"Expected diagnostic to {startOrEnd} at column {expectedCharacter}, actual: {actualCharacter}{GetMessage()}");
            }

            string GetMessage()
            {
                return $"\r\n\r\nExpected diagnostic:\r\n{expectedDiagnostic}\r\n\r\nActual diagnostic:\r\n{actualDiagnostic}\r\n";
            }
        }

        private protected Diagnostic CreateDiagnostic(string source, TextSpan span)
        {
            LinePositionSpan lineSpan = span.ToLinePositionSpan(source);

            return CreateDiagnostic(span, lineSpan);
        }

        private protected Diagnostic CreateDiagnostic(TextSpan span, LinePositionSpan lineSpan)
        {
            Location location = Location.Create(FileUtility.DefaultCSharpFileName, span, lineSpan);

            return Diagnostic.Create(Descriptor, location);
        }
    }
}
