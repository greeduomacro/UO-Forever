/* Author Giric*/
using System;
using System.Collections;
using Server;
using Server.Items;
using Server.Network;
using Server.Mobiles;
using Server.Accounting;
using Server.Gumps;
namespace Server.Items
{ 
	public class StartGoldDeed : Item 
	{ 
		


		[Constructable]
		public StartGoldDeed() : this( 1 )
		{
			ItemID = 5360;
			Movable = true;
			Hue = 52;
			Name = " A Deed for a 10k BankCheck";
					
		}

        public override void OnDoubleClick(Mobile from)
        {
            Account acct = (Account)from.Account;

            if (acct == null)
            {
                return;
            }

            bool StartGold = Convert.ToBoolean(acct.GetTag("StartGold"));

            if (StartGold) //account tag check

                {
                    this.SendLocalizedMessageTo(from, 1042971, "You have Already Received your Starting Gold!");
                    from.PlaySound(1066); //giggle
                }

            else
            {
                from.AddToBackpack(new BankCheck(10000));
                acct.SetTag("StartGold", "true");
                this.Delete();
            }
        }
		[Constructable]
		public StartGoldDeed( int amount ) 
        {
		}
		
		

		public StartGoldDeed( Serial serial ) : base( serial ) 
		{ 
		} 

		public override void Serialize( GenericWriter writer ) 
		{ 
		base.Serialize( writer ); 

		writer.Write( (int) 0 ); // version 
		} 

		public override void Deserialize( GenericReader reader ) 
		{ 
		base.Deserialize( reader ); 

		int version = reader.ReadInt(); 
		}

	} 
} 
