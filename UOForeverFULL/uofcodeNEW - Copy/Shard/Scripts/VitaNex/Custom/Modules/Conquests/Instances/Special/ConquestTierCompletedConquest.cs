#region References
using System;
using System.Linq;
#endregion

namespace Server.Engines.Conquests
{
    public sealed class ConquestTierCompletedContainer : ConquestStateContainer
    {
        public int Tier { get; set; }

        public ConquestTierCompletedContainer(ConquestState s, int tier)
            : base(s)
        {
            Tier = tier;
        }
    }

    public class ConquestTierCompletedConquest : RecursiveConquest<ConquestTierCompletedContainer>
	{
		public override string DefCategory { get { return "Conquests/Tiers"; } }

		public override int DefProgressMax { get { return base.DefProgressMax * 10; } }

		public virtual int DefTierReq { get { return 1; } }

		[CommandProperty(Conquests.Access)]
		public int TierReq { get; set; }

		public ConquestTierCompletedConquest()
		{ }

		public ConquestTierCompletedConquest(GenericReader reader)
			: base(reader)
		{ }

		public override void EnsureDefaults()
		{
			base.EnsureDefaults();

			TierReq = DefTierReq;
		}

        protected override int GetProgress(ConquestState state, ConquestTierCompletedContainer args)
		{

            if (state.User == null)
                return 0;

			int progress = base.GetProgress(state, args);

			if (progress <= 0)
			{
				return progress;
			}

			int count = args.Tier >= TierReq ? 1 : 0;

			ConquestProfile p = Conquests.EnsureProfile(state.User);

			if (p != null)
			{
				count += p.Count(s => s != state && Include(s));
			}
			
			return Math.Max(0, count - state.Progress);
		}

		protected override bool Include(ConquestState s)
		{
			return base.Include(s) && s.Tier >= TierReq;
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			int version = writer.SetVersion(1);

			switch (version)
			{
				case 1:
					writer.Write(TierReq);
					goto case 0;
				case 0:
					break;
			}
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			int version = reader.GetVersion();

			switch (version)
			{
				case 1:
					TierReq = reader.ReadInt();
					goto case 0;
				case 0:
					break;
			}

			if (version < 1)
			{
				TierReq = DefTierReq;
			}
		}
	}
}