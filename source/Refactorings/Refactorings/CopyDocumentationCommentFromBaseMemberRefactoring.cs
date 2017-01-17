// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.CSharp.Extensions;
using Roslynator.Extensions;

namespace Roslynator.CSharp.Refactorings
{
    internal static class CopyDocumentationCommentFromBaseMemberRefactoring
    {
        public static async Task ComputeRefactoringAsync(RefactoringContext context, MethodDeclarationSyntax methodDeclaration)
        {
            if (!methodDeclaration.HasSingleLineDocumentationComment())
            {
                SemanticModel semanticModel = await context.GetSemanticModelAsync().ConfigureAwait(false);

                IMethodSymbol methodSymbol = semanticModel.GetDeclaredSymbol(methodDeclaration);

                if (methodSymbol?.IsErrorType() == false)
                    ComputeRefactoring<IMethodSymbol>(context, methodDeclaration, methodSymbol, methodSymbol.OverriddenMethod, semanticModel);
            }
        }

        public static async Task ComputeRefactoringAsync(RefactoringContext context, PropertyDeclarationSyntax propertyDeclaration)
        {
            if (!propertyDeclaration.HasSingleLineDocumentationComment())
            {
                SemanticModel semanticModel = await context.GetSemanticModelAsync().ConfigureAwait(false);

                IPropertySymbol propertySymbol = semanticModel.GetDeclaredSymbol(propertyDeclaration);

                if (propertySymbol?.IsErrorType() == false)
                    ComputeRefactoring<IPropertySymbol>(context, propertyDeclaration, propertySymbol, propertySymbol.OverriddenProperty, semanticModel);
            }
        }

        public static async Task ComputeRefactoringAsync(RefactoringContext context, IndexerDeclarationSyntax indexerDeclaration)
        {
            if (!indexerDeclaration.HasSingleLineDocumentationComment())
            {
                SemanticModel semanticModel = await context.GetSemanticModelAsync().ConfigureAwait(false);

                IPropertySymbol propertySymbol = semanticModel.GetDeclaredSymbol(indexerDeclaration);

                if (propertySymbol?.IsErrorType() == false)
                    ComputeRefactoring<IPropertySymbol>(context, indexerDeclaration, propertySymbol, propertySymbol.OverriddenProperty, semanticModel);
            }
        }

        public static async Task ComputeRefactoringAsync(RefactoringContext context, EventDeclarationSyntax eventDeclaration)
        {
            if (!eventDeclaration.HasSingleLineDocumentationComment())
            {
                SemanticModel semanticModel = await context.GetSemanticModelAsync().ConfigureAwait(false);

                IEventSymbol eventSymbol = semanticModel.GetDeclaredSymbol(eventDeclaration);

                if (eventSymbol?.IsErrorType() == false)
                    ComputeRefactoring<IEventSymbol>(context, eventDeclaration, eventSymbol, eventSymbol.OverriddenEvent, semanticModel);
            }
        }

        public static async Task ComputeRefactoringAsync(RefactoringContext context, EventFieldDeclarationSyntax eventFieldDeclaration)
        {
            if (!eventFieldDeclaration.HasSingleLineDocumentationComment())
            {
                VariableDeclaratorSyntax variableDeclarator = eventFieldDeclaration.Declaration?.Variables.FirstOrDefault();

                if (variableDeclarator != null)
                {
                    SemanticModel semanticModel = await context.GetSemanticModelAsync().ConfigureAwait(false);

                    var eventSymbol = semanticModel.GetDeclaredSymbol(variableDeclarator) as IEventSymbol;

                    if (eventSymbol?.IsErrorType() == false)
                        ComputeRefactoring<IEventSymbol>(context, eventFieldDeclaration, eventSymbol, eventSymbol.OverriddenEvent, semanticModel);
                }
            }
        }

        public static async Task ComputeRefactoringAsync(RefactoringContext context, ConstructorDeclarationSyntax constructorDeclaration)
        {
            if (!constructorDeclaration.HasSingleLineDocumentationComment())
            {
                ConstructorInitializerSyntax initializer = constructorDeclaration.Initializer;

                if (initializer?.IsKind(SyntaxKind.BaseConstructorInitializer) == true)
                {
                    SemanticModel semanticModel = await context.GetSemanticModelAsync().ConfigureAwait(false);

                    ISymbol symbol = semanticModel.GetSymbol(initializer);

                    if (symbol?.IsErrorType() == false)
                        ComputeRefactoring<ISymbol>(context, constructorDeclaration, null, symbol, semanticModel);
                }
            }
        }

