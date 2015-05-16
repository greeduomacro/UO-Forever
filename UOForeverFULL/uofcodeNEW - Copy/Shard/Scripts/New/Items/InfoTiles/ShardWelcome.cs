using System;

namespace Server.Items
{
	public class ShardWelcomeTile : ShardInfoTile
	{
		private static readonly string m_DefaultName = String.Format( "Welcome to {0}!", ShardInfo.DisplayName );
		public override string DefaultName{ get{ return m_DefaultName; } }

		[Constructable]
		public ShardWelcomeTile() : base()
		{
			Message = String.Format( "<CENTER><BASEFONT SIZE=7 COLOR=#111111 >{0}<BR><BR><BASEFONT COLOR=#333388 >You will find these blue tiles scattered around Britannia. These tiles give useful information about the game. For more information about the shard, click <A HREF=\"http://www.casiopia.net\">here</A>. </BASEFONT></BASEFONT></CENTER>", m_DefaultName );
		}

		public ShardWelcomeTile( Serial serial ) : base( serial )
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