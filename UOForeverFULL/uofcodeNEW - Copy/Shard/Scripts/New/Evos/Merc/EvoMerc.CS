#region References
using System;
using System.Linq;

using Server.Engines.XmlSpawner2;
using Server.Gumps;
using Server.Items;
using Server.Network;
using Server.Targeting;
#endregion

namespace Server.Mobiles
{
	public class EvoMerc : BaseCreature
	{
		public static readonly object From = new object();
		public static readonly object Vendor = new object();
		public static readonly object Price = new object();

		private static readonly EvoMercBuyInfo[] m_SellList = new[]
		{
			new EvoMercBuyInfo(
				1018357,
				0,
				Layer.Hair,
				typeof(ChangeHairstylGump),
				new[] {From, Vendor, Price, Layer.Hair, ChangeHairstyleEnt.HairEntries}),
			new EvoMercBuyInfo(
				1018358,
				0,
				Layer.FacialHair,
				typeof(ChangeHairstylGump),
				new[] {From, Vendor, Price, Layer.FacialHair, ChangeHairstyleEnt.BeardEntries}),
			new EvoMercBuyInfo(
				1018359,
				0,
				Layer.Hair,
				typeof(ChangeHairHuGump),
				new[] {From, Vendor, Price, new[] {Layer.Hair, Layer.FacialHair}, ChangeHairHueEnt.RegularEntries}),
			new EvoMercBuyInfo(
				1018360,
				0,
				Layer.Hair,
				typeof(ChangeHairHuGump),
				new[] {From, Vendor, Price, new[] {Layer.Hair, Layer.FacialHair}, ChangeHairHueEnt.BrightEntries}),
			new EvoMercBuyInfo(
				1018361,
				0,
				Layer.Hair,
				typeof(ChangeHairHuGump),
				new[] {From, Vendor, Price, new[] {Layer.Hair}, ChangeHairHueEnt.RegularEntries}),
			new EvoMercBuyInfo(
				1018362,
				0,
				Layer.FacialHair,
				typeof(ChangeHairHuGump),
				new[] {From, Vendor, Price, new[] {Layer.FacialHair}, ChangeHairHueEnt.RegularEntries}),
			new EvoMercBuyInfo(
				1018363,
				0,
				Layer.Hair,
				typeof(ChangeHairHuGump),
				new[] {From, Vendor, Price, new[] {Layer.Hair}, ChangeHairHueEnt.BrightEntries}),
			new EvoMercBuyInfo(
				1018364,
				0,
				Layer.FacialHair,
				typeof(ChangeHairHuGump),
				new[] {From, Vendor, Price, new[] {Layer.FacialHair}, ChangeHairHueEnt.BrightEntries})
		};

		private Timer m_BreatheTimer;
		private DateTime m_EndBreathe;

		private Timer m_MercLoyaltyTimer;

		public DateTime EndMercLoyalty { get; set; }

		public bool S1 { get; set; }
		public bool S2 { get; set; }
		public bool S3 { get; set; }
		public bool S4 { get; set; }
		public bool S5 { get; set; }
		public bool S6 { get; set; }

		[CommandProperty(AccessLevel.GameMaster)]
		public int KP { get; set; }

		[CommandProperty(AccessLevel.GameMaster)]
		public int Stage { get; set; }

		public Type TypeOfCurrency { get { return Expansion == Expansion.T2A ? typeof(Silver) : typeof(Gold); } }

		[Constructable]
		public EvoMerc()
			: base(AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4)
		{
			Name = "A Mercenary";
			Title = "";

			SpeechHue = Utility.RandomDyedHue();

			Hue = Utility.RandomSkinHue();
			Female = Utility.RandomBool();

			Body = Female ? 401 : 400;

			if (Female)
			{
				AddItem(
					new Item(Utility.RandomList(0x203B, 0x203C, 0x203D, 0x2044, 0x2045, 0x2047, 0x2049, 0x204A))
					{
						Hue = Utility.RandomHairHue(),
						Layer = Layer.Hair,
						Movable = false
					});
			}
			else
			{
				var hair = new Item(Utility.RandomList(0x203B, 0x203C, 0x203D, 0x2044, 0x2045, 0x2047, 0x2049, 0x204A))
				{
					Hue = Utility.RandomHairHue(),
					Layer = Layer.Hair,
					Movable = false
				};

				AddItem(hair);

				AddItem(
					new Item(Utility.RandomList(0x203E, 0x203F, 0x2040, 0x2041, 0x204B, 0x204C, 0x204D))
					{
						Hue = hair.Hue,
						Layer = Layer.FacialHair,
						Movable = false
					});
			}

			InitStats(40, 30, 5);

			AddItem(new Robe());

			Item weapon;

			switch (Utility.Random(1))
			{
				case 0:
					weapon = new MercenaryKatana();
					break;
				default:
					weapon = new MercenaryKatana();
					break;
			}

			weapon.Movable = false;
			AddItem(weapon);

			SetSkill(SkillName.Swords, 20.0);
			SetSkill(SkillName.Macing, 20.0);
			SetSkill(SkillName.Fencing, 20.0);
			SetSkill(SkillName.Swords, 20.0);

			SetStr(296, 325);
			SetDex(56, 75);
			SetInt(76, 96);

			SetDamage(11, 17);

			S1 = true;
			S2 = true;
			S3 = true;
			S4 = true;
			S5 = true;
			S6 = true;
			Stage = 1;

			SetSkill(SkillName.Fencing, 120, 120);
			SetSkill(SkillName.Swords, 120, 120);
			SetSkill(SkillName.Anatomy, 120, 120);
			SetSkill(SkillName.Archery, 120, 120);
			SetSkill(SkillName.Tactics, 120, 120);
			SetSkill(SkillName.Macing, 120, 120);
			SetSkill(SkillName.MagicResist, 120, 120);
			SetSkill(SkillName.Parry, 120, 120);

			Fame = 300;
			Karma = -300;

			VirtualArmor = 30;

			ControlSlots = 3;
			PackItem(new Diamond());
			m_MercLoyaltyTimer = new MercLoyaltyTimer(this, TimeSpan.FromSeconds(5.0));
			m_MercLoyaltyTimer.Start();
			EndMercLoyalty = DateTime.UtcNow + TimeSpan.FromSeconds(5.0);
		}

		public EvoMerc(Serial serial)
			: base(serial)
		{ }

		public override bool HandlesOnSpeech(Mobile from)
		{
			return true;
		}

