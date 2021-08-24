using Exiled.API.Features;
using System;

namespace BanLogger
{
	public class Plugin : Plugin<Config>
	{
		public override string Name { get; } = "Ban Logger";
		public override string Author { get; } = "Jesus-QC";
		public override string Prefix { get; } = "BanLogger";
		public override Version Version { get; } = new Version(1, 0, 5);
		public override Version RequiredExiledVersion { get; } = new Version(2, 3, 3);

		public static Plugin Instance { get; private set; }
		private EventHandlers _eventHandlers;

		public override void OnEnabled()
		{
			Instance = this;
			_eventHandlers = new EventHandlers();
			
			Exiled.Events.Handlers.Player.ChangingMuteStatus += _eventHandlers.OnMuted;
			Exiled.Events.Handlers.Player.Banning += _eventHandlers.OnBanning;
			Exiled.Events.Handlers.Player.Kicking += _eventHandlers.OnKicking;
			Exiled.Events.Handlers.Player.Banned += _eventHandlers.OnPlayerOBan;

			base.OnEnabled();
		}

		public override void OnDisabled()
		{
			Exiled.Events.Handlers.Player.ChangingMuteStatus -= _eventHandlers.OnMuted;
			Exiled.Events.Handlers.Player.Banning -= _eventHandlers.OnBanning;
			Exiled.Events.Handlers.Player.Kicking -= _eventHandlers.OnKicking;
			Exiled.Events.Handlers.Player.Banned -= _eventHandlers.OnPlayerOBan;

			_eventHandlers = null;
			Instance = null;

			base.OnDisabled();
		}
	}
}