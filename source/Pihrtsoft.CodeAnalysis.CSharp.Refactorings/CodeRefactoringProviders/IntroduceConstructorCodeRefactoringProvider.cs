// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeRefactorings;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Formatting;
using Pihrtsoft.CodeAnalysis.Text;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Pihrtsoft.CodeAnalysis.CSharp.CodeRefactoringProviders
{
    [ExportCodeRefactoringProvider(LanguageNames.CSharp, Name = nameof(IntroduceConstructorCodeRefactoringProvider))]
    public class IntroduceConstructorCodeRefactoringProvider : CodeRefactoringProvider
    {
        public override async Task ComputeRefactoringsAsync(CodeRefactoringContext context)
        {
            SyntaxNode root = await context.Document.GetSyntaxRootAsync(context.CancellationToken);

            MemberDeclarationSyntax declaration = root
                .FindNode(context.Span, getInnermostNodeForTie: true)?
                .FirstAncestorOrSelf<MemberDeclarationSyntax>();

            if (declaration == null)
                return;

            if (context.Document.SupportsSemanticModel)
            {
                SemanticModel semanticModel = await context.Document.GetSemanticModelAsync(context.CancellationToken);

                List<MemberDeclarationSyntax> members = GetAssignableMembers(declaration, context, semanticModel).ToList();

                if (members.Count > 0)
                {
                    string text = "Introduce constructor from selected member";

                    if (members.Count > 1)
                        text += "s";

                    context.RegisterRefactoring(
                        text,
                        cancellationToken =>
                        {
                            return CreateChangedDocumentAsync(
                                context.Document,
                                declaration,
                                members,
                                cancellationToken);
                        });
                }
            }
        }

        public static IEnumerable<MemberDeclarationSyntax> GetAssignableMembers(
            MemberDeclarationSyntax declaration,
            CodeRefactoringContext context,
            SemanticModel semanticModel)
        {
            switch (declaration.Kind())
            {
                case SyntaxKind.PropertyDeclaration:
                case SyntaxKind.FieldDeclaration:
                    {
                        if (context.Span.Contains(declaration.Span)
                            && CanBeAssignedFromConstructor(declaration, context, semanticModel))
                        {
                            yield return declaration;
                        }

                        break;
                    }
                case SyntaxKind.ClassDeclaration:
                case SyntaxKind.StructDeclaration:
                    {
                        SyntaxList<MemberDeclarationSyntax> members = GetMembers(declaration);

                        if (members.Count > 0)
                        {
                            foreach (MemberDeclarationSyntax member in members)
                            {
                                if (context.Span.Contains(member.Span)
                                    && CanBeAssignedFromConstructor(member, context, semanticModel))
                                {
                                    yield return member;
                                }
                            }
                        }

                        break;
                    }
            }
        }

        public static bool CanBeAssignedFromConstructor(
            MemberDeclarationSyntax declaration,
            CodeRefactoringContext context,
            SemanticModel semanticModel)
        {
            switch (declaration.Kind())
            {
                case SyntaxKind.PropertyDeclaration:
                    return CanBeAssignedFromConstructor((PropertyDeclarationSyntax)declaration, context, semanticModel);
                case SyntaxKind.FieldDeclaration:
                    return CanBeAssignedFromConstructor((FieldDeclarationSyntax)declaration, context, semanticModel);
            }

            return false;
        }

        private static bool CanBeAssignedFromConstructor(
            FieldDeclarationSyntax fieldDeclaration,
            CodeRefactoringContext context,
            SemanticModel semanticModel)
        {
            if (fieldDeclaration.Declaration != null
                && fieldDeclaration.Declaration.Variables.Count == 1)
            {
                MemberDeclarationSyntax parentDeclaration = GetContainingDeclaration(fieldDeclaration);

                if (parentDeclaration != null)
                {
                    ISymbol symbol = semanticModel.GetDeclaredSymbol(fieldDeclaration.Declaration.Variables[0], context.CancellationToken);

                    return symbol != null
                        && !symbol.IsStatic
                        && !IsBackingField(symbol, GetMembers(parentDeclaration), context, semanticModel);
                }
            }

            return false;
        }

        private static bool IsBackingField(
            ISymbol symbol,
            SyntaxList<MemberDeclarationSyntax> members,
            CodeRefactoringContext context,
            SemanticModel semanticModel)
        {
            foreach (MemberDeclarationSyntax member in members)
            {
                if (member.IsKind(SyntaxKind.PropertyDeclaration)
                    && context.Span.Contains(member.Span))
                {
                    var propertyDeclaration = (PropertyDeclarationSyntax)member;

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

        private static bool CanBeAssignedFromConstructor(
            PropertyDeclarationSyntax propertyDeclaration,
            CodeRefactoringContext context,
            SemanticModel semanticModel)
        {
            ISymbol symbol = semanticModel.GetDeclaredSymbol(propertyDeclaration, context.CancellationToken);

            if (symbol == null
                || symbol.IsStatic
                || propertyDeclaration.Parent == null
                || !propertyDeclaration.Parent.IsAnyKind(SyntaxKind.ClassDeclaration, SyntaxKind.StructDeclaration))
            {
                return false;
            }

            AccessorDeclarationSyntax getter = propertyDeclaration.Getter();

            if (getter != null)
            {
                if (getter.Body == null)
                    return true;

                if (getter.Body.Statements.Count == 1)
                    return GetBackingFieldSymbol(getter.Body.Statements[0], semanticModel, context.CancellationToken) != null;
            }
            else
            {
                ExpressionSyntax expression = propertyDeclaration.ExpressionBody?.Expression;

                if (expression != null)
                    return GetBackingFieldSymbol(expression, semanticModel, context.CancellationToken) != null;
            }

            return false;
        }

        private static ISymbol GetBackingFieldSymbol(StatementSyntax statement, SemanticModel semanticModel, CancellationToken cancellationToken)
        {
            if (statement.IsKind(SyntaxKind.ReturnStatement))
            {
                var returnStatement = (ReturnStatementSyntax)statement;

                if (returnStatement.Expression != null)
                    return GetBackingFieldSymbol(returnStatement.Expression, semanticModel, cancellationToken);
            }

            return null;
        }

        private static ISymbol GetBackingFieldSymbol(ExpressionSyntax expression, SemanticModel semanticModel, CancellationToken cancellationToken)
        {
            ISymbol symbol = semanticModel.GetSymbolInfo(expression, cancellationToken).Symbol;

            if (symbol != null
                && !symbol.IsStatic
                && symbol.Kind == SymbolKind.Field)
            {
                return symbol;
            }

            return null;
        }

        private static SyntaxList<MemberDeclarationSyntax> GetMembers(MemberDeclarationSyntax declaration)
        {
            switch (declaration.Kind())
            {
                case SyntaxKind.ClassDeclaration:
                    return ((ClassDeclarationSyntax)declaration).Members;
                case SyntaxKind.StructDeclaration:
                    return ((StructDeclarationSyntax)declaration).Members;
            }

            return default(SyntaxList<MemberDeclarationSyntax>);
        }

        private static MemberDeclarationSyntax SetMembers(MemberDeclarationSyntax declaration, SyntaxList<MemberDeclarationSyntax> newMembers)
        {
            switch (declaration.Kind())
            {
                case SyntaxKind.ClassDeclaration:
                    return ((ClassDeclarationSyntax)declaration).WithMembers(newMembers);
                case SyntaxKind.StructDeclaration:
                    return ((StructDeclarationSyntax)declaration).WithMembers(newMembers);
            }

            return declaration;
        }

        private static async Task<Document> CreateChangedDocumentAsync(
            Document document,
            MemberDeclarationSyntax declaration,
            List<MemberDeclarationSyntax> declarations,
            CancellationToken cancellationToken)
        {
            SyntaxNode oldRoot = await document.GetSyntaxRootAsync(cancellationToken);

            MemberDeclarationSyntax parentDeclaration = GetContainingDeclaration(declaration);

            SyntaxList<MemberDeclarationSyntax> members = GetMembers(parentDeclaration);

            SyntaxList<MemberDeclarationSyntax> newMembers = members.Insert(
                IndexOfLastConstructorOrField(members) + 1,
                CreateConstructor(GetConstructorIdentifierText(parentDeclaration), declarations));

            MemberDeclarationSyntax newNode = SetMembers(parentDeclaration, newMembers)
                .WithAdditionalAnnotations(Formatter.Annotation);

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

                string parameterName = NamingHelper.RemoveUnderscoreFromIdentifier(name);
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
            AccessorDeclarationSyntax getter = propertyDeclaration.Getter();

            if (getter != null)
            {
                if (getter.Body != null)
                {
                    var returnStatement = (ReturnStatementSyntax)getter.Body.Statements[0];

                    return ((IdentifierNameSyntax)returnStatement.Expression).Identifier;
                }
            }
            else
            {
                ExpressionSyntax expression = propertyDeclaration.ExpressionBody?.Expression;

                if (expression?.IsKind(SyntaxKind.IdentifierName) == true)
                    return ((IdentifierNameSyntax)expression).Identifier;
            }

            return propertyDeclaration.Identifier;
        }
    }
}
