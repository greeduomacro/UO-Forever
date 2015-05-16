#region References
using System;
using System.Collections.Generic;

using Server.Misc;
using Server.Network;
using Server.Spells;
using Server.Targeting;
#endregion

namespace Server.Items
{
	public class GreaterFirebomb : Item
	{
		private Timer m_Timer;
		private int m_Ticks;
		private Mobile m_LitBy;
		private List<Mobile> m_Users;

		[Constructable]
		public GreaterFirebomb()
			: this(0x99B)
		{ }

		[Constructable]
		public GreaterFirebomb(int itemID)
			: base(itemID)
		{
			//Name = "a firebomb";
			Weight = 2.0;
			Hue = 1260;
		}

		public GreaterFirebomb(Serial serial)
			: base(serial)
		{ }

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			writer.WriteEncodedInt(0); // version
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			int version = reader.ReadEncodedInt();
		}

		public override void OnDoubleClick(Mobile from)
		{
			if (!IsChildOf(from.Backpack))
			{
				from.SendLocalizedMessage(1042001); // That must be in your pack for you to use it.
			}
			else if (from.EraAOS && (from.Paralyzed || from.Frozen || (from.Spell != null && from.Spell.IsCasting)))
			{
				from.SendLocalizedMessage(1075857); // You cannot use that while paralyzed.
			}
			else
			{
				if (m_Timer == null)
				{
					m_Timer = Timer.DelayCall(TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(1), OnFirebombTimerTick);
					m_LitBy = from;
					from.SendLocalizedMessage(1060582); // You light the firebomb.  Throw it now!
					from.Target = new ThrowTarget(this);
				}
				//else
				//	from.SendLocalizedMessage( 1060581 ); // You've already lit it!  Better throw it now!
				/*
				if ( m_Users == null )
					m_Users = new List<Mobile>();

				if ( !m_Users.Contains( from ) )
					m_Users.Add( from );
*/
			}
		}

		private void OnFirebombTimerTick()
		{
			if (Deleted)
			{
				m_Timer.Stop();
				return;
			}

			if (Map == Map.Internal && HeldBy == null)
			{
				return;
			}

			switch (m_Ticks)
			{
				case 0:
				case 1:
				case 2:
					{
						++m_Ticks;

						if (HeldBy != null)
						{
							HeldBy.PublicOverheadMessage(MessageType.Regular, 957, false, m_Ticks.ToString());
						}
						else if (RootParent == null)
						{
							PublicOverheadMessage(MessageType.Regular, 957, false, m_Ticks.ToString());
						}
						else if (RootParent is Mobile)
						{
							((Mobile)RootParent).PublicOverheadMessage(MessageType.Regular, 957, false, m_Ticks.ToString());
						}

						break;
					}
				default:
					{
						if (HeldBy != null)
						{
							HeldBy.DropHolding();
						}

						if (m_Users != null)
						{
							foreach (Mobile m in m_Users)
							{
								var targ = m.Target as ThrowTarget;

								if (targ != null && targ.Bomb == this)
								{
									Target.Cancel(m);
								}
							}

							m_Users.Clear();
							m_Users = null;
						}

						if (RootParent is Mobile)
						{
							var parent = (Mobile)RootParent;
							parent.SendLocalizedMessage(1060583); // The firebomb explodes in your hand!
							parent.Damage(Utility.Random(3) + 4);
						}
						else if (RootParent == null)
						{
							Geometry.Circle2D(Location, Map, 4, FireBombEffect);
						}

						m_Timer.Stop();
						Delete();
						break;
					}
			}
		}

		public void FireBombEffect(Point3D p, Map map)
		{
			if (map.CanFit(p, 12, true, false))
			{
				new InternalItem(null, p, map, 5, 5);
			}
		}

		private void OnFirebombTarget(Mobile from, object obj)
		{
			if (Deleted || Map == Map.Internal || !IsChildOf(from.Backpack))
			{
				return;
			}

			var p = obj as IPoint3D;

			if (p != null)
			{
				SpellHelper.GetSurfaceTop(ref p);

				from.RevealingAction();

				IEntity to;

				if (p is Mobile)
				{
					to = (Mobile)p;
				}
				else
				{
					to = new Entity(Serial.Zero, new Point3D(p), Map);
				}

				Effects.SendMovingEffect(from, to, ItemID, 7, 0, false, false, Hue - 1, 0);

				Timer.DelayCall(TimeSpan.FromSeconds(1.0), FirebombReposition_OnTick, to);
				Internalize();
			}
		}

