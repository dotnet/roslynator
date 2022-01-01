// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static Roslynator.CSharp.CSharpFactory;
using System.Collections.Generic;

namespace Roslynator.CodeGeneration.CSharp
{
    internal class WrapRewriter : CSharpSyntaxRewriter
    {
        private int _classDeclarationDepth;
        private int _maxArgumentNameLength;
        private int _maxFieldDeclarationLength;

        public WrapRewriter(WrapRewriterOptions options)
        {
            Options = options;
        }

        public WrapRewriterOptions Options { get; }

        public override SyntaxNode VisitClassDeclaration(ClassDeclarationSyntax node)
        {
            IEnumerable<MemberDeclarationSyntax> fields = node.Members.Where(f => f.IsKind(SyntaxKind.FieldDeclaration));

            if (fields.Any())
            {
                _maxFieldDeclarationLength = fields
                    .Cast<FieldDeclarationSyntax>()
                    .Max(f => f.Declaration.Variables.First().Initializer.EqualsToken.SpanStart - f.SpanStart);
            }

            _classDeclarationDepth++;
            SyntaxNode result = base.VisitClassDeclaration(node);
            _classDeclarationDepth--;
            _maxFieldDeclarationLength = 0;

            return result;
        }

        public override SyntaxNode VisitFieldDeclaration(FieldDeclarationSyntax node)
        {
            node = (FieldDeclarationSyntax)base.VisitFieldDeclaration(node);

            if ((Options & WrapRewriterOptions.IndentFieldInitializer) != 0)
            {
                SyntaxToken equalsToken = node.Declaration.Variables.First().Initializer.EqualsToken;

                int count = _maxFieldDeclarationLength - (equalsToken.SpanStart - node.SpanStart);

                SyntaxToken newEqualsToken = equalsToken.AppendToLeadingTrivia(Whitespace(new string(' ', count)));

                node = node.ReplaceToken(equalsToken, newEqualsToken);
            }

            if ((Options & WrapRewriterOptions.WrapArguments) != 0)
            {
                return node.AppendToTrailingTrivia(NewLine());
            }
            else
            {
                return node;
            }
        }

        public override SyntaxNode VisitArgument(ArgumentSyntax node)
        {
            if ((Options & WrapRewriterOptions.WrapArguments) != 0
                && node.NameColon != null)
            {
                return node
                    .WithNameColon(node.NameColon.AppendToLeadingTrivia(TriviaList(NewLine(), Whitespace(new string(' ', 4 * (2 + _classDeclarationDepth))))))
                    .WithExpression(node.Expression.PrependToLeadingTrivia(Whitespace(new string(' ', _maxArgumentNameLength - node.NameColon.Name.Identifier.ValueText.Length))));
            }

            return node;
        }

        public override SyntaxNode VisitArgumentList(ArgumentListSyntax node)
        {
            _maxArgumentNameLength = node.Arguments.Max(f => f.NameColon?.Name.Identifier.ValueText.Length ?? 0);
            SyntaxNode result = base.VisitArgumentList(node);
            _maxArgumentNameLength = 0;

            return result;
        }

        public override SyntaxNode VisitAttribute(AttributeSyntax node)
        {
            return node;
        }
    }
}
