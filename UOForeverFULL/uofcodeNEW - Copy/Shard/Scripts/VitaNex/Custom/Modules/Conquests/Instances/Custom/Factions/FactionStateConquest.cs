#region References

using System;
using Server.Factions;

#endregion



namespace Server.Engines.Conquests
{
    public class FactionStateConquest : Conquest
    {
        public override string DefCategory { get { return "Factions"; } }

        public virtual string DefFaction { get { return null; } }

        [CommandProperty(Conquests.Access)]
        public virtual String Faction { get; set; }

        [CommandProperty(Conquests.Access)]
        public virtual bool IsFactionQuit { get; set; }

        public FactionStateConquest()
        {}

        public FactionStateConquest(GenericReader reader)
            : base(reader)
        {}

        public override void EnsureDefaults()
        {
            base.EnsureDefaults();

            Faction = DefFaction;
            IsFactionQuit = false;
        }

        public override sealed int GetProgress(ConquestState state, object args)
        {
            return GetProgress(state, args as Faction);
        }

        protected virtual int GetProgress(ConquestState state, Faction faction)
        {
            if (state.User == null)
                return 0;

            if (faction == null && !IsFactionQuit)
            {
                return 0;
            }

            if (faction != null && !string.IsNullOrEmpty(Faction) && faction.Definition.FriendlyName != Faction)
            {
                return 0;
            }

            return 1;
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            int version = writer.SetVersion(0);

            switch (version)
            {
                case 0:
                {
                    writer.Write(IsFactionQuit);
                    writer.Write(Faction);
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
                    IsFactionQuit = reader.ReadBool();
                    Faction = reader.ReadString();
                }
                    break;
            }
        }
    }
}