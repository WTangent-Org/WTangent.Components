namespace WTangent.Core;

/// <summary>统一日志接口：空壳（Client 接收器）实现，注入所有组件。
/// 组件代码一般直接用全局门面 <see cref="Log"/>（免 Entry.App.Logger 长链）；
/// 本接口为门面背后的契约，同时为兼容旧组件保留 Application.Logger 通道。
/// 全组件统一级别与格式，组件不自行输出。</summary>
public interface ILogger
{
    void Debug(string msg);
    void Info(string msg);
    void Warn(string msg);
    void Error(string msg, Exception? ex = null);
}
