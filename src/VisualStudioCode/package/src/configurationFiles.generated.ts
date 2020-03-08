export const configurationFileContent = {
	ruleset: `<?xml version="1.0" encoding="utf-8"?>
<RuleSet Name="roslynator.ruleset" ToolsVersion="16.0">
  <!-- Specify default action that should be applied to all analyzers except those explicitly specified. -->
  <!-- <IncludeAll Action="None,Hidden,Info,Warning,Error" /> -->
  <Rules AnalyzerId="Roslynator.CSharp.Analyzers" RuleNamespace="Roslynator.CSharp.Analyzers">
    <!-- Specify default action that should be applied to a specified analyzer. -->
    <!-- <Rule Id="RCSxxxx" Action="None,Hidden,Info,Warning,Error" /> -->
  </Rules>
</RuleSet>`,
	config: `<?xml version="1.0" encoding="utf-8"?>
<Roslynator>
  <Settings>
    <General>
      <!-- <PrefixFieldIdentifierWithUnderscore>true</PrefixFieldIdentifierWithUnderscore> -->
    </General>
    <Refactorings>
      <!-- <Refactoring Id="RRxxxx" IsEnabled="false" /> -->
    </Refactorings>
    <CodeFixes>
      <!-- <CodeFix Id="CSxxxx.RCFxxxx" IsEnabled="false" /> -->
      <!-- <CodeFix Id="CSxxxx" IsEnabled="false" /> -->
      <!-- <CodeFix Id="RCFxxxx" IsEnabled="false" /> -->
    </CodeFixes>
  </Settings>
</Roslynator>`
};