		public override void OnSpeech(SpeechEventArgs e)
		{
			if (XmlScript.HasTrigger(this, TriggerName.onSpeech) &&
				UberScriptTriggers.Trigger(this, e.Mobile, TriggerName.onSpeech, null, e.Speech))
			{
				return;
			}

			Mobile from = e.Mobile;

			string keyword = Name + " restyle";
			string keyword2 = Name + " undress";
			string keyword3 = Name + " dismount";
			string keyword4 = Name + " mount";
			string keyword5 = Name + " stats";
			string keyword6 = Name + " help";

			if (e.Speech.ToLower().IndexOf(keyword.ToLower(), StringComparison.Ordinal) >= 0)
			{
				if (ControlMaster != from)
				{
					return;
				}

				from.SendGump(new EvoMercBuyGump(from, this, m_SellList));
				from.SendMessage("Your Mercenary will style it's hair for free for you!");
				e.Handled = true;
			}

			if (e.Speech.ToLower().IndexOf(keyword2.ToLower(), StringComparison.Ordinal) >= 0)
			{
				if (ControlMaster != from)
				{
					return;
				}

				if (Stage < 6)
				{
					from.SendMessage("Your Mercenary cannot do that yet.");
					return;
				}

				for (int i = Items.Count - 1; i >= 0; --i)
				{
					if (i >= Items.Count)
					{
						continue;
					}

					Item item = Items[i];

					if (item is Container || item.Layer == Layer.FacialHair || item.Layer == Layer.Hair || item is IMountItem)
					{
						continue;
					}

					item.Movable = true;
					from.AddToBackpack(item);
				}

				e.Handled = true;
			}

			if (e.Speech.ToLower().IndexOf(keyword3.ToLower(), StringComparison.Ordinal) >= 0)
			{
				if (ControlMaster != from)
				{
					return;
				}

				if (Mount != null)
				{
					for (int i = 0; i < Items.Count; ++i)
					{
						Item item = Items[i];

						if (!(item is IMountItem))
						{
							continue;
						}

						IMount mount = ((IMountItem)item).Mount;

						if (mount != null)
						{
							mount.Rider = null;

							if (mount is EtherealMount)
							{
								var ethy = (EtherealMount)mount;

								if (from.Backpack != null)
								{
									from.Backpack.DropItem(ethy);
								}
							}
						}

						if (Items.IndexOf(item) == -1)
						{
							--i;
						}
					}

					for (int i = 0; i < Items.Count; ++i)
					{
						Item item = Items[i];

						if (item.Layer != Layer.Mount)
						{
							continue;
						}

						if (from.Backpack != null)
						{
							from.Backpack.DropItem(item);
						}

						--i;
					}
				}
				else
				{
					from.SendMessage(Name + " is not on a mount.");
				}

				e.Handled = true;
			}

			if (e.Speech.ToLower().IndexOf(keyword4.ToLower(), StringComparison.Ordinal) >= 0)
			{
				if (ControlMaster != from)
				{
					return;
				}

				from.SendMessage("Target a mount or an ethereal mount you would like to place " + Name + " on.");
				from.Target = new MountTarget(this);
				e.Handled = true;
			}

			if (e.Speech.ToLower().IndexOf(keyword5.ToLower(), StringComparison.Ordinal) >= 0)
			{
				if (ControlMaster != from)
				{
					return;
				}

				from.SendGump(new StatGump(from, this));
				e.Handled = true;
			}

			if (e.Speech.ToLower().IndexOf(keyword6.ToLower(), StringComparison.Ordinal) >= 0)
			{
				if (ControlMaster != from)
				{
					return;
				}

				Say("I will follow these commands: restyle, undress, mount, dismount, and stats.");
				e.Handled = true;
			}

			base.OnSpeech(e);
		}

		private class MountTarget : Target
		{
			private readonly EvoMerc m_Merc;

			public MountTarget(EvoMerc merc)
				: base(1, false, TargetFlags.None)
			{
				m_Merc = merc;
			}

			protected override void OnTarget(Mobile from, object o)
			{
				if (m_Merc.Mount == null)
				{
					if (o is EtherealMount)
					{
						var mount = (EtherealMount)o;

						if (mount.Rider == null)
						{
							if (mount.IsChildOf(from.Backpack))
							{
								mount.Rider = m_Merc;
							}
							else
							{
								from.SendMessage("The ethereal mount must been in your pack for you to use it.");
							}
						}
						else
						{
							from.SendMessage("This ethereal mount is already in use by something else.");
						}
					}
					else if (o is BaseMount)
					{
						var mount = (BaseMount)o;

						if (mount.Rider == null)
						{
							if (mount.ControlMaster == from)
							{
								mount.Rider = m_Merc;
							}
							else
							{
								from.SendMessage("You do not own this mount.");
							}
						}
						else
						{
							from.SendMessage("This mount is already in use by something else.");
						}
					}
					else
					{
						from.SendMessage("That is an invalid target.");
					}
				}
				else
				{
					from.SendMessage(m_Merc.Name + " is already mounted.");
				}
			}
		}

