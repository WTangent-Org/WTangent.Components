using System.Net.Http;

namespace WTangent.Core;

/// <summary>组件运行时上下文：空壳（Client 接收器）启动时构造同一实例，经生成器注入每个组件
/// （Entry.App，引用传递）。组件间不互引 dll，协作全走 App：
/// 下行 = 宿主注入能力；上行 = Services 注册能力 + Events 广播。
/// 本类只承载契约（接口），实现在宿主；组件引 WTangent.Core 包即可用。
/// 日志/配置另有全局门面 Log/Config（≥0.0.9），新代码优先用门面；Logger/Config 属性为兼容旧组件永不删除。</summary>
public sealed class Application
{
    /// <summary>日志（兼容通道）：新代码用全局门面 <see cref="Log"/>；宿主仍须赋值（旧组件运行时用）</summary>
    [Obsolete("新代码用全局门面 Log；此属性为兼容旧组件保留，宿主须继续赋值")]
    public ILogger Logger { get; init; } = null!;
    public IEventBus Events { get; init; } = null!;
    /// <summary>配置（兼容通道）：新代码用全局门面 <see cref="Config"/>；宿主仍须赋值（旧组件运行时用）</summary>
    [Obsolete("新代码用全局门面 Config；此属性为兼容旧组件保留，宿主须继续赋值")]
    public IConfig Config { get; init; } = null!;
    public IAppStore Store { get; init; } = null!;
    public IRemoteClient Remote { get; init; } = null!;
    public IGuiHost GuiHost { get; init; } = null!;
    public HttpClient Http { get; init; } = null!;
    public IServiceRegistry Services { get; init; } = null!;
}
