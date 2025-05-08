using Content.Server.Chat.Systems;
using System.Linq;

namespace Content.Server._WL.ChatGpt.Elements.OpenAi.Functions
{
    public sealed partial class MadeNotifyFunction : ToolFunctionModel
    {
        private readonly ChatSystem _chat;
        private readonly EntityUid _station;

        public override LocId Name => "gpt-command-made-notify-name";
        public override LocId Description => "gpt-command-made-notify-desc";
        public override IReadOnlyDictionary<string, Parameter<object>> Parameters => new Dictionary<string, Parameter<object>>()
        {
            ["text"] = new Parameter<string>()
            {
                Required = true,
                Description = "gpt-command-made-notify-arg-text-desc"
            }
        };
        public override JsonSchemeType ReturnType => JsonSchemeType.Object;
        public override LocId FallbackMessage => "gpt-command-made-notify-fallback";

        public override string? Invoke(Arguments? arguments)
        {
            if (arguments == null)
                return null;

            if (!arguments.TryCaste<string>("text", out var text))
                return null;

            _chat.DispatchStationAnnouncement(_station, text, Loc.GetString("admin-announce-announcer-default"), colorOverride: Color.Yellow);

            return Loc.GetString(FallbackMessage, ("text", text.Split().FirstOrDefault() ?? ""));
        }

        public MadeNotifyFunction(ChatSystem chat, EntityUid station)
        {
            _chat = chat;
            _station = station;
        }
    }
}
