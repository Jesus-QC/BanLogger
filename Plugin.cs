using Exiled.API.Features;
using System;

namespace BanLogger
{
	public class Plugin : Plugin<Config>
	{
		public override string Name { get; } = "Ban Logger";
		public override string Author { get; } = "Jesus-QC";
		public override string Prefix { get; } = "BanLogger";
		public override Version Version { get; } = new Version(1, 0, 1);
		public override Version RequiredExiledVersion { get; } = new Version(2, 1, 35);

		public EventHandlers EventHandlers;

		public override void OnEnabled()
		{
			EventHandlers = new EventHandlers(this);
			
			Exiled.Events.Handlers.Player.Banning += EventHandlers.OnBanning;
			Exiled.Events.Handlers.Player.Kicking += EventHandlers.OnKicking;

			base.OnEnabled();
		}

		public override void OnDisabled()
		{
			Exiled.Events.Handlers.Player.Banning -= EventHandlers.OnBanning;
			Exiled.Events.Handlers.Player.Kicking -= EventHandlers.OnKicking;
			
			EventHandlers = null;

			base.OnDisabled();
		}
	}
}