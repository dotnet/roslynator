// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Immutable;
using System.Composition;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.CodeFixes;

namespace Roslynator.CSharp.CodeFixes
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(ObjectReferenceIsRequiredForNonStaticMemberCodeFixProvider))]
    [Shared]
    public sealed class ObjectReferenceIsRequiredForNonStaticMemberCodeFixProvider : CompilerDiagnosticCodeFixProvider
    {
        public override ImmutableArray<string> FixableDiagnosticIds
        {
            get { return ImmutableArray.Create(CompilerDiagnosticIdentifiers.CS0120_ObjectReferenceIsRequiredForNonStaticMember); }
        }

        public override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            SyntaxNode root = await context.GetSyntaxRootAsync().ConfigureAwait(false);

            if (!TryFindNode(root, context.Span, out SyntaxNode node))
                return;

            Diagnostic diagnostic = context.Diagnostics[0];

            for (SyntaxNode parent = node.Parent; parent != null; parent = parent.Parent)
            {
                if (parent is MemberDeclarationSyntax memberDeclaration)
                {
                    SemanticModel semanticModel = await context.GetSemanticModelAsync().ConfigureAwait(false);

                    ISymbol symbol = semanticModel.GetSymbol(node, context.CancellationToken);

                    if (symbol?.IsErrorType() != false)
                        return;

                    SyntaxDebug.Assert(SyntaxInfo.ModifierListInfo(memberDeclaration).IsStatic, memberDeclaration);

                    if (SyntaxInfo.ModifierListInfo(memberDeclaration).IsStatic)
                    {
                        if (IsEnabled(diagnostic.Id, CodeFixIdentifiers.MakeMemberNonStatic, context.Document, root.SyntaxTree))
                        {
                            ModifiersCodeFixRegistrator.RemoveModifier(
                                context,
                                diagnostic,
                                memberDeclaration,
                                SyntaxKind.StaticKeyword,
                                title: $"Make containing {CSharpFacts.GetTitle(memberDeclaration)} non-static",
                                additionalKey: CodeFixIdentifiers.MakeMemberNonStatic);
                        }

                        if (IsEnabled(diagnostic.Id, CodeFixIdentifiers.AddStaticModifier, context.Document, root.SyntaxTree))
                            AddStaticModifier(context, diagnostic, node, semanticModel);
                    }

                    return;
                }
                else if (parent is ConstructorInitializerSyntax)
                {
                    if (IsEnabled(diagnostic.Id, CodeFixIdentifiers.AddStaticModifier, context.Document, root.SyntaxTree))
                    {
                        SemanticModel semanticModel = await context.GetSemanticModelAsync().ConfigureAwait(false);

                        AddStaticModifier(context, diagnostic, node, semanticModel);
                    }

                    return;
                }
            }
        }

        private static void AddStaticModifier(
            CodeFixContext context,
            Diagnostic diagnostic,
            SyntaxNode node,
            SemanticModel semanticModel)
        {
            ISymbol symbol = semanticModel.GetSymbol(node, context.CancellationToken);

            if (symbol == null)
                return;

            SyntaxNode syntax = symbol.GetSyntaxOrDefault(context.CancellationToken);

            if (syntax == null)
                return;

            if (syntax.IsKind(SyntaxKind.VariableDeclarator))
                syntax = syntax.Parent?.Parent;

            SyntaxDebug.Assert(syntax.IsKind(SyntaxKind.EventDeclaration, SyntaxKind.EventFieldDeclaration, SyntaxKind.FieldDeclaration, SyntaxKind.MethodDeclaration, SyntaxKind.PropertyDeclaration), syntax);

            if (syntax is not MemberDeclarationSyntax memberDeclaration)
                return;

            if (SyntaxInfo.ModifierListInfo(memberDeclaration).IsStatic)
                return;

            Document document = context.Document;

            SyntaxTree tree = memberDeclaration.SyntaxTree;

            if (tree != node.SyntaxTree)
                document = context.Solution().GetDocument(tree);

            ModifiersCodeFixRegistrator.AddModifier(
                context,
                document,
                diagnostic,
                memberDeclaration,
                SyntaxKind.StaticKeyword,
                title: $"Make '{symbol.Name}' static",
                additionalKey: CodeFixIdentifiers.AddStaticModifier);
        }
    }
}
