// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Pihrtsoft.CodeAnalysis.CSharp.CodeFixProviders
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(LockStatementCodeFixProvider))]
    [Shared]
    public class LockStatementCodeFixProvider : CodeFixProvider
    {
        private const string LockObjectName = "_lockObject";

        public sealed override ImmutableArray<string> FixableDiagnosticIds
            => ImmutableArray.Create(DiagnosticIdentifiers.AvoidLockingOnPubliclyAccessibleInstance);

        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            SyntaxNode root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);

            LockStatementSyntax lockStatement = root
                .FindNode(context.Span, getInnermostNodeForTie: true)?
                .FirstAncestorOrSelf<LockStatementSyntax>();

            if (lockStatement == null)
                return;

            CodeAction codeAction = CodeAction.Create(
                "Introduce field to lock on",
                cancellationToken => IntroduceFieldToLockOnAsync(context.Document, lockStatement, cancellationToken),
                DiagnosticIdentifiers.AvoidLockingOnPubliclyAccessibleInstance + BaseCodeFixProvider.EquivalenceKeySuffix);

            context.RegisterCodeFix(codeAction, context.Diagnostics);
        }

        private static async Task<Document> IntroduceFieldToLockOnAsync(
            Document document,
            LockStatementSyntax lockStatement,
            CancellationToken cancellationToken)
        {
            SyntaxNode oldRoot = await document.GetSyntaxRootAsync(cancellationToken);

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

                    LockStatementSyntax newLockStatement = lockStatement
                        .WithExpression(IdentifierName(LockObjectName));

                    MemberDeclarationSyntax newContainingMember = containingMember
                        .ReplaceNode(lockStatement, newLockStatement);

                    FieldDeclarationSyntax field = CreateField()
                        .WithFormatterAnnotation();

                    SyntaxList<MemberDeclarationSyntax> newMembers = members
                        .Replace(members[index], newContainingMember)
                        .Insert(FindField(members, index) + 1, field);

                    MemberDeclarationSyntax newNode = containingDeclaration.SetMembers(newMembers);

                    SyntaxNode newRoot = oldRoot.ReplaceNode(containingDeclaration, newNode);

                    return document.WithSyntaxRoot(newRoot);
                }
            }

            return document;
        }

        private static int FindField(SyntaxList<MemberDeclarationSyntax> members, int index)
        {
            for (int i = index; i >= 0; i--)
            {
                if (members[i].IsKind(SyntaxKind.FieldDeclaration))
                    return i;
            }

            return -1;
        }

        private static FieldDeclarationSyntax CreateField()
        {
            return FieldDeclaration(
                List<AttributeListSyntax>(),
                TokenList(
                    Token(SyntaxKind.PrivateKeyword),
                    Token(SyntaxKind.StaticKeyword),
                    Token(SyntaxKind.ReadOnlyKeyword)),
                VariableDeclaration(
                    PredefinedType(Token(SyntaxKind.ObjectKeyword)),
                    SingletonSeparatedList(
                        VariableDeclarator(
                            Identifier(LockObjectName),
                            null,
                            EqualsValueClause(
                                ObjectCreationExpression(
                                    PredefinedType(Token(SyntaxKind.ObjectKeyword)),
                                    ArgumentList(),
                                    null))))));
        }
    }
}
