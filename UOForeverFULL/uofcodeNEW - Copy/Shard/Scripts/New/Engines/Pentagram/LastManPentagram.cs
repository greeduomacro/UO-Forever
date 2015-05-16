using System;
using System.Collections.Generic;
using Server.Mobiles;
using Server.Gumps;
using Server;
using Server.Items;
using Server.Commands;
using Server.Engines.Cleanup;

namespace Server.Events.LastManPentagram
{
	public class LastManPentagram : Item
	{
		private Timer m_Timer;

		private String m_Location;
		private int m_TimerTicksRequired; //to win. Timer ticks every 6 seconds, so we need 10 ticks tor 60 seconds time.
		private int m_Count;
		private PentagramAddon m_Addon;

		[CommandProperty(AccessLevel.GameMaster)]
		public String PentagramLocation { get { return m_Location; } set { m_Location = value; InvalidateProperties(); } }

		[CommandProperty(AccessLevel.GameMaster)]
		public int SecondsRequired
		{
			get { return m_TimerTicksRequired * 6; }
			set { m_TimerTicksRequired = value / 6; InvalidateProperties(); } }

		[CommandProperty(AccessLevel.GameMaster)]
		public int Count { get { return m_Count; } set { m_Count = value; InvalidateProperties(); } }

		[CommandProperty(AccessLevel.GameMaster)]
		public bool Running
		{
			get { return m_Timer != null && m_Timer.Running; }
			set
			{
				if (value != Running)
				{
					if (Running)
					{
						if (m_Timer != null && m_Timer.Running)
							m_Timer.Stop();

						m_Timer = null;
						CommandHandlers.BroadcastMessage(AccessLevel.Player, 1150, "The pentagram event has been cancelled!");
						return;
					}

					else
					{
						m_Timer = new LMPTimer(this, m_Count, m_Location, m_TimerTicksRequired);
						m_Timer.Start();
					}
				}
			}
		}

		[Constructable]
		public LastManPentagram()
			: base(0x1F1C)
		{
			m_Count = 10;
			m_TimerTicksRequired = 10;
			m_Addon = new PentagramAddon();
			Movable = false;
			Visible = false;
		}

		public override void OnDoubleClick(Mobile from)
		{
			if(from.AccessLevel >= AccessLevel.GameMaster)
				from.SendGump(new PropertiesGump(from, this));
			base.OnDoubleClick(from);
		}

		public override void OnDelete()
		{
			if (m_Addon != null && !m_Addon.Deleted)
				m_Addon.Delete();
		}

		public override void OnLocationChange(Point3D oldLocation)
		{
			base.OnLocationChange(oldLocation);
			if (m_Addon != null && !m_Addon.Deleted)
				m_Addon.Location = Location;
		}

		public override void OnMapChange()
		{
			base.OnMapChange();
			if (m_Addon != null && !m_Addon.Deleted)
				m_Addon.Map = Map;
		}

		public LastManPentagram(Serial serial) : base(serial) { }

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			writer.WriteEncodedInt( (int)0 ); //version

