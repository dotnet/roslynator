// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static Roslynator.CSharp.CSharpFactory;

namespace Roslynator.CSharp.Refactorings
{
    internal static class NotifyWhenPropertyChangeRefactoring
    {
        public static async Task ComputeRefactoringAsync(
            RefactoringContext context,
            PropertyDeclarationSyntax property)
        {
            AccessorDeclarationSyntax setter = property.Setter();

            if (setter == null)
                return;

            ExpressionSyntax expression = GetExpression();

            if (expression == null)
                return;

            SimpleAssignmentExpressionInfo simpleAssignment = SyntaxInfo.SimpleAssignmentExpressionInfo(expression);

            if (!simpleAssignment.Success)
                return;

            if (!simpleAssignment.Left.IsKind(SyntaxKind.IdentifierName))
                return;

            if (!(simpleAssignment.Right is IdentifierNameSyntax identifierName))
                return;

            if (identifierName.Identifier.ValueText != "value")
                return;

            SemanticModel semanticModel = await context.GetSemanticModelAsync().ConfigureAwait(false);

            INamedTypeSymbol containingType = semanticModel
                .GetDeclaredSymbol(property, context.CancellationToken)?
                .ContainingType;

            if (containingType == null)
                return;

            if (!containingType.Implements(MetadataNames.System_ComponentModel_INotifyPropertyChanged, allInterfaces: true))
                return;

            IMethodSymbol methodSymbol = SymbolUtility.FindMethodThatRaisePropertyChanged(containingType, expression.SpanStart, semanticModel);
            IEventSymbol propertyChangedEventSymbol = containingType.FindMember<IEventSymbol>("PropertyChanged");

            if (methodSymbol == null && propertyChangedEventSymbol == null)
                return;

            Document document = context.Document;

            context.RegisterRefactoring(
                "Notify when property change",
                ct => RefactorAsync(document, property, methodSymbol, containingType, ct),
                RefactoringIdentifiers.NotifyWhenPropertyChange);

            ExpressionSyntax GetExpression()
            {
                BlockSyntax body = setter.Body;

                if (body != null)
                {
                    if (body.Statements.SingleOrDefault(shouldThrow: false) is ExpressionStatementSyntax expressionStatement)
                        return expressionStatement.Expression;
                }
                else
                {
                    return setter.ExpressionBody?.Expression;
                }

                return null;
            }
        }

