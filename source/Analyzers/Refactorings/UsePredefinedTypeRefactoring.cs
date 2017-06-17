// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Roslynator.CSharp.Refactorings
{
    internal static class UsePredefinedTypeRefactoring
    {
        public static void AnalyzeIdentifierName(SyntaxNodeAnalysisContext context)
        {
            var identifierName = (IdentifierNameSyntax)context.Node;

            if (!identifierName.IsVar
                && !identifierName.IsParentKind(
                    SyntaxKind.SimpleMemberAccessExpression,
                    SyntaxKind.QualifiedName,
                    SyntaxKind.UsingDirective)
                && !identifierName.IsPartOfDocumentationComment()
                && !IsArgumentExpressionOfNameOfExpression(context, identifierName))
            {
                var typeSymbol = context.SemanticModel.GetSymbol(identifierName, context.CancellationToken) as ITypeSymbol;

                if (typeSymbol?.SupportsPredefinedType() == true)
                {
                    IAliasSymbol aliasSymbol = context.SemanticModel.GetAliasInfo(identifierName, context.CancellationToken);

                    if (aliasSymbol == null)
                        ReportDiagnostic(context, identifierName);
                }
            }
        }

        public static void AnalyzeXmlCrefAttribute(SyntaxNodeAnalysisContext context)
        {
            var xmlCrefAttribute = (XmlCrefAttributeSyntax)context.Node;

            CrefSyntax cref = xmlCrefAttribute.Cref;

            switch (cref?.Kind())
            {
                case SyntaxKind.NameMemberCref:
                    {
                        var nameMemberCref = (NameMemberCrefSyntax)cref;

                        TypeSyntax name = nameMemberCref.Name;

                        if (name?.IsKind(SyntaxKind.PredefinedType) == false
                            && IsFixable(context, name))
                        {
                            ReportDiagnostic(context, cref);
                        }

                        break;
                    }
                case SyntaxKind.QualifiedCref:
                    {
                        var qualifiedCref = (QualifiedCrefSyntax)cref;

                        MemberCrefSyntax memberCref = qualifiedCref.Member;

                        if (memberCref?.IsKind(SyntaxKind.NameMemberCref) == true)
                        {
                            var nameMemberCref = (NameMemberCrefSyntax)memberCref;

                            TypeSyntax name = nameMemberCref.Name;

                            if (name != null
                                && IsFixable(context, name))
                            {
                                ReportDiagnostic(context, cref);
                            }
                        }

                        break;
                    }
            }
        }

        private static bool IsFixable(SyntaxNodeAnalysisContext context, TypeSyntax name)
        {
            var typeSymbol = context.SemanticModel.GetSymbol(name, context.CancellationToken) as ITypeSymbol;

            if (typeSymbol?.SupportsPredefinedType() == true)
            {
                IAliasSymbol aliasSymbol = context.SemanticModel.GetAliasInfo(name, context.CancellationToken);

                return aliasSymbol == null;
            }

            return false;
        }

        public static void Analyze(SyntaxNodeAnalysisContext context, QualifiedNameSyntax qualifiedName)
        {
            if (!qualifiedName.IsParentKind(SyntaxKind.UsingDirective)
                && !IsArgumentExpressionOfNameOfExpression(context, qualifiedName))
            {
                var typeSymbol = context.SemanticModel.GetSymbol(qualifiedName, context.CancellationToken) as ITypeSymbol;

                if (typeSymbol?.SupportsPredefinedType() == true)
                {
                    ReportDiagnostic(context, qualifiedName);
                }
            }
        }

        public static void Analyze(SyntaxNodeAnalysisContext context, MemberAccessExpressionSyntax memberAccess)
        {
            if (!memberAccess.IsParentKind(SyntaxKind.SimpleMemberAccessExpression))
            {
                ExpressionSyntax expression = memberAccess.Expression;

                if (expression?.IsKind(
                    SyntaxKind.SimpleMemberAccessExpression,
                    SyntaxKind.IdentifierName) == true)
                {
                    var typeSymbol = context.SemanticModel.GetSymbol(expression, context.CancellationToken) as ITypeSymbol;

                    if (typeSymbol?.SupportsPredefinedType() == true)
                    {
                        IAliasSymbol aliasSymbol = context.SemanticModel.GetAliasInfo(expression, context.CancellationToken);

                        if (aliasSymbol == null)
                            ReportDiagnostic(context, expression);
                    }
                }
            }
        }

        private static bool IsArgumentExpressionOfNameOfExpression(SyntaxNodeAnalysisContext context, SyntaxNode node)
        {
            SyntaxNode parent = node.Parent;

            if (parent?.IsKind(SyntaxKind.Argument) == true)
            {
                parent = parent.Parent;

                if (parent?.IsKind(SyntaxKind.ArgumentList) == true)
                {
                    parent = parent.Parent;

                    return parent != null
                        && CSharpUtility.IsNameOfExpression(parent, context.SemanticModel, context.CancellationToken);
                }
            }

            return false;
        }

        private static void ReportDiagnostic(SyntaxNodeAnalysisContext context, SyntaxNode node)
        {
            context.ReportDiagnostic(DiagnosticDescriptors.UsePredefinedType, node);
        }

        public static Task<Document> RefactorAsync(
            Document document,
            SyntaxNode node,
            ITypeSymbol typeSymbol,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            SyntaxNode newNode = GetNewNode(node, typeSymbol.ToTypeSyntax())
                .WithTriviaFrom(node)
                .WithFormatterAnnotation();

            return document.ReplaceNodeAsync(node, newNode, cancellationToken);
        }

        private static SyntaxNode GetNewNode(SyntaxNode node, TypeSyntax type)
        {
            switch (node.Kind())
            {
                case SyntaxKind.NameMemberCref:
                case SyntaxKind.QualifiedCref:
                    return SyntaxFactory.NameMemberCref(type);
                default:
                    return type;
            }
        }
    }
}
