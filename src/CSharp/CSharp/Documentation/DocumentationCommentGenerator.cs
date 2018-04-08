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
using Roslynator.Text;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Roslynator.CSharp.Documentation
{
    internal static class DocumentationCommentGenerator
    {
        private static readonly Regex _commentedEmptyLineRegex = new Regex(@"^///\s*(\r?\n|$)", RegexOptions.Multiline);

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
                canGenerateReturns: false,
                settings: settings);
        }

        public static SyntaxTriviaList Generate(StructDeclarationSyntax structDeclaration, DocumentationCommentGeneratorSettings settings = null)
        {
            if (structDeclaration == null)
                throw new ArgumentNullException(nameof(structDeclaration));

            return Generate(
                structDeclaration.TypeParameterList,
                default(ParameterListSyntax),
                canGenerateReturns: false,
                settings: settings);
        }

        public static SyntaxTriviaList Generate(InterfaceDeclarationSyntax interfaceDeclaration, DocumentationCommentGeneratorSettings settings = null)
        {
            if (interfaceDeclaration == null)
                throw new ArgumentNullException(nameof(interfaceDeclaration));

            return Generate(
                interfaceDeclaration.TypeParameterList,
                default(ParameterListSyntax),
                canGenerateReturns: false,
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
                canGenerateReturns: false,
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
                canGenerateReturns: !methodDeclaration.ReturnsVoid(),
                settings: settings);
        }

        public static SyntaxTriviaList Generate(OperatorDeclarationSyntax operatorDeclaration, DocumentationCommentGeneratorSettings settings = null)
        {
            if (operatorDeclaration == null)
                throw new ArgumentNullException(nameof(operatorDeclaration));

            return Generate(
                default(TypeParameterListSyntax),
                operatorDeclaration.ParameterList,
                canGenerateReturns: true,
                settings: settings);
        }

        public static SyntaxTriviaList Generate(ConversionOperatorDeclarationSyntax conversionOperatorDeclaration, DocumentationCommentGeneratorSettings settings = null)
        {
            if (conversionOperatorDeclaration == null)
                throw new ArgumentNullException(nameof(conversionOperatorDeclaration));

            return Generate(
                default(TypeParameterListSyntax),
                conversionOperatorDeclaration.ParameterList,
                canGenerateReturns: false,
                settings: settings);
        }

        public static SyntaxTriviaList Generate(ConstructorDeclarationSyntax constructorDeclaration, DocumentationCommentGeneratorSettings settings = null)
        {
            if (constructorDeclaration == null)
                throw new ArgumentNullException(nameof(constructorDeclaration));

            return Generate(
                default(TypeParameterListSyntax),
                constructorDeclaration.ParameterList,
                canGenerateReturns: false,
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
                canGenerateReturns: true,
                settings: settings);
        }

        private static SyntaxTriviaList Generate(
            TypeParameterListSyntax typeParameterList,
            BaseParameterListSyntax parameterList,
            bool canGenerateReturns = false,
            DocumentationCommentGeneratorSettings settings = null)
        {
            SeparatedSyntaxList<TypeParameterSyntax> typeParameters = typeParameterList?.Parameters ?? default(SeparatedSyntaxList<TypeParameterSyntax>);

            SeparatedSyntaxList<ParameterSyntax> parameters = parameterList?.Parameters ?? default(SeparatedSyntaxList<ParameterSyntax>);

            return Generate(typeParameters, parameters, canGenerateReturns, settings);
        }

        private static SyntaxTriviaList Generate(
            SeparatedSyntaxList<TypeParameterSyntax> typeParameters = default(SeparatedSyntaxList<TypeParameterSyntax>),
            SeparatedSyntaxList<ParameterSyntax> parameters = default(SeparatedSyntaxList<ParameterSyntax>),
            bool canGenerateReturns = false,
            DocumentationCommentGeneratorSettings settings = null)
        {
            settings = settings ?? DocumentationCommentGeneratorSettings.Default;

            ImmutableArray<string> summary = settings.Summary;

            StringBuilder sb = StringBuilderCache.GetInstance();

            sb.Append(settings.Indentation);
            sb.Append("/// <summary>");

            if (settings.SingleLineSummary
                && summary.Length <= 1)
            {
                if (summary.Length == 1)
                    sb.Append(summary[0]);

                sb.AppendLine("</summary>");
            }
            else
            {
                sb.AppendLine();

                if (summary.Any())
                {
                    foreach (string comment in summary)
                    {
                        sb.Append(settings.Indentation);
                        sb.Append("/// ");
                        sb.AppendLine(comment);
                    }
                }
                else
                {
                    sb.Append(settings.Indentation);
                    sb.AppendLine("/// ");
                }

                sb.Append(settings.Indentation);
                sb.AppendLine("/// </summary>");
            }

            foreach (TypeParameterSyntax typeParameter in typeParameters)
            {
                sb.Append(settings.Indentation);
                sb.Append("/// <typeparam name=\"");
                sb.Append(typeParameter.Identifier.ValueText);
                sb.AppendLine("\"></typeparam>");
            }

            foreach (ParameterSyntax parameter in parameters)
            {
                sb.Append(settings.Indentation);
                sb.Append("/// <param name=\"");
                sb.Append(parameter.Identifier.ValueText);
                sb.AppendLine("\"></param>");
            }

            if (canGenerateReturns
                && settings.Returns)
            {
                sb.Append(settings.Indentation);
                sb.AppendLine("/// <returns></returns>");
            }

            return ParseLeadingTrivia(StringBuilderCache.GetStringAndFree(sb));
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

        internal static DocumentationCommentData GenerateFromBase(MemberDeclarationSyntax memberDeclaration, SemanticModel semanticModel, CancellationToken cancellationToken = default(CancellationToken))
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

        internal static DocumentationCommentData GenerateFromBase(MethodDeclarationSyntax methodDeclaration, SemanticModel semanticModel, CancellationToken cancellationToken = default(CancellationToken))
        {
            IMethodSymbol methodSymbol = semanticModel.GetDeclaredSymbol(methodDeclaration, cancellationToken);

            if (methodSymbol?.IsErrorType() == false)
            {
                int position = methodDeclaration.SpanStart;

                SyntaxTrivia trivia = GenerateFromOverriddenMethods(methodSymbol, semanticModel, position, cancellationToken);

                if (trivia.IsKind(SyntaxKind.SingleLineDocumentationCommentTrivia))
                    return new DocumentationCommentData(trivia, DocumentationCommentOrigin.BaseMember);

                trivia = GenerateFromInterfaceMember<IMethodSymbol>(methodSymbol, semanticModel, position, cancellationToken);

                if (trivia.IsKind(SyntaxKind.SingleLineDocumentationCommentTrivia))
                    return new DocumentationCommentData(trivia, DocumentationCommentOrigin.InterfaceMember);
            }

            return default(DocumentationCommentData);
        }

        internal static DocumentationCommentData GenerateFromBase(PropertyDeclarationSyntax propertyDeclaration, SemanticModel semanticModel, CancellationToken cancellationToken = default(CancellationToken))
        {
            IPropertySymbol propertySymbol = semanticModel.GetDeclaredSymbol(propertyDeclaration, cancellationToken);

            return GenerateFromBase(propertySymbol, semanticModel, propertyDeclaration.SpanStart, cancellationToken);
        }

        internal static DocumentationCommentData GenerateFromBase(IndexerDeclarationSyntax indexerDeclaration, SemanticModel semanticModel, CancellationToken cancellationToken = default(CancellationToken))
        {
            IPropertySymbol propertySymbol = semanticModel.GetDeclaredSymbol(indexerDeclaration, cancellationToken);

            return GenerateFromBase(propertySymbol, semanticModel, indexerDeclaration.SpanStart, cancellationToken);
        }

        private static DocumentationCommentData GenerateFromBase(IPropertySymbol propertySymbol, SemanticModel semanticModel, int position, CancellationToken cancellationToken)
        {
            if (propertySymbol?.IsErrorType() == false)
            {
                SyntaxTrivia trivia = GenerateFromOverriddenProperties(propertySymbol, semanticModel, position, cancellationToken);

                if (trivia.IsKind(SyntaxKind.SingleLineDocumentationCommentTrivia))
                    return new DocumentationCommentData(trivia, DocumentationCommentOrigin.BaseMember);

                trivia = GenerateFromInterfaceMember<IPropertySymbol>(propertySymbol, semanticModel, position, cancellationToken);

                if (trivia.IsKind(SyntaxKind.SingleLineDocumentationCommentTrivia))
                    return new DocumentationCommentData(trivia, DocumentationCommentOrigin.InterfaceMember);
            }

            return default(DocumentationCommentData);
        }

        internal static DocumentationCommentData GenerateFromBase(EventDeclarationSyntax eventDeclaration, SemanticModel semanticModel, CancellationToken cancellationToken = default(CancellationToken))
        {
            IEventSymbol eventSymbol = semanticModel.GetDeclaredSymbol(eventDeclaration, cancellationToken);

            return GenerateFromBase(eventSymbol, semanticModel, eventDeclaration.SpanStart, cancellationToken);
        }

        internal static DocumentationCommentData GenerateFromBase(EventFieldDeclarationSyntax eventFieldDeclaration, SemanticModel semanticModel, CancellationToken cancellationToken = default(CancellationToken))
        {
            VariableDeclaratorSyntax variableDeclarator = eventFieldDeclaration.Declaration?.Variables.FirstOrDefault();

            if (variableDeclarator != null)
            {
                var eventSymbol = semanticModel.GetDeclaredSymbol(variableDeclarator, cancellationToken) as IEventSymbol;

                return GenerateFromBase(eventSymbol, semanticModel, eventFieldDeclaration.SpanStart, cancellationToken);
            }

            return default(DocumentationCommentData);
        }

        private static DocumentationCommentData GenerateFromBase(IEventSymbol eventSymbol, SemanticModel semanticModel, int position, CancellationToken cancellationToken)
        {
            if (eventSymbol?.IsErrorType() == false)
            {
                SyntaxTrivia trivia = GenerateFromOverriddenEvents(eventSymbol, semanticModel, position, cancellationToken);

                if (trivia.IsKind(SyntaxKind.SingleLineDocumentationCommentTrivia))
                    return new DocumentationCommentData(trivia, DocumentationCommentOrigin.BaseMember);

                trivia = GenerateFromInterfaceMember<IEventSymbol>(eventSymbol, semanticModel, position, cancellationToken);

                if (trivia.IsKind(SyntaxKind.SingleLineDocumentationCommentTrivia))
                    return new DocumentationCommentData(trivia, DocumentationCommentOrigin.InterfaceMember);
            }

            return default(DocumentationCommentData);
        }

        internal static DocumentationCommentData GenerateFromBase(ConstructorDeclarationSyntax constructorDeclaration, SemanticModel semanticModel, CancellationToken cancellationToken = default(CancellationToken))
        {
            ConstructorInitializerSyntax initializer = constructorDeclaration.Initializer;

            if (initializer?.Kind() == SyntaxKind.BaseConstructorInitializer)
            {
                ISymbol baseConstructor = semanticModel.GetSymbol(initializer, cancellationToken);

                if (baseConstructor?.IsErrorType() == false)
                {
                    SyntaxTrivia trivia = GetDocumentationCommentTrivia(baseConstructor, semanticModel, constructorDeclaration.SpanStart, cancellationToken);

                    if (trivia.IsKind(SyntaxKind.SingleLineDocumentationCommentTrivia))
                        return new DocumentationCommentData(trivia, DocumentationCommentOrigin.BaseMember);
                }
            }

            return default(DocumentationCommentData);
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
            TInterfaceSymbol interfaceMember = memberSymbol.FindFirstImplementedInterfaceMember<TInterfaceSymbol>();

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
            string xml = symbol.GetDocumentationCommentXml(cancellationToken: cancellationToken);

            if (string.IsNullOrEmpty(xml))
                return default(SyntaxTrivia);

            string innerXml = GetInnerXml(xml);

            Debug.Assert(innerXml != null, xml);

            if (innerXml == null)
                return default(SyntaxTrivia);

            string innerXmlWithSlashes = AddSlashes(innerXml.TrimEnd());

            SyntaxTriviaList leadingTrivia = ParseLeadingTrivia(innerXmlWithSlashes);

            if (leadingTrivia.Count != 1)
                return default(SyntaxTrivia);

            SyntaxTrivia trivia = leadingTrivia.First();

            if (trivia.Kind() != SyntaxKind.SingleLineDocumentationCommentTrivia)
                return default(SyntaxTrivia);

            if (!trivia.HasStructure)
                return default(SyntaxTrivia);

            SyntaxNode structure = trivia.GetStructure();

            if (structure.Kind() != SyntaxKind.SingleLineDocumentationCommentTrivia)
                return default(SyntaxTrivia);

            var commentTrivia = (DocumentationCommentTriviaSyntax)structure;

            var rewriter = new DocumentationCommentTriviaRewriter(position, semanticModel);

            // Remove T: from cref attribute and replace `1 with {T}
            commentTrivia = (DocumentationCommentTriviaSyntax)rewriter.VisitDocumentationCommentTrivia(commentTrivia);

            // Remove <filterpriority> element
            commentTrivia = RemoveFilterPriorityElement(commentTrivia);

            string text = commentTrivia.ToFullString();

            // Remove /// from empty lines
            text = _commentedEmptyLineRegex.Replace(text, "");

            leadingTrivia = ParseLeadingTrivia(text);

            if (leadingTrivia.Count == 1)
                return leadingTrivia.First();

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

                    if (xmlElement.IsLocalName("filterpriority", StringComparison.OrdinalIgnoreCase))
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
            StringBuilder sb = StringBuilderCache.GetInstance();

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

            return StringBuilderCache.GetStringAndFree(sb);
        }
    }
}
