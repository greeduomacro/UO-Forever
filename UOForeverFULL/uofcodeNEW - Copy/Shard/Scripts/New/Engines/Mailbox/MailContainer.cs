using System;
using Server.Items;

namespace Server.Engines.MailboxSystem
{
	//This container holds the attachments for a peice of mail.
	public class MailContainer : Container
	{
		public override int MaxWeight{ get{ return 0; } } //Unlimited

		private IMailMessage m_AttachedTo;
		public IMailMessage AttachedTo{ get{ return m_AttachedTo; } set{ m_AttachedTo = value; } }

		[CommandProperty( AccessLevel.Lead )]
		public bool Attached{ get{ return m_AttachedTo != null; } } //Used to find/delete orphaned containers.

		//Not constructable!
		public MailContainer( IMailMessage attached ) : base( 0xE75 )
		{
			Movable = false; //Do not decay!
			MaxItems = 0; //Unlimited
			m_AttachedTo = attached;
		}

		public MailContainer( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.WriteEncodedInt( (int)0 ); //version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadEncodedInt();
		}
	}
}