#region References
using System;

using Server.Factions;
using Server.Mobiles;
using Server.Mobiles.MetaPet;
using Server.Network;
using Server.Spells;
using Server.Spells.Fifth;
using Server.Spells.Seventh;
using VitaNex.Targets;

#endregion

namespace Server.Items
{
    public class WandofLoyalty : MagicWand
	{
        private BaseCreature Pet { get; set; }

        private int m_CommandCharges;

        private int m_LoyaltyCharges;

        [CommandProperty(AccessLevel.GameMaster)]
        public int CommandCharges
        {
            get { return m_CommandCharges; }
            set { m_CommandCharges = value; InvalidateProperties(); }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public int LoyaltyCharges
        {
            get { return m_LoyaltyCharges; }
            set { m_LoyaltyCharges = value; InvalidateProperties(); }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool Activated { get; set; }

        [Constructable]
        public WandofLoyalty()
            : this(10, 10)
        {
        }

		[Constructable]
        public WandofLoyalty(int commandcharges, int loyaltycharges)
		{
			Name = "Wand of Command";
		    Hue = 1161;
			Weight = 2.0;
			Stackable = false;
            m_CommandCharges = commandcharges;
            LootType = LootType.Blessed;
		}

        public WandofLoyalty(Serial serial)
			: base(serial)
		{ }

	    public override void OnDoubleClick(Mobile from)
	    {
	        if (RootParentEntity == from && from is PlayerMobile)
	        {
                if (Pet == null || Pet.Deleted)
	            {
                    from.SendMessage(54, "Target the pet you would like to bind to this wand.");
	                GetPetTarget(from);
	            }
                else if (LoyaltyCharges > 0)
                {
                    from.SendMessage(54, "Target your pet to refill its loyalty.");
                    GetLoyaltyTarget(from);
                }
                else
                {
                    from.SendMessage(61, "Your wand has run out of loyalty charges.");
                }
	        }
	        else
	        {
                from.SendLocalizedMessage(1042001); // That must be in your pack for you to use it.
	        }
	    }

        public override void OnSingleClick(Mobile @from)
        {
            base.OnSingleClick(@from);
            PrivateOverheadMessage(MessageType.Label, 0, true , "Command Charges: " + CommandCharges + ", Loyalty Charges: " + LoyaltyCharges, from.NetState);
        }

        public override void OnAdded(object parent)
        {
            if (parent is PlayerMobile)
            {
                var pm = parent as PlayerMobile;

                if (CommandCharges > 0)
                {
                    Activated = true;
                    CommandCharges--;
                    pm.SendMessage(61, "The wand of command has granted you the ability to exhort greater control over your followers.");
                }
                else
                {
                    pm.SendMessage(61, "Whatever magic powered this wand has seemed to disappear.");
                }
            }
            base.OnAdded(parent);
        }

        public override void OnRemoved(object parent)
        {
            Activated = false;
            base.OnRemoved(parent);
        }

        public void GetPetTarget(Mobile user)
        {
            if (user != null && !user.Deleted)
            {
                user.Target = new MobileSelectTarget<Mobile>((m, t) => GetPet(m, t), m => { });
            }
        }

        public void GetPet(Mobile User, Mobile target)
        {
            var pet = target as BaseCreature;

            if (pet != null && pet.Controlled && pet.ControlMaster == User)
            {
                Pet = pet;
                Hue = Pet.Hue;
                User.SendMessage(54, "You have successfully bound your pet to this wand of command.");
            }
            else
            {
                User.SendMessage(54, "This can only be used on creatures that you own!");
            }
        }

        public void GetLoyaltyTarget(Mobile user)
        {
            if (user != null && !user.Deleted)
            {
                user.Target = new MobileSelectTarget<Mobile>((m, t) => GetPetLoyalty(m, t), m => { });
            }
        }

        public void GetPetLoyalty(Mobile User, Mobile target)
        {
            var pet = target as BaseCreature;

            if (Pet == target)
            {
                if (Pet.Loyalty < 100)
                {
                    Pet.Loyalty = 100;
                    User.SendMessage(61, "You have maxed out your pets loyalty.");
                    LoyaltyCharges--;
                }
                else
                {
                    User.SendMessage(61, "Your pet is already as loyal as it could possibly be.");
                }
            }
            else
            {
                User.SendMessage(54, "This can only be used on the pet this wand is bound to!");
            }
        }

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

            int version = writer.SetVersion(0);

            switch (version)
            {
                case 0:
                {
                    writer.WriteMobile(Pet);
                    writer.Write((int)m_CommandCharges);
                    writer.Write((int)m_LoyaltyCharges);
                    writer.Write(Activated);
                }
                    break;
            }
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

            int version = reader.GetVersion();


            switch (version)
            {
                case 0:
                {
                    Pet = reader.ReadMobile<BaseCreature>();
                    m_CommandCharges = reader.ReadInt();
                    m_LoyaltyCharges = reader.ReadInt();
                    Activated = reader.ReadBool();
                }
                    break;
            }
		}
	}
}