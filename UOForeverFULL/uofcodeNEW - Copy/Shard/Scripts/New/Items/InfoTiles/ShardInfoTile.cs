using System;

namespace Server.Items
{
	public class ShardInfoTile : InfoTile
	{
		public override string DefaultName{ get{ return "Shard Information!"; } }

		[CommandProperty( AccessLevel.GameMaster, AccessLevel.Developer )]
		public override string Message{ get{ return GetMessage( base.Message ); } set{ base.Message = value; } }

		private static string GetMessage( string message )
		{
			return String.Format( "<BODY><BASEFONT COLOR=#333388 >{0} </BASEFONT></BODY>", message );
		}

		[Constructable]
		public ShardInfoTile() : base()
		{
			Visible = true;
			Active = true;
			Range = 1;
		}

		public ShardInfoTile( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.WriteEncodedInt( (int)0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadEncodedInt();
		}
	}
}