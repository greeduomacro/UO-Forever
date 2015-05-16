#region References
using VitaNex;
#endregion

namespace Server.Engines.CustomTitles
{
	public sealed class TitlesOptions : CoreModuleOptions
	{
		[CommandProperty(CustomTitles.Access)]
		public int DefaultTitleHue { get; set; }

		public TitlesOptions()
			: base(typeof(CustomTitles))
		{
			EnsureDefaults();
		}

		public TitlesOptions(GenericReader reader)
			: base(reader)
		{ }

		public void EnsureDefaults()
		{
			DefaultTitleHue = 33;
		}

		public override void Reset()
		{
			base.Reset();

			EnsureDefaults();
		}

		public override void Clear()
		{
			base.Clear();

			EnsureDefaults();
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			int version = writer.SetVersion(0);

			switch (version)
			{
				case 0:
					writer.Write(DefaultTitleHue);
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
					DefaultTitleHue = reader.ReadInt();
					break;
			}
		}
	}
}