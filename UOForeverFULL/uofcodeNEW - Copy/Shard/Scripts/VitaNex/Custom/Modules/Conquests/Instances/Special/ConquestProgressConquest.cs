#region References
using System;
using System.Linq;
#endregion

namespace Server.Engines.Conquests
{
    public sealed class ConquestProgressContainer : ConquestStateContainer
    {
        public int Offset { get; set; }

        public ConquestProgressContainer(ConquestState s, int offset)
            : base(s)
        {
            Offset = offset;
        }
    }

    public class ConquestProgressConquest : RecursiveConquest<ConquestProgressContainer>
	{
		public override string DefCategory { get { return "Conquests/Progress"; } }

		public override int DefProgressMax { get { return base.DefProgressMax * 100; } }

		public ConquestProgressConquest()
		{ }

		public ConquestProgressConquest(GenericReader reader)
			: base(reader)
		{ }

        protected override int GetProgress(ConquestState state, ConquestProgressContainer args)
		{
            if (state.User == null)
                return 0;

			int progress = base.GetProgress(state, args);

			if (progress <= 0)
			{
				return progress;
			}

			int total = args.State.Progress + args.Offset;

			ConquestProfile p = Conquests.EnsureProfile(state.User);

			if (p != null)
			{
				total += p.Sum(s => s != state && Include(s) ? s.Progress : 0);
			}

			return Math.Max(0, total - state.Progress);
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			int version = writer.SetVersion(0);

			switch (version)
			{
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
				case 0:
					break;
			}
		}
	}
}