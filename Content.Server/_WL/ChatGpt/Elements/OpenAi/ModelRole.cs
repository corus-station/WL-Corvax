using static Content.Server._WL.ChatGpt.Elements.OpenAi.ModelRole;
using static Content.Server._WL.ChatGpt.Elements.OpenAi.ModelRole.Constants;

namespace Content.Server._WL.ChatGpt.Elements.OpenAi
{
    /// <summary>
    /// Статический класс, содержащий api для преобразования строковых roles в тип <see cref="ModelRoleType"/>.
    /// </summary>
    public static class ModelRole
    {
        /// <summary>
        /// Тип роли при ответе/запросе к модели.
        /// </summary>
        public enum ModelRoleType : byte
        {
            /// <summary>
            /// Сообщение от системы.
            /// </summary>
            System,

            /// <summary>
            /// Сообщение от пользователя.
            /// </summary>
            User,

            /// <summary>
            /// Н/Д. Я хз что это, но это что-то новое.
            /// </summary>
            Assistant,

            /// <summary>
            /// Роль утилиты.
            /// </summary>
            Tool,

            /// <summary>
            /// Роль функции.
            /// </summary>
            [Obsolete("Устарело, используйте методику Tools")]
            Function,

            /// <summary>
            /// НЕ ИСПОЛЬЗОВАТЬ ДЛЯ ОТПРАВКИ ЗАПРОСОВ.
            /// Обычно объект имеет это значение, когда <see cref="FromString(string)"/> не смог определить нужный тип.
            /// Проверяйте объект на это значение и логгируйте.
            /// </summary>
            Invalid
        }

        public static ModelRoleType FromString(string role)
        {
            return role switch
            {
                UserRoleString => ModelRoleType.User,
                SystemRoleString => ModelRoleType.System,
                ToolRoleString => ModelRoleType.Tool,
                AssistantRoleString => ModelRoleType.Assistant,
#pragma warning disable CS0618
                FunctionToleString => ModelRoleType.Function,
#pragma warning restore
                _ => ModelRoleType.Invalid
            };
        }

        public static bool IsStringValid(string role)
        {
            var role_ = FromString(role);

            return role_ != ModelRoleType.Invalid;
        }

        public static string FromModelRoleType(ModelRoleType role_type)
        {
            return role_type switch
            {
                ModelRoleType.System => SystemRoleString,
                ModelRoleType.User => UserRoleString,
                ModelRoleType.Assistant => AssistantRoleString,
                ModelRoleType.Tool => ToolRoleString,
#pragma warning disable CS0618
                ModelRoleType.Function => FunctionToleString,
#pragma warning restore
                _ => throw new NotImplementedException($"Невалидный объект перечисления {nameof(ModelRoleType)}")
            };
        }

        /// <summary>
        /// Константы для текущего класса.
        /// </summary>
        public static class Constants
        {
            public const string UserRoleString = "user";
            public const string SystemRoleString = "system";
            public const string ToolRoleString = "tool";
            public const string AssistantRoleString = "assistant";
            public const string FunctionToleString = "function";
        }
    }

    /// <summary>
    /// Расширения для <see cref="ModelRoleType"/>.
    /// </summary>
    public static class ModelRoleTypeExt
    {
        public static string ToQueryString(this ModelRoleType type)
        {
            return FromModelRoleType(type);
        }
    }
}
