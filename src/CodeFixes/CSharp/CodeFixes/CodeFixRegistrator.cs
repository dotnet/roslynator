// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Roslynator.CSharp.CodeFixes
{
    internal static class CodeFixRegistrator
    {
        public static CodeFixRegistrationResult ChangeType(
            CodeFixContext context,
            Diagnostic diagnostic,
            TypeSyntax type,
            ITypeSymbol newTypeSymbol,
            SemanticModel semanticModel,
            string additionalKey = null)
        {
            CodeAction codeAction = CodeActionFactory.ChangeType(context.Document, type, newTypeSymbol, semanticModel, equivalenceKey: EquivalenceKey.Create(diagnostic, additionalKey));

            context.RegisterCodeFix(codeAction, diagnostic);

            return new CodeFixRegistrationResult(true);
        }

        public static CodeFixRegistrationResult ChangeTypeToVar(
            CodeFixContext context,
            Diagnostic diagnostic,
            TypeSyntax type,
            string additionalKey = null)
        {
            CodeAction codeAction = CodeActionFactory.ChangeTypeToVar(context.Document, type, equivalenceKey: EquivalenceKey.Create(diagnostic, additionalKey));

            context.RegisterCodeFix(codeAction, diagnostic);

            return new CodeFixRegistrationResult(true);
        }

        public static void AddExplicitCast(
            CodeFixContext context,
            Diagnostic diagnostic,
            ExpressionSyntax expression,
            ITypeSymbol destinationType,
            SemanticModel semanticModel)
        {
            CodeAction codeAction = CodeActionFactory.AddExplicitCast(context.Document, expression, destinationType, semanticModel, equivalenceKey: EquivalenceKey.Create(diagnostic, CodeFixIdentifiers.AddExplicitCast));

            context.RegisterCodeFix(codeAction, diagnostic);
        }

        public static void RemoveMemberDeclaration(
            CodeFixContext context,
            Diagnostic diagnostic,
            MemberDeclarationSyntax memberDeclaration,
            string additionalKey = null)
        {
            CodeAction codeAction = CodeActionFactory.RemoveMemberDeclaration(context.Document, memberDeclaration, equivalenceKey: EquivalenceKey.Create(diagnostic, additionalKey));

            context.RegisterCodeFix(codeAction, diagnostic);
        }

        public static void RemoveStatement(
            CodeFixContext context,
            Diagnostic diagnostic,
            StatementSyntax statement,
            string title = null,
            string additionalKey = null)
        {
            if (statement.IsEmbedded())
                return;

            CodeAction codeAction = CodeActionFactory.RemoveStatement(context.Document, statement, title: title, equivalenceKey: EquivalenceKey.Create(diagnostic, additionalKey));

            context.RegisterCodeFix(codeAction, diagnostic);
        }

        public static void ReplaceNullWithDefaultValue(
            CodeFixContext context,
            Diagnostic diagnostic,
            ExpressionSyntax expression,
            SemanticModel semanticModel,
            string additionalKey = null)
        {
            ITypeSymbol typeSymbol = semanticModel.GetTypeInfo(expression, context.CancellationToken).ConvertedType;

            if (typeSymbol == null)
                return;

            ReplaceNullWithDefaultValue(context, diagnostic, expression, typeSymbol, additionalKey);
        }

        public static void ReplaceNullWithDefaultValue(
            CodeFixContext context,
            Diagnostic diagnostic,
            ExpressionSyntax expression,
            ITypeSymbol typeSymbol,
            string additionalKey = null)
        {
            if (!typeSymbol.SupportsExplicitDeclaration())
                return;

            CodeAction codeAction = CodeActionFactory.ReplaceNullWithDefaultValue(context.Document, expression, typeSymbol, equivalenceKey: EquivalenceKey.Create(diagnostic, additionalKey));

            context.RegisterCodeFix(codeAction, diagnostic);
        }

        public static void ChangeTypeOrReturnType(
            CodeFixContext context,
            Diagnostic diagnostic,
            SyntaxNode node,
            ITypeSymbol newTypeSymbol,
            SemanticModel semanticModel,
            string additionalKey = null)
        {
            if (newTypeSymbol.IsErrorType())
                return;

            TypeSyntax type = CSharpUtility.GetTypeOrReturnType(node);

            if (type == null)
                return;

            CodeAction codeAction = CodeActionFactory.ChangeType(context.Document, type, newTypeSymbol, semanticModel, equivalenceKey: EquivalenceKey.Create(diagnostic, additionalKey));

            context.RegisterCodeFix(codeAction, diagnostic);
        }
    }
}
