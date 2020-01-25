namespace Gu.Localization.Analyzers
{
    using Microsoft.CodeAnalysis;

    public static class GULOC01KeyExists
    {
        public static readonly DiagnosticDescriptor Descriptor = new DiagnosticDescriptor(
            id: "GULOC01",
            title: "Key does not exist.",
            messageFormat: "Key does not exist.",
            category: AnalyzerCategory.Correctness,
            defaultSeverity: DiagnosticSeverity.Warning,
            isEnabledByDefault: true,
            description: "Key does not exist.");
    }
}
