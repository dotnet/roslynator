// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Roslynator.CSharp.Analysis.Documentation
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class AddTypeParamElementToDocumentationCommentAnalyzer : BaseDiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        {
            get { return ImmutableArray.Create(DiagnosticDescriptors.AddTypeParamElementToDocumentationComment); }
        }

        public override void Initialize(AnalysisContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            base.Initialize(context);
            context.EnableConcurrentExecution();

            context.RegisterSyntaxNodeAction(AnalyzeSingleLineDocumentationCommentTrivia, SyntaxKind.SingleLineDocumentationCommentTrivia);
        }

        private static void AnalyzeSingleLineDocumentationCommentTrivia(SyntaxNodeAnalysisContext context)
        {
            var documentationComment = (DocumentationCommentTriviaSyntax)context.Node;

            SyntaxNode node = documentationComment.ParentTrivia.Token.Parent;

            (MemberDeclarationSyntax declaration, TypeParameterListSyntax typeParameterList) result = GetTypeParameterList(node);

            if (result.declaration == null)
            {
                result = GetTypeParameterList(node.Parent);

                if (result.declaration == null)
                    return;
            }

            SeparatedSyntaxList<TypeParameterSyntax> typeParameters = result.typeParameterList?.Parameters ?? default;

            if (!typeParameters.Any())
                return;

            ImmutableArray<string> values = DocumentationCommentAnalysis.GetAttributeValues(documentationComment, XmlElementKind.TypeParam, "name");

            if (values.IsDefault)
                return;

            foreach (TypeParameterSyntax typeParameter in typeParameters)
            {
                if (!typeParameter.IsMissing
                    && !values.Contains(typeParameter.Identifier.ValueText))
                {
                    context.ReportDiagnostic(DiagnosticDescriptors.AddTypeParamElementToDocumentationComment, documentationComment);
                    return;
                }
            }
        }

        private static (MemberDeclarationSyntax declaration, TypeParameterListSyntax typeParameterList) GetTypeParameterList(SyntaxNode node)
        {
            switch (node.Kind())
            {
                case SyntaxKind.ClassDeclaration:
                    {
                        var classDeclaration = (ClassDeclarationSyntax)node;
                        return (classDeclaration, classDeclaration.TypeParameterList);
                    }
                case SyntaxKind.InterfaceDeclaration:
                    {
                        var interfaceDeclaration = (InterfaceDeclarationSyntax)node;
                        return (interfaceDeclaration, interfaceDeclaration.TypeParameterList);
                    }
                case SyntaxKind.StructDeclaration:
                    {
                        var structDeclaration = (StructDeclarationSyntax)node;
                        return (structDeclaration, structDeclaration.TypeParameterList);
                    }
                case SyntaxKind.MethodDeclaration:
                    {
                        var methodDeclaration = (MethodDeclarationSyntax)node;
                        return (methodDeclaration, methodDeclaration.TypeParameterList);
                    }
                case SyntaxKind.DelegateDeclaration:
                    {
                        var delegateDeclaration = (DelegateDeclarationSyntax)node;
                        return (delegateDeclaration, delegateDeclaration.TypeParameterList);
                    }
            }

            return default;
        }
    }
}
