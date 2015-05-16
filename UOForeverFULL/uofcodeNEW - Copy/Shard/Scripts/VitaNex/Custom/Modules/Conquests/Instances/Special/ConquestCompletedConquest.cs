#region References
using System;
using System.Linq;
using Server.Misc;

#endregion

namespace Server.Engines.Conquests
{
    public sealed class ConquestCompletedContainer : ConquestStateContainer
    {
        public ConquestCompletedContainer(ConquestState s)
            : base(s)
        { }
    }

    public class ConquestCompletedConquest : RecursiveConquest<ConquestCompletedContainer>
	{
		public override string DefCategory { get { return "Conquests/Completion"; } }

		public override int DefProgressMax { get { return base.DefProgressMax * 10; } }

		public ConquestCompletedConquest()
		{ }

		public ConquestCompletedConquest(GenericReader reader)
			: base(reader)
		{ }

        protected override int GetProgress(ConquestState state, ConquestCompletedContainer args)
		{
		    if (state.User == null)
		        return 0;
			int progress = base.GetProgress(state, args);

			if (progress <= 0)
			{
				return progress;
			}

			int count = args.State.Completed ? 1 : 0;

			ConquestProfile p = Conquests.EnsureProfile(state.User);

			if (p != null)
			{
				count += p.Count(s => s != state && Include(s));
			}

			return Math.Max(0, (count - state.Progress)-1);
		}

		protected override bool Include(ConquestState s)
		{
			return base.Include(s) && s.Completed;
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			writer.SetVersion(0);
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			reader.GetVersion();
		}
	}
}