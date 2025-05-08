using static Content.Server._WL.ChatGpt.Elements.OpenAi.ModelTool.Constants;

namespace Content.Server._WL.ChatGpt.Elements.OpenAi
{
    /// <summary>
    /// Статический класс для преобразований <see cref="ModelToolType"/>.
    /// </summary>
    public static class ModelTool
    {
        /// <summary>
        /// Тип "утилиты", используемой для предоставления модели возможности строго делать какие-либо действия в зависимости от сгенерированного контекста.
        /// </summary>
        public enum ModelToolType : byte
        {
            /// <summary>
            /// Функция.
            /// </summary>
            Function,

            /// <summary>
            /// НЕ ИСПОЛЬЗОВАТЬ ДЛЯ ОТПРАВКИ ЗАПРОСОВ.
            /// Обычно объект имеет это значение, когда <see cref="FromString(string)"/> не смог определить нужный тип.
            /// Проверяйте объект на это значение и логгируйте.
            /// </summary>
            Invalid
        }

        public static ModelToolType FromString(string tool)
        {
            return tool switch
            {
                FunctionToolString => ModelToolType.Function,
                _ => ModelToolType.Invalid
            };
        }

        public static string FromModelToolType(ModelToolType tool_type)
        {
            return tool_type switch
            {
                ModelToolType.Function => FunctionToolString,
                _ => throw new NotImplementedException($"Невалидный объект перечисления {nameof(ModelToolType)}")
            };
        }

        /// <summary>
        /// Константы для текущего класса.
        /// </summary>
        public static class Constants
        {
            public const string FunctionToolString = "function";
        }
    }
}
