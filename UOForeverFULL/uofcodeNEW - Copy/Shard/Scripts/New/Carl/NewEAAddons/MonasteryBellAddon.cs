

using System;
using Server;
using Server.Items;

namespace Server.Items
{
	public class MonasteryBellAddon : BaseAddon
	{
        private static int[,] m_AddOnSimpleComponents = new int[,] {
			  {19600, 0, 0, 0}, {19601, 1, 0, 0}, {19602, 1, 1, 0}// 1	2	3	
			, {19603, 0, 1, 0}// 4	
		};

 
            
		public override BaseAddonDeed Deed
		{
			get
			{
				return new MonasteryBellAddonDeed();
			}
		}

		[ Constructable ]
		public MonasteryBellAddon()
		{

            for (int i = 0; i < m_AddOnSimpleComponents.Length / 4; i++)
                AddComponent( new AddonComponent( m_AddOnSimpleComponents[i,0] ), m_AddOnSimpleComponents[i,1], m_AddOnSimpleComponents[i,2], m_AddOnSimpleComponents[i,3] );


		}

		public MonasteryBellAddon( Serial serial ) : base( serial )
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

	public class MonasteryBellAddonDeed : BaseAddonDeed
	{
		public override BaseAddon Addon
		{
			get
			{
				return new MonasteryBellAddon();
			}
		}

		[Constructable]
		public MonasteryBellAddonDeed()
		{
			Name = "MonasteryBell";
		}

		public MonasteryBellAddonDeed( Serial serial ) : base( serial )
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