// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Text.RegularExpressions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

#pragma warning disable RCS0047

namespace Roslynator
{
    internal static class DiagnosticHelpers
    {
        #region SymbolAnalysisContext
        public static void ReportDiagnostic(
            SymbolAnalysisContext context,
            DiagnosticDescriptor descriptor,
            SyntaxNode node,
            params object[] messageArgs)
        {
            ReportDiagnostic(
                context: context,
                descriptor: descriptor,
                location: node.GetLocation(),
                messageArgs: messageArgs);
        }

        public static void ReportDiagnostic(
            SymbolAnalysisContext context,
            DiagnosticDescriptor descriptor,
            SyntaxToken token,
            params object[] messageArgs)
        {
            ReportDiagnostic(
                context: context,
                descriptor: descriptor,
                location: token.GetLocation(),
                messageArgs: messageArgs);
        }

        public static void ReportDiagnostic(
            SymbolAnalysisContext context,
            DiagnosticDescriptor descriptor,
            SyntaxTrivia trivia,
            params object[] messageArgs)
        {
            ReportDiagnostic(
                context: context,
                descriptor: descriptor,
                location: trivia.GetLocation(),
                messageArgs: messageArgs);
        }

        public static void ReportDiagnostic(
            SymbolAnalysisContext context,
            DiagnosticDescriptor descriptor,
            Location location,
            params object[] messageArgs)
        {
            ReportDiagnostic(
                context,
                Diagnostic.Create(
                    descriptor: descriptor,
                    location: location,
                    messageArgs: messageArgs));
        }

        public static void ReportDiagnostic(
            SymbolAnalysisContext context,
            DiagnosticDescriptor descriptor,
            Location location,
            IEnumerable<Location> additionalLocations,
            params object[] messageArgs)
        {
            ReportDiagnostic(
                context,
                Diagnostic.Create(
                    descriptor: descriptor,
                    location: location,
                    additionalLocations: additionalLocations,
                    messageArgs: messageArgs));
        }

        public static void ReportDiagnostic(
            SymbolAnalysisContext context,
            DiagnosticDescriptor descriptor,
            Location location,
            ImmutableDictionary<string, string> properties,
            params object[] messageArgs)
        {
            ReportDiagnostic(
                context,
                Diagnostic.Create(
                    descriptor: descriptor,
                    location: location,
                    properties: properties,
                    messageArgs: messageArgs));
        }

        public static void ReportDiagnostic(
            SymbolAnalysisContext context,
            DiagnosticDescriptor descriptor,
            Location location,
            IEnumerable<Location> additionalLocations,
            ImmutableDictionary<string, string> properties,
            params object[] messageArgs)
        {
            ReportDiagnostic(
                context,
                Diagnostic.Create(
                    descriptor: descriptor,
                    location: location,
                    additionalLocations: additionalLocations,
                    properties: properties,
                    messageArgs: messageArgs));
        }

        private static void ReportDiagnostic(SymbolAnalysisContext context, Diagnostic diagnostic)
        {
            VerifyDiagnostic(diagnostic);

            context.ReportDiagnostic(diagnostic);
        }
        #endregion SymbolAnalysisContext

        #region SyntaxNodeAnalysisContext
        public static void ReportDiagnosticIfNotSuppressed(
            SyntaxNodeAnalysisContext context,
            DiagnosticDescriptor descriptor,
            SyntaxNode node,
            params object[] messageArgs)
        {
            if (context.IsAnalyzerSuppressed(descriptor))
                return;

            ReportDiagnostic(context, descriptor, node, messageArgs);
        }

        public static void ReportDiagnosticIfNotSuppressed(
            SyntaxNodeAnalysisContext context,
            DiagnosticDescriptor descriptor,
            Location location,
            params object[] messageArgs)
        {
            if (context.IsAnalyzerSuppressed(descriptor))
                return;

            ReportDiagnostic(context, descriptor, location, messageArgs);
        }

        public static void ReportDiagnostic(
            SyntaxNodeAnalysisContext context,
            DiagnosticDescriptor descriptor,
            SyntaxNode node,
            params object[] messageArgs)
        {
            ReportDiagnostic(
                context: context,
                descriptor: descriptor,
                location: node.GetLocation(),
                messageArgs: messageArgs);
        }

        public static void ReportDiagnostic(
            SyntaxNodeAnalysisContext context,
            DiagnosticDescriptor descriptor,
            SyntaxToken token,
            params object[] messageArgs)
        {
            ReportDiagnostic(
                context: context,
                descriptor: descriptor,
                location: token.GetLocation(),
                messageArgs: messageArgs);
        }

        public static void ReportDiagnostic(
            SyntaxNodeAnalysisContext context,
            DiagnosticDescriptor descriptor,
            SyntaxTrivia trivia,
            params object[] messageArgs)
        {
            ReportDiagnostic(
                context: context,
                descriptor: descriptor,
                location: trivia.GetLocation(),
                messageArgs: messageArgs);
        }

        public static void ReportDiagnostic(
            SyntaxNodeAnalysisContext context,
            DiagnosticDescriptor descriptor,
            Location location,
            params object[] messageArgs)
        {
            ReportDiagnostic(
                context,
                Diagnostic.Create(
                    descriptor: descriptor,
                    location: location,
                    messageArgs: messageArgs));
        }

