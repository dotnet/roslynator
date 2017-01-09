// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

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
                    ComputeRefactoring<IMethodSymbol>(context, methodDeclaration, methodSymbol, methodSymbol.OverriddenMethod);
            }
        }

        public static async Task ComputeRefactoringAsync(RefactoringContext context, PropertyDeclarationSyntax propertyDeclaration)
        {
            if (!propertyDeclaration.HasSingleLineDocumentationComment())
            {
                SemanticModel semanticModel = await context.GetSemanticModelAsync().ConfigureAwait(false);

                IPropertySymbol propertySymbol = semanticModel.GetDeclaredSymbol(propertyDeclaration);

                if (propertySymbol?.IsErrorType() == false)
                    ComputeRefactoring<IPropertySymbol>(context, propertyDeclaration, propertySymbol, propertySymbol.OverriddenProperty);
            }
        }

        public static async Task ComputeRefactoringAsync(RefactoringContext context, IndexerDeclarationSyntax indexerDeclaration)
        {
            if (!indexerDeclaration.HasSingleLineDocumentationComment())
            {
                SemanticModel semanticModel = await context.GetSemanticModelAsync().ConfigureAwait(false);

                IPropertySymbol propertySymbol = semanticModel.GetDeclaredSymbol(indexerDeclaration);

                if (propertySymbol?.IsErrorType() == false)
                    ComputeRefactoring<IPropertySymbol>(context, indexerDeclaration, propertySymbol, propertySymbol.OverriddenProperty);
            }
        }

        public static async Task ComputeRefactoringAsync(RefactoringContext context, EventDeclarationSyntax eventDeclaration)
        {
            if (!eventDeclaration.HasSingleLineDocumentationComment())
            {
                SemanticModel semanticModel = await context.GetSemanticModelAsync().ConfigureAwait(false);

                IEventSymbol eventSymbol = semanticModel.GetDeclaredSymbol(eventDeclaration);

                if (eventSymbol?.IsErrorType() == false)
                    ComputeRefactoring<IEventSymbol>(context, eventDeclaration, eventSymbol, eventSymbol.OverriddenEvent);
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
                        ComputeRefactoring<IEventSymbol>(context, eventFieldDeclaration, eventSymbol, eventSymbol.OverriddenEvent);
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
                        ComputeRefactoring<ISymbol>(context, constructorDeclaration, null, symbol);
                }
            }
        }

        private static void ComputeRefactoring<TInterfaceSymbol>(
            RefactoringContext context,
            MemberDeclarationSyntax memberDeclaration,
            ISymbol memberSymbol,
            ISymbol baseSymbol) where TInterfaceSymbol : ISymbol
        {
            var commentTrivia = default(SyntaxTrivia);
            string title = null;

            if (baseSymbol != null)
            {
                commentTrivia = CreateDocumentationCommentTrivia(baseSymbol, context.CancellationToken);

                if (commentTrivia.IsKind(SyntaxKind.SingleLineDocumentationCommentTrivia))
                    title = GetTitleForBaseMember(memberDeclaration);
            }

            if (!commentTrivia.IsKind(SyntaxKind.SingleLineDocumentationCommentTrivia)
                && memberSymbol != null)
            {
                TInterfaceSymbol interfaceMember = memberSymbol.FindImplementedInterfaceMember<TInterfaceSymbol>();

                if (interfaceMember != null)
                {
                    commentTrivia = CreateDocumentationCommentTrivia(interfaceMember, context.CancellationToken);

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

        public static SyntaxTrivia CreateDocumentationCommentTrivia(ISymbol symbol, CancellationToken cancellationToken = default(CancellationToken))
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
                        return triviaList.First();
                }

                Debug.Assert(false, xmlString);
            }

            return default(SyntaxTrivia);
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
    }
}