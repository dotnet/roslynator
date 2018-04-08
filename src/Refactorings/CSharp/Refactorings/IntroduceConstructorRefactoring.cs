// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using Roslynator.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static Roslynator.CSharp.CSharpFactory;

namespace Roslynator.CSharp.Refactorings
{
    internal static class IntroduceConstructorRefactoring
    {
        public static async Task ComputeRefactoringsAsync(RefactoringContext context, MemberDeclarationSyntax declaration)
        {
            if (!context.IsRefactoringEnabled(RefactoringIdentifiers.IntroduceConstructor))
                return;

            List<MemberDeclarationSyntax> members = null;

            SyntaxKind kind = declaration.Kind();

            if (kind.Is(SyntaxKind.PropertyDeclaration, SyntaxKind.FieldDeclaration))
            {
                if (context.Span.Contains(declaration.Span))
                {
                    SemanticModel semanticModel = await context.GetSemanticModelAsync().ConfigureAwait(false);

                    if (CanBeAssignedFromConstructor(declaration, context.Span, semanticModel, context.CancellationToken))
                    {
                        members = new List<MemberDeclarationSyntax>() { declaration };
                    }
                }
            }
            else if (kind.Is(SyntaxKind.ClassDeclaration, SyntaxKind.StructDeclaration))
            {
                SemanticModel semanticModel = null;

                foreach (MemberDeclarationSyntax member in SyntaxInfo.MemberDeclarationListInfo(declaration).Members)
                {
                    if (context.Span.Contains(member.Span))
                    {
                        if (semanticModel == null)
                            semanticModel = await context.GetSemanticModelAsync().ConfigureAwait(false);

                        if (CanBeAssignedFromConstructor(member, context.Span, semanticModel, context.CancellationToken))
                        {
                            (members ?? (members = new List<MemberDeclarationSyntax>())).Add(member);
                        }
                    }
                }
            }

            if (members == null)
                return;

            context.RegisterRefactoring(
                "Introduce constructor",
                ct => RefactorAsync(context.Document, declaration, members, ct));
        }

        private static bool CanBeAssignedFromConstructor(
            MemberDeclarationSyntax member,
            TextSpan span,
            SemanticModel semanticModel,
            CancellationToken cancellationToken)
        {
            switch (member.Kind())
            {
                case SyntaxKind.PropertyDeclaration:
                    return CanPropertyBeAssignedFromConstructor((PropertyDeclarationSyntax)member, semanticModel, cancellationToken);
                case SyntaxKind.FieldDeclaration:
                    return CanFieldBeAssignedFromConstructor((FieldDeclarationSyntax)member, span, semanticModel, cancellationToken);
                default:
                    return false;
            }
        }

        private static bool CanPropertyBeAssignedFromConstructor(
            PropertyDeclarationSyntax propertyDeclaration,
            SemanticModel semanticModel,
            CancellationToken cancellationToken)
        {
            ISymbol symbol = semanticModel.GetDeclaredSymbol(propertyDeclaration, cancellationToken);

            if (symbol == null)
                return false;

            if (symbol.IsStatic)
                return false;

            if (!propertyDeclaration.IsParentKind(SyntaxKind.ClassDeclaration, SyntaxKind.StructDeclaration))
                return false;

            ArrowExpressionClauseSyntax expressionBody = propertyDeclaration.ExpressionBody;

            if (expressionBody != null)
            {
                ExpressionSyntax expression = expressionBody?.Expression;

                if (expression != null)
                    return GetBackingFieldSymbol(expression, semanticModel, cancellationToken) != null;
            }
            else
            {
                AccessorDeclarationSyntax getter = propertyDeclaration.Getter();

                if (getter != null)
                    return CanPropertyBeAssignedFromConstructor(getter, semanticModel, cancellationToken);
            }

            return false;
        }

        private static bool CanPropertyBeAssignedFromConstructor(
            AccessorDeclarationSyntax getter,
            SemanticModel semanticModel,
            CancellationToken cancellationToken)
        {
            BlockSyntax body = getter.Body;

            if (body != null)
            {
                StatementSyntax statement = body.Statements.SingleOrDefault(shouldThrow: false);

                if (statement != null)
                    return GetBackingFieldSymbol(statement, semanticModel, cancellationToken) != null;
            }
            else
            {
                ExpressionSyntax expression = getter.ExpressionBody?.Expression;

                return expression != null
                    && GetBackingFieldSymbol(expression, semanticModel, cancellationToken) != null;
            }

            return true;
        }

