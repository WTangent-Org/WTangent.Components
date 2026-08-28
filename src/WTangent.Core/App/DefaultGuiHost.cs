namespace WTangent.Core;

/// <summary>GUI 宿主占位默认实现（未来 gui 组件挂载视图用）。
/// 原 WTangent/Host/GuiHost.cs，空壳削薄并入 Core。</summary>
public sealed class DefaultGuiHost : IGuiHost
{
    public void ShowView(object view) { }
    public void CloseView(object view) { }
}
