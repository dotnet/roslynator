// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.CSharp.Comparers;
using Roslynator.Utilities;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static Roslynator.CSharp.CSharpFactory;

namespace Roslynator.CSharp.Refactorings
{
    internal static class IntroduceConstructorRefactoring
    {
        public static async Task ComputeRefactoringsAsync(RefactoringContext context, MemberDeclarationSyntax declaration)
        {
            if (context.IsRefactoringEnabled(RefactoringIdentifiers.IntroduceConstructor))
            {
                List<MemberDeclarationSyntax> members = await GetAssignableMembersAsync(context, declaration).ConfigureAwait(false);

                if (members?.Count > 0)
                {
                    context.RegisterRefactoring(
                        "Introduce constructor",
                        cancellationToken => RefactorAsync(context.Document, declaration, members, cancellationToken));
                }
            }
        }

        private static async Task<List<MemberDeclarationSyntax>> GetAssignableMembersAsync(
            RefactoringContext context,
            MemberDeclarationSyntax declaration)
        {
            if (declaration.IsKind(SyntaxKind.PropertyDeclaration, SyntaxKind.FieldDeclaration))
            {
                if (await CanBeAssignedFromConstructorAsync(context, declaration).ConfigureAwait(false))
                {
                    return new List<MemberDeclarationSyntax>() { declaration };
                }
            }
            else if (declaration.IsKind(SyntaxKind.ClassDeclaration, SyntaxKind.StructDeclaration))
            {
                List<MemberDeclarationSyntax> list = null;

                foreach (MemberDeclarationSyntax member in declaration.GetMembers())
                {
                    if (await CanBeAssignedFromConstructorAsync(context, member).ConfigureAwait(false))
                    {
                        (list ?? (list = new List<MemberDeclarationSyntax>())).Add(member);
                    }
                }

                return list;
            }

            return null;
        }

        private static async Task<bool> CanBeAssignedFromConstructorAsync(
            RefactoringContext context,
            MemberDeclarationSyntax declaration)
        {
            if (context.Span.Contains(declaration.Span))
            {
                switch (declaration.Kind())
                {
                    case SyntaxKind.PropertyDeclaration:
                        return await CanPropertyBeAssignedFromConstructorAsync(context, (PropertyDeclarationSyntax)declaration).ConfigureAwait(false);
                    case SyntaxKind.FieldDeclaration:
                        return await CanFieldBeAssignedFromConstructorAsync(context, (FieldDeclarationSyntax)declaration).ConfigureAwait(false);
                }
            }

            return false;
        }

        private static async Task<bool> CanPropertyBeAssignedFromConstructorAsync(
            RefactoringContext context,
            PropertyDeclarationSyntax propertyDeclaration)
        {
            SemanticModel semanticModel = await context.GetSemanticModelAsync().ConfigureAwait(false);
            CancellationToken cancellationToken = context.CancellationToken;

            ISymbol symbol = semanticModel.GetDeclaredSymbol(propertyDeclaration, cancellationToken);

            if (symbol?.IsStatic == false
                && propertyDeclaration.IsParentKind(SyntaxKind.ClassDeclaration, SyntaxKind.StructDeclaration))
            {
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
                SyntaxList<StatementSyntax> statements = body.Statements;

                return statements.Count == 1
                    && GetBackingFieldSymbol(statements[0], semanticModel, cancellationToken) != null;
            }
            else
            {
                ArrowExpressionClauseSyntax expressionBody = getter.ExpressionBody;

                if (expressionBody != null)
                {
                    ExpressionSyntax expression = expressionBody.Expression;

                    return expression != null
                        && GetBackingFieldSymbol(expression, semanticModel, cancellationToken) != null;
                }
            }

            return true;
        }

        private static async Task<bool> CanFieldBeAssignedFromConstructorAsync(
            RefactoringContext context,
            FieldDeclarationSyntax fieldDeclaration)
        {
            VariableDeclaratorSyntax variable = fieldDeclaration
                .Declaration?
                .Variables
                .SingleOrDefault(throwException: false);

            if (variable != null)
            {
                MemberDeclarationSyntax parentMember = GetContainingMember(fieldDeclaration);

                if (parentMember != null)
                {
                    SemanticModel semanticModel = await context.GetSemanticModelAsync().ConfigureAwait(false);

                    ISymbol symbol = semanticModel.GetDeclaredSymbol(variable, context.CancellationToken);

                    return symbol?.IsStatic == false
                        && !parentMember
                            .GetMembers()
                            .Any(member => IsBackingField(member, symbol, context, semanticModel));
                }
            }

            return false;
        }

