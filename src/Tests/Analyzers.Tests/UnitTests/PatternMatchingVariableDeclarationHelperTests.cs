// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.CSharp.Analysis;
using Xunit;

namespace Roslynator.Analyzers.Tests.UnitTests;

public class PatternMatchingVariableDeclarationHelperTests
{
    [Fact]
    public void SingleVariableDesignation()
    {
        VariableDesignationSyntax designation = SyntaxFactory.SingleVariableDesignation(
            SyntaxFactory.Identifier("x")
        );
        var vars = new HashSet<string>() { "x" }.ToImmutableHashSet();
        Assert.True(PattenMatchingVariableDeclarationHelper.AnyDeclaredVariablesMatch(designation, vars));
        
        vars = new HashSet<string>() { "z" }.ToImmutableHashSet();
        Assert.False(PattenMatchingVariableDeclarationHelper.AnyDeclaredVariablesMatch(designation, vars));
    }
    
    [Fact]
    public void ParenthesizedVariableDesignation()
    {
        VariableDesignationSyntax designation = SyntaxFactory.ParenthesizedVariableDesignation(
            SyntaxFactory.SeparatedList(new List<VariableDesignationSyntax>
            {
                SyntaxFactory.SingleVariableDesignation(SyntaxFactory.Identifier("x")),
                SyntaxFactory.SingleVariableDesignation(SyntaxFactory.Identifier("y"))
            })
        );

        var vars = new HashSet<string>() { "x" }.ToImmutableHashSet();
        Assert.True(PattenMatchingVariableDeclarationHelper.AnyDeclaredVariablesMatch(designation, vars));

        vars = new HashSet<string>() { "y" }.ToImmutableHashSet();
        Assert.True(PattenMatchingVariableDeclarationHelper.AnyDeclaredVariablesMatch(designation, vars));
        
        vars = new HashSet<string>() { "z" }.ToImmutableHashSet();
        Assert.False(PattenMatchingVariableDeclarationHelper.AnyDeclaredVariablesMatch(designation, vars));

    }

    [Fact]
    public void DiscardDesignation()
    {
        VariableDesignationSyntax designation = SyntaxFactory.DiscardDesignation();
        var vars = new HashSet<string>() { "z" }.ToImmutableHashSet();
        Assert.False(PattenMatchingVariableDeclarationHelper.AnyDeclaredVariablesMatch(designation, vars));
    }
    
    [Fact]
    public void NullTest()
    {
        VariableDesignationSyntax designation = null;
        var vars = new HashSet<string>() { "x" }.ToImmutableHashSet();
        Assert.False(PattenMatchingVariableDeclarationHelper.AnyDeclaredVariablesMatch(designation, vars));
    
    }
    
