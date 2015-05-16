

using System;
using Server;
using Server.Items;

namespace Server.Items
{
	public class fancystonetable1Addon : BaseAddon
	{
        private static int[,] m_AddOnSimpleComponents = new int[,] {
			  {19632, 1, 1, 0}, {19634, 0, 0, 0}, {19635, 1, 0, 0}// 1	2	3	
			, {19633, 0, 1, 0}// 4	
		};

 
            
		public override BaseAddonDeed Deed
		{
			get
			{
				return new fancystonetable1AddonDeed();
			}
		}

		[ Constructable ]
		public fancystonetable1Addon()
		{

            for (int i = 0; i < m_AddOnSimpleComponents.Length / 4; i++)
                AddComponent( new AddonComponent( m_AddOnSimpleComponents[i,0] ), m_AddOnSimpleComponents[i,1], m_AddOnSimpleComponents[i,2], m_AddOnSimpleComponents[i,3] );


		}

		public fancystonetable1Addon( Serial serial ) : base( serial )
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

	public class fancystonetable1AddonDeed : BaseAddonDeed
	{
		public override BaseAddon Addon
		{
			get
			{
				return new fancystonetable1Addon();
			}
		}

		[Constructable]
		public fancystonetable1AddonDeed()
		{
			Name = "fancystonetable1";
		}

		public fancystonetable1AddonDeed( Serial serial ) : base( serial )
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