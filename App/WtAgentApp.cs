using System.Net.Http;

namespace WTangent.Core;

/// <summary>组件运行时上下文：空壳（Client 接收器）启动时构造同一实例，经生成器注入每个组件
/// （Entry.App，引用传递）。组件间不互引 dll，协作全走 App：
/// 下行 = 宿主注入能力；上行 = Services 注册能力 + Events 广播。
/// 本类只承载契约（接口），实现在宿主；组件引 WTangent.Core 包即可用。</summary>
public sealed class WtAgentApp
{
    public ILogger Logger { get; init; } = null!;
    public IEventBus Events { get; init; } = null!;
    public IConfig Config { get; init; } = null!;
    public IAppStore Store { get; init; } = null!;
    public IRemoteClient Remote { get; init; } = null!;
    public IGuiHost GuiHost { get; init; } = null!;
    public HttpClient Http { get; init; } = null!;
    public IServiceRegistry Services { get; init; } = null!;
}
