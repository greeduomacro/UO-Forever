#region References

using System;
using System.Collections.Generic;
using System.Linq;
using Server.Engines.CustomTitles;
using Server.Items;
using Server.Network;

#endregion

namespace Server.Mobiles
{
    [CorpseName("Llewellyn's corpse")]
    public class RoyalArchivist : BaseCreature
    {
        [CommandProperty(AccessLevel.GameMaster)]
        public int NotesAquired { get; set; }

        private Dictionary<PlayerMobile, int> NotesGiven;

        private static readonly SkillName[] _Skills = ((SkillName) 0).GetValues<SkillName>();

        [Constructable]
        public RoyalArchivist()
            : base(AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = "Llewellyn";
            Body = 400;
            Hue = 33777;
            SpecialTitle = "Royal Archivist";
            TitleHue = 1926;

            Blessed = true;

            CantWalk = true;

            SpeechHue = YellHue = 34;

            foreach (SkillName skill in _Skills)
            {
                SetSkill(skill, 80.0, 110);
            }

            VirtualArmor = 36;

            NotesAquired = 0;

            NotesGiven = new Dictionary<PlayerMobile, int>();

            AddItem(new Robe(Utility.RandomYellowHue()));
            AddItem(new Sandals(Utility.RandomYellowHue()));

            var book = new Spellbook();
            book.Name = "Tome of Chaos";
            book.Hue = 1194;
            book.LootType = LootType.Newbied;
            AddItem(Immovable(book));

            HairItemID = 8265;
            FacialHairItemID = 8254;

            HairHue = 2577;
            FacialHairHue = 2577;
        }

        public override bool HandlesOnSpeech(Mobile from)
        {
            if (InRange(from, 5))
            {
                return true;
            }
            return base.HandlesOnSpeech(from);
        }

        public override void OnSpeech(SpeechEventArgs e)
        {
            Mobile m = e.Mobile;

            if (!e.Handled && m.InRange(Location, 5))
            {
                string speech = e.Speech.Trim().ToLower();


                if ((speech.IndexOf("help") > -1 || speech.IndexOf("amount") > -1 || speech.IndexOf("need") > -1 ||
                     speech.IndexOf("quest") > -1) && NotesAquired < 500)
                {
                    e.Handled = true;

                    Say(
                        "I need a reference to decipher the Tome of Chaos. Rumors have been floating about that agents of Lord Blackthorn have been seen occupying the dungeons of Britannia.");
                    Say(
                        "I would start looking there. Bring me anything that looks like it might help decipher this infernal tome.");
                }
                else if (speech.IndexOf("scroll") > -1 && NotesAquired < 500)
                {
                    e.Handled = true;

                    Say(
                        "If you have a chaos scroll, hand it over here already so we can get this forsaken book deciphered!");
                }
                else if (NotesAquired == 500)
                {
                    Say(
                        "The Tome of Chaos has already been deciphered. The words of power to open the Marble Keep's gates are Ex Por Rel Uus.");
                }
            }

            base.OnSpeech(e);
        }

        public override bool OnDragDrop(Mobile from, Item dropped)
        {
            var giver = from as PlayerMobile;
            if (giver == null)
            {
                return false;
            }
            if (dropped is ChaosScroll && NotesAquired < 499)
            {
                if (NotesAquired < 100)
                {
                    Say("I still need many more chaos scrolls to decipher the Tome of Chaos.");
                }
                else if (NotesAquired <= 299 && NotesAquired >= 101)
                {
                    Say(
                        "The Tome of Chaos is starting to make a bit of sense, I still need more chaos scrolls to decipher the finer details.");
                }
                else if (NotesAquired < 499 && NotesAquired >= 301)
                {
                    Say(
                        "Hmm..I think I understand. It is a variation on Ex Por, the unlock spell. I think a few more chaos scrolls will provide what I need to decipher the spell.");
                }

                if (NotesGiven != null)
                {
                    if (NotesGiven.ContainsKey(giver))
                    {
                        NotesGiven[giver]++;
                    }
                    else
                    {
                        NotesGiven.Add(giver, 1);
                    }
                }
                NotesAquired++;
                dropped.Delete();
            }
            else if (dropped is ChaosScroll && NotesAquired == 499)
            {
                NetState.Instances.Where(ns => ns != null && ns.Mobile is PlayerMobile)
                    .ForEach(
                        ns =>
                            ns.Mobile.SendNotification(
                                "The Tome of Chaos has been deciphered! Speak to Llewellyn to learn the key words to open the marble keep's gates.",
                                true, 1.0, 10.0));
                if (NotesGiven != null)
                {
                    if (NotesGiven.ContainsKey(giver))
                    {
                        NotesGiven[giver]++;
                    }
                    else
                    {
                        NotesGiven.Add(giver, 1);
                    }
                }
                NotesAquired++;
                Say("That's it! The key words to open the marble keep's gates are Ex Por Rel Uus!");
                Say("Here, take this scroll for your contribution towards deciphering the Tome of Chaos.");
                if (NotesGiven != null)
                {
                    foreach (KeyValuePair<PlayerMobile, int> kvp in NotesGiven)
                    {
                        kvp.Key.BankBox.DropItem(new CrippledKingScroll(kvp.Key.Name, kvp.Value));
                        kvp.Key.SendMessage(
                            "A scroll commemorating your contribution to deciphering the Tome of Chaos has been added to your bank.");
                    }
                    Dictionary<PlayerMobile, int> topThree = (from entry in NotesGiven
                        orderby entry.Value descending
                        select entry)
                        .Take(3)
                        .ToDictionary(pair => pair.Key, pair => pair.Value);
                    foreach (KeyValuePair<PlayerMobile, int> kvp in topThree)
                    {
                        kvp.Key.BankBox.DropItem(new TitleScroll("The Archivist"));
                        kvp.Key.SendMessage(
                            "You were in the top 3 for Chaos Scroll contributions! The title scroll: The Archivist has been placed in your bankbox.");
                    }
                }
            }
            else if (dropped is ChaosScroll && NotesAquired == 500)
            {
                Say("Why don't you keep that? The tome has already been deciphered.");
                return false;
            }

            return true;
        }

        public RoyalArchivist(Serial serial)
            : base(serial)
        {}

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int) 0);

            writer.Write(NotesAquired);
            if (NotesGiven == null)
            {
                writer.Write(0);
            }
            else
            {
                writer.Write(NotesGiven.Count);

                foreach (KeyValuePair<PlayerMobile, int> kvp in NotesGiven)
                {
                    writer.Write(kvp.Key);
                    writer.Write(kvp.Value);
                }
            }
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
            NotesAquired = reader.ReadInt();
            int notesCount = reader.ReadInt();

            if (notesCount > 0)
            {
                NotesGiven = new Dictionary<PlayerMobile, int>();

                for (int i = 0; i < notesCount; i++)
                {
                    var p = reader.ReadMobile<PlayerMobile>();
                    int num = reader.ReadInt();
                    NotesGiven.Add(p, num);
                }
            }
        }
    }
}