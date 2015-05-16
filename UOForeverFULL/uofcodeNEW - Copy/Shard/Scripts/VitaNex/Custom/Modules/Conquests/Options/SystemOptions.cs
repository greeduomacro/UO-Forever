#region References
using VitaNex;
using VitaNex.MySQL;
#endregion

namespace Server.Engines.Conquests
{
	public sealed class ConquestsOptions : CoreModuleOptions
	{
		[CommandProperty(Conquests.Access)]
		public bool UseCategories { get; set; }

        [CommandProperty(Conquests.Access)]
        public bool StaffWorldFirsts { get; set; }

        [CommandProperty(Conquests.Access)]
        public bool SupressWorldFirsts { get; set; }

		[CommandProperty(Conquests.Access)]
		public bool MySQLEnabled { get; set; }

		[CommandProperty(Conquests.Access)]
		public MySQLConnectionInfo MySQLInfo { get; set; }

		public ConquestsOptions()
			: base(typeof(Conquests))
		{
			EnsureDefaults();
		}

		public ConquestsOptions(GenericReader reader)
			: base(reader)
		{ }

		public void EnsureDefaults()
		{
			UseCategories = true;

		    StaffWorldFirsts = false;

		    SupressWorldFirsts = false;

			MySQLEnabled = true;
			MySQLInfo = MySQLConnectionInfo.Default;
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

			int version = writer.SetVersion(3);

			switch (version)
			{
                case 3:
                    {
                        writer.Write(SupressWorldFirsts);
                    }
                    goto case 2;
                case 2:
                    {
                       writer.Write(StaffWorldFirsts);
                    }
                    goto case 1;
				case 1:
					{
						writer.Write(MySQLEnabled);
						MySQLInfo.Serialize(writer);
					}
					goto case 0;
				case 0:
					writer.Write(UseCategories);
					break;
			}
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			int version = reader.GetVersion();

			switch (version)
			{
                case 3:
                    {
                        SupressWorldFirsts = reader.ReadBool();
                    }
                    goto case 2;
                case 2:
			        {
			            StaffWorldFirsts = reader.ReadBool();
			        }
                    goto case 1;
				case 1:
					{
						MySQLEnabled = reader.ReadBool();
						MySQLInfo = new MySQLConnectionInfo(reader);
					}
					goto case 0;
				case 0:
					{
						if (version < 1)
						{
							MySQLEnabled = true;
							MySQLInfo = MySQLConnectionInfo.Default;
						}

						UseCategories = reader.ReadBool();
					}
					break;
			}
		}
	}
}