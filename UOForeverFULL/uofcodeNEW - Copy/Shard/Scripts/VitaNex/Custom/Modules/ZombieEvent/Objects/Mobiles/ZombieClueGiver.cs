#region References

using System;
using Server.Engines.ZombieEvent;
using Server.Items;
using Server.Network;
using Server.Targeting;
using VitaNex.SuperGumps.UI;

#endregion

namespace Server.Mobiles
{
    [CorpseName("a corpse")]
    public class ZombieClueGiver : BaseCreature
    {
        [CommandProperty(AccessLevel.GameMaster)]
        public int StringNumber { get; set; }

        [Constructable]
        public ZombieClueGiver()
            : base(AIType.AI_Melee, FightMode.Aggressor, 10, 1, 0.25, 0.8)
        {
            SpeechHue = Utility.RandomDyedHue();
            Name = NameList.RandomName( "male" );
            HairItemID = 0;
            FacialHairItemID = 0;
            SpecialTitle = "The Sage";
            TitleHue = 1272;

            Hue = Utility.RandomSkinHue();
            if (Utility.RandomBool())
            {
                Body = 0x191;
                Female = true;
            }
            else
            {
                Body = 0x190;
            }

            Utility.AssignRandomHair(this, Utility.RandomHairHue());

            VirtualArmor = 0;
            BardImmuneCustom = true;
            WeaponDamage = true;
            TakesNormalDamage = true;
            Pseu_AllowInterrupts = true;

            Pseu_SpellDelay = TimeSpan.Zero;
            ClearHandsOnCast = true;
            PowerWords = true;

            Fame = 0;
            Karma = 0;

            Blessed = true;
            CantWalk = true;

            Pseu_SpellBookRequired = true;
            Pseu_ConsumeReagents = true;

            MonkRobe chest = new MonkRobe();
            chest.Hue = 1153;
            chest.Identified = true;
            AddItem(Immovable(chest));
        }

        public ZombieClueGiver(Serial serial)
            : base(serial)
        {}

        public override void OnDoubleClick(Mobile @from)
        {
            if (from is ZombieAvatar)
            {
                var avatar = from as ZombieAvatar;
                switch (StringNumber)
                {
                    case 0:
                    {
                        string title = "The Cure [Part 1]";
                        string html = "  The first step in obtaining the cure is to have the right items to work with! You will need a mortar and pestle, a knife and an empty bowl. Those are all the tools you really need to succeed... but make sure to follow the recipe precisely, or else it could blow up in your face!";
                        new ZombieClueGiverUI(avatar.Owner, avatar, title, html).Send();
                    }
                        break;
                    case 1:
                        {
                            string title = "The Cure [Part 2]";
                            string html = "  A chunk of undying flesh is required in the mixture-a sort of inoculation, as it were... be sure to chop it up with a blade so that you can easily grind it in the mortar! If you are fortunate, you may find some such flesh on the corpse of one of the poor infected souls.";
                            new ZombieClueGiverUI(avatar.Owner, avatar, title, html).Send();
                        }
                        break;
                    case 2:
                        {
                            string title = "The Cure [Part 3]";
                            string html = "  The next ingredient you must obtain are vile tentacles. To obtain these, you must slay a horrifying tentacle beast. These too must be cut up with a knife and then ground in a mortar.";
                            new ZombieClueGiverUI(avatar.Owner, avatar, title, html).Send();
                        }
                        break;
                    case 3:
                        {
                            string title = "The Cure [Part 4]";
                            string html = "  Another ingredient required in order to create the cure is as vial of vitriol. Mix this in with the mortar and pestle! You can sometimes find enough vitriol from the abominations roaming this land.";
                            new ZombieClueGiverUI(avatar.Owner, avatar, title, html).Send();
                        }
                        break;
                    case 4:
                        {
                            string title = "The Cure [Part 5]";
                            string html = "  The magic found in fey wings will be required in order to bring life back to the undead. The wings must be ground up in the mortar in order to release their power. The fey creatures from which these wings are plucked are fearsome, so be careful!";
                            new ZombieClueGiverUI(avatar.Owner, avatar, title, html).Send();
                        }
                        break;
                    case 5:
                        {
                            string title = "The Cure [Part 6]";
                            string html = "  Place the mortar with the four ingredients in to a fire and cook it for several minutes. Make sure to have all four ground up ingredients (and nothing else) before cooking!";
                            new ZombieClueGiverUI(avatar.Owner, avatar, title, html).Send();
                        }
                        break;
                    case 6:
                        {
                            string title = "The Cure [Part 7]";
                            string html = "  Next you must pour the alchemical concoction into an empty pewter bowl.  Once the concoction has settled, you can begin adding further ingredients to the bowl.";
                            new ZombieClueGiverUI(avatar.Owner, avatar, title, html).Send();
                        }
                        break;
                    case 7:
                        {
                            string title = "The Cure [Part 8]";
                            string html = "  Slay one of the many daemons wandering these cursed lands and free him of one of his claws!  Place this claw into the pewter bowl.";
                            new ZombieClueGiverUI(avatar.Owner, avatar, title, html).Send();
                        }
                        break;
                    case 8:
                        {
                            string title = "The Cure [Part 9]";
                            string html = "  Next you must retrieve a seed of renewal from the blighted Treefellows wandering about. Once obtained, place it within the pewter bowl.";
                            new ZombieClueGiverUI(avatar.Owner, avatar, title, html).Send();
                        }
                        break;
                    case 9:
                        {
                            string title = "The Cure [Part 10]";
                            string html = "  The last ingredient of the cure is a spider's carapace. This must be placed into the pewter bowl with the rest of the ingredients. The spiders that infest the abandoned keep will probably be your best source. Be careful with the carapace!  It is rumored that these zombified spiders keep their undead young within.";
                            new ZombieClueGiverUI(avatar.Owner, avatar, title, html).Send();
                        }
                        break;
                    case 10:
                        {
                            string title = "The Cure [Part 11]";
                            string html = "  Lastly, you must obtain a crystal flask from one of the darknight creepers roaming the land.  In life, they were gifted alchemists. However, their toying with zombie curse has transformed them into the horrific beasts they are today.  Once you have obtained a crystal vial, pour all of your ingredients within.  Beware, if any wrong steps were taken in the creation of the cure, the resulting mixture could be very dangerous..";
                            new ZombieClueGiverUI(avatar.Owner, avatar, title, html).Send();
                        }
                        break;
                }
            }
            base.OnDoubleClick(@from);
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int) 0); // version 

            writer.Write(StringNumber);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            StringNumber = reader.ReadInt();

        }
    }
}