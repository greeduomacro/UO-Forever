using System;
using Server.Mobiles;
using Server.Targeting;
using VitaNex;

namespace Server.Scripts.Items.Deeds
{
	public class EtherealCharcoal : Target // Create our targeting class (which we derive from the base target class)
	{
        private readonly EtherealCharcoalDeed _mDeed;

        public EtherealCharcoal(EtherealCharcoalDeed deed)
            : base(1, false, TargetFlags.None)
		{
			_mDeed = deed;

		}

		protected override void OnTarget( Mobile from, object target ) // Override the protected OnTarget() for our feature
		{
			if ( _mDeed.Deleted || _mDeed.RootParent != from )
				return;

            Item eth = target as Item;
            if (eth.TypeEquals(_mDeed.EtherealType, false) && eth is EtherealMount)
            {
                var mount = eth as EtherealMount;
		        if (!mount.IsDehued)
		        {
		            mount.IsCharcoal = true;
                    _mDeed.Delete();
                    from.SendMessage(54, "You have successfully dyed your ethereal mount.");
		        }
		        else
		        {
                    from.SendMessage(54, "This mount is already been dyed or is dehued.");	            
		        }
			}
            else if (!(eth is EtherealMount))
                from.SendMessage(54, "You must use this on an ethereal mount.");
            else if (!eth.TypeEquals(_mDeed.EtherealType, false))
                from.SendMessage(54, "This type of deed cannot be used on this ethereal type.");	
		}
	}

	public class EtherealCharcoalDeed : Item // Create the item class which is derived from the base item class
	{
        [CommandProperty(AccessLevel.GameMaster)]
        public ItemTypeSelectProperty EtherealType { get; set; }

		[Constructable]
		public EtherealCharcoalDeed() : base( 0x14F0 )
		{
            Name = "an ethereal charcoal dyeing deed";
			Weight = 1.0;
            LootType = LootType.Blessed;
		    EtherealType = typeof(EtherealHorse);
		    Hue = 1175;
		}

        public EtherealCharcoalDeed(Serial serial)
            : base(serial)
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( 0 ); // version

            writer.WriteType(EtherealType);
		}

		public override void Deserialize( GenericReader reader )
		{
		    base.Deserialize( reader );
			LootType = LootType.Blessed;

		    reader.ReadInt();

            EtherealType = reader.ReadType();
		}

        public override void OnSingleClick(Mobile from)
        {
            string ethtype = string.Empty;

            if (EtherealType == typeof(EtherealHorse))
            {
                ethtype = "Horse";
            }
            else if (EtherealType == typeof(EtherealBeetle))
            {
                ethtype = "Beetle";
            }
            else if (EtherealType == typeof(EtherealBoura))
            {
                ethtype = "Boura";
            }
            else if (EtherealType == typeof(EtherealCuSidhe))
            {
                ethtype = "Cu Sidhe";
            }
            else if (EtherealType == typeof(EtherealForestOstard))
            {
                ethtype = "Forest Ostard";
            }
            else if (EtherealType == typeof(EtherealFrenziedOstard))
            {
                ethtype = "Frenzied Ostard";
            }
            else if (EtherealType == typeof(EtherealHiryu))
            {
                ethtype = "Hiryu";
            }
            else if (EtherealType == typeof(EtherealKirin))
            {
                ethtype = "Kirin";
            }
            else if (EtherealType == typeof(EtherealLlama))
            {
                ethtype = "Llama";
            }
            else if (EtherealType == typeof(EtherealLongManeHorse))
            {
                ethtype = "Longmane Horse";
            }
            else if (EtherealType == typeof(EtherealOstard))
            {
                ethtype = "Ostard";
            }
            else if (EtherealType == typeof(EtherealPolarBear))
            {
                ethtype = "Polar Bear";
            }
            else if (EtherealType == typeof(EtherealReptalon))
            {
                ethtype = "Reptalon";
            }
            else if (EtherealType == typeof(EtherealRidgeback))
            {
                ethtype = "Ridgeback";
            }
            else if (EtherealType == typeof(EtherealSkeletalSteed))
            {
                ethtype = "Skeletal Steed";
            }
            else if (EtherealType == typeof(EtherealSwampDragon))
            {
                ethtype = "Swamp Dragon";
            }
            else if (EtherealType == typeof(EtherealUnicorn))
            {
                ethtype = "Unicorn";
            }

            LabelTo(from, "Dyeable Type: " + ethtype, 54);

            base.OnSingleClick(from);
        }

	    public override bool DisplayLootType{ get{ return false; } }

		public override void OnDoubleClick( Mobile from ) // Override double click of the deed to call our target
		{
			if ( !IsChildOf( from.Backpack ) ) // Make sure its in their pack
				 from.SendLocalizedMessage( 1042001 ); // That must be in your pack for you to use it.
			else
			{
				from.SendMessage( "Which ethereal do you wish to dye charcoal?" );
                from.Target = new EtherealCharcoal(this); // Call our target
			}
		}
	}
}