    [Fact]
    public void DeclarationPattern()
    {
        PatternSyntax pattern = SyntaxFactory.DeclarationPattern(
            SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.IntKeyword)),
            SyntaxFactory.SingleVariableDesignation(SyntaxFactory.Identifier("x"))
        );
        var vars = new HashSet<string>() { "x" }.ToImmutableHashSet();
        Assert.True(PattenMatchingVariableDeclarationHelper.AnyDeclaredVariablesMatch(pattern, vars));
        
        vars = new HashSet<string>() { "z" }.ToImmutableHashSet();
        Assert.False(PattenMatchingVariableDeclarationHelper.AnyDeclaredVariablesMatch(pattern, vars));
    }
    
    [Fact]
    public void RecursivePattern_WithPositional()
    {
        PatternSyntax pattern = SyntaxFactory.RecursivePattern(
            SyntaxFactory.IdentifierName("TypeA"),
            positionalPatternClause: SyntaxFactory.PositionalPatternClause(
                SyntaxFactory.SeparatedList(new List<SubpatternSyntax>
                {
                    SyntaxFactory.Subpattern(
                        SyntaxFactory.DeclarationPattern(
                            SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.IntKeyword)),
                            SyntaxFactory.SingleVariableDesignation(SyntaxFactory.Identifier("x"))
                        )
                        ),
                    SyntaxFactory.Subpattern(
                        SyntaxFactory.DeclarationPattern(
                            SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.IntKeyword)),
                            SyntaxFactory.SingleVariableDesignation(SyntaxFactory.Identifier("y"))
                        )
                    )
                })
            ),
            propertyPatternClause: default,
            designation: default
        );
        
        var vars = new HashSet<string>() { "x" }.ToImmutableHashSet();
        Assert.True(PattenMatchingVariableDeclarationHelper.AnyDeclaredVariablesMatch(pattern, vars));

        vars = new HashSet<string>() { "y" }.ToImmutableHashSet();
        Assert.True(PattenMatchingVariableDeclarationHelper.AnyDeclaredVariablesMatch(pattern, vars));
        
        vars = new HashSet<string>() { "z" }.ToImmutableHashSet();
        Assert.False(PattenMatchingVariableDeclarationHelper.AnyDeclaredVariablesMatch(pattern, vars));
    }
    
    [Fact]
    public void RecursivePattern_WithProperty()
    {
        PatternSyntax pattern = SyntaxFactory.RecursivePattern(
            SyntaxFactory.IdentifierName("TypeA"),
            positionalPatternClause: default,
            propertyPatternClause: SyntaxFactory.PropertyPatternClause(
                SyntaxFactory.SeparatedList(new List<SubpatternSyntax>
                {
                    SyntaxFactory.Subpattern(
                        SyntaxFactory.NameColon(SyntaxFactory.IdentifierName("PropertyName")),
                        SyntaxFactory.VarPattern(
                            SyntaxFactory.SingleVariableDesignation(SyntaxFactory.Identifier("x"))
                        )
                    ),
                })
            ),
            designation: default
        );
        var vars = new HashSet<string>() { "x" }.ToImmutableHashSet();
        Assert.True(PattenMatchingVariableDeclarationHelper.AnyDeclaredVariablesMatch(pattern, vars));
        
        vars = new HashSet<string>() { "z" }.ToImmutableHashSet();
        Assert.False(PattenMatchingVariableDeclarationHelper.AnyDeclaredVariablesMatch(pattern, vars));
    }
    
    [Fact]
    public void RecursivePattern_WithDesignation()
    {
        PatternSyntax pattern = SyntaxFactory.RecursivePattern(
            SyntaxFactory.IdentifierName("TypeA"),
            positionalPatternClause: default,
            propertyPatternClause: SyntaxFactory.PropertyPatternClause(
                SyntaxFactory.SeparatedList(new List<SubpatternSyntax>
                {
                    SyntaxFactory.Subpattern(
                        SyntaxFactory.NameColon(SyntaxFactory.IdentifierName("PropertyName")),
                        SyntaxFactory.ConstantPattern(SyntaxFactory.LiteralExpression(SyntaxKind.NumericLiteralExpression, SyntaxFactory.Literal(42)))
                    ),
                })
            ),
           designation: SyntaxFactory.SingleVariableDesignation(SyntaxFactory.Identifier("x"))
        );
        var vars = new HashSet<string>() { "x" }.ToImmutableHashSet();
        Assert.True(PattenMatchingVariableDeclarationHelper.AnyDeclaredVariablesMatch(pattern, vars));
        
        vars = new HashSet<string>() { "z" }.ToImmutableHashSet();
        Assert.False(PattenMatchingVariableDeclarationHelper.AnyDeclaredVariablesMatch(pattern, vars));
    
    }
    
    [Fact]
    public void VarPattern()
    {
        PatternSyntax pattern = SyntaxFactory.VarPattern(
            SyntaxFactory.SingleVariableDesignation(SyntaxFactory.Identifier("x"))
        );
        var vars = new HashSet<string>() { "x" }.ToImmutableHashSet();
        Assert.True(PattenMatchingVariableDeclarationHelper.AnyDeclaredVariablesMatch(pattern, vars));
        
        vars = new HashSet<string>() { "z" }.ToImmutableHashSet();
        Assert.False(PattenMatchingVariableDeclarationHelper.AnyDeclaredVariablesMatch(pattern, vars));
    
    }
    
    [Fact]
    public void BinaryPattern()
    {
        PatternSyntax pattern = SyntaxFactory.BinaryPattern(
            SyntaxKind.AndPattern,
            SyntaxFactory.ConstantPattern(SyntaxFactory.LiteralExpression(SyntaxKind.NumericLiteralExpression, SyntaxFactory.Literal(42))),
            SyntaxFactory.ConstantPattern(SyntaxFactory.LiteralExpression(SyntaxKind.NumericLiteralExpression, SyntaxFactory.Literal(99)))
        );
        var vars = new HashSet<string>() { "z" }.ToImmutableHashSet();
        Assert.False(PattenMatchingVariableDeclarationHelper.AnyDeclaredVariablesMatch(pattern, vars));
    }
    
    [Fact]
    public void ParenthesizedPattern()
    {
        PatternSyntax pattern = SyntaxFactory.ParenthesizedPattern(
            SyntaxFactory.VarPattern(
                SyntaxFactory.SingleVariableDesignation(SyntaxFactory.Identifier("x"))
            )
        );
        var vars = new HashSet<string>() { "x" }.ToImmutableHashSet();
        Assert.True(PattenMatchingVariableDeclarationHelper.AnyDeclaredVariablesMatch(pattern, vars));
        
        vars = new HashSet<string>() { "z" }.ToImmutableHashSet();
        Assert.False(PattenMatchingVariableDeclarationHelper.AnyDeclaredVariablesMatch(pattern, vars));
    }
}