using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.CSharp.Analysis;
using Xunit;

namespace Roslynator.Testing.CSharp;

public class PatternMatchingVariableDeclarationHelperTests
{
    [Fact]
    public void SingleVariableDesignation()
    {
        VariableDesignationSyntax designationSyntax = SyntaxFactory.SingleVariableDesignation(
            SyntaxFactory.Identifier("x")
        );
        var variables = PattenMatchingVariableDeclarationHelper.GetVariablesDeclared(designationSyntax);
        Assert.True(1==variables.Count());
        Assert.True("x"== variables.First());
    }
    
    [Fact]
    public void ParenthesizedVariableDesignation()
    {
        VariableDesignationSyntax designationSyntax = SyntaxFactory.ParenthesizedVariableDesignation(
            SyntaxFactory.SeparatedList(new List<VariableDesignationSyntax>
            {
                SyntaxFactory.SingleVariableDesignation(SyntaxFactory.Identifier("x")),
                SyntaxFactory.SingleVariableDesignation(SyntaxFactory.Identifier("y"))
            })
        );

        var variables = PattenMatchingVariableDeclarationHelper.GetVariablesDeclared(designationSyntax);
        Assert.True(2 == variables.Count());
        Assert.True(variables.Contains("x"));
        Assert.True(variables.Contains("y"));
    }

    [Fact]
    public void DiscardDesignation()
    {
        VariableDesignationSyntax designationSyntax = SyntaxFactory.DiscardDesignation();
        var variables = PattenMatchingVariableDeclarationHelper.GetVariablesDeclared(designationSyntax);
        Assert.True(!variables.Any());
    }
    
    [Fact]
    public void NullTest()
    {
        VariableDesignationSyntax designationSyntax = null;
        var variables = PattenMatchingVariableDeclarationHelper.GetVariablesDeclared(designationSyntax);
        Assert.True(!variables.Any());

    }
    
    [Fact]
    public void DeclarationPattern()
    {
        PatternSyntax patternSyntax = SyntaxFactory.DeclarationPattern(
            SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.IntKeyword)),
            SyntaxFactory.SingleVariableDesignation(SyntaxFactory.Identifier("x"))
        );
        var variables = PattenMatchingVariableDeclarationHelper.GetVariablesDeclared(patternSyntax);
        Assert.True(1==variables.Count());
        Assert.True("x"== variables.First());
    }
    
    [Fact]
    public void RecursivePattern_WithPositional()
    {
        PatternSyntax patternSyntax = SyntaxFactory.RecursivePattern(
            SyntaxFactory.IdentifierName("TypeA"),
            positionalPatternClause: SyntaxFactory.PositionalPatternClause(
                SyntaxFactory.SeparatedList(new List<SubpatternSyntax>
                {
                    SyntaxFactory.Subpattern(
                        SyntaxFactory.DeclarationPattern(
                            SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.IntKeyword)),
                            SyntaxFactory.SingleVariableDesignation(SyntaxFactory.Identifier("p1"))
                        )
                        ),
                    SyntaxFactory.Subpattern(
                        SyntaxFactory.DeclarationPattern(
                            SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.IntKeyword)),
                            SyntaxFactory.SingleVariableDesignation(SyntaxFactory.Identifier("p2"))
                        )
                    )
                })
            ),
            propertyPatternClause: default,
            designation: default
        );
        var variables = PattenMatchingVariableDeclarationHelper.GetVariablesDeclared(patternSyntax);
        Assert.True(2==variables.Count());
        Assert.True(variables.Contains("p1"));
        Assert.True(variables.Contains("p2"));
    }
    
    [Fact]
    public void RecursivePattern_WithProperty()
    {
        PatternSyntax patternSyntax = SyntaxFactory.RecursivePattern(
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
        var variables = PattenMatchingVariableDeclarationHelper.GetVariablesDeclared(patternSyntax);
        Assert.True(1==variables.Count());
        Assert.True(variables.Contains("x"));
    }
    
    [Fact]
    public void RecursivePattern_WithDesignation()
    {
        PatternSyntax patternSyntax = SyntaxFactory.RecursivePattern(
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
        var variables = PattenMatchingVariableDeclarationHelper.GetVariablesDeclared(patternSyntax);
        Assert.True(1==variables.Count());
        Assert.True(variables.Contains("x"));
    
    }
    
    [Fact]
    public void VarPattern()
    {
        PatternSyntax patternSyntax = SyntaxFactory.VarPattern(
            SyntaxFactory.SingleVariableDesignation(SyntaxFactory.Identifier("x"))
        );
        var variables = PattenMatchingVariableDeclarationHelper.GetVariablesDeclared(patternSyntax);
        Assert.True(1==variables.Count());
        Assert.True("x"==variables.First());

    }
    
    [Fact]
    public void BinaryPattern()
    {
        PatternSyntax patternSyntax = SyntaxFactory.BinaryPattern(
            SyntaxKind.AndPattern,
            SyntaxFactory.ConstantPattern(SyntaxFactory.LiteralExpression(SyntaxKind.NumericLiteralExpression, SyntaxFactory.Literal(42))),
            SyntaxFactory.ConstantPattern(SyntaxFactory.LiteralExpression(SyntaxKind.NumericLiteralExpression, SyntaxFactory.Literal(99)))
        );
        var variables = PattenMatchingVariableDeclarationHelper.GetVariablesDeclared(patternSyntax);
        Assert.True(!variables.Any());
    }

    [Fact]
    public void ParenthesizedPattern()
    {
        PatternSyntax patternSyntax = SyntaxFactory.ParenthesizedPattern(
            SyntaxFactory.VarPattern(
                SyntaxFactory.SingleVariableDesignation(SyntaxFactory.Identifier("x"))
            )
        );
        var variables = PattenMatchingVariableDeclarationHelper.GetVariablesDeclared(patternSyntax);
        Assert.True(1==variables.Count());
        Assert.True("x"==variables.First());
    }

}