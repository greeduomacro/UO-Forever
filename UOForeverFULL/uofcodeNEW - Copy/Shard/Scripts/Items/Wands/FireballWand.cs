#region References
using Server.Spells.Third;
#endregion

namespace Server.Items
{
	public class FireballWand : BaseWand
	{
		[Constructable]
		public FireballWand()
			: base(WandEffect.Fireball, 5, 15)
		{ }

		public FireballWand(Serial serial)
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
			Cast(new FireballSpell(from, this));
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