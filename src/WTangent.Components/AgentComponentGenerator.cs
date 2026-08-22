using System;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace WTangent.Components;

/// <summary>增量源生成器（scope 式依赖收集 + 生命周期接线）：
/// [AgentComponent] → Commands 属性直接生成（含父路径）；[AgentTool] → Tools 属性；
/// [Entry] 元数据 → Identifier（RootNamespace 末段小写，id 覆盖）/ Name / SupportAsyncStart；
/// [EntryScope] → Scope；[EntryStart]/[EntryStop] 钩子 → StartAsync/StopAsync（检测 async）；
/// [AgentEvent] 方法 → 事件订阅接线进 StartAsync。手写 Entry 只剩钩子和声明。</summary>
[Generator]
public sealed class AgentComponentGenerator : IIncrementalGenerator
{
    private const string ComponentAttr = "WTangent.Components.AgentComponentAttribute";
    private const string ToolAttr = "WTangent.Components.AgentToolAttribute";
    private const string EventAttr = "WTangent.Components.AgentEventAttribute";
    private const string EntryAttr = "WTangent.Components.EntryAttribute";
    private const string EntryScopeAttr = "WTangent.Components.EntryScopeAttribute";
    private const string EntryStartAttr = "WTangent.Components.EntryStartAttribute";
    private const string EntryStopAttr = "WTangent.Components.EntryStopAttribute";

    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var commands = CollectClasses(context, ComponentAttr);
        var tools = CollectClasses(context, ToolAttr);
        var events = CollectMethods(context, EventAttr);
        var entries = CollectClasses(context, EntryAttr);
        var scopes = CollectClasses(context, EntryScopeAttr);

        var rootNs = context.AnalyzerConfigOptionsProvider
            .Select(static (p, _) =>
            {
                p.GlobalOptions.TryGetValue("build_property.RootNamespace", out var ns);
                return ns is { Length: > 0 } ? ns : "Agent";
            });

