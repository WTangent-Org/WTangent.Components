// 原 WTangent.Client/Session/IAgentEvents.cs（与 WTangent.Server/Session/AgentCore.cs 约 38 行的同名接口逐成员对照：六成员完全一致，无并集增量），跨仓去重并入 Core（逻辑未改）
namespace WTangent.Core;

/// <summary>Agent 会话事件回调（Pi 风格：turn / tool / message 生命周期）</summary>
public interface IAgentEvents
{
    /// <summary>一轮开始（LLM 收到 prompt）</summary>
    void OnTurnStart() { }
    /// <summary>LLM 回复增量（流式文本）</summary>
    void OnMessageDelta(string delta) { }
    /// <summary>思维链增量（reasoning 模型，可选显示）</summary>
    void OnReasoningDelta(string delta) { }
    /// <summary>工具开始执行</summary>
    void OnToolStart(string name, string arguments) { }
    /// <summary>工具执行完成</summary>
    void OnToolEnd(string name, string result) { }
    /// <summary>一轮完成（含工具结果）</summary>
    void OnTurnEnd(string? finalText) { }
    /// <summary>危险命令确认请求（y/n；回执走 confirm 通道）</summary>
    void OnConfirmReq(string id, string prompt) { }
    /// <summary>结构化提问请求（askuser 工具；回执走 answer 通道，selected = 选项 label）</summary>
    void OnQuestionReq(string id, string question, string header, string optionsJson) { }
}
