// Copyright (c) .NET Foundation and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Roslynator.CSharp.CodeStyle;

namespace Roslynator.CSharp.Analysis;

internal abstract class ImplicitOrExplicitCreationAnalysis
{
    protected static readonly ImmutableDictionary<string, string> _implicitToCollectionExpression = ImmutableDictionary.CreateRange(new[]
        {
            new KeyValuePair<string, string>(DiagnosticPropertyKeys.ImplicitToCollectionExpression, null)
        });

    protected static readonly ImmutableDictionary<string, string> _collectionExpressionToImplicit = ImmutableDictionary.CreateRange(new[]
        {
            new KeyValuePair<string, string>(DiagnosticPropertyKeys.CollectionExpressionToImplicit, null)
        });

    protected static readonly ImmutableDictionary<string, string> _explicitToCollectionExpression = ImmutableDictionary.CreateRange(new[]
        {
            new KeyValuePair<string, string>(DiagnosticPropertyKeys.ExplicitToCollectionExpression, null)
        });

    public abstract TypeStyle GetTypeStyle(ref SyntaxNodeAnalysisContext context);

    protected abstract void ReportExplicitToImplicit(ref SyntaxNodeAnalysisContext context);

    protected abstract void ReportImplicitToExplicit(ref SyntaxNodeAnalysisContext context);

#if ROSLYN_4_7
    protected abstract void ReportExplicitToCollectionExpression(ref SyntaxNodeAnalysisContext context);

    protected abstract void ReportImplicitToCollectionExpression(ref SyntaxNodeAnalysisContext context);

    protected abstract void ReportCollectionExpressionToImplicit(ref SyntaxNodeAnalysisContext context);
#endif

    protected virtual bool IsInitializerObvious(ref SyntaxNodeAnalysisContext context) => false;

