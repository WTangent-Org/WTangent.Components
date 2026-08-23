namespace WTangent.Core;

/// <summary>统一日志接口：空壳实现，组件经全局门面 <see cref="Log"/> 使用（任意位置 Log.Info）。
/// 全组件统一级别与格式，组件不自行输出。</summary>
public interface ILogger
{
    void Debug(string msg);
    void Info(string msg);
    void Warn(string msg);
    void Error(string msg, Exception? ex = null);
}
