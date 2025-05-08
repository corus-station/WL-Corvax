using Content.Server.Administration;
using Content.Shared._WL.CCVars;
using Content.Shared.Administration;
using Robust.Shared.Configuration;
using Robust.Shared.Console;

namespace Content.Server._WL.ChatGpt.Commands
{
    [AdminCommand(AdminFlags.Round | AdminFlags.Adminchat)]
    public sealed partial class EnableCCAICommand : LocalizedCommands
    {
        [Dependency] private readonly IConfigurationManager _configMan = default!;

        public override string Command => "togglecentralcommandai";
        public override string Description => "Отключает или выключает отправку запросов на ЦК к текстовой нейросети.";

        public override void Execute(IConsoleShell shell, string argStr, string[] args)
        {
            var now = _configMan.GetCVar(WLCVars.IsGptEnabled);

            _configMan.SetCVar(WLCVars.IsGptEnabled, !now);

            var text = now
                ? "выключен"
                : "включен";
            shell.WriteLine($"Автоответчик ЦК сейчас: {text}");
        }
    }
}
