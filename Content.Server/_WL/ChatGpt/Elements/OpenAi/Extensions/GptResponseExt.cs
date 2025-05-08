using Content.Server._WL.ChatGpt.Elements.OpenAi.Response;
using Robust.Shared.Random;

namespace Content.Server._WL.ChatGpt.Elements.OpenAi.Extensions
{
    public static class GptResponseExt
    {
        public static string? GetRawStringResponse(this GptChatResponse response, IRobustRandom random)
        {
            var choices = response.Choices;

            if (choices.Length == 0)
                return null;

            var chosen = random.Pick(choices).Message;

            return chosen.Content ?? chosen.RefusalMessage;
        }
    }
}
