namespace WTangent.Core;

/// <summary>服务注册表默认实现：类型 → 单例。同类型重复注册抛异常（覆盖是 bug 源）。
/// 原 WTangent/Host/ServiceRegistry.cs，空壳削薄并入 Core。</summary>
public sealed class DefaultServiceRegistry : IServiceRegistry
{
    private readonly Lock _lock = new();
    private readonly Dictionary<Type, object> _map = new();

    public void Register<T>(T impl) where T : class
    {
        using (_lock.EnterScope())
        {
            if (!_map.TryAdd(typeof(T), impl))
                throw new InvalidOperationException($"服务 {typeof(T).Name} 已注册");
        }
    }

    public bool TryRegister<T>(T impl) where T : class
    {
        using (_lock.EnterScope()) return _map.TryAdd(typeof(T), impl);
    }

    public T? Resolve<T>() where T : class
    {
        using (_lock.EnterScope()) return _map.TryGetValue(typeof(T), out var v) ? (T)v : null;
    }
}
