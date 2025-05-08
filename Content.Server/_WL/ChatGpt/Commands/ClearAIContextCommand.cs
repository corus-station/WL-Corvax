using Content.Server._WL.ChatGpt.Systems;
using Content.Server.Administration;
using Content.Shared.Administration;
using Robust.Shared.Console;
using Robust.Shared.Prototypes;
using System.Linq;

namespace Content.Server._WL.ChatGpt.Commands
{
    [AdminCommand(AdminFlags.Round | AdminFlags.Adminchat)]
    public sealed partial class ClearAIContextCommand : LocalizedCommands
    {
        [Dependency] private readonly IEntitySystemManager _entSysMan = default!;
        [Dependency] private readonly IPrototypeManager _protoMan = default!;

        public override string Command => "clearaicontext";
        public override string Description => "Очищает контекст выбранного прототипа ИИ.";

        public override CompletionResult GetCompletion(IConsoleShell shell, string[] args)
        {
            if (args.Length == 1)
            {
                return CompletionResult.FromOptions(_protoMan.EnumeratePrototypes<AIChatPrototype>()
                    .Select(x => x.ID));
            }

            return CompletionResult.Empty;
        }

        public override void Execute(IConsoleShell shell, string argStr, string[] args)
        {
            if (args.Length > 1)
            {
                shell.WriteError(LocalizationManager.GetString("shell-wrong-arguments-number"));
                return;
            }

            var _gpt = _entSysMan.GetEntitySystem<ChatGptSystem>();

            var str_proto = (string?)null;

            if (args.Length == 1)
                str_proto = args[0];

            if (str_proto != null)
            {
                if (!_protoMan.TryIndex<AIChatPrototype>(str_proto, out var proto))
                {
                    shell.WriteError($"Не найден указанный прототип {str_proto}!");
                    return;
                }

                _gpt.ClearMemory(proto.ID);
            }
            else
            {
                _gpt.ClearMemory();
            }
        }
    }
}
