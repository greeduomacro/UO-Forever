using System;
using System.Collections;
using Server;
using Server.Items;
using Server.Misc;
using Server.Mobiles;

namespace Server.Items
{
	public class HueListGenerator : Item
	{
		[Constructable]
		public HueListGenerator() :  this( 0xFAB ) {}

		[Constructable]
		public HueListGenerator(int itemid) :  base( itemid )
		{
			Hue = 0x0;
			ItemID = itemid;
			Name = "Hue List Generator";
			LootType = LootType.Blessed;
			Visible = false;
		}

		public override void OnDoubleClick( Mobile m )
		{
			Map map = m.Map;
			int MX = m.X;
			int MY = m.Y;
			int MZ = m.Z;

			if ( map != null )
			{
				for ( int x = 0; x <= 99; ++x )
				{
					for ( int y = 0; y <= 29; ++y )
					{
						SpecialHueListItem cake = new SpecialHueListItem( ((100 * y) + x), this.ItemID);
						cake.Name = "Hue = " + Convert.ToString(cake.Hue);
						cake.Movable = false;
						if (cake != null) cake.MoveToWorld( new Point3D( (MX + (2* x)), (MY + (2 * y)), MZ ), map );
					}
				}
			}
		}

		public HueListGenerator( Serial serial ) : base( serial ) { }
		public override void Serialize( GenericWriter writer ) { base.Serialize( writer ); writer.Write( (int) 0 ); }
		public override void Deserialize( GenericReader reader ) { base.Deserialize( reader ); int version = reader.ReadInt(); }
	}
}