			writer.WriteEncodedInt(m_TimerTicksRequired);
			writer.Write(m_Location);
			writer.Write((Item)m_Addon);
			writer.Write(m_Count);
	   }

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);
			int version = reader.ReadInt();

			switch ( version )
			{
				case 0:
				{
					m_TimerTicksRequired = reader.ReadEncodedInt();
					m_Location = reader.ReadString();
					m_Addon = reader.ReadItem() as PentagramAddon;
					m_Count = reader.ReadInt();
					break;
				}
			}
		}

		private class LMPTimer : Timer
		{
			private LastManPentagram m_LMP;
			private int m_Count;
			private String m_Location;
			private bool m_GetWinner;
			private Mobile m_Winner;
			private int m_ConsWinner;
			private int m_TimerTicksRequired;

			public LMPTimer(LastManPentagram lmp, int count, String location, int ticksRequired)
				: base(TimeSpan.FromSeconds(6.0), TimeSpan.FromSeconds(6.0))
			{
				m_LMP = lmp;
				m_Count = count * 10;
				m_Location = location;
				m_TimerTicksRequired = ticksRequired;
				CommandHandlers.BroadcastMessage(AccessLevel.Player, 1150, String.Format("The pentagram at {0} will be active in {1} minutes. The first one who stands on it alone for {2} seconds will receive a random reward. Hiding at this pentagram is deadly mistake.", location, count, ticksRequired * 6 ));
			}

			protected override void OnTick()
			{
				if (m_LMP == null || m_LMP.Deleted)
					Stop();

				string message = "";

				if (!m_GetWinner)
				{
					m_Count--;

					if (m_Count < 1)
					{
						m_GetWinner = true;
						CommandHandlers.BroadcastMessage(AccessLevel.Player, 1150, String.Format("The pentagram at {0} is active!", m_Location));
					}

					else if (m_Count % 10 == 0)
						CommandHandlers.BroadcastMessage(AccessLevel.Player, 1150, String.Format("The pentagram at {0} will be active in {1} minutes. The first one who stands on it alone for {2} seconds will receive a random reward. Hiding at this pentagram is deadly mistake.", m_Location, m_Count / 10, m_TimerTicksRequired * 6));
				}

				else
				{
					List<Mobile> moblist = new List<Mobile>();

					foreach (Mobile m in m_LMP.Map.GetMobilesInRange(m_LMP.Location, 1)) // Edit by Silver: Changed range from 2 to 1
						if (m.Player && m.Alive && m.AccessLevel == AccessLevel.Player && m.Z + 2 >= m_LMP.Z && m.Z - 5 <= m_LMP.Z ) // Edit by Silver: Z-check
								moblist.Add(m);

						for (int i = moblist.Count -1; i >= 0; i-- )
						{
							Mobile m = moblist[i];
							if (m.Hidden)
							{
								m.Kill();
								moblist.RemoveAt(i);
							}
						}

					if (moblist.Count == 1)
					{
						if (moblist[0] == m_Winner)
						{
							if (m_ConsWinner == 0)
								CommandHandlers.BroadcastMessage(AccessLevel.Player, 1150, m_Winner.Name + String.Format(" was appointed as the sole person standing on the {0} pentagram!", m_Location));
							else
								message = m_Winner.Name + String.Format(" was appointed as the sole person standing on the {0} pentagram!", m_Location);
							m_ConsWinner++;
						}
						else
						{
							m_Winner = moblist[0];
							m_ConsWinner = 0;
						}

						if (m_ConsWinner >= m_TimerTicksRequired)
						{
							CommandHandlers.BroadcastMessage(AccessLevel.Player, 1150, m_Winner.Name + " has won the pentagram event.");
							
						/*	GoldMarkCoin p = new GoldMarkCoin();
							p.Amount = 1000;

							m_Winner.BankBox.DropItem(p);
							m_Winner.SendMessage("You have won the pentagram event and 1000 Marks have been deposited into your bankbox.");
						*/	
							message = "";
							Stop();
						}
					}

					else
					{
						m_ConsWinner = 0;
						if (Utility.Random(6) == 0)
							CommandHandlers.BroadcastMessage(AccessLevel.Player, 1150, String.Format("No person could be appointed to be the only one standing on the {0} pentagram", m_Location));
						else
							message = String.Format("No person could be appointed to be the only one standing on the {0} pentagram", m_Location);
					}
				}

				List<Mobile> mobilelist = new List<Mobile>();
				foreach (Mobile m in m_LMP.Map.GetMobilesInRange(m_LMP.Location, 20))
					if (m.Player && m.Alive)
						mobilelist.Add(m);

				foreach (Mobile m in mobilelist)
				{
					if (!m.Criminal && m.AccessLevel == AccessLevel.Player)
						m.Criminal = true;

					if (message != "")
						m.SendMessage(message);
				}
			}
		}
	}
}