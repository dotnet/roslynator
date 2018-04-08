// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.CSharp.Syntax;

namespace Roslynator.CSharp.Refactorings.IntroduceAndInitialize
{
    internal abstract class IntroduceAndInitializeRefactoring
    {
        protected IntroduceAndInitializeRefactoring(IntroduceAndInitializeInfo info)
        {
            Infos = ImmutableArray.Create(info);
        }

        protected IntroduceAndInitializeRefactoring(IEnumerable<IntroduceAndInitializeInfo> infos)
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
            if (!parameter.Identifier.Span.Contains(context.Span))
                return;

            if (!IsValid(parameter))
                return;

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

        public static void ComputeRefactoring(RefactoringContext context, ParameterListSyntax parameterList)
        {
            if (!SeparatedSyntaxListSelection<ParameterSyntax>.TryCreate(parameterList.Parameters, context.Span, out SeparatedSyntaxListSelection<ParameterSyntax> selection))
                return;

            ImmutableArray<ParameterSyntax> parameters = selection
                .Where(IsValid)
                .ToImmutableArray();

            if (!parameters.Any())
                return;

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
            ConstructorDeclarationSyntax constructor = Constructor;

            MemberDeclarationListInfo info = SyntaxInfo.MemberDeclarationListInfo(constructor.Parent);

            SyntaxList<MemberDeclarationSyntax> members = info.Members;

            SyntaxList<MemberDeclarationSyntax> newMembers = members.Replace(
                constructor,
                constructor.AddBodyStatements(CreateAssignments().ToArray()));

            SemanticModel semanticModel = await document.GetSemanticModelAsync(cancellationToken).ConfigureAwait(false);

            newMembers = newMembers.InsertRange(
                GetDeclarationIndex(members),
                CreateDeclarations(constructor, semanticModel, cancellationToken));

            return await document.ReplaceMembersAsync(info, newMembers, cancellationToken).ConfigureAwait(false);
        }

        private IEnumerable<ExpressionStatementSyntax> CreateAssignments()
        {
            return Infos
                .Select(f => f.CreateAssignment());
        }

        private IEnumerable<MemberDeclarationSyntax> CreateDeclarations(ConstructorDeclarationSyntax constructor, SemanticModel semanticModel, CancellationToken cancellationToken)
        {
            IMethodSymbol methodSymbol = semanticModel.GetDeclaredSymbol(constructor, cancellationToken);

            if (methodSymbol != null)
            {
                INamedTypeSymbol containingType = methodSymbol.ContainingType;

                if (containingType != null)
                {
                    ImmutableArray<ISymbol> members = containingType.GetMembers();

                    return Infos
                        .Where(f => NameGenerator.IsUniqueName(f.Name, members, isCaseSensitive: true))
                        .Select(f => f.CreateDeclaration());
                }
            }

            return Infos.Select(f => f.CreateDeclaration());
        }

        private static bool IsValid(ParameterSyntax parameter)
        {
            if (parameter.Type == null)
                return false;

            if (parameter.Identifier.IsMissing)
                return false;

            SyntaxNode parent = parameter.Parent;

            if (parent?.Kind() != SyntaxKind.ParameterList)
                return false;

            parent = parent.Parent;

            return parent?.Kind() == SyntaxKind.ConstructorDeclaration
                && !((ConstructorDeclarationSyntax)parent).Modifiers.Contains(SyntaxKind.StaticKeyword);
        }
    }
}
