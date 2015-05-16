#region References

using Server.Ethics.Evil;
using Server.Ethics.Hero;
using Server.Guilds;
#endregion

namespace Server.Items
{
	public class ChaosShield : BaseShield
	{
		public override int InitMinHits { get { return 100; } }
		public override int InitMaxHits { get { return 125; } }

		public override int ArmorBase { get { return 32; } }

		public override ArmorMeditationAllowance DefMedAllowance { get { return ArmorMeditationAllowance.All; } }

		[Constructable]
		public ChaosShield()
			: base(0x1BC3)
		{
			Weight = 5.0;
		}

		public override bool AllowEquippedCast(Mobile from)
		{
			return false;
		}

		public ChaosShield(Serial serial)
			: base(serial)
		{ }

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			writer.Write(0); //version
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			reader.ReadInt();
		}

		public override bool OnEquip(Mobile from)
		{
			return Validate(from) && base.OnEquip(from);
		}

		public override void OnSingleClick(Mobile from)
		{
			if (Validate(Parent as Mobile))
			{
				base.OnSingleClick(from);
			}
			else
			{
				LabelToExpansion(from);
			}
		}

		public virtual bool Validate(Mobile m)
		{
			if (EraAOS || m == null || !m.Player || m.AccessLevel != AccessLevel.Player)
			{
				return true;
			}

			var g = m.Guild as Guild;

            var ethic = Ethics.Player.Find(m);

            if (g == null && ethic == null || g != null && g.Type != GuildType.Chaos && ethic == null || ethic != null && !(ethic.Ethic is EvilEthic))
			{
				m.FixedEffect(0x3728, 10, 13);
				Delete();

				return false;
			}

			return true;
		}
	}
}