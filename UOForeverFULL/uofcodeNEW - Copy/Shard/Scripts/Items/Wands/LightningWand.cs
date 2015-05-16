#region References
using Server.Spells.Fourth;
#endregion

namespace Server.Items
{
	public class LightningWand : BaseWand
	{
		[Constructable]
		public LightningWand()
			: base(WandEffect.Lightning, 5, 20)
		{ }

		public LightningWand(Serial serial)
			: base(serial)
		{ }

		protected override void OnExpansionChanged(Expansion old)
		{
			base.OnExpansionChanged(old);

			if (EraML)
			{
				Charges = Utility.RandomMinMax(5, 109);
			}
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

		public override void OnWandUse(Mobile from)
		{
			Cast(new LightningSpell(from, this));
		}
	}
}