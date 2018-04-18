namespace Gu.Localization.Analyzers.Tests.UseResourceTests
{
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Gu.Roslyn.Asserts;
    using Microsoft.CodeAnalysis.CodeFixes;
    using Microsoft.CodeAnalysis.Diagnostics;
    using NUnit.Framework;

    public class CodeFix
    {
        private static readonly DiagnosticAnalyzer Analyzer = new LiteralAnalyzer();
        private static readonly CodeFixProvider Fix = new UseResourceFix();
        private static readonly ExpectedDiagnostic ExpectedDiagnostic = ExpectedDiagnostic.Create("GULOC03");

        [Explicit]
        [Test]
        public async Task ForDebug()
        {
            var projectFile = CodeFactory.FindProjectFile("Gu.Localization.TestStub.csproj");
            var sln = CodeFactory.CreateSolution(projectFile, MetadataReferences.FromAttributes());
            sln = sln.WithProjectFilePath(sln.ProjectIds[0], projectFile.FullName);
            var diagnosticsAsync = await Analyze.GetDiagnosticsAsync(sln, Analyzer).ConfigureAwait(false);
            await HackFix.ApplyAsync(sln, Fix, diagnosticsAsync.Single().Single(), null, CancellationToken.None).ConfigureAwait(false);
        }
    }
}