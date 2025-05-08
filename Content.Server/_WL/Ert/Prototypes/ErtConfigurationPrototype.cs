using Content.Shared._WL.Ert;
using JetBrains.Annotations;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;
using Robust.Shared.Utility;
using System.Numerics;

namespace Content.Server._WL.Ert.Prototypes
{
    [Prototype("ertConfig")]
    public sealed partial class ErtConfigurationPrototype : IPrototype
    {
        [IdDataField]
        public string ID { get; } = default!;

        [DataField(required: true)]
        public Dictionary<ErtType, ErtConfigEntry> Entry { get; private set; } = new();

        [DataDefinition]
        [UsedImplicitly]
        public sealed partial class ErtConfigEntry
        {
            [DataField(required: true)]
            public ResPath ShuttlePath { get; private set; }

            [DataField]
            public float ShuttleSpawnOffset { get; private set; } = 300;

            [DataField]
            public Vector2i MinMax { get; private set; } = new Vector2i(2, 3);

            [DataField(required: true)]
            public EntProtoId SpawnPoint { get; private set; } = default!;
        }

        public float ShuttleOffset(ErtType ert)
        {
            return Entry[ert].ShuttleSpawnOffset;
        }

        public EntProtoId SpawnPoint(ErtType ert)
        {
            return Entry[ert].SpawnPoint;
        }

        public Vector2i MinMax(ErtType ert)
        {
            var vector = Entry[ert].MinMax;

            return vector;
        }

        public ResPath ShuttlePath(ErtType ert)
        {
            return Entry[ert].ShuttlePath;
        }
    }
}
