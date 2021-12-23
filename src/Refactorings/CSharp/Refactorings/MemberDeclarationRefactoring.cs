// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Roslynator.CSharp.Refactorings
{
    internal static class MemberDeclarationRefactoring
    {
        public static async Task ComputeRefactoringsAsync(RefactoringContext context, MemberDeclarationSyntax member)
        {
            SyntaxKind kind = member.Kind();

            switch (kind)
            {
                case SyntaxKind.MethodDeclaration:
                case SyntaxKind.IndexerDeclaration:
                case SyntaxKind.PropertyDeclaration:
                case SyntaxKind.OperatorDeclaration:
                case SyntaxKind.ConversionOperatorDeclaration:
                case SyntaxKind.ConstructorDeclaration:
                case SyntaxKind.EventDeclaration:
                case SyntaxKind.NamespaceDeclaration:
                case SyntaxKind.ClassDeclaration:
                case SyntaxKind.RecordDeclaration:
                case SyntaxKind.StructDeclaration:
                case SyntaxKind.RecordStructDeclaration:
                case SyntaxKind.InterfaceDeclaration:
                case SyntaxKind.EnumDeclaration:
                    {
                        if (context.IsAnyRefactoringEnabled(
                            RefactoringDescriptors.RemoveMemberDeclaration,
                            RefactoringDescriptors.CopyMemberDeclaration,
                            RefactoringDescriptors.CommentOutMemberDeclaration))
                        {
                            (SyntaxToken openBrace, SyntaxToken closeBrace) = GetBraces(member);

                            if ((!openBrace.IsKind(SyntaxKind.None)
                                && openBrace.Span.Contains(context.Span))
                                || (!closeBrace.IsKind(SyntaxKind.None)
                                    && closeBrace.Span.Contains(context.Span)))
                            {
                                if (member.Parent != null
                                    && CSharpFacts.CanHaveMembers(member.Parent.Kind()))
                                {
                                    if (context.IsRefactoringEnabled(RefactoringDescriptors.RemoveMemberDeclaration))
                                    {
                                        context.RegisterRefactoring(CodeActionFactory.RemoveMemberDeclaration(context.Document, member, equivalenceKey: EquivalenceKey.Create(RefactoringDescriptors.RemoveMemberDeclaration)));
                                    }

                                    if (context.IsRefactoringEnabled(RefactoringDescriptors.CopyMemberDeclaration))
                                    {
                                        context.RegisterRefactoring(
                                            $"Copy {CSharpFacts.GetTitle(member)}",
                                            ct => CopyMemberDeclarationRefactoring.RefactorAsync(
                                                context.Document,
                                                member,
                                                copyAfter: closeBrace.Span.Contains(context.Span),
                                                ct),
                                            RefactoringDescriptors.CopyMemberDeclaration);
                                    }
                                }

                                if (context.IsRefactoringEnabled(RefactoringDescriptors.CommentOutMemberDeclaration))
                                    CommentOutRefactoring.RegisterRefactoring(context, member);
                            }
                        }

                        break;
                    }
            }

            if (context.IsRefactoringEnabled(RefactoringDescriptors.RemoveAllStatements))
                RemoveAllStatementsRefactoring.ComputeRefactoring(context, member);

            if (context.IsRefactoringEnabled(RefactoringDescriptors.RemoveAllMemberDeclarations))
                RemoveAllMemberDeclarationsRefactoring.ComputeRefactoring(context, member);

            if (context.IsAnyRefactoringEnabled(
                RefactoringDescriptors.SwapMemberDeclarations,
                RefactoringDescriptors.RemoveMemberDeclarations)
                && !member.Span.IntersectsWith(context.Span))
            {
                MemberDeclarationsRefactoring.ComputeRefactoring(context, member);
            }

            switch (kind)
            {
                case SyntaxKind.NamespaceDeclaration:
                    {
                        var namespaceDeclaration = (NamespaceDeclarationSyntax)member;
                        NamespaceDeclarationRefactoring.ComputeRefactorings(context, namespaceDeclaration);

                        if (MemberDeclarationListSelection.TryCreate(namespaceDeclaration, context.Span, out MemberDeclarationListSelection selectedMembers))
                            await SelectedMemberDeclarationsRefactoring.ComputeRefactoringAsync(context, selectedMembers).ConfigureAwait(false);

                        break;
                    }
                case SyntaxKind.ClassDeclaration:
                    {
                        var classDeclaration = (ClassDeclarationSyntax)member;
                        await ClassDeclarationRefactoring.ComputeRefactoringsAsync(context, classDeclaration).ConfigureAwait(false);

                        if (MemberDeclarationListSelection.TryCreate(classDeclaration, context.Span, out MemberDeclarationListSelection selectedMembers))
                            await SelectedMemberDeclarationsRefactoring.ComputeRefactoringAsync(context, selectedMembers).ConfigureAwait(false);

                        break;
                    }
                case SyntaxKind.RecordDeclaration:
                case SyntaxKind.RecordStructDeclaration:
                    {
                        var recordDeclaration = (RecordDeclarationSyntax)member;
                        await RecordDeclarationRefactoring.ComputeRefactoringsAsync(context, recordDeclaration).ConfigureAwait(false);

                        if (MemberDeclarationListSelection.TryCreate(recordDeclaration, context.Span, out MemberDeclarationListSelection selectedMembers))
                            await SelectedMemberDeclarationsRefactoring.ComputeRefactoringAsync(context, selectedMembers).ConfigureAwait(false);

                        break;
                    }
                case SyntaxKind.StructDeclaration:
                    {
                        var structDeclaration = (StructDeclarationSyntax)member;
                        await StructDeclarationRefactoring.ComputeRefactoringsAsync(context, structDeclaration).ConfigureAwait(false);

                        if (MemberDeclarationListSelection.TryCreate(structDeclaration, context.Span, out MemberDeclarationListSelection selectedMembers))
                            await SelectedMemberDeclarationsRefactoring.ComputeRefactoringAsync(context, selectedMembers).ConfigureAwait(false);

                        break;
                    }
                case SyntaxKind.InterfaceDeclaration:
                    {
                        var interfaceDeclaration = (InterfaceDeclarationSyntax)member;
                        InterfaceDeclarationRefactoring.ComputeRefactorings(context, interfaceDeclaration);

                        if (MemberDeclarationListSelection.TryCreate(interfaceDeclaration, context.Span, out MemberDeclarationListSelection selectedMembers))
                            await SelectedMemberDeclarationsRefactoring.ComputeRefactoringAsync(context, selectedMembers).ConfigureAwait(false);

                        break;
                    }
                case SyntaxKind.EnumDeclaration:
                    {
                        await EnumDeclarationRefactoring.ComputeRefactoringAsync(context, (EnumDeclarationSyntax)member).ConfigureAwait(false);
                        break;
                    }
                case SyntaxKind.EnumMemberDeclaration:
                    {
                        await EnumMemberDeclarationRefactoring.ComputeRefactoringAsync(context, (EnumMemberDeclarationSyntax)member).ConfigureAwait(false);
                        break;
                    }
                case SyntaxKind.DelegateDeclaration:
                    {
                        DelegateDeclarationRefactoring.ComputeRefactorings(context, (DelegateDeclarationSyntax)member);
                        break;
                    }
                case SyntaxKind.MethodDeclaration:
                    {
                        await MethodDeclarationRefactoring.ComputeRefactoringsAsync(context, (MethodDeclarationSyntax)member).ConfigureAwait(false);
                        break;
                    }
                case SyntaxKind.ConstructorDeclaration:
                    {
                        await ConstructorDeclarationRefactoring.ComputeRefactoringsAsync(context, (ConstructorDeclarationSyntax)member).ConfigureAwait(false);
                        break;
                    }
                case SyntaxKind.DestructorDeclaration:
                    {
                        DestructorDeclarationRefactoring.ComputeRefactorings(context, (DestructorDeclarationSyntax)member);
                        break;
                    }
                case SyntaxKind.IndexerDeclaration:
                    {
                        await IndexerDeclarationRefactoring.ComputeRefactoringsAsync(context, (IndexerDeclarationSyntax)member).ConfigureAwait(false);
                        break;
                    }
                case SyntaxKind.PropertyDeclaration:
                    {
                        await PropertyDeclarationRefactoring.ComputeRefactoringsAsync(context, (PropertyDeclarationSyntax)member).ConfigureAwait(false);
                        break;
                    }
                case SyntaxKind.OperatorDeclaration:
                    {
                        ComputeRefactorings(context, (OperatorDeclarationSyntax)member);
                        break;
                    }
                case SyntaxKind.ConversionOperatorDeclaration:
                    {
                        ComputeRefactorings(context, (ConversionOperatorDeclarationSyntax)member);
                        break;
                    }
                case SyntaxKind.FieldDeclaration:
                    {
                        await FieldDeclarationRefactoring.ComputeRefactoringsAsync(context, (FieldDeclarationSyntax)member).ConfigureAwait(false);
                        break;
                    }
                case SyntaxKind.EventDeclaration:
                    {
                        await EventDeclarationRefactoring.ComputeRefactoringsAsync(context, (EventDeclarationSyntax)member).ConfigureAwait(false);
                        break;
                    }
                case SyntaxKind.EventFieldDeclaration:
                    {
                        await EventFieldDeclarationRefactoring.ComputeRefactoringsAsync(context, (EventFieldDeclarationSyntax)member).ConfigureAwait(false);
                        break;
                    }
            }

            if (context.IsRefactoringEnabled(RefactoringDescriptors.MoveUnsafeContextToContainingDeclaration))
                MoveUnsafeContextToContainingDeclarationRefactoring.ComputeRefactoring(context, member);
        }

        private static void ComputeRefactorings(RefactoringContext context, OperatorDeclarationSyntax operatorDeclaration)
        {
            if (context.IsRefactoringEnabled(RefactoringDescriptors.ConvertBlockBodyToExpressionBody)
                && context.SupportsCSharp6
                && ConvertBlockBodyToExpressionBodyRefactoring.CanRefactor(operatorDeclaration, context.Span))
            {
                context.RegisterRefactoring(
                    ConvertBlockBodyToExpressionBodyRefactoring.Title,
                    ct => ConvertBlockBodyToExpressionBodyRefactoring.RefactorAsync(context.Document, operatorDeclaration, ct),
                    RefactoringDescriptors.ConvertBlockBodyToExpressionBody);
            }
        }

        private static void ComputeRefactorings(RefactoringContext context, ConversionOperatorDeclarationSyntax operatorDeclaration)
        {
            if (context.IsRefactoringEnabled(RefactoringDescriptors.ConvertBlockBodyToExpressionBody)
                && context.SupportsCSharp6
                && ConvertBlockBodyToExpressionBodyRefactoring.CanRefactor(operatorDeclaration, context.Span))
            {
                context.RegisterRefactoring(
                    ConvertBlockBodyToExpressionBodyRefactoring.Title,
                    ct => ConvertBlockBodyToExpressionBodyRefactoring.RefactorAsync(context.Document, operatorDeclaration, ct),
                    RefactoringDescriptors.ConvertBlockBodyToExpressionBody);
            }
        }

        private static (SyntaxToken openBrace, SyntaxToken closeBrace) GetBraces(MemberDeclarationSyntax member)
        {
            switch (member.Kind())
            {
                case SyntaxKind.MethodDeclaration:
                    {
                        return FromBody(((MethodDeclarationSyntax)member).Body);
                    }
                case SyntaxKind.IndexerDeclaration:
                    {
                        return FromAccessorList(((IndexerDeclarationSyntax)member).AccessorList);
                    }
                case SyntaxKind.OperatorDeclaration:
                    {
                        return FromBody(((OperatorDeclarationSyntax)member).Body);
                    }
                case SyntaxKind.ConversionOperatorDeclaration:
                    {
                        return FromBody(((ConversionOperatorDeclarationSyntax)member).Body);
                    }
                case SyntaxKind.ConstructorDeclaration:
                    {
                        return FromBody(((ConstructorDeclarationSyntax)member).Body);
                    }
                case SyntaxKind.PropertyDeclaration:
                    {
                        return FromAccessorList(((PropertyDeclarationSyntax)member).AccessorList);
                    }
                case SyntaxKind.EventDeclaration:
                    {
                        return FromAccessorList(((EventDeclarationSyntax)member).AccessorList);
                    }
                case SyntaxKind.NamespaceDeclaration:
                    {
                        var member1 = ((NamespaceDeclarationSyntax)member);
                        return (member1.OpenBraceToken, member1.CloseBraceToken);
                    }
                case SyntaxKind.ClassDeclaration:
                    {
                        var member2 = ((ClassDeclarationSyntax)member);
                        return (member2.OpenBraceToken, member2.CloseBraceToken);
                    }
                case SyntaxKind.RecordDeclaration:
                case SyntaxKind.RecordStructDeclaration:
                    {
                        var member3 = ((RecordDeclarationSyntax)member);
                        return (member3.OpenBraceToken, member3.CloseBraceToken);
                    }
                case SyntaxKind.StructDeclaration:
                    {
                        var member4 = ((StructDeclarationSyntax)member);
                        return (member4.OpenBraceToken, member4.CloseBraceToken);
                    }
                case SyntaxKind.InterfaceDeclaration:
                    {
                        var member5 = ((InterfaceDeclarationSyntax)member);
                        return (member5.OpenBraceToken, member5.CloseBraceToken);
                    }
                case SyntaxKind.EnumDeclaration:
                    {
                        var member6 = ((EnumDeclarationSyntax)member);
                        return (member6.OpenBraceToken, member6.CloseBraceToken);
                    }
            }

            return default;

            (SyntaxToken openBrace, SyntaxToken closeBrace) FromBody(BlockSyntax body)
            {
                return (body != null)
                    ? (body.OpenBraceToken, body.CloseBraceToken)
                    : default;
            }

            (SyntaxToken openBrace, SyntaxToken closeBrace) FromAccessorList(AccessorListSyntax accessorList)
            {
                return (accessorList != null)
                    ? (accessorList.OpenBraceToken, accessorList.CloseBraceToken)
                    : default;
            }
        }
    }
}
