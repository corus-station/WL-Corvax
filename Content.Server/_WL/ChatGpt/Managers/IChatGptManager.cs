using Content.Server._WL.ChatGpt.Elements.OpenAi.Request;
using Content.Server._WL.ChatGpt.Elements.OpenAi.Response;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using System.Threading;
using Content.Server._WL.ChatGpt.Elements.OpenAi;

namespace Content.Server._WL.ChatGpt.Managers
{
    public interface IChatGptManager
    {
        void Initialize();
        void PostInject();

        bool IsEnabled();
        bool IsEnabled([NotNullWhen(false)] out string? reason);
        Task<GptChatResponse> SendChatQueryAsync(
            GptChatRequest gpt_request,
            IEnumerable<ToolFunctionModel>? methods = null,
            CancellationToken cancel = default);

        Task<string?> SendChatQuery(string prompt);

        Task<decimal?> GetBalanceAsync(CancellationToken cancel = default);
    }
}
