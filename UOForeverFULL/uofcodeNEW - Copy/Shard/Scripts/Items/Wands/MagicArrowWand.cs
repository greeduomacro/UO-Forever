#region References
using Server.Spells.First;
#endregion

namespace Server.Items
{
	public class MagicArrowWand : BaseWand
	{
		[Constructable]
		public MagicArrowWand()
			: base(WandEffect.MagicArrow, 5, 30)
		{ }

		public MagicArrowWand(Serial serial)
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

		public override void OnWandUse(Mobile from)
		{
			Cast(new MagicArrowSpell(from, this));
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