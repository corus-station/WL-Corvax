using Content.Server.GameTicking;
using Robust.Shared.Prototypes;
using System.Linq;

namespace Content.Server._WL.ChatGpt.Elements.OpenAi.Functions
{
    public sealed partial class SpawnSuppliesFunction : ToolFunctionModel
    {
        private readonly GameTicker _gameTicker;
        private readonly IPrototypeManager _protoMan;
        private readonly HashSet<object?> _protos;

        [ValidatePrototypeId<EntityPrototype>]
        private readonly string _basePrototype = "CargoGiftsBase";

        public override LocId Name => "gpt-command-spawn-supplies-name";
        public override LocId Description => "gpt-command-spawn-supplies-desc";

        public override IReadOnlyDictionary<string, Parameter<object>> Parameters => new Dictionary<string, Parameter<object>>()
        {
            ["types"] = new Parameter<string>
            {
                Description = "gpt-command-spawn-supplies-arg-type-desc",
                Enum = _protos
            }
        };

        public override JsonSchemeType ReturnType => JsonSchemeType.Object;
        public override LocId FallbackMessage => "gpt-command-spawn-supplies-fallback";
        public override string? Invoke(Arguments? arguments)
        {
            if (arguments == null)
                return null;

            if (!arguments.TryCaste<string>("types", out var parsed))
                return null;

            if (!_protoMan.TryIndex<EntityPrototype>(parsed, out var proto))
                return null;

            var ent = _gameTicker.AddGameRule(proto.ID);
            _gameTicker.StartGameRule(ent);

            return Loc.GetString(FallbackMessage, ("types", proto.ID));
        }

        public SpawnSuppliesFunction(GameTicker ticker, IPrototypeManager protoMan)
        {
            _gameTicker = ticker;
            _protoMan = protoMan;

            _protos = _protoMan.EnumeratePrototypes<EntityPrototype>()
                .Where(p => p.Parents?.ToHashSet().Contains(_basePrototype) == true)
                .Select(x => (object?)x.ID)
                .ToHashSet();
        }
    }
}
