#region References
using System;
using System.Globalization;

using Server;
using Server.Mobiles;
using Server.Network;

using VitaNex.FX;
#endregion

namespace VitaNex.Modules.AutoPvP.Battles
{
	public class BoWCrystal : Item
	{
		private static readonly TimeSpan FiveSeconds = TimeSpan.FromSeconds(5.0);
		private static readonly TimeSpan OneSecond = TimeSpan.FromSeconds(1.0);
		private static readonly TimeSpan HalfSecond = TimeSpan.FromSeconds(0.5);

		private PlayerMobile _Carrier;

		//private CarryEffectTimer _Timer;
		private DateTime _nextEffectValidation = DateTime.UtcNow;
		private DateTime _nextPointUpdate = DateTime.UtcNow;

		public override bool Nontransferable { get { return Parent is Mobile; } }
		public override bool HandlesOnMovement { get { return true; } } //for power crystal shock

		[CommandProperty(AutoPvP.Access)]
		public BoWBattle Battle { get; set; }

		[CommandProperty(AutoPvP.Access)]
		public PlayerMobile Carrier
		{
			get { return _Carrier; }
			set
			{
				if (_Carrier == null || _Carrier == value)
				{
					return;
				}

				if (value == null)
				{
					Drop(_Carrier);
				}
				else
				{
					Drop(_Carrier);
					Steal(value);
				}
			}
		}

		public BoWCrystal(BoWBattle battle)
			: base(8738)
		{
			Battle = battle;

			Name = "the Crystal of Power";
			Hue = 2498;
			Movable = false;
		}

		public BoWCrystal(Serial serial)
			: base(serial)
		{ }

		//reset power crystal
		public void Reset()
		{
			if (Battle == null || Battle.Map == null || Battle.Map == Map.Internal || Battle.CrystalLoc == Point3D.Zero)
			{
				return;
			}

			Battle.LocalBroadcast("The Crystal of Power has been reset.");
			MoveToWorld(Battle.CrystalLoc, Battle.Map);
		}

		public void Drop(PlayerMobile attacker)
		{
			if (attacker == null || attacker.Deleted || attacker != _Carrier)
			{
				return;
			}

			_Carrier = null;

			var t = Battle.FindTeam<BoWTeam>(attacker);

			if (t != null)
			{
				attacker.SolidHueOverride = t.Color;

				MoveToWorld(attacker.Location, attacker.Map);
				Battle.OnDrop(t);
			}
			else
			{
				Reset();
			}

			InvalidateCarrier();
		}

		public override void OnMovement(Mobile m, Point3D oldLocation)
		{
			if (Parent != null || !Utility.InRange(Location, m.Location, 2) || Utility.InRange(Location, oldLocation, 2))
			{
				return;
			}

			PublicOverheadMessage(
				MessageType.Regular, 1287, false, "The crystal emits an audible hum and discharges a bolt of energy");

			Effects.SendBoltEffect(m, true, 33);

			m.Damage(10);
		}

		public void Steal(PlayerMobile attacker)
		{
			if (Battle == null || Battle.Deleted || attacker == null || attacker.Deleted || attacker == _Carrier || attacker.Backpack == null ||
				attacker.Backpack.Deleted || !attacker.Backpack.TryDropItem(attacker, this, true))
			{
				return;
			}

			Effects.SendIndividualFlashEffect(attacker, (FlashType)2);

			_Carrier = attacker;

			attacker.SolidHueOverride = 2498;

			Battle.EnsureStatistics(Carrier)["Crystal Steals"]++;

			Battle.OnStolen(attacker, Battle.FindTeam<BoWTeam>(attacker));

			InvalidateCarrier();
		}

		public override void OnParentDeleted(object parent)
		{
			Drop(parent as PlayerMobile);

			base.OnParentDeleted(parent);
		}

		public override void OnDoubleClick(Mobile from)
		{
			if (Battle != null && Battle.State != PvPBattleState.Running)
			{
				PublicOverheadMessage(
					MessageType.Regular, 1287, false, "The Crystal of Power is currently being used to power the dimensional gateway.");
				return;
			}

			if (!this.CheckDoubleClick(from, true, false, 2) || !(from is PlayerMobile))
			{
				return;
			}

			var pm = (PlayerMobile)from;

			if (Battle != null && !Battle.IsParticipant(pm))
			{
				pm.SendMessage("You must be a participant to perform that action.");
				return;
			}

			if (_Carrier != pm)
			{
				_nextPointUpdate = DateTime.UtcNow;
				Steal(pm);
			}
			else
			{
				Drop(pm);
			}
		}

		public void InvalidateCarrier()
		{
			if (_Carrier == null || _Carrier.Deleted)
			{
				return;
			}

			if (IsChildOf(_Carrier.Backpack))
			{
				InvalidateCarryEffect();
				return;
			}

			Movable = false;
		}

		public void InvalidateCarryEffect()
		{
			if (_Carrier == null || _Carrier.Deleted || !IsChildOf(_Carrier.Backpack) || DateTime.UtcNow < _nextEffectValidation)
			{
				return;
			}

			new EffectInfo(_Carrier.Clone3D(0, 0, 22), _Carrier.Map, ItemID, 2498).Send();

			_nextEffectValidation = DateTime.UtcNow + (_Carrier.Direction.HasFlag(Direction.Running) ? HalfSecond : OneSecond);
		}

		public void UpdatePoints()
		{
			if (_Carrier == null || _Carrier.Deleted || !IsChildOf(_Carrier.Backpack) || DateTime.UtcNow < _nextPointUpdate)
			{
				return;
			}

			var owner = Battle.FindTeam<BoWTeam>(Carrier);

			if (owner != null && !owner.Deleted)
			{
				owner.Points = owner.Points + (1 * owner.Multi);

				Battle.EnsureStatistics(Carrier)["Crystal Points"] += 1 * owner.Multi;

				Carrier.RevealingAction();
				Carrier.PublicOverheadMessage(MessageType.Regular, 2049, false, owner.Points.ToString(CultureInfo.InvariantCulture));
			}

			_nextPointUpdate = DateTime.UtcNow + FiveSeconds;
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			int version = writer.SetVersion(0);

			switch (version)
			{
				case 0:
					writer.Write(_Carrier);
					break;
			}
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			int version = reader.GetVersion();

			switch (version)
			{
				case 0:
					_Carrier = reader.ReadMobile<PlayerMobile>();
					break;
			}
		}
	}
}