    public virtual void AnalyzeExplicitCreation(ref SyntaxNodeAnalysisContext context)
    {
        if (context.Node.ContainsDiagnostics)
            return;

        TypeStyle style = GetTypeStyle(ref context);

        if (style != TypeStyle.Implicit
            && style != TypeStyle.ImplicitWhenTypeIsObvious)
        {
            return;
        }

        var expression = (ExpressionSyntax)context.Node;

        SyntaxNode parent = expression.Parent;

        switch (parent.Kind())
        {
            case SyntaxKind.ThrowExpression:
            case SyntaxKind.ThrowStatement:
                {
                    if (context.SemanticModel
                        .GetTypeSymbol(expression, context.CancellationToken)?
                        .HasMetadataName(MetadataNames.System_Exception) == true)
                    {
                        ReportExplicitToImplicit(ref context);
                    }

                    break;
                }
            case SyntaxKind.EqualsValueClause:
                {
                    parent = parent.Parent;

                    SyntaxDebug.Assert(parent.IsKind(SyntaxKind.VariableDeclarator, SyntaxKind.PropertyDeclaration), parent);

                    if (parent.IsKind(SyntaxKind.VariableDeclarator))
                    {
                        parent = parent.Parent;

                        if (parent is VariableDeclarationSyntax variableDeclaration)
                        {
                            SyntaxDebug.Assert(variableDeclaration.IsParentKind(SyntaxKind.FieldDeclaration, SyntaxKind.LocalDeclarationStatement, SyntaxKind.UsingStatement), variableDeclaration.Parent);

                            if (variableDeclaration.IsParentKind(SyntaxKind.FieldDeclaration))
                            {
                                AnalyzeType(ref context, expression, variableDeclaration.Type);
                            }
                            else if (variableDeclaration.IsParentKind(SyntaxKind.LocalDeclarationStatement, SyntaxKind.UsingStatement))
                            {
                                if (context.UseVarInsteadOfImplicitObjectCreation() == false)
                                {
                                    if (variableDeclaration.Type.IsVar)
                                    {
                                        ReportExplicitToImplicit(ref context);
                                    }
                                    else
                                    {
                                        AnalyzeType(ref context, expression, variableDeclaration.Type);
                                    }
                                }
                            }
                        }
                    }
                    else if (parent.IsKind(SyntaxKind.PropertyDeclaration))
                    {
                        AnalyzeType(ref context, expression, ((PropertyDeclarationSyntax)parent).Type);
                    }

                    break;
                }
            case SyntaxKind.ArrowExpressionClause:
                {
                    TypeSyntax type = DetermineReturnType(parent.Parent);

                    SyntaxDebug.Assert(type is not null, parent);

                    if (type is not null)
                        AnalyzeType(ref context, expression, type);

                    break;
                }
            case SyntaxKind.ArrayInitializerExpression:
                {
                    SyntaxDebug.Assert(parent.IsParentKind(SyntaxKind.ArrayCreationExpression, SyntaxKind.ImplicitArrayCreationExpression, SyntaxKind.EqualsValueClause, SyntaxKind.ImplicitStackAllocArrayCreationExpression), parent.Parent);

                    if (parent.IsParentKind(SyntaxKind.ArrayCreationExpression))
                    {
                        var arrayCreationExpression = (ArrayCreationExpressionSyntax)parent.Parent;

                        AnalyzeType(ref context, expression, arrayCreationExpression.Type.ElementType);
                    }
                    else if (parent.IsParentKind(SyntaxKind.EqualsValueClause))
                    {
                        parent = parent.Parent.Parent;

                        if (parent.IsKind(SyntaxKind.VariableDeclarator))
                        {
                            parent = parent.Parent;

                            if (parent is VariableDeclarationSyntax variableDeclaration)
                            {
                                if (parent.IsParentKind(SyntaxKind.FieldDeclaration))
                                {
                                    AnalyzeArrayType(ref context, expression, variableDeclaration.Type);
                                }
                                else if (parent.IsParentKind(SyntaxKind.LocalDeclarationStatement, SyntaxKind.UsingStatement))
                                {
                                    if (!variableDeclaration.Type.IsVar)
                                        AnalyzeArrayType(ref context, expression, variableDeclaration.Type);
                                }
                            }
                        }
                        else if (parent.IsKind(SyntaxKind.PropertyDeclaration))
                        {
                            AnalyzeArrayType(ref context, expression, ((PropertyDeclarationSyntax)parent).Type);
                        }
                    }

                    break;
                }
            case SyntaxKind.ReturnStatement:
            case SyntaxKind.YieldReturnStatement:
                {
                    if (style != TypeStyle.Implicit
                        && !IsSingleReturnStatement(parent))
                    {
                        return;
                    }

                    for (SyntaxNode node = parent.Parent; node is not null; node = node.Parent)
                    {
                        if (CSharpFacts.IsAnonymousFunctionExpression(node.Kind()))
                            return;

                        TypeSyntax type = DetermineReturnType(node);

                        if (type is not null)
                        {
                            if (parent.IsKind(SyntaxKind.YieldReturnStatement))
                            {
                                ITypeSymbol typeSymbol = context.SemanticModel.GetTypeSymbol(type, context.CancellationToken);

                                if (typeSymbol?.OriginalDefinition.SpecialType == SpecialType.System_Collections_Generic_IEnumerable_T)
                                {
                                    var ienumerableOfT = (INamedTypeSymbol)typeSymbol;

                                    ITypeSymbol typeSymbol2 = ienumerableOfT.TypeArguments.Single();

                                    AnalyzeTypeSymbol(ref context, expression, typeSymbol2);
                                }
                            }
                            else
                            {
                                AnalyzeType(ref context, expression, type);
                            }

                            return;
                        }
                    }

                    break;
                }
            case SyntaxKind.SimpleAssignmentExpression:
            case SyntaxKind.CoalesceAssignmentExpression:
            case SyntaxKind.AddAssignmentExpression:
            case SyntaxKind.SubtractAssignmentExpression:
                {
                    if (style == TypeStyle.Implicit)
                    {
                        var assignment = (AssignmentExpressionSyntax)parent;
                        AnalyzeExpression(ref context, expression, assignment.Left);
                    }

                    break;
                }
            case SyntaxKind.CoalesceExpression:
                {
                    if (style == TypeStyle.Implicit)
                    {
                        var coalesceExpression = (BinaryExpressionSyntax)parent;
                        AnalyzeExpression(ref context, expression, coalesceExpression.Left);
                    }
                    else if (style == TypeStyle.ImplicitWhenTypeIsObvious
                        && parent.IsParentKind(SyntaxKind.EqualsValueClause))
                    {
                        if (parent.Parent.Parent is VariableDeclaratorSyntax variableDeclarator)
                        {
                            if (variableDeclarator.Parent is VariableDeclarationSyntax variableDeclaration
                                && variableDeclaration.IsParentKind(SyntaxKind.FieldDeclaration))
                            {
                                AnalyzeType(ref context, expression, variableDeclaration.Type);
                            }
                        }
                        else if (parent.Parent.Parent is PropertyDeclarationSyntax propertyDeclaration)
                        {
                            AnalyzeType(ref context, expression, propertyDeclaration.Type);
                        }
                    }

                    break;
                }
            case SyntaxKind.CollectionInitializerExpression:
                {
                    SyntaxDebug.Assert(parent.IsParentKind(SyntaxKind.ObjectCreationExpression, SyntaxKind.ImplicitObjectCreationExpression, SyntaxKind.SimpleAssignmentExpression), parent.Parent);

                    parent = parent.Parent;
                    if (parent.IsKind(SyntaxKind.ObjectCreationExpression, SyntaxKind.ImplicitObjectCreationExpression))
                    {
                        SyntaxNode parentObjectCreation = parent;

                        parent = parent.Parent;
                        if (parent.IsKind(SyntaxKind.EqualsValueClause))
                        {
                            parent = parent.Parent;
                            if (parent.IsKind(SyntaxKind.VariableDeclarator))
                            {
                                parent = parent.Parent;
                                if (parent is VariableDeclarationSyntax variableDeclaration
                                    && parent.IsParentKind(SyntaxKind.FieldDeclaration, SyntaxKind.LocalDeclarationStatement))
                                {
                                    if (parentObjectCreation is ExpressionSyntax parentObjectCreationExpression)
                                    {
                                        AnalyzeExpression(ref context, expression, parentObjectCreationExpression, extractGenericType: true);
                                    }
                                    else
                                    {
                                        AnalyzeType(ref context, expression, variableDeclaration.Type, extractGenericType: true);
                                    }
                                }
                            }
                            else if (parent.IsKind(SyntaxKind.PropertyDeclaration))
                            {
                                AnalyzeType(ref context, expression, ((PropertyDeclarationSyntax)parent).Type);
                            }
                        }
                    }

                    break;
                }
        }
    }

