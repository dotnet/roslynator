// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.CSharp.Extensions;
using Roslynator.CSharp.SyntaxRewriters;

namespace Roslynator.CSharp
{
    public static class DocumentationCommentGenerator
    {
        public static MemberDeclarationSyntax GenerateAndAttach(MemberDeclarationSyntax memberDeclaration, DocumentationCommentGeneratorSettings settings = null)
        {
            if (memberDeclaration == null)
                throw new ArgumentNullException(nameof(memberDeclaration));

            SyntaxTriviaList leadingTrivia = memberDeclaration.GetLeadingTrivia();

            int index = 0;

            string indent = "";

            if (leadingTrivia.Any())
            {
                index = leadingTrivia.Count - 1;

                for (int i = leadingTrivia.Count - 1; i >= 0; i--)
                {
                    if (leadingTrivia[i].IsWhitespaceTrivia())
                    {
                        index = i;
                    }
                    else
                    {
                        break;
                    }
                }

                indent = string.Concat(leadingTrivia.Skip(index));
            }

            settings = settings ?? DocumentationCommentGeneratorSettings.Default;

            settings = settings.WithIndent(indent);

            SyntaxTriviaList comment = Generate(memberDeclaration, settings);

            SyntaxTriviaList newLeadingTrivia = leadingTrivia.InsertRange(index, comment);

            return memberDeclaration.WithLeadingTrivia(newLeadingTrivia);
        }

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

        public static async Task<Document> GenerateAndAttachAsync(Document document, DocumentationCommentGeneratorSettings settings = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (document == null)
                throw new ArgumentNullException(nameof(document));

            SyntaxNode root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);

            SyntaxNode newRoot = GenerateDocumentationCommentSyntaxRewriter.GenerateAndAttach(root, settings);

            return document.WithSyntaxRoot(newRoot);
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
            SeparatedSyntaxList<TypeParameterSyntax> typeParameters = (typeParameterList != null)
                ? typeParameterList.Parameters
                : default(SeparatedSyntaxList<TypeParameterSyntax>);

            SeparatedSyntaxList<ParameterSyntax> parameters = (parameterList != null)
                ? parameterList.Parameters
                : default(SeparatedSyntaxList<ParameterSyntax>);

            return Generate(typeParameters, parameters, generateReturns, settings);
        }

        public static SyntaxTriviaList Generate(
            SeparatedSyntaxList<TypeParameterSyntax> typeParameters = default(SeparatedSyntaxList<TypeParameterSyntax>),
            SeparatedSyntaxList<ParameterSyntax> parameters = default(SeparatedSyntaxList<ParameterSyntax>),
            bool generateReturns = false,
            DocumentationCommentGeneratorSettings settings = null)
        {
            settings = settings ?? DocumentationCommentGeneratorSettings.Default;

            var sb = new StringBuilder();

            sb.Append(settings.Indent);
            sb.Append(@"/// <summary>");

            if (settings.SingleLineSummary)
            {
                sb.AppendLine(@"</summary>");
            }
            else
            {
                sb.AppendLine();
                sb.Append(settings.Indent);
                sb.AppendLine(@"/// ");
                sb.Append(settings.Indent);
                sb.AppendLine(@"/// </summary>");
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
    }
}
