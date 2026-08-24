namespace WTangent.Core;

/// <summary>组件运行时上下文：空壳启动时构造同一实例，经生成器注入每个组件
/// （Entry.App，引用传递）。组件间不互引 dll，协作全走 App：
/// 下行 = 宿主注入能力；上行 = Services 注册能力 + Events 广播。
/// 本类只承载契约（接口），实现在宿主；组件引 WTangent.Core 包即可用。
/// 日志/配置不进 App，走全局门面 Log/Config（单 ALC 下静态唯一）。</summary>
public sealed class Application
{
    public IEventBus Events { get; init; } = null!;
    public IAppStore Store { get; init; } = null!;
    public IRemoteClient Remote { get; init; } = null!;
    public IGuiHost GuiHost { get; init; } = null!;
    public HttpClient Http { get; init; } = null!;
    public IServiceRegistry Services { get; init; } = null!;
}
