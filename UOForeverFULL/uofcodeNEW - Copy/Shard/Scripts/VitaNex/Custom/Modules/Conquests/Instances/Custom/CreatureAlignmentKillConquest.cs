#region References
using System;
using Server.Mobiles;
using VitaNex;
#endregion

namespace Server.Engines.Conquests
{
    public class CreatureAlignmentKillConquest : CreatureKillConquest
    {
        public Alignment DefAlignment { get { return Alignment.None; } }

        [CommandProperty(Conquests.Access)]
        public Alignment Alignment { get; set; }

        public CreatureAlignmentKillConquest()
        {}

        public CreatureAlignmentKillConquest(GenericReader reader)
            : base(reader)
        {}

        public override void EnsureDefaults()
        {
            base.EnsureDefaults();

            Alignment = DefAlignment;
        }

        protected override int GetProgress(ConquestState state, CreatureConquestContainer args)
        {
            if (state.User == null)
                return 0;

            if (args == null || args.Creature == null || args.Killer != null && args.Killer is PlayerMobile && args.Killer.Account != state.User.Account || !(args.Creature is BaseCreature))
            {
                return 0;
            }

            var creature = args.Creature as BaseCreature;

            if (Alignment != Alignment.None && creature.Alignment != Alignment)
            {
                return 0;
            }

            return 1;
            //return base.GetProgress(state, args);
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            int version = writer.SetVersion(0);

            switch (version)
            {
                case 0:
                {
                    writer.Write((int) Alignment);
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
                    Alignment = (Alignment) reader.ReadInt();
                }
                    break;
            }
        }
    }
}