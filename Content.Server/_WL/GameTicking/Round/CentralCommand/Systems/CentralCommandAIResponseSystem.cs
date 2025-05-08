using Content.Server._WL.ChatGpt;
using Content.Server._WL.ChatGpt.Elements.OpenAi;
using Content.Server._WL.ChatGpt.Elements.OpenAi.Functions;
using Content.Server._WL.ChatGpt.Elements.OpenAi.Request;
using Content.Server._WL.ChatGpt.Elements.OpenAi.Response;
using Content.Server._WL.ChatGpt.Managers;
using Content.Server._WL.ChatGpt.Systems;
using Content.Server._WL.Ert;
using Content.Server.AlertLevel;
using Content.Server.Chat.Managers;
using Content.Server.Chat.Systems;
using Content.Server.Clock;
using Content.Server.Fax;
using Content.Server.GameTicking;
using Content.Server.RoundEnd;
using Content.Server.Station.Systems;
using Content.Shared._WL.CCVars;
using Content.Shared._WL.Fax.Events;
using Content.Shared.Fax.Components;
using Content.Shared.GameTicking;
using Content.Shared.Paper;
using Robust.Shared.Configuration;
using Robust.Shared.Prototypes;
using Robust.Shared.Random;
using Robust.Shared.Timing;
using Robust.Shared.Utility;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Content.Server._WL.GameTicking.Round.CentralCommand.Systems
{
    public sealed partial class CentralCommandAIResponseSystem : EntitySystem
    {
        [Dependency] private readonly IGameTiming _timing = default!;
        [Dependency] private readonly IConfigurationManager _configMan = default!;
        [Dependency] private readonly ChatGptSystem _gptSys = default!;
        [Dependency] private readonly IChatGptManager _gptMan = default!;
        [Dependency] private readonly IChatManager _chatMan = default!;
        [Dependency] private readonly IPrototypeManager _protoMan = default!;
        [Dependency] private readonly AlertLevelSystem _alertLevel = default!;
        [Dependency] private readonly StationSystem _station = default!;
        [Dependency] private readonly IRobustRandom _random = default!;
        [Dependency] private readonly GameTicker _ticker = default!;
        [Dependency] private readonly FaxSystem _fax = default!;
        [Dependency] private readonly ILogManager _log = default!;
        [Dependency] private readonly ChatSystem _chat = default!;
        [Dependency] private readonly RoundEndSystem _roundEnd = default!;
        [Dependency] private readonly ErtSystem _ert = default!;
        [Dependency] private readonly ClockSystem _clock = default!;

        private ISawmill _sawmill = default!;

        private const int QUERIES_UPDATE_TIME = 1; //в минутах

        [ValidatePrototypeId<EntityPrototype>]
        private static readonly string CCFaxMachineId = "FaxMachineCentcom";

        [ValidatePrototypeId<AIChatPrototype>]
        private static readonly string CentcomAIPrototypeId = "CentralCommand";

        [GeneratedRegex(@"(?:^[ =═]{40,}\n)([\s\S]*?)(?=\n[ =═]{40,}$)", RegexOptions.Multiline)]
        private static partial Regex SearchRegex();

        private int _queryCounter = 0;
        private TimeSpan _accumUpdate = TimeSpan.Zero;
        private int _queriesPerMinute = -1;

        private TimeSpan _maxRespTime = default!;
        private TimeSpan _minRespTime = default!;
        private TimeSpan _respTime = default;

        private readonly Queue<QueueEntry> _messagesQuery = new();

        public override void Initialize()
        {
            base.Initialize();

            _sawmill = _log.GetSawmill("cc.ai");

            SubscribeLocalEvent<FaxMachineComponent, FaxRecieveMessageEvent>(OnFaxRecieve);
            SubscribeLocalEvent<RoundRestartCleanupEvent>((_) => Clear());

            _configMan.OnValueChanged(WLCVars.CCMaxQueriesPerMinute, (value) => _queriesPerMinute = value, true);
            _queriesPerMinute = _configMan.GetCVar(WLCVars.CCMaxQueriesPerMinute);

            _configMan.OnValueChanged(WLCVars.CCMinResponseTime, (value) => _minRespTime = TimeSpan.FromSeconds(value), true);
            _minRespTime = TimeSpan.FromSeconds(_configMan.GetCVar(WLCVars.CCMinResponseTime));

            _configMan.OnValueChanged(WLCVars.CCMaxResponseTime, (value) => _maxRespTime = TimeSpan.FromSeconds(value), true);
            _maxRespTime = TimeSpan.FromSeconds(_configMan.GetCVar(WLCVars.CCMaxResponseTime));

            _accumUpdate = _timing.CurTime;

            _respTime = _random.Next(_minRespTime, _maxRespTime);
        }

        public override async void Update(float frameTime)
        {
            base.Update(frameTime);

            if (!_timing.IsFirstTimePredicted)
                return;

            if (_timing.CurTime.Subtract(_accumUpdate) >= TimeSpan.FromMinutes(QUERIES_UPDATE_TIME))
            {
                _accumUpdate = _timing.CurTime;
                _queryCounter = 0;
            }

            if (_messagesQuery.Count == 0)
                return;

            _respTime -= _timing.TickPeriod;

            if (_respTime <= TimeSpan.Zero)
            {
                _respTime = (_random.Next(_minRespTime, _maxRespTime) + _random.Next(_minRespTime, _maxRespTime)) / 2;

                try
                {
                    await PopQuery();
                }
                catch (Exception ex)
                {
                    _sawmill.Error(ex.ToStringBetter());
                }
            }
        }

        private void OnFaxRecieve(EntityUid fax, FaxMachineComponent comp, FaxRecieveMessageEvent args)
        {
            if (args.Sender == null)
                return;

            if (!_gptMan.IsEnabled())
                return;

            if (_queryCounter >= _queriesPerMinute)
                return;

            if (Prototype(fax)?.ID != CCFaxMachineId)
                return;

            var station = _station.GetOwningStation(args.Sender);
            if (station == null)
                return;

            var printout = args.Message;

            _queryCounter += 1;

            var gpt_messages = new List<GptChatMessage>();

            //Уровень угрозы на станции
            var alert_level = _alertLevel.GetLevel(station.Value);

            var alert_level_loc = _alertLevel.GetLevelLocString(alert_level);

            //TODO: заменить на строки локализации
            var alert_message = new GptChatMessage.System($"У них на станции сейчас {alert_level_loc} код!");
            gpt_messages.Add(alert_message);

            //Вызван ли шаттл
            var is_round_end = _roundEnd.IsRoundEndRequested();

            var str = is_round_end ? "вызван" : "не вызван";

            var is_round_end_msg = new GptChatMessage.System($"Эвакуационный шаттл {str}!");
            gpt_messages.Add(is_round_end_msg);

            //Контент сообщения
            var content_builder = SearchContent(printout.Content);
            var stamps = GetStampsString(printout); // Печати

            content_builder.AppendLine(stamps);

            var content = content_builder.ToString();

            if (string.IsNullOrWhiteSpace(content) || string.IsNullOrEmpty(content))
                return;

            var content_message = new GptChatMessage.User(content);
            gpt_messages.Add(content_message);

            //Сам запрос
            var gpt_request = new GptChatRequest()
            {
                Messages = gpt_messages.ToArray()
            };

            var entry = new QueueEntry()
            {
                Station = station.Value,
                Request = gpt_request,
                Fax = (fax, comp),
                Sender = args.Sender.Value
            };

            _messagesQuery.Enqueue(entry);

            _chatMan.SendAdminAnnouncement($"Сообщение, полученное на {ToPrettyString(fax)}, будет обработано ИИ через {_respTime.TotalMinutes} минут!");
        }

        private StringBuilder SearchContent(string input)
        {
            var regex = SearchRegex();

            input = FormattedMessage.RemoveMarkupOrThrow(input);

            var matches = regex.Matches(input).ToList();
            if (matches.Count == 0)
                return new(input);

            var builder = new StringBuilder(250);

            foreach (var match in matches)
            {
                builder.AppendLine(match.Value);
            }

            return builder;
        }

        private string GetStampsString(FaxPrintout printout)
        {
            var builder = new StringBuilder();

            if (printout.StampedBy.Count > 0)
            {
                foreach (var stamp in printout.StampedBy)
                {
                    builder.AppendLine($"*ЗДЕСЬ ЕСТЬ СЛЕДУЮЩАЯ ПЕЧАТЬ: {Loc.GetString(stamp.StampedName)}*");
                }
            }
            else builder.Append("*ЗДЕСЬ НЕТ ПЕЧАТЕЙ!*");

            return builder.ToString();
        }

        private async Task PopQuery()
        {
            if (!_messagesQuery.TryDequeue(out var queue))
                return;

            var proto = _protoMan.Index<AIChatPrototype>(CentcomAIPrototypeId);

            try
            {
                var methods = GetMethodInfos(queue.Station);

                var resp = await _gptSys.SendWithMemory(proto, queue.Request, methods);
                if (resp.Choices.Length == 0)
                    return;

                var choice = _random.Pick(resp.Choices);

                var content = choice.Message.Content;

                var tools = choice.Message.Tools;
                if (tools != null)
                {
                    var tool_resp = await HandleToolResponse([.. tools], methods, proto);

                    content = tool_resp?.Message.Content;
                }

                if (content == null)
                {
                    _sawmill.Warning("Обработанная функция вернула <NULL>!");
                    return;
                }

                var fax_content = NTLogo(DateTime.UtcNow, content, Name(queue.Station));
                var fax = new FaxPrintout(
                    fax_content,
                    "Ответ от ЦК",
                    stampState: "paper_stamp-centcom",
                    locked: true,
                    stampedBy: [new StampDisplayInfo()
                {
                    StampedColor = Color.Green,
                    StampedName = "Центком"
                }]);

                _fax.Receive(queue.Sender, fax, "Центком", null, queue.Fax.Owner);
            }
            catch (Exception e)
            {
                _sawmill.Error(e.ToStringBetter());
            }
            finally
            {
                //_gptSys.ClearMemory(proto);
            }
        }

        private async Task<GptChoice?> HandleToolResponse(
            IEnumerable<GptChoice.ChoiceMessage.ResponseToolCall> called,
            IEnumerable<ToolFunctionModel> tools,
            AIChatPrototype proto)
        {
            var chosen_tools = ToolFunctionModel.GiveChosenModels(called, tools);

            var skip_sending = false;

            var messages = new List<GptChatMessage>();

            foreach (var (response, function) in chosen_tools)
            {
                if (function is NoNeedAIAnswerFunction)
                    skip_sending = true;

                var arguments = response.Function.ParseArguments();

                var content = function.Invoke(ToolFunctionModel.Arguments.FromNode(arguments));

                var msg = new GptChatMessage.Tool(content)
                {
                    ToolId = response.ID
                };

                messages.Add(msg);
            }

            if (skip_sending)
            {
                _gptSys.AddToMemory(proto.ID, messages);
                return null;
            }

            var req = new GptChatRequest()
            {
                Messages = messages.ToArray()
            };

            var resp = await _gptSys.SendWithMemory(proto, req, null);

            return _random.Pick(resp.Choices);
        }

        private List<ToolFunctionModel> GetMethodInfos(EntityUid station)
        {
            var list = new List<ToolFunctionModel>()
            {
                new SetAlertLevelFunction(_alertLevel, station, _protoMan), // Смена кодов
                new MadeNotifyFunction(_chat, station),
                new CallEvacShuttleFunction(_roundEnd),
                new ERTSpawnShuttleFunction(_ert),
                new NoNeedAIAnswerFunction(),
                new SpawnSuppliesFunction(_ticker, _protoMan)
            };

            return list;
        }

        private void Clear()
        {
            _messagesQuery.Clear();
        }

        [Obsolete("Заменить на строку локализации")]
        private static string NTLogo(DateTime date, string content, string station)
        {
            return NTLogoFirst() + NTLogoEnd();

            string NTLogoFirst()
            {
                return
                $"""
                [color=#1b487e]███░███░░░░██░░░░[/color]
                [color=#1b487e]░██░████░░░██░░░░[/color]      [head=3]Бланк документа[/head]
                [color=#1b487e]░░█░██░██░░██░█░░[/color]               [head=3]NanoTrasen[/head]
                [color=#1b487e]░░░░██░░██░██░██░[/color]    [bold]Станция {station} ЦК-КОМ[/bold]
                [color=#1b487e]░░░░██░░░████░███[/color]
                ═════════════════════════════════════════
                ОТВЕТ НА ФАКС
                ═════════════════════════════════════════
                Дата: {date}
                Ответ: {content}

                """;
            }

            string NTLogoEnd()
            {
                return
                """

                ═════════════════════════════════════════
                [italic]Место для печатей[/italic]
                """;
            }
        }

        private sealed class QueueEntry
        {
            public required EntityUid Station;
            public required GptChatRequest Request;
            public required Entity<FaxMachineComponent> Fax;
            public required EntityUid Sender;
        }
    }
}
