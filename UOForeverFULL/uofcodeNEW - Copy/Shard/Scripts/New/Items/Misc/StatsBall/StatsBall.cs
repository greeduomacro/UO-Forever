/*
 * Copyright (c) 2005, Kai Sassmannshausen <kai@sassie.org>
 * All rights reserved.
 *
 * Redistribution and use in source and binary forms, with or without
 * modification, are permitted provided that the following conditions
 * are met:
 *
 * - Redistributions of source code must retain the above copyright
 * notice, this list of conditions and the following disclaimer.
 *
 * - Redistributions in binary form must reproduce the above copyright
 * notice, this list of conditions and the following disclaimer in the
 * documentation and/or other materials provided with the
 * distribution.
 *
 * - Neither the name of Kai Sassmannshausen, nor the names of its
 * contributors may be used to endorse or promote products derived from
 * this software without specific prior written permission.
 * THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND
 * CONTRIBUTORS "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING,
 * BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS
 * FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE
 * COPYRIGHT OWNER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT,
 * INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
 * (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR
 * SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION)
 * HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT,
 * STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE)
 * ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED
 * OF THE POSSIBILITY OF SUCH DAMAGE.
 *
 *  Stats Ball
 *  Version 0
 */

using System;
using System.Collections;
using System.Net;
using Server.Network;
using Server.Items;
using Server.Gumps;
using Server.Accounting;


namespace Server.Items
{
    public class StatsBall : Item
    {

        [Constructable]
        public StatsBall() : base(3699)
        {
            Movable = true;
            Hue = 1161;
            Name = "a Stats Ball";
            LootType = LootType.Blessed;
        }

        public StatsBall(Serial serial)
            : base(serial)
        {
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

        public virtual bool IsValidUse(Mobile from)
        {
            return true;
        }

        public override void OnDoubleClick(Mobile from)
        {

            if (from == null || from.Deleted || from.Backpack == null)
                return; /* pedantic check */

            if (!IsValidUse(from))
                return;

            if (!IsChildOf(from.Backpack) )
            {
                from.SendLocalizedMessage(1042001); // That must be in your pack for you to use it.
                return;
            }

            from.CloseGump(typeof(StatsBallGump));
            from.SendGump(new StatsBallGump(this, from));

        }
    }

    public class NewPlayerStatsBall : StatsBall
    {
        private Mobile m_NewPlayer;

        [Constructable]
        public NewPlayerStatsBall(Mobile newplayer)
        {
            Movable = true;
            Hue = 1163;
            Name = "a personal Stats Ball";
            LootType = LootType.Blessed;
            m_NewPlayer = newplayer;
        }

        public NewPlayerStatsBall(Serial serial) : base(serial)
        {
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public Mobile Owner
        {
            get { return m_NewPlayer; }
            // No reason for this
            //set { m_NewPlayer = value; }
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); // version
            writer.Write((Mobile)m_NewPlayer);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
            m_NewPlayer = reader.ReadMobile();

        }

        public override bool IsValidUse(Mobile from)
        {
            if (from != m_NewPlayer)
            {
                from.SendMessage("This SkillBall only workes on its original owner. Item deleted.");
                this.Delete();
                return false;
            }
            return true;
        }

	    public override void OnSingleClick(Mobile from)
	    {
		    if (Owner != null)
		    {
			    LabelToExpansion(from);
			    LabelTo(from, String.Format("personal statsball of {0}", Owner.Name));
		    }
		    else
			    base.OnSingleClick(from);
	    }
    }
}