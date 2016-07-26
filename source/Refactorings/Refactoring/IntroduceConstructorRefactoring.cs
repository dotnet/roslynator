// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Pihrtsoft.CodeAnalysis.Text;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Pihrtsoft.CodeAnalysis.CSharp.Refactoring
{
    internal static class IntroduceConstructorRefactoring
    {
        public static async Task ComputeRefactoringsAsync(RefactoringContext context, MemberDeclarationSyntax declaration)
        {
            if (context.Settings.IsRefactoringEnabled(RefactoringIdentifiers.IntroduceConstructor)
                && context.SupportsSemanticModel)
            {
                List<MemberDeclarationSyntax> members = await GetAssignableMembersAsync(context, declaration);

                if (members?.Count > 0)
                {
                    string text = "Introduce constructor from selected member";

                    if (members.Count > 1)
                        text += "s";

                    context.RegisterRefactoring(
                        text,
                        cancellationToken =>
                        {
                            return RefactorAsync(
                                context.Document,
                                declaration,
                                members,
                                cancellationToken);
                        });
                }
            }
        }

        private static async Task<List<MemberDeclarationSyntax>> GetAssignableMembersAsync(
            RefactoringContext context,
            MemberDeclarationSyntax declaration)
        {
            if (declaration.IsKind(SyntaxKind.PropertyDeclaration, SyntaxKind.FieldDeclaration))
            {
                if (await CanBeAssignedFromConstructorAsync(context, declaration))
                {
                    var list = new List<MemberDeclarationSyntax>();
                    list.Add(declaration);
                    return list;
                }
            }
            else if (declaration.IsKind(SyntaxKind.ClassDeclaration, SyntaxKind.StructDeclaration))
            {
                List<MemberDeclarationSyntax> list = null;

                foreach (MemberDeclarationSyntax member in declaration.GetMembers())
                {
                    if (await CanBeAssignedFromConstructorAsync(context, member))
                    {
                        if (list == null)
                            list = new List<MemberDeclarationSyntax>();

                        list.Add(member);
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
                        return await CanPropertyBeAssignedFromConstructorAsync(context, (PropertyDeclarationSyntax)declaration);
                    case SyntaxKind.FieldDeclaration:
                        return await CanFieldBeAssignedFromConstructorAsync(context, (FieldDeclarationSyntax)declaration);
                }
            }

            return false;
        }

        private static async Task<bool> CanPropertyBeAssignedFromConstructorAsync(
            RefactoringContext context,
            PropertyDeclarationSyntax propertyDeclaration)
        {
            SemanticModel semanticModel = await context.GetSemanticModelAsync();

            ISymbol symbol = semanticModel.GetDeclaredSymbol(propertyDeclaration, context.CancellationToken);

            if (symbol != null
                && !symbol.IsStatic
                && propertyDeclaration.Parent != null
                && propertyDeclaration.Parent.IsKind(SyntaxKind.ClassDeclaration, SyntaxKind.StructDeclaration))
            {
                if (propertyDeclaration.ExpressionBody != null)
                {
                    ExpressionSyntax expression = propertyDeclaration.ExpressionBody?.Expression;

                    if (expression != null)
                        return GetBackingFieldSymbol(expression, semanticModel, context.CancellationToken) != null;
                }
                else
                {
                    AccessorDeclarationSyntax getter = propertyDeclaration.Getter();

                    if (getter != null)
                    {
                        if (getter.Body == null)
                            return true;

                        if (getter.Body.Statements.Count == 1)
                            return GetBackingFieldSymbol(getter.Body.Statements[0], semanticModel, context.CancellationToken) != null;
                    }
                }
            }

            return false;
        }

        private static async Task<bool> CanFieldBeAssignedFromConstructorAsync(
            RefactoringContext context,
            FieldDeclarationSyntax fieldDeclaration)
        {
            if (fieldDeclaration.Declaration != null
                && fieldDeclaration.Declaration.Variables.Count == 1)
            {
                MemberDeclarationSyntax parentDeclaration = GetContainingDeclaration(fieldDeclaration);

                if (parentDeclaration != null)
                {
                    SemanticModel semanticModel = await context.GetSemanticModelAsync();

                    ISymbol symbol = semanticModel.GetDeclaredSymbol(fieldDeclaration.Declaration.Variables[0], context.CancellationToken);

                    return symbol != null
                        && !symbol.IsStatic
                        && !parentDeclaration
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

                if (propertyDeclaration.ExpressionBody?.Expression != null)
                {
                    ISymbol symbol2 = GetBackingFieldSymbol(propertyDeclaration.ExpressionBody.Expression, semanticModel, context.CancellationToken);

                    if (symbol.Equals(symbol2))
                        return true;
                }
                else
                {
                    AccessorDeclarationSyntax getter = propertyDeclaration.Getter();

                    if (getter?.Body?.Statements.Count == 1)
                    {
                        ISymbol symbol2 = GetBackingFieldSymbol(getter.Body.Statements[0], semanticModel, context.CancellationToken);

                        if (symbol.Equals(symbol2))
                            return true;
                    }
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
            ISymbol symbol = semanticModel.GetSymbolInfo(expression, cancellationToken).Symbol;

            if (symbol != null
                && !symbol.IsStatic
                && symbol.IsField())
            {
                return symbol;
            }

            return null;
        }

        private static async Task<Document> RefactorAsync(
            Document document,
            MemberDeclarationSyntax declaration,
            List<MemberDeclarationSyntax> assignableMembers,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            SyntaxNode oldRoot = await document.GetSyntaxRootAsync(cancellationToken);

            MemberDeclarationSyntax parentDeclaration = GetContainingDeclaration(declaration);

            SyntaxList<MemberDeclarationSyntax> members = parentDeclaration.GetMembers();

            SyntaxList<MemberDeclarationSyntax> newMembers = members.Insert(
                IndexOfLastConstructorOrField(members) + 1,
                CreateConstructor(GetConstructorIdentifierText(parentDeclaration), assignableMembers));

            MemberDeclarationSyntax newNode = parentDeclaration.SetMembers(newMembers)
                .WithFormatterAnnotation();

            SyntaxNode newRoot = oldRoot.ReplaceNode(parentDeclaration, newNode);

            return document.WithSyntaxRoot(newRoot);
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

        private static MemberDeclarationSyntax GetContainingDeclaration(MemberDeclarationSyntax declaration)
        {
            switch (declaration.Kind())
            {
                case SyntaxKind.ClassDeclaration:
                case SyntaxKind.StructDeclaration:
                    return declaration;
                default:
                    return (MemberDeclarationSyntax)declaration.Parent;
            }
        }

        private static ConstructorDeclarationSyntax CreateConstructor(string identifierText, IEnumerable<MemberDeclarationSyntax> members)
        {
            var parameters = new List<ParameterSyntax>();
            var statements = new List<ExpressionStatementSyntax>();

            foreach (MemberDeclarationSyntax member in members)
            {
                string name = GetIdentifier(member).ValueText;

                string parameterName = IdentifierHelper.RemoveUnderscoreFromIdentifier(name);
                parameterName = TextUtility.FirstCharToLower(parameterName);

                statements.Add(ExpressionStatement(
                    AssignmentExpression(
                        SyntaxKind.SimpleAssignmentExpression,
                        IdentifierName(name),
                        IdentifierName(parameterName))));

                parameters.Add(Parameter(
                    default(SyntaxList<AttributeListSyntax>),
                    default(SyntaxTokenList),
                    GetType(member),
                    Identifier(parameterName),
                    null));
            }

            return ConstructorDeclaration(
                default(SyntaxList<AttributeListSyntax>),
                TokenList(Token(SyntaxKind.PublicKeyword)),
                Identifier(identifierText),
                ParameterList(SeparatedList(parameters)),
                null,
                Block(statements));
        }

        private static int IndexOfLastConstructorOrField(SyntaxList<MemberDeclarationSyntax> members)
        {
            for (int i = members.Count - 1; i >= 0; i--)
            {
                if (members[i].IsKind(SyntaxKind.ConstructorDeclaration))
                    return i;
            }

            for (int i = members.Count - 1; i >= 0; i--)
            {
                if (members[i].IsKind(SyntaxKind.FieldDeclaration))
                    return i;
            }

            return -1;
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
            if (propertyDeclaration.ExpressionBody != null)
            {
                ExpressionSyntax expression = propertyDeclaration.ExpressionBody?.Expression;

                if (expression?.IsKind(SyntaxKind.IdentifierName) == true)
                    return ((IdentifierNameSyntax)expression).Identifier;
            }
            else
            {
                AccessorDeclarationSyntax getter = propertyDeclaration.Getter();

                if (getter?.Body != null)
                {
                    var returnStatement = (ReturnStatementSyntax)getter.Body.Statements[0];

                    return ((IdentifierNameSyntax)returnStatement.Expression).Identifier;
                }
            }

            return propertyDeclaration.Identifier;
        }
    }
}
