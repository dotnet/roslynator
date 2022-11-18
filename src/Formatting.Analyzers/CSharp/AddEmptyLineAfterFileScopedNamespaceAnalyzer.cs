// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Text;
using Roslynator.CSharp;

namespace Roslynator.Formatting.CSharp
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class AddEmptyLineAfterFileScopedNamespaceAnalyzer : BaseDiagnosticAnalyzer
    {
        private static ImmutableArray<DiagnosticDescriptor> _supportedDiagnostics;

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        {
            get
            {
                if (_supportedDiagnostics.IsDefault)
                    Immutable.InterlockedInitialize(ref _supportedDiagnostics, DiagnosticRules.AddEmptyLineAfterFileScopedNamespace);

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
            var fileScopedNamespace = (FileScopedNamespaceDeclarationSyntax)context.Node;

            MemberDeclarationSyntax member = fileScopedNamespace.Members.FirstOrDefault();

            if (member is not null
                && AnalyzeTrailingTrivia()
                && AnalyzeLeadingTrivia())
            {
                context.ReportDiagnostic(
                    DiagnosticRules.AddEmptyLineAfterFileScopedNamespace,
                    Location.Create(fileScopedNamespace.SyntaxTree, new TextSpan(member.FullSpan.Start, 0)));
            }

            bool AnalyzeTrailingTrivia()
            {
                SyntaxTriviaList.Enumerator en = fileScopedNamespace.SemicolonToken.TrailingTrivia.GetEnumerator();

                if (!en.MoveNext())
                    return true;

                if (en.Current.IsWhitespaceTrivia()
                    && !en.MoveNext())
                {
                    return true;
                }

                if (en.Current.IsKind(SyntaxKind.SingleLineCommentTrivia)
                    && !en.MoveNext())
                {
                    return true;
                }

                return en.Current.IsEndOfLineTrivia();
            }

            bool AnalyzeLeadingTrivia()
            {
                SyntaxTriviaList.Enumerator en = member.GetLeadingTrivia().GetEnumerator();

                if (!en.MoveNext())
                    return true;

                if (en.Current.IsWhitespaceTrivia()
                    && !en.MoveNext())
                {
                    return true;
                }

                return en.Current.IsWhitespaceTrivia();
            }
        }
    }
}
