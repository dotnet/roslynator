// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.CSharp.Comparers;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static Roslynator.CSharp.CSharpFactory;

namespace Roslynator.CSharp.Refactorings
{
    internal static class IntroduceFieldToLockOnRefactoring
    {
        private const string LockObjectName = "_lockObject";

        public static async Task<Document> RefactorAsync(
            Document document,
            LockStatementSyntax lockStatement,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (document == null)
                throw new ArgumentNullException(nameof(document));

            if (lockStatement == null)
                throw new ArgumentNullException(nameof(lockStatement));

            MemberDeclarationSyntax containingMember = lockStatement.FirstAncestorOrSelf<MemberDeclarationSyntax>();

            if (containingMember != null)
            {
                var containingDeclaration = (MemberDeclarationSyntax)containingMember
                    .Ancestors()
                    .FirstOrDefault(f => f.IsKind(
                        SyntaxKind.ClassDeclaration,
                        SyntaxKind.InterfaceDeclaration,
                        SyntaxKind.StructDeclaration));

                if (containingDeclaration != null)
                {
                    SyntaxList<MemberDeclarationSyntax> members = containingDeclaration.GetMembers();

                    int index = members.IndexOf(containingMember);

                    SemanticModel semanticModel = await document.GetSemanticModelAsync(cancellationToken).ConfigureAwait(false);

                    string name = NameGenerator.Default.EnsureUniqueMemberName(
                        LockObjectName,
                        semanticModel,
                        lockStatement.Expression.SpanStart,
                        cancellationToken: cancellationToken);

                    LockStatementSyntax newLockStatement = lockStatement
                        .WithExpression(IdentifierName(Identifier(name).WithRenameAnnotation()));

                    MemberDeclarationSyntax newContainingMember = containingMember
                        .ReplaceNode(lockStatement, newLockStatement);

                    bool isStatic = containingMember.GetModifiers().Contains(SyntaxKind.StaticKeyword);

                    FieldDeclarationSyntax field = CreateFieldDeclaration(name, isStatic).WithFormatterAnnotation();

                    SyntaxList<MemberDeclarationSyntax> newMembers = members.ReplaceAt(index, newContainingMember);

                    newMembers = newMembers.InsertMember(field, MemberDeclarationComparer.ByKind);

                    MemberDeclarationSyntax newNode = containingDeclaration.WithMembers(newMembers);

                    return await document.ReplaceNodeAsync(containingDeclaration, newNode, cancellationToken).ConfigureAwait(false);
                }
            }

            return document;
        }

        private static FieldDeclarationSyntax CreateFieldDeclaration(string name, bool isStatic)
        {
            return FieldDeclaration(
                (isStatic) ? Modifiers.PrivateStaticReadOnly() : Modifiers.PrivateReadOnly(),
                ObjectType(),
                Identifier(name),
                ObjectCreationExpression(ObjectType(), ArgumentList()));
        }
    }
}
