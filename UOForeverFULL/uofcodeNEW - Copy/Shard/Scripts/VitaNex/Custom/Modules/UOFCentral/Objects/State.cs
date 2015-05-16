#region References
using Server;
#endregion

namespace VitaNex.Modules.UOFCentral
{
	public sealed class State : PropertyObject
	{
		[CommandProperty(UOFCentral.Access, true)]
		public RootPage Root { get; private set; }

		[CommandProperty(UOFCentral.Access)]
		public int X { get; set; }

		[CommandProperty(UOFCentral.Access)]
		public int Y { get; set; }

		public State()
		{
			EnsureDefaults();
		}

		public void EnsureDefaults()
		{
			Root = new RootPage();

			X = 100;
			Y = 200;
		}

		public override void Clear()
		{
			EnsureDefaults();
		}

		public override void Reset()
		{
			EnsureDefaults();
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			int version = writer.SetVersion(0);

			Root.Serialize(writer);

			switch (version)
			{
				case 0:
					{
						writer.Write(X);
						writer.Write(Y);
					}
					break;
			}
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			int version = reader.GetVersion();

			Root.Deserialize(reader);

			switch (version)
			{
				case 0:
					{
						X = reader.ReadInt();
						Y = reader.ReadInt();
					}
					break;
			}
		}
	}
}