using Server.Items;
using Server.Mobiles;
using Server.Targeting;

namespace Server.Scripts.Items.Deeds
{
	public class EtherealDehue : Target // Create our targeting class (which we derive from the base target class)
	{
        private readonly EtherealDehueDeed _mDeed;

        public EtherealDehue(EtherealDehueDeed deed)
            : base(1, false, TargetFlags.None)
		{
			_mDeed = deed;

		}

		protected override void OnTarget( Mobile from, object target ) // Override the protected OnTarget() for our feature
		{
			if ( _mDeed.Deleted || _mDeed.RootParent != from )
				return;

            EtherealMount eth = target as EtherealMount;
		    if ( eth != null )
		    {
		        if (!eth.IsDehued)
		        {
		            eth.IsDehued = true;
                    _mDeed.Delete();
                    from.SendMessage(54, "You have successfully dehued your ethereal mount.");
		        }
		        else
		        {
                    from.SendMessage(54, "This mount is already dehued.");	            
		        }
			}
			else
                from.SendMessage(54, "You must use this on an ethereal mount.");	  
		}
	}

	public class EtherealDehueDeed : Item // Create the item class which is derived from the base item class
	{
		public override string DefaultName
		{
			get { return "an ethereal dehue deed"; }
		}

		[Constructable]
		public EtherealDehueDeed() : base( 0x14F0 )
		{
			Weight = 1.0;
            LootType = LootType.Blessed;
		}

        public EtherealDehueDeed(Serial serial)
            : base(serial)
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
		    base.Deserialize( reader );
			LootType = LootType.Blessed;

		    reader.ReadInt();
		}

	    public override bool DisplayLootType{ get{ return false; } }

		public override void OnDoubleClick( Mobile from ) // Override double click of the deed to call our target
		{
			if ( !IsChildOf( from.Backpack ) ) // Make sure its in their pack
				 from.SendLocalizedMessage( 1042001 ); // That must be in your pack for you to use it.
			else
			{
				from.SendMessage( "Which ethereal do you wish to remove the hue from?" );
                from.Target = new EtherealDehue(this); // Call our target
			}
		}
	}
}