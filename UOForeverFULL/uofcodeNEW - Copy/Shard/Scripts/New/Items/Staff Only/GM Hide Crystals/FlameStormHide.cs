using System;
using Server.Network;
using Server.Targeting;
using Server.Mobiles;
using Server.Accounting;

namespace Server.Items
{
	public class FlamestormHide : BaseGMJewel
	{
        private int tempHue;
        private int tempBody;
        private bool tohide;
        private bool playing;
        public override bool CastHide{ get{ return false; } }

		public override void HideEffects(Mobile from)
		{
            if (from == null || from.Deleted)
                return;

			if (playing)
                from.SendMessage("You must wait for the animation to finish.");
            else if (from.Hidden)
            {
                playing = true;
                tohide = false;
                tempHue = from.Hue;
                tempBody = from.BodyValue;
                from.Hue = 0;
                from.BodyValue = 15;
                from.Hidden = false;
                from.Animate(12, 10, 0, true, false, 0);
                from.PlaySound(273);
                Timer.DelayCall<Mobile>(TimeSpan.FromSeconds(1.5), new TimerStateCallback<Mobile>(AnimStop_Callback), from);
            }
            else
            {
                playing = true;
                tohide = true;
                from.Animate(17, 4, 1, true, false, 0);
                Timer.DelayCall<Mobile>(TimeSpan.FromSeconds(0.75), new TimerStateCallback<Mobile>(CastStop_Callback), from);
            }

        }

        private void AnimStop_Callback(Mobile from)
        {
			from.Hue = tempHue;
			from.BodyValue = tempBody;
			if (tohide)
				from.Hidden = true;
			else
				from.Animate(17, 4, 1, false, false, 0);

			playing = false;
        }

        private void CastStop_Callback(Mobile from)
        {
            tempHue = from.Hue;
            tempBody = from.BodyValue;
            from.Hue = 0;
            from.BodyValue = 15;
            from.Animate(12, 8, 1, false, false, 0);
            from.PlaySound(274);
            Timer.DelayCall<Mobile>(TimeSpan.FromMilliseconds(1500.0), new TimerStateCallback<Mobile>(AnimStop_Callback), from);

        }

        [Constructable]
		public FlamestormHide() : base(AccessLevel.GameMaster, 0xCB, 0x1ECD )
		{
			Hue = 1160;
			Name = "GM Flamestorm Ball";
		}
		public FlamestormHide( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.WriteEncodedInt( (int) 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadEncodedInt();
		}
	}
}