    public virtual void AnalyzeImplicitCreation(ref SyntaxNodeAnalysisContext context)
    {
        AnalyzeImplicit(ref context);
    }

    public virtual void AnalyzeCollectionExpression(ref SyntaxNodeAnalysisContext context)
    {
        AnalyzeImplicit(ref context);
    }

    private void AnalyzeImplicit(ref SyntaxNodeAnalysisContext context)
    {
        if (context.Node.ContainsDiagnostics)
            return;

        TypeStyle style = GetTypeStyle(ref context);

        SyntaxNode parent = context.Node.Parent;

        switch (parent.Kind())
        {
            case SyntaxKind.ThrowExpression:
            case SyntaxKind.ThrowStatement:
                {
                    if (style == TypeStyle.Explicit
                        && context.SemanticModel.GetTypeSymbol(context.Node, context.CancellationToken)?
                            .HasMetadataName(MetadataNames.System_Exception) == true)
                    {
                        ReportImplicitToExplicit(ref context);
                    }

                    break;
                }
            case SyntaxKind.EqualsValueClause:
                {
                    parent = parent.Parent;

                    SyntaxDebug.Assert(parent.IsKind(SyntaxKind.VariableDeclarator, SyntaxKind.PropertyDeclaration, SyntaxKind.Parameter), parent);

                    if (parent.IsKind(SyntaxKind.VariableDeclarator))
                    {
                        parent = parent.Parent;

                        if (parent is VariableDeclarationSyntax variableDeclaration)
                        {
                            bool isVar = variableDeclaration.Type.IsVar;

#if ROSLYN_4_7
                            SyntaxDebug.Assert(!isVar || context.Node.IsKind(SyntaxKind.CollectionExpression, SyntaxKind.ImplicitArrayCreationExpression), variableDeclaration);
#else
                            SyntaxDebug.Assert(!isVar || context.Node.IsKind(SyntaxKind.ImplicitArrayCreationExpression), variableDeclaration);
#endif
                            SyntaxDebug.Assert(parent.IsParentKind(SyntaxKind.FieldDeclaration, SyntaxKind.LocalDeclarationStatement, SyntaxKind.UsingStatement), parent.Parent);

                            if (!AnalyzeImplicit(ref context, isObvious: !isVar, allowCollectionExpression: !isVar)
                                && parent.IsParentKind(SyntaxKind.LocalDeclarationStatement, SyntaxKind.UsingStatement)
                                && variableDeclaration.Variables.Count == 1
                                && !isVar
                                && context.UseVarInsteadOfImplicitObjectCreation() == true)
                            {
                                DiagnosticHelpers.ReportDiagnostic(context, DiagnosticRules.UseImplicitOrExplicitObjectCreation, variableDeclaration, "explicit");
                            }
                        }
                    }
                    else if (parent.IsKind(SyntaxKind.PropertyDeclaration))
                    {
                        AnalyzeImplicitObvious(ref context);
                    }

                    break;
                }
            case SyntaxKind.ArrowExpressionClause:
                {
                    if (style == TypeStyle.Explicit)
                    {
                        TypeSyntax type = DetermineReturnType(parent.Parent);

                        SyntaxDebug.Assert(type is not null, parent);

                        if (type is not null)
                            AnalyzeImplicitObvious(ref context);
                    }

                    break;
                }
            case SyntaxKind.ArrayInitializerExpression:
                {
                    SyntaxDebug.Assert(parent.IsParentKind(SyntaxKind.ArrayCreationExpression, SyntaxKind.ImplicitArrayCreationExpression, SyntaxKind.EqualsValueClause), parent.Parent);

                    if (parent.IsParentKind(SyntaxKind.ArrayCreationExpression))
                        AnalyzeImplicitObvious(ref context);

                    break;
                }
            case SyntaxKind.ReturnStatement:
            case SyntaxKind.YieldReturnStatement:
                {
                    if (style != TypeStyle.Explicit
                        && (style != TypeStyle.ImplicitWhenTypeIsObvious || IsSingleReturnStatement(parent)))
                    {
                        return;
                    }

                    for (SyntaxNode node = parent.Parent; node is not null; node = node.Parent)
                    {
                        if (CSharpFacts.IsAnonymousFunctionExpression(node.Kind()))
                            return;

                        TypeSyntax type = DetermineReturnType(node);

                        if (type is not null)
                        {
                            if (parent.IsKind(SyntaxKind.YieldReturnStatement))
                            {
                                ITypeSymbol typeSymbol = context.SemanticModel.GetTypeSymbol(type, context.CancellationToken);

                                if (typeSymbol?.OriginalDefinition.SpecialType == SpecialType.System_Collections_Generic_IEnumerable_T)
                                    AnalyzeImplicitNotObvious(ref context);
                            }
                            else
                            {
                                AnalyzeImplicit(ref context, isObvious: IsSingleReturnStatement(parent));
                            }
                        }
                    }

                    break;
                }
            case SyntaxKind.SimpleAssignmentExpression:
            case SyntaxKind.CoalesceAssignmentExpression:
            case SyntaxKind.AddAssignmentExpression:
            case SyntaxKind.SubtractAssignmentExpression:
                {
                    AnalyzeImplicitNotObvious(ref context);
                    break;
                }
            case SyntaxKind.CoalesceExpression:
                {
                    if (parent.IsParentKind(SyntaxKind.EqualsValueClause))
                    {
                        switch (parent.Parent.Parent)
                        {
                            case VariableDeclaratorSyntax variableDeclarator:
                                {
                                    if (variableDeclarator.Parent is VariableDeclarationSyntax variableDeclaration)
                                        AnalyzeImplicit(ref context, isObvious: !variableDeclaration.Type.IsVar);

                                    return;
                                }
                            case PropertyDeclarationSyntax:
                                {
                                    AnalyzeImplicitObvious(ref context);
                                    return;
                                }
                        }
                    }

                    AnalyzeImplicitNotObvious(ref context);
                    break;
                }
            case SyntaxKind.CollectionInitializerExpression:
                {
                    SyntaxDebug.Assert(parent.IsParentKind(SyntaxKind.ObjectCreationExpression, SyntaxKind.ImplicitObjectCreationExpression, SyntaxKind.SimpleAssignmentExpression), parent.Parent);

                    if (style != TypeStyle.Explicit)
                        return;

                    parent = parent.Parent;
                    if (parent.IsKind(SyntaxKind.ObjectCreationExpression, SyntaxKind.ImplicitObjectCreationExpression))
                    {
                        parent = parent.Parent;
                        if (parent.IsKind(SyntaxKind.EqualsValueClause))
                        {
                            parent = parent.Parent;
                            if (parent.IsKind(SyntaxKind.VariableDeclarator))
                            {
                                parent = parent.Parent;
                                if (parent is VariableDeclarationSyntax)
                                {
                                    SyntaxDebug.Assert(parent.IsParentKind(SyntaxKind.FieldDeclaration, SyntaxKind.LocalDeclarationStatement, SyntaxKind.UsingStatement), parent.Parent);

                                    AnalyzeImplicitObvious(ref context);
                                }
                            }
                            else if (parent.IsKind(SyntaxKind.PropertyDeclaration))
                            {
                                AnalyzeImplicitObvious(ref context);
                            }
                        }
                    }

                    break;
                }
        }
    }

