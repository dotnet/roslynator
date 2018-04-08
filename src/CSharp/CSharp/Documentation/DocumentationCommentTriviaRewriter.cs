// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Roslynator.CSharp.Documentation
{
    internal class DocumentationCommentTriviaRewriter : CSharpSyntaxRewriter
    {
        private readonly SemanticModel _semanticModel;

        private readonly int _position;

        public DocumentationCommentTriviaRewriter(int position, SemanticModel semanticModel)
            : base(visitIntoStructuredTrivia: true)
        {
            _semanticModel = semanticModel ?? throw new ArgumentNullException(nameof(semanticModel));
            _position = position;
        }

        public override SyntaxNode VisitXmlTextAttribute(XmlTextAttributeSyntax node)
        {
            if (node.Name?.IsLocalName("cref", StringComparison.OrdinalIgnoreCase) == true)
            {
                SyntaxTokenList tokens = node.TextTokens;

                if (tokens.Count == 1)
                {
                    SyntaxToken token = tokens.First();

                    string text = token.Text;

                    string valueText = token.ValueText;

                    if (text.StartsWith("T:", StringComparison.Ordinal))
                        text = GetMinimalDisplayString(text.Substring(2));

                    if (valueText.StartsWith("T:", StringComparison.Ordinal))
                        valueText = GetMinimalDisplayString(valueText.Substring(2));

                    SyntaxToken newToken = SyntaxFactory.Token(
                        default(SyntaxTriviaList),
                        SyntaxKind.XmlTextLiteralToken,
                        text,
                        valueText,
                        default(SyntaxTriviaList));

                    return node.WithTextTokens(tokens.Replace(token, newToken));
                }
            }

            return base.VisitXmlTextAttribute(node);
        }

        private string GetMinimalDisplayString(string metadataName)
        {
            INamedTypeSymbol typeSymbol = _semanticModel.GetTypeByMetadataName(metadataName);

            if (typeSymbol != null)
            {
                return SymbolDisplay.ToMinimalDisplayString(typeSymbol, _semanticModel, _position, SymbolDisplayFormats.Default)
                    .Replace('<', '{')
                    .Replace('>', '}');
            }

            return metadataName;
        }
    }
}
