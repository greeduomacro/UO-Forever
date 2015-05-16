#region References

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using Server.Engines.Craft;
using Server.Engines.LegendaryCrafting;
using Server.Items;
using Server.Network;
using VitaNex.SuperGumps.UI;

#endregion

namespace Server.Mobiles
{
    [CorpseName("Llewellyn's corpse")]
    public class RoyalArchivistSF : BaseCreature
    {
        private Dictionary<PlayerMobile, bool> TranscriptionBooks;

        [Constructable]
        public RoyalArchivistSF()
            : base(AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = "Llewellyn";
            Body = 400;
            Hue = 33777;
            SpecialTitle = "Royal Archivist";
            TitleHue = 1926;

            Blessed = true;

            CantWalk = true;

            SpeechHue = YellHue = 2049;

            VirtualArmor = 36;

            TranscriptionBooks = new Dictionary<PlayerMobile, bool>();

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

        public override void OnDoubleClick(Mobile from)
        {
            QuestDialogue(from);
            base.OnDoubleClick(from);
        }

        public override void OnSpeech(SpeechEventArgs e)
        {
            Mobile m = e.Mobile;

            if (!e.Handled && m.InRange(Location, 5))
            {
                e.Handled = true;
                QuestDialogue(e.Mobile);
            }

            base.OnSpeech(e);
        }

        public void QuestIntro_Callback(Tuple<int, Mobile> obj)
        {
            int sequence = obj.Item1;
            Mobile m = obj.Item2;
            switch (sequence)
            {
                case 2:
                {
                    PrivateOverheadMessage(MessageType.Label, 2049, true,
                        "Hmmmmm.....yes....ah, that's it!",
                        m.NetState);
                    Timer.DelayCall(TimeSpan.FromSeconds(10), QuestIntro_Callback, Tuple.Create(3, m));
                    break;
                }
                case 3:
                {
                    PrivateOverheadMessage(MessageType.Label, 2049, true,
                        "Alright, I have left detailed notes in your transcriptions.  This notepad should help you unlock the secrets of the Soulforge!",
                        m.NetState);
                    break;
                }
            }
        }

        public void QuestDialogue(Mobile from)
        {
            if (from is PlayerMobile && from.Skills[SkillName.Blacksmith].Value >= 120.0 &&
                from.Skills[SkillName.Tinkering].Value >= 120)
            {
                var hammer = from.Backpack.FindItemByType<LegendaryHammer>(true,
                    i => i != null && !i.Deleted && i.Quantity == 10);

                var notepad = from.Backpack.FindItemByType<TranscriptionBook>(true,
                    i => i != null && !i.Deleted && i.ScrollsFound.Count == 5);

                if (!TranscriptionBooks.ContainsKey(from as PlayerMobile) && hammer == null)
                {
                    PrivateOverheadMessage(MessageType.Label, 2049, true,
                        "You have a smithing hammer you think may be related to the Soulforge?  I'd love to see it!  Bring it to me and I may be able to tell you more about it.",
                        from.NetState);
                }
                else if (notepad != null && notepad.ScrollsFound.Count == 5 && TranscriptionBooks.ContainsKey(from as PlayerMobile) &&
                         !TranscriptionBooks[from as PlayerMobile])
                {
                    TranscriptionBooks[from as PlayerMobile] = true;
                    PrivateOverheadMessage(MessageType.Label, 2049, true,
                        "Ah, so you have transcribed all the scrolls, yes?  Let me see your notepad.  Ah yes.., let me make some further notes for you.",
                       from.NetState);
                    notepad.Completed = true;
                    notepad.Name = "a fully transcribed notepad";
                    Timer.DelayCall(TimeSpan.FromSeconds(6), QuestIntro_Callback, Tuple.Create(2, from));
                }
                else if (hammer != null &&
                         !TranscriptionBooks.ContainsKey(from as PlayerMobile))
                {
                    var html = new StringBuilder();

                    html.AppendLine(
                        "Let me see the smithing hammer...hmmm. I have seen these runes on my travels. What I can tell you is that they relate to the legend of the "
                            .WrapUOHtmlColor(Color.DarkSeaGreen));
                    html.AppendLine("Soulforge".WrapUOHtmlColor(Color.Orange) + ".\n");
                    html.AppendLine(
                        "You knew this already though, didn't you? You probably thought ol' Llewellyn could translate it for ya? Well..I can't. Not without some reference material anyways.\n"
                            .WrapUOHtmlColor(Color.DarkSeaGreen));
                    html.AppendLine(
                        "Like I said, I've seen these runes before. Here, take this notepad. Travel to the dungeons of Sosaria and transcribe at least"
                            .WrapUOHtmlColor(Color.DarkSeaGreen) +
                        " 5 different reference materials".WrapUOHtmlColor(Color.Red));
                    html.AppendLine(
                        "for me. The reference materials look like old, ".WrapUOHtmlColor(Color.DarkSeaGreen) +
                        "tattered scrolls".WrapUOHtmlColor(Color.Red) +
                        ". Find these and then, maybe, I may just be able to help you.".WrapUOHtmlColor(
                            Color.DarkSeaGreen));
                    var UI =
                        new SFQuestGiver(from as PlayerMobile, null, null, null, "Quest Offer",
                            html.ToString(),
                            onAccept: x =>
                            {
                                TranscriptionBooks.Add(from as PlayerMobile, false);
                                from.SendMessage("A notepad has been added to your backpack.");
                                from.Backpack.DropItem(new TranscriptionBook());
                            }).
                            Send<SFQuestGiver>();
                }
                else if (notepad != null && notepad.ScrollsFound.Count < 5 && TranscriptionBooks.ContainsKey(from as PlayerMobile) &&
                         !TranscriptionBooks[from as PlayerMobile])
                {
                    PrivateOverheadMessage(MessageType.Label, 2049, true,
                        "I can't help you without some reference material!  Please, go fill the book I gave you and then we can take it from there.",
                        from.NetState);
                }
                else
                {
                    PrivateOverheadMessage(MessageType.Label, 2049, true,
                        "Hmmm? What do you want?  Can't you see I am busy here?!",
                        from.NetState);
                }
            }
            else
            {
                PrivateOverheadMessage(MessageType.Label, 2049, true,
                    "Hmmm? What do you want?  Can't you see I am busy here?!",
                    from.NetState);
            }            
        }

        public RoyalArchivistSF(Serial serial)
            : base(serial)
        {}

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int) 0);

            if (TranscriptionBooks == null)
            {
                writer.Write(0);
            }
            else
            {
                writer.Write(TranscriptionBooks.Count);

                foreach (KeyValuePair<PlayerMobile, bool> kvp in TranscriptionBooks)
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

            TranscriptionBooks = new Dictionary<PlayerMobile, bool>();

            int notesCount = reader.ReadInt();

            if (notesCount > 0)
            {
                for (int i = 0; i < notesCount; i++)
                {
                    var p = reader.ReadMobile<PlayerMobile>();
                    bool boo = reader.ReadBool();
                    TranscriptionBooks.Add(p, boo);
                }
            }
        }
    }
}