        private static bool CanFieldBeAssignedFromConstructor(
            FieldDeclarationSyntax fieldDeclaration,
            TextSpan span,
            SemanticModel semanticModel,
            CancellationToken cancellationToken)
        {
            VariableDeclaratorSyntax variable = fieldDeclaration
                .Declaration?
                .Variables
                .SingleOrDefault(shouldThrow: false);

            if (variable == null)
                return false;

            MemberDeclarationListInfo info = SyntaxInfo.MemberDeclarationListInfo(GetContainingDeclaration(fieldDeclaration));

            if (!info.Success)
                return false;

            ISymbol symbol = semanticModel.GetDeclaredSymbol(variable, cancellationToken);

            if (symbol == null)
                return false;

            if (symbol.IsStatic)
                return false;

            foreach (MemberDeclarationSyntax member in info.Members)
            {
                if (IsBackingField(member, symbol, span, semanticModel, cancellationToken))
                    return false;
            }

            return true;
        }

        private static bool IsBackingField(
            MemberDeclarationSyntax member,
            ISymbol symbol,
            TextSpan span,
            SemanticModel semanticModel,
            CancellationToken cancellationToken)
        {
            if (!member.IsKind(SyntaxKind.PropertyDeclaration))
                return false;

            if (!span.Contains(member.Span))
                return false;

            var propertyDeclaration = (PropertyDeclarationSyntax)member;

            ArrowExpressionClauseSyntax expressionBody = propertyDeclaration.ExpressionBody;

            if (expressionBody != null)
            {
                ExpressionSyntax expression = expressionBody.Expression;

                return expression != null
                    && symbol.Equals(GetBackingFieldSymbol(expression, semanticModel, cancellationToken));
            }
            else
            {
                AccessorDeclarationSyntax getter = propertyDeclaration.Getter();

                if (getter != null)
                    return IsBackingField(getter, symbol, semanticModel, cancellationToken);
            }

            return false;
        }

        private static bool IsBackingField(AccessorDeclarationSyntax getter, ISymbol symbol, SemanticModel semanticModel, CancellationToken cancellationToken)
        {
            BlockSyntax body = getter.Body;

            if (body != null)
            {
                StatementSyntax statement = body.Statements.SingleOrDefault(shouldThrow: false);

                if (statement != null)
                    return symbol.Equals(GetBackingFieldSymbol(statement, semanticModel, cancellationToken));
            }
            else
            {
                ExpressionSyntax expression = getter.ExpressionBody?.Expression;

                return expression != null
                    && symbol.Equals(GetBackingFieldSymbol(expression, semanticModel, cancellationToken));
            }

            return false;
        }

        private static ISymbol GetBackingFieldSymbol(
            StatementSyntax statement,
            SemanticModel semanticModel,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (statement is ReturnStatementSyntax returnStatement)
            {
                ExpressionSyntax expression = returnStatement.Expression;

                if (expression != null)
                    return GetBackingFieldSymbol(expression, semanticModel, cancellationToken);
            }

            return null;
        }

        private static ISymbol GetBackingFieldSymbol(
            ExpressionSyntax expression,
            SemanticModel semanticModel,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (IsIdentifierNameOptionallyQualifiedWithThis(expression))
            {
                ISymbol symbol = semanticModel.GetSymbol(expression, cancellationToken);

                if (symbol?.IsStatic == false
                    && symbol.Kind == SymbolKind.Field)
                {
                    return symbol;
                }
            }

            return null;
        }

        private static bool IsIdentifierNameOptionallyQualifiedWithThis(ExpressionSyntax expression)
        {
            switch (expression.Kind())
            {
                case SyntaxKind.SimpleMemberAccessExpression:
                    {
                        var memberAccess = (MemberAccessExpressionSyntax)expression;

                        return memberAccess.Expression.IsKind(SyntaxKind.ThisExpression)
                            && memberAccess.Name.IsKind(SyntaxKind.IdentifierName);
                    }
                case SyntaxKind.IdentifierName:
                    return true;
                default:
                    return false;
            }
        }

        private static Task<Document> RefactorAsync(
            Document document,
            MemberDeclarationSyntax declaration,
            List<MemberDeclarationSyntax> assignableMembers,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            MemberDeclarationListInfo info = SyntaxInfo.MemberDeclarationListInfo(GetContainingDeclaration(declaration));

            SyntaxList<MemberDeclarationSyntax> newMembers = MemberDeclarationInserter.Default.Insert(info.Members, CreateConstructor(GetConstructorIdentifierText(info.Parent), assignableMembers));

            SyntaxNode newNode = info.WithMembers(newMembers).Parent.WithFormatterAnnotation();

            return document.ReplaceNodeAsync(info.Parent, newNode, cancellationToken);
        }

