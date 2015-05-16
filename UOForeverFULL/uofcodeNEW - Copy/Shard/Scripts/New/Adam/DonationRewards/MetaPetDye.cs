using System;
using Server.Mobiles;
using Server.Mobiles.MetaPet;
using Server.Multis;
using Server.Engines.VeteranRewards;
using Server.Targeting;

namespace Server.Items
{
	public class MetaPetDye : Item
	{
        [Constructable]
        public MetaPetDye()
            : base(6195)
        {
            Name = "Meta Pet Dye";
            LootType = LootType.Blessed;
        }

		[Constructable]
		public MetaPetDye( int hue )
            : base(6195)
		{
            Name = "Meta Pet Dye";
		    Hue = hue;
			LootType = LootType.Blessed;
		}

		public override void OnDoubleClick( Mobile from )
		{
		    if (RootParent == from)
		    {
		        from.Target = new InternalTarget(this);
		    }

		    base.OnDoubleClick( from );
		}

        public override void OnSingleClick(Mobile from)
        {
            base.OnSingleClick(from);

            LabelTo(from, "Hue: " + Hue, Hue);
        }

        public MetaPetDye(Serial serial)
            : base(serial)
		{
		}

        private class InternalTarget : Target
        {
            private readonly MetaPetDye _Dyes;

            public InternalTarget(MetaPetDye dyes)
                : base(1, false, TargetFlags.None)
            {
                _Dyes = dyes;
            }

            protected override void OnTarget(Mobile from, object targeted)
            {
                if (targeted is BaseMetaPet)
                {
                    var mobile = targeted as BaseCreature;

                    if (mobile.ControlMaster == from)
                    {
                        mobile.Hue = _Dyes.Hue;
                        _Dyes.Delete();
                        from.SendMessage(54, "You have applied the dye to your meta pet.");
                    }
                    else if (mobile.Hue == _Dyes.Hue)
                    {
                        from.SendMessage(54, "Your meta pet is already this hue.");
                    }
                    else
                    {
                        from.SendMessage(54, "You must target a meta pet that you own.");
                    }
                }
                else
                {
                    from.SendMessage(54, "You must target a meta pet with this dye!");
                }
            }
        }

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 ); // version

		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();

		}
	}
}