#region References

using System;
using System.Drawing;
using System.Text;
using Server.Items;
using Server.Misc;
using Server.Network;
using VitaNex.SuperGumps.UI;
using EDI = Server.Mobiles.EscortDestinationInfo;

#endregion

namespace Server.Mobiles
{
    public class WispNewPlayerQuest : BaseCreature
    {

        private Timer SpeechTimer;
        [Constructable]
        public WispNewPlayerQuest()
            : base(AIType.AI_Melee, FightMode.Aggressor, 22, 1, 0.2, 1.0)
        {
            Name = "a wisp";
            SetStr(90, 100);
            SetDex(90, 100);
            SetInt(15, 25);
            Blessed = true;
            CantWalk = true;

            Body = 165;
            Hue = 0x901;

            SpeechHue = 61;

            SpeechTimer = new InternalTimer(this);
            SpeechTimer.Start();
        }

        public override void OnDoubleClick(Mobile @from)
        {
            if (from is PlayerMobile)
            {
                var player = from as PlayerMobile;
                string title = "To Slay a Dragon";
                StringBuilder html = new StringBuilder();
                html.Append("Greetings traveller!\n\n");
                html.Append("  Welcome to Ultima Online Forever.");
                new WispQuestGiverUI(player, title, html.ToString()).Send();
            }
        }

        private class InternalTimer : Timer
        {
            private readonly WispNewPlayerQuest wisp;

            public InternalTimer(WispNewPlayerQuest owner)
                : base(TimeSpan.FromSeconds(0.05), TimeSpan.FromSeconds(30))
            {
                wisp = owner;
            }

            protected override void OnTick()
            {
                wisp.Say("Double click me if you would like to learn more about Ultima Online Forever and take part in an epic quest to slay a dragon!");
            }
        }

        public WispNewPlayerQuest(Serial serial)
            : base(serial)
        {}

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            SpeechTimer = new InternalTimer(this);
            SpeechTimer.Start();
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }
}