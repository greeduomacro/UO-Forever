using VitaNex;

namespace Server.Engines.Portals
{
	public sealed class PortalsOptions : CoreServiceOptions
	{
        [CommandProperty(Portals.Access, true)]
        public int NumberofPortals { get; set; }

        [CommandProperty(Portals.Access, true)]
        public int PortalCloseTime { get; set; }

		public PortalsOptions()
            : base(typeof(Portals))
		{
			EnsureDefaults();
		}

        public PortalsOptions(GenericReader reader)
			: base(reader)
		{ }

		public void EnsureDefaults()
		{
		    NumberofPortals = 3;
		    PortalCloseTime = 6;
		}

		public override void Clear()
		{
			base.Clear();

			EnsureDefaults();
		}

		public override void Reset()
		{
			base.Reset();

			EnsureDefaults();
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

            int version = writer.SetVersion(0);

            writer.Write(NumberofPortals);
            writer.Write(PortalCloseTime);
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

		    int version = reader.ReadInt();

		    NumberofPortals = reader.ReadInt();
		    PortalCloseTime = reader.ReadInt();
		}
	}
}