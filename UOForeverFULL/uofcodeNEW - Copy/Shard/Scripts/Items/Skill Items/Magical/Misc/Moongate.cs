#region References
using System;

using Server.Engines.CannedEvil;
using Server.Engines.CentralGump;
using Server.Factions;
using Server.Games;
using Server.Gumps;
using Server.Misc;
using Server.Mobiles;
using Server.Multis;
using Server.Network;
using Server.Regions;
using Server.Spells;
#endregion

namespace Server.Items
{
	[DispellableField]
	public class Moongate : Item
	{
		private readonly Mobile m_Caster; // this is not serialized
		private Point3D m_Target;
		private Map m_TargetMap;
		private bool m_Dispellable;

		[CommandProperty(AccessLevel.GameMaster)]
		public Mobile Caster { get { return m_Caster; } }

		[CommandProperty(AccessLevel.GameMaster)]
		public Point3D Target { get { return m_Target; } set { m_Target = value; } }

		[CommandProperty(AccessLevel.GameMaster)]
		public Map TargetMap { get { return m_TargetMap; } set { m_TargetMap = value; } }

		[CommandProperty(AccessLevel.GameMaster)]
		public bool Dispellable { get { return m_Dispellable; } set { m_Dispellable = value; } }

		public virtual bool ShowFeluccaWarning { get { return false; } }

		[Constructable]
		public Moongate()
			: this(Point3D.Zero, null)
		{
			m_Dispellable = true;
		}

		[Constructable]
		public Moongate(bool Dispellable)
			: this(Point3D.Zero, null)
		{
			m_Dispellable = Dispellable;
		}

		[Constructable]
		public Moongate(Point3D target, Map targetMap)
			: base(0xF6C)
		{
			Movable = false;
			Light = LightType.Circle300;

			m_Target = target;
			m_TargetMap = targetMap;
			m_Caster = null;
		}

		public Moongate(Serial serial)
			: base(serial)
		{ }

		public override void OnDoubleClick(Mobile from)
		{
			if (from.Player ||
				(from is BaseCreature && !from.Deleted && from.NetState != null && ((BaseCreature)from).Pseu_CanUseGates))
				// pseudoseer controlled basecreature
			{
				if (from.InRange(GetWorldLocation(), 1))
				{
					CheckGate(from, 1);
				}
				else
				{
					from.SendLocalizedMessage(500446); // That is too far away.
				}
			}
			else
			{
				from.SendMessage("You aren't allowed to use that.");
			}
		}

		public override bool OnMoveOver(Mobile m)
		{
			if (m.Player || (m is BaseCreature && !m.Deleted && m.NetState != null && ((BaseCreature)m).Pseu_CanUseGates))
				// pseudoseer controlled basecreature
			{
				CheckGate(m, 0);
			}

			return true;
		}

		public virtual void CheckGate(Mobile m, int range)
		{
			#region Mondain's Legacy
			if (EraML && m.Hidden && m.AccessLevel == AccessLevel.Player)
			{
				m.RevealingAction();
				m.SendLocalizedMessage(501955); // Your spirit lacks the cohesion for gate travel at this time.
			}
			#endregion

            var pm = Caster as PlayerMobile;

		    if (pm != null && pm.PokerGame != null)
		    {
		        pm.SendMessage(61, "You cannot use this while part of a poker game.");
		        return;
		    }

			new DelayTimer(m, this, range).Start();
		}

		public virtual void OnGateUsed(Mobile m)
		{
			if (m == null || m.Deleted)
			{
				return;
			}

			Point3D newTarget;

			BaseHouse house = BaseHouse.FindHouseAt(m);

			if (house != null && !house.Public && !house.IsFriend(m))
			{
				newTarget = house.BanLocation;

				BaseCreature.TeleportPets(m, newTarget, m_TargetMap);
				m.MoveToWorld(newTarget, m_TargetMap);
			}
		}

