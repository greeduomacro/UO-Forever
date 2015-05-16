/*
 * Created by SharpDevelop.
 * User: Sharon
 * Date: 12/4/2007
 * Time: 7:21 AM
 * http://www.shazzyshard.org
 * Santa Claus Quest 2007
 */
 
using System;
using Server;

namespace Server.Items
{
	public class HolidayBobbles : SilverEarrings
	{

		public override int ArtifactRarity{ get{ return 25; } }

		[Constructable]
		public HolidayBobbles()
		{
			Name ="Holiday Bobbles";
			Hue = 2351;		
		}

		public HolidayBobbles( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 );
		}
		
		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}
	}
}
