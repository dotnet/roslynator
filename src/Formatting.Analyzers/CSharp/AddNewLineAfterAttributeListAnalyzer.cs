// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Text;

namespace Roslynator.Formatting.CSharp
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    internal class AddNewLineAfterAttributeListAnalyzer : BaseDiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        {
            get { return ImmutableArray.Create(DiagnosticDescriptors.AddNewLineAfterAttributeList); }
        }

        public override void Initialize(AnalysisContext context)
        {
            base.Initialize(context);

            context.RegisterSyntaxNodeAction(AnalyzeBaseTypeDeclaration, SyntaxKind.ClassDeclaration);
            context.RegisterSyntaxNodeAction(AnalyzeBaseMethodDeclaration, SyntaxKind.ConstructorDeclaration);
            context.RegisterSyntaxNodeAction(AnalyzeBaseMethodDeclaration, SyntaxKind.ConversionOperatorDeclaration);
            context.RegisterSyntaxNodeAction(AnalyzeDelegateDeclaration, SyntaxKind.DelegateDeclaration);
            context.RegisterSyntaxNodeAction(AnalyzeBaseMethodDeclaration, SyntaxKind.DestructorDeclaration);
            context.RegisterSyntaxNodeAction(AnalyzeBaseTypeDeclaration, SyntaxKind.EnumDeclaration);
            context.RegisterSyntaxNodeAction(AnalyzeEnumMemberDeclaration, SyntaxKind.EnumMemberDeclaration);
            context.RegisterSyntaxNodeAction(AnalyzeBasePropertyDeclaration, SyntaxKind.EventDeclaration);
            context.RegisterSyntaxNodeAction(AnalyzeBaseFieldDeclaration, SyntaxKind.EventFieldDeclaration);
            context.RegisterSyntaxNodeAction(AnalyzeBaseFieldDeclaration, SyntaxKind.FieldDeclaration);
            context.RegisterSyntaxNodeAction(AnalyzeBasePropertyDeclaration, SyntaxKind.IndexerDeclaration);
            context.RegisterSyntaxNodeAction(AnalyzeBaseTypeDeclaration, SyntaxKind.InterfaceDeclaration);
            context.RegisterSyntaxNodeAction(AnalyzeBaseMethodDeclaration, SyntaxKind.MethodDeclaration);
            context.RegisterSyntaxNodeAction(AnalyzeBaseMethodDeclaration, SyntaxKind.OperatorDeclaration);
            context.RegisterSyntaxNodeAction(AnalyzeBasePropertyDeclaration, SyntaxKind.PropertyDeclaration);
            context.RegisterSyntaxNodeAction(AnalyzeBaseTypeDeclaration, SyntaxKind.StructDeclaration);
            context.RegisterSyntaxNodeAction(AnalyzeAccessorDeclaration, SyntaxKind.GetAccessorDeclaration);
            context.RegisterSyntaxNodeAction(AnalyzeAccessorDeclaration, SyntaxKind.SetAccessorDeclaration);
            context.RegisterSyntaxNodeAction(AnalyzeAccessorDeclaration, SyntaxKind.AddAccessorDeclaration);
            context.RegisterSyntaxNodeAction(AnalyzeAccessorDeclaration, SyntaxKind.RemoveAccessorDeclaration);
        }

        private static void AnalyzeDelegateDeclaration(SyntaxNodeAnalysisContext context)
        {
            var delegateDeclaration = (DelegateDeclarationSyntax)context.Node;

            AttributeListSyntax attributeList = delegateDeclaration.AttributeLists.LastOrDefault();

            if (attributeList == null)
                return;

            SyntaxTokenList modifiers = delegateDeclaration.Modifiers;

            SyntaxToken token = (modifiers.Any())
                ? modifiers[0]
                : delegateDeclaration.DelegateKeyword;

            Analyze(context, attributeList, token);
        }

        private static void AnalyzeEnumMemberDeclaration(SyntaxNodeAnalysisContext context)
        {
            var enumMemberDeclaration = (EnumMemberDeclarationSyntax)context.Node;

            AttributeListSyntax attributeList = enumMemberDeclaration.AttributeLists.LastOrDefault();

            if (attributeList == null)
                return;

            SyntaxToken token = enumMemberDeclaration.Identifier;

            Analyze(context, attributeList, token);
        }

        private static void AnalyzeBaseFieldDeclaration(SyntaxNodeAnalysisContext context)
        {
            var baseFieldDeclaration = (BaseFieldDeclarationSyntax)context.Node;

            AttributeListSyntax attributeList = baseFieldDeclaration.AttributeLists.LastOrDefault();

            if (attributeList == null)
                return;

            SyntaxTokenList modifiers = baseFieldDeclaration.Modifiers;

            SyntaxToken token;

            if (modifiers.Any())
            {
                token = modifiers[0];
            }
            else if (baseFieldDeclaration is EventFieldDeclarationSyntax eventFieldDeclaration)
            {
                token = eventFieldDeclaration.EventKeyword;
            }
            else
            {
                token = attributeList.CloseBracketToken.GetNextToken();
            }

            Analyze(context, attributeList, token);
        }

        private static void AnalyzeBaseMethodDeclaration(SyntaxNodeAnalysisContext context)
        {
            var baseMethodDeclaration = (BaseMethodDeclarationSyntax)context.Node;

            AttributeListSyntax attributeList = baseMethodDeclaration.AttributeLists.LastOrDefault();

            if (attributeList == null)
                return;

            SyntaxTokenList modifiers = baseMethodDeclaration.Modifiers;

            SyntaxToken token;

            if (modifiers.Any())
            {
                token = modifiers[0];
            }
            else if (baseMethodDeclaration is ConstructorDeclarationSyntax constructorDeclaration)
            {
                token = constructorDeclaration.Identifier;
            }
            else if (baseMethodDeclaration is ConversionOperatorDeclarationSyntax conversionOperatorDeclaration)
            {
                token = conversionOperatorDeclaration.ImplicitOrExplicitKeyword;
            }
            else if (baseMethodDeclaration is DestructorDeclarationSyntax destructorDeclaration)
            {
                token = destructorDeclaration.TildeToken;
            }
            else
            {
                token = attributeList.CloseBracketToken.GetNextToken();
            }

            Analyze(context, attributeList, token);
        }

        private static void AnalyzeBasePropertyDeclaration(SyntaxNodeAnalysisContext context)
        {
            var basePropertyDeclaration = (BasePropertyDeclarationSyntax)context.Node;

            AttributeListSyntax attributeList = basePropertyDeclaration.AttributeLists.LastOrDefault();

            if (attributeList == null)
                return;

            SyntaxTokenList modifiers = basePropertyDeclaration.Modifiers;

            SyntaxToken token = (modifiers.Any())
                ? modifiers[0]
                : attributeList.CloseBracketToken.GetNextToken();

            Analyze(context, attributeList, token);
        }

        private static void AnalyzeBaseTypeDeclaration(SyntaxNodeAnalysisContext context)
        {
            var baseTypeDeclaration = (BaseTypeDeclarationSyntax)context.Node;

            AttributeListSyntax attributeList = baseTypeDeclaration.AttributeLists.LastOrDefault();

            if (attributeList == null)
                return;

            SyntaxTokenList modifiers = baseTypeDeclaration.Modifiers;

            SyntaxToken token;

            if (modifiers.Any())
            {
                token = modifiers[0];
            }
            else if (baseTypeDeclaration is ClassDeclarationSyntax classDeclaration)
            {
                token = classDeclaration.Keyword;
            }
            else if (baseTypeDeclaration is InterfaceDeclarationSyntax interfaceDeclaration)
            {
                token = interfaceDeclaration.Keyword;
            }
            else if (baseTypeDeclaration is StructDeclarationSyntax structDeclaration)
            {
                token = structDeclaration.Keyword;
            }
            else if (baseTypeDeclaration is EnumDeclarationSyntax enumDeclaration)
            {
                token = enumDeclaration.EnumKeyword;
            }
            else
            {
                throw new InvalidOperationException("");
            }

            Analyze(context, attributeList, token);
        }

        private static void AnalyzeAccessorDeclaration(SyntaxNodeAnalysisContext context)
        {
            var accessorDeclaration = (AccessorDeclarationSyntax)context.Node;

            AttributeListSyntax attributeList = accessorDeclaration.AttributeLists.LastOrDefault();

            if (attributeList == null)
                return;

            SyntaxTokenList modifiers = accessorDeclaration.Modifiers;

            SyntaxToken token = (modifiers.Any())
                ? modifiers[0]
                : accessorDeclaration.Keyword;

            Analyze(context, attributeList, token);
        }

        private static void Analyze(SyntaxNodeAnalysisContext context, AttributeListSyntax attributeList, SyntaxToken token)
        {
            if (attributeList.SyntaxTree.IsSingleLineSpan(TextSpan.FromBounds(attributeList.Span.End, token.SpanStart)))
            {
                context.ReportDiagnostic(
                    DiagnosticDescriptors.AddNewLineAfterAttributeList,
                    Location.Create(token.SyntaxTree, token.Span.WithLength(0)));
            }
        }
    }
}
