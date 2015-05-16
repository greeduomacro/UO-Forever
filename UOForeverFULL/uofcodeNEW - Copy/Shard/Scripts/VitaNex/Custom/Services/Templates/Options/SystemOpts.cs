#region References
using VitaNex;
#endregion

namespace Server.PvPTemplates
{
	public class PvPTemplatesOptions : CoreServiceOptions
	{
		public PvPTemplatesOptions()
			: base(typeof(PvPTemplates))
		{ }

		public PvPTemplatesOptions(GenericReader reader)
			: base(reader)
		{ }

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			int version = writer.SetVersion(0);

			switch (version)
			{
				case 0:
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
					break;
			}
		}
	}
}