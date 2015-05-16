#region References
using System;
using System.Linq;

using Server;
using Server.Mobiles;
#endregion

namespace VitaNex.Modules.AutoPvP.Battles
{
	public class BoWTeam : UOF_PvPTeam
	{
		private static readonly TimeSpan _OneSecond = TimeSpan.FromSeconds(1.0);

		private DateTime _NextArrowUpdate = DateTime.UtcNow;
		private bool _SolidHueOverride = true;
		
		[CommandProperty(AutoPvP.Access)]
		public BoWBattle BoWBattle { get { return Battle as BoWBattle; } }

		[CommandProperty(AutoPvP.Access)]
		public virtual Point3D CrystalLoc { get; set; }

		[CommandProperty(AutoPvP.Access)]
		public virtual Point3D GatePoint { get; set; }

		[CommandProperty(AutoPvP.Access)]
		public virtual int Points { get; set; }

		[CommandProperty(AutoPvP.Access)]
		public virtual int Multi { get; set; }

		[CommandProperty(AutoPvP.Access)]
		public virtual bool SolidHueOverride
		{
			get { return _SolidHueOverride; }
			set
			{
				_SolidHueOverride = value;
				InvalidateSolidHueOverride();
			}
		}

		/*
		protected override void EnsureConstructDefaults()
		{
			base.EnsureConstructDefaults();
		}
		*/

		public BoWTeam(PvPBattle battle, string name = "Team", int minCapacity = 0, int maxCapacity = 7, int color = 12)
			: base(battle, name, minCapacity, maxCapacity, color)
		{
			Points = 0;
			Multi = 1;
			RespawnOnDeath = true;
			KickOnDeath = false;
		}

		public BoWTeam(PvPBattle battle, GenericReader reader)
			: base(battle, reader)
		{ }

		protected override void OnMicroSync()
		{
			base.OnMicroSync();

			BoWBattle bow = BoWBattle;

			if (bow != null && !bow.Deleted && bow.State == PvPBattleState.Running && !bow.Hidden)
			{
				if (bow.Crystal != null && !bow.Crystal.Deleted)
				{
					bow.Crystal.InvalidateCarrier();
				}

				if (bow.Crystal != null && !bow.Crystal.Deleted && bow.Crystal.Carrier != null)
				{
					bow.Crystal.UpdatePoints();
				}

				if (bow.Crystal == null || DateTime.UtcNow < _NextArrowUpdate)
				{
					return;
				}

				foreach (PlayerMobile player in Members.Keys.Where(player => player != bow.Crystal.Carrier))
				{
					player.QuestArrow = bow.Crystal.Carrier != null
											? new BoWArrow(player, bow.Crystal.Carrier, -1, false)
											: new BoWArrow(player, bow.Crystal, -1, false);
				}

				if (bow.Crystal.Carrier != null && bow.Crystal.Carrier.QuestArrow != null)
				{
					bow.Crystal.Carrier.QuestArrow.Stop();
				}
			}

			_NextArrowUpdate = DateTime.UtcNow + _OneSecond;
		}

		public virtual void InvalidateSolidHueOverride()
		{
			if (!Deserializing)
			{
				ForEachMember(
					pm =>
					{
						InvalidateSolidHueOverride(pm);

						if (pm.QuestArrow != null)
						{
							pm.QuestArrow.Stop();
						}
					});
			}
		}

		public virtual void InvalidateSolidHueOverride(PlayerMobile defender)
		{
			if(Deserializing || defender == null || defender.Deleted || !IsMember(defender) || !defender.InRegion(Battle.BattleRegion))
			{
				return;
			}

			defender.SolidHueOverride = (Battle.State == PvPBattleState.Preparing || Battle.State == PvPBattleState.Running) &&
										SolidHueOverride
											? Color
											: -1;

			if(defender.QuestArrow != null)
			{
				defender.QuestArrow.Stop();
			}
		}

		public override void OnMemberAdded(PlayerMobile pm)
		{
			if(pm == null)
			{
				return;
			}

			BoWBattle bow = BoWBattle;

			if(bow != null && bow.IsFaction)
			{
				CapacityBalance();
			}

			base.OnMemberAdded(pm);

			InvalidateSolidHueOverride(pm);
		}

