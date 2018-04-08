// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Roslynator
{
    /// <summary>
    /// A set of extension methods for <see cref="SymbolAnalysisContext"/>, <see cref="SyntaxNodeAnalysisContext"/> and <see cref="SyntaxTreeAnalysisContext"/>.
    /// </summary>
    public static class DiagnosticsExtensions
    {
        #region SymbolAnalysisContext
        /// <summary>
        /// Report a <see cref="Diagnostic"/> about a <see cref="ISymbol"/>.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="descriptor"></param>
        /// <param name="node"></param>
        /// <param name="messageArgs"></param>
        public static void ReportDiagnostic(
            this SymbolAnalysisContext context,
            DiagnosticDescriptor descriptor,
            SyntaxNode node,
            params object[] messageArgs)
        {
            if (node == null)
                throw new ArgumentNullException(nameof(node));

            ReportDiagnostic(
                context: context,
                descriptor: descriptor,
                location: node.GetLocation(),
                messageArgs: messageArgs);
        }

        /// <summary>
        /// Report a <see cref="Diagnostic"/> about a <see cref="ISymbol"/>.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="descriptor"></param>
        /// <param name="token"></param>
        /// <param name="messageArgs"></param>
        public static void ReportDiagnostic(
            this SymbolAnalysisContext context,
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

        /// <summary>
        /// Report a <see cref="Diagnostic"/> about a <see cref="ISymbol"/>.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="descriptor"></param>
        /// <param name="trivia"></param>
        /// <param name="messageArgs"></param>
        public static void ReportDiagnostic(
            this SymbolAnalysisContext context,
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

        /// <summary>
        /// Report a <see cref="Diagnostic"/> about a <see cref="ISymbol"/>.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="descriptor">A <see cref="DiagnosticDescriptor"/> describing the diagnostic.</param>
        /// <param name="location"></param>
        /// <param name="messageArgs"></param>
        public static void ReportDiagnostic(
            this SymbolAnalysisContext context,
            DiagnosticDescriptor descriptor,
            Location location,
            params object[] messageArgs)
        {
            context.ReportDiagnostic(Diagnostic.Create(
                descriptor: descriptor,
                location: location,
                messageArgs: messageArgs));
        }

        /// <summary>
        /// Report a <see cref="Diagnostic"/> about a <see cref="ISymbol"/>.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="descriptor"></param>
        /// <param name="location"></param>
        /// <param name="additionalLocations"></param>
        /// <param name="messageArgs"></param>
        public static void ReportDiagnostic(
            this SymbolAnalysisContext context,
            DiagnosticDescriptor descriptor,
            Location location,
            IEnumerable<Location> additionalLocations,
            params object[] messageArgs)
        {
            context.ReportDiagnostic(Diagnostic.Create(
                descriptor: descriptor,
                location: location,
                additionalLocations: additionalLocations,
                messageArgs: messageArgs));
        }

        /// <summary>
        /// Report a <see cref="Diagnostic"/> about a <see cref="ISymbol"/>.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="descriptor"></param>
        /// <param name="location"></param>
        /// <param name="properties"></param>
        /// <param name="messageArgs"></param>
        public static void ReportDiagnostic(
            this SymbolAnalysisContext context,
            DiagnosticDescriptor descriptor,
            Location location,
            ImmutableDictionary<string, string> properties,
            params object[] messageArgs)
        {
            context.ReportDiagnostic(Diagnostic.Create(
                descriptor: descriptor,
                location: location,
                properties: properties,
                messageArgs: messageArgs));
        }

        /// <summary>
        /// Report a <see cref="Diagnostic"/> about a <see cref="ISymbol"/>.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="descriptor"></param>
        /// <param name="location"></param>
        /// <param name="additionalLocations"></param>
        /// <param name="properties"></param>
        /// <param name="messageArgs"></param>
        public static void ReportDiagnostic(
            this SymbolAnalysisContext context,
            DiagnosticDescriptor descriptor,
            Location location,
            IEnumerable<Location> additionalLocations,
            ImmutableDictionary<string, string> properties,
            params object[] messageArgs)
        {
            context.ReportDiagnostic(Diagnostic.Create(
                descriptor: descriptor,
                location: location,
                additionalLocations: additionalLocations,
                properties: properties,
                messageArgs: messageArgs));
        }

        internal static INamedTypeSymbol GetTypeByMetadataName(this SymbolAnalysisContext context, string fullyQualifiedMetadataName)
        {
            return context.Compilation.GetTypeByMetadataName(fullyQualifiedMetadataName);
        }
        #endregion SymbolAnalysisContext

        #region SyntaxNodeAnalysisContext
        /// <summary>
        /// Report a <see cref="Diagnostic"/> about a <see cref="SyntaxNode"/>.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="descriptor"></param>
        /// <param name="node"></param>
        /// <param name="messageArgs"></param>
        public static void ReportDiagnostic(
            this SyntaxNodeAnalysisContext context,
            DiagnosticDescriptor descriptor,
            SyntaxNode node,
            params object[] messageArgs)
        {
            if (node == null)
                throw new ArgumentNullException(nameof(node));

            ReportDiagnostic(
                context: context,
                descriptor: descriptor,
                location: node.GetLocation(),
                messageArgs: messageArgs);
        }

        /// <summary>
        /// Report a <see cref="Diagnostic"/> about a <see cref="SyntaxNode"/>.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="descriptor"></param>
        /// <param name="token"></param>
        /// <param name="messageArgs"></param>
        public static void ReportDiagnostic(
            this SyntaxNodeAnalysisContext context,
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

        /// <summary>
        /// Report a <see cref="Diagnostic"/> about a <see cref="SyntaxNode"/>.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="descriptor"></param>
        /// <param name="trivia"></param>
        /// <param name="messageArgs"></param>
        public static void ReportDiagnostic(
            this SyntaxNodeAnalysisContext context,
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

        /// <summary>
        /// Report a <see cref="Diagnostic"/> about a <see cref="SyntaxNode"/>.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="descriptor"></param>
        /// <param name="location"></param>
        /// <param name="messageArgs"></param>
        public static void ReportDiagnostic(
            this SyntaxNodeAnalysisContext context,
            DiagnosticDescriptor descriptor,
            Location location,
            params object[] messageArgs)
        {
            context.ReportDiagnostic(Diagnostic.Create(
                descriptor: descriptor,
                location: location,
                messageArgs: messageArgs));
        }

        /// <summary>
        /// Report a <see cref="Diagnostic"/> about a <see cref="SyntaxNode"/>.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="descriptor"></param>
        /// <param name="location"></param>
        /// <param name="additionalLocations"></param>
        /// <param name="messageArgs"></param>
        public static void ReportDiagnostic(
            this SyntaxNodeAnalysisContext context,
            DiagnosticDescriptor descriptor,
            Location location,
            IEnumerable<Location> additionalLocations,
            params object[] messageArgs)
        {
            context.ReportDiagnostic(Diagnostic.Create(
                descriptor: descriptor,
                location: location,
                additionalLocations: additionalLocations,
                messageArgs: messageArgs));
        }

        /// <summary>
        /// Report a <see cref="Diagnostic"/> about a <see cref="SyntaxNode"/>.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="descriptor"></param>
        /// <param name="location"></param>
        /// <param name="properties"></param>
        /// <param name="messageArgs"></param>
        public static void ReportDiagnostic(
            this SyntaxNodeAnalysisContext context,
            DiagnosticDescriptor descriptor,
            Location location,
            ImmutableDictionary<string, string> properties,
            params object[] messageArgs)
        {
            context.ReportDiagnostic(Diagnostic.Create(
                descriptor: descriptor,
                location: location,
                properties: properties,
                messageArgs: messageArgs));
        }

        /// <summary>
        /// Report a <see cref="Diagnostic"/> about a <see cref="SyntaxNode"/>.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="descriptor"></param>
        /// <param name="location"></param>
        /// <param name="additionalLocations"></param>
        /// <param name="properties"></param>
        /// <param name="messageArgs"></param>
        public static void ReportDiagnostic(
            this SyntaxNodeAnalysisContext context,
            DiagnosticDescriptor descriptor,
            Location location,
            IEnumerable<Location> additionalLocations,
            ImmutableDictionary<string, string> properties,
            params object[] messageArgs)
        {
            context.ReportDiagnostic(Diagnostic.Create(
                descriptor: descriptor,
                location: location,
                additionalLocations: additionalLocations,
                properties: properties,
                messageArgs: messageArgs));
        }

        internal static INamedTypeSymbol GetTypeByMetadataName(
            this SyntaxNodeAnalysisContext context,
            string fullyQualifiedMetadataName)
        {
            return context.SemanticModel.Compilation.GetTypeByMetadataName(fullyQualifiedMetadataName);
        }

        internal static void ReportToken(this SyntaxNodeAnalysisContext context, DiagnosticDescriptor descriptor, SyntaxToken token, params object[] messageArgs)
        {
            if (!token.IsMissing)
                context.ReportDiagnostic(descriptor, token, messageArgs);
        }

        internal static void ReportNode(this SyntaxNodeAnalysisContext context, DiagnosticDescriptor descriptor, SyntaxNode node, params object[] messageArgs)
        {
            if (!node.IsMissing)
                context.ReportDiagnostic(descriptor, node, messageArgs);
        }
        #endregion SyntaxNodeAnalysisContext

        #region SyntaxTreeAnalysisContext
        /// <summary>
        /// Report a <see cref="Diagnostic"/> about a <see cref="SyntaxTree"/>.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="descriptor"></param>
        /// <param name="node"></param>
        /// <param name="messageArgs"></param>
        public static void ReportDiagnostic(
            this SyntaxTreeAnalysisContext context,
            DiagnosticDescriptor descriptor,
            SyntaxNode node,
            params object[] messageArgs)
        {
            if (node == null)
                throw new ArgumentNullException(nameof(node));

            ReportDiagnostic(
                context: context,
                descriptor: descriptor,
                location: node.GetLocation(),
                messageArgs: messageArgs);
        }

        /// <summary>
        /// Report a <see cref="Diagnostic"/> about a <see cref="SyntaxTree"/>.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="descriptor"></param>
        /// <param name="token"></param>
        /// <param name="messageArgs"></param>
        public static void ReportDiagnostic(
            this SyntaxTreeAnalysisContext context,
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

        /// <summary>
        /// Report a <see cref="Diagnostic"/> about a <see cref="SyntaxTree"/>.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="descriptor"></param>
        /// <param name="trivia"></param>
        /// <param name="messageArgs"></param>
        public static void ReportDiagnostic(
            this SyntaxTreeAnalysisContext context,
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

        /// <summary>
        /// Report a <see cref="Diagnostic"/> about a <see cref="SyntaxTree"/>.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="descriptor"></param>
        /// <param name="location"></param>
        /// <param name="messageArgs"></param>
        public static void ReportDiagnostic(
            this SyntaxTreeAnalysisContext context,
            DiagnosticDescriptor descriptor,
            Location location,
            params object[] messageArgs)
        {
            context.ReportDiagnostic(Diagnostic.Create(
                descriptor: descriptor,
                location: location,
                messageArgs: messageArgs));
        }

        /// <summary>
        /// Report a <see cref="Diagnostic"/> about a <see cref="SyntaxTree"/>.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="descriptor"></param>
        /// <param name="location"></param>
        /// <param name="additionalLocations"></param>
        /// <param name="messageArgs"></param>
        public static void ReportDiagnostic(
            this SyntaxTreeAnalysisContext context,
            DiagnosticDescriptor descriptor,
            Location location,
            IEnumerable<Location> additionalLocations,
            params object[] messageArgs)
        {
            context.ReportDiagnostic(Diagnostic.Create(
                descriptor: descriptor,
                location: location,
                additionalLocations: additionalLocations,
                messageArgs: messageArgs));
        }

        /// <summary>
        /// Report a <see cref="Diagnostic"/> about a <see cref="SyntaxTree"/>.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="descriptor"></param>
        /// <param name="location"></param>
        /// <param name="properties"></param>
        /// <param name="messageArgs"></param>
        public static void ReportDiagnostic(
            this SyntaxTreeAnalysisContext context,
            DiagnosticDescriptor descriptor,
            Location location,
            ImmutableDictionary<string, string> properties,
            params object[] messageArgs)
        {
            context.ReportDiagnostic(Diagnostic.Create(
                descriptor: descriptor,
                location: location,
                properties: properties,
                messageArgs: messageArgs));
        }

        /// <summary>
        /// Report a <see cref="Diagnostic"/> about a <see cref="SyntaxTree"/>.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="descriptor"></param>
        /// <param name="location"></param>
        /// <param name="additionalLocations"></param>
        /// <param name="properties"></param>
        /// <param name="messageArgs"></param>
        public static void ReportDiagnostic(
            this SyntaxTreeAnalysisContext context,
            DiagnosticDescriptor descriptor,
            Location location,
            IEnumerable<Location> additionalLocations,
            ImmutableDictionary<string, string> properties,
            params object[] messageArgs)
        {
            context.ReportDiagnostic(Diagnostic.Create(
                descriptor: descriptor,
                location: location,
                additionalLocations: additionalLocations,
                properties: properties,
                messageArgs: messageArgs));
        }
        #endregion SyntaxTreeAnalysisContext
    }
}
