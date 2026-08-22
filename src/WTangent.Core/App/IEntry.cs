using System.CommandLine;

namespace WTangent.Core;

/// <summary>组件入口契约：手写实现（组件形态自由组合）。
/// 壳加载 dll 后找 IEntry 实现 → 实例化 → 注入 App → StartAsync → 按能力注册：
/// Commands 非空 → 注册命令（含父路径挂接）；Default 非空 → 顶级行为；Tools 非空 → serve 合并工具。</summary>
public interface IEntry
{
    /// <summary>宿主运行时上下文（构造注入，只读；组件内静态访问：Entry.Current.App）</summary>
    Application App { get; }

    /// <summary>组件标识（= components.json 索引别名，如 serve / tui / client / git）</summary>
    string Identifier { get; }

    /// <summary>组件显示名</summary>
    string Name { get; }

    /// <summary>组件命令：(命令, 父路径) 列表。父路径 null/空 = 顶级（挂根）；
    /// 非空 = 挂到该路径命令下（路径含根名 root，如 "root/remote/user"）。</summary>
    (Command Command, string? ParentPath)[] Commands => [];

    /// <summary>顶级行为（无子命令时执行；null = 无顶级行为）</summary>
    Func<string[], int>? Default => null;

    /// <summary>LLM 工具列表（serve 启动时合并）</summary>
    IReadOnlyList<ITool> Tools => [];

    /// <summary>是否支持异步启动：true = StartAsync 可与其他组件并行；false = 顺序串行（默认）</summary>
    bool SupportAsyncStart => false;

    /// <summary>作用域（事件键/服务命名前缀；缺省 = Identifier）</summary>
    string Scope => Identifier;

    /// <summary>启动：宿主注入 Application（日志/事件/配置/存储/远程/服务注册表）；挂事件订阅在此</summary>
    Task StartAsync(Application app) => Task.CompletedTask;

    /// <summary>停止：清理资源（宿主退出时调用）</summary>
    Task StopAsync() => Task.CompletedTask;
}
