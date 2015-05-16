using VitaNex;

namespace Server.Engines.DonationsTracker
{
	public sealed class DonationsTrackerOptions : CoreServiceOptions
	{
		
		public DonationsTrackerOptions()
			: base(typeof(DonationsTracker))
		{
			EnsureDefaults();
		}

        public DonationsTrackerOptions(GenericReader reader)
			: base(reader)
		{ }

		public void EnsureDefaults()
		{
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
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

		    int version = reader.ReadInt();
		}
	}
}