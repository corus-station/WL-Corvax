namespace Content.Shared._WL.Light.Events
{
    public sealed partial class BeforeDealHeatDamageFromLightBulbEvent : CancellableEntityEventArgs
    {
        public readonly EntityUid Bulb;

        public BeforeDealHeatDamageFromLightBulbEvent(EntityUid bulb)
        {
            Bulb = bulb;
        }
    }
}
