using JetBrains.Annotations;
using Robust.Client.UserInterface.Controllers;

namespace Content.Client._WL.DiscordAuth
{
    [UsedImplicitly]
    public sealed class PlayerSessionInfoUIController : UIController, IOnSystemLoaded<ClientDiscordAuthSystem>
    {
        private PlayerSessionInfoWindow? _sessionInfoWindow;

        public override void Initialize()
        {
            base.Initialize();
        }

        public void ToggleWindow()
        {
            if (_sessionInfoWindow == null)
                return;

            if (_sessionInfoWindow.IsOpen)
                _sessionInfoWindow.Close();
            else _sessionInfoWindow.OpenCentered();
        }

        public void OpenWindow()
        {
            if (_sessionInfoWindow == null)
                return;

            _sessionInfoWindow.OpenCentered();
        }

        public void CloseWindow()
        {
            if (_sessionInfoWindow == null || !_sessionInfoWindow.IsOpen)
                return;

            _sessionInfoWindow.Close();
        }

        public void OnSystemLoaded(ClientDiscordAuthSystem system)
        {
            _sessionInfoWindow = new PlayerSessionInfoWindow(system);
        }
    }
}