        private static bool IsBackingField(
            MemberDeclarationSyntax member,
            ISymbol symbol,
            RefactoringContext context,
            SemanticModel semanticModel)
        {
            if (member.IsKind(SyntaxKind.PropertyDeclaration)
                && context.Span.Contains(member.Span))
            {
                var propertyDeclaration = (PropertyDeclarationSyntax)member;

                ArrowExpressionClauseSyntax expressionBody = propertyDeclaration.ExpressionBody;

                if (expressionBody != null)
                {
                    ExpressionSyntax expression = expressionBody.Expression;

                    return expression != null
                        && symbol.Equals(GetBackingFieldSymbol(expression, semanticModel, context.CancellationToken));
                }
                else
                {
                    AccessorDeclarationSyntax getter = propertyDeclaration.Getter();

                    if (getter != null)
                        return IsBackingField(getter, symbol, semanticModel, context.CancellationToken);
                }
            }

            return false;
        }

        private static bool IsBackingField(AccessorDeclarationSyntax getter, ISymbol symbol, SemanticModel semanticModel, CancellationToken cancellationToken)
        {
            BlockSyntax getterBody = getter.Body;

            if (getterBody != null)
            {
                SyntaxList<StatementSyntax> statements = getterBody.Statements;

                return statements.Count == 1
                    && symbol.Equals(GetBackingFieldSymbol(statements[0], semanticModel, cancellationToken));
            }
            else
            {
                ArrowExpressionClauseSyntax getterExpressionBody = getter.ExpressionBody;

                if (getterExpressionBody != null)
                {
                    ExpressionSyntax expression = getterExpressionBody.Expression;

                    return expression != null
                        && symbol.Equals(GetBackingFieldSymbol(expression, semanticModel, cancellationToken));
                }
            }

            return false;
        }

        private static ISymbol GetBackingFieldSymbol(
            StatementSyntax statement,
            SemanticModel semanticModel,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (statement.IsKind(SyntaxKind.ReturnStatement))
            {
                var returnStatement = (ReturnStatementSyntax)statement;

                if (returnStatement.Expression != null)
                    return GetBackingFieldSymbol(returnStatement.Expression, semanticModel, cancellationToken);
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
                    && symbol.IsField())
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
                case SyntaxKind.IdentifierName:
                    return true;
                case SyntaxKind.SimpleMemberAccessExpression:
                    {
                        var memberAccess = (MemberAccessExpressionSyntax)expression;

                        return memberAccess.Expression.IsKind(SyntaxKind.ThisExpression)
                            && memberAccess.Name.IsKind(SyntaxKind.IdentifierName);
                    }
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
            MemberDeclarationSyntax parentMember = GetContainingMember(declaration);

            SyntaxList<MemberDeclarationSyntax> members = parentMember.GetMembers();

            SyntaxList<MemberDeclarationSyntax> newMembers = members.InsertMember(
                CreateConstructor(GetConstructorIdentifierText(parentMember), assignableMembers),
                MemberDeclarationComparer.ByKind);

            MemberDeclarationSyntax newNode = parentMember.WithMembers(newMembers)
                .WithFormatterAnnotation();

            return document.ReplaceNodeAsync(parentMember, newNode, cancellationToken);
        }

        private static string GetConstructorIdentifierText(MemberDeclarationSyntax declaration)
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

        private static MemberDeclarationSyntax GetContainingMember(MemberDeclarationSyntax declaration)
        {
            switch (declaration.Kind())
            {
                case SyntaxKind.ClassDeclaration:
                case SyntaxKind.StructDeclaration:
                    return declaration;
                default:
                    {
                        Debug.Assert(declaration.Parent is MemberDeclarationSyntax, "Parent is not MemberDeclarationSyntax");
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

                if (expression?.IsKind(SyntaxKind.IdentifierName) == true)
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