        context.RegisterSourceOutput(
            commands.Combine(tools).Combine(events).Combine(entries).Combine(scopes).Combine(rootNs),
            static (spc, pair) => Emit(spc, pair.Left.Left.Left.Left.Left, pair.Left.Left.Left.Left.Right,
                pair.Left.Left.Left.Right, pair.Left.Left.Right, pair.Left.Right, pair.Right));
    }

    private static IncrementalValueProvider<ImmutableArray<INamedTypeSymbol>> CollectClasses(
        IncrementalGeneratorInitializationContext context, string attr) =>
        context.SyntaxProvider.CreateSyntaxProvider(
                static (node, _) => node is ClassDeclarationSyntax,
                (ctx, _) => GetAttributedClass(ctx, attr))
            .Where(static s => s is not null).Select(static (s, _) => s!).Collect();

    private static IncrementalValueProvider<ImmutableArray<IMethodSymbol>> CollectMethods(
        IncrementalGeneratorInitializationContext context, string attr) =>
        context.SyntaxProvider.CreateSyntaxProvider(
                static (node, _) => node is MethodDeclarationSyntax,
                (ctx, _) => GetAttributedMethod(ctx, attr))
            .Where(static s => s is not null).Select(static (s, _) => s!).Collect();

    private static INamedTypeSymbol? GetAttributedClass(GeneratorSyntaxContext ctx, string attrFullName) =>
        ctx.Node is ClassDeclarationSyntax cls
        && ctx.SemanticModel.GetDeclaredSymbol(cls) is INamedTypeSymbol sym
        && HasAttribute(sym, attrFullName) ? sym : null;

    private static IMethodSymbol? GetAttributedMethod(GeneratorSyntaxContext ctx, string attrFullName) =>
        ctx.Node is MethodDeclarationSyntax mtd
        && ctx.SemanticModel.GetDeclaredSymbol(mtd) is IMethodSymbol sym
        && HasAttribute(sym, attrFullName) ? sym : null;

    private static bool HasAttribute(ISymbol symbol, string attrFullName) =>
        symbol.GetAttributes().Any(a => a.AttributeClass?.ToDisplayString() == attrFullName);

    private static void Emit(SourceProductionContext spc,
        ImmutableArray<INamedTypeSymbol> commands, ImmutableArray<INamedTypeSymbol> tools,
        ImmutableArray<IMethodSymbol> events, ImmutableArray<INamedTypeSymbol> entries,
        ImmutableArray<INamedTypeSymbol> scopes, string rootNs)
    {
        if (commands.IsDefaultOrEmpty && tools.IsDefaultOrEmpty && events.IsDefaultOrEmpty
            && entries.IsDefaultOrEmpty && scopes.IsDefaultOrEmpty) return;

        // [Entry] 元数据 + 生命周期钩子（在带 [Entry] 的类上找 [EntryStart]/[EntryStop]）
        string? id = null; bool isAsync = false; IMethodSymbol? startHook = null; IMethodSymbol? stopHook = null;
        foreach (var e in entries)
        {
            var (eid, easync) = ReadEntryAttr(e);
            if (eid is not null) id = eid;
            isAsync = easync;
            startHook ??= e.GetMembers().OfType<IMethodSymbol>()
                .FirstOrDefault(m => HasAttribute(m, EntryStartAttr));
            stopHook ??= e.GetMembers().OfType<IMethodSymbol>()
                .FirstOrDefault(m => HasAttribute(m, EntryStopAttr));
        }
        id ??= rootNs.Split('.').Last().ToLowerInvariant();
        var scope = scopes.Select(ReadScopeAttr).FirstOrDefault(s => s is not null);

        var sb = new StringBuilder();
        sb.AppendLine("// <auto-generated by Components.AgentComponentGenerator />");
        sb.AppendLine("#nullable enable");
        sb.AppendLine();
        sb.AppendLine($"namespace {rootNs}");
        sb.AppendLine("{");
        sb.AppendLine("    /// <summary>Entry 的生成部分（元数据 + 收集产物 + 生命周期接线）：手写 partial Entry 只留钩子</summary>");
        sb.AppendLine("    public sealed partial class Entry");
        sb.AppendLine("    {");
        sb.AppendLine("        /// <summary>组件标识（[Entry] id 覆盖或 RootNamespace 末段小写）</summary>");
        sb.AppendLine($"        public string Identifier => \"{id}\";");
        sb.AppendLine("        /// <summary>组件显示名（= Identifier）</summary>");
        sb.AppendLine($"        public string Name => \"{id}\";");
        sb.AppendLine("        /// <summary>是否支持异步启动（[Entry] isAsync）</summary>");
        sb.AppendLine($"        public bool SupportAsyncStart => {(isAsync ? "true" : "false")};");
        if (scope is not null)
        {
            sb.AppendLine("        /// <summary>作用域（[EntryScope]）</summary>");
            sb.AppendLine($"        public string Scope => \"{scope}\";");
        }
        if (commands.Length > 0)
        {
            sb.AppendLine("        /// <summary>收集的组件命令（[AgentComponent]）：(命令, 父路径)</summary>");
            sb.AppendLine("        public (System.CommandLine.Command Command, string? ParentPath)[] Commands =>");
            sb.AppendLine("        [");
            foreach (var cmd in commands)
            {
                var name = GetCommandName(cmd);
                var parent = ReadParentAttr(cmd);
                var fullType = cmd.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
                var parentLit = parent is null ? "null" : $"\"{parent}\"";
                sb.AppendLine($"            (new {fullType}(), {parentLit}),   // {name}");
            }
            sb.AppendLine("        ];");
        }
        if (tools.Length > 0)
        {
            sb.AppendLine("        /// <summary>收集的 LLM 工具（[AgentTool]）</summary>");
            sb.AppendLine("        public System.Collections.Generic.IReadOnlyList<WTangent.Core.ITool> Tools =>");
            sb.AppendLine("        [");
            foreach (var t in tools)
            {
                var fullType = t.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
                sb.AppendLine($"            new {fullType}(),   // {t.Name}");
            }
            sb.AppendLine("        ];");
        }
        // 生命周期：StartAsync = 事件订阅接线 + [EntryStart] 钩子；StopAsync = [EntryStop] 钩子
        if (events.Length > 0 || startHook is not null)
        {
            var startAsync = startHook is not null && ReturnsTask(startHook);
            sb.AppendLine("        /// <summary>启动：事件订阅接线 + [EntryStart] 钩子（生成器检测 async）</summary>");
            sb.AppendLine(startAsync
                ? "        public System.Threading.Tasks.Task StartAsync(WTangent.Core.Application app)"
                : "        public System.Threading.Tasks.Task StartAsync(WTangent.Core.Application app)");
            sb.AppendLine("        {");
            foreach (var m in events)
            {
                var key = ReadEventKey(m);
                if (key is null) continue;
                var fmt = SymbolDisplayFormat.FullyQualifiedFormat.AddMemberOptions(SymbolDisplayMemberOptions.IncludeContainingType);
                sb.AppendLine($"            app.Events.Subscribe(\"{key}\", {m.ToDisplayString(fmt)});");
            }
            if (startHook is not null)
                sb.AppendLine(startAsync
                    ? $"            return {startHook.Name}(app);"
                    : $"            {startHook.Name}(app);");
            if (!startAsync)
                sb.AppendLine("            return System.Threading.Tasks.Task.CompletedTask;");
            sb.AppendLine("        }");
        }
        if (stopHook is not null)
        {
            var stopAsync = ReturnsTask(stopHook);
            sb.AppendLine("        /// <summary>停止：[EntryStop] 钩子（生成器检测 async）</summary>");
            sb.AppendLine("        public System.Threading.Tasks.Task StopAsync()");
            sb.AppendLine("        {");
            sb.AppendLine(stopAsync
                ? $"            return {stopHook.Name}();"
                : $"            {stopHook.Name}();");
            if (!stopAsync)
                sb.AppendLine("            return System.Threading.Tasks.Task.CompletedTask;");
            sb.AppendLine("        }");
        }
        sb.AppendLine("    }");
        sb.AppendLine("}");

        spc.AddSource("Entry.g.cs", SourceText.From(sb.ToString(), Encoding.UTF8));
    }

    private static bool ReturnsTask(IMethodSymbol m) =>
        m.ReturnType.Name is "Task" or "ValueTask";

    private static (string? Id, bool Async) ReadEntryAttr(INamedTypeSymbol type)
    {
        foreach (var attr in type.GetAttributes().Where(a => a.AttributeClass?.ToDisplayString() == EntryAttr))
        {
            var isAsync = attr.ConstructorArguments.ElementAtOrDefault(0).Value is true;
            var id = attr.ConstructorArguments.ElementAtOrDefault(1).Value as string;
            return (id, isAsync);
        }
        return (null, false);
    }

    private static string? ReadScopeAttr(INamedTypeSymbol type) =>
        type.GetAttributes().Where(a => a.AttributeClass?.ToDisplayString() == EntryScopeAttr)
            .Select(a => a.ConstructorArguments.ElementAtOrDefault(0).Value as string)
            .FirstOrDefault();

    private static string? ReadParentAttr(INamedTypeSymbol type) =>
        type.GetAttributes().Where(a => a.AttributeClass?.ToDisplayString() == ComponentAttr)
            .SelectMany(a => a.NamedArguments)
            .Where(kv => kv is { Key: "Parent", Value.Value: string { Length: > 0 } })
            .Select(kv => (string?)kv.Value.Value)
            .FirstOrDefault();

    private static string? ReadEventKey(IMethodSymbol method) =>
        method.GetAttributes().Where(a => a.AttributeClass?.ToDisplayString() == EventAttr)
            .Select(a => a.ConstructorArguments.ElementAtOrDefault(0).Value as string)
            .FirstOrDefault();

    private static string GetCommandName(INamedTypeSymbol cmd)
    {
        foreach (var kv in cmd.GetAttributes()
                     .Where(attr => attr.AttributeClass?.ToDisplayString() == ComponentAttr)
                     .SelectMany(attr => attr.NamedArguments))
        {
            if (kv is { Key: "Name", Value.Value: string { Length: > 0 } s })
                return s;
        }
        var name = cmd.Name;
        const string suffix = "Command";
        if (name.EndsWith(suffix, StringComparison.Ordinal) && name.Length > suffix.Length)
            name = name.Substring(0, name.Length - suffix.Length);
        return char.ToLowerInvariant(name[0]) + name.Substring(1);
    }
}
