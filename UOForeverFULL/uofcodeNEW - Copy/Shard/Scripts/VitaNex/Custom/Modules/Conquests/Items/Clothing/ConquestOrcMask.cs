using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Server.Items;
using Server.Misc;
using Server.Network;

namespace Server.Scripts.VitaNex.Custom.Modules.Conquests.Items
{
    public class ConquestOrcMask : ConquestBaseWerable
    {
        public static void Initialize()
        {
            EventSink.Speech += EventSink_Speechhandler;
        }

        [Constructable]
        public ConquestOrcMask()
            : base(0x141B, Layer.Helm)
        {
            Weight = 1.0;
            Name = "Oomie to Urk Translator";
            Hue = 2655;
        }

        public ConquestOrcMask(Serial serial)
            : base(serial)
        { }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.WriteEncodedInt(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadEncodedInt();
        }

        public static void EventSink_Speechhandler(SpeechEventArgs e)
        {
            if (e.Mobile.FindItemOnLayer(Layer.Helm) is ConquestOrcMask)
            {
                e.Blocked = true;
                int words = e.Speech.Split(' ').Length;
                string speech = InhumanSpeech.Orc.ConstructSentance(words);
                e.Mobile.PublicOverheadMessage(MessageType.Label, 33, true, speech);
            }
        }
    }
}