    private void AnalyzeType(
        ref SyntaxNodeAnalysisContext context,
        ExpressionSyntax creationExpression,
        TypeSyntax type,
        bool extractGenericType = false)
    {
        if (!type.IsVar)
            AnalyzeExpression(ref context, creationExpression, type, extractGenericType: extractGenericType);
    }

    private void AnalyzeArrayType(
        ref SyntaxNodeAnalysisContext context,
        ExpressionSyntax creationExpression,
        TypeSyntax type)
    {
        if (type is ArrayTypeSyntax arrayType)
        {
            type = arrayType.ElementType;

            if (!type.IsVar)
                AnalyzeExpression(ref context, creationExpression, type);
        }
    }

    private void AnalyzeExpression(
        ref SyntaxNodeAnalysisContext context,
        ExpressionSyntax creationExpression,
        ExpressionSyntax expression,
        bool extractGenericType = false)
    {
        ITypeSymbol typeSymbol1 = context.SemanticModel.GetTypeSymbol(expression, context.CancellationToken);

        if (extractGenericType)
        {
            typeSymbol1 = (typeSymbol1 as INamedTypeSymbol)?.TypeArguments.SingleOrDefault(shouldThrow: false);
        }

        AnalyzeTypeSymbol(ref context, creationExpression, typeSymbol1);
    }

