// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.Linq;
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

            context.RegisterCompilationStartAction(startContext =>
            {
                INamedTypeSymbol debuggerDisplayAttribute = startContext.Compilation.GetTypeByMetadataName(MetadataNames.System_Diagnostics_DebuggerDisplayAttribute);

                startContext.RegisterSyntaxNodeAction(nodeContext => AnalyzeClassDeclaration(nodeContext, debuggerDisplayAttribute), SyntaxKind.ClassDeclaration);
                startContext.RegisterSyntaxNodeAction(nodeContext => AnalyzeStructDeclaration(nodeContext, debuggerDisplayAttribute), SyntaxKind.StructDeclaration);
            });
        }

        public static void AnalyzeClassDeclaration(SyntaxNodeAnalysisContext context, INamedTypeSymbol debuggerDisplayAttribute)
        {
            AnalyzeTypeDeclaration(context, (TypeDeclarationSyntax)context.Node, debuggerDisplayAttribute);
        }

        public static void AnalyzeStructDeclaration(SyntaxNodeAnalysisContext context, INamedTypeSymbol debuggerDisplayAttribute)
        {
            AnalyzeTypeDeclaration(context, (TypeDeclarationSyntax)context.Node, debuggerDisplayAttribute);
        }

        private static void AnalyzeTypeDeclaration(SyntaxNodeAnalysisContext context, TypeDeclarationSyntax typeDeclaration, INamedTypeSymbol debuggerDisplayAttribute)
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

                                walker.AddDelegate(declaration.Identifier.ValueText, declaration);
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

            if (debuggerDisplayAttribute != null
                && walker.Nodes.Any(f => f.CanBeInDebuggerDisplayAttribute)
                && ShouldAnalyzeDebuggerDisplayAttribute())
            {
                string value = context.SemanticModel
                    .GetDeclaredSymbol(typeDeclaration, context.CancellationToken)
                    .GetAttribute(debuggerDisplayAttribute)?
                    .ConstructorArguments
                    .SingleOrDefault(shouldThrow: false)
                    .Value?
                    .ToString();

                if (value != null)
                    RemoveMethodsAndPropertiesThatAreInDebuggerDisplayAttributeValue(walker.Nodes, value);

                if (walker.Nodes.Count == 0)
                    return;
            }

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

            bool ShouldAnalyzeDebuggerDisplayAttribute()
            {
                if (typeDeclaration.Modifiers.Contains(SyntaxKind.PartialKeyword))
                    return true;

                foreach (AttributeListSyntax attributeList in typeDeclaration.AttributeLists)
                {
                    foreach (AttributeSyntax attribute in attributeList.Attributes)
                    {
                        if (attribute.ArgumentList?.Arguments.Count(f => f.NameEquals == null) == 1)
                            return true;
                    }
                }

                return false;
            }
        }

        private static bool IsMainMethod(MethodDeclarationSyntax methodDeclaration, SyntaxTokenList modifiers, string methodName)
        {
            return string.Equals(methodName, "Main", StringComparison.Ordinal)
                && modifiers.Contains(SyntaxKind.StaticKeyword)
                && methodDeclaration.TypeParameterList == null
                && methodDeclaration.ParameterList?.Parameters.Count <= 1;
        }

        private static void RemoveMethodsAndPropertiesThatAreInDebuggerDisplayAttributeValue(
            Collection<NodeSymbolInfo> nodes,
            string value)
        {
            int length = value.Length;

            if (length == 0)
                return;

            for (int i = 0; i < length; i++)
            {
                switch (value[i])
                {
                    case '{':
                        {
                            i++;

                            int startIndex = i;

                            while (i < length)
                            {
                                char ch = value[i];

                                if (ch == '}'
                                    || ch == ','
                                    || ch == '(')
                                {
                                    int nameLength = i - startIndex;

                                    if (nameLength > 0)
                                    {
                                        for (int j = nodes.Count - 1; j >= 0; j--)
                                        {
                                            NodeSymbolInfo nodeSymbolInfo = nodes[j];

                                            if (nodeSymbolInfo.CanBeInDebuggerDisplayAttribute
                                                && string.CompareOrdinal(nodeSymbolInfo.Name, 0, value, startIndex, nameLength) == 0)
                                            {
                                                nodes.RemoveAt(j);

                                                if (nodes.Count == 0)
                                                    return;
                                            }
                                        }
                                    }

                                    if (ch != '}')
                                    {
                                        i++;

                                        while (i < length
                                            && value[i] != '}')
                                        {
                                            i++;
                                        }
                                    }

                                    break;
                                }

                                i++;
                            }

                            break;
                        }
                    case '}':
                        {
                            return;
                        }
                    case '\\':
                        {
                            i++;
                            break;
                        }
                }
            }
        }

        private static void ReportDiagnostic(SyntaxNodeAnalysisContext context, SyntaxNode node, string declarationName)
        {
            context.ReportDiagnostic(DiagnosticDescriptors.RemoveUnusedMemberDeclaration, CSharpUtility.GetIdentifier(node), declarationName);
        }
    }
}
