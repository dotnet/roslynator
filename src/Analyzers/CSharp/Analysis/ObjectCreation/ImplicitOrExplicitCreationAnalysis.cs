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

    protected virtual bool IsInitializerObvious(ref SyntaxNodeAnalysisContext context) => false;

    public abstract ObjectCreationTypeStyle GetTypeStyle(ref SyntaxNodeAnalysisContext context);

    protected abstract void ReportExplicitToImplicit(ref SyntaxNodeAnalysisContext context);

    protected abstract void ReportExplicitToCollectionExpression(ref SyntaxNodeAnalysisContext context);

    protected abstract void ReportImplicitToExplicit(ref SyntaxNodeAnalysisContext context);

    protected abstract void ReportImplicitToCollectionExpression(ref SyntaxNodeAnalysisContext context);

    protected abstract void ReportCollectionExpressionToImplicit(ref SyntaxNodeAnalysisContext context);

    public virtual void AnalyzeExplicitCreation(ref SyntaxNodeAnalysisContext context)
    {
        var expression = (ExpressionSyntax)context.Node;

        SyntaxNode parent = expression.Parent;

        switch (parent.Kind())
        {
            case SyntaxKind.ThrowExpression:
            case SyntaxKind.ThrowStatement:
                {
                    if (UseImplicitOrImplicitWhenObvious(ref context)
                        && context.SemanticModel.GetTypeSymbol(expression, context.CancellationToken)?
                            .HasMetadataName(MetadataNames.System_Exception) == true)
                    {
                        ReportExplicitToImplicit(ref context);
                    }

                    break;
                }
            case SyntaxKind.EqualsValueClause:
                {
                    if (!UseImplicitOrImplicitWhenObvious(ref context))
                        return;

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
                    if (UseImplicitOrImplicitWhenObvious(ref context))
                    {
                        TypeSyntax type = DetermineReturnType(parent.Parent);

                        SyntaxDebug.Assert(type is not null, parent);

                        if (type is not null)
                            AnalyzeType(ref context, expression, type);
                    }

                    break;
                }
            case SyntaxKind.ArrayInitializerExpression:
                {
                    SyntaxDebug.Assert(parent.IsParentKind(SyntaxKind.ArrayCreationExpression, SyntaxKind.ImplicitArrayCreationExpression, SyntaxKind.EqualsValueClause, SyntaxKind.ImplicitStackAllocArrayCreationExpression), parent.Parent);

                    if (UseImplicitOrImplicitWhenObvious(ref context))
                    {
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
                    }

                    break;
                }
            case SyntaxKind.ReturnStatement:
            case SyntaxKind.YieldReturnStatement:
                {
                    bool isAnalyzable = GetTypeStyle(ref context) switch
                    {
                        ObjectCreationTypeStyle.Explicit => false,
                        ObjectCreationTypeStyle.Implicit => true,
                        ObjectCreationTypeStyle.ImplicitWhenTypeIsObvious => IsSingleReturnStatement(parent),
                        _ => false,
                    };

                    if (!isAnalyzable)
                        return;

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
                    if (UseImplicit(ref context))
                    {
                        var assignment = (AssignmentExpressionSyntax)parent;
                        AnalyzeExpression(ref context, expression, assignment.Left);
                    }

                    break;
                }
            case SyntaxKind.CoalesceExpression:
                {
                    ObjectCreationTypeStyle style = GetTypeStyle(ref context);

                    if (style == ObjectCreationTypeStyle.Implicit)
                    {
                        var coalesceExpression = (BinaryExpressionSyntax)parent;
                        AnalyzeExpression(ref context, expression, coalesceExpression.Left);
                    }
                    else if (style == ObjectCreationTypeStyle.ImplicitWhenTypeIsObvious
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

                    if (!UseImplicitOrImplicitWhenObvious(ref context))
                        return;

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
                                        AnalyzeExpression(ref context, expression, parentObjectCreationExpression, isGenericType: true);
                                    }
                                    else
                                    {
                                        AnalyzeType(ref context, expression, variableDeclaration.Type, isGenericType: true);
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
            case SyntaxKind.ComplexElementInitializerExpression:
                {
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

    public void AnalyzeImplicit(ref SyntaxNodeAnalysisContext context)
    {
        SyntaxNode node = context.Node;
        SyntaxNode parent = node.Parent;

        switch (parent.Kind())
        {
            case SyntaxKind.ThrowExpression:
            case SyntaxKind.ThrowStatement:
                {
                    if (UseExplicit(ref context)
                        && context.SemanticModel.GetTypeSymbol(node, context.CancellationToken)?
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
                            SyntaxDebug.Assert(!isVar || node.IsKind(SyntaxKind.CollectionExpression, SyntaxKind.ImplicitArrayCreationExpression), variableDeclaration);
                            SyntaxDebug.Assert(parent.IsParentKind(SyntaxKind.FieldDeclaration, SyntaxKind.LocalDeclarationStatement, SyntaxKind.UsingStatement), parent.Parent);

                            if (!AnalyzeImplicit(ref context, isObvious: !isVar, canUseCollectionExpression: !isVar)
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
                    if (UseExplicit(ref context))
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
                    bool isAnalyzable = GetTypeStyle(ref context) switch
                    {
                        ObjectCreationTypeStyle.Explicit => true,
                        ObjectCreationTypeStyle.Implicit => false,
                        ObjectCreationTypeStyle.ImplicitWhenTypeIsObvious => !IsSingleReturnStatement(parent),
                        _ => false,
                    };

                    if (!isAnalyzable)
                        return;

                    for (SyntaxNode node2 = parent.Parent; node2 is not null; node2 = node2.Parent)
                    {
                        if (CSharpFacts.IsAnonymousFunctionExpression(node2.Kind()))
                            return;

                        TypeSyntax type = DetermineReturnType(node2);

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

                    if (!UseExplicit(ref context))
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
            case SyntaxKind.ComplexElementInitializerExpression:
                {
                    break;
                }
        }
    }

    protected bool UseExplicit(ref SyntaxNodeAnalysisContext context)
    {
        return GetTypeStyle(ref context) == ObjectCreationTypeStyle.Explicit;
    }

    protected bool UseImplicit(ref SyntaxNodeAnalysisContext context)
    {
        return GetTypeStyle(ref context) == ObjectCreationTypeStyle.Implicit;
    }

    protected bool UseImplicitOrImplicitWhenObvious(ref SyntaxNodeAnalysisContext context)
    {
        ObjectCreationTypeStyle style = GetTypeStyle(ref context);

        return style == ObjectCreationTypeStyle.Implicit
            || style == ObjectCreationTypeStyle.ImplicitWhenTypeIsObvious;
    }

    protected void AnalyzeType(
        ref SyntaxNodeAnalysisContext context,
        ExpressionSyntax creationExpression,
        TypeSyntax type,
        bool isGenericType = false)
    {
        if (!type.IsVar)
        {
            AnalyzeExpression(ref context, creationExpression, type, isGenericType: isGenericType);
        }
    }

    protected void AnalyzeArrayType(
        ref SyntaxNodeAnalysisContext context,
        ExpressionSyntax creationExpression,
        TypeSyntax type,
        bool isGenericType = false)
    {
        if (type is ArrayTypeSyntax arrayType)
        {
            type = arrayType.ElementType;

            if (!type.IsVar)
                AnalyzeExpression(ref context, creationExpression, type, isGenericType: isGenericType);
        }
    }

    protected void AnalyzeExpression(
        ref SyntaxNodeAnalysisContext context,
        ExpressionSyntax creationExpression,
        ExpressionSyntax expression,
        bool isGenericType = false)
    {
        ITypeSymbol typeSymbol1 = context.SemanticModel.GetTypeSymbol(expression, context.CancellationToken);

        if (isGenericType)
        {
            typeSymbol1 = (typeSymbol1 as INamedTypeSymbol)?.TypeArguments.SingleOrDefault(shouldThrow: false);
        }

        AnalyzeTypeSymbol(ref context, creationExpression, typeSymbol1);
    }

    protected void AnalyzeTypeSymbol(
        ref SyntaxNodeAnalysisContext context,
        ExpressionSyntax creationExpression,
        ITypeSymbol typeSymbol1)
    {
        if (typeSymbol1?.IsErrorType() == false)
        {
            ITypeSymbol typeSymbol2 = context.SemanticModel.GetTypeSymbol(creationExpression, context.CancellationToken);

            if (SymbolEqualityComparer.Default.Equals(typeSymbol1, typeSymbol2))
            {
                if (((ObjectCreationExpressionSyntax)context.Node).ArgumentList?.Arguments.Any() != true
                    && UseCollectionExpression(ref context))
                {
                    ReportExplicitToCollectionExpression(ref context);
                }
                else
                {
                    ReportExplicitToImplicit(ref context);
                }
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

    private bool AnalyzeImplicit(ref SyntaxNodeAnalysisContext context, bool isObvious, bool canUseCollectionExpression = true)
    {
        ObjectCreationTypeStyle style = GetTypeStyle(ref context);

        if (style == ObjectCreationTypeStyle.Explicit)
        {
            ReportImplicitToExplicit(ref context);
            return true;
        }

        if (style == ObjectCreationTypeStyle.Implicit)
        {
            if (context.Node.IsKind(SyntaxKind.CollectionExpression))
            {
                if (context.UseCollectionExpression() == false)
                {
                    ReportCollectionExpressionToImplicit(ref context);
                    return true;
                }
            }
            else if (canUseCollectionExpression
                && UseCollectionExpressionFromImplicit(ref context))
            {
                ReportImplicitToCollectionExpression(ref context);
                return true;
            }
        }
        else if (style == ObjectCreationTypeStyle.ImplicitWhenTypeIsObvious)
        {
            if (!isObvious
                && !IsInitializerObvious(ref context))
            {
                ReportImplicitToExplicit(ref context);
                return true;
            }

            if (context.Node.IsKind(SyntaxKind.CollectionExpression))
            {
                if (context.UseCollectionExpression() == false)
                {
                    ReportCollectionExpressionToImplicit(ref context);
                    return true;
                }
            }
            else if (canUseCollectionExpression
                && UseCollectionExpressionFromImplicit(ref context))
            {
                ReportImplicitToCollectionExpression(ref context);
                return true;
            }
        }

        return false;
    }

    protected virtual bool UseCollectionExpressionFromImplicit(ref SyntaxNodeAnalysisContext context)
    {
        return ((ImplicitObjectCreationExpressionSyntax)context.Node).ArgumentList?.Arguments.Any() != true
            && UseCollectionExpression(ref context);
    }

    protected static bool IsSingleReturnStatement(SyntaxNode parent)
    {
        return parent.IsKind(SyntaxKind.ReturnStatement)
            && parent.Parent is BlockSyntax block
            && block.Statements.Count == 1
            && parent.Parent.Parent is MemberDeclarationSyntax;
    }

    protected static bool UseCollectionExpression(ref SyntaxNodeAnalysisContext context)
    {
        Debug.Assert(!context.Node.IsKind(SyntaxKind.CollectionExpression), context.Node.Kind().ToString());

        return context.UseCollectionExpression() == true
            && ((CSharpCompilation)context.Compilation).SupportsCollectionExpression()
            && SyntaxUtility.CanConvertToCollectionExpression(context.Node, context.SemanticModel, context.CancellationToken);
    }

    protected static bool IsInitializerObvious(ref SyntaxNodeAnalysisContext context, CollectionExpressionSyntax collectionExpression)
    {
        SeparatedSyntaxList<CollectionElementSyntax> elements = collectionExpression.Elements;

        IArrayTypeSymbol arrayTypeSymbol = null;
        var isObvious = false;

        foreach (CollectionElementSyntax element in elements)
        {
            if (element is not ExpressionElementSyntax expressionElement)
                return false;

            if (arrayTypeSymbol is null)
            {
                ITypeSymbol type = context.SemanticModel.GetTypeInfo(collectionExpression, context.CancellationToken).ConvertedType;

                arrayTypeSymbol = type as IArrayTypeSymbol;

                if (arrayTypeSymbol?.ElementType.SupportsExplicitDeclaration() != true)
                    return true;
            }

            isObvious = CSharpTypeAnalysis.IsTypeObvious(expressionElement.Expression, arrayTypeSymbol.ElementType, includeNullability: true, context.SemanticModel, context.CancellationToken);

            if (!isObvious)
                return false;
        }

        return isObvious;
    }
}