        private static void ComputeRefactoring<TInterfaceSymbol>(
            RefactoringContext context,
            MemberDeclarationSyntax memberDeclaration,
            ISymbol memberSymbol,
            ISymbol baseSymbol,
            SemanticModel semanticModel) where TInterfaceSymbol : ISymbol
        {
            var commentTrivia = default(SyntaxTrivia);
            string title = null;

            if (baseSymbol != null)
            {
                commentTrivia = CreateDocumentationCommentTrivia(baseSymbol, semanticModel, memberDeclaration.SpanStart, context.CancellationToken);

                if (commentTrivia.IsKind(SyntaxKind.SingleLineDocumentationCommentTrivia))
                    title = GetTitleForBaseMember(memberDeclaration);
            }

            if (!commentTrivia.IsKind(SyntaxKind.SingleLineDocumentationCommentTrivia)
                && memberSymbol != null)
            {
                TInterfaceSymbol interfaceMember = memberSymbol.FindImplementedInterfaceMember<TInterfaceSymbol>();

                if (interfaceMember != null)
                {
                    commentTrivia = CreateDocumentationCommentTrivia(interfaceMember, semanticModel, memberDeclaration.SpanStart, context.CancellationToken);

                    if (commentTrivia.IsKind(SyntaxKind.SingleLineDocumentationCommentTrivia))
                        title = GetTitleForInterfaceMember(memberDeclaration);
                }
            }

            if (commentTrivia.IsKind(SyntaxKind.SingleLineDocumentationCommentTrivia))
            {
                context.RegisterRefactoring(
                    title,
                    cancellationToken => RefactorAsync(context.Document, memberDeclaration, commentTrivia, cancellationToken));
            }
        }

        private static string GetTitleForBaseMember(MemberDeclarationSyntax memberDeclaration)
        {
            const string s = "Insert comment from ";

            switch (memberDeclaration.Kind())
            {
                case SyntaxKind.ConstructorDeclaration:
                    return s + "base constructor";
                case SyntaxKind.MethodDeclaration:
                    return s + "overriden method";
                case SyntaxKind.PropertyDeclaration:
                    return s + "overriden property";
                case SyntaxKind.IndexerDeclaration:
                    return s + "overriden indexer";
                case SyntaxKind.EventDeclaration:
                case SyntaxKind.EventFieldDeclaration:
                    return s + "overriden event";
                default:
                    {
                        Debug.Assert(false, memberDeclaration.Kind().ToString());
                        return "";
                    }
            }
        }

        private static string GetTitleForInterfaceMember(MemberDeclarationSyntax memberDeclaration)
        {
            const string s = "Insert comment from ";

            switch (memberDeclaration.Kind())
            {
                case SyntaxKind.MethodDeclaration:
                    return s + "implemented method";
                case SyntaxKind.PropertyDeclaration:
                    return s + "implemented property";
                case SyntaxKind.IndexerDeclaration:
                    return s + "implemented indexer";
                case SyntaxKind.EventDeclaration:
                case SyntaxKind.EventFieldDeclaration:
                    return s + "implemented event";
                default:
                    {
                        Debug.Assert(false, memberDeclaration.Kind().ToString());
                        return "";
                    }
            }
        }

        public static SyntaxTrivia CreateDocumentationCommentTrivia(ISymbol symbol, SemanticModel semanticModel, int position, CancellationToken cancellationToken = default(CancellationToken))
        {
            string xmlString = symbol.GetDocumentationCommentXml(cancellationToken: cancellationToken);

            if (!string.IsNullOrEmpty(xmlString))
            {
                string innerXml = GetInnerXml(xmlString);

                if (innerXml != null)
                {
                    string text = AddSlashes(innerXml.TrimEnd());

                    SyntaxTriviaList triviaList = SyntaxFactory.ParseLeadingTrivia(text);

                    if (triviaList.Count == 1)
                    {
                        SyntaxTrivia trivia = triviaList.First();

                        if (trivia.IsKind(SyntaxKind.SingleLineDocumentationCommentTrivia)
                            && trivia.HasStructure)
                        {
                            SyntaxNode structure = trivia.GetStructure();

                            if (structure.IsKind(SyntaxKind.SingleLineDocumentationCommentTrivia))
                            {
                                var commentTrivia = (DocumentationCommentTriviaSyntax)structure;

                                var rewriter = new DocumentationCommentTriviaRewriter(position, semanticModel);

                                // Remove T: from cref attribute and replace `1 with {T}
                                commentTrivia = (DocumentationCommentTriviaSyntax)rewriter.Visit(commentTrivia);

                                // Remove <filterpriority> element
                                commentTrivia = RemoveFilterPriorityElement(commentTrivia);

                                string commentTriviaText = commentTrivia.ToFullString();

                                commentTriviaText = Regex.Replace(commentTriviaText, @"^///\s*(\r?\n|$)", "", RegexOptions.Multiline);

                                triviaList = SyntaxFactory.ParseLeadingTrivia(commentTriviaText);

                                if (triviaList.Count == 1)
                                    return triviaList.First();
                            }
                        }
                    }
                }

                Debug.Assert(false, xmlString);
            }

            return default(SyntaxTrivia);
        }

