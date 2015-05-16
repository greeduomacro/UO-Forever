#region References
using Server.Spells.Second;
#endregion

namespace Server.Items
{
	public class HarmWand : BaseWand
	{
		[Constructable]
		public HarmWand()
			: base(WandEffect.Harming, 5, 30)
		{ }

		public HarmWand(Serial serial)
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
			Cast(new HarmSpell(from, this));
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