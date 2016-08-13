// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static Pihrtsoft.CodeAnalysis.CSharp.CSharpFactory;

namespace Pihrtsoft.CodeAnalysis.CSharp.Refactoring.IntroduceAndInitialize
{
    internal abstract class IntroduceAndInitializeRefactoring
    {
        public IntroduceAndInitializeRefactoring(ParameterSyntax parameter)
        {
            Parameter = parameter;
            Identifier = parameter.Identifier;
            ParameterName = Identifier.ValueText;
        }

        public ParameterSyntax Parameter { get; }
        public string ParameterName { get; }
        public SyntaxToken Identifier { get; }

        public abstract string Name { get; }

        public ConstructorDeclarationSyntax Constructor
        {
            get { return Parameter.Parent?.Parent as ConstructorDeclarationSyntax; }
        }

        public TypeSyntax Type
        {
            get { return Parameter.Type?.WithoutTrivia(); }
        }

        protected abstract MemberDeclarationSyntax CreateDeclaration();

        protected abstract int GetDeclarationIndex(SyntaxList<MemberDeclarationSyntax> members);

        protected virtual ExpressionStatementSyntax CreateAssignment()
        {
            AssignmentExpressionSyntax assignment = SimpleAssignmentExpression(
                CreateAssignmentLeft(),
                IdentifierName(ParameterName));

            return ExpressionStatement(assignment);
        }

        private ExpressionSyntax CreateAssignmentLeft()
        {
            if (string.Equals(Name, ParameterName, StringComparison.Ordinal))
                return SimpleMemberAccessExpression(ThisExpression(), IdentifierName(Name));

            return IdentifierName(Name);
        }

        public static bool IsValid(ParameterSyntax parameter)
        {
            if (parameter.Type != null
                && !parameter.Identifier.IsMissing
                && parameter.Parent?.IsKind(SyntaxKind.ParameterList) == true)
            {
                SyntaxNode parent = parameter.Parent;

                if (parent.Parent?.IsKind(SyntaxKind.ConstructorDeclaration) == true)
                    return true;
            }

            return false;
        }

        public static void ComputeRefactoring(RefactoringContext context, ParameterSyntax parameter)
        {
            if (IsValid(parameter)
                && parameter.Identifier.Span.Contains(context.Span))
            {
                if (context.Settings.IsRefactoringEnabled(RefactoringIdentifiers.IntroduceAndInitializeProperty))
                {
                    var propertyRefactoring = new IntroduceAndInitializePropertyRefactoring(parameter);

                    context.RegisterRefactoring(
                        $"Introduce and initialize property '{propertyRefactoring.Name}'",
                        cancellationToken => propertyRefactoring.RefactorAsync(context.Document, cancellationToken));
                }

                if (context.Settings.IsRefactoringEnabled(RefactoringIdentifiers.IntroduceAndInitializeField))
                {
                    var fieldRefactoring = new IntroduceAndInitializeFieldRefactoring(parameter, context.Settings.PrefixFieldIdentifierWithUnderscore);

                    context.RegisterRefactoring(
                        $"Introduce and initialize field '{fieldRefactoring.Name}'",
                        cancellationToken => fieldRefactoring.RefactorAsync(context.Document, cancellationToken));
                }
            }
        }

        private async Task<Document> RefactorAsync(
            Document document,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            SyntaxNode root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);

            ConstructorDeclarationSyntax constructor = Constructor;

            MemberDeclarationSyntax containingMember = constructor.GetContainingMember();

            SyntaxList<MemberDeclarationSyntax> members = containingMember.GetMembers();

            SyntaxList<MemberDeclarationSyntax> newMembers = members.Replace(
                constructor,
                constructor.AddBodyStatements(CreateAssignment()));

            newMembers = newMembers.Insert(
                GetDeclarationIndex(members),
                CreateDeclaration());

            SyntaxNode newRoot = root.ReplaceNode(
                containingMember,
                containingMember.SetMembers(newMembers));

            return document.WithSyntaxRoot(newRoot);
        }
    }
}
