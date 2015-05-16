#region References
using Server.Engines.Craft;

using VitaNex.SuperCrafts;
#endregion

namespace Server.Items
{
	public class BrewingKit : BaseTool
	{
		public override CraftSystem CraftSystem { get { return SuperCraftSystem.Resolve<Brewing>(); } }

		[Constructable]
		public BrewingKit()
			: this(50)
		{ }

		[Constructable]
		public BrewingKit(int uses)
			: base(uses, 6464)
		{
			Name = "Brewing Kit";
			Weight = 1.0;
		}

		public BrewingKit(Serial serial)
			: base(serial)
		{ }

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