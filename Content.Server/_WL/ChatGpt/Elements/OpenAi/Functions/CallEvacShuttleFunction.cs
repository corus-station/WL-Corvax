using Content.Server.RoundEnd;

namespace Content.Server._WL.ChatGpt.Elements.OpenAi.Functions
{
    public sealed partial class CallEvacShuttleFunction : ToolFunctionModel
    {
        private readonly RoundEndSystem _roundEndSys;

        public override LocId Name => "gpt-command-evac-shuttle-name";
        public override LocId Description => "gpt-command-evac-shuttle-desc";
        public override IReadOnlyDictionary<string, Parameter<object>> Parameters => new Dictionary<string, Parameter<object>>()
        {
            ["call"] = new Parameter<bool>()
            {
                Description = "gpt-command-evac-shuttle-arg-call-desc"
            },

            ["time"] = new Parameter<int>()
            {
                Description = "gpt-command-evac-shuttle-arg-time-desc",
                Enum = [1200, 600, 300],
                Required = false
            }
        };
        public override JsonSchemeType ReturnType => JsonSchemeType.Object;

        public override LocId FallbackMessage => "gpt-command-evac-shuttle-fallback";

        public override string? Invoke(Arguments? arguments)
        {
            if (arguments == null)
                return null;

            if (!arguments.TryCaste<bool>("call", out var call))
                return null;

            if (call)
            {
                if (!arguments.TryCaste<int>("time", out var time))
                    return null;

                var span = TimeSpan.FromSeconds(time);

                _roundEndSys.RequestRoundEnd(span);

                return Loc.GetString(FallbackMessage, ("time", time), ("call", true));
            }
            else
            {
                _roundEndSys.CancelRoundEndCountdown();

                return Loc.GetString(FallbackMessage, ("call", false));
            }
        }

        public CallEvacShuttleFunction(RoundEndSystem roundEnd)
        {
            _roundEndSys = roundEnd;
        }
    }
}
