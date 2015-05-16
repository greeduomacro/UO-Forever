using System;
using Server.Items;
using Server.Network;

namespace Server.Items
{
    public class FollowerCapToken : Item
    {
        public static int MaxFollowers = 7;

        [Constructable]
        public FollowerCapToken()
            : base(0x2AAA)
        {
            Name = "a follower increase token";
            LootType = LootType.Blessed;
            Light = LightType.Circle300;
            Weight = 5.0;
        }

        public FollowerCapToken(Serial serial)
            : base(serial)
        {
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);

            list.Add(1070998, "extra follower slot"); // Use this to redeem<br>your ~1_PROMO~
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (!IsChildOf(from.Backpack))
            {
                from.SendLocalizedMessage(1062334); // This item must be in your backpack to be used.
            }
            else
            {
                if (from.FollowersMax < MaxFollowers)
                {
                    from.FollowersMax += 1;
                    from.SendMessage("Your max followers slots have been increased by 1.");
                    this.Delete();
                }
                else
                {
                    from.SendMessage("You cannot increase your max followers any further.");
                }
            }
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }
}