namespace Content.Server._WL.ChatGpt
{
    public enum JsonSchemeType : byte
    {
        String,
        Integer,
        Number,
        Boolean,
        Array,
        Object,
        Null
    }

    public static class JsonSchemeTypeExt
    {
        public static string ToFormatString(this JsonSchemeType type)
        {
            return type switch
            {
                JsonSchemeType.Integer => "integer",
                JsonSchemeType.Number => "number",
                JsonSchemeType.Boolean => "boolean",
                JsonSchemeType.Array => "array",
                JsonSchemeType.String => "string",
                JsonSchemeType.Object => "object",
                JsonSchemeType.Null => "null",
                _ => throw new NotImplementedException()
            };
        }

        public static JsonSchemeType ToJsonSchemeType(this Type type)
        {
            return type switch
            {
                _ when type == typeof(int) => JsonSchemeType.Integer,
                _ when type == typeof(bool) => JsonSchemeType.Boolean,
                _ when type == typeof(float) || type == typeof(double) || type == typeof(decimal) => JsonSchemeType.Number,
                _ when type == typeof(string) => JsonSchemeType.String,
                _ when type.IsArray => JsonSchemeType.Array,
                _ => JsonSchemeType.Object
            };
        }

        public static string ToJsonSchemeString(this Type type)
        {
            return type.ToJsonSchemeType().ToFormatString();
        }
    }
}