    private void AnalyzeTypeSymbol(
        ref SyntaxNodeAnalysisContext context,
        ExpressionSyntax creationExpression,
        ITypeSymbol typeSymbol1)
    {
        if (typeSymbol1?.IsErrorType() == false)
        {
            ITypeSymbol typeSymbol2 = context.SemanticModel.GetTypeSymbol(creationExpression, context.CancellationToken);

            if (SymbolEqualityComparer.Default.Equals(typeSymbol1, typeSymbol2))
            {
#if ROSLYN_4_7
                if (((ObjectCreationExpressionSyntax)context.Node).ArgumentList?.Arguments.Any() != true
                    && UseCollectionExpression(ref context)
                    )
                {
                    ReportExplicitToCollectionExpression(ref context);
                }
                else
                {
#endif
                    ReportExplicitToImplicit(ref context);
#if ROSLYN_4_7
                }
#endif
            }
        }
    }

    protected static TypeSyntax DetermineReturnType(SyntaxNode node)
    {
        switch (node.Kind())
        {
            case SyntaxKind.LocalFunctionStatement:
                return ((LocalFunctionStatementSyntax)node).ReturnType;
            case SyntaxKind.MethodDeclaration:
                return ((MethodDeclarationSyntax)node).ReturnType;
            case SyntaxKind.OperatorDeclaration:
                return ((OperatorDeclarationSyntax)node).ReturnType;
            case SyntaxKind.ConversionOperatorDeclaration:
                return ((ConversionOperatorDeclarationSyntax)node).Type;
            case SyntaxKind.PropertyDeclaration:
                return ((PropertyDeclarationSyntax)node).Type;
            case SyntaxKind.IndexerDeclaration:
                return ((IndexerDeclarationSyntax)node).Type;
        }

        if (node is AccessorDeclarationSyntax)
        {
            SyntaxDebug.Assert(node.IsParentKind(SyntaxKind.AccessorList), node.Parent);

            if (node.IsParentKind(SyntaxKind.AccessorList))
                return DetermineReturnType(node.Parent.Parent);
        }

        return null;
    }

