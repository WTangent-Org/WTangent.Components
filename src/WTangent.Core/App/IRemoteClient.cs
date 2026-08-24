namespace WTangent.Core;

/// <summary>服务器条目（remotes.json）：Name 标签、Host IP、Port 端口（分离存储，http 自动拼）、EtCode EasyTier 加入码（可空）、Kind 传输类型。
/// lan = 局域网直连（无加入码）；et = 跨网（加入码）。名字唯一标识；账号凭据是**全局**的（AgentCredentials，不按服务器记账）。
/// default-server = last-used 缓存（ServerRegistry.GetLastUsed），不存这里。
/// （原为 3 参数版本；ServerRegistry 并入 Core 时按规则 6 并型到此，EtCode/Kind/Url 来自 WTangent.Client/Store/ServerRegistry.cs 版）</summary>
public sealed record RemoteEntry(string Name, string Host, int Port, string? EtCode = null, string Kind = "lan")
{
    public string Url => $"http://{Host}:{Port}";
}

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
