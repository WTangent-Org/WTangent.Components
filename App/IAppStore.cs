namespace WTangent.Core;

/// <summary>数据存储（%APPDATA%\agent 下数据文件）：remotes.json / credentials 等。
/// 写为原子写（temp + rename）；变更发 store.*.changed 事件。线程安全。</summary>
public interface IAppStore
{
    /// <summary>读数据文件为文本（不存在返回 null）</summary>
    string? ReadText(string name);

    /// <summary>原子写数据文件（temp + rename 防半写）</summary>
    void WriteText(string name, string content);

    /// <summary>读 JSON 数据文件（不存在或解析失败返回 null）</summary>
    T? ReadJson<T>(string name);

    /// <summary>写 JSON 数据文件（原子写）</summary>
    void WriteJson<T>(string name, T value);
}