    private bool AnalyzeImplicitObvious(ref SyntaxNodeAnalysisContext context)
    {
        return AnalyzeImplicit(ref context, isObvious: true);
    }

    private bool AnalyzeImplicitNotObvious(ref SyntaxNodeAnalysisContext context)
    {
        return AnalyzeImplicit(ref context, isObvious: false);
    }

    private bool AnalyzeImplicit(ref SyntaxNodeAnalysisContext context, bool isObvious, bool allowCollectionExpression = true)
    {
        TypeStyle style = GetTypeStyle(ref context);

        if (style == TypeStyle.Explicit)
        {
            ReportImplicitToExplicit(ref context);
            return true;
        }

        if (style == TypeStyle.Implicit)
        {
#if ROSLYN_4_7
            if (context.Node.IsKind(SyntaxKind.CollectionExpression))
            {
                if (context.UseCollectionExpression() == false)
                {
                    ReportCollectionExpressionToImplicit(ref context);
                    return true;
                }
            }
            else if (allowCollectionExpression
                && UseCollectionExpressionFromImplicit(ref context))
            {
                ReportImplicitToCollectionExpression(ref context);
                return true;
            }
#endif
        }
        else if (style == TypeStyle.ImplicitWhenTypeIsObvious)
        {
            if (!isObvious
                && !IsInitializerObvious(ref context))
            {
                ReportImplicitToExplicit(ref context);
                return true;
            }

#if ROSLYN_4_7
            if (context.Node.IsKind(SyntaxKind.CollectionExpression))
            {
                if (context.UseCollectionExpression() == false)
                {
                    ReportCollectionExpressionToImplicit(ref context);
                    return true;
                }
            }
            else if (allowCollectionExpression
                && UseCollectionExpressionFromImplicit(ref context))
            {
                ReportImplicitToCollectionExpression(ref context);
                return true;
            }
#endif
        }

        return false;
    }

    protected static bool IsSingleReturnStatement(SyntaxNode parent)
    {
        return parent.IsKind(SyntaxKind.ReturnStatement)
            && parent.Parent is BlockSyntax block
            && block.Statements.Count == 1
            && parent.Parent.Parent is MemberDeclarationSyntax;
    }

#if ROSLYN_4_7
    protected abstract bool UseCollectionExpressionFromImplicit(ref SyntaxNodeAnalysisContext context);

    protected static bool UseCollectionExpression(ref SyntaxNodeAnalysisContext context)
    {
        Debug.Assert(!context.Node.IsKind(SyntaxKind.CollectionExpression), context.Node.Kind().ToString());

        return context.UseCollectionExpression() == true
            && ((CSharpCompilation)context.Compilation).SupportsCollectionExpression()
            && SyntaxUtility.CanConvertToCollectionExpression(context.Node, context.SemanticModel, context.CancellationToken);
    }
#endif
}
