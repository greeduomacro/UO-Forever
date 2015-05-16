using System;
using Server.Mobiles;
using Server.Multis;
using Server.Targeting;
using VitaNex;

namespace Server.Scripts.Items.Deeds
{
	public class BoatDyeTarget : Target // Create our targeting class (which we derive from the base target class)
	{
        private readonly BoatDyeDeed _mDeed;

        public BoatDyeTarget(BoatDyeDeed deed)
            : base(1, false, TargetFlags.None)
		{
			_mDeed = deed;

		}

		protected override void OnTarget( Mobile from, object target ) // Override the protected OnTarget() for our feature
		{
            if (_mDeed.Deleted || _mDeed.RootParent != from)
                return;
		    BaseBoat boat;
		    var boatcomponent = target as BoatComponent;
		    if (boatcomponent != null)
		        boat = boatcomponent.Boat;
		    else
		        boat = target as BaseBoat;

		    if (boat == null)
		    {
		        from.SendMessage(54, "This deed only works on boats!  Please target either the boats wheel or the tillerman to dye the boat.");
		        return;
		    }

		    if (boat.Owner != from)
		    {
                from.SendMessage(54, "Only the owner of this boat may dye it.");		        
		    }
		    else
		    {
		        foreach (var component in boat.BoatComponents)
		        {
		            component.Hue = _mDeed.DyeableHue;
		        }
		        boat.Hue = _mDeed.DyeableHue;
                from.SendMessage(54, "You have successfully dyed your boat.");	
		        _mDeed.Consume();
		    }
		}
	}

	public class BoatDyeDeed : Item
	{
        [CommandProperty(AccessLevel.GameMaster)]
        public string HueName { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public int DyeableHue { get; set; }

		[Constructable]
		public BoatDyeDeed() : base( 0x14F0 )
		{
            Name = "a boat dyeing deed";
			Weight = 1.0;
            LootType = LootType.Blessed;
		    Hue = 1175;
		}

        public BoatDyeDeed(Serial serial)
            : base(serial)
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( 0 ); // version
            writer.Write(HueName);
            writer.Write(DyeableHue);
		}

		public override void Deserialize( GenericReader reader )
		{
		    base.Deserialize( reader );

		    reader.ReadInt();

		    HueName = reader.ReadString();
		    DyeableHue = reader.ReadInt();
		}

        public override void OnSingleClick(Mobile from)
        {
            base.OnSingleClick(from);

            LabelTo(from, "Dyeable Hue: " + HueName, 54);
        }

	    public override bool DisplayLootType{ get{ return false; } }

		public override void OnDoubleClick( Mobile from ) // Override double click of the deed to call our target
		{
			if ( !IsChildOf( from.Backpack ) ) // Make sure its in their pack
				 from.SendLocalizedMessage( 1042001 ); // That must be in your pack for you to use it.
			else
			{
				from.SendMessage( "Which boat do you wish to dye?" );
                from.Target = new BoatDyeTarget(this); // Call our target
			}
		}
	}
}