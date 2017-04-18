// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Roslynator
{
    public static class AnalysisContextExtensions
    {
        #region SymbolAnalysisContextExtensions
        public static void ReportDiagnostic(
            this SymbolAnalysisContext context,
            DiagnosticDescriptor descriptor,
            SyntaxNode node,
            params object[] messageArgs)
        {
            if (node == null)
                throw new ArgumentNullException(nameof(node));

            context.ReportDiagnostic(
                Diagnostic.Create(descriptor, node.GetLocation(), messageArgs));
        }

        public static void ReportDiagnostic(
            this SymbolAnalysisContext context,
            DiagnosticDescriptor descriptor,
            SyntaxToken token,
            params object[] messageArgs)
        {
            context.ReportDiagnostic(
                Diagnostic.Create(descriptor, token.GetLocation(), messageArgs));
        }

        public static void ReportDiagnostic(
            this SymbolAnalysisContext context,
            DiagnosticDescriptor descriptor,
            SyntaxTrivia trivia,
            params object[] messageArgs)
        {
            context.ReportDiagnostic(
                Diagnostic.Create(descriptor, trivia.GetLocation(), messageArgs));
        }

        public static void ReportDiagnostic(
            this SymbolAnalysisContext context,
            DiagnosticDescriptor descriptor,
            Location location,
            params object[] messageArgs)
        {
            context.ReportDiagnostic(
                Diagnostic.Create(descriptor, location, messageArgs));
        }

        internal static INamedTypeSymbol GetTypeByMetadataName(this SymbolAnalysisContext context, string fullyQualifiedMetadataName)
        {
            return context.Compilation.GetTypeByMetadataName(fullyQualifiedMetadataName);
        }
        #endregion

        #region SyntaxNodeAnalysisContextExtensions
        public static void ReportDiagnostic(
            this SyntaxNodeAnalysisContext context,
            DiagnosticDescriptor descriptor,
            SyntaxNode node,
            params object[] messageArgs)
        {
            if (node == null)
                throw new ArgumentNullException(nameof(node));

            context.ReportDiagnostic(
            Diagnostic.Create(descriptor, node.GetLocation(), messageArgs));
        }

        public static void ReportDiagnostic(
            this SyntaxNodeAnalysisContext context,
            DiagnosticDescriptor descriptor,
            SyntaxToken token,
            params object[] messageArgs)
        {
            context.ReportDiagnostic(
                Diagnostic.Create(descriptor, token.GetLocation(), messageArgs));
        }

        public static void ReportDiagnostic(
            this SyntaxNodeAnalysisContext context,
            DiagnosticDescriptor descriptor,
            SyntaxTrivia trivia,
            params object[] messageArgs)
        {
            context.ReportDiagnostic(
                Diagnostic.Create(descriptor, trivia.GetLocation(), messageArgs));
        }

        public static void ReportDiagnostic(
            this SyntaxNodeAnalysisContext context,
            DiagnosticDescriptor descriptor,
            Location location,
            params object[] messageArgs)
        {
            context.ReportDiagnostic(
                Diagnostic.Create(descriptor, location, messageArgs));
        }

        public static void ReportDiagnostic(
            this SyntaxNodeAnalysisContext context,
            DiagnosticDescriptor descriptor,
            Location location,
            ImmutableDictionary<string, string> properties,
            params object[] messageArgs)
        {
            context.ReportDiagnostic(
                Diagnostic.Create(descriptor, location, properties, messageArgs));
        }

        public static void ReportDiagnostic(
            this SyntaxNodeAnalysisContext context,
            DiagnosticDescriptor descriptor,
            Location location,
            IEnumerable<Location> additionalLocations,
            params object[] messageArgs)
        {
            context.ReportDiagnostic(
                Diagnostic.Create(descriptor, location, additionalLocations, messageArgs));
        }

        public static INamedTypeSymbol GetTypeByMetadataName(
            this SyntaxNodeAnalysisContext context,
            string fullyQualifiedMetadataName)
        {
            return context.SemanticModel.Compilation.GetTypeByMetadataName(fullyQualifiedMetadataName);
        }

        public static SyntaxTree SyntaxTree(this SyntaxNodeAnalysisContext context)
        {
            return context.Node.SyntaxTree;
        }

        internal static void ReportToken(this SyntaxNodeAnalysisContext context, DiagnosticDescriptor descriptor, SyntaxToken token)
        {
            if (!token.IsMissing)
                context.ReportDiagnostic(descriptor, token);
        }

        internal static void ReportNode(this SyntaxNodeAnalysisContext context, DiagnosticDescriptor descriptor, SyntaxNode node)
        {
            if (!node.IsMissing)
                context.ReportDiagnostic(descriptor, node);
        }

        internal static void ReportBraces(this SyntaxNodeAnalysisContext context, DiagnosticDescriptor descriptor, BlockSyntax block)
        {
            ReportToken(context, descriptor, block.OpenBraceToken);
            ReportToken(context, descriptor, block.CloseBraceToken);
        }

        internal static void ReportBraces(this SyntaxNodeAnalysisContext context, DiagnosticDescriptor descriptor, AccessorListSyntax accessorList)
        {
            ReportToken(context, descriptor, accessorList.OpenBraceToken);
            ReportToken(context, descriptor, accessorList.CloseBraceToken);
        }

        internal static void ReportParentheses(this SyntaxNodeAnalysisContext context, DiagnosticDescriptor descriptor, ArgumentListSyntax argumentList)
        {
            ReportToken(context, descriptor, argumentList.OpenParenToken);
            ReportToken(context, descriptor, argumentList.CloseParenToken);
        }
        #endregion

        #region SyntaxTreeAnalysisContextExtensions
        public static void ReportDiagnostic(
            this SyntaxTreeAnalysisContext context,
            DiagnosticDescriptor descriptor,
            SyntaxNode node,
            params object[] messageArgs)
        {
            if (node == null)
                throw new ArgumentNullException(nameof(node));

            context.ReportDiagnostic(
                Diagnostic.Create(descriptor, node.GetLocation(), messageArgs));
        }

        public static void ReportDiagnostic(
            this SyntaxTreeAnalysisContext context,
            DiagnosticDescriptor descriptor,
            SyntaxToken token,
            params object[] messageArgs)
        {
            context.ReportDiagnostic(
                Diagnostic.Create(descriptor, token.GetLocation(), messageArgs));
        }

        public static void ReportDiagnostic(
            this SyntaxTreeAnalysisContext context,
            DiagnosticDescriptor descriptor,
            SyntaxTrivia trivia,
            params object[] messageArgs)
        {
            context.ReportDiagnostic(
                Diagnostic.Create(descriptor, trivia.GetLocation(), messageArgs));
        }

        public static void ReportDiagnostic(
            this SyntaxTreeAnalysisContext context,
            DiagnosticDescriptor descriptor,
            Location location,
            params object[] messageArgs)
        {
            context.ReportDiagnostic(
                Diagnostic.Create(descriptor, location, messageArgs));
        }
        #endregion
    }
}
