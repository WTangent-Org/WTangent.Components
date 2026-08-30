namespace WTangent.Core;

/// <summary>结构化提问的单个选项（kimi AskUser 同构：短标签 + 取舍说明）</summary>
public sealed record QuestionOption(string Label, string Description);

/// <summary>结构化提问规格（askuser 工具 → UI 渲染 → answer 回执）</summary>
public sealed record QuestionSpec(
    string Question,
    string Header,
    IReadOnlyList<QuestionOption> Options,
    bool MultiSelect);

/// <summary>交互提问提供者：单槽替换式（同 ConfirmProvider 模式）。
/// UI 层（serve/WS 桥）设置后退出时置回 null。无处理器时返回 null（工具向 LLM 报告无交互通道）。</summary>
public static class QuestionProvider
{
    /// <summary>提问处理器：返回用户选中的选项 label（多选逗号连接；自由输入则为文本）；null = 无通道/取消</summary>
    public static Func<QuestionSpec, string?> Ask { get; set; } = _ => null;
}
