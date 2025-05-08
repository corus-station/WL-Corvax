using Content.Server._WL.Ert;
using Content.Shared._WL.Ert;
using System.Linq;

namespace Content.Server._WL.ChatGpt.Elements.OpenAi.Functions
{
    public sealed class ERTSpawnShuttleFunction : ToolFunctionModel
    {
        private readonly ErtSystem _ert;

        public override LocId Name => "gpt-command-spawn-ert-shuttle-name";
        public override LocId Description => "gpt-command-spawn-ert-shuttle-desc";
        public override JsonSchemeType ReturnType => JsonSchemeType.Object;
        public override IReadOnlyDictionary<string, Parameter<object>> Parameters => new Dictionary<string, Parameter<object>>()
        {
            ["type"] = new Parameter<string>()
            {
                Required = true,
                Enum = Enum.GetNames(typeof(ErtType))?
                    .Select(x => (object?)x)
                    .ToHashSet(),
                Description = "gpt-command-spawn-ert-shuttle-arg-level-desc"
            }
        };
        public override LocId FallbackMessage => "gpt-command-spawn-ert-shuttle-fallback";

        public override string? Invoke(Arguments? arguments)
        {
            if (arguments == null)
                return null;

            if (!arguments.TryCaste<string>("type", out var parsed))
                return null;

            if (!Enum.TryParse<ErtType>(parsed, out var result))
                return null;

            var chosen = Loc.GetString(FallbackMessage, ("type", "other"));

            if (!_ert.TrySpawn(result, out _))
                return chosen;

            return Loc.GetString(FallbackMessage, ("type", parsed));
        }

        public ERTSpawnShuttleFunction(ErtSystem ertSys)
        {
            _ert = ertSys;
        }
    }
}
