#region References
using System;
using System.Collections.Generic;

using Server.Misc;
using Server.Mobiles;
using Server.Spells;
using Server.Targeting;
#endregion

namespace Server.Items
{
	public abstract class BaseConfusionBlastPotion : BasePotion
	{
		public abstract int Radius { get; }

		public override bool RequireFreeHand { get { return false; } }

		public BaseConfusionBlastPotion(PotionEffect effect)
			: base(0xF06, effect)
		{
			Hue = 0x48D;
		}

		public BaseConfusionBlastPotion(Serial serial)
			: base(serial)
		{ }

		public override bool Drink(Mobile from)
		{
			if (EraAOS && (from.Paralyzed || from.Frozen || (from.Spell != null && from.Spell.IsCasting)))
			{
				from.SendLocalizedMessage(1062725); // You can not use that potion while paralyzed.
				return false;
			}

			int delay = GetDelay(from);

			if (delay > 0)
			{
				from.SendLocalizedMessage(1072529, String.Format("{0}\t{1}", delay, delay > 1 ? "seconds." : "second."));
					// You cannot use that for another ~1_NUM~ ~2_TIMEUNITS~
				return false;
			}

			var targ = from.Target as ThrowTarget;

			if (targ != null && targ.Potion == this)
			{
				return false;
			}

			from.RevealingAction();

			if (!m_Users.Contains(from))
			{
				m_Users.Add(from);
			}

			from.Target = new ThrowTarget(this);
			return true;
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			writer.Write(0); // version
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			int version = reader.ReadInt();
		}

		private readonly List<Mobile> m_Users = new List<Mobile>();

		public void Explode_Callback(IEntity[] states)
		{
			Explode((Mobile)states[0], states[1].Location, states[1].Map);
		}

		public virtual void Explode(Mobile from, Point3D loc, Map map)
		{
			if (Deleted || map == null)
			{
				return;
			}

			Consume();

			// Check if any other players are using this potion
			for (int i = 0; i < m_Users.Count; i ++)
			{
				var targ = m_Users[i].Target as ThrowTarget;

				if (targ != null && targ.Potion == this)
				{
					Target.Cancel(from);
				}
			}

			// Effects
			Effects.PlaySound(loc, map, 0x207);

			Geometry.Circle2D(loc, map, Radius, BlastEffect, 270, 90);

			Timer.DelayCall(
				TimeSpan.FromSeconds(0.3), new TimerStateCallback<IEntity>(CircleEffect2), new Entity(Serial.Zero, loc, map));

			foreach (Mobile mobile in map.GetMobilesInRange(loc, Radius))
			{
				if (mobile is BaseCreature)
				{
					var mon = (BaseCreature)mobile;

					mon.Pacify(from, DateTime.UtcNow + TimeSpan.FromSeconds(5.0)); // TODO check
				}
			}
		}

		#region Effects
		public virtual void BlastEffect(Point3D p, Map map)
		{
			if (map.CanFit(p, 12, true, false))
			{
				Effects.SendLocationEffect(p, map, 0x376A, 4, 9);
			}
		}

		public void CircleEffect2(IEntity ent)
		{
			Geometry.Circle2D(ent.Location, ent.Map, Radius, BlastEffect, 90, 270);
		}
		#endregion

		#region Delay
		private static readonly Dictionary<Mobile, Timer> m_Delay = new Dictionary<Mobile, Timer>();

		public static void AddDelay(Mobile m)
		{
			Timer timer;
			m_Delay.TryGetValue(m, out timer);

			if (timer != null)
			{
				timer.Stop();
			}

			m_Delay[m] = Timer.DelayCall(TimeSpan.FromSeconds(60), EndDelay, m);
		}

		public static int GetDelay(Mobile m)
		{
			Timer timer;
			m_Delay.TryGetValue(m, out timer);

			if (timer != null && timer.Next > DateTime.UtcNow)
			{
				return (int)(timer.Next - DateTime.UtcNow).TotalSeconds;
			}

			return 0;
		}

		public static void EndDelay(Mobile m)
		{
			Timer timer;
			m_Delay.TryGetValue(m, out timer);

			if (timer != null)
			{
				timer.Stop();
				m_Delay.Remove(m);
			}
		}
		#endregion

		private class ThrowTarget : Target
		{
			private readonly BaseConfusionBlastPotion m_Potion;

			public BaseConfusionBlastPotion Potion { get { return m_Potion; } }

			public ThrowTarget(BaseConfusionBlastPotion potion)
				: base(12, true, TargetFlags.None)
			{
				m_Potion = potion;
			}

			protected override void OnTarget(Mobile from, object targeted)
			{
				if (m_Potion.Deleted || m_Potion.Map == Map.Internal)
				{
					return;
				}

				var p = targeted as IPoint3D;

				if (p == null || from.Map == null)
				{
					return;
				}

				// Add delay
				AddDelay(from);

				SpellHelper.GetSurfaceTop(ref p);

				from.RevealingAction();

				IEntity to = new Entity(Serial.Zero, new Point3D(p), from.Map);

				Effects.SendMovingEffect(
					from, p is Mobile ? (Mobile)p : to, 0xF0D, 7, 0, false, false, Math.Min(m_Potion.Hue - 1, 0), 0);
				Timer.DelayCall(TimeSpan.FromSeconds(1.0), m_Potion.Explode_Callback, new[] {from, to});
			}
		}
	}
}