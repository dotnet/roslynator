// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

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
            if (IsSuppressed(descriptor))
                return;

            context.ReportDiagnostic(Diagnostic.Create(
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
            if (IsSuppressed(descriptor))
                return;

            context.ReportDiagnostic(Diagnostic.Create(
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
            if (IsSuppressed(descriptor))
                return;

            context.ReportDiagnostic(Diagnostic.Create(
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
            if (IsSuppressed(descriptor))
                return;

            context.ReportDiagnostic(Diagnostic.Create(
                descriptor: descriptor,
                location: location,
                additionalLocations: additionalLocations,
                properties: properties,
                messageArgs: messageArgs));
        }
        #endregion SymbolAnalysisContext

        #region SyntaxNodeAnalysisContext
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
            if (IsSuppressed(descriptor))
                return;

            context.ReportDiagnostic(Diagnostic.Create(
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
            if (IsSuppressed(descriptor))
                return;

            context.ReportDiagnostic(Diagnostic.Create(
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
            if (IsSuppressed(descriptor))
                return;

            context.ReportDiagnostic(Diagnostic.Create(
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
            if (IsSuppressed(descriptor))
                return;

            context.ReportDiagnostic(Diagnostic.Create(
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
            if (IsSuppressed(descriptor))
                return;

            context.ReportDiagnostic(Diagnostic.Create(
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
            if (IsSuppressed(descriptor))
                return;

            context.ReportDiagnostic(Diagnostic.Create(
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
            if (IsSuppressed(descriptor))
                return;

            context.ReportDiagnostic(Diagnostic.Create(
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
            if (IsSuppressed(descriptor))
                return;

            context.ReportDiagnostic(Diagnostic.Create(
                descriptor: descriptor,
                location: location,
                additionalLocations: additionalLocations,
                properties: properties,
                messageArgs: messageArgs));
        }
        #endregion SyntaxTreeAnalysisContext

        private static bool IsSuppressed(DiagnosticDescriptor descriptor)
        {
            Debug.WriteLineIf(!AnalyzerSettings.Current.IsEnabled(descriptor.Id), $"{descriptor.Id} suppressed");

            return !AnalyzerSettings.Current.IsEnabled(descriptor.Id);
        }
    }
}
