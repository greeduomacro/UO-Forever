#region References
using System.Collections.Generic;

using Server.ContextMenus;

using VitaNex.FX;
using VitaNex.Network;
#endregion

namespace Server.Mobiles
{
	[CorpseName("a pool of ectoplasm")]
	public class DamnedSoul : BaseCreature
	{
		public override string DefaultName { get { return "damned soul"; } }

		public override int DefaultBloodHue { get { return -1; } }

		public override bool AlwaysMurderer { get { return true; } }
		public override bool BleedImmune { get { return true; } }

		public override Poison PoisonImmune { get { return Poison.Lethal; } }

		[Constructable]
		public DamnedSoul()
			: base(AIType.AI_Mage, FightMode.Evil, 10, 1, 0.2, 0.4)
		{
			Body = 970;
			SolidHueOverride = Hue = 0x4001;
			BaseSoundID = 0x482;

            Alignment = Alignment.Undead;

			SetStr(350, 400);
			SetDex(100);
			SetInt(30, 50);

			SetHits(200, 250);

			SetDamage(15, 25);

			SetSkill(SkillName.Magery, 50.0);
			SetSkill(SkillName.MagicResist, 25.0);
			SetSkill(SkillName.EvalInt, 25.0);

			Fame = 1000;
			Karma = -1000;

			VirtualArmor = 20;

			EquipItem(
				new Spellbook46
				{
					Movable = false
				});
		}

		public DamnedSoul(Serial serial)
			: base(serial)
		{ }

		public override void DisplayPaperdollTo(Mobile to)
		{ }

		public override void GetContextMenuEntries(Mobile from, List<ContextMenuEntry> list)
		{
			base.GetContextMenuEntries(from, list);

			list.RemoveAll(e => e is PaperdollEntry);
		}

		public override void GenerateLoot()
		{
			AddLoot(LootPack.Poor);
			AddLoot(LootPack.LowScrolls);
		}

		public override bool OnBeforeDeath()
		{
			if (base.OnBeforeDeath())
			{
				new EffectInfo(Location.Clone3D(0, 0, -5), Map, 14120, 0, 10, 10, EffectRender.Darken).Send();

				Body = 51;

				return true;
			}

			return false;
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			writer.SetVersion(0);
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			reader.GetVersion();
		}
	}
}