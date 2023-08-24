// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Roslynator.CSharp;

namespace Roslynator;

internal static class CommonWorkspaceExtensions
{
    public static bool TryGetAnalyzerOptionValue(
        this Document document,
        SyntaxNode node,
        string optionKey,
        out string value)
    {
        return TryGetAnalyzerOptionValue(document, node.SyntaxTree, optionKey, out value);
    }

    public static bool TryGetAnalyzerOptionValue(
        this Document document,
        SyntaxToken token,
        string optionKey,
        out string value)
    {
        return TryGetAnalyzerOptionValue(document, token.SyntaxTree, optionKey, out value);
    }

    public static bool TryGetAnalyzerOptionValue(
        this Document document,
        SyntaxTree syntaxTree,
        string optionKey,
        out string value)
    {
        if (document
            .Project
            .AnalyzerOptions
            .AnalyzerConfigOptionsProvider
            .GetOptions(syntaxTree)
            .TryGetValue(optionKey, out value))
        {
            return true;
        }

        value = null;
        return false;
    }

    internal static AnalyzerConfigOptions GetConfigOptions(this Document document, SyntaxTree syntaxTree)
    {
        return document.Project.AnalyzerOptions.AnalyzerConfigOptionsProvider.GetOptions(syntaxTree);
    }

    internal static string GetIncreasedIndentation(this Document document, SyntaxNode node, CancellationToken cancellationToken = default)
    {
        if (document is null)
            throw new ArgumentNullException(nameof(document));

        if (node is null)
            throw new ArgumentNullException(nameof(node));

        AnalyzerConfigOptions configOptions = document.GetConfigOptions(node.SyntaxTree);

        bool hasIndentSize = configOptions.TryGetIndentSize(out int indentSize);

        bool useTabs = configOptions.TryGetIndentStyle(out IndentStyle indentStyle)
            && indentStyle == IndentStyle.Tab;

        if (useTabs)
        {
            SyntaxTrivia indentation = SyntaxTriviaAnalysis.DetermineIndentation(node, cancellationToken);

            return indentation.ToString() + "\t";
        }
        else if (hasIndentSize)
        {
            SyntaxTrivia indentation = SyntaxTriviaAnalysis.DetermineIndentation(node, cancellationToken);

            return indentation.ToString() + new string(' ', indentSize);
        }
        else
        {
            return IndentationAnalysis.Create(node, cancellationToken).GetIncreasedIndentation();
        }
    }

    internal static int GetIncreasedIndentationLength(this Document document, SyntaxNode node, CancellationToken cancellationToken = default)
    {
        if (document is null)
            throw new ArgumentNullException(nameof(document));

        if (node is null)
            throw new ArgumentNullException(nameof(node));

        AnalyzerConfigOptions configOptions = document.GetConfigOptions(node.SyntaxTree);

        bool hasIndentSize = configOptions.TryGetIndentSize(out int indentSize);

        bool useTabs = configOptions.TryGetIndentStyle(out IndentStyle indentStyle)
            && indentStyle == IndentStyle.Tab;

        if (useTabs)
        {
            if (!configOptions.TryGetTabLength(out int tabLength))
                tabLength = 4;

            SyntaxTrivia indentation = SyntaxTriviaAnalysis.DetermineIndentation(node, cancellationToken);
            return (indentation.Span.Length + 1) * tabLength;
        }
        else if (hasIndentSize)
        {
            SyntaxTrivia indentation = SyntaxTriviaAnalysis.DetermineIndentation(node, cancellationToken);

            return indentation.Span.Length + indentSize;
        }
        else
        {
            return IndentationAnalysis.Create(node, cancellationToken).IncreasedIndentationLength;
        }
    }
}
