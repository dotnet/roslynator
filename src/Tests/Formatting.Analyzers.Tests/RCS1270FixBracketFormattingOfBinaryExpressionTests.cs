// Copyright (c) .NET Foundation and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Roslynator.Formatting.CodeFixes.CSharp;
using Roslynator.Testing.CSharp;
using Xunit;

namespace Roslynator.Formatting.CSharp.Tests;

public class RCS1270FixBracketFormattingOfBinaryExpressionTests :
    AbstractCSharpDiagnosticVerifier<FixBracketFormattingOfBinaryExpressionAnalyzer, FixBracketFormattingOfBinaryExpressionFixProvider>
{
    public override DiagnosticDescriptor Descriptor { get; } = DiagnosticRules.FixBracketFormattingOfBinaryExpression;

    public override CSharpTestOptions Options
        => base.Options.AddConfigOption(ConfigOptionKeys.TargetBracesStyle, "both");

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.FixBracketFormattingOfBinaryExpression)]
    public async Task Inserts_new_line_Before_and_After_binary_expression()
    {
        await VerifyDiagnosticAndFixAsync(
            """
            class C
            {
                void M()
                {
                    bool x = false;
                    bool y = false;
                    bool z = false;

                    if [|(x &&
                        y &&
                        z)|]
                    {
                    }
                }
            }
            """,
            """
            class C
            {
                void M()
                {
                    bool x = false;
                    bool y = false;
                    bool z = false;

                    if (
                        x &&
                        y &&
                        z
                    )
                    {
                    }
                }
            }
            """
        );
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.FixBracketFormattingOfBinaryExpression)]
    public async Task Inserts_new_line_Before_and_After_binary_expression_2()
    {
        await VerifyDiagnosticAndFixAsync(
            """
            class C
            {
                void M()
                {
                    bool x = false;
                    bool y = false;
                    bool z = false;

                    if [|(x
                        && y
                        && z)|]
                    {
                    }
                }
            }
            """,
            """
            class C
            {
                void M()
                {
                    bool x = false;
                    bool y = false;
                    bool z = false;

                    if (
                        x
                        && y
                        && z
                    )
                    {
                    }
                }
            }
            """
        );
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.FixBracketFormattingOfBinaryExpression)]
    public async Task Inserts_new_line_Before_and_After_binary_expression_3()
    {
        await VerifyDiagnosticAndFixAsync(
            """
            class C
            {
                void M()
                {
                    bool x = false;
                    bool y = false;
                    bool z = false;

                    if (x && y)
                    {
                    }
                    else if [|(x
                        && y
                        && z)|]
                    {
                    }
                }
            }
            """,
            """
            class C
            {
                void M()
                {
                    bool x = false;
                    bool y = false;
                    bool z = false;

                    if (x && y)
                    {
                    }
                    else if (
                        x
                        && y
                        && z
                    )
                    {
                    }
                }
            }
            """
        );
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.FixBracketFormattingOfBinaryExpression)]
    public async Task Inserts_new_line_Before_and_After_binary_expression_4()
    {
        await VerifyDiagnosticAndFixAsync(
            """
            class C
            {
                void M()
                {
                    bool x = false;
                    bool y = false;
                    bool z = false;

                    if [|(x
                        || y
                        || z)|]
                    {
                    }
                }
            }
            """,
            """
            class C
            {
                void M()
                {
                    bool x = false;
                    bool y = false;
                    bool z = false;

                    if (
                        x
                        || y
                        || z
                    )
                    {
                    }
                }
            }
            """
        );
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.FixBracketFormattingOfBinaryExpression)]
    public async Task Inserts_new_line_Before_and_After_binary_expression_5()
    {
        await VerifyDiagnosticAndFixAsync(
            """
            class C
            {
                void M()
                {
                    bool x = false;
                    bool y = false;
                    bool z = false;

                    if [|(F()
                        || F()
                        || F())|]
                    {
                    }
                }

                bool F() => true;
            }
            """,
            """
            class C
            {
                void M()
                {
                    bool x = false;
                    bool y = false;
                    bool z = false;

                    if (
                        F()
                        || F()
                        || F()
                    )
                    {
                    }
                }

                bool F() => true;
            }
            """
        );
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.FixBracketFormattingOfBinaryExpression)]
    public async Task Inserts_new_line_Before_and_After_binary_expression_6()
    {
        await VerifyDiagnosticAndFixAsync(
            """
            class C
            {
                void M()
                {
                    bool x = false;
                    bool y = false;
                    bool z = false;

                    while [|(x
                        || y
                        || z)|]
                    {
                    }
                }
            }
            """,
            """
            class C
            {
                void M()
                {
                    bool x = false;
                    bool y = false;
                    bool z = false;

                    while (
                        x
                        || y
                        || z
                    )
                    {
                    }
                }
            }
            """
        );
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.FixBracketFormattingOfBinaryExpression)]
    public async Task Inserts_new_line_Before_and_After_binary_expression_7()
    {
        await VerifyDiagnosticAndFixAsync(
            """
            class C
            {
                void M()
                {
                    bool x = false;
                    bool y = false;
                    bool z = false;

                    do {}
                    while [|(x
                        || y
                        || z)|];
                }
            }
            """,
            """
            class C
            {
                void M()
                {
                    bool x = false;
                    bool y = false;
                    bool z = false;

                    do {}
                    while (
                        x
                        || y
                        || z
                    );
                }
            }
            """
        );
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.FixBracketFormattingOfBinaryExpression)]
    public async Task NoDiagnostic_inside_for()
    {
        await VerifyNoDiagnosticAsync(
            """
            class C
            {
                void M()
                {
                    bool x = false;
                    bool y = false;
                    bool z = false;

                    for (int i = 0; 
                        x || y || z; i++)
                    {
                    }
                }
            }
            """
        );
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.FixBracketFormattingOfBinaryExpression)]
    public async Task TestNoDiagnostic()
    {
        await VerifyNoDiagnosticAsync(
            """
            class C
            {
                void M()
                {
                    bool x = false;
                    bool y = false;
                    bool z = false;

                    if (
                        x
                        && y
                        && z
                    )
                    {
                    }
                }
            }
            """
        );
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.FixBracketFormattingOfBinaryExpression)]
    public async Task TestNoDiagnostic_SingleLine()
    {
        await VerifyNoDiagnosticAsync(
            """
            class C
            {
                void M()
                {
                    bool x = false;
                    bool y = false;
                    bool z = false;

                    if (x && y && z)
                    {
                    }
                }
            }
            """
        );
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.FixBracketFormattingOfBinaryExpression)]
    public async Task Test_Expression_in_unnecessary_parentheses()
    {
        await VerifyDiagnosticAndFixAsync(
            """
            class C
            {
                void M()
                {
                    bool x = false;
                    bool y = false;
                    bool z = false;

                    if [|([|(x
                            || y)|])|]
                    {
                    }
                }
            }
            """,
            """
            class C
            {
                void M()
                {
                    bool x = false;
                    bool y = false;
                    bool z = false;

                    if (
                        (
                            x
                            || y
                        )
                    )
                    {
                    }
                }
            }
            """
        );
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.FixBracketFormattingOfBinaryExpression)]
    public async Task Test_Expression_in_unnecessary_parentheses2()
    {
        await VerifyDiagnosticAndFixAsync(
            """
            class C
            {
                void M()
                {
                    bool x = false;
                    bool y = false;
                    bool z = false;

                    if [|(
                        (x || y))|]
                    {
                    }
                }
            }
            """,
            """
            class C
            {
                void M()
                {
                    bool x = false;
                    bool y = false;
                    bool z = false;

                    if (
                        (x || y)
                    )
                    {
                    }
                }
            }
            """
        );
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.FixBracketFormattingOfBinaryExpression)]
    public async Task Test_Expression_in_unnecessary_parentheses3()
    {
        await VerifyDiagnosticAndFixAsync(
            """
            class C
            {
                void M()
                {
                    bool x = false;
                    bool y = false;
                    bool z = false;

                    if [|(
                        (((x || y))))|]
                    {
                    }
                }
            }
            """,
            """
            class C
            {
                void M()
                {
                    bool x = false;
                    bool y = false;
                    bool z = false;

                    if (
                        (((x || y)))
                    )
                    {
                    }
                }
            }
            """
        );
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.FixBracketFormattingOfBinaryExpression)]
    public async Task Test_NoDiagnostic_in_case_of_unnecessary_parentheses()
    {
        await VerifyNoDiagnosticAsync(
            """
            class C
            {
                void M()
                {
                    bool x = false;
                    bool y = false;
                    bool z = false;

                    if (
                        (((x || y)))
                    )
                    {
                    }
                }
            }
            """
        );
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.FixBracketFormattingOfBinaryExpression)]
    public async Task Inserts_new_line_Before_and_After_binary_expression_for_nested()
    {
        await VerifyDiagnosticAndFixAsync(
            """
            class C
            {
                void M()
                {
                    bool x = false;
                    bool y = false;
                    bool z = false;
                    bool w = false;
                    bool v = false;

                    if [|(x
                        && [|(y
                            || w)|]
                        && (z || v))|]
                    {
                    }
                }
            }
            """,
            """
            class C
            {
                void M()
                {
                    bool x = false;
                    bool y = false;
                    bool z = false;
                    bool w = false;
                    bool v = false;

                    if (
                        x
                        && (
                            y
                            || w
                        )
                        && (z || v)
                    )
                    {
                    }
                }
            }
            """
        );
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.FixBracketFormattingOfBinaryExpression)]
    public async Task Inserts_new_line_Before_and_After_binary_expression_not_if_case()
    {
        await VerifyDiagnosticAndFixAsync(
            """
            class C
            {
                void M()
                {
                    bool x = false;
                    bool y = false;
                    bool z = false;
                    bool w = false;
                    bool v = false;

                    bool result = [|(x
                        && [|(y
                            || w)|]
                        && (z || v))|];
                }
            }
            """,
            """
            class C
            {
                void M()
                {
                    bool x = false;
                    bool y = false;
                    bool z = false;
                    bool w = false;
                    bool v = false;

                    bool result = (
                        x
                        && (
                            y
                            || w
                        )
                        && (z || v)
                    );
                }
            }
            """
        );
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.FixBracketFormattingOfBinaryExpression)]
    public async Task Inserts_new_line_Before_and_After_binary_expression_not_if_case2()
    {
        await VerifyDiagnosticAndFixAsync(
            """
            class C
            {
                void M()
                {
                    bool x = false;
                    bool y = false;
                    bool z = false;
                    bool w = false;
                    bool v = false;

                    bool result = 
                        [|(x
                            && [|(y
                                || w)|]
                            && (z || v))|];
                }
            }
            """,
            """
            class C
            {
                void M()
                {
                    bool x = false;
                    bool y = false;
                    bool z = false;
                    bool w = false;
                    bool v = false;

                    bool result = 
                        (
                            x
                            && (
                                y
                                || w
                            )
                            && (z || v)
                        );
                }
            }
            """
        );
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.FixBracketFormattingOfBinaryExpression)]
    public async Task Inserts_new_line_Before_and_After_binary_expression_for_ternary_operator()
    {
        await VerifyDiagnosticAndFixAsync(
            """
            class C
            {
                void M()
                {
                    bool x = false;
                    bool y = false;
                    bool z = false;
                    bool w = false;
                    bool v = false;

                    bool result = 
                        [|(x
                            && [|(y
                                || w)|])|]
                        ? [|(z 
                            || v)|]
                        : [|(x 
                            && y)|];
                }
            }
            """,
            """
            class C
            {
                void M()
                {
                    bool x = false;
                    bool y = false;
                    bool z = false;
                    bool w = false;
                    bool v = false;

                    bool result = 
                        (
                            x
                            && (
                                y
                                || w
                            )
                        )
                        ? (
                            z 
                            || v
                        )
                        : (
                            x 
                            && y
                        );
                }
            }
            """
        );
    }
}
