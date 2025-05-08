using System.Text.Json.Serialization;

namespace Content.Server._WL.ChatGpt.Elements.OpenAi
{
    /// <summary>
    /// Класс, представляющий баланс аккаунта, на котором куплен апи токен.
    /// Специфичен...
    /// </summary>
    public sealed class AccountBalance
    {
        [JsonPropertyName("balance")]
        public required decimal Balance { get; set; }
    }
}