        private static string GetConstructorIdentifierText(SyntaxNode declaration)
        {
            switch (declaration.Kind())
            {
                case SyntaxKind.ClassDeclaration:
                    return ((ClassDeclarationSyntax)declaration).Identifier.Text;
                case SyntaxKind.StructDeclaration:
                    return ((StructDeclarationSyntax)declaration).Identifier.Text;
            }

            return null;
        }

        private static MemberDeclarationSyntax GetContainingDeclaration(MemberDeclarationSyntax declaration)
        {
            switch (declaration.Kind())
            {
                case SyntaxKind.ClassDeclaration:
                case SyntaxKind.StructDeclaration:
                    return declaration;
                default:
                    {
                        Debug.Assert(declaration.Parent is MemberDeclarationSyntax);
                        return declaration.Parent as MemberDeclarationSyntax;
                    }
            }
        }

        private static ConstructorDeclarationSyntax CreateConstructor(string identifierText, IEnumerable<MemberDeclarationSyntax> members)
        {
            var parameters = new List<ParameterSyntax>();
            var statements = new List<ExpressionStatementSyntax>();

            foreach (MemberDeclarationSyntax member in members)
            {
                string name = GetIdentifier(member).ValueText;
                string parameterName = StringUtility.ToCamelCase(name);

                statements.Add(SimpleAssignmentStatement(
                        IdentifierName(name).QualifyWithThis(),
                        IdentifierName(parameterName)));

                parameters.Add(Parameter(
                    default(SyntaxList<AttributeListSyntax>),
                    default(SyntaxTokenList),
                    GetType(member),
                    Identifier(parameterName),
                    default(EqualsValueClauseSyntax)));
            }

            return ConstructorDeclaration(
                default(SyntaxList<AttributeListSyntax>),
                Modifiers.Public(),
                Identifier(identifierText),
                ParameterList(SeparatedList(parameters)),
                default(ConstructorInitializerSyntax),
                Block(statements));
        }

        private static TypeSyntax GetType(MemberDeclarationSyntax memberDeclaration)
        {
            switch (memberDeclaration.Kind())
            {
                case SyntaxKind.PropertyDeclaration:
                    return ((PropertyDeclarationSyntax)memberDeclaration).Type;
                case SyntaxKind.FieldDeclaration:
                    return ((FieldDeclarationSyntax)memberDeclaration).Declaration.Type;
            }

            return null;
        }

        private static SyntaxToken GetIdentifier(MemberDeclarationSyntax memberDeclaration)
        {
            switch (memberDeclaration.Kind())
            {
                case SyntaxKind.PropertyDeclaration:
                    return GetPropertyIdentifier((PropertyDeclarationSyntax)memberDeclaration);
                case SyntaxKind.FieldDeclaration:
                    return ((FieldDeclarationSyntax)memberDeclaration).Declaration.Variables[0].Identifier;
            }

            return default(SyntaxToken);
        }

        private static SyntaxToken GetPropertyIdentifier(PropertyDeclarationSyntax propertyDeclaration)
        {
            ArrowExpressionClauseSyntax expressionBody = propertyDeclaration.ExpressionBody;

            if (expressionBody != null)
            {
                ExpressionSyntax expression = expressionBody.Expression;

                if (expression?.Kind() == SyntaxKind.IdentifierName)
                    return ((IdentifierNameSyntax)expression).Identifier;
            }
            else
            {
                AccessorDeclarationSyntax getter = propertyDeclaration.Getter();

                if (getter != null)
                {
                    BlockSyntax getterBody = getter.Body;

                    if (getterBody != null)
                    {
                        var returnStatement = (ReturnStatementSyntax)getterBody.Statements[0];

                        return GetIdentifier(returnStatement.Expression);
                    }
                    else
                    {
                        ArrowExpressionClauseSyntax getterExpressionBody = getter.ExpressionBody;

                        if (getterExpressionBody != null)
                            return GetIdentifier(getterExpressionBody.Expression);
                    }
                }
            }

            return propertyDeclaration.Identifier;
        }

        private static SyntaxToken GetIdentifier(ExpressionSyntax expression)
        {
            switch (expression.Kind())
            {
                case SyntaxKind.IdentifierName:
                    return ((IdentifierNameSyntax)expression).Identifier;
                case SyntaxKind.SimpleMemberAccessExpression:
                    {
                        var memberAccess = (MemberAccessExpressionSyntax)expression;

                        var identifierName = (IdentifierNameSyntax)memberAccess.Name;

                        return identifierName.Identifier;
                    }
            }

            Debug.Fail(expression.Kind().ToString());

            return default(SyntaxToken);
        }
    }
}
