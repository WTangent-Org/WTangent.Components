namespace WTangent.Core;

/// <summary>服务注册表（双向能力通道）：组件注册能力，宿主/兄弟组件解析调用。
/// 同类型重复注册显式失败（覆盖是 bug 源）；多实现用各自类型或命名注册。</summary>
public interface IServiceRegistry
{
    /// <summary>注册服务；同类型已注册则抛 InvalidOperationException</summary>
    void Register<T>(T impl) where T : class;

    /// <summary>尝试注册；冲突返回 false 不抛</summary>
    bool TryRegister<T>(T impl) where T : class;

    /// <summary>解析服务；未注册返回 null</summary>
    T? Resolve<T>() where T : class;
}
