// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Roslynator.CSharp.Analysis.UnusedMember
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class UnusedMemberAnalyzer : BaseDiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        {
            get { return ImmutableArray.Create(DiagnosticDescriptors.RemoveUnusedMemberDeclaration); }
        }

        public override void Initialize(AnalysisContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            base.Initialize(context);
            context.EnableConcurrentExecution();

            context.RegisterSyntaxNodeAction(AnalyzeClassDeclaration, SyntaxKind.ClassDeclaration);
            context.RegisterSyntaxNodeAction(AnalyzeStructDeclaration, SyntaxKind.StructDeclaration);
        }

        public static void AnalyzeClassDeclaration(SyntaxNodeAnalysisContext context)
        {
            AnalyzeTypeDeclaration(context, (TypeDeclarationSyntax)context.Node);
        }

        public static void AnalyzeStructDeclaration(SyntaxNodeAnalysisContext context)
        {
            AnalyzeTypeDeclaration(context, (TypeDeclarationSyntax)context.Node);
        }

        private static void AnalyzeTypeDeclaration(SyntaxNodeAnalysisContext context, TypeDeclarationSyntax typeDeclaration)
        {
            if (typeDeclaration.Modifiers.Contains(SyntaxKind.PartialKeyword))
                return;

            SyntaxList<MemberDeclarationSyntax> members = typeDeclaration.Members;

            UnusedMemberWalker walker = null;

            foreach (MemberDeclarationSyntax member in members)
            {
                if (member.ContainsDiagnostics)
                    continue;

                if (member.ContainsDirectives)
                    continue;

                switch (member.Kind())
                {
                    case SyntaxKind.DelegateDeclaration:
                        {
                            var declaration = (DelegateDeclarationSyntax)member;

                            if (SyntaxAccessibility.GetAccessibility(declaration) == Accessibility.Private)
                            {
                                if (walker == null)
                                    walker = UnusedMemberWalkerCache.GetInstance(context.SemanticModel, context.CancellationToken);

                                walker.AddNode(declaration.Identifier.ValueText, declaration);
                            }

                            break;
                        }
                    case SyntaxKind.EventDeclaration:
                        {
                            var declaration = (EventDeclarationSyntax)member;

                            if (declaration.ExplicitInterfaceSpecifier == null
                                && SyntaxAccessibility.GetAccessibility(declaration) == Accessibility.Private)
                            {
                                if (walker == null)
                                    walker = UnusedMemberWalkerCache.GetInstance(context.SemanticModel, context.CancellationToken);

                                walker.AddNode(declaration.Identifier.ValueText, declaration);
                            }

                            break;
                        }
                    case SyntaxKind.EventFieldDeclaration:
                        {
                            var declaration = (EventFieldDeclarationSyntax)member;

                            if (SyntaxAccessibility.GetAccessibility(declaration) == Accessibility.Private)
                            {
                                if (walker == null)
                                    walker = UnusedMemberWalkerCache.GetInstance(context.SemanticModel, context.CancellationToken);

                                walker.AddNodes(declaration.Declaration);
                            }

                            break;
                        }
                    case SyntaxKind.FieldDeclaration:
                        {
                            var declaration = (FieldDeclarationSyntax)member;
                            SyntaxTokenList modifiers = declaration.Modifiers;

                            if (SyntaxAccessibility.GetAccessibility(declaration) == Accessibility.Private)
                            {
                                if (walker == null)
                                    walker = UnusedMemberWalkerCache.GetInstance(context.SemanticModel, context.CancellationToken);

                                walker.AddNodes(declaration.Declaration, isConst: modifiers.Contains(SyntaxKind.ConstKeyword));
                            }

                            break;
                        }
                    case SyntaxKind.MethodDeclaration:
                        {
                            var declaration = (MethodDeclarationSyntax)member;

                            SyntaxTokenList modifiers = declaration.Modifiers;

                            if (declaration.ExplicitInterfaceSpecifier == null
                                && !declaration.AttributeLists.Any()
                                && SyntaxAccessibility.GetAccessibility(declaration) == Accessibility.Private)
                            {
                                string methodName = declaration.Identifier.ValueText;

                                if (!IsMainMethod(declaration, modifiers, methodName))
                                {
                                    if (walker == null)
                                        walker = UnusedMemberWalkerCache.GetInstance(context.SemanticModel, context.CancellationToken);

                                    walker.AddNode(methodName, declaration);
                                }
                            }

                            break;
                        }
                    case SyntaxKind.PropertyDeclaration:
                        {
                            var declaration = (PropertyDeclarationSyntax)member;

                            if (declaration.ExplicitInterfaceSpecifier == null
                                && SyntaxAccessibility.GetAccessibility(declaration) == Accessibility.Private)
                            {
                                if (walker == null)
                                    walker = UnusedMemberWalkerCache.GetInstance(context.SemanticModel, context.CancellationToken);

                                walker.AddNode(declaration.Identifier.ValueText, declaration);
                            }

                            break;
                        }
                }
            }

            if (walker == null)
                return;

            walker.Visit(typeDeclaration);

            foreach (NodeSymbolInfo info in UnusedMemberWalkerCache.GetNodesAndFree(walker))
            {
                SyntaxNode node = info.Node;

                if (node is VariableDeclaratorSyntax variableDeclarator)
                {
                    var variableDeclaration = (VariableDeclarationSyntax)variableDeclarator.Parent;

                    if (variableDeclaration.Variables.Count == 1)
                    {
                        ReportDiagnostic(context, variableDeclaration.Parent, CSharpFacts.GetTitle(variableDeclaration.Parent));
                    }
                    else
                    {
                        ReportDiagnostic(context, variableDeclarator, CSharpFacts.GetTitle(variableDeclaration.Parent));
                    }
                }
                else
                {
                    ReportDiagnostic(context, node, CSharpFacts.GetTitle(node));
                }
            }
        }

        private static bool IsMainMethod(MethodDeclarationSyntax methodDeclaration, SyntaxTokenList modifiers, string methodName)
        {
            return string.Equals(methodName, "Main", StringComparison.Ordinal)
                && modifiers.Contains(SyntaxKind.StaticKeyword)
                && methodDeclaration.TypeParameterList == null
                && methodDeclaration.ParameterList?.Parameters.Count <= 1;
        }

        private static void ReportDiagnostic(SyntaxNodeAnalysisContext context, SyntaxNode node, string declarationName)
        {
            context.ReportDiagnostic(DiagnosticDescriptors.RemoveUnusedMemberDeclaration, CSharpUtility.GetIdentifier(node), declarationName);
        }
    }
}
