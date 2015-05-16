#region References
using System;

using Server.Factions;
using Server.Mobiles;
using Server.Mobiles.MetaPet;
using Server.Spells;
using Server.Spells.Fifth;
using Server.Spells.Seventh;
using VitaNex.Targets;

#endregion

namespace Server.Items
{
	public class MetaStone : Item
	{
        private BaseMetaPet Pet { get; set; }

		[Constructable]
		public MetaStone()
			: base(6251)
		{
			Name = "Meta Stone";
		    Hue = 1161;
			Weight = 2.0;
			Stackable = false;
            LootType = LootType.Blessed;
		}

        public MetaStone(Serial serial)
			: base(serial)
		{ }

	    public override void OnDoubleClick(Mobile from)
	    {
	        if (IsChildOf(from.Backpack) && from is PlayerMobile)
	        {
                if (Pet == null)
	            {
                    from.SendMessage(54, "Target the Meta-pet you would like to bind to this stone.");
	                GetTargetDragon(from);
	            }
	            else
	            {
                    new MetaStoneUI(from as PlayerMobile, Pet, this).Send();
	            }
	        }
	        else
	        {
                from.SendLocalizedMessage(1042001); // That must be in your pack for you to use it.
	        }
	    }

        public void GetTargetRelic(Mobile user)
        {
            if (user != null && !user.Deleted)
            {
                user.Target = new ItemSelectTarget<Item>(ConsumeRelic, m => { });
            }
        }

        public void ConsumeRelic(Mobile m, Item item)
        {
            if (!item.IsChildOf(m.Backpack))
            {
                m.SendMessage(54, "The relic must be in your backpack for you to use it!");
                return;
            }
            if (item is BaseMetaRelic)
            {
                var relic = item as BaseMetaRelic;
                relic.GetMeta(m, Pet);
            }
            else
            {
                m.SendMessage(54, "You must use this on a relic!");
            }
        }

        public void GetTargetDragon(Mobile user)
        {
            if (user != null && !user.Deleted)
            {
                user.Target = new MobileSelectTarget<Mobile>((m, t) => GetMeta(m, t), m => { });
            }
        }

        public void GetMeta(Mobile User, Mobile target)
        {
            var metapet = target as BaseMetaPet;

            if (metapet != null && metapet.Controlled && metapet.ControlMaster == User)
            {
                Pet = metapet;
                Hue = Pet.Hue;
                User.SendMessage(54, "You have successfully bound this meta stone to your Meta-Pet.");
            }
            else
            {
                User.SendMessage(54, "This can only be used on a Meta-Pet that you own!");
            }
        }

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

            int version = writer.SetVersion(1);

            switch (version)
            {
                case 1:
                    writer.WriteMobile(Pet);
                    goto case 0;
                case 0:
                    break;
            }
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

            int version = reader.GetVersion();


            switch (version)
            {
                case 1:
                    Pet = reader.ReadMobile<BaseMetaPet>();
                    goto case 0;
                case 0:
                    break;
            }
		}
	}
}