        private static DocumentationCommentTriviaSyntax RemoveFilterPriorityElement(DocumentationCommentTriviaSyntax commentTrivia)
        {
            SyntaxList<XmlNodeSyntax> content = commentTrivia.Content;

            for (int i = content.Count - 1; i >= 0; i--)
            {
                XmlNodeSyntax xmlNode = content[i];

                if (xmlNode.IsKind(SyntaxKind.XmlElement))
                {
                    var xmlElement = (XmlElementSyntax)xmlNode;

                    string name = xmlElement.StartTag?.Name?.LocalName.ValueText;

                    if (string.Equals(name, "filterpriority", StringComparison.OrdinalIgnoreCase))
                        content = content.RemoveAt(i);
                }
            }

            return commentTrivia.WithContent(content);
        }

        public static string GetInnerXml(string comment)
        {
            using (var sr = new StringReader(comment))
            {
                var settings = new XmlReaderSettings() { ConformanceLevel = ConformanceLevel.Fragment };

                using (XmlReader reader = XmlReader.Create(sr, settings))
                {
                    if (reader.Read()
                        && reader.NodeType == XmlNodeType.Element)
                    {
                        switch (reader.Name)
                        {
                            case "member":
                            case "doc":
                                return reader.ReadInnerXml();
                            default:
                                {
                                    Debug.Assert(false, reader.Name);
                                    break;
                                }
                        }
                    }
                }
            }

            return null;
        }

        private static string AddSlashes(string innerXml)
        {
            var sb = new StringBuilder();

            string indent = null;

            using (var sr = new StringReader(innerXml))
            {
                string s = null;

                while ((s = sr.ReadLine()) != null)
                {
                    if (s.Length > 0)
                    {
                        if (indent == null)
                            indent = Regex.Match(s, "^ *").Value;

                        sb.Append("/// ");
                        s = Regex.Replace(s, $"^{indent}", "");

                        sb.AppendLine(s);
                    }
                }
            }

            return sb.ToString();
        }

        public static async Task<Document> RefactorAsync(
            Document document,
            MemberDeclarationSyntax memberDeclaration,
            SyntaxTrivia commentTrivia,
            CancellationToken cancellationToken)
        {
            SyntaxTriviaList leadingTrivia = memberDeclaration.GetLeadingTrivia();

            SyntaxTriviaList newLeadingTrivia = InsertDocumentationCommentTrivia(leadingTrivia, commentTrivia);

            MemberDeclarationSyntax newMemberDeclaration = memberDeclaration
                .WithLeadingTrivia(newLeadingTrivia)
                .WithFormatterAnnotation();

            return await document.ReplaceNodeAsync(memberDeclaration, newMemberDeclaration, cancellationToken).ConfigureAwait(false);
        }

        private static SyntaxTriviaList InsertDocumentationCommentTrivia(SyntaxTriviaList leadingTrivia, SyntaxTrivia commentTrivia)
        {
            int index = leadingTrivia.LastIndexOf(SyntaxKind.EndOfLineTrivia);

            if (index != -1)
            {
                return leadingTrivia.Insert(index + 1, commentTrivia);
            }
            else
            {
                return leadingTrivia.Insert(0, commentTrivia);
            }
        }

        private class DocumentationCommentTriviaRewriter : CSharpSyntaxRewriter
        {
            private readonly SemanticModel _semanticModel;
            private readonly int _position;

            public DocumentationCommentTriviaRewriter(int position, SemanticModel semanticModel)
                : base(visitIntoStructuredTrivia: true)
            {
                if (semanticModel == null)
                    throw new ArgumentNullException(nameof(semanticModel));

                _position = position;
                _semanticModel = semanticModel;
            }

            public override SyntaxNode VisitXmlTextAttribute(XmlTextAttributeSyntax node)
            {
                XmlNameSyntax name = node.Name;

                if (name?.LocalName.ValueText == "cref")
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
                INamedTypeSymbol typeSymbol = _semanticModel.Compilation.GetTypeByMetadataName(metadataName);

                if (typeSymbol != null)
                {
                    return SymbolDisplay.GetMinimalDisplayString(typeSymbol, _position, _semanticModel)
                        .Replace('<', '{')
                        .Replace('>', '}');
                }

                return metadataName;
            }
        }
    }
}