		public override void OnGaveMeleeAttack(Mobile defender)
		{
			int kpgainmin, kpgainmax;

			switch (Stage)
			{
				case 1:
					{
						if (defender is BaseCreature)
						{
							var bc = (BaseCreature)defender;

							if (bc.Controlled != true)
							{
								kpgainmin = 5 + (bc.HitsMax) / 10;
								kpgainmax = 5 + (bc.HitsMax) / 5;

								KP += Utility.RandomList(kpgainmin, kpgainmax);
							}
						}

						if (KP >= 85000)
						{
							if (S1)
							{
								S1 = false;

								int hits = HitsMax + 500;
								int va = VirtualArmor + 10;
								int mindamage = DamageMin + 1;
								int maxdamage = DamageMax + 1;

								Warmode = false;

								Say("*" + Name + " achieves a new level of combat*");
								SetDamage(mindamage, maxdamage);
								SetHits(hits);

								VirtualArmor = va;
								Stage = 2;
								Title = "the Shadow Knight";

								RawStr += 200;
								RawInt += 30;
								RawDex += 20;

								Item del = FindItemOnLayer(Layer.OuterTorso); // The Robe

								if (del != null)
								{
									del.Delete();
								}

								del = FindItemOnLayer(Layer.InnerTorso);

								if (del != null)
								{
									del.Delete();
								}

								BaseArmor chest = new LeatherChest();
								chest.Hue = 1175;
								chest.Movable = false;
								AddItem(chest);

								del = FindItemOnLayer(Layer.Pants);

								if (del != null)
								{
									del.Delete();
								}

								BaseArmor legs = new LeatherLegs();
								legs.Hue = 1175;
								legs.Movable = false;
								AddItem(legs);

								del = FindItemOnLayer(Layer.Neck);

								if (del != null)
								{
									del.Delete();
								}

								BaseArmor gorget = new LeatherGorget();
								gorget.Hue = 1175;
								gorget.Movable = false;
								AddItem(gorget);

								del = FindItemOnLayer(Layer.Gloves);

								if (del != null)
								{
									del.Delete();
								}

								BaseArmor gloves = new LeatherGloves();
								gloves.Hue = 1175;
								gloves.Movable = false;
								AddItem(gloves);

								del = FindItemOnLayer(Layer.Helm);

								if (del != null)
								{
									del.Delete();
								}

								BaseArmor helm = new LeatherCap();
								helm.Hue = 1175;
								helm.Movable = false;
								AddItem(helm);

								del = FindItemOnLayer(Layer.Arms);

								if (del != null)
								{
									del.Delete();
								}

								BaseArmor arms = new LeatherArms();
								arms.Hue = 1175;
								arms.Movable = false;
								AddItem(arms);

								del = FindItemOnLayer(Layer.Shoes);

								if (del != null)
								{
									del.Delete();
								}

								Item sandals = new Sandals();
								sandals.Hue = 1153;
								sandals.Movable = false;
								AddItem(sandals);
							}
						}
					}
					break;
				case 2:
					{
						if (defender is BaseCreature)
						{
							var bc = (BaseCreature)defender;

							if (bc.Controlled != true)
							{
								kpgainmin = 5 + bc.HitsMax / 20;
								kpgainmax = 5 + bc.HitsMax / 10;

								KP += Utility.RandomList(kpgainmin, kpgainmax);
							}
						}

						if (KP >= 100000)
						{
							if (S2)
							{
								S2 = false;

								int hits = HitsMax + 100;
								int va = VirtualArmor + 10;
								int mindamage = DamageMin + 1;
								int maxdamage = DamageMax + 1;

								Warmode = false;

								Say("*" + Name + " achieves a new level of combat*");
								SetDamage(mindamage, maxdamage);
								SetHits(hits);

								VirtualArmor = va;
								Stage = 3;
								Title = "the Golden Knight";

								RawStr += 100;
								RawInt += 20;
								RawDex += 10;

								Item del = FindItemOnLayer(Layer.InnerTorso);

								if (del != null)
								{
									del.Delete();
								}

								BaseArmor chest = new StuddedChest();
								chest.Hue = 1161;
								chest.Movable = false;
								AddItem(chest);

								del = FindItemOnLayer(Layer.Pants);

								if (del != null)
								{
									del.Delete();
								}

								BaseArmor legs = new StuddedLegs();
								legs.Hue = 1161;
								legs.Movable = false;
								AddItem(legs);

								del = FindItemOnLayer(Layer.Neck);

								if (del != null)
								{
									del.Delete();
								}

								BaseArmor gorget = new StuddedGorget();
								gorget.Hue = 1161;
								gorget.Movable = false;
								AddItem(gorget);

								del = FindItemOnLayer(Layer.Gloves);

								if (del != null)
								{
									del.Delete();
								}

								BaseArmor gloves = new StuddedGloves();
								gloves.Hue = 1161;
								gloves.Movable = false;
								AddItem(gloves);

								del = FindItemOnLayer(Layer.Helm);

								if (del != null)
								{
									del.Delete();
								}

								BaseArmor helm = new LeatherCap();
								helm.Hue = 1161;
								helm.Movable = false;
								AddItem(helm);

								del = FindItemOnLayer(Layer.Arms);

								if (del != null)
								{
									del.Delete();
								}

								BaseArmor arms = new StuddedArms();
								arms.Hue = 1161;
								arms.Movable = false;
								AddItem(arms);
							}
						}
					}
					break;
				case 3:
					{
						if (defender is BaseCreature)
						{
							var bc = (BaseCreature)defender;

							if (bc.Controlled != true)
							{
								kpgainmin = 5 + bc.HitsMax / 30;
								kpgainmax = 5 + bc.HitsMax / 20;

								KP += Utility.RandomList(kpgainmin, kpgainmax);
							}
						}

						if (KP >= 500000)
						{
							if (S3)
							{
								S3 = false;

								int hits = HitsMax + 100;
								int va = VirtualArmor + 10;
								int mindamage = DamageMin + 1;
								int maxdamage = DamageMax + 1;

								Warmode = false;

								Say("*" + Name + " achieves a new level of combat*");
								SetDamage(mindamage, maxdamage);
								SetHits(hits);

								VirtualArmor = va;
								Stage = 4;
								Title = "the Blood Knight";

								RawStr += 100;
								RawInt += 120;
								RawDex += 10;

								Item del = FindItemOnLayer(Layer.InnerTorso);

								if (del != null)
								{
									del.Delete();
								}

								BaseArmor chest = new PlateChest();
								chest.Hue = 1157;
								chest.Movable = false;
								AddItem(chest);

								del = FindItemOnLayer(Layer.Pants);

								if (del != null)
								{
									del.Delete();
								}

								BaseArmor legs = new PlateLegs();
								legs.Hue = 1157;
								legs.Movable = false;
								AddItem(legs);

								del = FindItemOnLayer(Layer.Neck);

								if (del != null)
								{
									del.Delete();
								}

								BaseArmor gorget = new PlateGorget();
								gorget.Hue = 1157;
								gorget.Movable = false;
								AddItem(gorget);

								del = FindItemOnLayer(Layer.Gloves);

								if (del != null)
								{
									del.Delete();
								}

								BaseArmor gloves = new PlateGloves();
								gloves.Hue = 1157;
								gloves.Movable = false;
								AddItem(gloves);

								del = FindItemOnLayer(Layer.Helm);

								if (del != null)
								{
									del.Delete();
								}

								BaseArmor helm = new PlateHelm();
								helm.Hue = 1157;
								helm.Movable = false;
								AddItem(helm);

								del = FindItemOnLayer(Layer.Arms);

								if (del != null)
								{
									del.Delete();
								}

								BaseArmor arms = new PlateArms();
								arms.Hue = 1157;
								arms.Movable = false;
								AddItem(arms);

								del = FindItemOnLayer(Layer.Shoes);

								if (del != null)
								{
									del.Delete();
								}
							}
						}
					}
					break;
				case 4:
					{
						if (defender is BaseCreature)
						{
							var bc = (BaseCreature)defender;

							if (bc.Controlled != true)
							{
								kpgainmin = 5 + bc.HitsMax / 50;
								kpgainmax = 5 + bc.HitsMax / 40;

								KP += Utility.RandomList(kpgainmin, kpgainmax);
							}
						}

						if (KP >= 1000000)
						{
							if (S4)
							{
								S4 = false;

								int hits = HitsMax + 100;
								int va = VirtualArmor + 10;
								int mindamage = DamageMin + 5;
								int maxdamage = DamageMax + 5;

								Warmode = false;

								Say("*" + Name + " achieves a new level of combat*");
								SetDamage(mindamage, maxdamage);
								SetHits(hits);

								VirtualArmor = va;
								Stage = 5;
								Title = "the Knight of Destiny";

								RawStr += 100;
								RawInt += 120;
								RawDex += 20;

								Item del = FindItemOnLayer(Layer.InnerTorso);

								if (del != null)
								{
									del.Delete();
								}

								BaseArmor chest = new BoneChest();
								chest.Hue = 1152;
								chest.Movable = false;
								AddItem(chest);

								del = FindItemOnLayer(Layer.Pants);
								if (del != null)
								{
									del.Delete();
								}

								BaseArmor legs = new BoneLegs();
								legs.Hue = 1152;
								legs.Movable = false;
								AddItem(legs);

								del = FindItemOnLayer(Layer.Neck);

								if (del != null)
								{
									del.Delete();
								}

								del = FindItemOnLayer(Layer.Gloves);

								if (del != null)
								{
									del.Delete();
								}

								BaseArmor gloves = new BoneGloves();
								gloves.Hue = 1152;
								gloves.Movable = false;
								AddItem(gloves);

								del = FindItemOnLayer(Layer.Helm);

								if (del != null)
								{
									del.Delete();
								}

								BaseArmor helm = new BoneHelm();
								helm.Hue = 1152;
								helm.Movable = false;
								AddItem(helm);

								del = FindItemOnLayer(Layer.Arms);

								if (del != null)
								{
									del.Delete();
								}

								BaseArmor arms = new BoneArms();
								arms.Hue = 1152;
								arms.Movable = false;
								AddItem(arms);

								del = FindItemOnLayer(Layer.Shoes);

								if (del != null)
								{
									del.Delete();
								}

								Item sandals = new Sandals();
								sandals.Hue = 1152;
								sandals.Movable = false;
								AddItem(sandals);
							}
						}
					}
					break;
				case 5:
					{
						if (defender is BaseCreature)
						{
							var bc = (BaseCreature)defender;

							if (bc.Controlled != true)
							{
								kpgainmin = 5 + bc.HitsMax / 160;
								kpgainmax = 5 + bc.HitsMax / 100;

								KP += Utility.RandomList(kpgainmin, kpgainmax);
							}
						}

						if (KP >= 5000000)
						{
							if (S5)
							{
								S5 = false;

								int hits = HitsMax + 100;
								int va = VirtualArmor + 100;
								int mindamage = DamageMin + 5;
								int maxdamage = DamageMax + 5;

								Warmode = false;
								
								Say("*" + Name + " is Reborn*");
								Say("Master, please lend me some armor");
								SetDamage(mindamage, maxdamage);
								SetHits(hits);
								
								VirtualArmor = va;
								Stage = 6;
								Title = "the Champion";

								RawStr += 100;
								RawInt += 120;
								RawDex += 20;

								Item del = FindItemOnLayer(Layer.InnerTorso);

								if (del != null)
								{
									del.Delete();
								}

								del = FindItemOnLayer(Layer.Pants);

								if (del != null)
								{
									del.Delete();
								}

								del = FindItemOnLayer(Layer.Neck);

								if (del != null)
								{
									del.Delete();
								}

								del = FindItemOnLayer(Layer.Gloves);

								if (del != null)
								{
									del.Delete();
								}

								del = FindItemOnLayer(Layer.Helm);

								if (del != null)
								{
									del.Delete();
								}

								del = FindItemOnLayer(Layer.Arms);

								if (del != null)
								{
									del.Delete();
								}

								del = FindItemOnLayer(Layer.Shoes);

								if (del != null)
								{
									del.Delete();
								}
							}
						}
					}
					break;
				case 6:
					{
						if (defender is BaseCreature)
						{
							var bc = (BaseCreature)defender;

							if (bc.Controlled != true)
							{
								kpgainmin = 5 + bc.HitsMax / 540;
								kpgainmax = 5 + bc.HitsMax / 480;

								KP += Utility.RandomList(kpgainmin, kpgainmax);
							}
						}

						if (KP >= 10000000)
						{
							if (S6)
							{
								S6 = false;

								int hits = HitsMax + 350;
								int va = VirtualArmor + 100;
								int mindamage = DamageMin + 15;
								int maxdamage = DamageMax + 15;

								Warmode = false;
								Say("*" + Name + " is now a Great Champion and loyal friend*");

								if (ControlMaster != null)
								{
									Title = ", Guardian of " + ControlMaster.Name + "";
								}

								SetDamage(mindamage, maxdamage);
								SetHits(hits);
								
								VirtualArmor = va;
								Stage = 7;

								RawStr += 125;
								RawInt += 125;
								RawDex += 35;
							}
						}
					}
					break;
				case 7:
					{
						if (defender is BaseCreature)
						{
							var bc = (BaseCreature)defender;

							if (bc.Controlled != true)
							{
								kpgainmin = 5 + bc.Hits / 740;
								kpgainmax = 5 + bc.Hits / 660;

								KP += Utility.RandomList(kpgainmin, kpgainmax);
							}
						}
					}
					break;
			}

			base.OnGaveMeleeAttack(defender);
		}

