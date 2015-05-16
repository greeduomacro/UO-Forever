#region References
using Server.Spells.Fourth;
#endregion

namespace Server.Items
{
	public class GreaterHealWand : BaseWand
	{
		[Constructable]
		public GreaterHealWand()
			: base(WandEffect.GreaterHealing, 1, 5)
		{ }

		public GreaterHealWand(Serial serial)
			: base(serial)
		{ }

		protected override void OnExpansionChanged(Expansion old)
		{
			base.OnExpansionChanged(old);

			if (EraML)
			{
				Charges = Utility.RandomMinMax(1, 109);
			}
		}

		public override void OnWandUse(Mobile from)
		{
			Cast(new GreaterHealSpell(from, this));
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