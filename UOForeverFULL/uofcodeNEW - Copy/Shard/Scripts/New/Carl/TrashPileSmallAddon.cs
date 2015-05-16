
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
	public class TrashPileSmallAddonAddon : BaseAddon
	{
        private static int[,] m_AddOnSimpleComponents = new int[,] {
			  {7073, 0, 1, 0}, {7077, 0, 0, 0}, {7089, 1, 1, 0}// 1	2	3	
			, {7088, 1, 0, 0}// 4	
		};

 
            
		public override BaseAddonDeed Deed
		{
			get
			{
				return new TrashPileSmallAddonAddonDeed();
			}
		}

		[ Constructable ]
		public TrashPileSmallAddonAddon()
		{

            for (int i = 0; i < m_AddOnSimpleComponents.Length / 4; i++)
                AddComponent( new AddonComponent( m_AddOnSimpleComponents[i,0] ), m_AddOnSimpleComponents[i,1], m_AddOnSimpleComponents[i,2], m_AddOnSimpleComponents[i,3] );


		}

		public TrashPileSmallAddonAddon( Serial serial ) : base( serial )
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

	public class TrashPileSmallAddonAddonDeed : BaseAddonDeed
	{
		public override BaseAddon Addon
		{
			get
			{
				return new TrashPileSmallAddonAddon();
			}
		}

		[Constructable]
		public TrashPileSmallAddonAddonDeed()
		{
			Name = "small trash pile deed";
		}

		public TrashPileSmallAddonAddonDeed( Serial serial ) : base( serial )
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