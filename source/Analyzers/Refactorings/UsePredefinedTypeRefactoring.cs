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

            if (identifierName.IsVar)
                return;

            if (identifierName.IsParentKind(
                SyntaxKind.SimpleMemberAccessExpression,
                SyntaxKind.QualifiedName,
                SyntaxKind.UsingDirective))
            {
                return;
            }

            if (!SupportsPredefinedType(identifierName))
                return;

            if (identifierName.IsPartOfDocumentationComment())
                return;

            if (IsArgumentExpressionOfNameOfExpression(context, identifierName))
                return;

            var typeSymbol = context.SemanticModel.GetSymbol(identifierName, context.CancellationToken) as ITypeSymbol;

            if (typeSymbol?.SupportsPredefinedType() != true)
                return;

            IAliasSymbol aliasSymbol = context.SemanticModel.GetAliasInfo(identifierName, context.CancellationToken);

            if (aliasSymbol != null)
                return;

            ReportDiagnostic(context, identifierName);
        }

        public static void AnalyzeXmlCrefAttribute(SyntaxNodeAnalysisContext context)
        {
            var xmlCrefAttribute = (XmlCrefAttributeSyntax)context.Node;

            CrefSyntax cref = xmlCrefAttribute.Cref;

            switch (cref?.Kind())
            {
                case SyntaxKind.NameMemberCref:
                    {
                        Analyze(context, cref, (NameMemberCrefSyntax)cref);
                        break;
                    }
                case SyntaxKind.QualifiedCref:
                    {
                        var qualifiedCref = (QualifiedCrefSyntax)cref;

                        MemberCrefSyntax memberCref = qualifiedCref.Member;

                        if (memberCref?.IsKind(SyntaxKind.NameMemberCref) != true)
                            break;

                        Analyze(context, cref, (NameMemberCrefSyntax)memberCref);
                        break;
                    }
            }
        }

        private static void Analyze(SyntaxNodeAnalysisContext context, CrefSyntax cref, NameMemberCrefSyntax nameMemberCref)
        {
            if (!(nameMemberCref.Name is IdentifierNameSyntax identifierName))
                return;

            if (!SupportsPredefinedType(identifierName))
                return;

            var typeSymbol = context.SemanticModel.GetSymbol(identifierName, context.CancellationToken) as ITypeSymbol;

            if (typeSymbol?.SupportsPredefinedType() != true)
                return;

            IAliasSymbol aliasSymbol = context.SemanticModel.GetAliasInfo(identifierName, context.CancellationToken);

            if (aliasSymbol != null)
                return;

            ReportDiagnostic(context, cref);
        }

        public static void Analyze(SyntaxNodeAnalysisContext context, QualifiedNameSyntax qualifiedName)
        {
            if (qualifiedName.IsParentKind(SyntaxKind.UsingDirective))
                return;

            if (!(qualifiedName.Right is IdentifierNameSyntax identifierName))
                return;

            if (!SupportsPredefinedType(identifierName))
                return;

            if (IsArgumentExpressionOfNameOfExpression(context, qualifiedName))
                return;

            var typeSymbol = context.SemanticModel.GetSymbol(qualifiedName, context.CancellationToken) as ITypeSymbol;

            if (typeSymbol?.SupportsPredefinedType() != true)
                return;

            ReportDiagnostic(context, qualifiedName);
        }

        public static void Analyze(SyntaxNodeAnalysisContext context, MemberAccessExpressionSyntax memberAccess)
        {
            if (memberAccess.IsParentKind(SyntaxKind.SimpleMemberAccessExpression))
                return;

            ExpressionSyntax expression = memberAccess.Expression;

            if (expression == null)
                return;

            SyntaxKind kind = expression.Kind();

            if (kind == SyntaxKind.IdentifierName)
            {
                if (!SupportsPredefinedType((IdentifierNameSyntax)expression))
                    return;
            }
            else if (kind == SyntaxKind.SimpleMemberAccessExpression)
            {
                memberAccess = (MemberAccessExpressionSyntax)expression;

                if (!(memberAccess.Name is IdentifierNameSyntax identifierName))
                    return;

                if (!SupportsPredefinedType(identifierName))
                    return;
            }
            else
            {
                return;
            }

            var typeSymbol = context.SemanticModel.GetSymbol(expression, context.CancellationToken) as ITypeSymbol;

            if (typeSymbol?.SupportsPredefinedType() != true)
                return;

            IAliasSymbol aliasSymbol = context.SemanticModel.GetAliasInfo(expression, context.CancellationToken);

            if (aliasSymbol != null)
                return;

            ReportDiagnostic(context, expression);
        }

        private static bool IsArgumentExpressionOfNameOfExpression(SyntaxNodeAnalysisContext context, SyntaxNode node)
        {
            SyntaxNode parent = node.Parent;

            if (parent?.IsKind(SyntaxKind.Argument) != true)
                return false;

            parent = parent.Parent;

            if (parent?.IsKind(SyntaxKind.ArgumentList) != true)
                return false;

            parent = parent.Parent;

            return parent != null
                && CSharpUtility.IsNameOfExpression(parent, context.SemanticModel, context.CancellationToken);
        }

        private static bool SupportsPredefinedType(IdentifierNameSyntax identifierName)
        {
            if (identifierName == null)
                return false;

            switch (identifierName.Identifier.ValueText)
            {
                case "Object":
                case "Boolean":
                case "Char":
                case "SByte":
                case "Byte":
                case "Int16":
                case "UInt16":
                case "Int32":
                case "UInt32":
                case "Int64":
                case "UInt64":
                case "Decimal":
                case "Single":
                case "Double":
                case "String":
                    return true;
                default:
                    return false;
            }
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
