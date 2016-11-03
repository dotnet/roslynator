// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.FindSymbols;
using Roslynator.CSharp.SyntaxRewriters;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static Roslynator.CSharp.CSharpFactory;

namespace Roslynator.CSharp.CodeFixProviders
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(PropertyDeclarationCodeFixProvider))]
    [Shared]
    public class PropertyDeclarationCodeFixProvider : BaseCodeFixProvider
    {
        public sealed override ImmutableArray<string> FixableDiagnosticIds
        {
            get { return ImmutableArray.Create(DiagnosticIdentifiers.ReplacePropertyWithAutoImplementedProperty); }
        }

        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            SyntaxNode root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);

            PropertyDeclarationSyntax property = root
                .FindNode(context.Span, getInnermostNodeForTie: true)?
                .FirstAncestorOrSelf<PropertyDeclarationSyntax>();

            if (property == null)
                return;

            foreach (Diagnostic diagnostic in context.Diagnostics)
            {
                switch (diagnostic.Id)
                {
                    case DiagnosticIdentifiers.ReplacePropertyWithAutoImplementedProperty:
                        {
                            CodeAction codeAction = CodeAction.Create(
                                "Use auto-property",
                                cancellationToken => ReplacePropertyWithAutoPropertyAsync(context.Document, property, cancellationToken),
                                diagnostic.Id + EquivalenceKeySuffix);

                            context.RegisterCodeFix(codeAction, diagnostic);

                            break;
                        }
                }
            }
        }

        private static async Task<Document> ReplacePropertyWithAutoPropertyAsync(
            Document document,
            PropertyDeclarationSyntax property,
            CancellationToken cancellationToken)
        {
            SyntaxNode oldRoot = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);

            var parentMember = (MemberDeclarationSyntax)property.Parent;

            SemanticModel semanticModel = await document.GetSemanticModelAsync(cancellationToken).ConfigureAwait(false);

            ISymbol fieldSymbol = GetFieldSymbol(property, semanticModel, cancellationToken);

            var declarator = (VariableDeclaratorSyntax)await fieldSymbol.DeclaringSyntaxReferences[0].GetSyntaxAsync(cancellationToken).ConfigureAwait(false);

            var variableDeclaration = (VariableDeclarationSyntax)declarator.Parent;

            SyntaxList<MemberDeclarationSyntax> members = parentMember.GetMembers();

            int propertyIndex = members.IndexOf(property);

            int fieldIndex = members.IndexOf((FieldDeclarationSyntax)variableDeclaration.Parent);

            IEnumerable<ReferencedSymbol> referencedSymbols = await SymbolFinder.FindReferencesAsync(
                fieldSymbol,
                document.Project.Solution,
                cancellationToken).ConfigureAwait(false);

            ImmutableArray<IdentifierNameSyntax> identifierNames = SyntaxUtility
                .FindNodes<IdentifierNameSyntax>(oldRoot, referencedSymbols)
                .ToImmutableArray();

            var rewriter = new IdentifierNameSyntaxRewriter(identifierNames, Identifier(property.Identifier.ValueText));

            var newParentMember = (MemberDeclarationSyntax)rewriter.Visit(parentMember);

            members = newParentMember.GetMembers();

            if (variableDeclaration.Variables.Count == 1)
            {
                newParentMember = newParentMember.RemoveNode(
                    newParentMember.GetMemberAt(fieldIndex),
                    SyntaxRemoveOptions.KeepUnbalancedDirectives);

                if (propertyIndex > fieldIndex)
                    propertyIndex--;
            }
            else
            {
                var field = (FieldDeclarationSyntax)members[fieldIndex];

                FieldDeclarationSyntax newField = field.RemoveNode(
                    field.Declaration.Variables[variableDeclaration.Variables.IndexOf(declarator)],
                    SyntaxRemoveOptions.KeepUnbalancedDirectives);

                members = members.Replace(field, newField.WithFormatterAnnotation());

                newParentMember = newParentMember.SetMembers(members);
            }

            members = newParentMember.GetMembers();

            property = (PropertyDeclarationSyntax)members[propertyIndex];

            PropertyDeclarationSyntax newProperty = CreateAutoProperty(property, declarator.Initializer);

            members = members.Replace(property, newProperty);

            newParentMember = newParentMember.SetMembers(members);

            SyntaxNode newRoot = oldRoot.ReplaceNode(parentMember, newParentMember);

            return document.WithSyntaxRoot(newRoot);
        }

        private static ISymbol GetFieldSymbol(PropertyDeclarationSyntax property, SemanticModel semanticModel, CancellationToken cancellationToken)
        {
            if (property.ExpressionBody != null)
            {
                return semanticModel
                    .GetSymbolInfo(property.ExpressionBody.Expression, cancellationToken)
                    .Symbol;
            }
            else
            {
                var returnStatement = (ReturnStatementSyntax)property.Getter().Body.Statements[0];

                return semanticModel
                    .GetSymbolInfo(returnStatement.Expression, cancellationToken)
                    .Symbol;
            }
        }

        private static PropertyDeclarationSyntax CreateAutoProperty(PropertyDeclarationSyntax property, EqualsValueClauseSyntax initializer)
        {
            AccessorListSyntax accessorList = CreateAccessorList(property);

            if (accessorList
                .DescendantTrivia()
                .All(f => f.IsWhitespaceOrEndOfLineTrivia()))
            {
                accessorList = SyntaxRemover.RemoveWhitespaceOrEndOfLine(accessorList);
            }

            PropertyDeclarationSyntax newProperty = property
                .WithIdentifier(property.Identifier.WithTrailingSpace())
                .WithExpressionBody(null)
                .WithAccessorList(accessorList);

            if (initializer != null)
            {
                newProperty = newProperty
                    .WithInitializer(initializer)
                    .WithSemicolonToken(SemicolonToken());
            }
            else
            {
                newProperty = newProperty.WithoutSemicolonToken();
            }

            return newProperty
                .WithTriviaFrom(property)
                .WithFormatterAnnotation();
        }

        private static AccessorListSyntax CreateAccessorList(PropertyDeclarationSyntax property)
        {
            if (property.ExpressionBody != null)
            {
                return AccessorList(AutoGetter())
                    .WithTriviaFrom(property.ExpressionBody);
            }
            else
            {
                AccessorListSyntax accessorList = property.AccessorList;

                IEnumerable<AccessorDeclarationSyntax> newAccessors = accessorList
                    .Accessors
                    .Select(accessor =>
                    {
                        return accessor
                            .WithBody(null)
                            .WithSemicolonToken(SemicolonToken())
                            .WithTriviaFrom(accessor);
                    });

                return accessorList.WithAccessors(List(newAccessors));
            }
        }
    }
}
