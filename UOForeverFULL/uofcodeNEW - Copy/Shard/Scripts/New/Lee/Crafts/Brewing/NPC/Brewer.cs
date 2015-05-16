using System.Collections.Generic;
using Server.Items;

namespace Server.Mobiles
{
	public class Brewer : BaseVendor
	{
		private readonly List<SBInfo> _SBInfos = new List<SBInfo>();

		protected override List<SBInfo> SBInfos { get { return _SBInfos; } }
	
		[CommandProperty(AccessLevel.GameMaster)]
		public bool VisitedMoonglow { get; private set; }
		
		public override VendorShoeType ShoeType { get { return Utility.RandomBool() ? VendorShoeType.ThighBoots : VendorShoeType.Boots; } }

		[Constructable]
		public Brewer()
			: base("the brewer")
		{
			VisitedMoonglow = false;
		}

		public Brewer(Serial serial)
			: base(serial)
		{ }

		public override void InitSBInfo()
		{
			_SBInfos.Add(new SBBrewer(VisitedMoonglow));
		}

		public override void InitOutfit()
		{
			base.InitOutfit();

			AddItem(new HalfApron(RandomBrightHue()));
		}

		protected override void OnLocationChange(Point3D oldLocation)
		{
			base.OnLocationChange(oldLocation);

			if (Region != null && Insensitive.Contains(Region.Name, "Moonglow"))
			{
				OnVisitedMoonglow();
			}
		}

		protected virtual void OnVisitedMoonglow()
		{
			VisitedMoonglow = true;

			LoadSBInfo();
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			int version = writer.SetVersion(0);

			switch (version)
			{
				case 0:
					writer.Write(VisitedMoonglow);
					break;
			}
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			int version = reader.GetVersion();

			switch (version)
			{
				case 0:
					VisitedMoonglow = reader.ReadBool();
					break;
			}
		}
	}
}