		private void FirebombReposition_OnTick(IEntity ent)
		{
			if (Deleted)
			{
				return;
			}

			MoveToWorld(ent.Location, ent.Map);
		}

		private class ThrowTarget : Target
		{
			private readonly GreaterFirebomb m_Bomb;

			public GreaterFirebomb Bomb { get { return m_Bomb; } }

			public ThrowTarget(GreaterFirebomb bomb)
				: base(12, true, TargetFlags.None)
			{
				m_Bomb = bomb;
			}

			protected override void OnTarget(Mobile from, object targeted)
			{
				m_Bomb.OnFirebombTarget(from, targeted);
			}
		}

		public class InternalItem : Item
		{
			private Mobile m_From;
			private int m_MinDamage;
			private int m_MaxDamage;
			private DateTime m_End;
			private Timer m_Timer;

			public Mobile From { get { return m_From; } }

			public override bool BlocksFit { get { return true; } }

			public InternalItem(Mobile from, Point3D loc, Map map, int min, int max)
				: base(0x398C)
			{
				Movable = false;
				Light = LightType.Circle300;

				MoveToWorld(loc, map);

				m_From = from;
				m_End = DateTime.UtcNow + TimeSpan.FromSeconds(4 + Utility.Random(5));

				SetDamage(min, max);

				m_Timer = new InternalTimer(this, m_End);
				m_Timer.Start();
			}

			public override void OnAfterDelete()
			{
				base.OnAfterDelete();

				if (m_Timer != null)
				{
					m_Timer.Stop();
				}
			}

			public InternalItem(Serial serial)
				: base(serial)
			{ }

			public int GetDamage()
			{
				return Utility.RandomMinMax(m_MinDamage, m_MaxDamage);
			}

			private void SetDamage(int min, int max)
			{
				m_MinDamage = min;
				m_MaxDamage = max;
			}

			public override void Serialize(GenericWriter writer)
			{
				base.Serialize(writer);

				writer.Write(0); // version

				writer.Write(m_From);
				writer.Write(m_End);
				writer.Write(m_MinDamage);
				writer.Write(m_MaxDamage);
			}

			public override void Deserialize(GenericReader reader)
			{
				base.Deserialize(reader);

				int version = reader.ReadInt();

				m_From = reader.ReadMobile();
				m_End = reader.ReadDateTime();
				m_MinDamage = reader.ReadInt();
				m_MaxDamage = reader.ReadInt();

				m_Timer = new InternalTimer(this, m_End);
				m_Timer.Start();
			}

			public override bool OnMoveOver(Mobile m)
			{
				if (Visible && m_From != null && (!m_From.EraAOS || m != m_From) && SpellHelper.ValidIndirectTarget(m_From, m) &&
					m_From.CanBeHarmful(m, false))
				{
					m_From.DoHarmful(m);

					m.Damage(GetDamage(), m_From);
					m.PlaySound(0x208);
				}

				return true;
			}

			private class InternalTimer : Timer
			{
				private readonly InternalItem m_Item;
				private readonly DateTime m_End;

				public InternalTimer(InternalItem item, DateTime end)
					: base(TimeSpan.Zero, TimeSpan.FromSeconds(1.0))
				{
					m_Item = item;
					m_End = end;

					Priority = TimerPriority.FiftyMS;
				}

				protected override void OnTick()
				{
					if (m_Item.Deleted)
					{
						return;
					}

					if (DateTime.UtcNow > m_End)
					{
						m_Item.Delete();
						Stop();
						return;
					}

					Mobile from = m_Item.From;

					if (m_Item.Map == null || from == null)
					{
						return;
					}

					var mobiles = new List<Mobile>();

					foreach (Mobile mobile in m_Item.GetMobilesInRange(0))
					{
						mobiles.Add(mobile);
					}

					for (int i = 0; i < mobiles.Count; i++)
					{
						Mobile m = mobiles[i];

						if ((m.Z + 16) > m_Item.Z && (m_Item.Z + 12) > m.Z && (!from.EraAOS || m != from) &&
							SpellHelper.ValidIndirectTarget(from, m) && from.CanBeHarmful(m, false))
						{
							from.DoHarmful(m);

							m.Damage(m_Item.GetDamage(), from);
							m.PlaySound(0x208);
						}
					}
				}
			}
		}
	}
}