		public override bool OnDragDrop(Mobile from, Item dropped)
		{
			// trigger returns true if returnoverride
			if (XmlScript.HasTrigger(this, TriggerName.onDragDrop) &&
				UberScriptTriggers.Trigger(this, from, TriggerName.onDragDrop, dropped))
			{
				return true;
			}

			var player = from as PlayerMobile;

			if (player != null)
			{
				if (Stage > 5)
				{
					if (ControlMaster == from)
					{
						if (dropped is BaseArmor || dropped is BaseClothing || dropped is BaseWeapon)
						{
							if (dropped is BaseWeapon)
							{
								Item check4 = FindItemOnLayer(Layer.OneHanded);
								Item check2 = FindItemOnLayer(Layer.TwoHanded);
								Item check3 = FindItemOnLayer(Layer.FirstValid);

								if (check4 != null)
								{
									check4.Movable = true;
									from.AddToBackpack(check4);
								}
								else if (check2 != null)
								{
									check2.Movable = true;
									from.AddToBackpack(check2);
								}
								else if (check3 != null)
								{
									check3.Movable = true;
									from.AddToBackpack(check3);
								}

								Container pack = Backpack;
								
								if (pack == null)
								{
									AddItem(pack = new Backpack
									{
										Movable = false
									});
								}
								
								pack.DropItem(dropped);
								AddItem(dropped);
								dropped.Movable = false;
								from.SendMessage("You give " + Name + " a new weapon.");
								
								return true;
							}
							
							if (dropped is BaseArmor)
							{
								var ba = (BaseArmor)dropped;
								
								if (!ba.AllowMaleWearer && from.Body.IsMale)
								{
									from.SendMessage("Only Females can wear this.");
								}
								else if (!ba.AllowFemaleWearer && from.Body.IsFemale)
								{
									from.SendMessage("Only Females can wear this.");
								}
								else
								{
									Item check = FindItemOnLayer(dropped.Layer);
									
									if (check != null)
									{
										check.Movable = true;
										from.AddToBackpack(check);

										Container pack = Backpack;
										
										if (pack == null)
										{
											AddItem(pack = new Backpack
											{
												Movable = false
											});
										}

										pack.DropItem(dropped);
										AddItem(dropped);
										dropped.Movable = false;

										from.SendMessage("You give " + Name + " a piece of gear.");
										return true;
									}
									else
									{
										Container pack = Backpack;
										
										if (pack == null)
										{
											AddItem(pack = new Backpack
											{
												Movable = false
											});
										}

										pack.DropItem(dropped);
										AddItem(dropped);
										dropped.Movable = true;

										from.SendMessage("You give " + Name + " a piece of gear.");
										return true;
									}
								}
							}
							else
							{
								Item check = FindItemOnLayer(dropped.Layer);
								
								if (check != null)
								{
									check.Movable = true;
									from.AddToBackpack(check);

									Container pack = Backpack;
									
									if (pack == null)
									{
										AddItem(pack= new Backpack {
											Movable = false
										});
									}
									
									pack.DropItem(dropped);
									AddItem(dropped);
									dropped.Movable = false;

									from.SendMessage("You give " + Name + " a piece of gear.");
									return true;
								}
								else
								{
									Container pack = Backpack;
									
									if (pack == null)
									{
										AddItem(
											pack = new Backpack
											{
												Movable = false
											});
									}

									pack.DropItem(dropped);
									AddItem(dropped);
									dropped.Movable = true;

									from.SendMessage("You give " + Name + " a piece of gear.");
									return true;
								}
							}
						}
						else if (dropped is Arrow || dropped is Bolt)
						{
							Container pack = Backpack;
							
							if (pack == null)
							{
								AddItem(
									pack = new Backpack
									{
										Movable = false
									});
							}

							pack.DropItem(dropped);
							from.SendMessage("You give " + Name + " some arrows.");
						}
						else
						{
							from.SendMessage("This must be a weapon, or a piece of armor or clothing.");
							return false;
						}
					}
					else
					{
						from.SendMessage("You do not own this Mercenary.");
						return false;
					}
				}
				else
				{
					from.SendMessage(Name + " is not ready to use this.");
					return false;
				}
			}

			return base.OnDragDrop(from, dropped);
		}

		private DateTime m_NextBreathe;

		public override void OnActionCombat()
		{
			Mobile combatant = Combatant;

			if (combatant == null || combatant.Deleted || combatant.Map != Map || !InRange(combatant, 12) ||
				!CanBeHarmful(combatant) || !InLOS(combatant))
			{
				return;
			}

			if (DateTime.UtcNow < m_NextBreathe)
			{
				return;
			}

			Breathe(combatant);

			m_NextBreathe = DateTime.UtcNow + TimeSpan.FromSeconds(12.0 + (3.0 * Utility.RandomDouble())); // 12-15 seconds
		}

		public void Breathe(Mobile m)
		{
			DoHarmful(m);

			m_BreatheTimer = new BreatheTimer(m, this, this, TimeSpan.FromSeconds(1.0));
			m_BreatheTimer.Start();
			m_EndBreathe = DateTime.UtcNow + TimeSpan.FromSeconds(1.0);

			Frozen = true;

			if (Stage > 0)
			{
				FixedEffect(0x3728, 10, 15);
				PlaySound(0x2A1);
			}
			else
			{
				PublicOverheadMessage(
					MessageType.Regular,
					SpeechHue,
					true,
					"Please call a GM if you are getting this message, they will fix the breathe, thank you :)",
					false);
			}
		}

		private class BreatheTimer : Timer
		{
			private readonly EvoMerc m_Evo;
			private readonly Mobile m_Mobile;
			private readonly Mobile m_From;

			public BreatheTimer(Mobile m, EvoMerc owner, Mobile from, TimeSpan duration)
				: base(duration)
			{
				m_Evo = owner;
				m_Mobile = m;
				m_From = from;
				Priority = TimerPriority.TwoFiftyMS;
			}

