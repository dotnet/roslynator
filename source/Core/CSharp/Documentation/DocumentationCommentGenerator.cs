// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Xml;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Roslynator.CSharp.Documentation
{
    public static class DocumentationCommentGenerator
    {
        public static SyntaxTriviaList Generate(MemberDeclarationSyntax memberDeclaration, DocumentationCommentGeneratorSettings settings = null)
        {
            if (memberDeclaration == null)
                throw new ArgumentNullException(nameof(memberDeclaration));

            switch (memberDeclaration.Kind())
            {
                case SyntaxKind.NamespaceDeclaration:
                    return Generate((NamespaceDeclarationSyntax)memberDeclaration, settings);
                case SyntaxKind.ClassDeclaration:
                    return Generate((ClassDeclarationSyntax)memberDeclaration, settings);
                case SyntaxKind.StructDeclaration:
                    return Generate((StructDeclarationSyntax)memberDeclaration, settings);
                case SyntaxKind.InterfaceDeclaration:
                    return Generate((InterfaceDeclarationSyntax)memberDeclaration, settings);
                case SyntaxKind.EnumDeclaration:
                    return Generate((EnumDeclarationSyntax)memberDeclaration, settings);
                case SyntaxKind.DelegateDeclaration:
                    return Generate((DelegateDeclarationSyntax)memberDeclaration, settings);
                case SyntaxKind.EnumMemberDeclaration:
                    return Generate((EnumMemberDeclarationSyntax)memberDeclaration, settings);
                case SyntaxKind.FieldDeclaration:
                    return Generate((FieldDeclarationSyntax)memberDeclaration, settings);
                case SyntaxKind.EventFieldDeclaration:
                    return Generate((EventFieldDeclarationSyntax)memberDeclaration, settings);
                case SyntaxKind.MethodDeclaration:
                    return Generate((MethodDeclarationSyntax)memberDeclaration, settings);
                case SyntaxKind.OperatorDeclaration:
                    return Generate((OperatorDeclarationSyntax)memberDeclaration, settings);
                case SyntaxKind.ConversionOperatorDeclaration:
                    return Generate((ConversionOperatorDeclarationSyntax)memberDeclaration, settings);
                case SyntaxKind.ConstructorDeclaration:
                    return Generate((ConstructorDeclarationSyntax)memberDeclaration, settings);
                case SyntaxKind.DestructorDeclaration:
                    return Generate((DestructorDeclarationSyntax)memberDeclaration, settings);
                case SyntaxKind.PropertyDeclaration:
                    return Generate((PropertyDeclarationSyntax)memberDeclaration, settings);
                case SyntaxKind.EventDeclaration:
                    return Generate((EventDeclarationSyntax)memberDeclaration, settings);
                case SyntaxKind.IndexerDeclaration:
                    return Generate((IndexerDeclarationSyntax)memberDeclaration, settings);
                default:
                    throw new ArgumentException("", nameof(memberDeclaration));
            }
        }

        public static SyntaxTriviaList Generate(NamespaceDeclarationSyntax namespaceDeclaration, DocumentationCommentGeneratorSettings settings = null)
        {
            if (namespaceDeclaration == null)
                throw new ArgumentNullException(nameof(namespaceDeclaration));

            return Generate(settings: settings);
        }

        public static SyntaxTriviaList Generate(ClassDeclarationSyntax classDeclaration, DocumentationCommentGeneratorSettings settings = null)
        {
            if (classDeclaration == null)
                throw new ArgumentNullException(nameof(classDeclaration));

            return Generate(
                classDeclaration.TypeParameterList,
                default(ParameterListSyntax),
                generateReturns: false,
                settings: settings);
        }

        public static SyntaxTriviaList Generate(StructDeclarationSyntax structDeclaration, DocumentationCommentGeneratorSettings settings = null)
        {
            if (structDeclaration == null)
                throw new ArgumentNullException(nameof(structDeclaration));

            return Generate(
                structDeclaration.TypeParameterList,
                default(ParameterListSyntax),
                generateReturns: false,
                settings: settings);
        }

        public static SyntaxTriviaList Generate(InterfaceDeclarationSyntax interfaceDeclaration, DocumentationCommentGeneratorSettings settings = null)
        {
            if (interfaceDeclaration == null)
                throw new ArgumentNullException(nameof(interfaceDeclaration));

            return Generate(
                interfaceDeclaration.TypeParameterList,
                default(ParameterListSyntax),
                generateReturns: false,
                settings: settings);
        }

        public static SyntaxTriviaList Generate(EnumDeclarationSyntax enumDeclaration, DocumentationCommentGeneratorSettings settings = null)
        {
            if (enumDeclaration == null)
                throw new ArgumentNullException(nameof(enumDeclaration));

            return Generate(settings: settings);
        }

        public static SyntaxTriviaList Generate(DelegateDeclarationSyntax delegateDeclaration, DocumentationCommentGeneratorSettings settings = null)
        {
            if (delegateDeclaration == null)
                throw new ArgumentNullException(nameof(delegateDeclaration));

            return Generate(
                delegateDeclaration.TypeParameterList,
                delegateDeclaration.ParameterList,
                generateReturns: false,
                settings: settings);
        }

        public static SyntaxTriviaList Generate(EnumMemberDeclarationSyntax enumMemberDeclaration, DocumentationCommentGeneratorSettings settings = null)
        {
            if (enumMemberDeclaration == null)
                throw new ArgumentNullException(nameof(enumMemberDeclaration));

            return Generate(settings: settings);
        }

        public static SyntaxTriviaList Generate(FieldDeclarationSyntax fieldDeclaration, DocumentationCommentGeneratorSettings settings = null)
        {
            if (fieldDeclaration == null)
                throw new ArgumentNullException(nameof(fieldDeclaration));

            return Generate(settings: settings);
        }

        public static SyntaxTriviaList Generate(EventFieldDeclarationSyntax eventFieldDeclaration, DocumentationCommentGeneratorSettings settings = null)
        {
            if (eventFieldDeclaration == null)
                throw new ArgumentNullException(nameof(eventFieldDeclaration));

            return Generate(settings: settings);
        }

        public static SyntaxTriviaList Generate(MethodDeclarationSyntax methodDeclaration, DocumentationCommentGeneratorSettings settings = null)
        {
            if (methodDeclaration == null)
                throw new ArgumentNullException(nameof(methodDeclaration));

            return Generate(
                methodDeclaration.TypeParameterList,
                methodDeclaration.ParameterList,
                generateReturns: !methodDeclaration.ReturnsVoid(),
                settings: settings);
        }

        public static SyntaxTriviaList Generate(OperatorDeclarationSyntax operatorDeclaration, DocumentationCommentGeneratorSettings settings = null)
        {
            if (operatorDeclaration == null)
                throw new ArgumentNullException(nameof(operatorDeclaration));

            return Generate(
                default(TypeParameterListSyntax),
                operatorDeclaration.ParameterList,
                generateReturns: true,
                settings: settings);
        }

        public static SyntaxTriviaList Generate(ConversionOperatorDeclarationSyntax conversionOperatorDeclaration, DocumentationCommentGeneratorSettings settings = null)
        {
            if (conversionOperatorDeclaration == null)
                throw new ArgumentNullException(nameof(conversionOperatorDeclaration));

            return Generate(
                default(TypeParameterListSyntax),
                conversionOperatorDeclaration.ParameterList,
                generateReturns: false,
                settings: settings);
        }

        public static SyntaxTriviaList Generate(ConstructorDeclarationSyntax constructorDeclaration, DocumentationCommentGeneratorSettings settings = null)
        {
            if (constructorDeclaration == null)
                throw new ArgumentNullException(nameof(constructorDeclaration));

            return Generate(
                default(TypeParameterListSyntax),
                constructorDeclaration.ParameterList,
                generateReturns: false,
                settings: settings);
        }

        public static SyntaxTriviaList Generate(DestructorDeclarationSyntax destructorDeclaration, DocumentationCommentGeneratorSettings settings = null)
        {
            if (destructorDeclaration == null)
                throw new ArgumentNullException(nameof(destructorDeclaration));

            return Generate(settings: settings);
        }

        public static SyntaxTriviaList Generate(PropertyDeclarationSyntax propertyDeclaration, DocumentationCommentGeneratorSettings settings = null)
        {
            if (propertyDeclaration == null)
                throw new ArgumentNullException(nameof(propertyDeclaration));

            return Generate(settings: settings);
        }

        public static SyntaxTriviaList Generate(EventDeclarationSyntax eventDeclaration, DocumentationCommentGeneratorSettings settings = null)
        {
            if (eventDeclaration == null)
                throw new ArgumentNullException(nameof(eventDeclaration));

            return Generate(settings: settings);
        }

        public static SyntaxTriviaList Generate(IndexerDeclarationSyntax indexerDeclaration, DocumentationCommentGeneratorSettings settings = null)
        {
            if (indexerDeclaration == null)
                throw new ArgumentNullException(nameof(indexerDeclaration));

            return Generate(
                default(TypeParameterListSyntax),
                indexerDeclaration.ParameterList,
                generateReturns: true,
                settings: settings);
        }

        private static SyntaxTriviaList Generate(
            TypeParameterListSyntax typeParameterList,
            BaseParameterListSyntax parameterList,
            bool generateReturns = false,
            DocumentationCommentGeneratorSettings settings = null)
        {
            SeparatedSyntaxList<TypeParameterSyntax> typeParameters = typeParameterList?.Parameters ?? default(SeparatedSyntaxList<TypeParameterSyntax>);

            SeparatedSyntaxList<ParameterSyntax> parameters = parameterList?.Parameters ?? default(SeparatedSyntaxList<ParameterSyntax>);

            return Generate(typeParameters, parameters, generateReturns, settings);
        }

        private static SyntaxTriviaList Generate(
            SeparatedSyntaxList<TypeParameterSyntax> typeParameters = default(SeparatedSyntaxList<TypeParameterSyntax>),
            SeparatedSyntaxList<ParameterSyntax> parameters = default(SeparatedSyntaxList<ParameterSyntax>),
            bool generateReturns = false,
            DocumentationCommentGeneratorSettings settings = null)
        {
            settings = settings ?? DocumentationCommentGeneratorSettings.Default;

            ImmutableArray<string> comments = settings.Comments;

            var sb = new StringBuilder();

            sb.Append(settings.Indent);
            sb.Append("/// <summary>");

            if (settings.SingleLineSummary
                && comments.Length <= 1)
            {
                if (comments.Length == 1)
                    sb.Append(comments[0]);

                sb.AppendLine("</summary>");
            }
            else
            {
                sb.AppendLine();

                if (comments.Any())
                {
                    foreach (string comment in comments)
                    {
                        sb.Append(settings.Indent);
                        sb.Append("/// ");
                        sb.AppendLine(comment);
                    }
                }
                else
                {
                    sb.Append(settings.Indent);
                    sb.AppendLine("/// ");
                }

                sb.Append(settings.Indent);
                sb.AppendLine("/// </summary>");
            }

            foreach (TypeParameterSyntax typeParameter in typeParameters)
            {
                sb.Append(settings.Indent);
                sb.Append("/// <typeparam name=\"");
                sb.Append(typeParameter.Identifier.ValueText);
                sb.AppendLine("\"></typeparam>");
            }

            foreach (ParameterSyntax parameter in parameters)
            {
                sb.Append(settings.Indent);
                sb.Append("/// <param name=\"");
                sb.Append(parameter.Identifier.ValueText);
                sb.AppendLine("\"></param>");
            }

            if (generateReturns
                && settings.GenerateReturns)
            {
                sb.Append(settings.Indent);
                sb.AppendLine("/// <returns></returns>");
            }

            return SyntaxFactory.ParseLeadingTrivia(sb.ToString());
        }

        internal static bool CanGenerateFromBase(SyntaxKind kind)
        {
            switch (kind)
            {
                case SyntaxKind.MethodDeclaration:
                case SyntaxKind.PropertyDeclaration:
                case SyntaxKind.IndexerDeclaration:
                case SyntaxKind.EventFieldDeclaration:
                case SyntaxKind.EventDeclaration:
                case SyntaxKind.ConstructorDeclaration:
                    return true;
                default:
                    return false;
            }
        }

        public static BaseDocumentationCommentData GenerateFromBase(MemberDeclarationSyntax memberDeclaration, SemanticModel semanticModel, CancellationToken cancellationToken = default(CancellationToken))
        {
            switch (memberDeclaration.Kind())
            {
                case SyntaxKind.MethodDeclaration:
                    return GenerateFromBase((MethodDeclarationSyntax)memberDeclaration, semanticModel, cancellationToken);
                case SyntaxKind.PropertyDeclaration:
                    return GenerateFromBase((PropertyDeclarationSyntax)memberDeclaration, semanticModel, cancellationToken);
                case SyntaxKind.IndexerDeclaration:
                    return GenerateFromBase((IndexerDeclarationSyntax)memberDeclaration, semanticModel, cancellationToken);
                case SyntaxKind.EventFieldDeclaration:
                    return GenerateFromBase((EventFieldDeclarationSyntax)memberDeclaration, semanticModel, cancellationToken);
                case SyntaxKind.EventDeclaration:
                    return GenerateFromBase((EventDeclarationSyntax)memberDeclaration, semanticModel, cancellationToken);
                case SyntaxKind.ConstructorDeclaration:
                    return GenerateFromBase((ConstructorDeclarationSyntax)memberDeclaration, semanticModel, cancellationToken);
                default:
                    throw new ArgumentException("", nameof(memberDeclaration));
            }
        }

        public static BaseDocumentationCommentData GenerateFromBase(MethodDeclarationSyntax methodDeclaration, SemanticModel semanticModel, CancellationToken cancellationToken = default(CancellationToken))
        {
            IMethodSymbol methodSymbol = semanticModel.GetDeclaredSymbol(methodDeclaration, cancellationToken);

            if (methodSymbol?.IsErrorType() == false)
            {
                int position = methodDeclaration.SpanStart;

                SyntaxTrivia trivia = GenerateFromOverriddenMethods(methodSymbol, semanticModel, position, cancellationToken);

                if (trivia.IsKind(SyntaxKind.SingleLineDocumentationCommentTrivia))
                    return new BaseDocumentationCommentData(trivia, BaseDocumentationCommentOrigin.BaseMember);

                trivia = GenerateFromInterfaceMember<IMethodSymbol>(methodSymbol, semanticModel, position, cancellationToken);

                if (trivia.IsKind(SyntaxKind.SingleLineDocumentationCommentTrivia))
                    return new BaseDocumentationCommentData(trivia, BaseDocumentationCommentOrigin.InterfaceMember);
            }

            return default(BaseDocumentationCommentData);
        }

        public static BaseDocumentationCommentData GenerateFromBase(PropertyDeclarationSyntax propertyDeclaration, SemanticModel semanticModel, CancellationToken cancellationToken = default(CancellationToken))
        {
            IPropertySymbol propertySymbol = semanticModel.GetDeclaredSymbol(propertyDeclaration, cancellationToken);

            return GenerateFromBase(propertySymbol, semanticModel, propertyDeclaration.SpanStart, cancellationToken);
        }

        public static BaseDocumentationCommentData GenerateFromBase(IndexerDeclarationSyntax indexerDeclaration, SemanticModel semanticModel, CancellationToken cancellationToken = default(CancellationToken))
        {
            IPropertySymbol propertySymbol = semanticModel.GetDeclaredSymbol(indexerDeclaration, cancellationToken);

            return GenerateFromBase(propertySymbol, semanticModel, indexerDeclaration.SpanStart, cancellationToken);
        }

        private static BaseDocumentationCommentData GenerateFromBase(IPropertySymbol propertySymbol, SemanticModel semanticModel, int position, CancellationToken cancellationToken)
        {
            if (propertySymbol?.IsErrorType() == false)
            {
                SyntaxTrivia trivia = GenerateFromOverriddenProperties(propertySymbol, semanticModel, position, cancellationToken);

                if (trivia.IsKind(SyntaxKind.SingleLineDocumentationCommentTrivia))
                    return new BaseDocumentationCommentData(trivia, BaseDocumentationCommentOrigin.BaseMember);

                trivia = GenerateFromInterfaceMember<IPropertySymbol>(propertySymbol, semanticModel, position, cancellationToken);

                if (trivia.IsKind(SyntaxKind.SingleLineDocumentationCommentTrivia))
                    return new BaseDocumentationCommentData(trivia, BaseDocumentationCommentOrigin.InterfaceMember);
            }

            return default(BaseDocumentationCommentData);
        }

        public static BaseDocumentationCommentData GenerateFromBase(EventDeclarationSyntax eventDeclaration, SemanticModel semanticModel, CancellationToken cancellationToken = default(CancellationToken))
        {
            IEventSymbol eventSymbol = semanticModel.GetDeclaredSymbol(eventDeclaration, cancellationToken);

            return GenerateFromBase(eventSymbol, semanticModel, eventDeclaration.SpanStart, cancellationToken);
        }

        public static BaseDocumentationCommentData GenerateFromBase(EventFieldDeclarationSyntax eventFieldDeclaration, SemanticModel semanticModel, CancellationToken cancellationToken = default(CancellationToken))
        {
            VariableDeclaratorSyntax variableDeclarator = eventFieldDeclaration.Declaration?.Variables.FirstOrDefault();

            if (variableDeclarator != null)
            {
                var eventSymbol = semanticModel.GetDeclaredSymbol(variableDeclarator, cancellationToken) as IEventSymbol;

                return GenerateFromBase(eventSymbol, semanticModel, eventFieldDeclaration.SpanStart, cancellationToken);
            }

            return default(BaseDocumentationCommentData);
        }

        private static BaseDocumentationCommentData GenerateFromBase(IEventSymbol eventSymbol, SemanticModel semanticModel, int position, CancellationToken cancellationToken)
        {
            if (eventSymbol?.IsErrorType() == false)
            {
                SyntaxTrivia trivia = GenerateFromOverriddenEvents(eventSymbol, semanticModel, position, cancellationToken);

                if (trivia.IsKind(SyntaxKind.SingleLineDocumentationCommentTrivia))
                    return new BaseDocumentationCommentData(trivia, BaseDocumentationCommentOrigin.BaseMember);

                trivia = GenerateFromInterfaceMember<IEventSymbol>(eventSymbol, semanticModel, position, cancellationToken);

                if (trivia.IsKind(SyntaxKind.SingleLineDocumentationCommentTrivia))
                    return new BaseDocumentationCommentData(trivia, BaseDocumentationCommentOrigin.InterfaceMember);
            }

            return default(BaseDocumentationCommentData);
        }

        public static BaseDocumentationCommentData GenerateFromBase(ConstructorDeclarationSyntax constructorDeclaration, SemanticModel semanticModel, CancellationToken cancellationToken = default(CancellationToken))
        {
            ConstructorInitializerSyntax initializer = constructorDeclaration.Initializer;

            if (initializer?.IsKind(SyntaxKind.BaseConstructorInitializer) == true)
            {
                ISymbol baseConstructor = semanticModel.GetSymbol(initializer, cancellationToken);

                if (baseConstructor?.IsErrorType() == false)
                {
                    SyntaxTrivia trivia = GetDocumentationCommentTrivia(baseConstructor, semanticModel, constructorDeclaration.SpanStart, cancellationToken);

                    if (trivia.IsKind(SyntaxKind.SingleLineDocumentationCommentTrivia))
                        return new BaseDocumentationCommentData(trivia, BaseDocumentationCommentOrigin.BaseMember);
                }
            }

            return default(BaseDocumentationCommentData);
        }

        private static SyntaxTrivia GenerateFromOverriddenMethods(IMethodSymbol methodSymbol, SemanticModel semanticModel, int position, CancellationToken cancellationToken)
        {
            for (IMethodSymbol overriddenMethod = methodSymbol.OverriddenMethod; overriddenMethod != null; overriddenMethod = overriddenMethod.OverriddenMethod)
            {
                SyntaxTrivia trivia = GetDocumentationCommentTrivia(overriddenMethod, semanticModel, position, cancellationToken);

                if (trivia.IsKind(SyntaxKind.SingleLineDocumentationCommentTrivia))
                    return trivia;
            }

            return default(SyntaxTrivia);
        }

        private static SyntaxTrivia GenerateFromOverriddenProperties(IPropertySymbol propertySymbol, SemanticModel semanticModel, int position, CancellationToken cancellationToken)
        {
            for (IPropertySymbol overriddenProperty = propertySymbol.OverriddenProperty; overriddenProperty != null; overriddenProperty = overriddenProperty.OverriddenProperty)
            {
                SyntaxTrivia trivia = GetDocumentationCommentTrivia(overriddenProperty, semanticModel, position, cancellationToken);

                if (trivia.IsKind(SyntaxKind.SingleLineDocumentationCommentTrivia))
                    return trivia;
            }

            return default(SyntaxTrivia);
        }

        private static SyntaxTrivia GenerateFromOverriddenEvents(IEventSymbol eventSymbol, SemanticModel semanticModel, int position, CancellationToken cancellationToken)
        {
            for (IEventSymbol overriddenEvent = eventSymbol.OverriddenEvent; overriddenEvent != null; overriddenEvent = overriddenEvent.OverriddenEvent)
            {
                SyntaxTrivia trivia = GetDocumentationCommentTrivia(overriddenEvent, semanticModel, position, cancellationToken);

                if (trivia.IsKind(SyntaxKind.SingleLineDocumentationCommentTrivia))
                    return trivia;
            }

            return default(SyntaxTrivia);
        }

        private static SyntaxTrivia GenerateFromInterfaceMember<TInterfaceSymbol>(
            ISymbol memberSymbol,
            SemanticModel semanticModel,
            int position,
            CancellationToken cancellationToken) where TInterfaceSymbol : ISymbol
        {
            TInterfaceSymbol interfaceMember = memberSymbol.FindImplementedInterfaceMember<TInterfaceSymbol>();

            if (!EqualityComparer<TInterfaceSymbol>.Default.Equals(interfaceMember, default(TInterfaceSymbol)))
            {
                return GetDocumentationCommentTrivia(interfaceMember, semanticModel, position, cancellationToken);
            }
            else
            {
                return default(SyntaxTrivia);
            }
        }

        private static SyntaxTrivia GetDocumentationCommentTrivia(ISymbol symbol, SemanticModel semanticModel, int position, CancellationToken cancellationToken = default(CancellationToken))
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

                Debug.Fail(xmlString);
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

                    if (xmlElement.IsLocalName("filterpriority", "FILTERPRIORITY"))
                        content = content.RemoveAt(i);
                }
            }

            return commentTrivia.WithContent(content);
        }

        private static string GetInnerXml(string comment)
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
                                {
                                    try
                                    {
                                        return reader.ReadInnerXml();
                                    }
                                    catch (XmlException ex)
                                    {
                                        Debug.Fail(ex.ToString());
                                        return null;
                                    }
                                }
                            default:
                                {
                                    Debug.Fail(reader.Name);
                                    return null;
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
                        indent = indent ?? Regex.Match(s, "^ *").Value;

                        sb.Append("/// ");
                        s = Regex.Replace(s, $"^{indent}", "");

                        sb.AppendLine(s);
                    }
                }
            }

            return sb.ToString();
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
                if (node.Name?.IsLocalName("cref", "CREF") == true)
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
                    return SymbolDisplay.GetMinimalString(typeSymbol, _semanticModel, _position)
                        .Replace('<', '{')
                        .Replace('>', '}');
                }

                return metadataName;
            }
        }
    }
}