		public virtual void UseGate(Mobile m)
		{
			//ClientFlags flags = m.NetState == null ? ClientFlags.None : m.NetState.Flags;
			Region region = Region.Find(m_Target, m_TargetMap);
			var champregion = region.GetRegion(typeof(ChampionSpawn)) as ChampionSpawnRegion;
			var customRegion = region as CustomRegion;

			if (Sigil.ExistsOn(m))
			{
				m.SendLocalizedMessage(1061632); // You can't do that while carrying the sigil.
			}
			else if (SpellHelper.CheckCombat(m))
			{
				m.SendLocalizedMessage(1005564, "", 0x22); // Wouldst thou flee during the heat of battle??
			}
            else if (SpellHelper.IsWind(m_Target, m_TargetMap) && m.Skills[SkillName.Magery].Base < 70.0)
            {
                m.SendLocalizedMessage(503382); // You are not worthy of entrance to the city of Wind!
            }
			else if (m.Spell != null)
			{
				m.SendLocalizedMessage(1049616); // You are too busy to do that at the moment.
			}
			else if ((!m.Alive || (m is PlayerMobile && ((PlayerMobile)m).Young)) && champregion != null && !champregion.CanSpawn())
			{
				m.SendLocalizedMessage(501942); // That location is blocked.
			}
			else if (customRegion != null && customRegion.Controller != null &&
					 (!customRegion.Controller.AllowGateIn || !customRegion.Controller.CanEnter))
			{
				m.SendLocalizedMessage(501942); // That location is blocked.
			}
			else if (customRegion != null && customRegion.Controller != null && customRegion.Controller.NoMounts && m.Mounted)
			{
				m.SendMessage("Your mount refuses to go into the gate!");
			}
			else if (customRegion != null && customRegion.Controller != null && customRegion.Controller.NoPets && m.Followers > 0)
			{
				m.SendMessage("Sorry, but no pets are allowed in that region at this time (including your mount).");
			}
			else if (m_TargetMap != null && m_TargetMap != Map.Internal)
			{
				BaseCreature.TeleportPets(m, m_Target, m_TargetMap);

				m.MoveToWorld(m_Target, m_TargetMap);

				if (m.AccessLevel == AccessLevel.Player || !m.Hidden)
				{
					m.PlaySound(0x1FE);
				}

				OnGateUsed(m);
			}
			else
			{
				m.SendMessage("This moongate does not seem to go anywhere.");
			}
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			writer.Write(1); // version

			writer.Write(m_Target);
			writer.Write(m_TargetMap);

			// Version 1
			writer.Write(m_Dispellable);
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			int version = reader.ReadInt();

			m_Target = reader.ReadPoint3D();
			m_TargetMap = reader.ReadMap();

			if (version >= 1)
			{
				m_Dispellable = reader.ReadBool();
			}
		}

		public virtual bool ValidateUse(Mobile from, bool message)
		{
			if (from.Deleted || Deleted)
			{
				return false;
			}

			if (!PseudoSeerStone.AllowCriminalUseGate && from.Criminal)
			{
				if (PvPController._CriminalCanUseOthersGates)
				{
					if (from == m_Caster)
					{
						from.SendLocalizedMessage(1005561, "", 0x22); // Thou'rt a criminal and cannot escape so easily.
						return false;
					}
				}
				else
				{
					from.SendLocalizedMessage(1005561, "", 0x22); // Thou'rt a criminal and cannot escape so easily.
					return false;
				}
			}

			if (from.Map != Map || !from.InRange(this, 1))
			{
				if (message)
				{
					from.SendLocalizedMessage(500446); // That is too far away.
				}

				return false;
			}

			return true;
		}