        public static void ReportDiagnostic(
            SyntaxNodeAnalysisContext context,
            DiagnosticDescriptor descriptor,
            Location location,
            IEnumerable<Location> additionalLocations,
            params object[] messageArgs)
        {
            ReportDiagnostic(
                context,
                Diagnostic.Create(
                    descriptor: descriptor,
                    location: location,
                    additionalLocations: additionalLocations,
                    messageArgs: messageArgs));
        }

        public static void ReportDiagnostic(
            SyntaxNodeAnalysisContext context,
            DiagnosticDescriptor descriptor,
            Location location,
            ImmutableDictionary<string, string> properties,
            params object[] messageArgs)
        {
            ReportDiagnostic(
                context,
                Diagnostic.Create(
                    descriptor: descriptor,
                    location: location,
                    properties: properties,
                    messageArgs: messageArgs));
        }

        public static void ReportDiagnostic(
            SyntaxNodeAnalysisContext context,
            DiagnosticDescriptor descriptor,
            Location location,
            IEnumerable<Location> additionalLocations,
            ImmutableDictionary<string, string> properties,
            params object[] messageArgs)
        {
            ReportDiagnostic(
                context,
                Diagnostic.Create(
                    descriptor: descriptor,
                    location: location,
                    additionalLocations: additionalLocations,
                    properties: properties,
                    messageArgs: messageArgs));
        }

        public static void ReportToken(SyntaxNodeAnalysisContext context, DiagnosticDescriptor descriptor, SyntaxToken token, params object[] messageArgs)
        {
            if (!token.IsMissing)
                ReportDiagnostic(context, descriptor, token, messageArgs);
        }

        public static void ReportNode(SyntaxNodeAnalysisContext context, DiagnosticDescriptor descriptor, SyntaxNode node, params object[] messageArgs)
        {
            if (!node.IsMissing)
                ReportDiagnostic(context, descriptor, node, messageArgs);
        }

        public static void ReportDiagnostic(SyntaxNodeAnalysisContext context, Diagnostic diagnostic)
        {
            VerifyDiagnostic(diagnostic);

            context.ReportDiagnostic(diagnostic);
        }
        #endregion SyntaxNodeAnalysisContext

        #region SyntaxTreeAnalysisContext
        public static void ReportDiagnostic(
            SyntaxTreeAnalysisContext context,
            DiagnosticDescriptor descriptor,
            SyntaxNode node,
            params object[] messageArgs)
        {
            ReportDiagnostic(
                context: context,
                descriptor: descriptor,
                location: node.GetLocation(),
                messageArgs: messageArgs);
        }

        public static void ReportDiagnostic(
            SyntaxTreeAnalysisContext context,
            DiagnosticDescriptor descriptor,
            SyntaxToken token,
            params object[] messageArgs)
        {
            ReportDiagnostic(
                context: context,
                descriptor: descriptor,
                location: token.GetLocation(),
                messageArgs: messageArgs);
        }

        public static void ReportDiagnostic(
            SyntaxTreeAnalysisContext context,
            DiagnosticDescriptor descriptor,
            SyntaxTrivia trivia,
            params object[] messageArgs)
        {
            ReportDiagnostic(
                context: context,
                descriptor: descriptor,
                location: trivia.GetLocation(),
                messageArgs: messageArgs);
        }

        public static void ReportDiagnostic(
            SyntaxTreeAnalysisContext context,
            DiagnosticDescriptor descriptor,
            Location location,
            params object[] messageArgs)
        {
            ReportDiagnostic(
                context,
                Diagnostic.Create(
                    descriptor: descriptor,
                    location: location,
                    messageArgs: messageArgs));
        }

        public static void ReportDiagnostic(
            SyntaxTreeAnalysisContext context,
            DiagnosticDescriptor descriptor,
            Location location,
            IEnumerable<Location> additionalLocations,
            params object[] messageArgs)
        {
            ReportDiagnostic(
                context,
                Diagnostic.Create(
                    descriptor: descriptor,
                    location: location,
                    additionalLocations: additionalLocations,
                    messageArgs: messageArgs));
        }

        public static void ReportDiagnostic(
            SyntaxTreeAnalysisContext context,
            DiagnosticDescriptor descriptor,
            Location location,
            ImmutableDictionary<string, string> properties,
            params object[] messageArgs)
        {
            ReportDiagnostic(
                context,
                Diagnostic.Create(
                    descriptor: descriptor,
                    location: location,
                    properties: properties,
                    messageArgs: messageArgs));
        }

        public static void ReportDiagnostic(
            SyntaxTreeAnalysisContext context,
            DiagnosticDescriptor descriptor,
            Location location,
            IEnumerable<Location> additionalLocations,
            ImmutableDictionary<string, string> properties,
            params object[] messageArgs)
        {
            ReportDiagnostic(
                context,
                Diagnostic.Create(
                    descriptor: descriptor,
                    location: location,
                    additionalLocations: additionalLocations,
                    properties: properties,
                    messageArgs: messageArgs));
        }

        private static void ReportDiagnostic(SyntaxTreeAnalysisContext context, Diagnostic diagnostic)
        {
            VerifyDiagnostic(diagnostic);

            context.ReportDiagnostic(diagnostic);
        }
        #endregion SyntaxTreeAnalysisContext

        [Conditional("DEBUG")]
        private static void VerifyDiagnostic(Diagnostic diagnostic)
        {
            Debug.Assert(Regex.IsMatch(diagnostic.Id, @"\ARCS[0-9]{4}(FadeOut)?\z"), $"Invalid diagnostic id '{diagnostic.Id}'.");
        }
    }
}
