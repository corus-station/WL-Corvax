using Content.Shared.Fax.Components;

namespace Content.Shared._WL.Fax.Events
{
    public sealed partial class FaxRecieveMessageEvent : EntityEventArgs
    {
        public readonly FaxPrintout Message;
        public readonly EntityUid? Sender;
        public readonly Entity<FaxMachineComponent> Reciever;

        public FaxRecieveMessageEvent(FaxPrintout msg, EntityUid? sender, Entity<FaxMachineComponent> reciever)
        {
            Message = msg;
            Sender = sender;
            Reciever = reciever;
        }
    }
}
