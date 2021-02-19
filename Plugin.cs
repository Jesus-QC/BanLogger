using Exiled.API.Features;
using System;

namespace BanLogger
{
	public class Plugin : Plugin<Config>
	{
		public override string Name { get; } = "Ban Logger";
		public override string Author { get; } = "JesusQC with <3";
		public override string Prefix { get; } = "Ban Logger";
		public override Version Version { get; } = new Version(1, 0);
		public override Version RequiredExiledVersion { get; } = new Version(2, 1, 35);

		public static EventHandlers instance;

		public EventHandlers EventHandlers;

		public override void OnEnabled()
		{
			EventHandlers = new EventHandlers(this);
			base.OnEnabled();

			Exiled.Events.Handlers.Player.Banning += EventHandlers.OnBanning;
			Exiled.Events.Handlers.Player.Kicking += EventHandlers.OnKicking;
		}

		public override void OnDisabled()
		{
			base.OnDisabled();

			Exiled.Events.Handlers.Player.Banning -= EventHandlers.OnBanning;
			Exiled.Events.Handlers.Player.Kicking -= EventHandlers.OnKicking;

			EventHandlers = null;
		}
	}
}
