namespace WTangent.Core;

/// <summary>GUI 宿主契约（未来）：gui 组件经此注册/挂载视图到 Client 接收器的宿主窗口。
/// tui 组件不需要。</summary>
public interface IGuiHost
{
    /// <summary>挂载主视图（gui 组件调用一次）</summary>
    void ShowView(object view);

    /// <summary>卸载视图</summary>
    void CloseView(object view);
}
