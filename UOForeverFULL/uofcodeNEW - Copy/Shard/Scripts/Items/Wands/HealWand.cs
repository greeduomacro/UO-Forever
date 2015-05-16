#region References
using Server.Spells.First;
#endregion

namespace Server.Items
{
	public class HealWand : BaseWand
	{
		[Constructable]
		public HealWand()
			: base(WandEffect.Healing, 10, 25)
		{ }

		public HealWand(Serial serial)
			: base(serial)
		{ }

		protected override void OnExpansionChanged(Expansion old)
		{
			base.OnExpansionChanged(old);

			if (EraML)
			{
				Charges = Utility.RandomMinMax(10, 109);
			}
		}

		public override void OnWandUse(Mobile from)
		{
			Cast(new HealSpell(from, this));
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			writer.Write(0); // version
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			reader.ReadInt();
		}
	}
}