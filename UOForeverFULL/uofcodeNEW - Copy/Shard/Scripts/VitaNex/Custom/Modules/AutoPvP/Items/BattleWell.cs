#region References
using System;
using System.Linq;

using Server;
using Server.Items;
using Server.Mobiles;
using Server.Network;

using VitaNex.Targets;
#endregion

namespace VitaNex.Modules.AutoPvP.Battles
{
	public class BattleWell : BaseAddon
	{
		[CommandProperty(AutoPvP.Access, true)]
		public UOF_PvPBattle Battle { get; set; }

		[CommandProperty(AutoPvP.Access)]
		public int RequiredQuantity { get; set; }

		[CommandProperty(AutoPvP.Access)]
		public int CurrentQuantity { get; set; }

		[CommandProperty(AutoPvP.Access)]
		public int WeaponDamageValue { get; set; }

		[CommandProperty(AutoPvP.Access)]
		public int WeaponAccuracyValue { get; set; }

		[CommandProperty(AutoPvP.Access)]
		public int WeaponSlayerValue { get; set; }

		[CommandProperty(AutoPvP.Access)]
		public int ArmorProtectionValue { get; set; }

		[CommandProperty(AutoPvP.Access)]
		public int ArmorSlayerValue { get; set; }

		[Constructable]
		public BattleWell(UOF_PvPBattle battle)
		{
			Battle = battle;

			RequiredQuantity = 1000000;

			AddComponent(new BattleWellPiece(this, 9156), 2, 1, 15);
			AddComponent(new BattleWellPiece(this, 3348), 0, 1, 3);
			AddComponent(new BattleWellPiece(this, 9358), 0, 0, 0);
			AddComponent(new BattleWellPiece(this, 9364), 0, 0, 5);
			AddComponent(new BattleWellPiece(this, 6008), 2, -1, 0);
			AddComponent(new BattleWellPiece(this, 3244), 2, -1, 0);
			AddComponent(new BattleWellPiece(this, 9364), 0, -1, 5);
			AddComponent(new BattleWellPiece(this, 9158), 1, 1, 15);
			AddComponent(new BattleWellPiece(this, 3248), 1, 1, 0);
			AddComponent(new BattleWellPiece(this, 9357), 1, 0, 0);
			AddComponent(new BattleWellPiece(this, 9364), 1, 0, 5);
			AddComponent(new BattleWellPiece(this, 6039), 1, 0, 0);
			AddComponent(new BattleWellPiece(this, 9158), 1, 0, 15);
			AddComponent(new BattleWellPiece(this, 9156), 2, 0, 15);
			AddComponent(new BattleWellPiece(this, 6007), 2, 0, 0);
			AddComponent(new BattleWellPiece(this, 4090), 2, 0, 9);
			AddComponent(new BattleWellPiece(this, 7840), 2, 0, 4);
			AddComponent(new BattleWellPiece(this, 3244), 2, 0, 0);
			AddComponent(new BattleWellPiece(this, 3347), 2, 0, 3);
			AddComponent(new BattleWellPiece(this, 7070), -1, 0, 0);
			AddComponent(new BattleWellPiece(this, 9359), 1, -1, 0);
			AddComponent(new BattleWellPiece(this, 9364), 1, -1, 5);

			WeaponAccuracyValue = 1000;
			WeaponDamageValue = 1000;
			WeaponSlayerValue = 5000;

			ArmorProtectionValue = 800;
			ArmorSlayerValue = 10000;
		}

		public BattleWell(Serial serial)
			: base(serial)
		{ }

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			writer.Write(0); // version

			writer.Write(RequiredQuantity);
			writer.Write(CurrentQuantity);

			writer.Write(WeaponAccuracyValue);
			writer.Write(WeaponDamageValue);
			writer.Write(WeaponSlayerValue);

			writer.Write(ArmorProtectionValue);
			writer.Write(ArmorSlayerValue);
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			reader.ReadInt();

			RequiredQuantity = reader.ReadInt();
			CurrentQuantity = reader.ReadInt();

			WeaponAccuracyValue = reader.ReadInt();
			WeaponDamageValue = reader.ReadInt();
			WeaponSlayerValue = reader.ReadInt();

