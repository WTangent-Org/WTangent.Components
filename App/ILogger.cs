namespace WTangent.Core;

/// <summary>统一日志接口：空壳（Client 接收器）实现，注入所有组件（Entry.App.Logger）。
/// 全组件统一级别与格式，组件不自行输出。</summary>
public interface ILogger
{
    void Debug(string msg);
    void Info(string msg);
    void Warn(string msg);
    void Error(string msg, Exception? ex = null);
}
