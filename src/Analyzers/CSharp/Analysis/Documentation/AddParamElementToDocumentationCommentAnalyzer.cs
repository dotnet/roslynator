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
    public class AddParamElementToDocumentationCommentAnalyzer : BaseDiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        {
            get { return ImmutableArray.Create(DiagnosticDescriptors.AddParamElementToDocumentationComment); }
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

            (MemberDeclarationSyntax declaration, BaseParameterListSyntax parameterList) result = GetParameterList(node);

            if (result.declaration == null)
            {
                result = GetParameterList(node.Parent);

                if (result.declaration == null)
                    return;
            }

            SeparatedSyntaxList<ParameterSyntax> parameters = result.parameterList?.Parameters ?? default;

            if (!parameters.Any())
                return;

            ImmutableArray<string> values = DocumentationCommentAnalysis.GetAttributeValues(documentationComment, XmlElementKind.Param, "name");

            if (values.IsDefault)
                return;

            foreach (ParameterSyntax parameter in parameters)
            {
                if (!parameter.IsMissing
                    && !values.Contains(parameter.Identifier.ValueText))
                {
                    context.ReportDiagnostic(DiagnosticDescriptors.AddParamElementToDocumentationComment, documentationComment);
                    return;
                }
            }
        }

        private static (MemberDeclarationSyntax declaration, BaseParameterListSyntax parameterList)
            GetParameterList(SyntaxNode node)
        {
            switch (node.Kind())
            {
                case SyntaxKind.MethodDeclaration:
                    {
                        var methodDeclaration = (MethodDeclarationSyntax)node;
                        return (methodDeclaration, methodDeclaration.ParameterList);
                    }
                case SyntaxKind.ConstructorDeclaration:
                    {
                        var constructorDeclaration = (ConstructorDeclarationSyntax)node;
                        return (constructorDeclaration, constructorDeclaration.ParameterList);
                    }
                case SyntaxKind.OperatorDeclaration:
                    {
                        var operatorDeclaration = (OperatorDeclarationSyntax)node;
                        return (operatorDeclaration, operatorDeclaration.ParameterList);
                    }
                case SyntaxKind.ConversionOperatorDeclaration:
                    {
                        var conversionOperatorDeclaration = (ConversionOperatorDeclarationSyntax)node;
                        return (conversionOperatorDeclaration, conversionOperatorDeclaration.ParameterList);
                    }
                case SyntaxKind.DelegateDeclaration:
                    {
                        var delegateDeclaration = (DelegateDeclarationSyntax)node;
                        return (delegateDeclaration, delegateDeclaration.ParameterList);
                    }
                case SyntaxKind.IndexerDeclaration:
                    {
                        var indexerDeclaration = (IndexerDeclarationSyntax)node;
                        return (indexerDeclaration, indexerDeclaration.ParameterList);
                    }
            }

            return default;
        }
    }
}
