using Robust.Shared.Prototypes;

namespace Content.Server._WL.ChatGpt
{
    [Prototype("aiChat")]
    public sealed partial class AIChatPrototype : IPrototype
    {

        [IdDataField]
        public string ID { get; private set; } = default!;

        [DataField]
        public bool UseMemory { get; private set; } = false;

        [DataField(required: true)]
        public LocId BasePrompt { get; private set; } = string.Empty;
    }
}