			protected override void OnTick()
			{
				int damagemin = m_Evo.Hits / 20;
				int damagemax = m_Evo.Hits / 25;

				m_Evo.Frozen = false;

				m_Mobile.PlaySound(0x11D);
				m_Mobile.Damage(Utility.RandomMinMax(damagemin, damagemax), m_From);
				Stop();
			}
		}

		public override WeaponAbility GetWeaponAbility()
		{
			switch (Utility.Random(3))
			{
				case 0:
					return WeaponAbility.ArmorIgnore;
				default:
					return WeaponAbility.WhirlwindAttack;
			}
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			writer.Write(1); // version

			writer.Write(S1);
			writer.Write(S2);
			writer.Write(S3);
			writer.Write(S4);
			writer.Write(S5);
			writer.Write(S6);
			writer.Write(KP);
			writer.Write(Stage);
			writer.WriteDeltaTime(m_EndBreathe);
			writer.WriteDeltaTime(EndMercLoyalty);
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			int version = reader.ReadInt();
			switch (version)
			{
				case 1:
					{
						S1 = reader.ReadBool();
						S2 = reader.ReadBool();
						S3 = reader.ReadBool();
						S4 = reader.ReadBool();
						S5 = reader.ReadBool();
						S6 = reader.ReadBool();
						KP = reader.ReadInt();
						Stage = reader.ReadInt();

						m_EndBreathe = reader.ReadDeltaTime();
						m_BreatheTimer = new BreatheTimer(this, this, this, m_EndBreathe - DateTime.UtcNow);
						m_BreatheTimer.Start();

						EndMercLoyalty = reader.ReadDeltaTime();
						m_MercLoyaltyTimer = new MercLoyaltyTimer(this, EndMercLoyalty - DateTime.UtcNow);
						m_MercLoyaltyTimer.Start();

						break;
					}
				case 0:
					{
						TimeSpan durationbreathe = TimeSpan.FromSeconds(1.0);
						TimeSpan durationloyalty = TimeSpan.FromSeconds(5.0);

						m_BreatheTimer = new BreatheTimer(this, this, this, durationbreathe);
						m_BreatheTimer.Start();
						m_EndBreathe = DateTime.UtcNow + durationbreathe;

						m_MercLoyaltyTimer = new MercLoyaltyTimer(this, durationloyalty);
						m_MercLoyaltyTimer.Start();
						EndMercLoyalty = DateTime.UtcNow + durationloyalty;

						break;
					}
			}
		}

		public override bool OnBeforeDeath()
		{
			Mobile from = ControlMaster;

			if (from != null)
			{
				if (Mount != null)
				{
					for (int i = 0; i < Items.Count; ++i)
					{
						Item item = Items[i];

						if (!(item is IMountItem))
						{
							continue;
						}

						IMount mount = ((IMountItem)item).Mount;

						if (mount != null)
						{
							mount.Rider = null;
							
							if (mount is EtherealMount)
							{
								var ethy = (EtherealMount)mount;
								
								if (from.Backpack != null)
								{
									from.Backpack.DropItem(ethy);
								}
							}
						}

						if (Items.IndexOf(item) == -1)
						{
							--i;
						}
					}

					for (int i = 0; i < Items.Count; ++i)
					{
						Item item = Items[i];

						if (item.Layer != Layer.Mount)
						{
							continue;
						}

						if (from.Backpack != null)
						{
							from.Backpack.DropItem(item);
						}

						--i;
					}
				}
			}

			return base.OnBeforeDeath();
		}
	}

	public class StatGump : Gump
	{
		private static string FormatSkill(BaseCreature c, SkillName name)
		{
			Skill skill = c.Skills[name];

			if (skill.Base < 10.0)
			{
				return "<div align=right>---</div>";
			}

			return String.Format("<div align=right>{0:F1}</div>", skill.Base);
		}

		private static string FormatAttributes(int cur, int max)
		{
			if (max == 0)
			{
				return "<div align=right>---</div>";
			}

			return String.Format("<div align=right>{0}/{1}</div>", cur, max);
		}

		private static string FormatStat(int val)
		{
			if (val == 0)
			{
				return "<div align=right>---</div>";
			}

			return String.Format("<div align=right>{0}</div>", val);
		}

		private static string FormatElement(int val)
		{
			if (val <= 0)
			{
				return "<div align=right>---</div>";
			}

			return String.Format("<div align=right>{0}%</div>", val);
		}

		private const int LabelColor = 0x24E5;