			ArmorProtectionValue = reader.ReadInt();
			ArmorSlayerValue = reader.ReadInt();
		}
	}

	public class BattleWellPiece : AddonComponent
	{
		[CommandProperty(AccessLevel.GameMaster)]
		public BattleWell Well { get; set; }

		public BattleWellPiece(BattleWell well, int itemid)
			: base(itemid)
		{
			Well = well;
		}

		public override void OnSingleClick(Mobile m)
		{
			if (Well.Battle == null || Well.Battle.Deleted || Well.Battle.State == PvPBattleState.Internal)
			{
				LabelTo(m, "an empty battle well");
				return;
			}

			LabelTo(m, "a {0} battle well", Well.Battle.Name);

			if (Well.Battle.Hidden)
			{
				WhisperTo(
					m,
					54,
					"Battle Goblin: {0:#,0} / {1:#,0} points needed to start {2}",
					Well.CurrentQuantity,
					Well.RequiredQuantity,
					Well.Battle.Name);
			}
		}

		public override void OnDoubleClick(Mobile m)
		{
			if (Well.Battle == null || Well.Battle.Deleted || Well.Battle.State == PvPBattleState.Internal)
			{
				return;
			}

			if (!Well.Battle.Hidden)
			{
				WhisperTo(m, 34, "Battle Goblin: I'm a little busy right now, come back soon!");
				return;
			}

			WhisperTo(m, 54, "Battle Goblin: Throw me down something tasty!");
			m.Target = new ItemSelectTarget<Item>(ProcessItem, null);
		}

		public void WhisperTo(Mobile m, int hue, string message, params object[] args)
		{
			if (m != null && !m.Deleted && !String.IsNullOrWhiteSpace(message))
			{
				PrivateOverheadMessage(MessageType.Whisper, hue, false, String.Format(message, args), m.NetState);
			}
		}

		public void ProcessItem(Mobile owner, Item i)
		{
			if (!i.CheckDoubleClick(owner, false, false, 2, true) || !Well.Battle.Hidden || Well.Battle.State == PvPBattleState.Internal)
			{
				return;
			}

			int value = BaseItemValue(i);

			if (value <= 0)
			{
				WhisperTo(owner, 34, "Battle Goblin: YUCK, what are you trying to do, poison me?");
				return;
			}

			i.Delete();

			WhisperTo(owner, 54, "Battle Goblin: Mmmmm, I'd say this is worth about {0:#,0} points.", value);

			Well.CurrentQuantity += value;

			if (Well.CurrentQuantity < Well.RequiredQuantity)
			{
				WhisperTo(
					owner,
					54,
					"Battle Goblin: {0:#,0} / {1:#,0} points needed to start {2}",
					Well.CurrentQuantity,
					Well.RequiredQuantity,
					Well.Battle.Name);
				return;
			}

			// Is this necessary since everyone gets a join gump popup?
			NetState.Instances.Where(ns => ns != null && ns.Mobile is PlayerMobile)
					.ForEach(
						ns =>
						ns.Mobile.SendNotification<BattleNotifyGump>(
							Well.Battle.Name + " has been started through the battle well!", true, 1.0, 10.0));

			Well.CurrentQuantity = Well.CurrentQuantity - Well.RequiredQuantity;

			Well.Battle.PlayerStarted = true;
			Well.Battle.Hidden = false;
		}

		public int BaseItemValue(Item i)
		{
			int value;

			if (i is BaseWeapon)
			{
				value = WeaponValue(i as BaseWeapon);
			}
			else if (i is BaseArmor)
			{
				value = ArmorValue(i as BaseArmor);
			}
			else if (i is BaseClothing)
			{
				value = ClothingValue(i as BaseClothing);
			}
			else
			{
				value = MiscValue(i);
			}

			return value;
		}

		public int WeaponValue(BaseWeapon w)
		{
			int value = 0;

			switch (w.DamageLevel)
			{
				case WeaponDamageLevel.Regular:
					value += 100;
					break;
				default:
					value += Well.WeaponDamageValue * (int)w.DamageLevel;
					break;
			}

			if (w.Slayer > SlayerName.None)
			{
				value += Well.WeaponSlayerValue;
			}

			if (w.AccuracyLevel > WeaponAccuracyLevel.Regular)
			{
				value += Well.WeaponAccuracyValue * (int)w.AccuracyLevel;
			}

		    if (w is Dagger || w is BaseStaff)
		    {
		        value = 1;
		    }

			return value;
		}

		public int ArmorValue(BaseArmor a)
		{
			int value = 100;

			if (a.Quality == ArmorQuality.Exceptional)
			{
				value += 300;
			}

			if (a.ProtectionLevel > ArmorProtectionLevel.Regular)
			{
				value += Well.ArmorProtectionValue * (int)a.ProtectionLevel;
			}

			if (a.Slayer > SlayerName.None)
			{
				value += Well.ArmorSlayerValue;
			}

			return value;
		}

		public int ClothingValue(BaseClothing c)
		{
			int value = 100;

			if (c.LootType == LootType.Blessed)
			{
				value += 65000;
			}

			return value;
		}

		public int MiscValue(Item i)
		{
			int value = 0;

			if (i is Gold)
			{
				value += i.Amount;
			}

			var check = i as BankCheck;

			if (check != null)
			{
				value += check.Worth;
			}

			var instrument = i as BaseInstrument;

			if (instrument != null)
			{
				if (instrument.Slayer > SlayerName.None)
				{
					value += 5000;
				}
				else
				{
					value += 300;
				}
			}

			return value;
		}

		public BattleWellPiece(Serial serial)
			: base(serial)
		{ }

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			writer.Write(0); // version

			writer.Write(Well);
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			int version = reader.ReadInt();

			switch (version)
			{
				case 0:
					Well = reader.ReadItem<BattleWell>();
					break;
			}
		}
	}
}