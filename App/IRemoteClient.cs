namespace WTangent.Core;

/// <summary>远程服务器条目（remotes.json）</summary>
public sealed record RemoteEntry(string Name, string Host, int Port);

/// <summary>远程连接能力：连接 serve（会话 API WS/SSE、run 一次性问答）。
/// 组件经 Entry.App.Remote 使用，不直接发 HTTP。</summary>
public interface IRemoteClient
{
    /// <summary>已注册远程列表（remotes.json）</summary>
    IReadOnlyList<RemoteEntry> ListRemotes();

    /// <summary>一次性问答（run）：只发 prompt，LLM 由 serve 调用</summary>
    Task<string?> AskAsync(string remote, string prompt, CancellationToken ct = default);

    /// <summary>流式会话（SSE 增量文本）</summary>
    IAsyncEnumerable<string> StreamAsync(string remote, string prompt, CancellationToken ct = default);
}