		public override void OnMemberRemoved(PlayerMobile pm)
		{
			if(pm == null)
			{
				return;
			}

			BoWBattle bow = BoWBattle;

			if(bow != null && bow.IsFaction)
			{
				CapacityBalance();
			}

			base.OnMemberRemoved(pm);

			if(bow != null && !bow.Deleted && bow.Crystal != null && !bow.Crystal.Deleted && bow.Crystal.Carrier == pm)
			{
				bow.Crystal.Reset();
			}

			InvalidateSolidHueOverride(pm);
		}

		public void CapacityBalance()
		{
			BoWBattle bow = BoWBattle;

			if(bow == null || bow.Deleted)
			{
				return;
			}

			foreach(BoWTeam bowTeam in Battle.Teams.OfType<BoWTeam>())
			{
				if(bow.FirstMaxTeam == null)
				{
					bow.FirstMaxTeam = bowTeam;
				}
				else if(bowTeam.Members.Count > bow.FirstMaxTeam.Count)
				{
				    if (bow.SecondMaxTeam != null && bow.FirstMaxTeam.Count > bow.SecondMaxTeam.Count|| bow.SecondMaxTeam == null)
				    {
				        bow.SecondMaxTeam = bow.FirstMaxTeam;
				    }
					bow.FirstMaxTeam = bowTeam;
				}

				if(bow.FirstMaxTeam == bow.SecondMaxTeam)
				{
					bow.SecondMaxTeam = null;
				}

				if(bow.SecondMaxTeam == null && bow.FirstMaxTeam != bowTeam)
				{
					bow.SecondMaxTeam = bowTeam;
				}
				else if(bow.SecondMaxTeam != null && bowTeam.Count > bow.SecondMaxTeam.Count && bowTeam != bow.FirstMaxTeam)
				{
					bow.SecondMaxTeam = bowTeam;
				}
			}

			foreach(BoWTeam bowTeam in Battle.Teams.OfType<BoWTeam>())
			{
				if(bow.SecondMaxTeam != null)
				{
					bowTeam.MaxCapacity = bow.SecondMaxTeam.Count + 2;
				}
				else if(bow.SecondMaxTeam == null)
				{
					bowTeam.MaxCapacity = 7;
				}
			}

			if(bow.FirstMaxTeam != null && bow.FirstMaxTeam.Count < 7)
			{
				bow.FirstMaxTeam = null;
			}

			if(bow.SecondMaxTeam != null && bow.SecondMaxTeam.Count < 5)
			{
				bow.SecondMaxTeam = null;
			}
		}

		public override void OnBattlePreparing(DateTime when)
		{
			base.OnBattlePreparing(when);

			InvalidateSolidHueOverride();
		}

		public override void OnBattleStarted(DateTime when)
		{
			base.OnBattleStarted(when);

			Points = 0;
			Multi = 1;
			InvalidateSolidHueOverride();
		}

		public override void OnBattleEnded(DateTime when)
		{
			base.OnBattleEnded(when);

			InvalidateSolidHueOverride();
			MaxCapacity = 7;
		}

		public override void RemoveMember(PlayerMobile pm, bool teleport)
		{
			base.RemoveMember(pm, true);
		}

		public override void OnMemberDeath(PlayerMobile pm)
		{
			BoWBattle bow = BoWBattle;

			if(bow != null && !bow.Deleted)
			{
				bow.Teams.OfType<BoWTeam>()
				   .Where(t => t != this && bow.Crystal != null && bow.Crystal.Carrier == pm)
				   .ForEach(t => bow.Crystal.Carrier = null);
			}

            InvalidateSolidHueOverride(pm);

			base.OnMemberDeath(pm);
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			int version = writer.SetVersion(1);

			switch(version)
			{
				case 1:
					{
						writer.Write(CrystalLoc);
						writer.Write(GatePoint);
					}
					goto case 0;
				case 0:
					{
						writer.Write(Points);

						writer.Write(SolidHueOverride);
					}
					break;
			}
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			int version = reader.GetVersion();

			switch(version)
			{
				case 1:
					{
						CrystalLoc = reader.ReadPoint3D();
						GatePoint = reader.ReadPoint3D();
					}
					goto case 0;
				case 0:
					{
						Points = reader.ReadInt();

						SolidHueOverride = reader.ReadBool();
					}
					break;
			}
		}
	}
}