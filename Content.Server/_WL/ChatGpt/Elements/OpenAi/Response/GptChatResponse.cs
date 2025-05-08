using Content.Server._WL.ChatGpt.Elements.Response;
using System.Text.Json.Serialization;

namespace Content.Server._WL.ChatGpt.Elements.OpenAi.Response
{
    /// <summary>
    /// Класс, содержащий поля - в JSON ответе текстовой модели.
    /// </summary>
    public sealed class GptChatResponse
    {
        /// <summary>
        /// Уникальный идентификатор каждого ответа модели.
        /// </summary>
        [JsonPropertyName("id")]
        public required string ID { get; set; }

        /// <summary>
        /// Список вариантов ответов текстовой модели OpenAi на единственный запрос пользователя.
        /// </summary>
        [JsonPropertyName("choices")]
        public required GptChoice[] Choices { get; set; }

        /// <summary>
        /// Unix-образное число, показывающее время создания ответа на запрос пользователя.
        /// </summary>
        [JsonPropertyName("created")]
        public required int Created { get; set; }

        /// <summary>
        /// Модель, использованная при генерации ответа на запрос.
        /// </summary>
        [JsonPropertyName("model")]
        public required string Model { get; set; }

        /// <summary>
        /// Уровень обслуживание(?).
        /// Будет NULL, если в запросе не было включено соимённое поле.
        /// </summary>
        [JsonPropertyName("service_tier")]
        public string? ServiceTier { get; set; }

        /// <summary>
        /// Строка, в которой зашифрована конфигурация бэкенд-стороны модели, которая генерировала ответ.
        /// </summary>
        [JsonPropertyName("system_fingerprint")]
        public required string Fingerprint { get; set; }

        /// <summary>
        /// Объект, использованный для генерации...
        /// Короче, в данном случае оно всегда 'chat.completion'.
        /// </summary>
        [JsonPropertyName("object")]
        public required string Object { get; set; }

        /// <summary>
        /// Информация о расходах токенов, их количестве и т.п.
        /// </summary>
        [JsonPropertyName("usage")]
        public required GptTokenUsage Usage { get; set; }

        /// <summary>
        /// Возвращает чисто ответ модели.
        /// А зачем нужна другая информация, действительно.
        /// </summary>
        /// <returns>NULL, если модель ничего не сгенерировала.</returns>
        public string? GetRawStringResponse()
        {
            if (Choices.Length == 0)
                return null;

            var chosen = Choices[0].Message;

            return chosen.Content ?? chosen.RefusalMessage;
        }
    }
}
