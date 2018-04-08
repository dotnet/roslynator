// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Immutable;
using System.Diagnostics;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Roslynator.CSharp.Analysis
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class UnnecessaryUnsafeContextAnalyzer : BaseDiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        {
            get { return ImmutableArray.Create(DiagnosticDescriptors.UnnecessaryUnsafeContext); }
        }

        public override void Initialize(AnalysisContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            base.Initialize(context);
            context.EnableConcurrentExecution();

            context.RegisterSyntaxNodeAction(AnalyzeUnsafeStatement, SyntaxKind.UnsafeStatement);
            context.RegisterSyntaxNodeAction(AnalyzeTypeDeclaration, SyntaxKind.ClassDeclaration);
            context.RegisterSyntaxNodeAction(AnalyzeTypeDeclaration, SyntaxKind.StructDeclaration);
            context.RegisterSyntaxNodeAction(AnalyzeTypeDeclaration, SyntaxKind.InterfaceDeclaration);
            context.RegisterSyntaxNodeAction(AnalyzeDelegateDeclaration, SyntaxKind.DelegateDeclaration);
            context.RegisterSyntaxNodeAction(AnalyzeLocalFunctionStatement, SyntaxKind.LocalFunctionStatement);
            context.RegisterSyntaxNodeAction(AnalyzeMethodDeclaration, SyntaxKind.MethodDeclaration);
            context.RegisterSyntaxNodeAction(AnalyzeOperatorDeclaration, SyntaxKind.OperatorDeclaration);
            context.RegisterSyntaxNodeAction(AnalyzeConversionOperatorDeclaration, SyntaxKind.ConversionOperatorDeclaration);
            context.RegisterSyntaxNodeAction(AnalyzeConstructorDeclaration, SyntaxKind.ConstructorDeclaration);
            context.RegisterSyntaxNodeAction(AnalyzeDestructorDeclaration, SyntaxKind.DestructorDeclaration);
            context.RegisterSyntaxNodeAction(AnalyzePropertyDeclaration, SyntaxKind.PropertyDeclaration);
            context.RegisterSyntaxNodeAction(AnalyzeIndexerDeclaration, SyntaxKind.IndexerDeclaration);
            context.RegisterSyntaxNodeAction(AnalyzeEventDeclaration, SyntaxKind.EventDeclaration);
            context.RegisterSyntaxNodeAction(AnalyzeEventFieldDeclaration, SyntaxKind.EventFieldDeclaration);
            context.RegisterSyntaxNodeAction(AnalyzeFieldDeclaration, SyntaxKind.FieldDeclaration);
        }

        public static void AnalyzeUnsafeStatement(SyntaxNodeAnalysisContext context)
        {
            var unsafeStatement = (UnsafeStatementSyntax)context.Node;

            if (!unsafeStatement.Block.Statements.Any())
                return;

            if (!ParentDeclarationsContainsUnsafeModifier(unsafeStatement))
                return;

            context.ReportDiagnostic(DiagnosticDescriptors.UnnecessaryUnsafeContext, unsafeStatement.UnsafeKeyword);
        }

        public static void AnalyzeLocalFunctionStatement(SyntaxNodeAnalysisContext context)
        {
            var localFunctionStatement = (LocalFunctionStatementSyntax)context.Node;

            SyntaxTokenList modifiers = localFunctionStatement.Modifiers;

            int index = modifiers.IndexOf(SyntaxKind.UnsafeKeyword);

            if (index == -1)
                return;

            SyntaxNode parent = localFunctionStatement.Parent;

            Debug.Assert(parent.IsKind(SyntaxKind.Block), parent.Kind().ToString());

            if (!(parent is BlockSyntax))
                return;

            parent = parent.Parent;

            if (!ParentDeclarationsContainsUnsafeModifier(parent))
                return;

            context.ReportDiagnostic(DiagnosticDescriptors.UnnecessaryUnsafeContext, modifiers[index]);
        }

        public static void AnalyzeTypeDeclaration(SyntaxNodeAnalysisContext context)
        {
            var typeDeclaration = (TypeDeclarationSyntax)context.Node;

            AnalyzeMemberDeclaration(context, typeDeclaration, typeDeclaration.Modifiers);
        }

        public static void AnalyzeDelegateDeclaration(SyntaxNodeAnalysisContext context)
        {
            var delegateDeclaration = (DelegateDeclarationSyntax)context.Node;

            AnalyzeMemberDeclaration(context, delegateDeclaration, delegateDeclaration.Modifiers);
        }

        public static void AnalyzeMethodDeclaration(SyntaxNodeAnalysisContext context)
        {
            var methodDeclaration = (MethodDeclarationSyntax)context.Node;

            AnalyzeMemberDeclaration(context, methodDeclaration, methodDeclaration.Modifiers);
        }

        public static void AnalyzeOperatorDeclaration(SyntaxNodeAnalysisContext context)
        {
            var operatorDeclaration = (OperatorDeclarationSyntax)context.Node;

            AnalyzeMemberDeclaration(context, operatorDeclaration, operatorDeclaration.Modifiers);
        }

        public static void AnalyzeConversionOperatorDeclaration(SyntaxNodeAnalysisContext context)
        {
            var conversionOperatorDeclaration = (ConversionOperatorDeclarationSyntax)context.Node;

            AnalyzeMemberDeclaration(context, conversionOperatorDeclaration, conversionOperatorDeclaration.Modifiers);
        }

        public static void AnalyzeConstructorDeclaration(SyntaxNodeAnalysisContext context)
        {
            var constructorDeclaration = (ConstructorDeclarationSyntax)context.Node;

            AnalyzeMemberDeclaration(context, constructorDeclaration, constructorDeclaration.Modifiers);
        }

        public static void AnalyzeDestructorDeclaration(SyntaxNodeAnalysisContext context)
        {
            var destructorDeclaration = (DestructorDeclarationSyntax)context.Node;

            AnalyzeMemberDeclaration(context, destructorDeclaration, destructorDeclaration.Modifiers);
        }

        public static void AnalyzeEventDeclaration(SyntaxNodeAnalysisContext context)
        {
            var eventDeclaration = (EventDeclarationSyntax)context.Node;

            AnalyzeMemberDeclaration(context, eventDeclaration, eventDeclaration.Modifiers);
        }

        public static void AnalyzeEventFieldDeclaration(SyntaxNodeAnalysisContext context)
        {
            var eventFieldDeclaration = (EventFieldDeclarationSyntax)context.Node;

            AnalyzeMemberDeclaration(context, eventFieldDeclaration, eventFieldDeclaration.Modifiers);
        }

        public static void AnalyzeFieldDeclaration(SyntaxNodeAnalysisContext context)
        {
            var fieldDeclaration = (FieldDeclarationSyntax)context.Node;

            AnalyzeMemberDeclaration(context, fieldDeclaration, fieldDeclaration.Modifiers);
        }

        public static void AnalyzePropertyDeclaration(SyntaxNodeAnalysisContext context)
        {
            var propertyDeclaration = (PropertyDeclarationSyntax)context.Node;

            AnalyzeMemberDeclaration(context, propertyDeclaration, propertyDeclaration.Modifiers);
        }

        public static void AnalyzeIndexerDeclaration(SyntaxNodeAnalysisContext context)
        {
            var indexerDeclaration = (IndexerDeclarationSyntax)context.Node;

            AnalyzeMemberDeclaration(context, indexerDeclaration, indexerDeclaration.Modifiers);
        }

        private static void AnalyzeMemberDeclaration(
            SyntaxNodeAnalysisContext context,
            MemberDeclarationSyntax memberDeclaration,
            SyntaxTokenList modifiers)
        {
            int index = modifiers.IndexOf(SyntaxKind.UnsafeKeyword);

            if (index == -1)
                return;

            if (!ParentTypeDeclarationsContainsUnsafeModifier(memberDeclaration))
                return;

            context.ReportDiagnostic(DiagnosticDescriptors.UnnecessaryUnsafeContext, modifiers[index]);
        }

        private static bool ParentDeclarationsContainsUnsafeModifier(UnsafeStatementSyntax unsafeStatement)
        {
            SyntaxNode parent = unsafeStatement.Parent;

            while (parent != null)
            {
                SyntaxKind kind = parent.Kind();

                if (kind == SyntaxKind.UnsafeStatement)
                {
                    return true;
                }
                else if (kind == SyntaxKind.LocalFunctionStatement)
                {
                    break;
                }

                if (parent is AccessorDeclarationSyntax)
                {
                    parent = parent.Parent;

                    if (parent is AccessorListSyntax)
                        parent = parent.Parent;

                    break;
                }

                if (parent is MemberDeclarationSyntax)
                    break;

                parent = parent.Parent;
            }

            return ParentDeclarationsContainsUnsafeModifier(parent);
        }

        private static bool ParentDeclarationsContainsUnsafeModifier(SyntaxNode node)
        {
            while (node.IsKind(SyntaxKind.LocalFunctionStatement))
            {
                var localFunction = (LocalFunctionStatementSyntax)node;

                if (localFunction.Modifiers.Contains(SyntaxKind.UnsafeKeyword))
                    return true;

                node = node.Parent;

                Debug.Assert(node.IsKind(SyntaxKind.Block), node.Kind().ToString());

                if (node.Kind() != SyntaxKind.Block)
                    break;

                node = node.Parent;
            }

            Debug.Assert(node is MemberDeclarationSyntax, node.Kind().ToString());

            if (node is MemberDeclarationSyntax memberDeclaration)
            {
                if (SyntaxInfo.ModifierListInfo(memberDeclaration).IsUnsafe)
                    return true;

                return ParentTypeDeclarationsContainsUnsafeModifier(memberDeclaration);
            }

            return false;
        }

        private static bool ParentTypeDeclarationsContainsUnsafeModifier(MemberDeclarationSyntax memberDeclaration)
        {
            SyntaxNode parent = memberDeclaration.Parent;

            while (parent.IsKind(SyntaxKind.ClassDeclaration, SyntaxKind.StructDeclaration, SyntaxKind.InterfaceDeclaration))
            {
                if (((TypeDeclarationSyntax)parent).Modifiers.Contains(SyntaxKind.UnsafeKeyword))
                    return true;

                parent = parent.Parent;
            }

            return false;
        }
    }
}