        private static async Task<Document> RefactorAsync(
            Document document,
            PropertyDeclarationSyntax property,
            IMethodSymbol method,
            INamedTypeSymbol containingType,
            CancellationToken cancellationToken = default)
        {
            AccessorDeclarationSyntax setter = property.Setter();

            string propertyName = property.Identifier.ValueText;

            ArgumentListSyntax argumentList;
            if(method?.Parameters[0].HasAttribute(MetadataNames.System_Runtime_CompilerServices_CallerMemberNameAttribute) != false)
            {
                argumentList = ArgumentList();
            }
            else if (document.SupportsLanguageFeature(CSharpLanguageFeature.NameOf))
            {
                argumentList = ArgumentList(Argument(NameOfExpression(propertyName)));
            }
            else
            {
                argumentList = ArgumentList(Argument(StringLiteralExpression(propertyName)));
            }

            IdentifierNameSyntax backingFieldName = GetBackingFieldIdentifierName(setter).WithoutTrivia();

            AccessorDeclarationSyntax newSetter = SetAccessorDeclaration(
                Block(
                    IfStatement(
                        NotEqualsExpression(
                            backingFieldName,
                            IdentifierName("value")),
                        Block(
                            SimpleAssignmentStatement(
                                backingFieldName,
                                IdentifierName("value")),
                            ExpressionStatement(
                                InvocationExpression(
                                    IdentifierName(method?.Name ?? "OnPropertyChanged"),
                                    argumentList))))));

            newSetter = newSetter
                .WithTriviaFrom(property)
                .WithFormatterAnnotation();

            Document newDocument = await document.ReplaceNodeAsync(setter, newSetter, cancellationToken).ConfigureAwait(false);

            if (method == null)
            {
                SyntaxNode syntaxRoot = await newDocument.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);

                SyntaxNode propertyChangedEventDeclaration =
                    syntaxRoot.DescendantNodes()
                    .OfType<ClassDeclarationSyntax>()
                    .Single(c => GetFullName(c) == containingType.ToString())
                    .ChildNodes()
                    .OfType<EventFieldDeclarationSyntax>()
                    .Single(node => node.DescendantNodes().OfType<VariableDeclaratorSyntax>().SingleOrDefault()?.Identifier.ValueText == "PropertyChanged");

                MethodDeclarationSyntax onPropertyChangedDeclaration = MethodDeclaration(
                    Modifiers.Protected(),
                    VoidType(),
                    Identifier("OnPropertyChanged"),
                    ParameterList(
                        Parameter(
                            SingletonList(AttributeList(Attribute(IdentifierName("CallerMemberName")))),
                            default,
                            PredefinedStringType(),
                            Identifier("propertyName"),
                            EqualsValueClause(LiteralExpression(null)))),
                    Block(
                        ExpressionStatement(
                            ConditionalAccessExpression(
                                IdentifierName("PropertyChanged"),
                                InvocationExpression(
                                    MemberBindingExpression(
                                        IdentifierName("Invoke")),
                                    ArgumentList(
                                        Argument(
                                            ThisExpression()),
                                        Argument(
                                            ObjectCreationExpression(
                                                    IdentifierName("PropertyChangedEventArgs"),
                                                    ArgumentList(Argument(IdentifierName("propertyName")))))))))));

                newDocument = await newDocument.InsertNodeAfterAsync(propertyChangedEventDeclaration, onPropertyChangedDeclaration, cancellationToken).ConfigureAwait(false);

                var compilationUnitSyntax = await newDocument.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false) as CompilationUnitSyntax;
                NameSyntax compilerServicesName = ParseName("System.Runtime.CompilerServices");
                if(!compilationUnitSyntax.Usings.Any(u => u.Name.ToString() == compilerServicesName.ToString()))
                {
                    CompilationUnitSyntax newCompilationUnitSyntax = compilationUnitSyntax.AddUsings(UsingDirective(compilerServicesName));
                    newDocument = await newDocument.ReplaceNodeAsync(compilationUnitSyntax, newCompilationUnitSyntax, cancellationToken).ConfigureAwait(false);
                }
            }

            return newDocument;
        }

        public static IdentifierNameSyntax GetBackingFieldIdentifierName(AccessorDeclarationSyntax accessor)
        {
            BlockSyntax body = accessor.Body;

            if (body != null)
            {
                var expressionStatement = (ExpressionStatementSyntax)body.Statements[0];

                var assignment = (AssignmentExpressionSyntax)expressionStatement.Expression;

                return (IdentifierNameSyntax)assignment.Left;
            }
            else
            {
                ArrowExpressionClauseSyntax expressionBody = accessor.ExpressionBody;

                var assignment = (AssignmentExpressionSyntax)expressionBody.Expression;

                return (IdentifierNameSyntax)assignment.Left;
            }
        }

        private static string GetFullName(ClassDeclarationSyntax @class)
        {
            string fullName = @class.Identifier.ValueText;
            SyntaxNode node = @class;
            while(true)
            {
                switch (node.Parent)
                {
                    case NamespaceDeclarationSyntax @namespace:
                        {
                            fullName = $"{@namespace.Name}.{fullName}";
                            break;
                        }
                    case ClassDeclarationSyntax outerClass:
                        {
                            fullName = $"{outerClass.Identifier.ValueText}.{fullName}";
                            break;
                        }
                    default:
                        return fullName;
                }

                node = node.Parent;
            }
        }
    }
}
