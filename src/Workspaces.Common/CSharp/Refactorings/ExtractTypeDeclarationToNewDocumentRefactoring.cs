// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Collections.Immutable;
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

            return document.Solution();
        }

        private static SyntaxNode RemoveAllButMember(CompilationUnitSyntax compilationUnit, MemberDeclarationSyntax memberDeclaration)
        {
            IEnumerable<MemberDeclarationSyntax> membersToRemove = GetNonNestedTypeDeclarations(compilationUnit.Members)
                .Where(f => f != memberDeclaration);

            CompilationUnitSyntax newCompilationUnit = compilationUnit.RemoveNodes(
                 membersToRemove,
                 SyntaxRemoveOptions.KeepUnbalancedDirectives);

            SyntaxList<AttributeListSyntax> attributeLists = newCompilationUnit.AttributeLists;

            if (attributeLists.Any())
                newCompilationUnit = newCompilationUnit.RemoveNodes(attributeLists, SyntaxRemoveOptions.KeepUnbalancedDirectives);

            return RemoveEmptyNamespaces(newCompilationUnit, SyntaxRemoveOptions.KeepUnbalancedDirectives);
        }

        private static string GetDocumentName(MemberDeclarationSyntax memberDeclaration, SemanticModel semanticModel, CancellationToken cancellationToken)
        {
            string documentName = CSharpUtility.GetIdentifier(memberDeclaration).ValueText;

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

        private static IEnumerable<MemberDeclarationSyntax> GetNonNestedTypeDeclarations(SyntaxList<MemberDeclarationSyntax> members)
        {
            foreach (MemberDeclarationSyntax member in members)
            {
                SyntaxKind kind = member.Kind();

                if (kind == SyntaxKind.NamespaceDeclaration)
                {
                    var namespaceDeclaration = (NamespaceDeclarationSyntax)member;

                    foreach (MemberDeclarationSyntax member2 in GetNonNestedTypeDeclarations(namespaceDeclaration.Members))
                        yield return member2;
                }
                else if (SyntaxFacts.IsTypeDeclaration(kind))
                {
                    yield return member;
                }
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
                .Where(f => f.Kind() == SyntaxKind.NamespaceDeclaration)
                .Cast<NamespaceDeclarationSyntax>()
                .Where(f => !f.Members.Any());

            return node.RemoveNodes(emptyNamespaces, removeOptions);
        }
    }
}