		public virtual void BeginConfirmation(Mobile from)
		{
		    CentralGumpProfile profile = null;
		    if (from is PlayerMobile)
		    {
		        profile = CentralGump.EnsureProfile(from as PlayerMobile);
		    }
		    bool ignoremoongate = false;
            if (profile != null)
                ignoremoongate = profile.IgnoreMoongates;
            if (IsInTown(from.Location, from.Map) && !IsInTown(m_Target, m_TargetMap) && !ignoremoongate ||
				(from.Map != Map.Felucca && TargetMap == Map.Felucca && ShowFeluccaWarning))
			{
				if (from.AccessLevel == AccessLevel.Player || !from.Hidden)
				{
					from.Send(new PlaySound(0x20E, from.Location));
				}
				from.CloseGump(typeof(MoongateConfirmGump));
				from.SendGump(new MoongateConfirmGump(from, this));
			}
			else
			{
				EndConfirmation(from);
			}
		}

		public virtual void EndConfirmation(Mobile from)
		{
			if (!ValidateUse(from, true))
			{
				return;
			}

			UseGate(from);
		}

		public virtual void DelayCallback(Mobile from, int range)
		{
			if (!ValidateUse(from, false) || !from.InRange(this, range))
			{
				return;
			}

			if (m_TargetMap != null)
			{
				BeginConfirmation(from);
			}
			else
			{
				from.SendMessage("This moongate does not seem to go anywhere.");
			}
		}

		public static bool IsInTown(Point3D p, Map map)
		{
			if (map == null)
			{
				return false;
			}

			var reg = (GuardedRegion)Region.Find(p, map).GetRegion(typeof(GuardedRegion));

			return (reg != null && !reg.IsDisabled());
		}

		private class DelayTimer : Timer
		{
			private readonly Mobile m_From;
			private readonly Moongate m_Gate;
			private readonly int m_Range;

			public DelayTimer(Mobile from, Moongate gate, int range)
				: base(TimeSpan.FromSeconds(1.0))
			{
				m_From = from;
				m_Gate = gate;
				m_Range = range;
			}

			protected override void OnTick()
			{
				m_Gate.DelayCallback(m_From, m_Range);
			}
		}
	}

	public class ConfirmationMoongate : Moongate
	{
		private int m_GumpWidth;
		private int m_GumpHeight;

		private int m_TitleColor;
		private int m_MessageColor;

		private int m_TitleNumber;
		private int m_MessageNumber;

		private string m_MessageString;

		[CommandProperty(AccessLevel.GameMaster)]
		public int GumpWidth { get { return m_GumpWidth; } set { m_GumpWidth = value; } }

		[CommandProperty(AccessLevel.GameMaster)]
		public int GumpHeight { get { return m_GumpHeight; } set { m_GumpHeight = value; } }

		[CommandProperty(AccessLevel.GameMaster)]
		public int TitleColor { get { return m_TitleColor; } set { m_TitleColor = value; } }

		[CommandProperty(AccessLevel.GameMaster)]
		public int MessageColor { get { return m_MessageColor; } set { m_MessageColor = value; } }

		[CommandProperty(AccessLevel.GameMaster)]
		public int TitleNumber { get { return m_TitleNumber; } set { m_TitleNumber = value; } }

		[CommandProperty(AccessLevel.GameMaster)]
		public int MessageNumber { get { return m_MessageNumber; } set { m_MessageNumber = value; } }

		[CommandProperty(AccessLevel.GameMaster)]
		public string MessageString { get { return m_MessageString; } set { m_MessageString = value; } }

		[Constructable]
		public ConfirmationMoongate()
			: this(Point3D.Zero, null)
		{ }

		[Constructable]
		public ConfirmationMoongate(Point3D target, Map targetMap)
			: base(target, targetMap)
		{ }

		public ConfirmationMoongate(Serial serial)
			: base(serial)
		{ }

		public virtual void Warning_Callback(Mobile from, bool okay, object state)
		{
			if (okay)
			{
				EndConfirmation(from);
			}
		}

