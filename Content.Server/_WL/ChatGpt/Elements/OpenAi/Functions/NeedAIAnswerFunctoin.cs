
namespace Content.Server._WL.ChatGpt.Elements.OpenAi.Functions
{
    public sealed class NoNeedAIAnswerFunction : ToolFunctionModel
    {
        public override LocId Name => "gpt-command-need-ai-answer-name";

        public override LocId Description => "gpt-command-need-ai-answere-desc";

        public override IReadOnlyDictionary<string, Parameter<object>> Parameters => new Dictionary<string, Parameter<object>>()
        {
        };

        public override JsonSchemeType ReturnType => JsonSchemeType.Object;
    }
}
