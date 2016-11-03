// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace Roslynator.CSharp.Refactorings.IntroduceAndInitialize
{
    internal abstract class IntroduceAndInitializeRefactoring
    {
        public IntroduceAndInitializeRefactoring(IntroduceAndInitializeInfo info)
        {
            Infos = ImmutableArray.Create(info);
        }

        public IntroduceAndInitializeRefactoring(IEnumerable<IntroduceAndInitializeInfo> infos)
        {
            Infos = infos.ToImmutableArray();
        }

        public ImmutableArray<IntroduceAndInitializeInfo> Infos { get; }

        public IntroduceAndInitializeInfo FirstInfo
        {
            get { return Infos[0]; }
        }

        public ConstructorDeclarationSyntax Constructor
        {
            get { return FirstInfo.Parameter.Parent?.Parent as ConstructorDeclarationSyntax; }
        }

        protected abstract int GetDeclarationIndex(SyntaxList<MemberDeclarationSyntax> members);

        protected abstract string GetTitle();

        public static void ComputeRefactoring(RefactoringContext context, ParameterSyntax parameter)
        {
            if (parameter.Identifier.Span.Contains(context.Span)
                && IsValid(parameter))
            {
                if (context.IsRefactoringEnabled(RefactoringIdentifiers.IntroduceAndInitializeProperty))
                {
                    var propertyInfo = new IntroduceAndInitializePropertyInfo(parameter, context.SupportsCSharp6);
                    var refactoring = new IntroduceAndInitializePropertyRefactoring(propertyInfo);
                    refactoring.RegisterRefactoring(context);
                }

                if (context.IsRefactoringEnabled(RefactoringIdentifiers.IntroduceAndInitializeField))
                {
                    var fieldInfo = new IntroduceAndInitializeFieldInfo(parameter, context.Settings.PrefixFieldIdentifierWithUnderscore);
                    var refactoring = new IntroduceAndInitializeFieldRefactoring(fieldInfo);
                    refactoring.RegisterRefactoring(context);
                }
            }
        }

        public static void ComputeRefactoring(RefactoringContext context, ParameterListSyntax parameterList)
        {
            ImmutableArray<ParameterSyntax> parameters = GetParameters(parameterList, context.Span);

            if (parameters.Any())
            {
                if (context.IsRefactoringEnabled(RefactoringIdentifiers.IntroduceAndInitializeProperty))
                {
                    IEnumerable<IntroduceAndInitializePropertyInfo> propertyInfos = parameters
                        .Select(parameter => new IntroduceAndInitializePropertyInfo(parameter, context.SupportsCSharp6));

                    var refactoring = new IntroduceAndInitializePropertyRefactoring(propertyInfos);
                    refactoring.RegisterRefactoring(context);
                }

                if (context.IsRefactoringEnabled(RefactoringIdentifiers.IntroduceAndInitializeField))
                {
                    IEnumerable<IntroduceAndInitializeFieldInfo> fieldInfos = parameters
                        .Select(parameter => new IntroduceAndInitializeFieldInfo(parameter, context.Settings.PrefixFieldIdentifierWithUnderscore));

                    var refactoring = new IntroduceAndInitializeFieldRefactoring(fieldInfos);
                    refactoring.RegisterRefactoring(context);
                }
            }
        }

        private static ImmutableArray<ParameterSyntax> GetParameters(ParameterListSyntax parameterList, TextSpan span)
        {
            return GetSelectedParameters(parameterList, span)
                .Where(f => IsValid(f))
                .ToImmutableArray();
        }

        private void RegisterRefactoring(RefactoringContext context)
        {
            context.RegisterRefactoring(
                GetTitle(),
                cancellationToken => RefactorAsync(context.Document, cancellationToken));
        }

        protected string GetNames()
        {
            return string.Join(", ", Infos.Select(f => $"'{f.Name}'"));
        }

        private async Task<Document> RefactorAsync(
            Document document,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            SyntaxNode root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);

            ConstructorDeclarationSyntax constructor = Constructor;

            MemberDeclarationSyntax containingMember = constructor.GetParentMember();

            SyntaxList<MemberDeclarationSyntax> members = containingMember.GetMembers();

            SyntaxList<MemberDeclarationSyntax> newMembers = members.Replace(
                constructor,
                constructor.AddBodyStatements(CreateAssignments().ToArray()));

            newMembers = newMembers.InsertRange(
                GetDeclarationIndex(members),
                CreateDeclarations());

            SyntaxNode newRoot = root.ReplaceNode(
                containingMember,
                containingMember.SetMembers(newMembers));

            return document.WithSyntaxRoot(newRoot);
        }

        private IEnumerable<ExpressionStatementSyntax> CreateAssignments()
        {
            return Infos
                .Select(f => f.CreateAssignment());
        }

        private IEnumerable<MemberDeclarationSyntax> CreateDeclarations()
        {
            return Infos
                .Select(f => f.CreateDeclaration());
        }

        private static IEnumerable<ParameterSyntax> GetSelectedParameters(ParameterListSyntax parameterList, TextSpan span)
        {
            return parameterList.Parameters
                .SkipWhile(f => span.Start > f.Span.Start)
                .TakeWhile(f => span.End >= f.Span.End);
        }

        private static bool IsValid(ParameterSyntax parameter)
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
    }
}
