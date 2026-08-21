namespace WTangent.Core;

/// <summary>事件总线：string 键 + 任意值，开放任意键（发布者自定），组件与宿主全向广播。
/// 约定键由宿主发布：app.startup / app.ready / app.shutdown / ui.activated / ui.deactivated /
/// store.remotes.changed / config.changed / session.started / session.ended。
/// 线程安全；同步发布；单个 handler 抛异常只记日志，不中断其他 handler。</summary>
public interface IEventBus
{
    /// <summary>发布事件（键任意，payload 任意对象/值）</summary>
    void Publish(string key, object? payload);

    /// <summary>订阅事件，返回退订句柄（Dispose 即退订）</summary>
    IDisposable Subscribe(string key, Action<object?> handler);
}
