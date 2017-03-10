// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.CSharp.Extensions;

namespace Roslynator.CSharp.Documentation
{
    public static partial class DocumentationCommentGenerator
    {
        private class Rewriter : CSharpSyntaxRewriter
        {
            private readonly SyntaxNode _node;
            private readonly DocumentationCommentGeneratorSettings _settings;

            public Rewriter(SyntaxNode node, DocumentationCommentGeneratorSettings settings)
            {
                _node = node;
                _settings = settings ?? DocumentationCommentGeneratorSettings.Default;
            }

            public static TNode GenerateAndInsert<TNode>(
                TNode node,
                DocumentationCommentGeneratorSettings settings = null) where TNode : SyntaxNode
            {
                if (node == null)
                    throw new ArgumentNullException(nameof(node));

                var rewriter = new Rewriter(node, settings);

                return (TNode)rewriter.Visit(node);
            }

            public override SyntaxNode VisitNamespaceDeclaration(NamespaceDeclarationSyntax node)
            {
                node = (NamespaceDeclarationSyntax)base.VisitNamespaceDeclaration(node);

                if (!_settings.SkipNamespaceDeclaration
                    && !node.HasSingleLineDocumentationComment())
                {
                    return DocumentationCommentGenerator.GenerateAndInsert(node, _settings);
                }
                else
                {
                    return node;
                }
            }

            public override SyntaxNode VisitClassDeclaration(ClassDeclarationSyntax node)
            {
                bool isPubliclyVisible = node.IsPubliclyVisible();

                node = (ClassDeclarationSyntax)base.VisitClassDeclaration(node);

                if (isPubliclyVisible
                    && !node.HasSingleLineDocumentationComment())
                {
                    return DocumentationCommentGenerator.GenerateAndInsert(node, _settings);
                }
                else
                {
                    return node;
                }
            }

            public override SyntaxNode VisitStructDeclaration(StructDeclarationSyntax node)
            {
                bool isPubliclyVisible = node.IsPubliclyVisible();

                node = (StructDeclarationSyntax)base.VisitStructDeclaration(node);

                if (isPubliclyVisible
                    && !node.HasSingleLineDocumentationComment())
                {
                    return DocumentationCommentGenerator.GenerateAndInsert(node, _settings);
                }
                else
                {
                    return node;
                }
            }

            public override SyntaxNode VisitInterfaceDeclaration(InterfaceDeclarationSyntax node)
            {
                bool isPubliclyVisible = node.IsPubliclyVisible();

                node = (InterfaceDeclarationSyntax)base.VisitInterfaceDeclaration(node);

                if (isPubliclyVisible
                    && !node.HasSingleLineDocumentationComment())
                {
                    return DocumentationCommentGenerator.GenerateAndInsert(node, _settings);
                }
                else
                {
                    return node;
                }
            }

            public override SyntaxNode VisitEnumDeclaration(EnumDeclarationSyntax node)
            {
                bool isPubliclyVisible = node.IsPubliclyVisible();

                node = (EnumDeclarationSyntax)base.VisitEnumDeclaration(node);

                if (isPubliclyVisible
                    && !node.HasSingleLineDocumentationComment())
                {
                    return DocumentationCommentGenerator.GenerateAndInsert(node, _settings);
                }
                else
                {
                    return node;
                }
            }

            public override SyntaxNode VisitDelegateDeclaration(DelegateDeclarationSyntax node)
            {
                bool isPubliclyVisible = node.IsPubliclyVisible();

                node = (DelegateDeclarationSyntax)base.VisitDelegateDeclaration(node);

                if (isPubliclyVisible
                    && !node.HasSingleLineDocumentationComment())
                {
                    return DocumentationCommentGenerator.GenerateAndInsert(node, _settings);
                }
                else
                {
                    return node;
                }
            }

            public override SyntaxNode VisitEnumMemberDeclaration(EnumMemberDeclarationSyntax node)
            {
                bool isPubliclyVisible = node.IsPubliclyVisible();

                node = (EnumMemberDeclarationSyntax)base.VisitEnumMemberDeclaration(node);

                if (isPubliclyVisible
                    && !node.HasSingleLineDocumentationComment())
                {
                    return DocumentationCommentGenerator.GenerateAndInsert(node, _settings);
                }
                else
                {
                    return node;
                }
            }

            public override SyntaxNode VisitFieldDeclaration(FieldDeclarationSyntax node)
            {
                bool isPubliclyVisible = node.IsPubliclyVisible();

                node = (FieldDeclarationSyntax)base.VisitFieldDeclaration(node);

                if (isPubliclyVisible
                    && !node.HasSingleLineDocumentationComment())
                {
                    return DocumentationCommentGenerator.GenerateAndInsert(node, _settings);
                }
                else
                {
                    return node;
                }
            }

