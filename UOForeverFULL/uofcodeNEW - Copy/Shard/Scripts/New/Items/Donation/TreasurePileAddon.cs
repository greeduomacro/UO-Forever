
////////////////////////////////////////
//                                     //
//     //
// Addon Generator  //
//          //
//             //
//                                     //
////////////////////////////////////////
using System;
using Server;
using Server.Items;

namespace Server.Items
{
	public class TreasurePileAddon : BaseAddon
	{
        private static int[,] m_AddOnSimpleComponents = new int[,] {
			  {7014, -1, -1, 0}, {7013, -1, 0, 0}, {7011, -1, 1, 0}// 1	2	3	
			, {7016, 0, -1, 0}, {7015, 0, 0, 0}, {7004, 0, 1, 0}// 4	5	6	
			, {7008, 0, 2, 0}, {7018, 1, -1, 0}, {7017, 1, 0, 0}// 7	8	9	
			, {7005, 1, 1, 0}, {7007, 1, 2, 0}, {7019, 2, -1, 0}// 10	11	12	
			, {7009, 2, 0, 0}, {7010, 2, 1, 0}// 13	14	
		};

 
            
		public override BaseAddonDeed Deed
		{
			get
			{
				return new TreasurePileAddonDeed();
			}
		}

		[ Constructable ]
		public TreasurePileAddon()
		{

            for (int i = 0; i < m_AddOnSimpleComponents.Length / 4; i++)
                AddComponent( new AddonComponent( m_AddOnSimpleComponents[i,0] ), m_AddOnSimpleComponents[i,1], m_AddOnSimpleComponents[i,2], m_AddOnSimpleComponents[i,3] );


		}

		public TreasurePileAddon( Serial serial ) : base( serial )
		{
		}


		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( 0 ); // Version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
		}
	}

	public class TreasurePileAddonDeed : BaseAddonDeed
	{
		public override BaseAddon Addon
		{
			get
			{
				return new TreasurePileAddon();
			}
		}

		[Constructable]
		public TreasurePileAddonDeed()
		{
			Name = "TreasurePile";
		}

		public TreasurePileAddonDeed( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( 0 ); // Version
		}

		public override void	Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
		}
	}
}