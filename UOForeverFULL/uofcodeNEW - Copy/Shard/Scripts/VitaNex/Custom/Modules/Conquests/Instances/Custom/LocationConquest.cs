#region References



#endregion

namespace Server.Engines.Conquests
{
    public class LocationConquest : Conquest
    {
        public override string DefCategory { get { return "Movement"; } }

        public virtual Map DefMap { get { return null; } }
        public virtual Point3D DefLocation { get { return Point3D.Zero; } }

        [CommandProperty(Conquests.Access)]
        public virtual Point3D Location { get; set; }

        [CommandProperty(Conquests.Access)]
        public Map Map { get; set; }

        public LocationConquest()
        {}

        public LocationConquest(GenericReader reader)
            : base(reader)
        {}

        public override void EnsureDefaults()
        {
            base.EnsureDefaults();

            Location = DefLocation;
            Map = DefMap;
        }

        public override sealed int GetProgress(ConquestState state, object args)
        {
            return GetProgress(state, args as MovementConquestContainer);
        }

        protected virtual int GetProgress(ConquestState state, MovementConquestContainer args)
        {
            if (args == null || args.Blocked)
            {
                return 0;
            }

            if (state.User == null)
                return 0;

            if (Map != null && Map != Map.Internal &&
                (args.Mobile.Map == null || args.Mobile.Map != Map ||
                 args.Mobile.Map == Map && args.Mobile.Location != Location))
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
                    writer.Write(Location);
                    writer.Write(Map);
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
                    Location = reader.ReadPoint3D();
                    Map = reader.ReadMap();
                }
                    break;
            }
        }
    }
}