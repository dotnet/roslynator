// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Roslynator.CSharp.Syntax;

namespace Roslynator.CSharp.Analysis
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class AddEmptyLineBetweenDeclarationsAnalyzer : BaseDiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        {
            get { return ImmutableArray.Create(DiagnosticDescriptors.AddEmptyLineBetweenDeclarations); }
        }

        public override void Initialize(AnalysisContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            base.Initialize(context);
            context.EnableConcurrentExecution();

            context.RegisterSyntaxNodeAction(AnalyzeConstructorDeclaration, SyntaxKind.ConstructorDeclaration);
            context.RegisterSyntaxNodeAction(AnalyzeDestructorDeclaration, SyntaxKind.DestructorDeclaration);
            context.RegisterSyntaxNodeAction(AnalyzeEventDeclaration, SyntaxKind.EventDeclaration);
            context.RegisterSyntaxNodeAction(AnalyzePropertyDeclaration, SyntaxKind.PropertyDeclaration);
            context.RegisterSyntaxNodeAction(AnalyzeIndexerDeclaration, SyntaxKind.IndexerDeclaration);
            context.RegisterSyntaxNodeAction(AnalyzeMethodDeclaration, SyntaxKind.MethodDeclaration);
            context.RegisterSyntaxNodeAction(AnalyzeConversionOperatorDeclaration, SyntaxKind.ConversionOperatorDeclaration);
            context.RegisterSyntaxNodeAction(AnalyzeOperatorDeclaration, SyntaxKind.OperatorDeclaration);
            context.RegisterSyntaxNodeAction(AnalyzeEnumDeclaration, SyntaxKind.EnumDeclaration);
            context.RegisterSyntaxNodeAction(AnalyzeInterfaceDeclaration, SyntaxKind.InterfaceDeclaration);
            context.RegisterSyntaxNodeAction(AnalyzeStructDeclaration, SyntaxKind.StructDeclaration);
            context.RegisterSyntaxNodeAction(AnalyzeClassDeclaration, SyntaxKind.ClassDeclaration);
            context.RegisterSyntaxNodeAction(AnalyzeNamespaceDeclaration, SyntaxKind.NamespaceDeclaration);
        }

        public static void AnalyzeConstructorDeclaration(SyntaxNodeAnalysisContext context)
        {
            var constructorDeclaration = (ConstructorDeclarationSyntax)context.Node;

            BlockSyntax body = constructorDeclaration.Body;

            if (body == null)
                return;

            Analyze(context, constructorDeclaration, body.OpenBraceToken, body.CloseBraceToken);
        }

        public static void AnalyzeDestructorDeclaration(SyntaxNodeAnalysisContext context)
        {
            var destructorDeclaration = (DestructorDeclarationSyntax)context.Node;

            BlockSyntax body = destructorDeclaration.Body;

            if (body == null)
                return;

            Analyze(context, destructorDeclaration, body.OpenBraceToken, body.CloseBraceToken);
        }

        public static void AnalyzeEventDeclaration(SyntaxNodeAnalysisContext context)
        {
            var eventDeclaration = (EventDeclarationSyntax)context.Node;

            AccessorListSyntax accessorList = eventDeclaration.AccessorList;

            if (accessorList == null)
                return;

            Analyze(context, eventDeclaration, accessorList.OpenBraceToken, accessorList.CloseBraceToken);
        }

        public static void AnalyzePropertyDeclaration(SyntaxNodeAnalysisContext context)
        {
            var propertyDeclaration = (PropertyDeclarationSyntax)context.Node;

            AccessorListSyntax accessorList = propertyDeclaration.AccessorList;

            if (accessorList == null)
                return;

            Analyze(context, propertyDeclaration, accessorList.OpenBraceToken, accessorList.CloseBraceToken);
        }

        public static void AnalyzeIndexerDeclaration(SyntaxNodeAnalysisContext context)
        {
            var indexerDeclaration = (IndexerDeclarationSyntax)context.Node;

            AccessorListSyntax accessorList = indexerDeclaration.AccessorList;

            if (accessorList == null)
                return;

            Analyze(context, indexerDeclaration, accessorList.OpenBraceToken, accessorList.CloseBraceToken);
        }

        public static void AnalyzeMethodDeclaration(SyntaxNodeAnalysisContext context)
        {
            var methodDeclaration = (MethodDeclarationSyntax)context.Node;

            BlockSyntax body = methodDeclaration.Body;

            if (body == null)
                return;

            Analyze(context, methodDeclaration, body.OpenBraceToken, body.CloseBraceToken);
        }

        public static void AnalyzeConversionOperatorDeclaration(SyntaxNodeAnalysisContext context)
        {
            var conversionOperatorDeclaration = (ConversionOperatorDeclarationSyntax)context.Node;

            BlockSyntax body = conversionOperatorDeclaration.Body;

            if (body == null)
                return;

            Analyze(context, conversionOperatorDeclaration, body.OpenBraceToken, body.CloseBraceToken);
        }

        public static void AnalyzeOperatorDeclaration(SyntaxNodeAnalysisContext context)
        {
            var operatorDeclaration = (OperatorDeclarationSyntax)context.Node;

            BlockSyntax body = operatorDeclaration.Body;

            if (body == null)
                return;

            Analyze(context, operatorDeclaration, body.OpenBraceToken, body.CloseBraceToken);
        }

        public static void AnalyzeEnumDeclaration(SyntaxNodeAnalysisContext context)
        {
            var enumDeclaration = (EnumDeclarationSyntax)context.Node;

            Analyze(context, enumDeclaration, enumDeclaration.OpenBraceToken, enumDeclaration.CloseBraceToken);
        }

        public static void AnalyzeInterfaceDeclaration(SyntaxNodeAnalysisContext context)
        {
            var interfaceDeclaration = (InterfaceDeclarationSyntax)context.Node;

            Analyze(context, interfaceDeclaration, interfaceDeclaration.OpenBraceToken, interfaceDeclaration.CloseBraceToken);
        }

        public static void AnalyzeStructDeclaration(SyntaxNodeAnalysisContext context)
        {
            var structDeclaration = (StructDeclarationSyntax)context.Node;

            Analyze(context, structDeclaration, structDeclaration.OpenBraceToken, structDeclaration.CloseBraceToken);
        }

        public static void AnalyzeClassDeclaration(SyntaxNodeAnalysisContext context)
        {
            var classDeclaration = (ClassDeclarationSyntax)context.Node;

            Analyze(context, classDeclaration, classDeclaration.OpenBraceToken, classDeclaration.CloseBraceToken);
        }

        public static void AnalyzeNamespaceDeclaration(SyntaxNodeAnalysisContext context)
        {
            var namespaceDeclaration = (NamespaceDeclarationSyntax)context.Node;

            Analyze(context, namespaceDeclaration, namespaceDeclaration.OpenBraceToken, namespaceDeclaration.CloseBraceToken);
        }

        private static void Analyze(SyntaxNodeAnalysisContext context, MemberDeclarationSyntax declaration, SyntaxToken openToken, SyntaxToken closeToken)
        {
            if (declaration.IsParentKind(SyntaxKind.CompilationUnit))
                return;

            if (openToken.IsMissing)
                return;

            if (closeToken.IsMissing)
                return;

            int closeTokenLine = closeToken.GetSpanEndLine();

            if (openToken.GetSpanEndLine() == closeTokenLine)
                return;

            MemberDeclarationSyntax nextDeclaration = GetNextDeclaration(declaration);

            if (nextDeclaration == null)
                return;

            int diff = nextDeclaration.GetSpanStartLine() - closeTokenLine;

            if (diff >= 2)
                return;

            SyntaxTrivia trivia = declaration.GetTrailingTrivia().LastOrDefault();

            if (trivia.IsEndOfLineTrivia())
            {
                context.ReportDiagnostic(DiagnosticDescriptors.AddEmptyLineBetweenDeclarations, trivia);
            }
            else
            {
                context.ReportDiagnostic(DiagnosticDescriptors.AddEmptyLineBetweenDeclarations, closeToken);
            }
        }

        private static MemberDeclarationSyntax GetNextDeclaration(MemberDeclarationSyntax declaration)
        {
            MemberDeclarationListInfo info = SyntaxInfo.MemberDeclarationListInfo(declaration.Parent);

            if (!info.Success)
                return null;

            SyntaxList<MemberDeclarationSyntax> members = info.Members;

            if (members.Count <= 1)
                return null;

            int index = members.IndexOf(declaration);

            if (index == members.Count - 1)
                return null;

            return members[index + 1];
        }
    }
}
