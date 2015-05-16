using Server.Items;

namespace Server.Mobiles
{
	public class Chronos : BaseAspect
	{
		public override Aspects Aspects { get { return Aspects.Time | Aspects.Illusion; } }

		[Constructable]
		public Chronos()
			: base(AIType.AI_Mage, FightMode.Weakest, 16, 1, 0.1, 0.2)
		{
			Name = "Chronos";
		}

		public Chronos(Serial serial)
			: base(serial)
		{ }

		protected override int InitBody()
		{
			return 830;
		}

		public override int GetIdleSound()
		{
			return 1495;
		}

		public override int GetAngerSound()
		{
			return 1492;
		}

		public override int GetHurtSound()
		{
			return 1494;
		}

		public override int GetDeathSound()
		{
			return 1493;
		}

		/*
		public override int GetAttackSound()
		{
			return 0;
		}
		*/

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			writer.Write(0);
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			reader.ReadInt();
		}
	}
}