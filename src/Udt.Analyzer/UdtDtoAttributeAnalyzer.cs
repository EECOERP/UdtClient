using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace UdtClient.Analyzer;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class UdtDtoAttributeAnalyzer : DiagnosticAnalyzer
{
    private static readonly DiagnosticDescriptor MissingTable = new DiagnosticDescriptor(
        id: "UDT001",
        title: "Missing [UdtTable]",
        messageFormat: "'{0}' implements IUdtDto but is missing [UdtTable]",
        category: "UdtClient",
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true);

    private static readonly DiagnosticDescriptor MissingUid = new DiagnosticDescriptor(
        id: "UDT002",
        title: "Missing [UdtUid]",
        messageFormat: "'{0}' implements IUdtDto but has no property marked [UdtUid]",
        category: "UdtClient",
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true);

    private static readonly DiagnosticDescriptor MissingColumn = new DiagnosticDescriptor(
        id: "UDT003",
        title: "Missing [UdtColumn]",
        messageFormat: "'{0}' implements IUdtDto but has no properties marked [UdtColumn]",
        category: "UdtClient",
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true);

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics =>
        ImmutableArray.Create(MissingTable, MissingUid, MissingColumn);

    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();
        context.RegisterSymbolAction(Analyze, SymbolKind.NamedType);
    }

    private static void Analyze(SymbolAnalysisContext context)
    {
        var type = (INamedTypeSymbol)context.Symbol;

        if (type.TypeKind != TypeKind.Class || type.IsAbstract)
            return;

        var udtDto = context.Compilation.GetTypeByMetadataName("UdtClient.IUdtDto");
        if (udtDto is null)
            return;

        var implements = false;
        foreach (var iface in type.AllInterfaces)
        {
            if (SymbolEqualityComparer.Default.Equals(iface, udtDto))
            {
                implements = true;
                break;
            }
        }

        if (!implements)
            return;

        var udtTable = context.Compilation.GetTypeByMetadataName("UdtClient.UdtTableAttribute");
        var udtUid   = context.Compilation.GetTypeByMetadataName("UdtClient.UdtUidAttribute");
        var udtCol   = context.Compilation.GetTypeByMetadataName("UdtClient.UdtColumnAttribute");

        var location = type.Locations[0];

        if (udtTable != null && !HasAttribute(type.GetAttributes(), udtTable))
            context.ReportDiagnostic(Diagnostic.Create(MissingTable, location, type.Name));

        var properties = type.GetMembers().OfType<IPropertySymbol>();

        var hasUid = false;
        var hasColumn = false;

        foreach (var prop in properties)
        {
            var attrs = prop.GetAttributes();
            if (udtUid != null && HasAttribute(attrs, udtUid))
                hasUid = true;
            if (udtCol != null && HasAttribute(attrs, udtCol))
                hasColumn = true;
        }

        if (udtUid != null && !hasUid)
            context.ReportDiagnostic(Diagnostic.Create(MissingUid, location, type.Name));

        if (udtCol != null && !hasColumn)
            context.ReportDiagnostic(Diagnostic.Create(MissingColumn, location, type.Name));
    }

    private static bool HasAttribute(ImmutableArray<AttributeData> attributes, INamedTypeSymbol attributeType)
    {
        foreach (var attr in attributes)
        {
            if (SymbolEqualityComparer.Default.Equals(attr.AttributeClass, attributeType))
                return true;
        }
        return false;
    }
}