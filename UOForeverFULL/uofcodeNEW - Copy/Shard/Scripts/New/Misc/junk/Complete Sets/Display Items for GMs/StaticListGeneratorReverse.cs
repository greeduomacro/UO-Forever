using System;
using System.Collections;
using Server;
using Server.Items;
using Server.Misc;
using Server.Mobiles;

namespace Server.Items
{
	public class StaticListGeneratorReverse : Item
	{
		[Constructable]
		public StaticListGeneratorReverse() :  base( 0x1870 )
		{
			Hue = 0x0;
			Name = "Static List Generator Reverse";
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
				for ( int x = 99; x >= 0; --x )
				{
					for ( int y = 163; y >= 0; --y )
					{
						RewardCake cake = new RewardCake();
						cake.ItemID = (16400 - ((100 * y) + x) );
						cake.Name = "ItemID = " + Convert.ToString(cake.ItemID);
						cake.Hue = 0;
						cake.Movable = false;
						if (cake != null) cake.MoveToWorld( new Point3D( (MX + (2* x)), (MY + (2 * y) + 340), MZ ), map );
					}
				}
			}
		}

		public StaticListGeneratorReverse( Serial serial ) : base( serial ) { }
		public override void Serialize( GenericWriter writer ) { base.Serialize( writer ); writer.Write( (int) 0 ); }
		public override void Deserialize( GenericReader reader ) { base.Deserialize( reader ); int version = reader.ReadInt(); }
	}
}