		public StatGump(Mobile from, BaseCreature c)
			: base(250, 50)
		{
			AddPage(0);

			AddImage(100, 100, 2080);
			AddImage(118, 137, 2081);
			AddImage(118, 207, 2081);
			AddImage(118, 277, 2081);
			AddImage(118, 347, 2083);

			AddHtml(147, 108, 210, 18, String.Format("<center><i>{0}</i></center>", c.Name), false, false);

			AddButton(240, 77, 2093, 2093, 2, GumpButtonType.Reply, 0);

			AddImage(140, 138, 2091);
			AddImage(140, 335, 2091);

			int pages = c.EraAOS ? 5 : 3;
			int page = 0;

			#region Attributes
			AddPage(++page);

			AddImage(128, 152, 2086);
			AddHtmlLocalized(147, 150, 160, 18, 1049593, 200, false, false); // Attributes

			AddHtmlLocalized(153, 168, 160, 18, 1049578, LabelColor, false, false); // Hits
			AddHtml(280, 168, 75, 18, FormatAttributes(c.Hits, c.HitsMax), false, false);

			AddHtmlLocalized(153, 186, 160, 18, 1049579, LabelColor, false, false); // Stamina
			AddHtml(280, 186, 75, 18, FormatAttributes(c.Stam, c.StamMax), false, false);

			AddHtmlLocalized(153, 204, 160, 18, 1049580, LabelColor, false, false); // Mana
			AddHtml(280, 204, 75, 18, FormatAttributes(c.Mana, c.ManaMax), false, false);

			AddHtmlLocalized(153, 222, 160, 18, 1028335, LabelColor, false, false); // Strength
			AddHtml(320, 222, 35, 18, FormatStat(c.Str), false, false);

			AddHtmlLocalized(153, 240, 160, 18, 3000113, LabelColor, false, false); // Dexterity
			AddHtml(320, 240, 35, 18, FormatStat(c.Dex), false, false);

			AddHtmlLocalized(153, 258, 160, 18, 3000112, LabelColor, false, false); // Intelligence
			AddHtml(320, 258, 35, 18, FormatStat(c.Int), false, false);

			AddImage(128, 278, 2086);
			AddHtmlLocalized(147, 276, 160, 18, 3001016, 200, false, false); // Miscellaneous

			AddHtmlLocalized(153, 294, 160, 18, 1049581, LabelColor, false, false); // Armor Rating
			AddHtml(320, 294, 35, 18, FormatStat(c.VirtualArmor), false, false);

			AddButton(340, 358, 5601, 5605, 0, GumpButtonType.Page, page + 1);
			AddButton(317, 358, 5603, 5607, 0, GumpButtonType.Page, pages);
			#endregion

			#region Skills
			AddPage(++page);

			AddImage(128, 152, 2086);
			AddHtmlLocalized(147, 150, 160, 18, 3001030, 200, false, false); // Combat Ratings

			AddHtmlLocalized(153, 168, 160, 18, 1044103, LabelColor, false, false); // Wrestling
			AddHtml(320, 168, 35, 18, FormatSkill(c, SkillName.Wrestling), false, false);

			AddHtmlLocalized(153, 186, 160, 18, 1044087, LabelColor, false, false); // Tactics
			AddHtml(320, 186, 35, 18, FormatSkill(c, SkillName.Tactics), false, false);

			AddHtmlLocalized(153, 204, 160, 18, 1044086, LabelColor, false, false); // Magic Resistance
			AddHtml(320, 204, 35, 18, FormatSkill(c, SkillName.MagicResist), false, false);

			AddHtmlLocalized(153, 222, 160, 18, 1044061, LabelColor, false, false); // Anatomy
			AddHtml(320, 222, 35, 18, FormatSkill(c, SkillName.Anatomy), false, false);

			AddHtmlLocalized(153, 240, 160, 18, 1044090, LabelColor, false, false); // Poisoning
			AddHtml(320, 240, 35, 18, FormatSkill(c, SkillName.Poisoning), false, false);

			AddImage(128, 260, 2086);
			AddHtmlLocalized(147, 258, 160, 18, 3001032, 200, false, false); // Lore & Knowledge

			AddHtmlLocalized(153, 276, 160, 18, 1044085, LabelColor, false, false); // Magery
			AddHtml(320, 276, 35, 18, FormatSkill(c, SkillName.Magery), false, false);

			AddHtmlLocalized(153, 294, 160, 18, 1044076, LabelColor, false, false); // Evaluating Intelligence
			AddHtml(320, 294, 35, 18, FormatSkill(c, SkillName.EvalInt), false, false);

			AddHtmlLocalized(153, 312, 160, 18, 1044106, LabelColor, false, false); // Meditation
			AddHtml(320, 312, 35, 18, FormatSkill(c, SkillName.Meditation), false, false);

			AddButton(340, 358, 5601, 5605, 0, GumpButtonType.Page, page + 1);
			AddButton(317, 358, 5603, 5607, 0, GumpButtonType.Page, page - 1);
			#endregion

			#region Misc
			AddPage(++page);

			AddImage(128, 152, 2086);
			AddHtmlLocalized(147, 150, 160, 18, 1049563, 200, false, false); // Preferred Foods

			int foodPref = 3000340;

			//if ( (c.FavoriteFood & FoodType.Fruits) != 0 )
			//foodPref = 1049565; // Fruits and Vegetables
			if ((c.FavoriteFood & FoodType.GrainsAndHay) != 0)
			{
				foodPref = 1049566; // Grains and Hay
			}
			else if ((c.FavoriteFood & FoodType.Fish) != 0)
			{
				foodPref = 1049568; // Fish
			}
			else if ((c.FavoriteFood & FoodType.Meat) != 0)
			{
				foodPref = 1049564; // Meat
			}

			AddHtmlLocalized(153, 168, 160, 18, foodPref, LabelColor, false, false);

			AddImage(128, 188, 2086);
			AddHtmlLocalized(147, 186, 160, 18, 1049569, 200, false, false); // Pack Instincts

			int packInstinct = 3000340;

			if ((c.PackInstinct & PackInstinct.Canine) != 0)
			{
				packInstinct = 1049570; // Canine
			}
			else if ((c.PackInstinct & PackInstinct.Ostard) != 0)
			{
				packInstinct = 1049571; // Ostard
			}
			else if ((c.PackInstinct & PackInstinct.Feline) != 0)
			{
				packInstinct = 1049572; // Feline
			}
			else if ((c.PackInstinct & PackInstinct.Arachnid) != 0)
			{
				packInstinct = 1049573; // Arachnid
			}
			else if ((c.PackInstinct & PackInstinct.Daemon) != 0)
			{
				packInstinct = 1049574; // Daemon
			}
			else if ((c.PackInstinct & PackInstinct.Bear) != 0)
			{
				packInstinct = 1049575; // Bear
			}
			else if ((c.PackInstinct & PackInstinct.Equine) != 0)
			{
				packInstinct = 1049576; // Equine
			}
			else if ((c.PackInstinct & PackInstinct.Bull) != 0)
			{
				packInstinct = 1049577; // Bull
			}

			AddHtmlLocalized(153, 204, 160, 18, packInstinct, LabelColor, false, false);

			if (!from.EraAOS)
			{
				AddImage(128, 224, 2086);
				AddHtmlLocalized(147, 222, 160, 18, 1049594, 200, false, false); // Loyalty Rating

				//AddHtmlLocalized( 153, 240, 160, 18, (!c.Controlled || c.Loyalty == PetLoyalty.MaxLoyalty ) ? 1061643 : 1049594 + (int)c.Loyalty, LabelColor, false, false );
			}

			AddButton(340, 358, 5601, 5605, 0, GumpButtonType.Page, 1);
			AddButton(317, 358, 5603, 5607, 0, GumpButtonType.Page, page - 1);
			#endregion
		}
	}

	public class MercLoyaltyTimer : Timer
	{
		private readonly EvoMerc ed;

		public MercLoyaltyTimer(EvoMerc owner, TimeSpan duration)
			: base(duration)
		{
			Priority = TimerPriority.OneSecond;
			ed = owner;
		}

		protected override void OnTick()
		{
			/*
			foreach (NetState state in NetState.Instances)
			{
				if (state.Mobile == null)
				{
					continue;
				}

				
				Mobile owner = state.Mobile;

				if (ed.ControlMaster == owner) 
					ed.Loyalty = Loyalty.WonderfullyHappy; 
				
			}
			*/

			var lt = new MercLoyaltyTimer(ed, TimeSpan.FromSeconds(5.0));
			lt.Start();
			ed.EndMercLoyalty = DateTime.UtcNow + TimeSpan.FromSeconds(5.0);

			Stop();
		}
	}

	public class EvoMercBuyInfo
	{
		public int Title { get; private set; }
		public string TitleString { get; private set; }
		public int Price { get; private set; }
		public Layer Layer { get; private set; }
		public Type GumpType { get; private set; }
		public object[] GumpArgs { get; private set; }

		public EvoMercBuyInfo(int title, int price, Layer layer, Type gumpType, object[] args)
		{
			Title = title;
			Price = price;
			Layer = layer;
			GumpType = gumpType;
			GumpArgs = args;
		}

		public EvoMercBuyInfo(string title, int price, Layer layer, Type gumpType, object[] args)
		{
			TitleString = title;
			Price = price;
			Layer = layer;
			GumpType = gumpType;
			GumpArgs = args;
		}
	}

	public class EvoMercBuyGump : Gump
	{
		private readonly Mobile m_From;
		private readonly EvoMerc m_Vendor;
		private readonly EvoMercBuyInfo[] m_SellList;

		public EvoMercBuyGump(Mobile from, EvoMerc vendor, EvoMercBuyInfo[] sellList)
			: base(50, 50)
		{
			m_From = from;
			m_Vendor = vendor;
			m_SellList = sellList;

			from.CloseGump(typeof(EvoMercBuyGump));
			from.CloseGump(typeof(ChangeHairHuGump));
			from.CloseGump(typeof(ChangeHairstylGump));

			bool isFemale = m_Vendor.Body.IsFemale;

			int balance = Banker.GetBalance(from, m_Vendor.TypeOfCurrency);
			int canAfford = sellList.Count(t => balance >= t.Price && (t.Layer != Layer.FacialHair || !isFemale));

			AddPage(0);

			AddBackground(50, 10, 450, 100 + (canAfford * 25), 2600);

			AddHtmlLocalized(100, 40, 350, 20, 1018356, false, false); // Choose your hairstyle change:

			int index = 0;

			for (int i = 0; i < sellList.Length; ++i)
			{
				if (balance < sellList[i].Price || (sellList[i].Layer == Layer.FacialHair && isFemale))
				{
					continue;
				}

				if (sellList[i].TitleString != null)
				{
					AddHtml(140, 75 + (index * 25), 300, 20, sellList[i].TitleString, false, false);
				}
				else
				{
					AddHtmlLocalized(140, 75 + (index * 25), 300, 20, sellList[i].Title, false, false);
				}

				AddButton(100, 75 + (index++ * 25), 4005, 4007, 1 + i, GumpButtonType.Reply, 0);
			}
		}

