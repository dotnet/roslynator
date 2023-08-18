// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Text;
using Roslynator.CSharp;
using Roslynator.CSharp.CodeStyle;

namespace Roslynator.Formatting.CSharp;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class BlankLineAfterFileScopedNamespaceDeclarationAnalyzer : BaseDiagnosticAnalyzer
{
    private static ImmutableArray<DiagnosticDescriptor> _supportedDiagnostics;

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
    {
        get
        {
            if (_supportedDiagnostics.IsDefault)
                Immutable.InterlockedInitialize(ref _supportedDiagnostics, DiagnosticRules.BlankLineAfterFileScopedNamespaceDeclaration);

            return _supportedDiagnostics;
        }
    }

    public override void Initialize(AnalysisContext context)
    {
        base.Initialize(context);

        context.RegisterSyntaxNodeAction(f => AnalyzeFileScopedNamespaceDeclaration(f), SyntaxKind.FileScopedNamespaceDeclaration);
    }

    private static void AnalyzeFileScopedNamespaceDeclaration(SyntaxNodeAnalysisContext context)
    {
        var namespaceDeclaration = (FileScopedNamespaceDeclarationSyntax)context.Node;

        SyntaxNode node = GetNodeAfterNamespaceDeclaration(namespaceDeclaration);

        if (node is null)
            return;

        BlankLineStyle style = context.GetBlankLineAfterFileScopedNamespaceDeclaration();

        if (style == BlankLineStyle.None)
            return;

        BlankLineStyle currentStyle = GetCurrentStyle(namespaceDeclaration, node);

        if (style != currentStyle)
            return;

        context.ReportDiagnostic(
            DiagnosticRules.BlankLineAfterFileScopedNamespaceDeclaration,
            Location.Create(namespaceDeclaration.SyntaxTree, new TextSpan(node.FullSpan.Start, 0)),
            (style == BlankLineStyle.Add) ? "Add" : "Remove");
    }

    internal static BlankLineStyle GetCurrentStyle(
        FileScopedNamespaceDeclarationSyntax namespaceDeclaration,
        SyntaxNode node)
    {
        (bool add, bool remove) = AnalyzeTrailingTrivia();

        if (add || remove)
        {
            BlankLineStyle style = AnalyzeLeadingTrivia();

            if (style == BlankLineStyle.Add)
            {
                if (add)
                    return style;
            }
            else if (style == BlankLineStyle.Remove)
            {
                if (remove)
                    return style;
            }
        }

        return BlankLineStyle.None;

        (bool add, bool remove) AnalyzeTrailingTrivia()
        {
            SyntaxTriviaList.Enumerator en = namespaceDeclaration.SemicolonToken.TrailingTrivia.GetEnumerator();

            if (!en.MoveNext())
                return (true, false);

            if (en.Current.IsWhitespaceTrivia()
                && !en.MoveNext())
            {
                return (true, false);
            }

            if (en.Current.IsKind(SyntaxKind.SingleLineCommentTrivia)
                && !en.MoveNext())
            {
                return (true, false);
            }

            return (en.Current.IsEndOfLineTrivia())
                ? (true, true)
                : (false, false);
        }

        BlankLineStyle AnalyzeLeadingTrivia()
        {
            SyntaxTriviaList.Enumerator en = node.GetLeadingTrivia().GetEnumerator();

            if (!en.MoveNext())
                return BlankLineStyle.Add;

            if (en.Current.IsWhitespaceTrivia()
                && !en.MoveNext())
            {
                return BlankLineStyle.Add;
            }

            switch (en.Current.Kind())
            {
                case SyntaxKind.SingleLineCommentTrivia:
                case SyntaxKind.SingleLineDocumentationCommentTrivia:
                    return BlankLineStyle.Add;
                case SyntaxKind.EndOfLineTrivia:
                    return BlankLineStyle.Remove;
            }

            return BlankLineStyle.None;
        }
    }

    internal static SyntaxNode GetNodeAfterNamespaceDeclaration(FileScopedNamespaceDeclarationSyntax namespaceDeclaration)
    {
        MemberDeclarationSyntax memberDeclaration = namespaceDeclaration.Members.FirstOrDefault();
        UsingDirectiveSyntax usingDirective = namespaceDeclaration.Usings.FirstOrDefault();

        return (usingDirective?.SpanStart > namespaceDeclaration.SpanStart)
            ? usingDirective
            : memberDeclaration;
    }
}
