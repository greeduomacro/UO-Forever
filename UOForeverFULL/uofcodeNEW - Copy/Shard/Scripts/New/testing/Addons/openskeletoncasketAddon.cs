
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
	public class openskeletoncasketAddon : BaseAddon
	{
        private static int[,] m_AddOnSimpleComponents = new int[,] {
			  {7501, 1, 1, 0}, {7446, 0, 0, 0}, {7502, 0, 1, 0}// 1	2	3	
			, {7500, 2, 1, 0}, {7447, 1, 0, 0}, {7505, 2, 0, 0}// 4	5	6	
			, {7211, -1, 0, 0}// 7	
		};

 
            
		public override BaseAddonDeed Deed
		{
			get
			{
				return new openskeletoncasketAddonDeed();
			}
		}

		[ Constructable ]
		public openskeletoncasketAddon()
		{

            for (int i = 0; i < m_AddOnSimpleComponents.Length / 4; i++)
                AddComponent( new AddonComponent( m_AddOnSimpleComponents[i,0] ), m_AddOnSimpleComponents[i,1], m_AddOnSimpleComponents[i,2], m_AddOnSimpleComponents[i,3] );


		}

		public openskeletoncasketAddon( Serial serial ) : base( serial )
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

	public class openskeletoncasketAddonDeed : BaseAddonDeed
	{
		public override BaseAddon Addon
		{
			get
			{
				return new openskeletoncasketAddon();
			}
		}

		[Constructable]
		public openskeletoncasketAddonDeed()
		{
			Name = "openskeletoncasket";
		}

		public openskeletoncasketAddonDeed( Serial serial ) : base( serial )
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