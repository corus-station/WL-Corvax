using Robust.Shared.Serialization;

namespace Content.Shared._WL.DiscordAuth
{
    [Serializable, NetSerializable]
    public sealed partial class DiscordAuthExpirationTimeSyncEvent : EntityEventArgs
    {
        public readonly TimeSpan ExpirationAccum;
        public readonly TimeSpan ExpirationTime;

        public DiscordAuthExpirationTimeSyncEvent(TimeSpan expirationAccum, TimeSpan expirationTime)
        {
            ExpirationAccum = expirationAccum;
            ExpirationTime = expirationTime;
        }
    }
}
