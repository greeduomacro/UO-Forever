using System;
using Server;
using Server.Network;
using Server.Mobiles;
using Server.Gumps;

namespace Server.Items
{
	public class GenderChangeDeed : Item
	{
		[Constructable]
		public GenderChangeDeed() : base( 0x14F0 )
		{
			Name = "a gender change deed";
			Hue = 1158;
            LootType = LootType.Blessed;
		}

		public GenderChangeDeed( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.WriteEncodedInt( (int) 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadEncodedInt();
		}

		public override void OnDoubleClick( Mobile from )
		{
			if ( IsChildOf( from.Backpack ) )
				from.SendGump( new GenderChangeConfirmGump( from, this ) );
			else
				from.SendLocalizedMessage( 1042001 ); // That must be in your pack for you to use it.
		}

		protected void Use( Mobile from )
		{
			from.SendMessage( "You feel your body proportions change." );

			from.BodyValue = (from.BodyValue == 400 ? 401 : 400 );
			from.Female = !from.Female;

			/*if ( from.Female )
			{
				from.FacialHairItemID = 0;
				from.SendMessage( "You feel a sharp burning pain as your beard disintegrates." );
			}*/

			Delete();
		}

		private class GenderChangeConfirmGump : Gump
		{
			Mobile m_Mobile;
			GenderChangeDeed m_Deed;

			public GenderChangeConfirmGump( Mobile from, GenderChangeDeed deed ) : base( 200, 200 )
			{
				m_Deed = deed;
				m_Mobile = from;

				this.Resizable=false;
				this.AddPage(0);
				this.AddBackground(0, 0, 300, 120, 9270);
				//this.AddAlphaRegion(11, 14, 332, 95);
				//this.AddItem(297, 38, 4168);
				this.AddLabel( 28, 15, 255, "Changing gender requires intensely concentrated magic." );
				if ( !m_Mobile.Female )
					this.AddLabel( 28, 31, 255, "Your facial hair, if any, will dissapear." );
				this.AddLabel( 28, 47, 255, "Are you sure you want to proceed?" );

				AddButton( 28, 70, 4005, 4007, 1, GumpButtonType.Reply, 0 );
				AddLabel( 80, 70, 255, "Yes" );

				AddButton( 130, 70, 4005, 4007, 0, GumpButtonType.Reply, 0 );
				AddLabel( 182, 70, 255, "No" );
			}

			public override void OnResponse( NetState state, RelayInfo info )
			{
				if ( info.ButtonID == 1 )
				{
					if ( m_Deed != null && !m_Deed.Deleted && m_Deed.IsChildOf( m_Mobile.Backpack ) )
						m_Deed.Use( m_Mobile );
					else
						m_Mobile.SendLocalizedMessage( 1042001 ); // That must be in your pack for you to use it.
				}
			}
		}
	}
}