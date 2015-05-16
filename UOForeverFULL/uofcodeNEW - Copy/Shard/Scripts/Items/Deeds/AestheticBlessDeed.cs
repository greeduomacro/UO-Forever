using Server.Items;
using Server.Network;
using Server.Targeting;

namespace Server.Scripts.Items.Deeds
{
	public class AestheticBlessTarget : Target // Create our targeting class (which we derive from the base target class)
	{
		private readonly AestheticBlessDeed _mDeed;

		public AestheticBlessTarget( AestheticBlessDeed deed ) : base( 1, false, TargetFlags.None )
		{
			_mDeed = deed;
		}

		protected override void OnTarget( Mobile from, object target ) // Override the protected OnTarget() for our feature
		{
			if ( _mDeed.Deleted || _mDeed.RootParent != from )
				return;

		    var armor = target as BaseArmor;
		    var weapon = target as BaseWeapon;
		    if ( armor != null || weapon != null)
			{
			    if (armor != null)
			    {
                    if (armor.IsAesthetic)
			            from.SendMessage(61, "This item is already an aesthetic item.");
                    else if (armor.RootParent != from)
			            from.SendMessage(61, "You must have this item on your person to turn it into an aesthetic item.");
			        else
			        {
                        armor.IsAesthetic = true;
			            from.SendMessage(61, "You turn the item into an aesthetic item.");

			            _mDeed.Delete();
			        }
			    }
                else
			    {
                    if (weapon.IsAesthetic)
                        from.SendMessage(61, "This item is already an aesthetic item.");
                    else if (weapon.RootParent != from)
                        from.SendMessage(61, "You must have this item on your person to turn it into an aesthetic item.");
                    else
                    {
                        weapon.IsAesthetic = true;
                        from.SendMessage(61, "You turn the item into an aesthetic item.");

                        _mDeed.Delete();
                    }
			    }
			}
			else
				from.SendMessage(61, "Aesthetic deeds are only usable on weapons and armor.");
		}
	}

	public class AestheticBlessDeed : Item
	{
		public override string DefaultName
		{
			get { return "an aesthetic item deed"; }
		}

		[Constructable]
		public AestheticBlessDeed() : base( 0x14F0 )
		{
			Weight = 1.0;
		    Hue = 61;
		}

		public AestheticBlessDeed( Serial serial ) : base( serial )
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
			//LootType = LootType.Blessed;

		    reader.ReadInt();
		}

	    public override bool DisplayLootType{ get{ return false; } }

		public override void OnDoubleClick( Mobile from ) // Override double click of the deed to call our target
		{
			if ( !IsChildOf( from.Backpack ) ) // Make sure its in their pack
				 from.SendLocalizedMessage( 1042001 ); // That must be in your pack for you to use it.
			else
			{
				from.SendMessage( "Which piece of armor or weapon would you like to make aesthetic?" );
				from.Target = new AestheticBlessTarget( this ); // Call our target
			}
		}
	}
}