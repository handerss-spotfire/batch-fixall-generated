using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace AnalyzerProject;

[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(AddPotatoPrefixAnalyzer)), Shared]
public class AddPotatoPrefixCodeFixProvider : CodeFixProvider
{
    private const string CodeFixTitle = "Add potato prefix";

    public override ImmutableArray<string> FixableDiagnosticIds => ImmutableArray.Create(AddPotatoPrefixAnalyzer.DiagnosticId);

    public override async Task RegisterCodeFixesAsync(CodeFixContext context)
    {
        var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);
        var diagnostic = context.Diagnostics.First();
        var diagnosticSpan = diagnostic.Location.SourceSpan;
        var node = root.FindToken(diagnosticSpan.Start).Parent;
        var variable = node.AncestorsAndSelf().OfType<VariableDeclarationSyntax>().First();

        context.RegisterCodeFix(
            CodeAction.Create(title: CodeFixTitle, createChangedDocument: c => AddPotatoPrefix(context.Document, variable, c), equivalenceKey: CodeFixTitle), 
            diagnostic);
    }

    private static async Task<Document> AddPotatoPrefix(Document document, VariableDeclarationSyntax variable, CancellationToken c)
    {
        var syntaxRoot = await document.GetSyntaxRootAsync(c);
        var newVariable = variable.Variables[0];
        newVariable = newVariable.WithIdentifier(SyntaxFactory.Identifier($"potato{newVariable.Identifier.Text}"));
        syntaxRoot = syntaxRoot.ReplaceNode(variable, variable.WithVariables(SyntaxFactory.SeparatedList(new [] { newVariable })));
        return document.WithSyntaxRoot(syntaxRoot);
    }

    public override FixAllProvider GetFixAllProvider()
    {
        return WellKnownFixAllProviders.BatchFixer;
    }
}