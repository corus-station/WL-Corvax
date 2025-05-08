using System.Text.Json.Serialization;

namespace Content.Server._WL.ChatGpt.Elements.OpenAi.Request
{
    /// <summary>
    /// Класс, описывающий инструмент, который может использовать модель при генерации ответа.
    /// </summary>
    public sealed class GptChatTool
    {
        /// <summary>
        /// Тип "инструмента".
        /// Смотреть <see cref="ModelTool.ModelToolType"/>.
        /// </summary>
        [JsonPropertyName("type")]
        public required string Type { get; set; }

        /// <summary>
        /// Инструмент.
        /// </summary>
        [JsonPropertyName("function")]
        public required object UsingTool { get; set; }

        /// <summary>
        /// Внутренний класс, используемый для конкретизации инструмента, который нужно использовать.
        /// </summary>
        public abstract class Tool
        {
            /// <summary>
            /// Простая функция.
            /// </summary>
            public sealed class Function : Tool
            {
                /// <summary>
                /// Название вызываемой функции.
                /// </summary>
                [JsonPropertyName("name")]
                public required string Name { get; set; }

                /// <summary>
                /// Описание функции.
                /// </summary>
                [JsonPropertyName("description")]
                public string? Description { get; set; }

                /// <summary>
                /// Параметры функции.
                /// </summary>
                [JsonPropertyName("parameters")]
                public FunctionArgumentsScheme? Parameters { get; set; }

                /// <summary>
                /// Следует ли модели генерировать аргументы для функций ЧЁТКО по схеме,
                /// А то иначе она может немного косячить со схемой.
                /// </summary>
                [JsonPropertyName("strict")]
                public bool? Strict { get; set; } = true;

                /// <summary>
                /// Схема, описывающая параметры функции.
                /// </summary>
                public sealed class FunctionArgumentsScheme
                {
                    /// <summary>
                    /// Возвращаемый тип функции.
                    /// </summary>
                    [JsonPropertyName("type")]
                    public required string ReturnType { get; set; }

                    /// <summary>
                    /// Обязательные параметры функции.
                    /// </summary>
                    [JsonPropertyName("required")]
                    public string[]? Required { get; set; }

                    /// <summary>
                    /// Сами параметры.
                    /// </summary>
                    [JsonPropertyName("properties")]
                    public Dictionary<string, Property>? Properties { get; set; }

                    [JsonPropertyName("additionalProperties")]
                    public bool AdditionalProperties { get; set; } = false;

                    /// <summary>
                    /// Класс, описывающий параметр функции.
                    /// </summary>
                    public sealed class Property
                    {
                        /// <summary>
                        /// Тип параметра.
                        /// </summary>
                        [JsonPropertyName("type")]
                        public required object Type { get; set; }

                        /// <summary>
                        /// Описание параметра функции.
                        /// </summary>
                        [JsonPropertyName("description")]
                        public string? Description { get; set; }

                        /// <summary>
                        /// Константные значения.
                        /// То есть список всех возможных значений аргумента.
                        /// </summary>
                        [JsonPropertyName("enum")]
                        public object?[]? Enum { get; set; } = null;
                    }
                }
            }
        }
    }
}
