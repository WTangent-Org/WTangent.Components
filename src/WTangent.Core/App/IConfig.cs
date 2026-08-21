namespace WTangent.Core;

/// <summary>配置读写（config.json）：变更发 config.changed 事件。线程安全。</summary>
public interface IConfig
{
    T? Get<T>(string key);
    void Set<T>(string key, T value);
    void Remove(string key);
}
