// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Roslynator.CSharp.Refactorings
{
    internal static class ExtractTypeDeclarationToNewDocumentRefactoring
    {
        public static async Task<Solution> RefactorAsync(
            Document document,
            MemberDeclarationSyntax memberDeclaration,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (document == null)
                throw new ArgumentNullException(nameof(document));

            if (memberDeclaration == null)
                throw new ArgumentNullException(nameof(memberDeclaration));

            SyntaxNode root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);

            SemanticModel semanticModel = await document.GetSemanticModelAsync(cancellationToken).ConfigureAwait(false);

            SyntaxNode newRoot = root.RemoveNode(memberDeclaration, SyntaxRemoveOptions.KeepUnbalancedDirectives);

            newRoot = RemoveEmptyNamespaces(newRoot, SyntaxRemoveOptions.KeepUnbalancedDirectives);

            document = document.WithSyntaxRoot(newRoot);

            newRoot = RemoveAllButMember((CompilationUnitSyntax)root, memberDeclaration);

            string documentName = GetDocumentName(memberDeclaration, semanticModel, cancellationToken);

            var folders = new List<string>(document.Folders.Count + 1) { document.Project.Name };

            folders.AddRange(document.Folders);

            document = document.Project.AddDocument(
                documentName,
                newRoot,
                ImmutableArray.CreateRange(folders));

            return document.Project.Solution;
        }

        private static SyntaxNode RemoveAllButMember(CompilationUnitSyntax compilationUnit, MemberDeclarationSyntax memberDeclaration)
        {
            IEnumerable<MemberDeclarationSyntax> membersToRemove = GetNonNestedTypeDeclarations(compilationUnit.Members)
                .Where(f => f != memberDeclaration);

            CompilationUnitSyntax newCompilationUnit = compilationUnit.RemoveNodes(
                 membersToRemove,
                 SyntaxRemoveOptions.KeepUnbalancedDirectives);

            return RemoveEmptyNamespaces(newCompilationUnit, SyntaxRemoveOptions.KeepUnbalancedDirectives);
        }

        private static string GetDocumentName(MemberDeclarationSyntax memberDeclaration, SemanticModel semanticModel, CancellationToken cancellationToken)
        {
            string documentName = GetIdentifier(memberDeclaration).ValueText;

            var namedTypeSymbol = semanticModel.GetDeclaredSymbol(memberDeclaration, cancellationToken) as INamedTypeSymbol;

            if (namedTypeSymbol?.Arity > 0)
                documentName += "`" + namedTypeSymbol.Arity.ToString();

            documentName += ".cs";

            return documentName;
        }

        public static IEnumerable<MemberDeclarationSyntax> GetNonNestedTypeDeclarations(CompilationUnitSyntax compilationUnit)
        {
            return GetNonNestedTypeDeclarations(compilationUnit.Members);
        }

        public static IEnumerable<MemberDeclarationSyntax> GetNonNestedTypeDeclarations(SyntaxList<MemberDeclarationSyntax> members)
        {
            Stack<NamespaceDeclarationSyntax> namespaces = null;

            foreach (MemberDeclarationSyntax member in members)
            {
                SyntaxKind kind = member.Kind();

                if (kind == SyntaxKind.NamespaceDeclaration)
                {
                    (namespaces ?? (namespaces = new Stack<NamespaceDeclarationSyntax>())).Push((NamespaceDeclarationSyntax)member);
                }
                else if (IsTypeDeclaration(kind))
                {
                    yield return member;
                }

                if (namespaces != null)
                {
                    while (namespaces.Count > 0)
                    {
                        NamespaceDeclarationSyntax namespaceDeclaration = namespaces.Pop();

                        foreach (MemberDeclarationSyntax member2 in namespaceDeclaration.Members)
                        {
                            SyntaxKind kind2 = member2.Kind();

                            if (kind2 == SyntaxKind.NamespaceDeclaration)
                            {
                                namespaces.Push((NamespaceDeclarationSyntax)member2);
                            }
                            else if (IsTypeDeclaration(kind2))
                            {
                                yield return member2;
                            }
                        }
                    }
                }
            }
        }

        public static SyntaxToken GetIdentifier(MemberDeclarationSyntax memberDeclaration)
        {
            switch (memberDeclaration.Kind())
            {
                case SyntaxKind.ClassDeclaration:
                    return ((ClassDeclarationSyntax)memberDeclaration).Identifier;
                case SyntaxKind.StructDeclaration:
                    return ((StructDeclarationSyntax)memberDeclaration).Identifier;
                case SyntaxKind.InterfaceDeclaration:
                    return ((InterfaceDeclarationSyntax)memberDeclaration).Identifier;
                case SyntaxKind.EnumDeclaration:
                    return ((EnumDeclarationSyntax)memberDeclaration).Identifier;
                case SyntaxKind.DelegateDeclaration:
                    return ((DelegateDeclarationSyntax)memberDeclaration).Identifier;
            }

            Debug.Assert(false, memberDeclaration.Kind().ToString());

            return default(SyntaxToken);
        }

        private static bool IsTypeDeclaration(SyntaxKind kind)
        {
            switch (kind)
            {
                case SyntaxKind.ClassDeclaration:
                case SyntaxKind.StructDeclaration:
                case SyntaxKind.InterfaceDeclaration:
                case SyntaxKind.EnumDeclaration:
                case SyntaxKind.DelegateDeclaration:
                    return true;
                default:
                    return false;
            }
        }

        public static string GetTitle(string name)
        {
            return $"Extract '{name}' to a new file";
        }

        private static SyntaxNode RemoveEmptyNamespaces(SyntaxNode node, SyntaxRemoveOptions removeOptions)
        {
            IEnumerable<NamespaceDeclarationSyntax> emptyNamespaces = node
                .DescendantNodes()
                .Where(f => f.IsKind(SyntaxKind.NamespaceDeclaration))
                .Cast<NamespaceDeclarationSyntax>()
                .Where(f => !f.Members.Any());

            return node.RemoveNodes(emptyNamespaces, removeOptions);
        }
    }
}