		public override void OnResponse(NetState sender, RelayInfo info)
		{
			int index = info.ButtonID - 1;

			if (index < 0 || index >= m_SellList.Length)
			{
				return;
			}

			EvoMercBuyInfo buyInfo = m_SellList[index];

			int balance = Banker.GetBalance(m_From, m_Vendor.TypeOfCurrency);

			bool isFemale = (m_Vendor.Female || m_Vendor.Body.IsFemale);

			if (buyInfo.Layer == Layer.FacialHair && isFemale)
			{
				// You cannot place facial hair on a woman!
				m_Vendor.PrivateOverheadMessage(MessageType.Regular, 0x3B2, 1010639, m_From.NetState);
			}
			else if (balance >= buyInfo.Price)
			{
				try
				{
					object[] origArgs = buyInfo.GumpArgs;
					var args = new object[origArgs.Length];

					for (int i = 0; i < args.Length; ++i)
					{
						if (origArgs[i] == EvoMerc.Price)
						{
							args[i] = m_SellList[index].Price;
						}
						else if (origArgs[i] == EvoMerc.From)
						{
							args[i] = m_From;
						}
						else if (origArgs[i] == EvoMerc.Vendor)
						{
							args[i] = m_Vendor;
						}
						else
						{
							args[i] = origArgs[i];
						}
					}

					var g = Activator.CreateInstance(buyInfo.GumpType, args) as Gump;

					m_From.SendGump(g);
				}
				catch
				{ }
			}
			else
			{
				// You cannot afford my services for that style.
				m_Vendor.PrivateOverheadMessage(MessageType.Regular, 0x3B2, 1042293, m_From.NetState);
			}
		}
	}

	public class ChangeHairHueEnt
	{
		public string Name { get; private set; }
		public int[] Hues { get; private set; }

		public ChangeHairHueEnt(string name, int[] hues)
		{
			Name = name;
			Hues = hues;
		}

		public ChangeHairHueEnt(string name, int start, int count)
		{
			Name = name;

			Hues = new int[count];

			for (int i = 0; i < count; ++i)
			{
				Hues[i] = start + i;
			}
		}

		public static readonly ChangeHairHueEnt[] BrightEntries = new[]
		{
			new ChangeHairHueEnt("*****", 12, 10), new ChangeHairHueEnt("*****", 32, 5), new ChangeHairHueEnt("*****", 38, 8),
			new ChangeHairHueEnt("*****", 54, 3), new ChangeHairHueEnt("*****", 62, 10), new ChangeHairHueEnt("*****", 81, 2),
			new ChangeHairHueEnt("*****", 89, 2), new ChangeHairHueEnt("*****", 1153, 2)
		};

		public static readonly ChangeHairHueEnt[] RegularEntries = new[]
		{
			new ChangeHairHueEnt("*****", 1602, 26), new ChangeHairHueEnt("*****", 1628, 27),
			new ChangeHairHueEnt("*****", 1502, 32), new ChangeHairHueEnt("*****", 1302, 32),
			new ChangeHairHueEnt("*****", 1402, 32), new ChangeHairHueEnt("*****", 1202, 24),
			new ChangeHairHueEnt("*****", 2402, 29), new ChangeHairHueEnt("*****", 2213, 6),
			new ChangeHairHueEnt("*****", 1102, 8), new ChangeHairHueEnt("*****", 1110, 8),
			new ChangeHairHueEnt("*****", 1118, 16), new ChangeHairHueEnt("*****", 1134, 16)
		};
	}

	public class ChangeHairHuGump : Gump
	{
		private readonly Mobile m_From;
		private readonly EvoMerc m_Vendor;
		private readonly int m_Price;
		private readonly Layer[] m_Layers;
		private readonly ChangeHairHueEnt[] m_Entries;

		public ChangeHairHuGump(Mobile from, EvoMerc vendor, int price, Layer[] layers, ChangeHairHueEnt[] entries)
			: base(50, 50)
		{
			m_From = from;
			m_Vendor = vendor;
			m_Price = price;
			m_Layers = layers;
			m_Entries = entries;

			from.CloseGump(typeof(EvoMercBuyGump));
			from.CloseGump(typeof(ChangeHairHuGump));
			from.CloseGump(typeof(ChangeHairstylGump));

			AddPage(0);

			AddBackground(100, 10, 350, 370, 2600);
			AddBackground(120, 54, 110, 270, 5100);

			AddHtmlLocalized(155, 25, 240, 30, 1011013, false, false); // <center>Hair Color Selection Menu</center>

			AddHtmlLocalized(150, 330, 220, 35, 1011014, false, false); // Dye my hair this color!
			AddButton(380, 330, 4005, 4007, 1, GumpButtonType.Reply, 0);

			for (int i = 0; i < entries.Length; ++i)
			{
				ChangeHairHueEnt entry = entries[i];

				AddLabel(130, 59 + (i * 22), entry.Hues[0] - 1, entry.Name);
				AddButton(207, 60 + (i * 22), 5224, 5224, 0, GumpButtonType.Page, 1 + i);
			}

			for (int i = 0; i < entries.Length; ++i)
			{
				ChangeHairHueEnt entry = entries[i];
				int[] hues = entry.Hues;
				string name = entry.Name;

				AddPage(1 + i);

				for (int j = 0; j < hues.Length; ++j)
				{
					AddLabel(278 + ((j / 16) * 80), 52 + ((j % 16) * 17), hues[j] - 1, name);
					AddRadio(260 + ((j / 16) * 80), 52 + ((j % 16) * 17), 210, 211, false, (j * entries.Length) + i);
				}
			}
		}

		public override void OnResponse(NetState sender, RelayInfo info)
		{
			if (info.ButtonID == 1)
			{
				int[] switches = info.Switches;

				if (switches.Length > 0)
				{
					int index = switches[0] % m_Entries.Length;
					int offset = switches[0] / m_Entries.Length;

					if (index >= 0 && index < m_Entries.Length)
					{
						if (offset >= 0 && offset < m_Entries[index].Hues.Length)
						{
							int hue = m_Entries[index].Hues[offset];

							bool hasConsumed = false;

							foreach (Item item in m_Layers.Select(l => m_Vendor.FindItemOnLayer(l)).Where(item => item != null))
							{
								if (!hasConsumed)
								{
									if (!Banker.Withdraw(m_From, m_Vendor.TypeOfCurrency, m_Price))
									{
										m_Vendor.PrivateOverheadMessage(MessageType.Regular, 0x3B2, 1042293, m_From.NetState);
										// You cannot afford my services for that style.
										return;
									}

									hasConsumed = true;
								}

								item.Hue = hue;
							}

							if (!hasConsumed)
							{
								// You have no hair to dye and you cannot use this.
								m_Vendor.PrivateOverheadMessage(MessageType.Regular, 0x3B2, 502623, m_From.NetState);
							}
						}
					}
				}
				else
				{
					// You decide not to change your hairstyle.
					m_Vendor.PrivateOverheadMessage(MessageType.Regular, 0x3B2, 1013009, m_From.NetState);
				}
			}
			else
			{
				// You decide not to change your hairstyle.
				m_Vendor.PrivateOverheadMessage(MessageType.Regular, 0x3B2, 1013009, m_From.NetState);
			}
		}
	}

	public class ChangeHairstyleEnt
	{
		public Type ItemType { get; private set; }
		public int GumpID { get; private set; }
		public int X { get; private set; }
		public int Y { get; private set; }

		public ChangeHairstyleEnt(int gumpID, int x, int y, Type itemType)
		{
			GumpID = gumpID;
			X = x;
			Y = y;
			ItemType = itemType;
		}

