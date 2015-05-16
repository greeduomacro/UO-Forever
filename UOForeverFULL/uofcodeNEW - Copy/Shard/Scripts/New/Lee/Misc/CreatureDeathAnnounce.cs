#region References
using System;
using System.Collections.Generic;
using System.Linq;

using Server.Network;

using VitaNex.Notify;
#endregion

namespace Server.Mobiles
{
	/// <summary>
	///     Announces the death of creatures listed in the BroadcastTypes collection.
	/// </summary>
	public static class CreatureDeathAnnounce
	{
		public static bool Enabled = false;

		public static TimeSpan AnnounceDelay = TimeSpan.FromMinutes(10.0);

		public static List<Type> BroadcastTypes = new List<Type>
		{
			typeof(BaseChampion)
		};

		private static bool _Configured;

		public static void Configure()
		{
			if (_Configured)
			{
				return;
			}

			_Configured = true;

			EventSink.CreatureDeath += OnDeath;
		}

		public static void OnDeath(CreatureDeathEventArgs e)
		{
			if (!Enabled)
			{
				return;
			}

			var c = e.Creature as BaseCreature;

			if (c == null)
			{
				return;
			}

			Type type = c.GetType();

			if (!BroadcastTypes.Any(type.IsEqualOrChildOf))
			{
				return;
			}

			string[] team =
				c.DamageEntries.Not(de => de.DamageGiven <= 0 || de.HasExpired)
				 .Where(de => de.Damager is PlayerMobile && !String.IsNullOrWhiteSpace(de.Damager.RawName))
				 .OrderByDescending(de => de.DamageGiven)
				 .Select(de => de.Damager.RawName.WrapUOHtmlColor(de.Damager.GetNotorietyColor()))
				 .ToArray();

			if (team.Length <= 0)
			{
				return;
			}

			string text = String.Format(
				"{0} minutes ago, {1} was slain by {2}!",
				(int)AnnounceDelay.TotalMinutes,
				c.RawName.ToUpperWords().WrapUOHtmlColor(c.GetNotorietyColor()),
				String.Join(", ", team));

			Timer.DelayCall(
				AnnounceDelay,
				() =>
				NetState.Instances.Where(ns => ns != null && ns.Mobile is PlayerMobile)
						.ForEach(ns => ns.Mobile.SendNotification<CreatureDeathNotifyGump>(text, true, 1.0, 30.0)));
		}
	}

	public sealed class CreatureDeathNotifyGump : NotifyGump
	{
		private static void InitSettings(NotifySettings settings)
		{
			settings.Name = "Creature Deaths";
			settings.CanIgnore = true;
		}

		public CreatureDeathNotifyGump(PlayerMobile user, string html)
			: base(user, html)
		{ }
	}
}