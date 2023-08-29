using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace AnalyzerProject;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class AddPotatoPrefixAnalyzer : DiagnosticAnalyzer
{
    public const string DiagnosticId = "POTATO1";

    public static readonly DiagnosticDescriptor Descriptor = new(DiagnosticId, string.Empty, "The variable '{0}' lacks the 'potato' prefix",
        "Design", DiagnosticSeverity.Warning, isEnabledByDefault: true);

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Descriptor);

    public override void Initialize(AnalysisContext context)
    {
        context.EnableConcurrentExecution();
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.Analyze | GeneratedCodeAnalysisFlags.ReportDiagnostics);

        context.RegisterCompilationStartAction(comContext =>
        {
            comContext.RegisterSyntaxNodeAction(AnalyzeSyntaxNode, SyntaxKind.VariableDeclaration);
        });
    }

    private void AnalyzeSyntaxNode(SyntaxNodeAnalysisContext context)
    {
        if (context.Node is VariableDeclarationSyntax variable)
        {
            var varName = variable.Variables[0].Identifier.Text;
            if (!varName.StartsWith("potato"))
            {
                var diagnostic = Diagnostic.Create(Descriptor, context.Node.GetLocation(), varName);
                context.ReportDiagnostic(diagnostic);
            }
        } 
    }
}
