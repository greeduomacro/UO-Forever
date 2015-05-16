namespace Server.Items
{
	public class NecromancerSpellbook : Spellbook
	{
		public override SpellbookType SpellbookType { get { return SpellbookType.Necromancer; } }
		public override int BookOffset { get { return 100; } }
		public override int BookCount { get { return EraSE ? 17 : 16; } }

		[Constructable]
		public NecromancerSpellbook()
			: this((ulong)0)
		{ }

		[Constructable]
		public NecromancerSpellbook(ulong content)
			: base(content, 0x2253)
		{
			Layer = Layer.OneHanded;
		}

		public NecromancerSpellbook(Serial serial)
			: base(serial)
		{ }

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			writer.Write(1); // version
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			reader.ReadInt();

			Layer = Layer.OneHanded;
		}
	}
}