		public static readonly ChangeHairstyleEnt[] HairEntries = new[]
		{
			new ChangeHairstyleEnt(50700, 70 - 137, 20 - 60, typeof(ShortHair)),
			new ChangeHairstyleEnt(60710, 193 - 260, 18 - 60, typeof(PageboyHair)),
			new ChangeHairstyleEnt(50703, 316 - 383, 25 - 60, typeof(Mohawk)),
			new ChangeHairstyleEnt(60708, 70 - 137, 75 - 125, typeof(LongHair)),
			new ChangeHairstyleEnt(60900, 193 - 260, 85 - 125, typeof(Afro)),
			new ChangeHairstyleEnt(60713, 320 - 383, 85 - 125, typeof(KrisnaHair)),
			new ChangeHairstyleEnt(60702, 70 - 137, 140 - 190, typeof(PonyTail)),
			new ChangeHairstyleEnt(60707, 193 - 260, 140 - 190, typeof(TwoPigTails)),
			new ChangeHairstyleEnt(60901, 315 - 383, 150 - 190, typeof(ReceedingHair)), new ChangeHairstyleEnt(0, 0, 0, null)
		};

		public static readonly ChangeHairstyleEnt[] BeardEntries = new[]
		{
			new ChangeHairstyleEnt(50800, 120 - 187, 30 - 80, typeof(Goatee)),
			new ChangeHairstyleEnt(50904, 243 - 310, 33 - 80, typeof(MediumShortBeard)),
			new ChangeHairstyleEnt(50906, 120 - 187, 100 - 150, typeof(Vandyke)),
			new ChangeHairstyleEnt(50801, 243 - 310, 95 - 150, typeof(LongBeard)),
			new ChangeHairstyleEnt(50802, 120 - 187, 173 - 220, typeof(ShortBeard)),
			new ChangeHairstyleEnt(50905, 243 - 310, 165 - 220, typeof(MediumLongBeard)),
			new ChangeHairstyleEnt(50808, 120 - 187, 242 - 290, typeof(Mustache)), new ChangeHairstyleEnt(0, 0, 0, null)
		};
	}

	public class ChangeHairstylGump : Gump
	{
		private readonly Mobile m_From;
		private readonly EvoMerc m_Vendor;
		private readonly int m_Price;
		private readonly Layer m_Layer;
		private readonly ChangeHairstyleEnt[] m_Entries;

		public ChangeHairstylGump(Mobile from, EvoMerc vendor, int price, Layer layer, ChangeHairstyleEnt[] entries)
			: base(50, 50)
		{
			m_From = from;
			m_Vendor = vendor;
			m_Price = price;
			m_Layer = layer;
			m_Entries = entries;

			from.CloseGump(typeof(EvoMercBuyGump));
			from.CloseGump(typeof(ChangeHairHuGump));
			from.CloseGump(typeof(ChangeHairstylGump));

			const int offsetWidth = 123;

			int tableWidth = layer == Layer.Hair ? 3 : 2;
			int tableHeight = (entries.Length + tableWidth - (layer == Layer.Hair ? 2 : 1)) / tableWidth;
			int offsetHeight = layer == Layer.Hair ? 65 : 70;

			AddPage(0);

			AddBackground(0, 0, 81 + (tableWidth * offsetWidth), 105 + (tableHeight * offsetHeight), 2600);

			AddButton(45, 45 + (tableHeight * offsetHeight), 4005, 4007, 1, GumpButtonType.Reply, 0);
			AddHtmlLocalized(77, 45 + (tableHeight * offsetHeight), 90, 35, 1006044, false, false); // Ok

			AddButton(
				81 + (tableWidth * offsetWidth) - 180, 45 + (tableHeight * offsetHeight), 4005, 4007, 0, GumpButtonType.Reply, 0);
			AddHtmlLocalized(
				81 + (tableWidth * offsetWidth) - 148, 45 + (tableHeight * offsetHeight), 90, 35, 1006045, false, false); // Cancel

			if (layer == Layer.Hair)
			{
				AddHtmlLocalized(50, 15, 350, 20, 1018353, false, false); // <center>New Hairstyle</center>
			}
			else
			{
				AddHtmlLocalized(55, 15, 200, 20, 1018354, false, false); // <center>New Beard</center>
			}

			for (int i = 0; i < entries.Length; ++i)
			{
				int xTable = i % tableWidth;
				int yTable = i / tableWidth;

				if (entries[i].GumpID != 0)
				{
					AddRadio(40 + (xTable * offsetWidth), 70 + (yTable * offsetHeight), 208, 209, false, i);
					AddBackground(87 + (xTable * offsetWidth), 50 + (yTable * offsetHeight), 50, 50, 2620);
					AddImage(
						87 + (xTable * offsetWidth) + entries[i].X, 50 + (yTable * offsetHeight) + entries[i].Y, entries[i].GumpID);
				}
				else if (layer == Layer.Hair)
				{
					AddRadio(40 + ((xTable + 1) * offsetWidth), 240, 208, 209, false, i);
					AddHtmlLocalized(60 + ((xTable + 1) * offsetWidth), 240, 85, 35, 1011064, false, false); // Bald
				}
				else
				{
					AddRadio(40 + (xTable * offsetWidth), 70 + (yTable * offsetHeight), 208, 209, false, i);
					AddHtmlLocalized(60 + (xTable * offsetWidth), 70 + (yTable * offsetHeight), 85, 35, 1011064, false, false); // Bald
				}
			}
		}

		public override void OnResponse(NetState sender, RelayInfo info)
		{
			if (m_Layer == Layer.FacialHair && (m_From.Female || m_From.Body.IsFemale))
			{
				return;
			}

			if (info.ButtonID == 1)
			{
				int[] switches = info.Switches;

				if (switches.Length > 0)
				{
					int index = switches[0];

					if (index >= 0 && index < m_Entries.Length)
					{
						ChangeHairstyleEnt entry = m_Entries[index];

						if (m_From is PlayerMobile)
						{
							((PlayerMobile)m_From).SetHairMods(-1, -1);
						}

						Item hair = m_Vendor.FindItemOnLayer(m_Layer);

						if (entry.ItemType == null)
						{
							if (hair == null)
							{
								return;
							}

							if (Banker.Withdraw(m_From, m_Vendor.TypeOfCurrency, m_Price))
							{
								hair.Delete();
							}
							else
							{
								// You cannot afford my services for that style.
								m_Vendor.PrivateOverheadMessage(MessageType.Regular, 0x3B2, 1042293, m_From.NetState);
							}
						}
						else
						{
							if (hair != null && hair.GetType() == entry.ItemType)
							{
								return;
							}

							Item newHair = null;

							try
							{
								newHair = Activator.CreateInstance(entry.ItemType, null) as Item;
							}
							catch
							{ }

							if (newHair == null)
							{
								return;
							}

							if (Banker.Withdraw(m_From, m_Vendor.TypeOfCurrency, m_Price))
							{
								if (hair != null)
								{
									newHair.Hue = hair.Hue;
									hair.Delete();
								}

								m_Vendor.AddItem(newHair);
							}
							else
							{
								newHair.Delete();

								// You cannot afford my services for that style.
								m_Vendor.PrivateOverheadMessage(MessageType.Regular, 0x3B2, 1042293, m_From.NetState);
							}
						}
					}
				}
				else
				{
					// You decide not to change your hairstyle.
					m_Vendor.PrivateOverheadMessage(MessageType.Regular, 0x3B2, 1013009, m_From.NetState);
				}
			}
			else
			{
				// You decide not to change your hairstyle.
				m_Vendor.PrivateOverheadMessage(MessageType.Regular, 0x3B2, 1013009, m_From.NetState);
			}
		}
	}
}