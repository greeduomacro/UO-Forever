

using System;
using Server;
using Server.Items;

namespace Server.Items
{
	public class fancystonetable2Addon : BaseAddon
	{
        private static int[,] m_AddOnSimpleComponents = new int[,] {
			  {19637, 1, 0, 0}, {19638, 1, 1, 0}, {19639, 0, 1, 0}// 1	2	3	
			, {19636, 0, 0, 0}// 4	
		};

 
            
		public override BaseAddonDeed Deed
		{
			get
			{
				return new fancystonetable2AddonDeed();
			}
		}

		[ Constructable ]
		public fancystonetable2Addon()
		{

            for (int i = 0; i < m_AddOnSimpleComponents.Length / 4; i++)
                AddComponent( new AddonComponent( m_AddOnSimpleComponents[i,0] ), m_AddOnSimpleComponents[i,1], m_AddOnSimpleComponents[i,2], m_AddOnSimpleComponents[i,3] );


		}

		public fancystonetable2Addon( Serial serial ) : base( serial )
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

	public class fancystonetable2AddonDeed : BaseAddonDeed
	{
		public override BaseAddon Addon
		{
			get
			{
				return new fancystonetable2Addon();
			}
		}

		[Constructable]
		public fancystonetable2AddonDeed()
		{
			Name = "fancystonetable2";
		}

		public fancystonetable2AddonDeed( Serial serial ) : base( serial )
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