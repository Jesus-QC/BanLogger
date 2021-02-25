using Exiled.API.Features;
using System;

namespace BanLogger
{
	public class Plugin : Plugin<Config>
	{
		public override string Name { get; } = "Ban Logger";
		public override string Author { get; } = "Jesus-QC";
		public override string Prefix { get; } = "BanLogger";
		public override Version Version { get; } = new Version(1, 0, 3);
		public override Version RequiredExiledVersion { get; } = new Version(2, 3, 3);

		public EventHandlers EventHandlers;

		public override void OnEnabled()
		{
			EventHandlers = new EventHandlers(this);
			
			Exiled.Events.Handlers.Player.Banning += EventHandlers.OnBanning;
			Exiled.Events.Handlers.Player.Kicking += EventHandlers.OnKicking;
			Exiled.Events.Handlers.Player.Banned += EventHandlers.OnPlayerOban;

			base.OnEnabled();
		}

		public override void OnDisabled()
		{
			Exiled.Events.Handlers.Player.Banning -= EventHandlers.OnBanning;
			Exiled.Events.Handlers.Player.Kicking -= EventHandlers.OnKicking;
			Exiled.Events.Handlers.Player.Banned -= EventHandlers.OnPlayerOban;

			EventHandlers = null;

			base.OnDisabled();
		}
	}
}