		public override void BeginConfirmation(Mobile from)
		{
			if (m_GumpWidth > 0 && m_GumpHeight > 0 && m_TitleNumber > 0 && (m_MessageNumber > 0 || m_MessageString != null))
			{
				from.CloseGump(typeof(WarningGump));
				from.SendGump(
					new WarningGump(
						m_TitleNumber,
						m_TitleColor,
						m_MessageString == null ? m_MessageNumber : (object)m_MessageString,
						m_MessageColor,
						m_GumpWidth,
						m_GumpHeight,
						Warning_Callback,
						from));
			}
			else
			{
				base.BeginConfirmation(from);
			}
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			writer.Write(0); // version

			writer.WriteEncodedInt(m_GumpWidth);
			writer.WriteEncodedInt(m_GumpHeight);

			writer.WriteEncodedInt(m_TitleColor);
			writer.WriteEncodedInt(m_MessageColor);

			writer.WriteEncodedInt(m_TitleNumber);
			writer.WriteEncodedInt(m_MessageNumber);

			writer.Write(m_MessageString);
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			int version = reader.ReadInt();

			switch (version)
			{
				case 0:
					{
						m_GumpWidth = reader.ReadEncodedInt();
						m_GumpHeight = reader.ReadEncodedInt();

						m_TitleColor = reader.ReadEncodedInt();
						m_MessageColor = reader.ReadEncodedInt();

						m_TitleNumber = reader.ReadEncodedInt();
						m_MessageNumber = reader.ReadEncodedInt();

						m_MessageString = reader.ReadString();

						break;
					}
			}
		}
	}

	public class MoongateConfirmGump : Gump
	{
		private readonly Mobile m_From;
		private readonly Moongate m_Gate;

		public MoongateConfirmGump(Mobile from, Moongate gate)
			: base(gate.EraAOS ? 110 : 20, gate.EraAOS ? 100 : 30)
		{
			m_From = from;
			m_Gate = gate;

			if (gate.EraAOS)
			{
				Closable = false;

				AddPage(0);

				AddBackground(0, 0, 420, 280, 5054);

				AddImageTiled(10, 10, 400, 20, 2624);
				AddAlphaRegion(10, 10, 400, 20);

				AddHtmlLocalized(10, 10, 400, 20, 1062051, 30720, false, false); // Gate Warning

				AddImageTiled(10, 40, 400, 200, 2624);
				AddAlphaRegion(10, 40, 400, 200);

				if (from.Map != Map.Felucca && gate.TargetMap == Map.Felucca && gate.ShowFeluccaWarning)
				{
					AddHtmlLocalized(10, 40, 400, 200, 1062050, 32512, false, true);
					// This Gate goes to Felucca... Continue to enter the gate, Cancel to stay here
				}
				else
				{
					AddHtmlLocalized(10, 40, 400, 200, 1062049, 32512, false, true);
					// Dost thou wish to step into the moongate? Continue to enter the gate, Cancel to stay here
				}

				AddImageTiled(10, 250, 400, 20, 2624);
				AddAlphaRegion(10, 250, 400, 20);

				AddButton(10, 250, 4005, 4007, 1, GumpButtonType.Reply, 0);
				AddHtmlLocalized(40, 250, 170, 20, 1011036, 32767, false, false); // OKAY

				AddButton(210, 250, 4005, 4007, 0, GumpButtonType.Reply, 0);
				AddHtmlLocalized(240, 250, 170, 20, 1011012, 32767, false, false); // CANCEL
			}
			else
			{
				AddPage(0);

				AddBackground(0, 0, 420, 400, 5054);
				AddBackground(10, 10, 400, 380, 3000);

				AddHtml(
					20,
					40,
					380,
					60,
					@"Dost thou wish to step into the moongate? Continue to enter the gate, Cancel to stay here",
					false,
					false);

				AddHtmlLocalized(55, 110, 290, 20, 1011012, false, false); // CANCEL
				AddButton(20, 110, 4005, 4007, 0, GumpButtonType.Reply, 0);

				AddHtmlLocalized(55, 140, 290, 40, 1011011, false, false); // CONTINUE
				AddButton(20, 140, 4005, 4007, 1, GumpButtonType.Reply, 0);
			}
		}

		public override void OnResponse(NetState state, RelayInfo info)
		{
			if (info.ButtonID == 1)
			{
				m_Gate.EndConfirmation(m_From);
			}
		}
	}
}