            public override SyntaxNode VisitEventFieldDeclaration(EventFieldDeclarationSyntax node)
            {
                bool isPubliclyVisible = node.IsPubliclyVisible();

                node = (EventFieldDeclarationSyntax)base.VisitEventFieldDeclaration(node);

                if (isPubliclyVisible
                    && !node.HasSingleLineDocumentationComment())
                {
                    return DocumentationCommentGenerator.GenerateAndInsert(node, _settings);
                }
                else
                {
                    return node;
                }
            }

            public override SyntaxNode VisitMethodDeclaration(MethodDeclarationSyntax node)
            {
                bool isPubliclyVisible = node.IsPubliclyVisible();

                node = (MethodDeclarationSyntax)base.VisitMethodDeclaration(node);

                if (isPubliclyVisible
                    && !node.HasSingleLineDocumentationComment())
                {
                    return DocumentationCommentGenerator.GenerateAndInsert(node, _settings);
                }
                else
                {
                    return node;
                }
            }

            public override SyntaxNode VisitOperatorDeclaration(OperatorDeclarationSyntax node)
            {
                bool isPubliclyVisible = node.IsPubliclyVisible();

                node = (OperatorDeclarationSyntax)base.VisitOperatorDeclaration(node);

                if (isPubliclyVisible
                    && !node.HasSingleLineDocumentationComment())
                {
                    return DocumentationCommentGenerator.GenerateAndInsert(node, _settings);
                }
                else
                {
                    return node;
                }
            }

            public override SyntaxNode VisitConversionOperatorDeclaration(ConversionOperatorDeclarationSyntax node)
            {
                bool isPubliclyVisible = node.IsPubliclyVisible();

                node = (ConversionOperatorDeclarationSyntax)base.VisitConversionOperatorDeclaration(node);

                if (isPubliclyVisible
                    && !node.HasSingleLineDocumentationComment())
                {
                    return DocumentationCommentGenerator.GenerateAndInsert(node, _settings);
                }
                else
                {
                    return node;
                }
            }

            public override SyntaxNode VisitConstructorDeclaration(ConstructorDeclarationSyntax node)
            {
                bool isPubliclyVisible = node.IsPubliclyVisible();

                node = (ConstructorDeclarationSyntax)base.VisitConstructorDeclaration(node);

                if (isPubliclyVisible
                    && !node.HasSingleLineDocumentationComment())
                {
                    return DocumentationCommentGenerator.GenerateAndInsert(node, _settings);
                }
                else
                {
                    return node;
                }
            }

            public override SyntaxNode VisitDestructorDeclaration(DestructorDeclarationSyntax node)
            {
                bool isPubliclyVisible = node.IsPubliclyVisible();

                node = (DestructorDeclarationSyntax)base.VisitDestructorDeclaration(node);

                if (isPubliclyVisible
                    && !node.HasSingleLineDocumentationComment())
                {
                    return DocumentationCommentGenerator.GenerateAndInsert(node, _settings);
                }
                else
                {
                    return node;
                }
            }

            public override SyntaxNode VisitPropertyDeclaration(PropertyDeclarationSyntax node)
            {
                bool isPubliclyVisible = node.IsPubliclyVisible();

                node = (PropertyDeclarationSyntax)base.VisitPropertyDeclaration(node);

                if (isPubliclyVisible
                    && !node.HasSingleLineDocumentationComment())
                {
                    return DocumentationCommentGenerator.GenerateAndInsert(node, _settings);
                }
                else
                {
                    return node;
                }
            }

            public override SyntaxNode VisitEventDeclaration(EventDeclarationSyntax node)
            {
                bool isPubliclyVisible = node.IsPubliclyVisible();

                node = (EventDeclarationSyntax)base.VisitEventDeclaration(node);

                if (isPubliclyVisible
                    && !node.HasSingleLineDocumentationComment())
                {
                    return DocumentationCommentGenerator.GenerateAndInsert(node, _settings);
                }
                else
                {
                    return node;
                }
            }

            public override SyntaxNode VisitIndexerDeclaration(IndexerDeclarationSyntax node)
            {
                bool isPubliclyVisible = node.IsPubliclyVisible();

                node = (IndexerDeclarationSyntax)base.VisitIndexerDeclaration(node);

                if (isPubliclyVisible
                    && !node.HasSingleLineDocumentationComment())
                {
                    return DocumentationCommentGenerator.GenerateAndInsert(node, _settings);
                }
                else
                {
                    return node;
                }
            }
        }
    }
}
