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
    private const string ComponentAttr = "WTangent.Components.AgentCommandAttribute";
    private const string ToolAttr = "WTangent.Components.AgentToolAttribute";
    private const string EventAttr = "WTangent.Components.AgentEventAttribute";
    private const string EntryAttr = "WTangent.Components.AgentEntryAttribute";
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

        // 写 agent-component.json 需要的 MSBuild 属性（随包 buildTransitive props 声明为 CompilerVisibleProperty）
        var manifestProps = context.AnalyzerConfigOptionsProvider
            .Select(static (p, _) =>
            {
                p.GlobalOptions.TryGetValue("build_property.ProjectDir", out var dir);
                p.GlobalOptions.TryGetValue("build_property.AssemblyName", out var asm);
                return (ProjDir: dir, AsmName: asm);
            });

        context.RegisterSourceOutput(
            commands.Combine(tools).Combine(events).Combine(entries).Combine(scopes).Combine(rootNs)
                .Combine(context.CompilationProvider).Combine(manifestProps),
            static (spc, pair) =>
            {
                var left = pair.Left.Left;   // (((((commands, tools), events), entries), scopes), rootNs)
                Emit(spc, left.Left.Left.Left.Left.Left, left.Left.Left.Left.Left.Right,
                    left.Left.Left.Left.Right, left.Left.Left.Right, left.Left.Right, left.Right,
                    pair.Left.Right, pair.Right.ProjDir, pair.Right.AsmName);
            });
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
        ImmutableArray<INamedTypeSymbol> scopes, string rootNs,
        Compilation compilation, string? projDir, string? asmName)
    {
        if (commands.IsDefaultOrEmpty && tools.IsDefaultOrEmpty && events.IsDefaultOrEmpty
            && entries.IsDefaultOrEmpty && scopes.IsDefaultOrEmpty) return;

        // [Entry] 元数据 + 生命周期钩子（在带 [Entry] 的类上找 [EntryStart]/[EntryStop]）
        string? id = null; string? displayName = null; bool isAsync = false;
        IMethodSymbol? startHook = null; IMethodSymbol? stopHook = null;
        foreach (var e in entries)
        {
            var (eid, ename, easync) = ReadEntryAttr(e);
            if (eid is not null) id = eid;
            if (ename is not null) displayName = ename;
            isAsync = easync;
            startHook ??= e.GetMembers().OfType<IMethodSymbol>()
                .FirstOrDefault(m => HasAttribute(m, EntryStartAttr));
            stopHook ??= e.GetMembers().OfType<IMethodSymbol>()
                .FirstOrDefault(m => HasAttribute(m, EntryStopAttr));
        }
        id ??= rootNs.Split('.').Last().ToLowerInvariant();
        displayName ??= id;
        WriteManifest(projDir, asmName, id, commands, tools, compilation);
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
        sb.AppendLine("        /// <summary>宿主运行时上下文（静态，构造注入；组件代码直接 Entry.App）</summary>");
        sb.AppendLine("        public static WTangent.Core.Application App = null!;");
        sb.AppendLine("        /// <summary>构造注入：静态 App 赋值（PCL-CE 式；null! 为静态固有，组件作者不可见）</summary>");
        sb.AppendLine("        public Entry(WTangent.Core.Application app) => App = app;");
        sb.AppendLine("        /// <summary>组件标识（[Entry] id 覆盖或 RootNamespace 末段小写）</summary>");
        sb.AppendLine($"        public string Identifier => \"{id}\";");
        sb.AppendLine("        /// <summary>组件显示名（[Entry] name 覆盖或 = Identifier）</summary>");
        sb.AppendLine($"        public string Name => \"{displayName}\";");
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

    /// <summary>写 agent-component.json 到仓根（空壳 install 时拉取）：name/asset/minCore/commands/tools。
    /// minCore = 编译引用的 Core 程序集版本（空壳门禁：内置 Core 低于它则拒装）。
    /// 内容不变不写（避免反复触发增量重建）；写失败不炸构建，下次构建再写。</summary>
    private static void WriteManifest(string? projDir, string? asmName, string id,
        ImmutableArray<INamedTypeSymbol> commands, ImmutableArray<INamedTypeSymbol> tools,
        Compilation compilation)
    {
        if (projDir is not { Length: > 0 }) return;
        try
        {
            var coreVer = compilation.ReferencedAssemblyNames
                .FirstOrDefault(n => n.Name == "WTangent.Core")?.Version;
            var sb = new StringBuilder();
            sb.Append("{ \"name\": \"").Append(id)
              .Append("\", \"asset\": \"").Append(asmName is { Length: > 0 } ? asmName : id).Append('"');
            if (coreVer is not null)
                sb.Append(", \"minCore\": \"").Append(TrimRevision(coreVer)).Append('"');
            AppendNames(sb, "commands", commands, static c => GetCommandName(c));
            AppendNames(sb, "tools", tools, static t => t.Name);
            sb.AppendLine(" }");
            var path = System.IO.Path.Combine(projDir, "agent-component.json");
            var content = sb.ToString();
            if (System.IO.File.Exists(path) && System.IO.File.ReadAllText(path) == content) return;
            System.IO.File.WriteAllText(path, content);
        }
        catch { /* manifest 写失败忽略，下次构建再写 */ }
    }

    private static void AppendNames(StringBuilder sb, string key,
        ImmutableArray<INamedTypeSymbol> symbols, Func<INamedTypeSymbol, string> nameOf)
    {
        if (symbols.IsDefaultOrEmpty) return;
        sb.Append(", \"").Append(key).Append("\": [");
        var first = true;
        foreach (var s in symbols)
        {
            if (!first) sb.Append(", ");
            sb.Append('"').Append(nameOf(s)).Append('"');
            first = false;
        }
        sb.Append(']');
    }

    /// <summary>程序集版本去尾零修订号（0.0.9.0 → 0.0.9）</summary>
    private static string TrimRevision(Version v) =>
        v.Revision == 0 ? $"{v.Major}.{v.Minor}.{v.Build}" : v.ToString();

    private static bool ReturnsTask(IMethodSymbol m) =>
        m.ReturnType.Name is "Task" or "ValueTask";

    private static (string? Id, string? Name, bool Async) ReadEntryAttr(INamedTypeSymbol type)
    {
        foreach (var attr in type.GetAttributes().Where(a => a.AttributeClass?.ToDisplayString() == EntryAttr))
        {
            var id = attr.ConstructorArguments.ElementAtOrDefault(0).Value as string;
            var name = attr.ConstructorArguments.ElementAtOrDefault(1).Value as string;
            var isAsync = attr.ConstructorArguments.ElementAtOrDefault(2).Value is true;
            return (id, name, isAsync);
        }
        return (null, null, false);
    }

    private static string? ReadScopeAttr(INamedTypeSymbol type) =>
        type.GetAttributes().Where(a => a.AttributeClass?.ToDisplayString() == EntryScopeAttr)
            .Select(a => a.ConstructorArguments.ElementAtOrDefault(0).Value as string)
            .FirstOrDefault();

    private static string? ReadParentAttr(INamedTypeSymbol type) =>
        type.GetAttributes().Where(a => a.AttributeClass?.ToDisplayString() == ComponentAttr)
            .Select(a => a.ConstructorArguments.ElementAtOrDefault(0).Value as string)
            .FirstOrDefault(s => s is { Length: > 0 });

    private static string? ReadEventKey(IMethodSymbol method) =>
        method.GetAttributes().Where(a => a.AttributeClass?.ToDisplayString() == EventAttr)
            .Select(a => a.ConstructorArguments.ElementAtOrDefault(0).Value as string)
            .FirstOrDefault();

    private static string GetCommandName(INamedTypeSymbol cmd)
    {
        var name = cmd.Name;
        const string suffix = "Command";
        if (name.EndsWith(suffix, StringComparison.Ordinal) && name.Length > suffix.Length)
            name = name.Substring(0, name.Length - suffix.Length);
        return char.ToLowerInvariant(name[0]) + name.Substring(1);
    }
}
